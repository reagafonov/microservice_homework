using System;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.EntityFramework;

/// <summary>
/// Фабрика для создания контекста БД, используется для механизма миграций
/// </summary>
// ReSharper disable once UnusedType.Global
public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    /// <summary>Creates a new instance of a derived context.</summary>
    /// <param name="args"> Arguments provided by the design-time service. </param>
    /// <returns> An instance of DatabaseContext. </returns>
    public DatabaseContext CreateDbContext(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddEnvironmentVariables()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        var configuration = builder.Build();
        var server = configuration["PG_CONNECTION_SERVER"];
        var user = configuration["PG_CONNECTION_USER"];
        var password = configuration["PG_CONNECTION_PASSWORD"];
        var port = configuration["PG_CONNECTION_PORT"];
        var dbName = configuration["PG_CONNECTION_DATABASE_NAME"];
        var connectionString = $"Server={server};Port={port};User Id={user};Password={password};Database={dbName}";
        if (connectionString == null)
        {
            throw new Exception("Connection string is null");
        }

        var dbContextOptionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
        dbContextOptionsBuilder.UseNpgsql(connectionString,
            opt => opt.MigrationsAssembly("Infrastructure.EntityFramework"));
        return new DatabaseContext(dbContextOptionsBuilder.Options);
    }
}