using Core;
using Core.Interfaces;
using ONVIFDevice;
using ONVIFPTZ;
using PTZSpeed = ONVIFPTZ.PTZSpeed;
using Vector2D = ONVIFPTZ.Vector2D;
using PTZ = ONVIFPTZ.PTZ;
using Media = ONVIFMedia.Media;

namespace Ovnif;

public class ONVIFSoap( PTZ ptz, Media media, IEnumerable<Device> device):IOnvif
{
    public async Task SetPositionAsync(string ip, string username, string password, Position position, CancellationToken token)
    {
        var getProfilesResponse = (await media.GetProfilesAsync(new()));
        var profile = getProfilesResponse.Profiles[0];
        
       // var pToken = profile.token;
        // var preset = await ptz.GetNodesAsync(
        //     new GetNodesRequest()
        //     {
        //         
        //     });
        // var node = await ptz.GetNodeAsync(preset.PTZNode[0].token);
        var response = await ptz.GetServiceCapabilitiesAsync();
        var vector = new PTZVector() { PanTilt = new Vector2D() { x = 0.5f, y = -0.5f }, Zoom = new Vector1D() };
        await ptz.AbsoluteMoveAsync(profile.PTZConfiguration.token, vector, profile.PTZConfiguration.DefaultPTZSpeed);
        //var configs = await ptz.GetConfigurationAsync(profile.token);
        var info = await device.Last().GetDeviceInformationAsync(new GetDeviceInformationRequest());
    }
}