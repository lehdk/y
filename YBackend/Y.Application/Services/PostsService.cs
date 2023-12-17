﻿using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using Y.Application.Services.Interfaces;
using Y.Domain.Models;
using Y.Infrastructure.Repositories.Interfaces;
using Y.Infrastructure.Tables;

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

    public Task<YPost?> GetPostAsync(Guid postId)
    {
        if(postId == Guid.Empty) 
            throw new ArgumentNullException(nameof(postId), "The post id must be defined");

        return _postRepository.GetPostAsync(postId);
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

}
