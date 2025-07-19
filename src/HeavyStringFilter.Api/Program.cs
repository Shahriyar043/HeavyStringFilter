using AspNetCoreRateLimit;
using HeavyStringFilter.Api.Converters;
using HeavyStringFilter.Api.Extensions;
using HeavyStringFilter.Application.DependencyInjection;
using HeavyStringFilter.Infrastructure.DependencyInjection;
using Scalar.AspNetCore;
using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        options.JsonSerializerOptions.Converters.Add(new StringTrimmerJsonConverter());
    });

builder.Services
    .AddMemoryCache()
    .AddAuthorization()
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddHeavyStringFilterServices()
    .AddOpenApi()
    .AddEndpointsApiExplorer();

builder.Services.AddOpenApi();
builder.Services.AddOpenTelemetry(builder);
builder.Services.AddRateLimiting(builder.Configuration);
builder.Services.AddHealthChecks();
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1");
    });

    app.UseReDoc(options =>
    {
        options.SpecUrl("/openapi/v1.json");
    });

    app.MapScalarApiReference();
}
app.UseIpRateLimiting();
app.UseExceptionHandling();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
public partial class Program { }
