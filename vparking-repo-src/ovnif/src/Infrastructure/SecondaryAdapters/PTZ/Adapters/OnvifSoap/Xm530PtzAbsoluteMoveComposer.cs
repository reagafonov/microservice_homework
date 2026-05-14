using Core;
using ONVIFMedia;
using ONVIFPTZ;
using Ovnif.Ports.Ptz;

namespace Ovnif.Adapters.OnvifSoap;

/// <summary>
/// Адаптер: преобразование <see cref="Core.Position"/> в ONVIF <c>PTZVector</c>/<c>PTZSpeed</c>
/// с использованием <see cref="Xm530PtzConstants"/> (generic normalized space); шкала входа — <see cref="Position.PanTiltFormat"/>.
/// </summary>
public sealed class Xm530PtzAbsoluteMoveComposer : IPtzAbsoluteMoveComposer
{
    public PtzAbsoluteMovePayload Compose(Position position, ONVIFMedia.PTZConfiguration ptzConfiguration)
    {
        ArgumentNullException.ThrowIfNull(position);
        ArgumentNullException.ThrowIfNull(ptzConfiguration);

        // Шаг 1: приводим pan/tilt к [-1,1] в generic space (шкала задаётся явно на Position)
        var (nx, ny) = ToGenericNormalized(position.Pan, position.Tilt, position.PanTiltFormat);
        var panTilt = new Vector2D
        {
            x = nx,
            y = ny,
            space = Xm530PtzConstants.GenericPanTiltPositionSpace
        };

        // Шаг 2: вектор позиции; zoom опционален по конфигу камеры
        var vector = new PTZVector { PanTilt = panTilt };
        if (!string.IsNullOrEmpty(ptzConfiguration.DefaultAbsoluteZoomPositionSpace))
            vector.Zoom = new Vector1D { x = 0f, space = ptzConfiguration.DefaultAbsoluteZoomPositionSpace };

        // Шаг 3: скорость из профиля или дефолт «единицы» в том же пространстве
        var speed = ptzConfiguration.DefaultPTZSpeed ?? new PTZSpeed
        {
            PanTilt = new Vector2D { x = 1f, y = 1f, space = Xm530PtzConstants.GenericPanTiltPositionSpace }
        };

        return new PtzAbsoluteMovePayload(vector, speed);
    }

    private static (float x, float y) ToGenericNormalized(float pan, float tilt, PtzPanTiltInputFormat format)
    {
        return format switch
        {
            PtzPanTiltInputFormat.Normalized => (Math.Clamp(pan, -1f, 1f), Math.Clamp(tilt, -1f, 1f)),
            PtzPanTiltInputFormat.Degrees => (
                Math.Clamp(pan / Xm530PtzConstants.DefaultMaxPanDegrees, -1f, 1f),
                Math.Clamp(tilt / Xm530PtzConstants.DefaultMaxTiltDegrees, -1f, 1f)),
            _ => throw new InvalidOperationException(
                "Position.PanTiltFormat must be Normalized or Degrees (CLI: --pan-tilt-format)."),
        };
    }
}
