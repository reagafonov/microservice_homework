using System.ServiceModel;
using System.ServiceModel.Channels;
using Core;
using ONVIFDevice;
using Ovnif.Ports.Ptz;

namespace Ovnif.Adapters.OnvifSoap;

/// <summary>
/// Вторичный адаптер (infrastructure): WCF <see cref="ONVIFDevice.DeviceClient"/>, запрос
/// <c>GetCapabilities</c> и нормализация <see cref="OnvifServiceEndpoints"/>.
/// </summary>
public sealed class OnvifEndpointsResolver(Binding binding) : IOnvifEndpointsResolver
{
    public async Task<OnvifServiceEndpoints> ResolveAsync(OnvifCredentials credentials, CancellationToken cancellationToken)
    {
        // Защита от null и отмены до сетевых вызовов
        ArgumentNullException.ThrowIfNull(credentials);
        cancellationToken.ThrowIfCancellationRequested();

        // Пробуем корень и /onvif1 — разные прошивки вешают Device по-разному
        var caps = await DiscoverCapabilitiesAsync(credentials, cancellationToken).ConfigureAwait(false)
                   ?? throw new InvalidOperationException(
                       $"GetCapabilities failed for http://{credentials.Host}:8899 and .../onvif1.");

        // Fallback-база, если в capabilities нет полного URL
        var defaultBase = $"http://{credentials.Host}:8899";
        // Подменяем хост в XAddr на тот, с которым реально коннектимся (NAT/DNS)
        var media = NormalizeServiceUrl(caps.Media?.XAddr, defaultBase, credentials.Host);
        var ptz = NormalizeServiceUrl(caps.PTZ?.XAddr, defaultBase, credentials.Host);
        return new OnvifServiceEndpoints(media, ptz);
    }

    private async Task<ONVIFDevice.Capabilities?> DiscoverCapabilitiesAsync(
        OnvifCredentials credentials,
        CancellationToken cancellationToken)
    {
        // Два типичных пути Device-сервиса на порту 8899
        foreach (var path in new[] { "", "/onvif1" })
        {
            cancellationToken.ThrowIfCancellationRequested();
            // Новый клиент на каждую попытку, чтобы не тащить faulted state
            var device = new ONVIFDevice.DeviceClient(binding, new EndpointAddress($"http://{credentials.Host}:8899{path}"));
            ApplyCredentials(device, credentials);
            try
            {
                // Явное открытие канала перед GetCapabilities
                await device.OpenAsync().ConfigureAwait(false);
                // Запрашиваем всё сразу — дальше берём только Media и PTZ
                var resp = await device.GetCapabilitiesAsync(new[] { ONVIFDevice.CapabilityCategory.All }).ConfigureAwait(false);
                var caps = resp.Capabilities;
                // Закрываем Device-канал: дальше работают Media/PTZ клиенты
                CloseQuietly(device);
                if (caps != null)
                    return caps;
            }
            catch
            {
                // Неверный путь или 401 — обрываем канал и пробуем следующий URL
                device.Abort();
            }
        }

        return null;
    }

    private static void ApplyCredentials<TChannel>(ClientBase<TChannel> device, OnvifCredentials credentials)
        where TChannel : class
    {
        // WS-Security UserNameToken на клиенте WCF
        device.ClientCredentials.UserName.UserName = credentials.Username;
        device.ClientCredentials.UserName.Password = credentials.Password;
    }

    private static string NormalizeServiceUrl(string? xAddr, string fallback, string preferredHost)
    {
        // Нет XAddr в capabilities — остаёмся на fallback (корень :8899)
        if (string.IsNullOrWhiteSpace(xAddr))
            return fallback;
        // Битая строка URI — тоже fallback
        if (!Uri.TryCreate(xAddr, UriKind.Absolute, out var uri))
            return fallback;
        // Выравниваем host под переданный IP/DNS клиента
        var builder = new UriBuilder(uri) { Host = preferredHost };
        return builder.Uri.AbsoluteUri.TrimEnd('/');
    }

    private static void CloseQuietly(ICommunicationObject client)
    {
        try
        {
            // Faulted Close не вызываем — только Abort
            if (client.State == CommunicationState.Faulted)
            {
                client.Abort();
                return;
            }

            // Нормальное закрытие с таймаутом, чтобы не зависнуть
            if (client.State is CommunicationState.Opened or CommunicationState.Opening)
                client.Close(TimeSpan.FromSeconds(10));
        }
        catch (CommunicationException)
        {
            client.Abort();
        }
        catch (TimeoutException)
        {
            client.Abort();
        }
    }
}
