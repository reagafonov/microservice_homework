using Core;
using Core.Interfaces;

namespace Services;

/// <summary>
/// Прикладной фасад: делегирует вызов реализации <see cref="IOnvif"/> (например <see cref="Ptz.Xm530OnvifPtzService"/>).
/// </summary>
/// <param name="onvif">Конкретная реализация управления PTZ.</param>
public class OnvifService(IOnvif onvif) : IOnvifRunService
{
    /// <inheritdoc />
    public async Task SetPositionAsync(string ip, string username, string password, Position position, CancellationToken token)
    {
        // Единственная строка сценария: вся логика внутри выбранной реализации IOnvif
        await onvif.SetPositionAsync(ip, username, password, position, token);
    }
}
