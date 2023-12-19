using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Y.Domain.Models;
using Y.Infrastructure.Extensions;
using Y.Infrastructure.Models;
using Y.Infrastructure.Repositories.Interfaces;
using Y.Infrastructure.Tables;

namespace Y.Infrastructure.Repositories;

public class UserRepository : IUserProfileRepository
{
    private readonly ILogger<UserRepository> _logger;
    private readonly string _connectionString;

    private SqlConnection GetSqlConnection => new(_connectionString);

    public UserRepository(ILogger<UserRepository> logger, ConnectionStringModel connectionStringModel)
    {
        _logger = logger;
        _connectionString = connectionStringModel.ConnectionString;
    }

    public async Task<YUser> CreateUser(string username, string email, string hash, string salt)
    {
        _logger.LogInformation("Creating user with username: {username} and email: {email}", username, email);

        var profile = await CreateEmptyProfile();

        Guid? insertedUserId = null;

        using (var connection = GetSqlConnection)
        {
            await connection.OpenAsync();

            const string query = @"
INSERT INTO [User]
([Guid], [Username], [Email], [Password], [ProfileId])
OUTPUT INSERTED.Guid
VALUES (@Guid, @Username, @Email, @Password, @ProfileId);
";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Guid", Guid.NewGuid());
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", hash);
                command.Parameters.AddWithValue("@ProfileId", profile.Guid);

                var reader = await command.ExecuteReaderAsync();

                if (reader.HasRows && await reader.ReadAsync())
                {
                    insertedUserId = reader.GetGuid(0);
                }
            }

