using Microsoft.Extensions.DependencyInjection;
using Y.Application.Services;
using Y.Application.Services.Interfaces;

namespace Y.Application.Extensions;

public static class Extensions
{
    public static IServiceCollection AddYApplication(this IServiceCollection services)
    {
        services.AddTransient<IUserProfileService, UserProfileService>();

        return services;
    }
}
