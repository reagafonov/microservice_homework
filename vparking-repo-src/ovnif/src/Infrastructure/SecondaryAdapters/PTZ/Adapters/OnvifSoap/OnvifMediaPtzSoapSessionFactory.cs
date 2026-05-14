using System.ServiceModel.Channels;
using Core;
using Ovnif.Ports.Ptz;

namespace Ovnif.Adapters.OnvifSoap;

/// <summary>Адаптер-фабрика: создаёт <see cref="OnvifMediaPtzSoapSession"/> и выполняет <c>OpenAsync</c> на клиентах.</summary>
public sealed class OnvifMediaPtzSoapSessionFactory(Binding binding) : IOnvifMediaPtzSoapSessionFactory
{
    public async Task<IOnvifMediaPtzSoapSession> CreateOpenedSessionAsync(
        OnvifServiceEndpoints endpoints,
        OnvifCredentials credentials,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(endpoints);
        ArgumentNullException.ThrowIfNull(credentials);
        cancellationToken.ThrowIfCancellationRequested();

        // Сессия ещё «холодная» — конструктор только задаёт endpoint и учётку
        var session = new OnvifMediaPtzSoapSession(binding, endpoints, credentials);
        // Открываем оба канала до возврата наружу
        await session.OpenAsync(cancellationToken).ConfigureAwait(false);
        return session;
    }
}
