using SWD_SoftwareArchitecture.DTOs;
using SWD_SoftwareArchitecture.Services;

namespace SWD_SoftwareArchitecture.Services.Interfaces
{
    /// <summary>
    /// Enrollment service interface
    /// Defines business operations for enrollment management following Service Layer Pattern
    /// </summary>
    public interface IEnrollmentService
    {
        Task<IEnumerable<EnrollmentListDto>> GetEnrollmentsByCourseAsync(int courseId);
        Task<EnrollmentDto?> GetEnrollmentByIdAsync(int enrollmentId);
        Task<ServiceResult<EnrollmentDto>> CreateEnrollmentAsync(EnrollmentDto enrollmentDto);
        Task<ServiceResult<EnrollmentDto>> UpdateEnrollmentAsync(EnrollmentDto enrollmentDto);
        Task<ServiceResult<bool>> DeleteEnrollmentAsync(int enrollmentId);
        Task<bool> ValidateEnrollmentAsync(EnrollmentDto enrollmentDto);
    }
}

