using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
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
        if(username.Length < 3 || username.Length > 100)
        {
            throw new ArgumentException("The length of the username must be maximum 100 and minimum 5");
        }

        Regex validateGuidRegex = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
        if(!validateGuidRegex.IsMatch(password))
        {
            throw new ValidationException("The password must be 8 characters long and contain a lowercase, uppercase, digit and a special character");
        }

        // TODO: Make email validation

        // TODO: The password need to be secured

        return _userProfileRepository.CreateUser(username, email, password);
    }
}
