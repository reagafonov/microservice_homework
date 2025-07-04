using Microsoft.AspNetCore.Mvc;
using VParkingSettings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<LiveProbe>(_=>new LiveProbe("OK"));
builder.Services.AddSwaggerGen();

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();
startup.Configure(app, app.Environment); 

app.MapGet("/health", async (HttpContext context,[FromServices]LiveProbe probe, [FromServices] ILogger<Startup> logger)  
        =>
    {
        logger.LogInformation($"Health check started {context.Request.Headers.Host}");
        await context.Response.WriteAsJsonAsync(probe);
    })
    .WithName("LiveProbe")
    .WithOpenApi();

app.Run();