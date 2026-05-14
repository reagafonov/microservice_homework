using ONVIFPTZ;

namespace Ovnif.Ports.Ptz;

/// <summary>
/// Переносимая структура данных для операции ONVIF <c>AbsoluteMove</c> (между композитором и исполнителем).
/// </summary>
/// <param name="Position">Целевой вектор pan/tilt/zoom в терминах ONVIF PTZ.</param>
/// <param name="Speed">Скорость движения; может соответствовать профилю по умолчанию.</param>
public sealed record PtzAbsoluteMovePayload(PTZVector Position, PTZSpeed Speed);
