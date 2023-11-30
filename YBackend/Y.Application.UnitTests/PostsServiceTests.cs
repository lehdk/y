using Microsoft.Extensions.Logging;
using Moq;
using Y.Application.Services;
using Y.Application.Services.Interfaces;
using Y.Domain.Models;
using Y.Infrastructure.Repositories.Interfaces;

namespace Y.Application.UnitTests;

public class PostsServiceTests
{
    private const string VALID_HEADLINE = "Headline";
    private const string VALID_CONTENT = "Content";
    private readonly Guid VALID_USER_GUID = Guid.NewGuid();

    private readonly Mock<ILogger<PostsService>> _loggerMock = new();
    private readonly Mock<IPostRepository> _postRepositoryMock = new();

    private IPostsService GetService() => new PostsService(
        _loggerMock.Object,
        _postRepositoryMock.Object
    );

    #region CreatePostAsync

    [Fact]
    public async void CreatePostAsync_ThrowArgumentException_NoHeadline()
    {
        // Arrange

        var sut = GetService();

        // Act Assert

        await Assert.ThrowsAsync<ArgumentException>(() => sut.CreatePostAsync("", VALID_CONTENT, VALID_USER_GUID));
    }

    [Fact]
    public async void CreatePostAsync_ThrowArgumentException_TooLongHeadline()
    {
        // Arrange

        var sut = GetService();

        string tooLongHeadline = new string('a', 76);

        // Act Assert

        await Assert.ThrowsAsync<ArgumentException>(() => sut.CreatePostAsync(tooLongHeadline, VALID_CONTENT, VALID_USER_GUID));
    }

    [Fact]
    public async void CreatePostAsync_ThrowArgumentException_NoContent()
    {
        // Arrange

        var sut = GetService();

        // Act Assert

        await Assert.ThrowsAsync<ArgumentException>(() => sut.CreatePostAsync(VALID_HEADLINE, "", VALID_USER_GUID));
    }

    [Fact]
    public async void CreatePostAsync_ThrowArgumentException_TooLongContent()
    {
        // Arrange

        var sut = GetService();

        string tooLongContent = new string('a', 1001);

        // Act Assert

        await Assert.ThrowsAsync<ArgumentException>(() => sut.CreatePostAsync(VALID_HEADLINE, tooLongContent, VALID_USER_GUID));
    }

    [Fact]
    public async void CreatePostAsync_ThrowArgumentException_InvalidUserGuid()
    {
        // Arrange

        var sut = GetService();

        Guid invalidEmptyGuid = new Guid();

        // Act Assert

        await Assert.ThrowsAsync<ArgumentException>(() => sut.CreatePostAsync(VALID_HEADLINE, VALID_CONTENT, invalidEmptyGuid));
    }

    [Fact]
    public async void CreatePostAsync_ReturnCorrectData_ValidInput()
    {
        // Arrange

        var toReturn = new YPost
        {
            Id = Guid.NewGuid(),
            UserId = VALID_USER_GUID,
            Headline = VALID_HEADLINE,
            Content = VALID_CONTENT,
            CreatedAt = DateTime.UtcNow
        };

        _postRepositoryMock.Setup(x => x.CreatePostAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(toReturn);

        var sut = GetService();

        // Act

        var result = await sut.CreatePostAsync(VALID_HEADLINE, VALID_CONTENT, VALID_USER_GUID);

        // Assert

        Assert.Equal(toReturn, result);
    }

    #endregion
}
