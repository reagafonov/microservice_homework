using Core;

namespace Ovnif.Ports.Ptz;

/// <summary>
/// Вторичный порт: создание уже открытой <see cref="IOnvifMediaPtzSoapSession"/> для последующих вызовов.
/// </summary>
public interface IOnvifMediaPtzSoapSessionFactory
{
    /// <summary>Создаёт сессию, выполняет открытие каналов и возвращает готовый объект.</summary>
    Task<IOnvifMediaPtzSoapSession> CreateOpenedSessionAsync(
        OnvifServiceEndpoints endpoints,
        OnvifCredentials credentials,
        CancellationToken cancellationToken);
}
