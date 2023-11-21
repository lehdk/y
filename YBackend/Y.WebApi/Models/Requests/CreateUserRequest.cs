using System.ComponentModel.DataAnnotations;

namespace Y.WebApi.Models.Requests;

public sealed class CreateUserRequest
{
    [MinLength(3)]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
