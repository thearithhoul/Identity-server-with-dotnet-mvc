
using IdentityServer.API;
using IdentityServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly IdpContext _idpContext;

        public UserRepository(IdpContext idpContext)
        {
            _idpContext = idpContext;
        }

        public async Task AddNewUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var isExists = await UserExistsAsync(user.Username, user.Email);
            if (isExists)
            {
                throw new ArgumentException("Username or email already exists.");
            }

            if (user.UserId == Guid.Empty)
            {
                user.UserId = Guid.NewGuid();
            }

            if (user.CreatedAt == default)
            {
                user.CreatedAt = DateTime.UtcNow;
            }

            user.UpdatedAt = DateTime.UtcNow;

            _idpContext.Users.Add(user);
            await _idpContext.SaveChangesAsync();
        }

        public async Task<User> GetUserAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User id must be provided.");
            }

            var user = await _idpContext.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                throw new KeyNotFoundException($"User not found for id {userId}.");
            }

            return user;
        }

        public async Task<User> GetUserAsync(string? username, string? email)
        {
            if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Username or email must be provided.");
            }

            User? user = null;
            if (!string.IsNullOrWhiteSpace(username))
            {
                user = await _idpContext.Users
                    .AsNoTracking()
                    .SingleOrDefaultAsync(u => u.Username == username);
            }

            if (user == null && !string.IsNullOrWhiteSpace(email))
            {
                user = await _idpContext.Users
                    .AsNoTracking()
                    .SingleOrDefaultAsync(u => u.Email == email);
            }

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            return user;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _idpContext.Users
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task UpdatePasswordAsync(Guid userId, string hashedPassword)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User id must be provided.");
            }

            if (string.IsNullOrWhiteSpace(hashedPassword))
            {
                throw new ArgumentException("Hashed password must be provided.");
            }

            var user = await _idpContext.Users
                .SingleOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                throw new KeyNotFoundException($"User not found for id {userId}.");
            }

            user.EncryptedPassword = hashedPassword;
            user.UpdatedAt = DateTime.UtcNow;
            await _idpContext.SaveChangesAsync();
        }
        public async Task<bool> UserExistsAsync(string username, string email)
        {
            if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Username or email must be provided.");
            }

            return await _idpContext.Users
                .AnyAsync(u => u.Username == username || u.Email == email);
        }
    }
}
