using Microsoft.Extensions.Logging;
using Y.Domain.Models;
using Y.Infrastructure.Extensions;
using Y.Infrastructure.Repositories.Interfaces;
using Y.Infrastructure.Tables;
using Y.Domain.Exceptions;

namespace Y.Infrastructure.Repositories;

public class UserProfileRepository : IUserProfileRepository
{
    private readonly ILogger<UserProfileRepository> _logger;
    private readonly DatabaseContext _context;

    public UserProfileRepository(ILogger<UserProfileRepository> logger, DatabaseContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<YUser?> GetUser(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user is null)
            return null;

        var dbProfile = await _context.Profiles.FindAsync(user.ProfileId);

        if(dbProfile is null)
        {
            _logger.LogError("Could not find profile for user {userId}", userId);
            return null;
        }

        return user.Parse(dbProfile.Parse());
    }

    public async Task<YUser> CreateUser(string username, string email, string hash, string salt)
    {
        _logger.LogInformation("Creating user with username: {username} and email: {email}", username, email);

        var profile = await CreateEmptyProfile();

        var user = await _context.Users.AddAsync(new User
        {
            Username = username,
            Email = email,
            Password = hash,
            ProfileId = profile.Guid,
            CreatedAt = DateTime.UtcNow,
        });

        await _context.PasswordSalts.AddAsync(new PasswordSalts()
        {
            UserId = user.Entity.Guid,
            Salt = salt,
        });
        await _context.SaveChangesAsync();

        return user.Entity.Parse(profile);
    }

    private async Task<YProfile> CreateEmptyProfile()
    {
        _logger.LogInformation("Creating empty profile");

        var profile = await _context.Profiles.AddAsync(new());
        await _context.SaveChangesAsync();

        return profile.Entity.Parse();
    }

    public async Task<(string Hash, string Salt)> GetHashAndSalt(Guid userGuid)
    {
        _logger.LogInformation("Getting hash and salt for user {userGuid}", userGuid);

        var user = await _context.Users.FindAsync(userGuid);

        if (user is null)
            throw new UserNotFoundException();

        var salt = await _context.PasswordSalts.FindAsync(userGuid);

        if (salt is null)
        {
            var error = new Exception($"Could not find password salt for user ${userGuid}");
            _logger.LogError(error, "Could not find password salt for user {userGuid}", userGuid);
            throw error;
        }

        return (user.Password, salt.Salt);
    }
}
