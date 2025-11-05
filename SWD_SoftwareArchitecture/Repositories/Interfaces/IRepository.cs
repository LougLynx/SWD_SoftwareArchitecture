using System.Linq.Expressions;

namespace SWD_SoftwareArchitecture.Repositories.Interfaces
{
    /// <summary>
    /// Generic repository interface following Repository Pattern
    /// Provides common CRUD operations for SPL architecture
    /// </summary>
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }
}

