using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWD_SoftwareArchitecture.Models;
using SWD_SoftwareArchitecture.Repositories.Interfaces;

namespace SWD_SoftwareArchitecture.Repositories
{
    // Kế thừa từ BaseRepository<Lesson> và triển khai ILessonRepository
    public class LessonRepository : Repository<Lesson>, ILessonRepository
    {
        public LessonRepository(ApplicationDbContext context, ILogger<LessonRepository> logger) 
            : base(context, logger)
        {
        }

        /// <summary>
        /// Lấy tất cả bài học thuộc một khóa học (dùng cho UC-04)
        /// </summary>
        public async Task<IEnumerable<Lesson>> GetLessonsByCourseIdAsync(int courseId)
        {
            // Truy vấn: Lesson -> Module -> Course
            return await _dbSet
                .Include(l => l.Module)
                .Where(l => l.Module.CourseId == courseId)
                .OrderBy(l => l.Module.OrderIndex)
                .ThenBy(l => l.OrderIndex)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy CourseId từ LessonId (dùng cho UC-03)
        /// </summary>
        public async Task<int?> GetCourseIdForLessonAsync(int lessonId)
        {
            var lesson = await _dbSet
                .Include(l => l.Module)
                .Where(l => l.LessonId == lessonId)
                .FirstOrDefaultAsync();

            return lesson?.Module.CourseId;
        }
    }
}

