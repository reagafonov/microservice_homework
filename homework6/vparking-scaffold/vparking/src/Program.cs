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
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var url = configuration.GetValue<string>("URL");
var userName = configuration.GetValue<string>("USER_NAME");
var password = configuration.GetValue<string>("USER_PASSWORD");
var clientId = configuration.GetValue<string>("CLIENT_ID");
var realmName = configuration.GetValue<string>("REALM");
var clientSecret = configuration.GetValue<string>("CLIENT_SECRET");


Debug.Assert(url != null, nameof(url) + " != null");
Debug.Assert(userName != null, nameof(userName) + " != null");
Debug.Assert(password != null, nameof(password) + " != null");
Debug.Assert(clientId != null, nameof(clientId) + " != null");
Debug.Assert(realmName != null, nameof(realmName) + " != null");

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks(); 
builder.Services.AddTransient(_ => new KeycloakClient
(
    url,
    userName,
    password,
    new KeycloakOptions(authenticationRealm: "master", adminClientId: clientId)
));

builder.Services.AddAutoMapper(typeof(UserInfo).Assembly);
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPost("/users/login", async (HttpContext context, [FromBody] UserLogin userLogin, ILogger<WebApplication> log) =>
{
    var client = new HttpClient();
    var request = new HttpRequestMessage(HttpMethod.Post, url + "/realms/vparking/protocol/openid-connect/token");
    var paramters = new Dictionary<string, string>
    {
        { "grant_type", "password" },
        { "client_id", clientId },
        { "password", userLogin.Password },
        { "username", userLogin.Login },
        { "scope", "openid profile email " },
        { "client_secret", clientSecret },
    };
    request.Content = new FormUrlEncodedContent(paramters);
    var response = await client.SendAsync(request);

    var responseContent = await response.Content.ReadAsStringAsync();

    return Results.Text(responseContent);
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

app.MapGet("/users/auth",
    async (HttpContext context) =>
    {
        var jwt = GetToken(context);
        if (jwt == null)
            return Results.Unauthorized();

        var (_, result) = await GetHeaders(url, jwt);
        return result;
    });


app.MapGet("/users/{id}", async (HttpContext context, [FromRoute] string id, IMapper mapper,
    ILogger<WebApplication> logger, CancellationToken token) =>
{
    var userApi = new KeycloakClient(url, () => GetToken(context));
    User saveUser;
    try
    {
        saveUser = await userApi.GetUserAsync(realmName, id, cancellationToken: token);
        await GetHeaders(url,GetToken(context));

    }
    catch (FlurlHttpException e)
    {
        logger.LogError(e, e.Message);
        if (e.StatusCode.HasValue)
            return Results.StatusCode(e.StatusCode.Value);
        return Results.InternalServerError();
    }
    catch (Exception e)
    {
        logger.LogError(e, e.Message);
        return Results.InternalServerError();
    }

    var result = mapper.Map<UserResult>(saveUser);
    return result != null ? Results.Ok(result) : Results.NotFound();
});


app.MapPut("/users/{id}", async (HttpContext context, [FromRoute] string id, [FromBody] UserUpdate userInfo,
    IMapper mapper, ILogger<WebApplication> logger, CancellationToken token) =>
{
    var userApi = new KeycloakClient(url, () => GetToken(context));
    var userRepresentation = mapper.Map<User>(userInfo);
    try
    {
        userRepresentation.Id = id;
        await userApi.UpdateUserAsync(realmName, id, userRepresentation, token);
        return Results.Ok(mapper.Map<UserResult>(userRepresentation));
    }
    catch (FlurlHttpException e)
    {
        logger.LogError(e, e.Message);
        if (e.StatusCode.HasValue)
            return Results.StatusCode(e.StatusCode.Value);
        return Results.InternalServerError();
    }
    catch (Exception e)
    {
        logger.LogError(e, e.Message);
        return Results.InternalServerError();
    }
});

app.MapDelete("/users/{id}",
    async (HttpContext context, [FromRoute] string id, CancellationToken token) =>
    {
        var userApi = new KeycloakClient(url, () => GetToken(context));
        await userApi.DeleteUserAsync(realmName, id, token);
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

string? GetToken(HttpContext context)
{
    var headersAuthorization = context.Request.Headers.Authorization;
    var authorizationValue = headersAuthorization.FirstOrDefault()?.Split(' ');

    if (authorizationValue.Length != 2)
        return null;

    return authorizationValue[1];
}

async Task<(HttpResponseMessage response, IResult result)> GetHeaders(string s, string jwt1)
{
    var client = new HttpClient();
    var request =
        new HttpRequestMessage(HttpMethod.Post, s + "/realms/vparking/protocol/openid-connect/userinfo");
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt1);
    var httpResponseMessage = await client.SendAsync(request);
    if (!httpResponseMessage.IsSuccessStatusCode)
        return (httpResponseMessage, Results.Unauthorized());
    var handler = new JwtSecurityTokenHandler();
    var jsonToken = handler.ReadToken(jwt1);
    var tokenS = jsonToken as JwtSecurityToken;
    var id = tokenS?.Id;
    if (id == null)
        return (httpResponseMessage, Results.Unauthorized());

    var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
    dynamic jsonData = JsonConvert.DeserializeObject(responseContent);
    var user = new UserResult()
    {
        FirstName = jsonData.given_name,
        LastName = jsonData.family_name,
        Email = jsonData.email,
        Login = jsonData.login,
        Id = jsonData.sub,
    };
    var serializeObject = JsonConvert.SerializeObject(user);
    var inArray = Encoding.UTF8.GetBytes(serializeObject);
    var base64String = Convert.ToBase64String(inArray);
    httpResponseMessage.Headers.Add("x-auth-request-user", base64String);
    httpResponseMessage.Headers.Add("x-auth-request-email", user.Email);
    httpResponseMessage.Headers.Add("x-auth-request-id", user.Id);
    return (httpResponseMessage, Results.Ok());
}