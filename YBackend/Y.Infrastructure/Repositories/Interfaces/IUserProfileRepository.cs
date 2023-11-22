using Y.Domain.Models;

namespace Y.Infrastructure.Repositories.Interfaces;

public interface IUserProfileRepository
{
    Task<YUser?> GetUser(Guid userId);
    Task<YUser> CreateUser(string username, string email, string hash, string salt);
}
