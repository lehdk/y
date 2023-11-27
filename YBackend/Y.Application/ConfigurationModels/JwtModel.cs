namespace Y.Application.ConfigurationModels;

public sealed class JwtModel
{
    public string TokenKey { get; } = string.Empty;

    public JwtModel(string? tokenKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tokenKey, nameof(tokenKey));
        TokenKey = tokenKey;
    }
}
