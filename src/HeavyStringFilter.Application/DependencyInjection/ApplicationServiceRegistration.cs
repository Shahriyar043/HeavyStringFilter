using HeavyStringFilter.Application.Interfaces;
using HeavyStringFilter.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HeavyStringFilter.Application.DependencyInjection;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        return services.AddScoped<IUploadService, UploadService>();
    }
}
