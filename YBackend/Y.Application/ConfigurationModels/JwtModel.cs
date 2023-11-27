using System.Text;

namespace Y.Application.ConfigurationModels;

public sealed class JwtModel
{
    public string Issuer { get; } = string.Empty;
    public string Audience { get; } = string.Empty;
    public string TokenKey { get; } = string.Empty;

    public JwtModel(string? tokenKey, string? issuer, string? audience)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tokenKey, nameof(tokenKey));
        ArgumentException.ThrowIfNullOrWhiteSpace(issuer, nameof(issuer));
        ArgumentException.ThrowIfNullOrWhiteSpace(audience, nameof(audience));

        TokenKey = tokenKey;
        Issuer = issuer;
        Audience = audience;
    }

    public byte[] TokenBytes => Encoding.UTF8.GetBytes(TokenKey);
}
