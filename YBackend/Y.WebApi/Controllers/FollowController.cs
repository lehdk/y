using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Y.Application.Services.Interfaces;
using Y.Domain.Exceptions;
using Y.Domain.Models;

namespace Y.WebApi.Controllers;

[Route("api/user/{userId:guid}/[controller]")]
[ApiController]
[Authorize]
public class FollowController : ControllerBase
{
    private readonly IFollowService _followService;
    private readonly IUserProfileService _userProfileService;

    public FollowController(IFollowService followService, IUserProfileService userProfileService)
    {
        _followService = followService;
        _userProfileService = userProfileService;
    }

    [HttpGet]
    public IAsyncEnumerable<YFollow> GetUserFollows(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ValidationException("Invalid userId");

        return _followService.WhoDoesUserFollow(userId);
    }

    [HttpPost]
    public async Task Follow(Guid userId)
    {
        var loggedIn = await GetUser(HttpContext);

        await _followService.Follow(loggedIn.Guid, userId);
    }

    [HttpDelete]
    public async Task Unfollow(Guid userId)
    {
        var loggedIn = await GetUser(HttpContext);

        await _followService.Unfollow(loggedIn.Guid, userId);
    }

    private async Task<YUser> GetUser(HttpContext httpContext)
    {
        var username = httpContext.User.FindFirst(ClaimTypes.Name)?.Value;

        if (username is null)
            throw new UnauthorizedAccessException();

        var user = await _userProfileService.GetUser(username);

        if (user is null)
            throw new UnauthorizedAccessException();

        return user;
    }

}
