using Microsoft.Extensions.Logging;
using Y.Application.Services.Interfaces;
using Y.Domain.Models;
using Y.Infrastructure.Repositories.Interfaces;

namespace Y.Application.Services;

public class UserProfileService: IUserProfileService
{
    private readonly ILogger<UserProfileService> _logger;
    private readonly IUserProfileRepository _userProfileRepository;

    public UserProfileService(ILogger<UserProfileService> logger, IUserProfileRepository userProfileRepository)
    {
        _logger = logger;
        _userProfileRepository = userProfileRepository;
    }

    public Task<YUser> CreateAsync(string username, string email, string password)
    {
        _logger.LogInformation($"Creating user with username {username} and email {email}");
        if(username.Length <= 5 || username.Length > 100)
        {
            throw new ArgumentException("The length of the username must be maximum 100 and minimum 5");
        }

        // TODO: Make regex for password

        // TODO: Make email validation

        return _userProfileRepository.CreateUser(username, email, password);
    }
}
