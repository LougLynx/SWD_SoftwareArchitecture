using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWD_SoftwareArchitecture.Core.Common;
using SWD_SoftwareArchitecture.Models;
using SWD_SoftwareArchitecture.Repositories.Interfaces;
using System.Linq.Expressions;

namespace SWD_SoftwareArchitecture.Repositories
{
    /// <summary>
    /// Generic repository implementation following Repository Pattern
    /// Implements common data access operations for SPL architecture
    /// Inherits from BaseRepository for SPL Core functionality
    /// </summary>
    public class Repository<T> : BaseRepository<T> where T : class
    {
        public Repository(ApplicationDbContext context, ILogger<Repository<T>> logger)
            : base(context, logger)
        {
        }
    }
}

