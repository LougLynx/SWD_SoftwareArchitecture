using SWD_SoftwareArchitecture.Models;

namespace SWD_SoftwareArchitecture.Repositories.Interfaces
{
    /// <summary>
    /// Enrollment-specific repository interface
    /// Extends generic repository for enrollment-specific operations
    /// </summary>
    public interface IProgressRepository : IRepository<Progress>
    {
        Task<List<int>> GetCompletedLessonIdsAsync(int studentId, int courseId);
    }
}