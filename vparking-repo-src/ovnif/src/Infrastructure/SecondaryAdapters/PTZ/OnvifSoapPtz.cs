using System.ServiceModel;
using System.ServiceModel.Channels;
using Core;
using Core.Interfaces;
using ONVIFDevice;
using ONVIFMedia;
using ONVIFPTZ;

namespace Ovnif;

/// <summary>
/// PTZ через ONVIF: адреса Media/PTZ берутся из Device.GetCapabilities (часто отличаются от корня :8899).
/// </summary>
public sealed class OnvifSoapPtz(Binding binding) : IOnvif
{
    public async Task SetPositionAsync(
        string ip,
        string username,
        string password,
        Position position,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(position);
        token.ThrowIfCancellationRequested();

        var caps = await DiscoverCapabilitiesAsync(ip, username, password, token).ConfigureAwait(false)
                   ?? throw new InvalidOperationException(
                       $"GetCapabilities failed for http://{ip}:8899 and http://{ip}:8899/onvif1 — check IP, port and credentials.");

        var defaultBase = $"http://{ip}:8899";
        var mediaUri = NormalizeServiceUrl(caps.Media?.XAddr, defaultBase, ip);
        var ptzUri = NormalizeServiceUrl(caps.PTZ?.XAddr, defaultBase, ip);

        var media = new ONVIFMedia.MediaClient(binding, new EndpointAddress(mediaUri));
        var ptz = new ONVIFPTZ.PTZClient(binding, new EndpointAddress(ptzUri));
        ApplyCredentials(media, username, password);
        ApplyCredentials(ptz, username, password);

        try
        {
            await media.OpenAsync().ConfigureAwait(false);
            await ptz.OpenAsync().ConfigureAwait(false);

            token.ThrowIfCancellationRequested();

            var getProfilesResponse = await media.GetProfilesAsync().ConfigureAwait(false);
            var profiles = getProfilesResponse.Profiles ?? Array.Empty<ONVIFMedia.Profile>();
            var profile = profiles.FirstOrDefault(static p => p.PTZConfiguration != null);
            if (profile?.PTZConfiguration == null || string.IsNullOrEmpty(profile.token))
                throw new InvalidOperationException(
                    "No media profile with PTZ configuration or profile token is missing.");

            token.ThrowIfCancellationRequested();

            var ptzConfig = profile.PTZConfiguration;
            var panTilt = new ONVIFPTZ.Vector2D
            {
                x = position.Pan,
                y = position.Tilt
            };
            if (!string.IsNullOrEmpty(ptzConfig.DefaultAbsolutePantTiltPositionSpace))
                panTilt.space = ptzConfig.DefaultAbsolutePantTiltPositionSpace;

            var vector = new ONVIFPTZ.PTZVector { PanTilt = panTilt };
            if (!string.IsNullOrEmpty(ptzConfig.DefaultAbsoluteZoomPositionSpace))
                vector.Zoom = new ONVIFPTZ.Vector1D { x = 0f, space = ptzConfig.DefaultAbsoluteZoomPositionSpace };

            var speed = ptzConfig.DefaultPTZSpeed ?? new ONVIFPTZ.PTZSpeed { PanTilt = new ONVIFPTZ.Vector2D { x = 1f, y = 1f } };

            await ptz.AbsoluteMoveAsync(profile.token, vector, speed).ConfigureAwait(false);
        }
        finally
        {
            CloseQuietly(media);
            CloseQuietly(ptz);
        }
    }

    private async Task<ONVIFDevice.Capabilities?> DiscoverCapabilitiesAsync(
        string host,
        string username,
        string password,
        CancellationToken token)
    {
        foreach (var path in new[] { "", "/onvif1" })
        {
            token.ThrowIfCancellationRequested();
            var device = new ONVIFDevice.DeviceClient(binding, new EndpointAddress($"http://{host}:8899{path}"));
            ApplyCredentials(device, username, password);
            try
            {
                await device.OpenAsync().ConfigureAwait(false);
                var resp = await device.GetCapabilitiesAsync(new[] { ONVIFDevice.CapabilityCategory.All }).ConfigureAwait(false);
                var caps = resp.Capabilities;
                CloseQuietly(device);
                if (caps != null)
                    return caps;
            }
            catch
            {
                device.Abort();
            }
        }

        return null;
    }

    private static void ApplyCredentials<TChannel>(ClientBase<TChannel> client, string user, string pass) where TChannel:class
    {
        client.ClientCredentials.UserName.UserName = user;
        client.ClientCredentials.UserName.Password = pass;
    }

    private static string NormalizeServiceUrl(string? xAddr, string fallback, string preferredHost)
    {
        if (string.IsNullOrWhiteSpace(xAddr))
            return fallback;
        if (!Uri.TryCreate(xAddr, UriKind.Absolute, out var uri))
            return fallback;
        var builder = new UriBuilder(uri) { Host = preferredHost };
        return builder.Uri.AbsoluteUri.TrimEnd('/');
    }

    private static void CloseQuietly(ICommunicationObject client)
    {
        try
        {
            if (client.State == CommunicationState.Faulted)
            {
                client.Abort();
                return;
            }

            if (client.State is CommunicationState.Opened or CommunicationState.Opening)
                client.Close(TimeSpan.FromSeconds(10));
        }
        catch (CommunicationException)
        {
            client.Abort();
        }
        catch (TimeoutException)
        {
            client.Abort();
        }
    }
}
