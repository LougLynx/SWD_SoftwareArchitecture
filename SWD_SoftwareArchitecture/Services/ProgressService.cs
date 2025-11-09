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
    public class ProgressService : BaseService, IProgressService
    {
        private readonly IProgressRepository _progressRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ISubmissionRepository _assignmentSubmissionRepository; // Mới

        public ProgressService(
            ILogger<ProgressService> logger,
            IProgressRepository progressRepository,
            ILessonRepository lessonRepository,
            ICourseRepository courseRepository,
            IEnrollmentRepository enrollmentRepository,
            ISubmissionRepository assignmentSubmissionRepository) // Mới
            : base(logger)
        {
            _progressRepository = progressRepository;
            _lessonRepository = lessonRepository;
            _courseRepository = courseRepository;
            _enrollmentRepository = enrollmentRepository;
            _assignmentSubmissionRepository = assignmentSubmissionRepository; // Mới
        }

        public async Task<ServiceResult<CourseProgressDto>> GetCourseProgressAsync(int studentId, int courseId)
        {
            // Thêm "operationName" như bạn đã sửa lỗi ở (lần 10)
            return await ExecuteWithErrorHandlingAsync(async () =>
            {
                // 1. Lấy thông tin cơ bản
                var course = await _courseRepository.GetByIdAsync(courseId);
                if (course == null)
                    return ServiceResult<CourseProgressDto>.Failure("Course not found.");

                // 2. Lấy danh sách bài học (UC-04)
                var allLessons = (await _lessonRepository.GetLessonsByCourseIdAsync(courseId)).ToList();
                var completedLessonIds = (await _progressRepository.GetCompletedLessonIdsAsync(studentId, courseId));

                // 3. Tính toán %
                int totalLessons = allLessons.Count;
                int completedLessons = completedLessonIds.Count;
                double percentage = (totalLessons > 0) ? ((double)completedLessons / totalLessons) * 100 : 0;

                // Cập nhật % vào Enrollment (như logic cũ)
                var enrollment = (await _enrollmentRepository.FindAsync(e => e.UserId == studentId && e.CourseId == courseId)).FirstOrDefault();
                if (enrollment != null)
                {
                    enrollment.ProgressPercent = percentage;
                    await _enrollmentRepository.UpdateAsync(enrollment);
                }

                // 4. Lấy danh sách điểm (UC-04)
                var submissions = await _assignmentSubmissionRepository.GetSubmissionsForStudentByCourseAsync(studentId, courseId);

                // 5. Build DTO
                var dto = new CourseProgressDto
                {
                    CourseId = courseId,
                    CourseTitle = course.Title,
                    CompletionPercentage = Math.Round(percentage, 0),
                    TotalLessons = totalLessons,
                    CompletedLessons = completedLessons,

                    // "List of chapters, lessons..."
                    Lessons = allLessons.Select(l => new LessonProgressDto
                    {
                        Title = l.Title,
                        // "Identification marks (e.g., tick marks)"
                        IsCompleted = completedLessonIds.Contains(l.LessonId)
                    }).ToList(),

                    // "Scores of graded assignments/tests"
                    Assignments = submissions.Select(s => new AssignmentProgressDto
                    {
                        Title = s.Assignment.Title,
                        Score = s.Grade.HasValue ? s.Grade.Value.ToString() : "Chưa chấm"
                    }).ToList()
                };

                return ServiceResult<CourseProgressDto>.Success(dto);

            }, "GetCourseProgressAsync");
        }
    }
}

