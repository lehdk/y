using Microsoft.Extensions.DependencyInjection;
using Y.Domain.Models;
using Y.Infrastructure.Repositories;
using Y.Infrastructure.Repositories.Interfaces;
using Y.Infrastructure.Tables;

namespace Y.Infrastructure.Extensions;

public static class Extensions
{
    public static IServiceCollection AddYInfrastructure(this IServiceCollection services)
    {
        services.AddTransient<IUserProfileRepository, UserProfileRepository>();

        return services;
    }

    public static YProfile Parse(this Profile profile)
    {
        return new YProfile
        {
            Guid = profile.Guid,
            Description = profile.Description,
            Quote = profile.Quote,
        };
    }

    public static YUser Parse(this User user, YProfile profile)
    {
        return new YUser(profile)
        {
            Guid = user.Guid,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            LastLogin = user.LastLogin,
        };
    }

    public static YUser Parse(this User user, Profile profile)
    {
        return user.Parse(profile.Parse());
    }
}
