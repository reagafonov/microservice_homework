using Consumer.Options;
using Domain.Entities;
using Infrastructure.EntityFramework;
using Infrastructure.HttpClient;
using Infrastructure.Queue.Implementation;
using Infrastructure.Repositories.Implementations;
using Infrastructure.Repositories.Implementations.Queries;
using Services.Abstractions;
using Services.Contracts;
using Services.Implementations;
using Services.Implementations.Validation;
using Services.Repositories.Abstractions;

namespace VParkingSettings;

/// <summary>
/// Регистратор сервиса
/// </summary>
public static class Registrar
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        var bootstrapServers = configuration["Kafka_BootstrapServers"];
        var topic = configuration["Kafka_Topic"];
        services.AddSingleton(new KafkaOptions()
        {
            BootstrapServers = bootstrapServers,
            Topic = topic
        });
        var server = configuration["PG_CONNECTION_SERVER"];
        var port = configuration["PG_CONNECTION_PORT"];
        var user = configuration["PG_CONNECTION_USER"];
        var password = configuration["PG_CONNECTION_PASSWORD"];
        var database = configuration["PG_CONNECTION_DATABASE_NAME"];
        var connectionString = $"Server={server};Port={port};User Id={user};Password={password};Database={database};";
        services.AddSingleton((IConfigurationRoot)configuration)
            .InstallServices()
            .ConfigureContext(connectionString)
            .InstallRepositories()
            .InstallAdapters((IConfigurationRoot)configuration);
    }

    private static IServiceCollection InstallServices(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddTransient<IOrderService, OrderService>()
            .AddTransient<IValidateDto<OrderDto>, ClientValidate>()
            .AddTransient<IOrderService, OrderService>();


        return serviceCollection;
    }

    private static IServiceCollection InstallRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddTransient<IOrderRepository, OrderRepository>()
            .AddTransient<ISimpleFilterQuery<Order, OrderFilter>, OrderSimpleFilterQuery>()
            .AddTransient<INotificationAsyncMessage, NotificationMessageProducer>();
        return serviceCollection;
    }
    
    private static void InstallAdapters(this IServiceCollection serviceCollection, IConfigurationRoot configuration)
    {
        serviceCollection
            .AddSingleton<IHttpPayment>(provider =>new BillingClient(configuration["BILLING_SERVICE_ADDRESS"]?.TrimEnd('\\') ?? "localhost", 
                provider.GetRequiredService<ILogger<BillingClient>>()))
            .AddTransient<ISimpleFilterQuery<Order, OrderFilter>, OrderSimpleFilterQuery>();
    }
}