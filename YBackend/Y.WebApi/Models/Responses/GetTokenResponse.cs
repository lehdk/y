namespace Y.WebApi.Models.Responses;

public sealed class GetTokenResponse
{
    public string Token { get; }

    public GetTokenResponse(string token)
    {
        Token = token;
    }
}
