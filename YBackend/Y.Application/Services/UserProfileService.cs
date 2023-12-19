using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Y.Application.ConfigurationModels;
using Y.Application.Services.Interfaces;
using Y.Domain.Exceptions;
using Y.Domain.Models;
using Y.Infrastructure.Repositories.Interfaces;

namespace Y.Application.Services;

public class UserProfileService : IUserProfileService
{
    private readonly ILogger<UserProfileService> _logger;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IArgon2idPasswordHashAlgorithm _hashing;
    private readonly JwtModel _jwtModel;

    public UserProfileService(
        ILogger<UserProfileService> logger,
        IUserProfileRepository userProfileRepository,
        IArgon2idPasswordHashAlgorithm hashing,
        JwtModel jwtModel
        )
    {
        _logger = logger;
        _userProfileRepository = userProfileRepository;
        _hashing = hashing;
        _jwtModel = jwtModel;
    }

    public Task<YUser?> GetUser(Guid userId)
    {
        _logger.LogInformation("Getting user {userId}", userId);

        return _userProfileRepository.GetUser(userId);
    }

    public Task<YUser?> GetUser(string username)
    {
        _logger.LogInformation("Getting user {username}", username);

        return _userProfileRepository.GetUserByUsername(username);    
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
        if (!emailValidationRegex.IsMatch(email))
        {
            throw new ValidationException($"The email address {email} is not valid");
        }

        var salt = _hashing.GenerateSalt();
        var hash = _hashing.HashPassword(password, salt);

        return await _userProfileRepository.CreateUser(username, email, hash, salt);
    }

    public async Task<string?> GetToken(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Incorrect username");

        var user = await _userProfileRepository.GetUserByUsername(username);

        if (user is null)
            return null;

        var hashAndSalt = await _userProfileRepository.GetHashAndSalt(user.Guid);

        var isCorrectPassword = _hashing.VerifyHashedPassword(password, hashAndSalt.Hash, hashAndSalt.Salt);

        if (!isCorrectPassword)
        {
            return null;
        }

        var lastLogin = DateTime.UtcNow;

        await _userProfileRepository.UpdateLastLogin(user.Guid, lastLogin);

        user.LastLogin = lastLogin;

        var jwt = GenerateToken(user);

        return jwt;
    }

    private string GenerateToken(YUser user)
    {
        try
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var key = new SymmetricSecurityKey(_jwtModel.TokenBytes);

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                issuer: _jwtModel.Issuer,
                audience: _jwtModel.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error generating token");
            throw;
        }
    }

    private TokenValidationParameters GetValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtModel.Issuer,
            ValidAudience = _jwtModel.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(_jwtModel.TokenBytes)
        };
    }
}
