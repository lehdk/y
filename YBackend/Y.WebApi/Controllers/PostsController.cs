using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Y.Application.Services.Interfaces;
using Y.Domain.Models;
using Y.WebApi.Models.Requests;

namespace Y.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PostsController : ControllerBase
{
    private readonly IUserProfileService _userProfileService;
    private readonly IPostsService _postService;

    public PostsController(IUserProfileService userProfileService, IPostsService postsService)
    {
        _userProfileService = userProfileService;
        _postService = postsService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(YPost), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest data)
    {
        if (data.Headline.Length < 1 || data.Headline.Length > 75)
            ModelState.AddModelError(nameof(data.Headline), "The headline must have a length of [1, 75] characters");

        if(data.Content.Length < 1 || data.Headline.Length > 1000)
            ModelState.AddModelError(nameof(data.Content), "The content must have a length of [1, 1000] characters");

        if(!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await GetUser(HttpContext);

        var post = await _postService.CreatePostAsync(data.Headline, data.Content, user.Guid);

        if (post is null)
            throw new Exception("Error creating post!");

        return Ok(post);
    }

    private async Task<YUser> GetUser(HttpContext httpContext)
    {
        var username = httpContext.User.FindFirst(ClaimTypes.Name)?.Value;

        if(username is null)
            throw new UnauthorizedAccessException();

        var user = await _userProfileService.GetUser(username);

        if(user is null)
            throw new UnauthorizedAccessException();

        return user;
    }
}
