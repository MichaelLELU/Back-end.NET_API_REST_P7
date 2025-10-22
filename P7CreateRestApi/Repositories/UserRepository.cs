using Microsoft.AspNetCore.Identity;
using P7CreateRestApi.Domain;

namespace P7CreateRestApi.Repositories
{
    public class UserRepository
    {
        private readonly UserManager<User> _userManager;

        public UserRepository(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        // --- CREATE ---
        public async Task<IdentityResult> CreateAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        // --- READ ---
        public async Task<User?> FindByIdAsync(int id)
        {
            return await _userManager.FindByIdAsync(id.ToString());
        }

        public async Task<User?> FindByUserNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public IQueryable<User> GetAll()
        {
            return _userManager.Users;
        }

        // --- UPDATE ---
        public async Task<IdentityResult> UpdateAsync(User user)
        {
            return await _userManager.UpdateAsync(user);
        }

        // --- DELETE ---
        public async Task<IdentityResult> DeleteAsync(User user)
        {
            return await _userManager.DeleteAsync(user);
        }
    }
}
