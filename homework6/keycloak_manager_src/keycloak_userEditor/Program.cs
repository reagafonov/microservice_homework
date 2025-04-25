using AutoMapper;
using Flurl.Http;
using keycloak_userEditor;
using Keycloak.AuthServices.Authentication;
using Keycloak.Net;
using Keycloak.Net.Models.Root;
using Keycloak.Net.Models.Users;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var keycloakSection = builder.Configuration.GetSection("KEYCLOAK");
var url = keycloakSection.GetValue<string>("URL");
var userName = keycloakSection.GetValue<string>("USER_NAME");
var password = keycloakSection.GetValue<string>("USER_PASSWORD");
var clientId = keycloakSection.GetValue<string>("CLIENT_ID");
var realmName = keycloakSection.GetValue<string>("REALM");

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();//.AddCheck<RequestTimeHealthCheck>("RequestTimeCheck");;
builder.Services.AddTransient<KeycloakClient>(_=>new KeycloakClient(
    url,
    userName,
    password,
    new KeycloakOptions(authenticationRealm: "master", adminClientId:clientId)
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

app.MapPost("/login", async () =>
{

});

app.MapGet("/users/{id}", async ( HttpContext context, [FromRoute]string id, KeycloakClient adminApi, IMapper mapper,ILogger<WebApplication> logger, CancellationToken token) =>
{
    try
    {
        var user = await adminApi.GetUserAsync(realmName, id, cancellationToken: token);
    }
    catch (FlurlHttpException e)
    {
        if (e.StatusCode.HasValue)
            return Results.StatusCode(e.StatusCode.Value);
        logger.LogError(e, e.Message);
    }
    var saveUser = await adminApi.GetUserAsync(realmName, id);
    
    // if (user.Identity?.Name != result.UserName)
    //     return Results.Unauthorized();
    var result = mapper.Map<UserResult>(saveUser);
    return result != null ? Results.Ok(result) : Results.NotFound();
});

app.MapPost("/users", async ([FromBody]UserInfo userInfo, KeycloakClient adminApi, IMapper mapper, ILogger<WebApplication> log, CancellationToken token) =>
{
    try
    {
        var userRepresentation = mapper.Map<User>(userInfo);
        userRepresentation.EmailVerified = true;
        userRepresentation.Enabled = true;
        await adminApi.CreateUserAsync(realmName,userRepresentation, token);
        var resultUser = await adminApi.GetUsersAsync(realmName, username: userRepresentation.UserName, cancellationToken:token);
        var singleOrDefault = resultUser?.SingleOrDefault();
        var userId = singleOrDefault?.Id;
        try
        {
            await adminApi.ResetUserPasswordAsync(realmName, userId, userInfo.Password, false, token);
        }
        catch (Exception e)
        {
            await adminApi.DeleteUserAsync(realmName, userId, cancellationToken:token);
            return Results.BadRequest(e.Message);
        }
        userRepresentation.Id = userId;
        return Results.Ok(mapper.Map<UserResult>(userRepresentation));
    }
    catch (Exception e)
    {
        log.LogError(e, e.Message);
        return Results.InternalServerError();
    }
  
    
});

app.MapPut("/users/{id}", async ( HttpContext context,[FromRoute]string id, [FromBody]UserUpdate userInfo, KeycloakClient adminApi, IMapper mapper, ILogger<WebApplication> logger, CancellationToken token) =>
{
    try
    {
        // if (user.Identity?.Name != userInfo.Login)
        //     return Results.Unauthorized();
        var userRepresentation = mapper.Map<User>(userInfo);
        try
        {
            userRepresentation.Id = id;
            await adminApi.UpdateUserAsync(realmName,id,userRepresentation, token);
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
        logger.LogError(e,e.Message);
        return Results.InternalServerError();
    }
   
});

app.MapDelete("/users/{id}", async (HttpContext context, [FromRoute]string id, KeycloakClient adminApi, CancellationToken token) =>
{
    // var result = await adminApi.GetUserAsync(realmName, id);
    // if (result == null)
    //     return Results.NotFound();
    // if (user.Identity?.Name != result.UserName)
    //     return Results.Unauthorized();
    await adminApi.DeleteUserAsync(realmName,id, token);
    return Results.Ok();
});

app.UseHealthChecks("/health");
app.Run();

