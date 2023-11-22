using System.Net.Mail;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Y.Application.Services.Interfaces;
using Y.Domain.Exceptions;
using Y.Domain.Models;
using Y.Infrastructure.Repositories.Interfaces;

namespace Y.Application.Services;

public class UserProfileService : IUserProfileService
{
    private readonly ILogger<UserProfileService> _logger;
    private readonly IUserProfileRepository _userProfileRepository;

    public UserProfileService(ILogger<UserProfileService> logger, IUserProfileRepository userProfileRepository)
    {
        _logger = logger;
        _userProfileRepository = userProfileRepository;
    }

    public Task<YUser?> GetUser(Guid userId)
    {
        _logger.LogInformation($"Getting user {userId}");

        return _userProfileRepository.GetUser(userId);
    }

    public async Task<YUser> CreateAsync(string username, string email, string password)
    {
        _logger.LogInformation($"Creating user with username {username} and email {email}");
        if (username.Length < 3 || username.Length > 100)
        {
            throw new ValidationException("The length of the username must be maximum 100 and minimum 5");
        }

        // Validate password
        Regex validateGuidRegex = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
        if (!validateGuidRegex.IsMatch(password))
        {
            throw new ValidationException("The password must be 8 characters long and contain a lowercase, uppercase, digit and a special character");
        }

        // Validate email
        
        email = email.Trim().ToLower();
        var emailValidationRegex = new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        if(!emailValidationRegex.IsMatch(email))
        {
            throw new ValidationException($"The email address {email} is not valid");
        }

        // TODO: The password need to be secured

        return await _userProfileRepository.CreateUser(username, email, password);
    }
}
