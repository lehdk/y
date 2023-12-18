using Y.Domain.Models;

namespace Y.Infrastructure.Repositories.Interfaces;

public interface IFollowRepository
{
    public IAsyncEnumerable<YFollow> WhoDoesUserFollow(Guid userId);
    public Task Follow(Guid follower, Guid follows);
    public Task Unfollow(Guid follower, Guid follows);
}
