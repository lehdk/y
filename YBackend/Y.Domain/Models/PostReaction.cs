namespace Y.Domain.Models;

public sealed class PostReaction
{
    public PostReactions Reaction { get; set; }
}

public enum PostReactions
{
    Like = 0,
    Dislike = 1,
}
