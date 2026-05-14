using ONVIFPTZ;
using Ovnif.Ports.Ptz;

namespace Ovnif.Adapters.OnvifSoap;

/// <summary>Тонкий адаптер: делегирует вызов <c>AbsoluteMoveAsync</c> контракту <see cref="PTZ"/>.</summary>
public sealed class PtzAbsoluteMoveExecutor : IPtzAbsoluteMoveExecutor
{
    public Task ExecuteAsync(
        PTZ ptzPort,
        string profileToken,
        PtzAbsoluteMovePayload payload,
        CancellationToken cancellationToken)
    {
        // Валидация контракта до SOAP
        ArgumentNullException.ThrowIfNull(ptzPort);
        ArgumentNullException.ThrowIfNull(payload);
        // Отмена до ухода в сеть
        cancellationToken.ThrowIfCancellationRequested();
        // Один вызов WCF — вся семантика на стороне камеры
        return ptzPort.AbsoluteMoveAsync(profileToken, payload.Position, payload.Speed);
    }
}
