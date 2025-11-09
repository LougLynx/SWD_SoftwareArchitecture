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
    public class ProgressRepository : Repository<Progress>, IProgressRepository
    {
        public ProgressRepository(ApplicationDbContext context, ILogger<ProgressRepository> logger)
            : base(context, logger) { }

        public async Task<List<int>> GetCompletedLessonIdsAsync(int studentId, int courseId)
        {
            // === THAY ĐỔI CHÍNH ===
            // Sử dụng Include để JOIN với Enrollment và lọc theo UserId/CourseId
            // thay vì giả định Progresses.UserId tồn tại.
            var completedLessonIds = await _dbSet
                .Include(p => p.Enrollment) // JOIN với bảng Enrollments
                .Where(p => p.Enrollment.UserId == studentId && p.Enrollment.CourseId == courseId)
                .Select(p => p.LessonId)
                .ToListAsync();
            // ======================

            Logger.LogInformation($"Found {completedLessonIds.Count} completed lessons for student {studentId} in course {courseId}");
            return completedLessonIds;
        }
    }
}

