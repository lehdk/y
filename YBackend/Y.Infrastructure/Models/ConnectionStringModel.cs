namespace Y.Infrastructure.Models;

public sealed class ConnectionStringModel
{
    public string ConnectionString { get; set; } = string.Empty;

    public ConnectionStringModel(string? connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString, nameof(connectionString));

        ConnectionString = connectionString;
    }
}
