using SWD_SoftwareArchitecture.Models;

namespace SWD_SoftwareArchitecture.Repositories.Interfaces
{
    /// <summary>
    /// Enrollment-specific repository interface
    /// Extends generic repository for enrollment-specific operations
    /// </summary>
    public interface IEnrollmentRepository : IRepository<Enrollment>
    {
        Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseIdAsync(int courseId);
        Task<IEnumerable<Enrollment>> GetEnrollmentsByUserIdAsync(int userId);
        Task<Enrollment?> GetEnrollmentByUserAndCourseAsync(int userId, int courseId);
        Task<int> GetEnrollmentCountByCourseIdAsync(int courseId);
        Task<bool> IsStudentEnrolledAsync(int userId, int courseId);
    }
}

