using Microsoft.Extensions.Logging;
using Y.Domain.Models;
using Y.Infrastructure.Extensions;
using Y.Infrastructure.Repositories.Interfaces;
using Y.Infrastructure.Tables;

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

    public async Task<YUser> CreateUser(string username, string email, string password)
    {
        _logger.LogInformation($"Creating user with username: {username} and email: {email}");

        var profile = await CreateEmptyProfile();

        var user = await _context.Users.AddAsync(new User
        {
            Username = username,
            Email = email,
            Password = password,
        });

        return user.Entity.Parse(profile);
    }

    private async Task<YProfile> CreateEmptyProfile()
    {
        _logger.LogInformation("Creating empty profile");

        var profile = await _context.Profiles.AddAsync(new());
        await _context.SaveChangesAsync();

        return profile.Entity.Parse();
    }
}
