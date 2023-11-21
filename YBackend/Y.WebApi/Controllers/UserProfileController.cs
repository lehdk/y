using Microsoft.AspNetCore.Mvc;
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

    [HttpPost]
    [ProducesResponseType(typeof(YUser), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest data)
    {
        if (data.Username.Length < 3 || data.Username.Length > 100)
        {
            ModelState.AddModelError("Username", "The length of the username must be [5, 100]");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _userProfileService.CreateAsync(data.Username, data.Email, data.Password);

        return Ok(result);
    }
}
