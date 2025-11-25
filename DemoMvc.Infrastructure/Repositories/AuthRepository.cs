using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DemoMvc.Core.Entities;
using DemoMvc.Core.Interfaces;
using DemoMvc.Infrastructure.Data;

namespace DemoMvc.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly LmsDbContext _context;

        public AuthRepository(LmsDbContext context)
        {
            _context = context;
        }

        // 1. Find user by username
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        // 2. Find user by email
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        // 3. Register a new user (hash the password!)
        public async Task<User> RegisterUserAsync(User user, string password)
        {
            // Hash the password using BCrypt
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        // 4. Validate password (compare plain password with hash)
        public async Task<bool> ValidatePasswordAsync(User user, string password)
        {
            // BCrypt.Verify compares the plain password with the hash
            return await Task.FromResult(
                BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)
            );
        }
    }
}