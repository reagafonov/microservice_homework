using ONVIFMedia;
using ONVIFPTZ;

namespace Ovnif.Ports.Ptz;

/// <summary>
/// Вторичный порт: единовременный доступ к ONVIF Media и PTZ в рамках одной открытой SOAP-сессии.
/// </summary>
public interface IOnvifMediaPtzSoapSession : IAsyncDisposable
{
    /// <summary>Контракт PTZ для вызова команд (например <c>AbsoluteMove</c>).</summary>
    PTZ PtzPort { get; }

    /// <summary>Загружает список медиа-профилей с устройства.</summary>
    Task<IReadOnlyList<Profile>> GetMediaProfilesAsync(CancellationToken cancellationToken);
}
