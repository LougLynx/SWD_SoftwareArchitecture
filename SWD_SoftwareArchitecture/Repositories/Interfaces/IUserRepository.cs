using SWD_SoftwareArchitecture.Models;

namespace SWD_SoftwareArchitecture.Repositories.Interfaces
{
    /// <summary>
    /// User repository interface
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
    }
}

