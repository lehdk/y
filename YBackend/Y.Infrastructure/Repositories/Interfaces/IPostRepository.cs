using Y.Domain.Models;

namespace Y.Infrastructure.Repositories.Interfaces;

public interface IPostRepository
{
    public Task<YPost?> GetPostAsync(Guid postId);
    public IAsyncEnumerable<YPost> GetPosts(Guid? userId, int page,  int pageSize);
    public Task<YPost> CreatePostAsync(string headline, string content, Guid userId);
    public Task<YPostComment?> GetCommentAsync(Guid commentId);
    public Task<YPostComment> CreateCommentAsync(Guid userId, Guid postId, string text, Guid? superComment);
    public Task CreateReactionAsync(Guid postId,  Guid userId, PostReactions reaction);
    public Task DeleteReactionAsync(Guid postId,  Guid userId);
    public IAsyncEnumerable<PostReactionPair> GetReactionsForPost(Guid postId);
}
