using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWD_SoftwareArchitecture.Models;
using SWD_SoftwareArchitecture.Repositories.Interfaces;

namespace SWD_SoftwareArchitecture.Repositories
{
    /// <summary>
    /// Course repository implementation
    /// </summary>
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        public CourseRepository(ApplicationDbContext context, ILogger<CourseRepository> logger) 
            : base(context, logger)
        {
        }

        public async Task<Course?> GetCourseWithEnrollmentsAsync(int courseId)
        {
            return await _dbSet
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);
        }
    }
}

