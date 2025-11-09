using SWD_SoftwareArchitecture.Models;

namespace SWD_SoftwareArchitecture.Repositories.Interfaces
{
    public interface ILessonRepository : IRepository<Lesson>
    {
        // Lấy tất cả bài học thuộc một CourseId
        Task<IEnumerable<Lesson>> GetLessonsByCourseIdAsync(int courseId);
        
        // Lấy CourseId từ LessonId (dùng cho việc kiểm tra Enrollment)
        Task<int?> GetCourseIdForLessonAsync(int lessonId);
    }
}