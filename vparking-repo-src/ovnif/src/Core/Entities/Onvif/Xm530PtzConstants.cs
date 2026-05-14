namespace Core;

// Константы, общие для разных реализаций XM530 (монолит и адаптер-композитор)

/// <summary>
/// Статические константы ONVIF для модулей на базе чипа XM530 (Xiongmai и совместимые прошивки).
/// </summary>
public static class Xm530PtzConstants
{
    // URI пространства по спецификации ONVIF для нормализованного absolute pan/tilt
    /// <summary>URI пространства нормализованного pan/tilt по спецификации ONVIF (generic absolute).</summary>
    public const string GenericPanTiltPositionSpace = "http://www.onvif.org/ver10/tptz/PanTiltSpaces/PositionGenericSpace";

    // Делитель «градусы → [-1,1]» по оси pan (когда <see cref="PtzPanTiltInputFormat.Degrees"/>).
    /// <summary>Делитель при переводе pan из градусов в [-1, 1] при <see cref="PtzPanTiltInputFormat.Degrees"/>.</summary>
    public const float DefaultMaxPanDegrees = 180f;

    // Делитель для tilt
    /// <summary>Делитель для tilt (градусы → [-1, 1]) при <see cref="PtzPanTiltInputFormat.Degrees"/>.</summary>
    public const float DefaultMaxTiltDegrees = 90f;
}
