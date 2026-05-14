namespace Core.Interfaces;

public interface IOnvifRunService
{
    Task SetPositionAsync(string ip, string username, string password, Position position, CancellationToken token);
}