using System.Diagnostics;
using System.Net;
using AutoMapper;
using Flurl;
using Flurl.Http;
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
var secret= keycloakSection.GetValue<string>("CLIENT_SECRET");
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

app.MapPost("/users/login",
    async ([FromBody] UserLogin login, ILogger<WebApplication> log, CancellationToken token) =>
    {
        var client = new RestClient(url);
        var wellKnownRequest = new RestRequest($"/realms/{realmName}/.well-known/openid-configuration", Method.Get);
        var restResponse = await client.ExecuteAsync(wellKnownRequest, token);
        
        if (restResponse.StatusCode != HttpStatusCode.OK)
            return Results.InternalServerError();
        var tokenData = JsonConvert.DeserializeObject<WellKnownInfo>(restResponse.Content);
        var tokenUrl = tokenData.TokenEndpoint;
        
        var nvc = new List<KeyValuePair<string, string>>();
        nvc.Add(new KeyValuePair<string, string>("grant_type", "password"));
        nvc.Add(new KeyValuePair<string, string>("username", login.Login));
        nvc.Add(new KeyValuePair<string, string>("password", login.Password));
        nvc.Add(new KeyValuePair<string, string>("client_id", clientID));
        nvc.Add(new KeyValuePair<string, string>("client_secret", secret));
        nvc.Add(new KeyValuePair<string, string>("scope", "openid profile email"));
        using var insecureHandler = new HttpClientHandlerInsecure(); 
        using var httpClient = new HttpClient(insecureHandler);
        using var req = new HttpRequestMessage(HttpMethod.Post, tokenUrl) { Content = new FormUrlEncodedContent(nvc) };
        using var res = await httpClient.SendAsync(req);
        
       
        if (res.StatusCode != System.Net.HttpStatusCode.OK)
        {
            if (res.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return Results.Unauthorized();
            return Results.InternalServerError();
        }

        var conentStrnig = await res.Content.ReadAsStringAsync();
        var tokenResult = JsonConvert.DeserializeObject<TokenResult>(conentStrnig);
        return Results.Ok(tokenResult);
    });

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

    var result = mapper.Map<UserResult>(saveUser);
    return result != null ? Results.Ok(result) : Results.NotFound();
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

//(https://webscraping.ai/faq/httpclient-c/how-do-i-configure-httpclient-c-to-ignore-ssl-certificate-errors)
class HttpClientHandlerInsecure : HttpClientHandler
{
    public HttpClientHandlerInsecure()
    {
        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
    }
}
