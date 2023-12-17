using System.ComponentModel.DataAnnotations;

namespace Y.Domain.Models;

public class YPost
{
    public Guid Id { get; set; }

    [MaxLength(75)]
    public string Headline { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Content { get; set; } = string.Empty;

    public Guid UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<PostReactionPair> Reactions { get; set; } = new();

    public List<YPostComment> PostComments { get; set; } = new();
}

public class PostReactionPair
{
    public PostReactions Reaction { get; set; }
    public string Username { get; set; } = string.Empty;
}
