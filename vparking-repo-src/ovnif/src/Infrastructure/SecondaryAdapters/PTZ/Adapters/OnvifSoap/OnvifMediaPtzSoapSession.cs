using System.ServiceModel;
using System.ServiceModel.Channels;
using Core;
using ONVIFMedia;
using ONVIFPTZ;
using Ovnif.Ports.Ptz;

namespace Ovnif.Adapters.OnvifSoap;

/// <summary>
/// Адаптер: пара WCF-клиентов <see cref="ONVIFMedia.MediaClient"/> и <see cref="ONVIFPTZ.PTZClient"/>,
/// общий жизненный цикл открытия и закрытия каналов.
/// </summary>
public sealed class OnvifMediaPtzSoapSession(
    Binding binding,
    OnvifServiceEndpoints endpoints,
    OnvifCredentials credentials) : IOnvifMediaPtzSoapSession
{
    // Клиенты создаются один раз; учётка применяется в фабричных методах ниже
    private readonly ONVIFMedia.MediaClient _media = CreateMediaClient(binding, endpoints, credentials);
    private readonly ONVIFPTZ.PTZClient _ptz = CreatePtzClient(binding, endpoints, credentials);

    private static ONVIFMedia.MediaClient CreateMediaClient(Binding b, OnvifServiceEndpoints e, OnvifCredentials c)
    {
        var client = new ONVIFMedia.MediaClient(b, new EndpointAddress(e.MediaServiceUrl));
        ApplyCredentials(client, c);
        return client;
    }

    private static ONVIFPTZ.PTZClient CreatePtzClient(Binding b, OnvifServiceEndpoints e, OnvifCredentials c)
    {
        var client = new ONVIFPTZ.PTZClient(b, new EndpointAddress(e.PtzServiceUrl));
        ApplyCredentials(client, c);
        return client;
    }

    internal async Task OpenAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        // Порядок: сначала Media (часто легче), затем PTZ
        await _media.OpenAsync().ConfigureAwait(false);
        await _ptz.OpenAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public ONVIFPTZ.PTZ PtzPort => _ptz;

    public async Task<IReadOnlyList<ONVIFMedia.Profile>> GetMediaProfilesAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        // Один вызов GetProfiles без фильтра — фильтрация снаружи
        var response = await _media.GetProfilesAsync().ConfigureAwait(false);
        var list = response.Profiles ?? Array.Empty<ONVIFMedia.Profile>();
        return list;
    }

    public ValueTask DisposeAsync()
    {
        // Синхронное закрытие обоих каналов в конце using
        CloseQuietly(_media);
        CloseQuietly(_ptz);
        return ValueTask.CompletedTask;
    }

    private static void ApplyCredentials<TChannel>(ClientBase<TChannel> client, OnvifCredentials cred)
        where TChannel : class
    {
        client.ClientCredentials.UserName.UserName = cred.Username;
        client.ClientCredentials.UserName.Password = cred.Password;
    }

    private static void CloseQuietly(ICommunicationObject client)
    {
        try
        {
            if (client.State == CommunicationState.Faulted)
            {
                client.Abort();
                return;
            }

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
