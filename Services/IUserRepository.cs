
using IdentityServer.Entities;

namespace IdentityServer.Services
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetUsersAsync();

        Task<User> GetUserAsync(Guid userId);

        Task<User> GetUserAsync(string? username, string? email);

        Task<bool> UserExistsAsync(string username, string email);

        Task AddNewUser(User user);

        Task UpdatePasswordAsync(Guid userId, string hashedPassword);



    }
}
