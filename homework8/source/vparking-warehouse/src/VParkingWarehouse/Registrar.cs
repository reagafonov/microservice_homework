using Consumer.Options;
using Domain.Entities;
using Infrastructure.EntityFramework;
using Infrastructure.Queue;
using Infrastructure.Repositories.Implementations;
using Infrastructure.Repositories.Implementations.Queries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Abstractions;
using Services.Contracts;
using Services.Implementations;
using Services.Implementations.Validation;
using Services.Repositories.Abstractions;
using VParkingBilling.Settings;

namespace VParkingBilling;

/// <summary>
/// Регистратор сервиса
/// </summary>
public static class Registrar
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        var applicationSettings = configuration.Get<ApplicationSettings>()!;
        
        services.AddSingleton(applicationSettings);
        var kafkaServer = configuration["KAFKA_ADDRESS"];
        var kafkaTopic = configuration["KAFKA_TOPIC_NAME"];
        var kafkaGroupID = configuration["KAFKA_GROUP_ID"];
        var kafkaOptions = new KafkaOptions
        {
            BootstrapServers = kafkaServer,
            Topic = kafkaTopic,
            GroupID = kafkaGroupID
        };

        services.AddSingleton(kafkaOptions);
        
        var server = configuration["PG_CONNECTION_SERVER"];
        var port = configuration["PG_CONNECTION_PORT"];
        var user = configuration["PG_CONNECTION_USER"];
        var password = configuration["PG_CONNECTION_PASSWORD"];
        var dbName = configuration["PG_CONNECTION_DATABASE_NAME"];
        var connectionString = $"Server={server};Port={port};User Id={user};Password={password};Database={dbName}";
        
        services.AddSingleton((IConfigurationRoot)configuration)
            .InstallServices()
            .ConfigureContext(connectionString)
            .InstallRepositories();
    }

    private static IServiceCollection InstallServices(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddSingleton<IBillingService, BillingService>()
            .AddSingleton<IValidateDto<BillingDto>, AccountValidate>()
            .AddHostedService<BillingConsumer>();
        
        return serviceCollection;
    }

    private static void InstallRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddSingleton<IAccountRepository, AccountRepository>()
            .AddSingleton<ISimpleFilterQuery<Account, AccountFilter>, AccountSimpleFilterQuery>();
    }
}