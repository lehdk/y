using Y.Domain.Models;

namespace Y.Infrastructure.Repositories.Interfaces;

public interface IUserProfileRepository
{
    Task<YUser> CreateUser(string username, string email, string password);
}
