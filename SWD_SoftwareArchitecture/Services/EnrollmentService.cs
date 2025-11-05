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
    public class EnrollmentService : BaseService, IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IUserRepository _userRepository;

        public EnrollmentService(
            ILogger<EnrollmentService> logger,
            IEnrollmentRepository enrollmentRepository,
            ICourseRepository courseRepository,
            IUserRepository userRepository)
            : base(logger)
        {
            _enrollmentRepository = enrollmentRepository;
            _courseRepository = courseRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<EnrollmentListDto>> GetEnrollmentsByCourseAsync(int courseId)
        {
            var enrollments = await _enrollmentRepository.GetEnrollmentsByCourseIdAsync(courseId);
            return enrollments.Select(e => new EnrollmentListDto
            {
                EnrollmentId = e.EnrollmentId,
                UserId = e.UserId,
                StudentName = e.User?.FullName ?? "Unknown",
                StudentEmail = e.User?.Email ?? "",
                CourseId = e.CourseId,
                CourseTitle = e.Course?.Title ?? "Unknown",
                EnrollmentDate = e.EnrollmentDate,
                Status = e.Status,
                ProgressPercent = e.ProgressPercent
            });
        }

        public async Task<EnrollmentDto?> GetEnrollmentByIdAsync(int enrollmentId)
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId);
            if (enrollment == null) return null;

            return new EnrollmentDto
            {
                EnrollmentId = enrollment.EnrollmentId,
                UserId = enrollment.UserId,
                CourseId = enrollment.CourseId,
                EnrollmentDate = enrollment.EnrollmentDate,
                Status = enrollment.Status,
                ProgressPercent = enrollment.ProgressPercent,
                StudentName = enrollment.User?.FullName,
                CourseTitle = enrollment.Course?.Title
            };
        }

        public async Task<ServiceResult<EnrollmentDto>> CreateEnrollmentAsync(EnrollmentDto enrollmentDto)
        {
            // Validate enrollment data (BR-03)
            var validationResult = await ValidateEnrollmentAsync(enrollmentDto);
            if (!validationResult)
            {
                return ServiceResult<EnrollmentDto>.ValidationFailure(new List<string> { "Enrollment validation failed" });
            }

            // Check for duplicate enrollment (BR-01: A student cannot be enrolled more than once in the same course)
            var isEnrolled = await _enrollmentRepository.IsStudentEnrolledAsync(enrollmentDto.UserId, enrollmentDto.CourseId);
            if (isEnrolled)
            {
                return ServiceResult<EnrollmentDto>.Failure("Student is already enrolled in this course. Duplicate enrollments are not allowed.");
            }

            // Check course capacity (BR-02: Course maximum capacity must not be exceeded)
            var course = await _courseRepository.GetCourseWithEnrollmentsAsync(enrollmentDto.CourseId);
            if (course == null)
            {
                return ServiceResult<EnrollmentDto>.Failure("Course not found.");
            }

            if (course.MaxCapacity.HasValue)
            {
                var currentEnrollmentCount = await _enrollmentRepository.GetEnrollmentCountByCourseIdAsync(enrollmentDto.CourseId);
                if (currentEnrollmentCount >= course.MaxCapacity.Value)
                {
                    return ServiceResult<EnrollmentDto>.Failure("Course has reached maximum capacity. No additional enrollments can be added.");
                }
            }

            // Create enrollment
            var enrollment = new Enrollment
            {
                UserId = enrollmentDto.UserId,
                CourseId = enrollmentDto.CourseId,
                EnrollmentDate = enrollmentDto.EnrollmentDate,
                Status = enrollmentDto.Status,
                ProgressPercent = enrollmentDto.ProgressPercent
            };

            try
            {
                await _enrollmentRepository.AddAsync(enrollment);
                enrollmentDto.EnrollmentId = enrollment.EnrollmentId;
                return ServiceResult<EnrollmentDto>.Success(enrollmentDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<EnrollmentDto>.Failure($"Database update failure: {ex.Message}");
            }
        }

        public async Task<ServiceResult<EnrollmentDto>> UpdateEnrollmentAsync(EnrollmentDto enrollmentDto)
        {
            if (!enrollmentDto.EnrollmentId.HasValue)
            {
                return ServiceResult<EnrollmentDto>.Failure("Enrollment ID is required for update.");
            }

            var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentDto.EnrollmentId.Value);
            if (enrollment == null)
            {
                return ServiceResult<EnrollmentDto>.Failure("Enrollment not found.");
            }

            // Validate enrollment data (BR-03)
            var validationResult = await ValidateEnrollmentAsync(enrollmentDto);
            if (!validationResult)
            {
                return ServiceResult<EnrollmentDto>.ValidationFailure(new List<string> { "Enrollment validation failed" });
            }

            // Check for duplicate enrollment if student or course changed (BR-01)
            if (enrollment.UserId != enrollmentDto.UserId || enrollment.CourseId != enrollmentDto.CourseId)
            {
                var isEnrolled = await _enrollmentRepository.IsStudentEnrolledAsync(enrollmentDto.UserId, enrollmentDto.CourseId);
                if (isEnrolled)
                {
                    var existingEnrollment = await _enrollmentRepository.GetEnrollmentByUserAndCourseAsync(enrollmentDto.UserId, enrollmentDto.CourseId);
                    if (existingEnrollment != null && existingEnrollment.EnrollmentId != enrollmentDto.EnrollmentId)
                    {
                        return ServiceResult<EnrollmentDto>.Failure("Student is already enrolled in this course. Duplicate enrollments are not allowed.");
                    }
                }
            }

            // Update enrollment
            enrollment.UserId = enrollmentDto.UserId;
            enrollment.CourseId = enrollmentDto.CourseId;
            enrollment.EnrollmentDate = enrollmentDto.EnrollmentDate;
            enrollment.Status = enrollmentDto.Status;
            enrollment.ProgressPercent = enrollmentDto.ProgressPercent;

            try
            {
                await _enrollmentRepository.UpdateAsync(enrollment);
                return ServiceResult<EnrollmentDto>.Success(enrollmentDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<EnrollmentDto>.Failure($"Database update failure: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteEnrollmentAsync(int enrollmentId)
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId);
            if (enrollment == null)
            {
                return ServiceResult<bool>.Failure("Enrollment not found.");
            }

            try
            {
                await _enrollmentRepository.DeleteAsync(enrollment);
                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure($"Database update failure: {ex.Message}");
            }
        }

        public async Task<bool> ValidateEnrollmentAsync(EnrollmentDto enrollmentDto)
        {
            // Validate that user exists
            var user = await _userRepository.GetByIdAsync(enrollmentDto.UserId);
            if (user == null)
            {
                return false;
            }

            // Validate that course exists
            var course = await _courseRepository.GetByIdAsync(enrollmentDto.CourseId);
            if (course == null)
            {
                return false;
            }

            // Validate status
            var validStatuses = new[] { "Active", "Completed", "Dropped", "Pending" };
            if (!validStatuses.Contains(enrollmentDto.Status))
            {
                return false;
            }

            // Validate progress percent
            if (enrollmentDto.ProgressPercent < 0 || enrollmentDto.ProgressPercent > 100)
            {
                return false;
            }

            return true;
        }
    }
}

