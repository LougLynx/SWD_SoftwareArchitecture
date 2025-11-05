using SWD_SoftwareArchitecture.Models;

namespace SWD_SoftwareArchitecture.Repositories.Interfaces
{
    /// <summary>
    /// Course repository interface for course-specific operations
    /// </summary>
    public interface ICourseRepository : IRepository<Course>
    {
        Task<Course?> GetCourseWithEnrollmentsAsync(int courseId);
    }
}

