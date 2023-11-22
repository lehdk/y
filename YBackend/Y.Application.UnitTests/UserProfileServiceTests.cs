using Microsoft.Extensions.Logging;
using Moq;
using Y.Application.Services;
using Y.Application.Services.Interfaces;
using Y.Domain.Exceptions;
using Y.Infrastructure.Repositories.Interfaces;

namespace Y.Application.UnitTests;

public class UserProfileServiceTests
{
    private const string VALID_USERNAME = "TestUsername";
    private const string VALID_EMAIL = "example@test.dk";
    private const string VALID_PASSWORD = "Password1234!";

    private readonly Mock<ILogger<UserProfileService>> _loggerMock = new();
    private readonly Mock<IUserProfileRepository> _userProfileRepositoryMock = new();

    private IUserProfileService GetService() => new UserProfileService(_loggerMock.Object, _userProfileRepositoryMock.Object);

    [Theory]
    [InlineData("u")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
    public async void CreateAsync_IncorrectUsername_ValidationException(string username)
    {
        // Arrange
        
        var sut = GetService();

        // Act Assert

        await Assert.ThrowsAsync<ValidationException>(() => sut.CreateAsync(username, VALID_EMAIL, VALID_PASSWORD));
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalidemail.dk")]
    [InlineData("invalidemail@dk")]
    [InlineData("@y.dk")]
    [InlineData("example@test.")]
    [InlineData("example@.dk")]
    public async void CreateAsync_InvalidEmail_ThrowsValidationException(string email)
    {
        // Arrange

        var sut = GetService();

        // Act Assert

        await Assert.ThrowsAsync<ValidationException>(() => sut.CreateAsync(VALID_USERNAME, email, VALID_PASSWORD));
    }
}