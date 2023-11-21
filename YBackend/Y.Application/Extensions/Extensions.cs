using Microsoft.Extensions.DependencyInjection;
using Y.Infrastructure.Repositories;
using Y.Infrastructure.Repositories.Interfaces;

namespace Y.Application.Extensions;

public static class Extensions
{
    public static IServiceCollection AddYApplication(this IServiceCollection services)
    {
        services.AddTransient<IUserProfileRepository, UserProfileRepository>();

        return services;
    }
}
