using Microsoft.EntityFrameworkCore;
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

    public async Task<YPost?> GetPostAsync(Guid postId)
    {
        var post = await _context.Posts.FindAsync(postId);
    
        return post?.Parse() ?? null;
    }

    public async IAsyncEnumerable<YPost> GetPosts(Guid? userId, int page, int pageSize)
    {
        var query = _context.Posts.AsQueryable();

        if (userId is not null)
            query = query.Where(x => x.UserId == userId);

        query = query.OrderBy(x => x.CreatedAt);

        var data = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        foreach (var post in data)
        {
            yield return post.Parse();
        }
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

    public async Task<YPostComment?> GetCommentAsync(Guid commentId)
    {
        var comment = await _context.PostComments.FindAsync(commentId);

        return comment?.Parse() ?? null;
    }

    public async Task<YPostComment> CreateCommentAsync(Guid userId, Guid postId, string text, Guid? superComment)
    {
        var comment = await _context.PostComments.AddAsync(new PostComments
        {
            UserId = userId,
            PostId = postId,
            SuperCommentId = null,
            Text = text,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        var parsed = comment.Entity.Parse();

        return parsed;
    }
}
