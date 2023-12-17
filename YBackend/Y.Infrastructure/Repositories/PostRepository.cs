using Y.Domain.Models;
using Y.Infrastructure.Repositories.Interfaces;

namespace Y.Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
    public Task<YPostComment> CreateCommentAsync(Guid userId, Guid postId, string text, Guid? superComment)
    {
        throw new NotImplementedException();
    }

    public Task<YPost> CreatePostAsync(string headline, string content, Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<YPostComment?> GetCommentAsync(Guid commentId)
    {
        throw new NotImplementedException();
    }

    public Task<YPost?> GetPostAsync(Guid postId)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<YPost> GetPosts(Guid? userId, int page, int pageSize)
    {
        throw new NotImplementedException();
    }
}
