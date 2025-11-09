using SWD_SoftwareArchitecture.DTOs;
using SWD_SoftwareArchitecture.Services;

namespace SWD_SoftwareArchitecture.Services.Interfaces
{
    /// <summary>
    /// Enrollment service interface
    /// Defines business operations for enrollment management following Service Layer Pattern
    /// </summary>
    public interface ILessonService
    {
        // Trả về link Google Meet/Classroom
        Task<ServiceResult<string>> GetOnlineClassLinkAsync(int lessonId, int studentId);
    }
}

