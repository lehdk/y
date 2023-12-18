using Microsoft.Extensions.DependencyInjection;
using Y.Application.Services;
using Y.Application.Services.Interfaces;

namespace Y.Application.Extensions;

public static class Extensions
{
    public static IServiceCollection AddYApplication(this IServiceCollection services)
    {
        services.AddTransient<IArgon2idPasswordHashAlgorithm, Argon2idPasswordHashAlgorithm>();
        
        services.AddTransient<IUserProfileService, UserProfileService>();

        services.AddTransient<IPostsService, PostsService>();

        services.AddTransient<IFollowService, FollowService>();

        return services;
    }
}
