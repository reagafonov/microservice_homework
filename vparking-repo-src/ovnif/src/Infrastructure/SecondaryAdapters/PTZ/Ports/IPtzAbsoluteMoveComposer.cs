using Core;
using ONVIFMedia;

namespace Ovnif.Ports.Ptz;

/// <summary>
/// Вторичный порт: преобразование доменной <see cref="Core.Position"/> и PTZ-конфигурации профиля в SOAP-полезную нагрузку.
/// </summary>
public interface IPtzAbsoluteMoveComposer
{
    /// <summary>Собирает вектор позиции и скорость для вызова <c>AbsoluteMove</c>.</summary>
    PtzAbsoluteMovePayload Compose(Position position, ONVIFMedia.PTZConfiguration ptzConfiguration);
}
