using Core;

namespace Core.Interfaces;

/// <summary>
/// Входящий порт приложения: установка положения PTZ по сети (реализации живут в прикладном или инфраструктурном слое).
/// </summary>
public interface IOnvif
{
    /// <summary>
    /// Перемещает камеру в абсолютное положение pan/tilt согласно реализации (ONVIF и т.д.).
    /// </summary>
    /// <param name="ip">Хост камеры.</param>
    /// <param name="username">Учётная запись.</param>
    /// <param name="password">Пароль.</param>
    /// <param name="position">Целевые углы или нормализованные значения — трактовка зависит от реализации.</param>
    /// <param name="token">Отмена операции.</param>
    Task SetPositionAsync(string ip, string username, string password, Position position, CancellationToken token);
}

