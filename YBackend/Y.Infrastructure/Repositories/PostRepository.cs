using Microsoft.Extensions.Logging;
using Y.Domain.Models;
using Y.Infrastructure.Extensions;
using Y.Infrastructure.Repositories.Interfaces;
using Y.Infrastructure.Tables;

namespace Y.Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
    private readonly ILogger<PostRepository> _logger;
    private readonly DatabaseContext _context;

    public PostRepository(ILogger<PostRepository> logger, DatabaseContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<YPost> CreatePostAsync(string headline, string content, Guid userId)
    {
        var post = await _context.Posts.AddAsync(new Posts()
        {
            UserId = userId,
            Headline = headline,
            Content = content,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        var parsed = post.Entity.Parse();

        return parsed;
    }
}
