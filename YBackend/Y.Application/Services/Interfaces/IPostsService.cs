﻿using Y.Domain.Models;

namespace Y.Application.Services.Interfaces;

public interface IPostsService
{
    public Task<List<YPost>> GetPostsAsync(Guid? userId, int page, int pageSize);
    public Task<YPost> CreatePostAsync(string headline, string content, Guid userId);
}
