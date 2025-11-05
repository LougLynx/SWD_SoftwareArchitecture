using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWD_SoftwareArchitecture.Models;
using SWD_SoftwareArchitecture.Repositories.Interfaces;

namespace SWD_SoftwareArchitecture.Repositories
{
    /// <summary>
    /// Assignment repository implementation
    /// </summary>
    public class AssignmentRepository : Repository<Assignment>, IAssignmentRepository
    {
        public AssignmentRepository(ApplicationDbContext context, ILogger<AssignmentRepository> logger) 
            : base(context, logger)
        {
        }

        public override async Task<Assignment?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(a => a.Course)
                .FirstOrDefaultAsync(a => a.AssignmentId == id);
        }

        public override async Task<IEnumerable<Assignment>> GetAllAsync()
        {
            return await _dbSet
                .Include(a => a.Course)
                .OrderBy(a => a.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Assignment>> GetAssignmentsByCourseIdAsync(int courseId)
        {
            return await _dbSet
                .Include(a => a.Course)
                .Where(a => a.CourseId == courseId)
                .ToListAsync();
        }
    }
}

