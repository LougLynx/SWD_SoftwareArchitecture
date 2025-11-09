using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWD_SoftwareArchitecture.Core.Common;
using SWD_SoftwareArchitecture.DTOs;
using SWD_SoftwareArchitecture.Models;
using SWD_SoftwareArchitecture.Repositories.Interfaces;
using SWD_SoftwareArchitecture.Services.Interfaces;

namespace SWD_SoftwareArchitecture.Services
{
    /// <summary>
    /// Enrollment service implementation
    /// Implements business logic for enrollment management following UC-08 requirements
    /// Inherits from BaseService for SPL Core functionality
    /// </summary>
    public class LessonService : BaseService, ILessonService
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;

        public LessonService(
            ILogger<LessonService> logger,
            ILessonRepository lessonRepository,
            IEnrollmentRepository enrollmentRepository) : base(logger)
        {
            _lessonRepository = lessonRepository;
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<ServiceResult<string>> GetOnlineClassLinkAsync(int lessonId, int studentId)
        {
            return await ExecuteWithErrorHandlingAsync(async () =>
            {
                var lesson = await _lessonRepository.GetByIdAsync(lessonId);
                if (lesson == null)
                {
                    return ServiceResult<string>.Failure("Lesson not found.");
                }

                // PRE-2: Kiểm tra đã đăng ký
                var courseId = (await _lessonRepository.GetCourseIdForLessonAsync(lessonId)); // Cần implement
                var isEnrolled = await _enrollmentRepository.ExistsAsync(e => e.UserId == studentId && e.CourseId == courseId);
                
                if (!isEnrolled) // [cite: 155]
                {
                    return ServiceResult<string>.Failure("Student is not enrolled in this course.");
                }

                // Giả định 'ContentUrl' lưu link Google Classroom
                if (string.IsNullOrEmpty(lesson.ContentUrl) || lesson.ContentType != "VirtualClassroom")
                {
                    return ServiceResult<string>.Failure("Invalid or broken link.");
                }

                // Normal Flow: Trả về link
                return ServiceResult<string>.Success(lesson.ContentUrl);
            }, "GetOnlineClassLinkAsync");
        }
    }
}

