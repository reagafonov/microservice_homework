using System.Net;
using System.Net.Mail;
using Consumer.Options;
using Domain.Entities;
using Infrastructure.Email;
using Infrastructure.EntityFramework;
using Infrastructure.Queue;
using Infrastructure.Repositories.Implementations;
using Infrastructure.Repositories.Implementations.Queries;
using Services.Abstractions;
using Services.Contracts;
using Services.Implementations;
using Services.Implementations.Validation;
using Services.Repositories.Abstractions;

namespace VParkingNotification;

/// <summary>
/// Регистратор сервиса
/// </summary>
public static class Registrar
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        var bootstrapServers = configuration["Kafka.BootstrapServers"];
        var topic = configuration["Kafka.Topic"];
        var groupID = configuration["Kafka.GroupId"];
        services.AddSingleton(new KafkaOptions()
        {
            BootstrapServers = bootstrapServers,
            Topic = topic,
            GroupID = groupID
        });
        var connectionString = $"Server={configuration["PG_CONNECTION_SERVER"]};Port={configuration["PG_CONNECTION_PORT"]};User Id={configuration["PG_CONNECTION_USER"]};Password={configuration["PG_CONNECTION_PASSWORD"]};Database={configuration["PG_CONNECTION_DATABASE_NAME"]};";
        services.AddSingleton((IConfigurationRoot)configuration)
            .AddSingleton(new SmtpClient(configuration["SMTP_SERVICE_HOST"], int.TryParse(configuration["SMTP_SERVICE_PORT"], out var port)? port:0)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(configuration["SMTP_USERNAME"], configuration["SMTP_PASSWORD"])
            })
            .InstallServices()
            .ConfigureContext(connectionString)
            .InstallRepositories();
    }

    private static IServiceCollection InstallServices(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddSingleton<INotificationService, NotificationService>()
            .AddHostedService<NotificationConsumer>()
            .AddSingleton<IEmailNotificationSender,EmailNotificationNotificationSender>()
            .AddSingleton<IValidateDto<NotificationDto>, NotificationValidate>()
            .AddSingleton<INotificationService, NotificationService>();


        return serviceCollection;
    }

    private static void InstallRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddSingleton<INotificationRepository, NotificationRepository>()
            .AddSingleton<ISimpleFilterQuery<Notification, AccountFilter>, NotificationSimpleFilterQuery>()
            ;
    }
}