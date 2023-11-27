using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Y.Application.Services.Interfaces;
using Y.Domain.Models;
using Y.WebApi.Models.Requests;

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

    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(YUser), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    // TODO: Should only return self
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

    [HttpPost("{userId:guid}/token")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetToken(Guid userId, [FromBody] string password)
    {
        var token = await _userProfileService.GetToken(userId, password);

        if(token is null)
            return Unauthorized();

        return Ok(token);
    }
}
