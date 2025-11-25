using System.Threading.Tasks;
using DemoMvc.Core.Entities;

namespace DemoMvc.Core.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByUsernameAsync(string username);  // Can return null
        Task<User?> GetUserByEmailAsync(string email);        // Can return null
        Task<User> RegisterUserAsync(User user, string password);
        Task<bool> ValidatePasswordAsync(User user, string password);
    }
}