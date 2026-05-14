using Core;
using Core.Interfaces;
using SharpOnvifClient;
using SharpOnvifCommon;

public class OnvifCamera() : IOnvif
{
    public async Task SetPositionAsync(string ip, string username, string password,Position position, CancellationToken token)
    {
        var onvifDevices = await OnvifDiscoveryClient.DiscoverAsync();
        var device = onvifDevices.FirstOrDefault(x => x.Addresses.Any(x => x.Contains(ip)));
        var address = device?.Addresses?.FirstOrDefault(x => x.Contains(ip));
        Console.WriteLine($"Address: {address}");
        var client = new SimpleOnvifClient(address, username, password);
        var services = await client.GetServicesAsync();
        var ptzService = services.Service.FirstOrDefault(x => x.Namespace == OnvifServices.PTZ);

        if (ptzService != null)
        {
            await client.AbsoluteMoveAsync("1", (float)Math.PI / 4, (float)Math.PI / 4, 1, 1);
        }
    }

}