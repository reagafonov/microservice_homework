using System.Diagnostics;
using System.Net;
using AutoMapper;
using keycloak_userEditor;
using Keycloak.Net;
using Keycloak.Net.Models.Users;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using RestSharp;


var builder = WebApplication.CreateBuilder(args);
var keycloakSection = builder.Configuration;
var url = keycloakSection.GetValue<string>("URL");
var userName = keycloakSection.GetValue<string>("USER_NAME");
var password = keycloakSection.GetValue<string>("USER_PASSWORD");
var clientID = keycloakSection.GetValue<string>("CLIENT_ID");
var adminClientID = keycloakSection.GetValue<string>("ADMIN_CLIENT_ID");
var realmName = keycloakSection.GetValue<string>("REALM");
var secret = keycloakSection.GetValue<string>("CLIENT_SECRET");
Debug.Assert(url != null, nameof(url) + " != null");
Debug.Assert(userName != null, nameof(userName) + " != null");
Debug.Assert(password != null, nameof(password) + " != null");
Debug.Assert(adminClientID != null, nameof(adminClientID) + " != null");
Debug.Assert(clientID != null, nameof(clientID) + " != null");
Debug.Assert(realmName != null, nameof(realmName) + " != null");
Debug.Assert(secret != null, nameof(secret) + " != null");

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks(); //.AddCheck<RequestTimeHealthCheck>("RequestTimeCheck");;
builder.Services.AddTransient(_ => new KeycloakClient(
    url,
    userName,
    password,
    new KeycloakOptions(authenticationRealm: "master", adminClientId: adminClientID)
));



builder.Services.AddAutoMapper(typeof(UserInfo).Assembly);

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


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


app.MapGet("/users/me", async (HttpContext context,
    [FromHeader(Name = "X-Auth-Request-Access-Token")] string accessToken,  CancellationToken token) =>
{
    var (value, error) = await GetUserInfoAsync(accessToken, token);
    if (error != null)
        return error;

    return Results.Ok(value);
});

async Task<(UserResult? value, IResult internalServerError1)> GetUserInfoAsync(string accessToken, CancellationToken cancellationToken)
{
    var (hasError, wellKnown, result) = await GetWellKnown(url, realmName, cancellationToken);
    if (hasError)
        return (null, result ?? Results.InternalServerError());
    var headers = new Dictionary<string, string>() { { "Authorization", "Bearer " + accessToken } };
    var (userResult, error) = await GetDataTypedAsync<UserResult>(wellKnown.UserInfoEndpoint, null, headers);
    if (error != null)
        return (userResult, error ?? Results.InternalServerError());
    return (userResult, null);
}


app.MapPut("/users/me", async (HttpContext _, [FromBody] UserUpdate userInfo,
    [FromHeader(Name = "X-Auth-Request-Access-Token")] string accessToken, 
    KeycloakClient adminApi, IMapper mapper, ILogger<WebApplication> logger, CancellationToken token) =>
{
    try
    {
        var (value, error) = await GetUserInfoAsync(accessToken, token);
        if (error != null)
            return error;
        var userRepresentation = mapper.Map<User>(userInfo);
        try
        {
            userRepresentation.Id = value.Id;
            await adminApi.UpdateUserAsync(realmName, value.Id, userRepresentation, token);
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
    async (HttpContext _,   [FromHeader(Name = "X-Auth-Request-Access-Token")] string accessToken,  KeycloakClient adminApi, CancellationToken token) =>
    {
        var (value, error) = await GetUserInfoAsync(accessToken, token);
        if (error != null)
            return error;
        await adminApi.DeleteUserAsync(realmName, value.Id, token);
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

async Task<(bool, WellKnownInfo wellKnown, IResult result)> GetWellKnown(string url, string realmName,
    CancellationToken cancellationToken)
{
    var client = new RestClient(url);
    var wellKnownRequest = new RestRequest($"/realms/{realmName}/.well-known/openid-configuration", Method.Get);
    var restResponse = await client.ExecuteAsync(wellKnownRequest, cancellationToken);

    if (restResponse.StatusCode != HttpStatusCode.OK)
    {
        var internalServerError = Results.InternalServerError();
        return (true, null, internalServerError);
    }

    var tokenData = JsonConvert.DeserializeObject<WellKnownInfo>(restResponse.Content);
    return (false, tokenData, null);
}

async Task<(TResult? value, IResult? error)> GetDataTypedAsync<TResult>(string url, HttpContent content,
    Dictionary<string, string>? headers = null)
    where TResult : class
{
    var (value, error) = await GetDataAsync(url, content, headers);
    if (error != null)
        return (null, error);
    var result = JsonConvert.DeserializeObject<TResult>(value);
    return (result, null);
}

async Task<(string value, IResult? error)> GetDataAsync(string url, HttpContent content,
    Dictionary<string, string>? headers = null)
{
    using var insecureHandler = new HttpClientHandlerInsecure();
    using var httpClient = new HttpClient(insecureHandler);
    using var req = new HttpRequestMessage(HttpMethod.Post, url);
    req.Content = content;
    if (headers != null)
        foreach (var header in headers)
            req.Headers.Add(header.Key, new[] { header.Value });

    using var res = await httpClient.SendAsync(req);


    if (res.StatusCode != System.Net.HttpStatusCode.OK)
    {
        if (res.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            return (null, Results.Unauthorized());
        return (null, Results.InternalServerError());
    }

    var contentString = await res.Content.ReadAsStringAsync();
    return (contentString, null);
}


//(https://webscraping.ai/faq/httpclient-c/how-do-i-configure-httpclient-c-to-ignore-ssl-certificate-errors)
class HttpClientHandlerInsecure : HttpClientHandler
{
    public HttpClientHandlerInsecure()
    {
        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
    }
}