using Y.Domain.Models;

namespace Y.Infrastructure.Repositories.Interfaces;

public interface IUserProfileRepository
{
    Task<YUser?> GetUser(Guid userId);
    Task<YUser?> GetUserByUsername(string username);
    Task<YUser> CreateUser(string username, string email, string hash, string salt);
    Task UpdateUserProfile(Guid userId, string quote, string description);
    Task UpdateLastLogin(Guid userGuid, DateTime lastLogin);
    Task<(string Hash, string Salt)> GetHashAndSalt(Guid userGuid);
    Task<bool> CheckUserExists(Guid userGuid);
}
