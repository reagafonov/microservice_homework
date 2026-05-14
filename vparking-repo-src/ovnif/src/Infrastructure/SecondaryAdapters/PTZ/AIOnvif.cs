// using Core;
// using Core.Interfaces;
// using CoreWCF;
// using ONVIFDevice;
//
// namespace Ovnif;
//
// public class AIOnvif:IOnvif
// {
//     public async Task SetPositionAsync(string ip, string username, string password, Position position, CancellationToken token)
//     {
//         var binding = new BasicHttpBinding
//             {
//                 Mess
//             };
//         binding.Security.Mode = BasicHttpSecurityMode.None;
//         var deviceManagement = new DeviceClient(binding, new EndpointAddress($"http://{ip}:8899"));
//         deviceManagement.ClientCredentials.UserName.UserName = username;
//         deviceManagement.ClientCredentials.UserName.Password = password;
//         await deviceManagement.OpenAsync();
//         
//         var deviceInfo = await deviceManagement.GetDeviceInformationAsync(new GetDeviceInformationRequest() { });
//     }
// }