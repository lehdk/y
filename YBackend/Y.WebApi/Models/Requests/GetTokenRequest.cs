namespace Y.WebApi.Models.Requests;

public sealed class GetTokenRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
