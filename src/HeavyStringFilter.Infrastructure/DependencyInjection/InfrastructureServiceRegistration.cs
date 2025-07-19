using HeavyStringFilter.Application.Interfaces;
using HeavyStringFilter.Application.Services;
using HeavyStringFilter.Infrastructure.Filtering;
using HeavyStringFilter.Infrastructure.Queue;
using HeavyStringFilter.Infrastructure.Storage;
using HeavyStringFilter.Infrastructure.Workers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HeavyStringFilter.Infrastructure.DependencyInjection;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddSingleton<IUploadStorage, InMemoryUploadStorage>()
            .AddSingleton<IProcessingQueue, InMemoryProcessingQueue>()
            .AddSingleton<IFilterService, FilterService>()
            .Configure<FilterConfig>(configuration.GetSection("FilterConfig"))
            .AddHostedService<FilteringBackgroundService>();

        return services;
    }
}
