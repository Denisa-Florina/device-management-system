using DeviceManagement.Application.Mappings;
using DeviceManagement.Application.Services;
using DeviceManagement.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DeviceManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

        services.AddScoped<IDeviceService, DeviceService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IDeviceAssignmentService, DeviceAssignmentService>();

        return services;
    }
}
