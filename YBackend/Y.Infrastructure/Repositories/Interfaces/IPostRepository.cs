using Y.Domain.Models;

namespace Y.Infrastructure.Repositories.Interfaces;

public interface IPostRepository
{
    public Task<YPost> CreatePostAsync(string headline, string content, Guid userId);
}
