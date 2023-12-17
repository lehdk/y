using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Y.Domain.Models;
using Y.Infrastructure.Models;
using Y.Infrastructure.Repositories.Interfaces;

namespace Y.Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
    private readonly ILogger<UserRepository> _logger;
    private readonly string _connectionString;

    private SqlConnection GetSqlConnection => new(_connectionString);

    public PostRepository(ILogger<UserRepository> logger, ConnectionStringModel connectionStringModel)
    {
        _logger = logger;
        _connectionString = connectionStringModel.ConnectionString;
    }

    public async Task<YPost> CreatePostAsync(string headline, string content, Guid userId)
    {
        Guid? insertedpostId = null;

        using (var connection = GetSqlConnection)
        {
            await connection.OpenAsync();

            string query = @"
INSERT INTO [Post] 
    ([Guid], [Headline], [Content], [UserId]) 
OUTPUT INSERTED.Guid
VALUES 
    (@Guid, @Headline, @Content, @UserId);
";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Guid", Guid.NewGuid());
                command.Parameters.AddWithValue("@Headline", headline);
                command.Parameters.AddWithValue("@Content", content);
                command.Parameters.AddWithValue("@UserId", userId);

                var reader = await command.ExecuteReaderAsync();

                if (reader.HasRows && await reader.ReadAsync())
                {
                    insertedpostId = reader.GetGuid(0);
                }
            }

            await connection.CloseAsync();
        }

        if(insertedpostId is null)
        {
            var exception = new Exception("Error creating post");
            _logger.LogError(exception, "An error occurred while creating a post for uset {userId}", userId);

            throw exception;
        }

        var result = await GetPostAsync(insertedpostId.Value);

        if(result is null)
        {
            var exception = new Exception("Unreachable code");
            _logger.LogError(exception, "Error getting post just created with it {postId}", insertedpostId);
            throw exception;
        }

        return result;
    }

    public async Task<YPost?> GetPostAsync(Guid postId)
    {
        YPost? result = null;

        using (var connection = GetSqlConnection)
        {
            await connection.OpenAsync();

            string query = @"
SELECT [Guid]
      ,[Headline]
      ,[Content]
      ,[CreatedAt]
      ,[UserId]
FROM [Post]
WHERE [Guid] = @PostId;";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@PostId", postId);

                var reader = await command.ExecuteReaderAsync();

                if (reader.HasRows && await reader.ReadAsync())
                {
                    result = new()
                    {
                        Id = reader.GetGuid(0),
                        Headline = reader.GetString(1),
                        Content = reader.GetString(2),
                        CreatedAt = reader.GetDateTime(3),
                        UserId = reader.GetGuid(4),
                    };
                }
            }

            await connection.CloseAsync();    
        }

        return result;
    }

    public async IAsyncEnumerable<YPost> GetPosts(Guid? userId, int page, int pageSize)
    {

        int rowsToSkip = (page - 1) * pageSize;

        
        using (var connection = GetSqlConnection)
        {
            await connection.OpenAsync();

            string query = @"
SELECT 
     [Guid]
    ,[Headline]
    ,[Content]
    ,[CreatedAt]
    ,[UserId] 
FROM [Post] ORDER BY [CreatedAt] DESC OFFSET @RowsToSkip ROWS FETCH NEXT @PageSize ROWS ONLY;
";

            using(var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@RowsToSkip", rowsToSkip);
                command.Parameters.AddWithValue("@PageSize", pageSize);

                using(var reader = command.ExecuteReader())
                {
                    while (await reader.ReadAsync())
                    {
                        yield return new()
                        {
                            Id = reader.GetGuid(0),
                            Headline = reader.GetString(1),
                            Content = reader.GetString(2),
                            CreatedAt = reader.GetDateTime(3),
                            UserId = reader.GetGuid(4),
                        };
                    }
                }
            }

            await connection.CloseAsync();
        }
    }

    public Task<YPostComment?> GetCommentAsync(Guid commentId)
    {
        throw new NotImplementedException();
    }

    public Task<YPostComment> CreateCommentAsync(Guid userId, Guid postId, string text, Guid? superComment)
    {
        throw new NotImplementedException();
    }
}
