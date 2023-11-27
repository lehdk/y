using Microsoft.Extensions.Logging;
using Moq;
using Y.Application.ConfigurationModels;
using Y.Application.Services;
using Y.Application.Services.Interfaces;
using Y.Domain.Exceptions;
using Y.Domain.Models;
using Y.Infrastructure.Repositories.Interfaces;

namespace Y.Application.UnitTests;

public class UserProfileServiceTests
{
    private const string VALID_USERNAME = "TestUsername";
    private const string VALID_EMAIL = "example@test.dk";
    private const string VALID_PASSWORD = "Password1234!";

    private readonly Mock<ILogger<UserProfileService>> _loggerMock = new();
    private readonly Mock<IUserProfileRepository> _userProfileRepositoryMock = new();
    private readonly Mock<IArgon2idPasswordHashAlgorithm> _passwordHasherMock = new();

    private IUserProfileService GetService() => new UserProfileService(
        _loggerMock.Object,
        _userProfileRepositoryMock.Object,
        _passwordHasherMock.Object,
        new JwtModel("sjtydgfxfgfvresd")
    );

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

    [Fact]
    public async void CreateAsync_ValidData_ReturnsUser()
    {
        // Arrange

        var sut = GetService();

        var profile = new YProfile();

        var user = new YUser(profile)
        {
            Guid = Guid.NewGuid(),
            Username = VALID_USERNAME,
            Email = VALID_EMAIL,
        };

        _userProfileRepositoryMock.Setup(x => x.CreateUser(VALID_USERNAME, VALID_EMAIL, It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(user);

        // Act

        var result = await sut.CreateAsync(VALID_USERNAME, VALID_EMAIL, VALID_PASSWORD);

        // Assert

        Assert.Equal(user, result);
    }
}