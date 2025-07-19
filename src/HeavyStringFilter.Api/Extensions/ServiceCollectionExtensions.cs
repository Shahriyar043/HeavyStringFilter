using AspNetCoreRateLimit;
using FluentValidation;
using FluentValidation.AspNetCore;
using HeavyStringFilter.Api.Mappings;
using HeavyStringFilter.Api.Validators;
using HeavyStringFilter.Infrastructure.Workers;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace HeavyStringFilter.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHeavyStringFilterServices(this IServiceCollection services)
    {
        return services
            .AddValidation()
            .AddAutoMapper(typeof(UploadMappingProfile))
            .AddHostedService<FilteringBackgroundService>();
    }

    private static IServiceCollection AddValidation(this IServiceCollection services)
    {
        return services
            .AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters()
            .AddValidatorsFromAssemblyContaining<UploadChunkRequestValidator>();
    }

    public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(builder.Environment.ApplicationName))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddJaegerExporter(jaeger =>
                    {
                        jaeger.AgentHost = builder.Configuration.GetSection("Jaeger:Host").Value;
                        jaeger.AgentPort = Convert.ToInt32(builder.Configuration.GetSection("Jaeger:Port").Value);
                    });
            });

        return services;
    }

    public static IServiceCollection AddRateLimiting(this IServiceCollection services, IConfiguration config)
    {
        return services
            .Configure<IpRateLimitOptions>(config.GetSection("IpRateLimiting"))
            .AddInMemoryRateLimiting()
            .AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
    }
}
