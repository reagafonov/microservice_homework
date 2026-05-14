using System;

namespace Core;

public class Position
{
    public int Id { get; set; }
    public float Pan { get; set; }
    public float Tilt { get; set; }

    /// <summary>Явная шкала для pan/tilt: нормализованный [-1,1] или градусы.</summary>
    public PtzPanTiltInputFormat PanTiltFormat { get; set; }
}
