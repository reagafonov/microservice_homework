using System.ComponentModel;
using Core;
using Core.Interfaces;
using ONVIFDevice;
using ONVIFMedia;
using ONVIFPTZ;

namespace Ovnif;

public class PTZTest(PTZ ptz, Media media, Device device):IOnvif
{
    public async Task SetPositionAsync(string ip, string username, string password, Position position, CancellationToken token)
    {
        var i1 = await device.GetServicesAsync(new GetServicesRequest());
        //var i2 = await device.GetCertificatesAsync(new GetCertificatesRequest());
        var i3 = await device.GetHostnameAsync();
        //var i4 = await device.GetAccessPolicyAsync();
        var i5 = await device.GetUsersAsync(new GetUsersRequest());
        var i6 = await device.GetScopesAsync(new GetScopesRequest());
        var i7 = await device.GetCapabilitiesAsync(new GetCapabilitiesRequest());
        var i8 = await device.GetDeviceInformationAsync(new GetDeviceInformationRequest());
        var i9 = await device.GetDiscoveryModeAsync();
        var i10  = await device.GetDot11CapabilitiesAsync(new GetDot11CapabilitiesRequest());
        //var r1 = await ptz.SendAuxiliaryCommandAsync()
    }
}