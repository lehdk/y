namespace Y.Domain.Models;

public sealed class YFollow
{
    public Guid Follower {  get; set; }

    public Guid Follows { get; set; }

    public DateTime CreatedAt { get; set; }
}
