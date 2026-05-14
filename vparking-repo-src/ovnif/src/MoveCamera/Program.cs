using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using CommandLine;
using Core;
using Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Ovnif.Adapters.OnvifSoap;
using Ovnif.Ports.Ptz;
using Services;
using Services.Ptz;

namespace MoveCamera;

/// <summary>
/// Точка входа первичного адаптера (CLI): разбор аргументов, сборка <see cref="Microsoft.Extensions.DependencyInjection.ServiceCollection"/>,
/// регистрация портов ONVIF PTZ и запуск сценария <see cref="Core.Interfaces.IOnvifRunService"/>.
/// </summary>
public static class Program
{
    public static async Task Main(string[] args)
    {
        // CommandLineParser: один колбэк на успешно разобранные опции
        await Parser.Default.ParseArguments<InputOptions>(args).WithParsedAsync(async (o) =>
        {
            // Доменная позиция из аргументов --pan / --tilt
            var position = new Position
            {
                Pan = o.Pan,
                Tilt = o.Tilt,
                PanTiltFormat = o.PanTiltFormat,
            };

            // WCF: кастомный binding под SOAP 1.2 + HTTP (типично для ONVIF)
            var binding = new CustomBinding();

            // Текстовая кодировка SOAP + транспорт HTTP без TLS (как на многих XM530)
            binding.Elements.Add(
                new TextMessageEncodingBindingElement(
                    MessageVersion.Soap12WSAddressing10,
                    Encoding.UTF8));
            binding.Elements.Add(new HttpTransportBindingElement());

            // Таймауты: capabilities + профили + движение не должны обрываться за 5 с
            binding.SendTimeout = new TimeSpan(0, 0, 30);
            binding.ReceiveTimeout = new TimeSpan(0, 0, 30);

            // Контейнер DI на время одного запуска процесса
            var collection = new ServiceCollection();
            // Фасад для внешнего вызова (тонкий слой над IOnvif)
            collection.AddSingleton<IOnvifRunService, OnvifService>();
            // Цепочка вторичных портов и их SOAP-реализаций
            collection.AddSingleton<IOnvifEndpointsResolver>(_ => new OnvifEndpointsResolver(binding));
            collection.AddSingleton<IMediaProfileSelector, Xm530MediaProfileSelector>();
            collection.AddSingleton<IPtzAbsoluteMoveComposer, Xm530PtzAbsoluteMoveComposer>();
            collection.AddSingleton<IPtzAbsoluteMoveExecutor, PtzAbsoluteMoveExecutor>();
            collection.AddSingleton<IOnvifMediaPtzSoapSessionFactory>(_ => new OnvifMediaPtzSoapSessionFactory(binding));
            // Реализация входящего порта IOnvif — прикладной сценарий XM530
            collection.AddSingleton<IOnvif, Xm530OnvifPtzService>();

            // Строим граф зависимостей и получаем корневой сервис
            var provider = collection.BuildServiceProvider();
            var service = provider.GetRequiredService<IOnvifRunService>();

            // Запускаем use case; учётка и IP пробрасываются вниз до SOAP
            await service.SetPositionAsync(o.ip, o.Username, o.Password, position, default);
        });
    }
}
