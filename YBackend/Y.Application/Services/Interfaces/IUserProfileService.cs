using Y.Domain.Models;

namespace Y.Application.Services.Interfaces;

public interface IUserProfileService
{

    Task<YUser> CreateAsync(string username, string email, string password);

}
