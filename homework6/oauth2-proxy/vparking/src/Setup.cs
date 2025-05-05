using System.Diagnostics;
using Keycloak.Net;

namespace keycloak_userEditor;

public static class Setup
{
    public static void PrepareBuilder(WebApplicationBuilder builder)
    {
       
        builder.Services.AddOpenApi();
        builder.Services.AddHealthChecks(); 
        builder.Services.AddTransient(_ => new KeycloakClient(
            url,
            userName,
            password,
            new KeycloakOptions(authenticationRealm: "master", adminClientId: adminClientID)
        ));

        builder.Services.AddAutoMapper(typeof(UserInfo).Assembly);

        builder.Services.AddSingleton<UserController>();
    }

    public static void PreInitApp(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
    }
}