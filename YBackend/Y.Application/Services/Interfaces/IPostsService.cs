using Y.Domain.Models;

namespace Y.Application.Services.Interfaces;

public interface IPostsService
{
    public Task<YPost> CreatePostAsync(string headline, string content, Guid userId);
}
