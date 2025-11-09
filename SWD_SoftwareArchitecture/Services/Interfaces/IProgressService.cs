using SWD_SoftwareArchitecture.DTOs;
using SWD_SoftwareArchitecture.Services;

namespace SWD_SoftwareArchitecture.Services.Interfaces
{
    /// <summary>
    /// Enrollment service interface
    /// Defines business operations for enrollment management following Service Layer Pattern
    /// </summary>
    public interface IProgressService
    {
    Task<ServiceResult<CourseProgressDto>> GetCourseProgressAsync(int studentId, int courseId);
    }
}