            await connection.CloseAsync();
        }

        if (insertedUserId is null)
            throw new Exception("Could not create user");

        await AddSaltEntryForUser(insertedUserId.Value, salt);

        var result = await GetUser(insertedUserId.Value);

        if (result is null)
            throw new Exception("Could not create user");

        return result;
    }

    public async Task UpdateUserProfile(Guid profileId, string quote, string description)
    {
        using (var connection = GetSqlConnection)
        {
            await connection.OpenAsync();

            const string query = @"
UPDATE [Profile] SET [Quote] = @Quote, [Description] = @Description WHERE [Guid] = @ProfileId;
";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Quote", quote);
                command.Parameters.AddWithValue("@Description", description);
                command.Parameters.AddWithValue("@ProfileId", profileId);

                int affectedRows = await command.ExecuteNonQueryAsync();

                if(affectedRows != 1)
                {
                    var exception = new Exception("An error occurred while updaint profile");
                    _logger.LogError(exception, "An error occurred while updating profile on user {userId} with quote {quote} and description {description}", profileId, quote, description);
                    throw exception;
                }
            }

            await connection.CloseAsync();
        }
    }

    public async Task<(string Hash, string Salt)> GetHashAndSalt(Guid userGuid)
    {
        string? hashResult = null;
        string? saltResult = null;

        var user = await GetInfrastructureUser(userGuid);

        if (user is null)
            throw new Exception($"Could not get user {userGuid}");

        hashResult = user.Password;

        using (var connection = GetSqlConnection)
        {
            await connection.OpenAsync();

            const string query = @"
SELECT [Salt] FROM [PasswordSalt] WHERE [UserId] = @UserId;
";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserId", userGuid);

                var reader = await command.ExecuteReaderAsync();

                if (reader.HasRows && await reader.ReadAsync())
                {
                    saltResult = reader.GetString(0);
                }
            }
        }

        if (hashResult is null || saltResult is null)
            throw new Exception($"Could not get hash or salt for user {userGuid}");

        return (hashResult, saltResult);
    }

    public async Task<YUser?> GetUser(Guid userId)
    {
        var result = await GetInfrastructureUser(userId);

        return result?.Parse(await GetUserProfile(result.ProfileId));
    }

    private async Task<User?> GetInfrastructureUser(Guid userId)
    {
        User? result = null;

        using (var connection = GetSqlConnection)
        {
            await connection.OpenAsync();

            const string query = @"
SELECT [Guid]
      ,[Username]
      ,[Email]
      ,[Password]
      ,[CreatedAt]
      ,[LastLogin]
      ,[ProfileId]
FROM [User]
WHERE [Guid] = @UserId;
";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserId", userId);

                var reader = await command.ExecuteReaderAsync();

                if (reader.HasRows && await reader.ReadAsync())
                {
                    DateTime? lastLogin = null;

                    if (!await reader.IsDBNullAsync(5))
                        lastLogin = reader.GetDateTime(5);

                    result = new() {
                        Guid = reader.GetGuid(0),
                        Username = reader.GetString(1),
                        Email = reader.GetString(2),
                        Password = reader.GetString(3),
                        CreatedAt = reader.GetDateTime(4),
                        LastLogin = lastLogin,
                        ProfileId = reader.GetGuid(6),
                    };
                }
            }
        }

        return result;
    }

    private async Task<YProfile?> GetUserProfile(Guid profileId)
    {
        Profile? result = null;

        using (var connection = GetSqlConnection)
        {
            await connection.OpenAsync();

            const string query = @"
SELECT [Guid]
      ,[Quote]
      ,[Description]
FROM [Profile]
WHERE [Guid] = @ProfileId;
";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ProfileId", profileId);

                var reader = await command.ExecuteReaderAsync();

                if (reader.HasRows && await reader.ReadAsync())
                {
                    result = new() {
                        Guid = reader.GetGuid(0),
                        Quote = reader.GetString(1),
                        Description = reader.GetString(2),
                    };
                }
            }
        }

        return result?.Parse();
    }

    public async Task<YUser?> GetUserByUsername(string username)
    {
        User? result = null;

        using (var connection = GetSqlConnection)
        {
            await connection.OpenAsync();

            const string query = @"
SELECT [Guid]
      ,[Username]
      ,[Email]
      ,[Password]
      ,[CreatedAt]
      ,[LastLogin]
      ,[ProfileId]
FROM [User]
WHERE [Username] = @Username;
";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);

                var reader = await command.ExecuteReaderAsync();

                if (reader.HasRows && await reader.ReadAsync())
                {
                    DateTime? lastLogin = null;

                    if (!await reader.IsDBNullAsync(5))
                        lastLogin = reader.GetDateTime(5);

                    result = new() {
                        Guid = reader.GetGuid(0),
                        Username = reader.GetString(1),
                        Email = reader.GetString(2),
                        Password = reader.GetString(3),
                        CreatedAt = reader.GetDateTime(4),
                        LastLogin = lastLogin,
                        ProfileId = reader.GetGuid(6),
                    };
                }
            }

            await connection.CloseAsync();
        }

        return result?.Parse(await GetUserProfile(result.ProfileId));
    }

    public async Task UpdateLastLogin(Guid userGuid, DateTime lastLogin)
    {
        using (var connection = GetSqlConnection)
        {
            await connection.OpenAsync();

            const string query = @"
UPDATE [User] SET [LastLogin] = @LastLogin WHERE [Guid] = @UserId
";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@LastLogin", lastLogin);
                command.Parameters.AddWithValue("@UserId", userGuid);

                int rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected < 0)
                {
                    _logger.LogError("Could not update last login for user {userId} to value {lastLogin}", userGuid, lastLogin);
                }
            }

            await connection.CloseAsync();
        }
    }

    private async Task AddSaltEntryForUser(Guid userGuid, string salt)
    {
        int? rowsAffected = null;

        using (var connection = GetSqlConnection)
        {
            await connection.OpenAsync();

            const string query = @"
INSERT INTO [PasswordSalt]
([UserId], [Salt])
VALUES
(@UserId, @Salt);
";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserId", userGuid);
                command.Parameters.AddWithValue("@Salt", salt);

                rowsAffected = await command.ExecuteNonQueryAsync();
            }

            await connection.CloseAsync();
        }

        if (rowsAffected is null || rowsAffected != 1)
            throw new Exception($"Could not create salt entry for user {userGuid}");
    }

    private async Task<Profile> CreateEmptyProfile()
    {

        Guid? insertedId = null;

        using (var connection = GetSqlConnection)
        {
            await connection.OpenAsync();

            const string query = @"
INSERT INTO [Profile]
([Guid], [Quote], [Description])
OUTPUT INSERTED.Guid
VALUES (@Guid, @Quote, @Description);
";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Guid", Guid.NewGuid());
                command.Parameters.AddWithValue("@Quote", "");
                command.Parameters.AddWithValue("@Description", "");

                var reader = await command.ExecuteReaderAsync();

                if (reader.HasRows && await reader.ReadAsync())
                {
                    insertedId = reader.GetGuid(0);
                }
            }

            await connection.CloseAsync();
        }

        if (insertedId is null)
            throw new Exception("Could not create empty profile");

        return new Profile {
            Guid = insertedId.Value,
            Quote = string.Empty,
            Description = string.Empty
        };
    }

    public async Task<bool> CheckUserExists(Guid userGuid)
    {
        bool exists = false;

        using (var connection = GetSqlConnection)
        {
            await connection.OpenAsync();

            string query = @"
SELECT 1 FROM [User] WHERE [Guid] = @UserId;
";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserId", userGuid);

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