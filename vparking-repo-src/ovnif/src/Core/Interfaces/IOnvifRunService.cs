using Core;

namespace Core.Interfaces;

/// <summary>Входящий порт: сценарий запуска установки PTZ (фасад над <see cref="IOnvif"/>).</summary>
public interface IOnvifRunService
{
    /// <inheritdoc cref="IOnvif.SetPositionAsync" />
    Task SetPositionAsync(string ip, string username, string password, Position position, CancellationToken token);
}