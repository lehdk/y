using Microsoft.Extensions.Logging;
using Y.Application.Services.Interfaces;
using Y.Domain.Exceptions;
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

    public async Task<List<YPost>> GetPostsAsync(Guid? userId = null, Guid? showFollowerForUser = null, int page = 1, int pageSize = 10)
    {
        if(page < 1)
            throw new ValidationException("The page must be > 1");
        
        if(pageSize < 1 || pageSize > 10)
            throw new ValidationException("The pageSize must be 0 < pageSize < 11");


        var posts = await _postRepository.GetPosts(userId, showFollowerForUser, page, pageSize).ToListAsync();

        foreach (var p in posts)
        {
            p.Reactions = await _postRepository.GetReactionsForPost(p.Id).ToListAsync();

            p.PostComments = await _postRepository.GetCommentsOnPost(p.Id).ToListAsync();
        }

        return posts;
    }

    public async Task<YPost?> GetPostAsync(Guid postId)
    {
        if(postId == Guid.Empty) 
            throw new ArgumentNullException(nameof(postId), "The post id must be defined");

        var post = await _postRepository.GetPostAsync(postId);

        if (post is null)
            return null;

        post.Reactions = await _postRepository.GetReactionsForPost(postId).ToListAsync();

        post.PostComments = await _postRepository.GetCommentsOnPost(postId).ToListAsync();

        return post;
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

    public Task<YPostComment?> GetComment(Guid commentId)
    {
        if (commentId == Guid.Empty)
            throw new ArgumentException("A valid userId must be provided");

        return _postRepository.GetCommentAsync(commentId);
    }

    public async Task<YPostComment> CreateCommentAsync(Guid userId, Guid postId, string text, Guid? superCommentId)
    {
        _logger.LogInformation("Creating comment on post {postId} by user {userId} on the superComment {superCommentId}", postId, userId, superCommentId);

        if (userId == Guid.Empty)
            throw new ArgumentException($"{nameof(userId)} must be defined");

        if (postId == Guid.Empty)
            throw new ArgumentException($"{nameof(postId)} must be defined");

        if (text.Length < 1 || text.Length > 250)
            throw new ArgumentException($"{nameof(text)} must be in the range [1, 250]");

        var post = await GetPostAsync(postId);

        if (post is null)
            throw new ValidationException($"No post with the id {postId} was found");

        if(superCommentId is not null)
        {
            var superComment = await GetComment(superCommentId.Value);

            if(superComment is null)
                throw new ValidationException($"No comment with the id {superCommentId} was found");
        }

        return await _postRepository.CreateCommentAsync(userId, postId, text, superCommentId);
    }

    public async Task CreateReaction(Guid postId, Guid userId, PostReactions reaction)
    {
        if(!Enum.IsDefined(typeof(PostReactions), reaction))
            throw new ValidationException("Invlid reaction");

        var post = await _postRepository.GetPostAsync(postId);
        if (post is null)
            throw new ValidationException("Post not found");

        await _postRepository.CreateReactionAsync(postId, userId, reaction);
    }

    public async Task DeleteReaction(Guid postId, Guid userId)
    {
        await _postRepository.DeleteReactionAsync(postId, userId);
    }
}
