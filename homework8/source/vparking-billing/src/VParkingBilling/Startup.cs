using System.Net.Mime;
using System.Reflection;
using System.Text;
using AutoMapper;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Prometheus;
using Prometheus.SystemMetrics;
using Services.Abstractions.Exceptions;
using Services.Repositories.Abstractions.Exceptions;
using VParkingBilling.Mapping;

namespace VParkingBilling;

public class Startup(IConfiguration configuration)
{
    // ReSharper disable once MemberCanBePrivate.Global
    public IConfiguration Configuration { get; } = configuration;

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AccountMappingsProfile>();
            cfg.AddProfile<Services.Implementations.Mapping.BillingMappingsProfile>();
        });
        configuration.AssertConfigurationIsValid();
        services.AddSingleton<IMapper>(new Mapper(configuration));
        services.AddServices(Configuration);
        services.AddControllers();
        services.UseHttpClientMetrics();
        services.AddSystemMetrics();
        // Register the Swagger generator, defining 1 or more Swagger documents
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "VParking-Orders",

                Contact = new OpenApiContact
                {
                    Name = "Roman Agafnonov",
                    Url = new Uri("https://t.me/roman_telegr"),
                    Email = "reagafonov@yandex.ru"
                }
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
        services.AddEndpointsApiExplorer();
    }


    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
       
        app.UseRouting();

        app.UseMetricServer();
        app.UseHttpMetrics();
        
        
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler(exceptionHandlerApp =>
            {
                exceptionHandlerApp.Run(async context =>
                {
                    var responseBuilder = new StringBuilder();
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                    // using static System.Net.Mime.MediaTypeNames;
                    context.Response.ContentType = MediaTypeNames.Text.Plain;

                    responseBuilder.Append("Ошибка:");

                    var exceptionHandlerPathFeature =
                        context.Features.Get<IExceptionHandlerPathFeature>();

                    switch (exceptionHandlerPathFeature?.Error)
                    {
                        case CrudUpdateException crud:
                            context.Response.StatusCode = StatusCodes.Status400BadRequest;
                            responseBuilder.AppendLine(crud.Message);
                            break;
                        case DtoValidationException dtoValidation:
                            context.Response.StatusCode = StatusCodes.Status400BadRequest;
                            responseBuilder.AppendLine(dtoValidation.Message);
                            break;
                        case ObjectNotFoundException notFoundException:
                            context.Response.StatusCode = StatusCodes.Status404NotFound;
                            responseBuilder.AppendLine(notFoundException.Message);
                            break;
                    }

                    await context.Response.WriteAsync(responseBuilder.ToString());
                });
            });
        }
        
        app.UseAuthorization();

        //включено для демонстрации
        //if (!env.IsProduction())
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(Configuration["SWAGGER_PREFIX"]+"/swagger/v1/swagger.json", "VParking-Settings V1");
                c.RoutePrefix = Configuration["SWAGGER_PREFIX"] ?? string.Empty;
            });
        }

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            
        });
        
    }
}