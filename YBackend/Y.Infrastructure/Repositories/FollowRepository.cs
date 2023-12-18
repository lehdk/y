using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Y.Domain.Exceptions;
using Y.Domain.Models;
using Y.Infrastructure.Models;
using Y.Infrastructure.Repositories.Interfaces;

namespace Y.Infrastructure.Repositories;

public class FollowRepository : IFollowRepository
{
    private readonly ILogger<UserRepository> _logger;
    private readonly string _connectionString;

    private SqlConnection GetSqlConnection => new(_connectionString);

    public FollowRepository(ILogger<UserRepository> logger, ConnectionStringModel connectionStringModel)
    {
        _logger = logger;
        _connectionString = connectionStringModel.ConnectionString;
    }

    public async IAsyncEnumerable<YFollow> WhoDoesUserFollow(Guid userId)
    {
        using (var connection = GetSqlConnection)
        {
            await connection.OpenAsync();

            const string query = @"
SELECT 
     [Follower]
    ,[Follows]
    ,[CreatedAt]
FROM 
    [Follow]
WHERE
    [Follower] = @Follower;
";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Follower", userId);

                using (var reader = command.ExecuteReader())
                {
                    while (await reader.ReadAsync())
                    {
                        yield return new()
                        {
                            Follower = reader.GetGuid(0),
                            Follows = reader.GetGuid(1),
                            CreatedAt = reader.GetDateTime(2)
                        };
                    }
                }
            }

            await connection.CloseAsync();
        }
    }

    public async Task Follow(Guid follower, Guid follows)
    {
        if(await DoesAlreadyFollow(follower, follows))
            throw new ValidationException("You already follow this person");

        using(var connection = GetSqlConnection)
        {
            await connection.OpenAsync();

            const string query = @"
INSERT INTO [Follow] ([Follower], [Follows]) VALUES (@Follower, @Follows);
";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Follower", follower);
                command.Parameters.AddWithValue("@Follows", follows);
            
                int affectedRows = await command.ExecuteNonQueryAsync();

                if (affectedRows != 1)
                {
                    var exception = new Exception("Error trying to follow");
                    _logger.LogError("Error occurred while user {userId} was trying to follow {userId}", follower, follows);
                    throw exception;
                }
            }

            await connection.CloseAsync();
        }
    }

    public async Task Unfollow(Guid follower, Guid follows)
    {
        if (!await DoesAlreadyFollow(follower, follows))
            throw new ValidationException("You already follow this person");

        using (var connection = GetSqlConnection)
        {
            await connection.OpenAsync();

            const string query = @"
DELETE FROM [Follow] WHERE [Follower] = @Follower AND [Follows] = @Follows;
";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Follower", follower);
                command.Parameters.AddWithValue("@Follows", follows);

                int affectedRows = await command.ExecuteNonQueryAsync();

                if (affectedRows != 1)
                {
                    var exception = new Exception("Error trying to unfollow");
                    _logger.LogError("Error occurred while user {userId} was trying to unfollow {userId}", follower, follows);
                    throw exception;
                }
            }

            await connection.CloseAsync();
        }
    }

    private async Task<bool> DoesAlreadyFollow(Guid follower, Guid follows)
    {
        bool exists = false;

        using (var connection = GetSqlConnection)
        {
            await connection.OpenAsync();

            string query = @"
SELECT 1 FROM [Follow] WHERE [Follower] = @Follower AND [Follows] = @Follows;
";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Follower", follower);
                command.Parameters.AddWithValue("@Follows", follows);

                var reader = await command.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    exists = true;
                }
            }

            await connection.CloseAsync();
        }

        return exists;
    }
}
