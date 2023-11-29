using Y.Domain.Models;

namespace Y.Application.Services.Interfaces;

public interface IUserProfileService
{
    Task<YUser?> GetUser(Guid userId);
    Task<YUser> CreateAsync(string username, string email, string password);
    Task<string?> GetToken(string username, string password);
}
