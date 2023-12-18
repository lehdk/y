using Y.Domain.Models;

namespace Y.Application.Services.Interfaces;

public interface IFollowService
{
    public Task Follow(Guid follower, Guid follows);
    public Task Unfollow(Guid follower, Guid follows);
    public IAsyncEnumerable<YFollow> WhoDoesUserFollow(Guid userId);
}
