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

    [HttpGet]
    [ProducesResponseType(typeof(List<YPost>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPosts(Guid? userId = null, int page = 1, int pageSize = 10)
    {
        var result = await _postService.GetPostsAsync(userId, page, pageSize);

        return Ok(result);
    }

    [HttpGet("{postId:guid}")]
    [ProducesResponseType(typeof(YPost), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPostById(Guid postId)
    {
        var result = await _postService.GetPostAsync(postId);

        if (result is null)
            return NotFound();

        return Ok(result);
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

    [HttpPost("{postId:guid}/comment")]
    [ProducesResponseType(typeof(YPostComment), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateCommentOnPost([FromBody] CreateCommentOnPostRequest data, Guid postId)
    {
        if (data.Text.Length < 1 || data.Text.Length > 250)
            ModelState.AddModelError(nameof(data.Text), "The text must be in the range of [1, 250]");

        if (postId == Guid.Empty)
            ModelState.AddModelError(nameof(postId), "The post id must be set");
                
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await GetUser(HttpContext);

        var result = await _postService.CreateCommentAsync(user.Guid, postId, data.Text, null);

        if (result is null)
            throw new Exception("Could not add the comment");

        return Ok(result);
    }

    [HttpPost("{postId:guid}/comment/{superCommentId:guid}")]
    [ProducesResponseType(typeof(YPostComment), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateCommentOnPostComment([FromBody] CreateCommentOnPostRequest data, Guid postId, Guid superCommentId)
    {
        if (data.Text.Length < 1 || data.Text.Length > 250)
            ModelState.AddModelError(nameof(data.Text), "The text must be in the range of [1, 250]");

        if (postId == Guid.Empty)
            ModelState.AddModelError(nameof(postId), "The post id must be set");

        if (postId == Guid.Empty)
            ModelState.AddModelError(nameof(postId), "The super comment id must be set");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await GetUser(HttpContext);

        var result = await _postService.CreateCommentAsync(user.Guid, postId, data.Text, superCommentId);

        if (result is null)
            throw new Exception("Could not add the comment");

        return Ok(result);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpPost("{postId:guid}/reaction")]
    public async Task<IActionResult> CreateReaction([FromBody] CreateReactionPostRequest data, Guid postId)
    {
        var user = await GetUser(HttpContext);

        ArgumentNullException.ThrowIfNull(data);

        await _postService.CreateReaction(postId, user.Guid, data.PostReaction);

        return NoContent();
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpDelete("{postId:guid}/reaction")]
    public async Task<IActionResult> DeleteReaction(Guid postId)
    {
        var user = await GetUser(HttpContext);

        await _postService.DeleteReaction(postId, user.Guid);

        return NoContent();
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
