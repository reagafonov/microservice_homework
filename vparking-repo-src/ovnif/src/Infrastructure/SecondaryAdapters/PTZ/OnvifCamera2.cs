using Core;
using Core.Interfaces;
using Onvif.Core.Client.Camera;
using Onvif.Core.Client.Camera.Requests;
using Onvif.Core.Client.Common;

namespace Ovnif;

public class OnvifCamera2:IOnvif
{
    public async Task SetPositionAsync(string ip, string username, string password, Position position, CancellationToken token)
    {
        var account = new Account(ip, username, password);
        var camera = new Camera(account);

        if (camera != null)
        {
            var vector = new Onvif.Core.Client.Common.PTZVector() { PanTilt = new Onvif.Core.Client.Common.Vector2D() { x = position.Pan, y = position.Tilt } };
            var speed = new Onvif.Core.Client.Common.PTZSpeed() { PanTilt = new Onvif.Core.Client.Common.Vector2D() { x = 1, y = 1 } };
            await camera.MoveAsync(MoveType.Absolute, vector, speed);
        }
    }
}