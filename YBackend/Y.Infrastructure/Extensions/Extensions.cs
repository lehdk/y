using System.ComponentModel.Design;
using Microsoft.Extensions.DependencyInjection;
using Y.Domain.Models;
using Y.Infrastructure.Repositories;
using Y.Infrastructure.Repositories.Interfaces;
using Y.Infrastructure.Tables;
using static System.Net.Mime.MediaTypeNames;

namespace Y.Infrastructure.Extensions;

public static class Extensions
{
    public static IServiceCollection AddYInfrastructure(this IServiceCollection services)
    {
        services.AddTransient<IUserProfileRepository, UserProfileRepository>();
        services.AddTransient<IPostRepository, PostRepository>();

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

    public static YPost Parse(this Posts post)
    {
        return new YPost
        {
            Id = post.PostId,
            UserId = post.UserId,
            Headline = post.Headline,
            Content = post.Content,
            CreatedAt = post.CreatedAt,
        };
    }

    public static YPostComment Parse(this PostComments comment)
    {
        return new YPostComment
        {
            CommentId = comment.CommentId,
            UserId = comment.UserId,
            PostId = comment.PostId,
            Text = comment.Text,
            SuperComment = comment.SuperCommentId,
            CreatedAt = comment.CreatedAt,
        };
    }
}
