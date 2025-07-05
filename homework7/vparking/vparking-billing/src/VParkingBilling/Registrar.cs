using Domain.Entities;
using Infrastructure.EntityFramework;
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
        var connectionString = $"Server={configuration["PG_CONNECTION_SERVER"]};Port=5432;User Id={configuration["PG_CONNECTION_USER"]};Password={configuration["PG_CONNECTION_PASSWORD"]};Database=billing";
        services.AddSingleton((IConfigurationRoot)configuration)
            .InstallServices()
            .ConfigureContext(connectionString)
            .InstallRepositories();
    }

    private static IServiceCollection InstallServices(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddTransient<IAccountService, AccountService>()
            .AddTransient<IValidateDto<AccountDto>, AccountValidate>();


        return serviceCollection;
    }

    private static void InstallRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddTransient<IAccountRepository, AccountRepository>()
            .AddTransient<ISimpleFilterQuery<Account, AccountFilter>, AccountSimpleFilterQuery>();
    }
}