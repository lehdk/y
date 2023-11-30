using Microsoft.Extensions.Logging;
using Y.Application.Services.Interfaces;
using Y.Domain.Models;
using Y.Infrastructure.Repositories.Interfaces;

namespace Y.Application.Services;

public class PostsService : IPostsService
{
    private readonly ILogger<PostsService> _logger;
    private readonly IPostRepository _postRepository;

    public PostsService(ILogger<PostsService> logger, IPostRepository postRepository)
    {
        _logger = logger;
        _postRepository = postRepository;
    }

    public async Task<List<YPost>> GetPostsAsync(Guid? userId = null, int page = 1, int pageSize = 10)
    {
        var posts = await _postRepository.GetPosts(userId, page, pageSize).ToListAsync();

        return posts;
    }

    public Task<YPost> CreatePostAsync(string headline, string content, Guid userId)
    {
        _logger.LogInformation("Creating a post for user {userId}", userId);

        if (headline.Length < 1 || headline.Length > 75)
            throw new ArgumentException("The headline must be between [1, 75] characters");
            
        if (content.Length < 1 || content.Length > 1000)
            throw new ArgumentException("The content must be between [1, 75] characters");

        if (userId == Guid.Empty)
            throw new ArgumentException("A valid userId must be provided");

        return _postRepository.CreatePostAsync(headline, content, userId);
    }
}
