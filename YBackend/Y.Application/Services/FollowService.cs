using Y.Application.Services.Interfaces;
using Y.Domain.Exceptions;
using Y.Domain.Models;
using Y.Infrastructure.Repositories.Interfaces;

namespace Y.Application.Services;

public class FollowService : IFollowService
{
    private readonly IFollowRepository _followRepository;
    private readonly IUserProfileRepository _userProfileRepository;

    public FollowService(IFollowRepository followRepository, IUserProfileRepository userProfileRepository)
    {
        _followRepository = followRepository;
        _userProfileRepository = userProfileRepository;
    }

    public async Task Follow(Guid follower, Guid follows)
    {
        if (follower == Guid.Empty)
            throw new ArgumentNullException("Invalid follower");
        
        if (follows == Guid.Empty)
            throw new ArgumentNullException("Invalid user to follow");

        if (follower == follows)
            throw new ValidationException("You can not follow yourself");

        var followsExist = await _userProfileRepository.CheckUserExists(follows);

        if(!followsExist)
            throw new ValidationException("The user you were trying to follow does not exists");  

        await _followRepository.Follow(follower, follows);
    }

    public async Task Unfollow(Guid follower, Guid follows)
    {
        if (follower == Guid.Empty)
            throw new ArgumentNullException("Invalid follower");

        if (follows == Guid.Empty)
            throw new ArgumentNullException("Invalid user to follow");

        await _followRepository.Unfollow(follower, follows);
    }

    public IAsyncEnumerable<YFollow> WhoDoesUserFollow(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ValidationException("Invalid userId");

        return _followRepository.WhoDoesUserFollow(userId);
    }
}
