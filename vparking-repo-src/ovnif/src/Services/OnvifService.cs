using Core;
using Core.Interfaces;

namespace Services;

public class OnvifService(IOnvif onvif):IOnvifRunService
{

    public async Task SetPositionAsync(string ip, string username, string password, Position position, CancellationToken token)
    { 
        await onvif.SetPositionAsync(ip, username, password, position, token);
    }
}