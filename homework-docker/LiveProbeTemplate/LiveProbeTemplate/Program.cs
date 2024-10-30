using LiveProbeTemplate;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<LiveProbe>(_=>new LiveProbe("OK"));
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.MapGet("/health", (HttpContext context,LiveProbe probe) 
        => context.Response.WriteAsJsonAsync(probe))
    .WithName("LiveProbe")
    .WithOpenApi();

app.Run();