using Y.Domain.Models;

namespace Y.Application.Services.Interfaces;

public interface IUserProfileService
{
    Task<YUser?> GetUser(Guid userId);
    Task<YUser?> GetUser(string username);
    Task<YUser> CreateUserAsyn(string username, string email, string password);
    Task<YUser> UpdateProfile(Guid userId, string quote, string desription);
    Task<string?> GetToken(string username, string password);
}
