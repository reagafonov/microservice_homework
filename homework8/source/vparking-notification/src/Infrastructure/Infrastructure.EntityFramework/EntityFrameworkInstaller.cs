using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.EntityFramework;

public static class EntityFrameworkInstaller
{
    public static IServiceCollection ConfigureContext(this IServiceCollection services,
        string? connectionString)
    {
        services
            .AddDbContext<DatabaseContext>(builder => builder
                .UseLazyLoadingProxies() // lazy loading
                .UseNpgsql(connectionString), ServiceLifetime.Singleton
                );
        return services;
    }
}