using System.Runtime.InteropServices;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Y.Domain.Exceptions;
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

    public async Task<bool> HasUserMadeReaction(Guid postId, Guid userId)
    {
        bool exists = false;

        using (var connection = GetSqlConnection)
        {
            await connection.OpenAsync();

            string query = @"
SELECT 1 FROM [PostReaction] WHERE [UserId] = @UserId AND [PostId] = @PostId;
";

            using(var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@PostId", postId);
            
                var reader = await command.ExecuteReaderAsync();

                if(reader.HasRows)
                {
                    exists = true;
                }
            }

            await connection.CloseAsync();
        }

        return exists;
    }

    public async Task CreateReactionAsync(Guid postId, Guid userId, PostReactions reaction)
    {
        if (await HasUserMadeReaction(postId, userId))
        {
            throw new ValidationException("The user has already made a reaciton to this post");
        }

        using (var connection = GetSqlConnection)
        {
            await connection.OpenAsync();

            string query = @"
INSERT INTO [PostReaction] ([UserId], [PostId], [Reaction]) VALUES (@UserId, @PostId, @Reaction);
";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@PostId", postId);
                command.Parameters.AddWithValue("@Reaction", reaction);

                int rowsAffeced = await command.ExecuteNonQueryAsync();

                if (rowsAffeced != 1)
                {
                    var exception = new Exception("Error while adding reaction");
                    _logger.LogError(exception, "Error occurred while user {userId} was creating a reaction on post {postId} with reaction {reaction}", userId, postId, reaction);
                    throw exception;
                }
            }

            await connection.CloseAsync();
        }
    }

    public async Task DeleteReactionAsync(Guid postId, Guid userId)
    {
        if (!await HasUserMadeReaction(postId, userId))
        {
            throw new ValidationException("The user has not made a reaction on this post");
        }

        using (var connection = GetSqlConnection)
        {
            await connection.OpenAsync();

            string query = @"
DELETE FROM [PostReaction] WHERE [UserId] = @UserId AND [PostId] = @PostId;
";

            using(var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@PostId", postId);

                int rowsAffeced = await command.ExecuteNonQueryAsync();

                if (rowsAffeced != 1)
                {
                    var exception = new Exception("Error while deleting reaction");
                    _logger.LogError(exception, "Error occurred while user {userId} was deleting a reaction on post {postId}", userId, postId);
                    throw exception;
                }
            }

            await connection.CloseAsync();
        }
    }

    public async IAsyncEnumerable<PostReactionPair> GetReactionsForPost(Guid postId)
    {

        using (var connection = GetSqlConnection)
        {
            await connection.OpenAsync();
            const string query = @"
SELECT [U].[Username]
      ,[PR].[Reaction]
  FROM [PostReaction] AS [PR]
  JOIN [User] AS [U] ON [PR].[UserId] = [U].[Guid]
  WHERE [PostId] = @PostId;
";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@PostId", postId);

                using (var reader = command.ExecuteReader())
                {
                    while (await reader.ReadAsync())
                    {
                        PostReactions reaction = (PostReactions)reader.GetInt32(1);

                        if (!Enum.IsDefined(typeof(PostReactions), reaction))
                        {
                            _logger.LogError("Error parsing an enum PostReactions while getting post {postId}", postId);
                            continue;
                        }

                        yield return new PostReactionPair
                        {
                            Username = reader.GetString(0),
                            Reaction = reaction,
                        };
                    }
                }
            }

            await connection.CloseAsync();
        }
    }
}
