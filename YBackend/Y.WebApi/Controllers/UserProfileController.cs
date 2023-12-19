using Microsoft.AspNetCore.Mvc;
using Y.Application.Services.Interfaces;
using Y.Domain.Models;
using Y.WebApi.Models.Requests;
using Y.WebApi.Models.Responses;

namespace Y.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public class UserProfileController : ControllerBase
{
    private readonly IUserProfileService _userProfileService;

    public UserProfileController(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    [HttpGet("{username}")]
    [ProducesResponseType(typeof(YUser), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    // TODO: Should only return self
    public async Task<IActionResult> GetUser(string username)
    {
        var result = await _userProfileService.GetUser(username);

        return Ok(result);
    }

    [HttpGet("id/{userId:guid}")]
    [ProducesResponseType(typeof(YUser), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUser(Guid userId)
    {
        var result = await _userProfileService.GetUser(userId);

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(YUser), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest data)
    {
        var result = await _userProfileService.CreateAsync(data.Username, data.Email, data.Password);

        return Ok(result);
    }

    [HttpPost("token")]
    [ProducesResponseType(typeof(GetTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetToken([FromBody] GetTokenRequest request)
    {
        var token = await _userProfileService.GetToken(request.Username, request.Password);

        if(token is null)
            return Unauthorized("Incorrect username or password");

        var response = new GetTokenResponse(token);

        return Ok(response);
    }
}