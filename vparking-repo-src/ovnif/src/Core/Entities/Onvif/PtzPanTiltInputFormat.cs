namespace Core;

/// <summary>
/// Как интерпретировать <see cref="Position.Pan"/> и <see cref="Position.Tilt"/> перед отправкой в ONVIF generic space.
/// </summary>
public enum PtzPanTiltInputFormat : byte
{
    /// <summary>Не задано — задайте явно, иначе композитор выбросит исключение.</summary>
    Unspecified = 0,

    /// <summary>Уже в диапазоне [-1, 1] для <c>PositionGenericSpace</c> (после clamp).</summary>
    Normalized = 1,

    /// <summary>Градусы; перевод в [-1, 1] через <see cref="Xm530PtzConstants"/>.</summary>
    Degrees = 2,
}
