using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWD_SoftwareArchitecture.Models;
using SWD_SoftwareArchitecture.Repositories.Interfaces;
using System.Linq.Expressions;

namespace SWD_SoftwareArchitecture.Core.Common
{
    /// <summary>
    /// Base repository class for SPL Core components
    /// Provides common data access operations shared across all products
    /// </summary>
    public abstract class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;
        protected readonly ILogger Logger;

        protected BaseRepository(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _dbSet = context.Set<T>();
            Logger = logger;
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            Logger.LogInformation($"Added {typeof(T).Name} entity");
            return entity;
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            Logger.LogInformation($"Updated {typeof(T).Name} entity");
        }

        public virtual async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            Logger.LogInformation($"Deleted {typeof(T).Name} entity");
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
    }
}

