using System.Collections.Generic;
using Y.Domain.Models;

namespace Y.Infrastructure.Repositories.Interfaces;

public interface IPostRepository
{
    public IAsyncEnumerable<YPost> GetPosts(Guid? userId, int page,  int pageSize);
    public Task<YPost> CreatePostAsync(string headline, string content, Guid userId);
}
