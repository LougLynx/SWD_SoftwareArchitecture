using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWD_SoftwareArchitecture.Models;
using SWD_SoftwareArchitecture.Repositories.Interfaces;

namespace SWD_SoftwareArchitecture.Repositories
{
    /// <summary>
    /// User repository implementation
    /// </summary>
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger) 
            : base(context, logger)
        {
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
        {
            return await _dbSet
                .Where(u => u.Role == role)
                .ToListAsync();
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.FullName == username);
        }
    }
}

