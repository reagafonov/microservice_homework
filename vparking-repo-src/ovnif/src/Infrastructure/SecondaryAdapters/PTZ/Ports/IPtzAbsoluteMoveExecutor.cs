using ONVIFPTZ;

namespace Ovnif.Ports.Ptz;

/// <summary>Вторичный порт: выполнение ONVIF <c>AbsoluteMove</c> через абстракцию <see cref="PTZ"/>.</summary>
public interface IPtzAbsoluteMoveExecutor
{
    /// <summary>Отправляет команду на устройство.</summary>
    /// <param name="ptzPort">Клиент или прокси PTZ-сервиса.</param>
    /// <param name="profileToken">Токен медиа-профиля.</param>
    /// <param name="payload">Позиция и скорость.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task ExecuteAsync(
        PTZ ptzPort,
        string profileToken,
        PtzAbsoluteMovePayload payload,
        CancellationToken cancellationToken);
}
