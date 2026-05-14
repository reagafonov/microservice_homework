using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using CommandLine;
using Core;
using Core.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Onvif.Core.Client.Media;
using ONVIFDevice;
using ONVIFPTZ;
using Ovnif;
using Services;
using SoapCore;

namespace MoveCamera;

public static class Program
{
    public static async Task Main(string[] args)
    {
        await Parser.Default.ParseArguments<InputOptions>(args).WithParsedAsync(async (o) =>
        {
            var position = new Position()
            {
                Pan = o.Pan,
                Tilt = o.Tilt,
            };

            var binding = new CustomBinding();
            
            binding.Elements.Add(
                new TextMessageEncodingBindingElement(
                    MessageVersion.Soap12WSAddressing10, 
                    Encoding.UTF8));
            binding.Elements.Add(new HttpTransportBindingElement());
            
            binding.SendTimeout = new TimeSpan(0, 0, 5);
            binding.ReceiveTimeout = new TimeSpan(0, 0, 10);
            binding.ReceiveTimeout = new TimeSpan(0, 0, 5);

            EndpointAddress address = new EndpointAddress($"http://{o.ip}:8899");
            var onvifAddress = new EndpointAddress($"http://{o.ip}:8899/onvif1");
            
            
            var collection = new ServiceCollection();
            collection.AddSingleton<IOnvifRunService, OnvifService>();
            collection.AddSingleton<IOnvif, PTZTest>();
            collection.AddSoapCore();
            collection.AddSingleton<PTZ, PTZClient>(_ => new PTZClient(binding, address)
            {
                ClientCredentials = { 
                    UserName = { 
                        UserName = o.Username,
                        Password = o.Password
                    },
                }
            }
            );
            collection.AddSingleton<ONVIFMedia.Media, ONVIFMedia.MediaClient>(_ => new ONVIFMedia.MediaClient(binding, address)
            {
                ClientCredentials = { 
                    UserName = { 
                        UserName = o.Username,
                        Password = o.Password
                    },
                }
            });
            collection.AddSingleton<ONVIFMedia.Media, ONVIFMedia.MediaClient>(_ => new ONVIFMedia.MediaClient(binding, address)
            {
                ClientCredentials = { 
                    UserName = { 
                        UserName = o.Username,
                        Password = o.Password
                    },
                }
            });
            collection.AddSingleton<ONVIFDevice.Device, DeviceClient>(_ => new DeviceClient(binding, address)
                {
                    ClientCredentials =
                    {
                        UserName =
                        {
                            UserName = o.Username,
                            Password = o.Password
                        }
                    }
                }
            );
            collection.AddSingleton<ONVIFDevice.Device, DeviceClient>(_ => new DeviceClient(binding, onvifAddress)
                {
                    ClientCredentials =
                    {
                        UserName =
                        {
                            UserName = o.Username,
                            Password = o.Password
                        }
                    }
                }
            );
            var provider = collection.BuildServiceProvider();
            var service = provider.GetRequiredService<IOnvifRunService>();

            await service.SetPositionAsync(o.ip, o.Username, o.Password, position, default);
        });
    }
}