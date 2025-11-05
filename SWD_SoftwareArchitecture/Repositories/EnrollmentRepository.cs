using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWD_SoftwareArchitecture.Models;
using SWD_SoftwareArchitecture.Repositories.Interfaces;

namespace SWD_SoftwareArchitecture.Repositories
{
    /// <summary>
    /// Enrollment repository implementation
    /// Provides data access operations for enrollment management
    /// </summary>
    public class EnrollmentRepository : Repository<Enrollment>, IEnrollmentRepository
    {
        public EnrollmentRepository(ApplicationDbContext context, ILogger<EnrollmentRepository> logger) 
            : base(context, logger)
        {
        }

        public async Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseIdAsync(int courseId)
        {
            return await _dbSet
                .Include(e => e.User)
                .Include(e => e.Course)
                .Where(e => e.CourseId == courseId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Enrollment>> GetEnrollmentsByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(e => e.User)
                .Include(e => e.Course)
                .Where(e => e.UserId == userId)
                .ToListAsync();
        }

        public async Task<Enrollment?> GetEnrollmentByUserAndCourseAsync(int userId, int courseId)
        {
            return await _dbSet
                .Include(e => e.User)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);
        }

        public async Task<int> GetEnrollmentCountByCourseIdAsync(int courseId)
        {
            return await _dbSet
                .Where(e => e.CourseId == courseId)
                .CountAsync();
        }

        public async Task<bool> IsStudentEnrolledAsync(int userId, int courseId)
        {
            return await _dbSet
                .AnyAsync(e => e.UserId == userId && e.CourseId == courseId);
        }
    }
}

