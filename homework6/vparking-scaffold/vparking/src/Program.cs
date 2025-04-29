using System.Diagnostics;
using System.Net.Http.Headers;
using AutoMapper;
using Flurl.Http;
using keycloak_userEditor;
using Keycloak.Net;
using Keycloak.Net.Models.Users;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;


var builder = WebApplication.CreateBuilder(args);


var configuration = builder.Configuration;
var url = configuration.GetValue<string>("URL");
var userName = configuration.GetValue<string>("USER_NAME");
var password = configuration.GetValue<string>("USER_PASSWORD");
var clientId = configuration.GetValue<string>("CLIENT_ID");
var realmName = configuration.GetValue<string>("REALM");


Debug.Assert(url != null, nameof(url) + " != null");
Debug.Assert(userName != null, nameof(userName) + " != null");
Debug.Assert(password != null, nameof(password) + " != null");
Debug.Assert(clientId != null, nameof(clientId) + " != null");
Debug.Assert(realmName != null, nameof(realmName) + " != null");

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks(); //.AddCheck<RequestTimeHealthCheck>("RequestTimeCheck");;
builder.Services.AddTransient(_ => new KeycloakClient(
    url,
    userName,
    password,
    new KeycloakOptions(authenticationRealm: "master", adminClientId: clientId)
));

// builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration);
// builder.Services.AddAuthorization();


builder.Services.AddAutoMapper(typeof(UserInfo).Assembly);

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseAuthentication();
// app.UseAuthorization();

app.MapPost("/login", () => { });

app.MapGet("/users/{id}", async (HttpContext _, [FromRoute] string id, KeycloakClient adminApi, IMapper mapper,
    ILogger<WebApplication> logger, CancellationToken token) =>
{
    User saveUser;
    try
    {
        saveUser = await adminApi.GetUserAsync(realmName, id, cancellationToken: token);
    }
    catch (FlurlHttpException e)
    {
        logger.LogError(e, e.Message);
        if (e.StatusCode.HasValue)
            return Results.StatusCode(e.StatusCode.Value);
        return Results.InternalServerError();
    }

    // if (user.Identity?.Name != result.UserName)
    //     return Results.Unauthorized();
    var result = mapper.Map<UserResult>(saveUser);
    return result != null ? Results.Ok(result) : Results.NotFound();
});

app.MapPost("/users/auth",  async (HttpContext context, KeycloakClient client, ILogger<WebApplication> log) =>
{
    return Results.Ok();
    log.LogCritical("crit");

    var httpRequest = new HttpRequestMessage(HttpMethod.Get, new Uri(url.TrimEnd('/')+"/realms/{realm}/.well-known/openid-configuration"));
    var httpClient = new HttpClient();
    var response = await httpClient.SendAsync(httpRequest);
    if (!response.IsSuccessStatusCode)
        return Results.InternalServerError();
    var result = await response.Content.ReadAsStringAsync();
    var jsonData = await response.Content.ReadAsStringAsync();
    dynamic? data = JsonConvert.DeserializeObject(jsonData);
    if (data == null)
        return Results.BadRequest();
    var endpoint = data.UserInfoEndpoint;
    var checkHttpRequest = new HttpRequestMessage(HttpMethod.Get, new Uri(endpoint));
    checkHttpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer",context.Request.Headers.Authorization.First());
    var responseUser = await httpClient.SendAsync(checkHttpRequest);
    if (!responseUser.IsSuccessStatusCode)
        return Results.StatusCode((int)responseUser.StatusCode);
    var resultUser = await responseUser.Content.ReadAsStringAsync();
    
    return Results.Ok(resultUser);
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

app.MapPut("/users/{id}", async (HttpContext _, [FromRoute] string id, [FromBody] UserUpdate userInfo,
    KeycloakClient adminApi, IMapper mapper, ILogger<WebApplication> logger, CancellationToken token) =>
{
    try
    {
        // if (user.Identity?.Name != userInfo.Login)
        //     return Results.Unauthorized();
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

app.MapDelete("/users/{id}",
    async (HttpContext _, [FromRoute] string id, KeycloakClient adminApi, CancellationToken token) =>
    {
        // var result = await adminApi.GetUserAsync(realmName, id);
        // if (result == null)
        //     return Results.NotFound();
        // if (user.Identity?.Name != result.UserName)
        //     return Results.Unauthorized();
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


