using Y.Domain.Models;

namespace Y.Application.Services.Interfaces;

public interface IPostsService
{
    public Task<List<YPost>> GetPostsAsync(Guid? userId, int page, int pageSize);
    public Task<YPost> GetPostAsync(Guid postId);
    public Task<YPost> CreatePostAsync(string headline, string content, Guid userId);
    public Task<YPostComment> CreateCommentAsync(Guid userId, Guid postId, string text, Guid? superCommentId);
}
