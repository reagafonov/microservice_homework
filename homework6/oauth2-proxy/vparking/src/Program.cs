using System.Diagnostics;
using AutoMapper;
using keycloak_userEditor;
using Keycloak.Net;
using Keycloak.Net.Models.Users;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;


var builder = WebApplication.CreateBuilder(args);

Setup.PrepareBuilder(builder);
var app = builder.Build();
// Configure the HTTP request pipeline.

Setup.PreInitApp(app);


var configuration = builder.Configuration;
var url = configuration.GetValue<string>("URL");
var userName = configuration.GetValue<string>("USER_NAME");
var password = configuration.GetValue<string>("USER_PASSWORD");
var clientID = configuration.GetValue<string>("CLIENT_ID");
var adminClientID = configuration.GetValue<string>("ADMIN_CLIENT_ID");
var realmName = configuration.GetValue<string>("REALM");
var secret = configuration.GetValue<string>("CLIENT_SECRET");

Debug.Assert(url != null, nameof(url) + " != null");
Debug.Assert(userName != null, nameof(userName) + " != null");
Debug.Assert(password != null, nameof(password) + " != null");
Debug.Assert(adminClientID != null, nameof(adminClientID) + " != null");
Debug.Assert(clientID != null, nameof(clientID) + " != null");
Debug.Assert(realmName != null, nameof(realmName) + " != null");
Debug.Assert(secret != null, nameof(secret) + " != null");


app.MapGet("/users/me", async ([FromHeader(Name = "X-Auth-Request-Access-Token")] string accessToken,
    [FromServices] UserController controller, IMapper mapper,
    ILogger<WebApplication> logger, CancellationToken token) =>
{
    var (value, error) = await controller.GetUserInfoAsync(accessToken, url, realmName, token);

    if (error != null)
        return error ?? Results.InternalServerError();
    return Results.Ok(value);
});

app.MapPost("/users/add", async ([FromBody] UserInfo userInfo, KeycloakClient adminApi, IMapper mapper,
    ILogger<WebApplication> log, CancellationToken token) =>
{
    try
    {
        var userRepresentation = mapper.Map<User>(userInfo);
        userRepresentation.EmailVerified = true;
        userRepresentation.Enabled = true;
        await adminApi.CreateUserAsync(realmName, userRepresentation, token);
        var resultUser =
            await adminApi.GetUsersAsync(realmName, username: userRepresentation.UserName, cancellationToken: token);
        var singleOrDefault = resultUser.SingleOrDefault();
        var userId = singleOrDefault?.Id;
        if (userId == null)
            return Results.InternalServerError("User not created");
        try
        {
            await adminApi.ResetUserPasswordAsync(realmName, userId, userInfo.Password, false, token);
        }
        catch (Exception e)
        {
            await adminApi.DeleteUserAsync(realmName, userId, cancellationToken: token);
            return Results.BadRequest(e.Message);
        }

        var user = await adminApi.GetUserAsync(realmName, userId, cancellationToken: token);
        return Results.Ok(mapper.Map<UserResult>(user));
    }
    catch (Exception e)
    {
        log.LogError(e, e.Message);
        return Results.InternalServerError();
    }
});

app.MapPut("/users/me", async ([FromBody] UserUpdate userInfo,
    [FromHeader(Name = "X-Auth-Request-Access-Token")] string accessToken,
    [FromServices] UserController controller, [FromServices] KeycloakClient adminApi, [FromServices] IMapper mapper,
    [FromServices] ILogger<WebApplication> logger, CancellationToken token) =>
{
    try
    {
        var (user, error) = await controller.GetUserInfoAsync(accessToken, url, realmName, token);
        if (error != null)
            return error;
        var id = user.Id;
        var userRepresentation = mapper.Map<User>(userInfo);
        try
        {
            userRepresentation.Id = id;
            await adminApi.UpdateUserAsync(realmName, id, userRepresentation, token);
            return Results.Ok(mapper.Map<UserResult>(userRepresentation));
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return Results.InternalServerError();
        }
    }
    catch (Exception e)
    {
        logger.LogError(e, e.Message);
        return Results.InternalServerError();
    }
});

app.MapDelete("/users/me",
    async (HttpContext _, [FromHeader(Name = "X-Auth-Request-Access-Token")] string accessToken,
        [FromServices] UserController controller, [FromServices] KeycloakClient adminApi, CancellationToken token) =>
    {
        var (user, error) = await controller.GetUserInfoAsync(accessToken, url, realmName, token);
        if (error != null)
            return error;
        var id = user.Id;
        await adminApi.DeleteUserAsync(realmName, id, token);
        return Results.Ok();
    });

app.UseHealthChecks("/users/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        switch (report.Status)
        {
            case HealthStatus.Unhealthy:
                await context.Response.WriteAsync("{Status: Unhealthy}");
                break;
            case HealthStatus.Degraded:
                await context.Response.WriteAsync("{Status: Degraded}");
                break;
            case HealthStatus.Healthy:
                await context.Response.WriteAsync("{Status: OK}");
                break;
            default:
                await context.Response.WriteAsync("Error");
                break;
        }
    }
});
app.Run();