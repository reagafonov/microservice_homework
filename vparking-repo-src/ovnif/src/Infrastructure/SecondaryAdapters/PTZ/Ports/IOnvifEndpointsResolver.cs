using Core;

namespace Ovnif.Ports.Ptz;

/// <summary>
/// Вторичный порт (driven): определение SOAP-адресов сервисов Media и PTZ по ответу Device.GetCapabilities.
/// </summary>
public interface IOnvifEndpointsResolver
{
    /// <summary>
    /// Выполняет запрос capabilities к устройству и возвращает нормализованные URL для Media и PTZ.
    /// </summary>
    /// <param name="credentials">Хост и учётные данные.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<OnvifServiceEndpoints> ResolveAsync(OnvifCredentials credentials, CancellationToken cancellationToken);
}
