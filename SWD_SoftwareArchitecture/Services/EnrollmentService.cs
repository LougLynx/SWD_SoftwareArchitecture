using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWD_SoftwareArchitecture.Core.Common;
using SWD_SoftwareArchitecture.DTOs;
using SWD_SoftwareArchitecture.Features;
using SWD_SoftwareArchitecture.Models;
using SWD_SoftwareArchitecture.Repositories.Interfaces;
using SWD_SoftwareArchitecture.Services.Interfaces;

namespace SWD_SoftwareArchitecture.Services
{
    /// <summary>
    /// Triển khai service cho Enrollment (Đăng ký khóa học)
    /// Cung cấp các nghiệp vụ quản lý đăng ký học dựa trên UC-08
    /// Kế thừa từ BaseService cho chức năng lõi SPL
    /// </summary>
    public class EnrollmentService : BaseService, IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IUserRepository _userRepository;
        private readonly FeatureManager _featureManager; // ✅ Singleton - SPL Architecture

        public EnrollmentService(
            ILogger<EnrollmentService> logger,
            IEnrollmentRepository enrollmentRepository,
            ICourseRepository courseRepository,
            IUserRepository userRepository,
            FeatureManager featureManager)
            : base(logger)
        {
            // Khởi tạo các repository cần thiết
            _enrollmentRepository = enrollmentRepository;
            _courseRepository = courseRepository;
            _userRepository = userRepository;
            _featureManager = featureManager;
        }

        // Lấy danh sách enrollment theo mã khóa học
        public async Task<IEnumerable<EnrollmentListDto>> GetEnrollmentsByCourseAsync(int courseId)
        {
            var enrollments = await _enrollmentRepository.GetEnrollmentsByCourseIdAsync(courseId);
            // Chuyển đổi sang DTO cho trả về client
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

        // Lấy thông tin đăng ký học theo mã enrollment
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

        // Tạo mới một bản ghi đăng ký học
        // SPL Architecture: Sử dụng FeatureManager (Singleton) để chọn logic dựa trên product variant
        public async Task<ServiceResult<EnrollmentDto>> CreateEnrollmentAsync(EnrollmentDto enrollmentDto)
        {
            // ✅ SPL Variability Point: Chọn logic dựa trên product variant
            var productVariant = _featureManager.GetProductVariant();
            
            // Conditional logic thay vì Strategy Pattern
            if (productVariant == "Premium" && _featureManager.IsEnabled("PremiumEnrollment"))
            {
                return await CreatePremiumEnrollmentAsync(enrollmentDto);
            }
            
            // Standard enrollment (default)
            return await CreateStandardEnrollmentAsync(enrollmentDto);
        }

        // Tạo enrollment theo logic Standard (default)
        private async Task<ServiceResult<EnrollmentDto>> CreateStandardEnrollmentAsync(EnrollmentDto enrollmentDto)
        {
            // Kiểm tra hợp lệ cho dữ liệu đăng ký học (BR-03)
            var validationResult = await ValidateEnrollmentAsync(enrollmentDto);
            if (!validationResult)
            {
                // Nếu không hợp lệ trả về lỗi
                return ServiceResult<EnrollmentDto>.ValidationFailure(new List<string> { "Enrollment validation failed" });
            }

            // Kiểm tra trùng đăng ký (BR-01: Mỗi sinh viên chỉ được đăng ký 1 lần cho mỗi khóa học)
            var isEnrolled = await _enrollmentRepository.IsStudentEnrolledAsync(enrollmentDto.UserId, enrollmentDto.CourseId);
            if (isEnrolled)
            {
                // Nếu đã tồn tại đăng ký trước đó cho cùng user, course thì trả về lỗi
                return ServiceResult<EnrollmentDto>.Failure("Student is already enrolled in this course. Duplicate enrollments are not allowed.");
            }

            // Kiểm tra sức chứa khóa học (BR-02: Không được vượt quá số lượng tối đa cho phép của khóa học)
            var course = await _courseRepository.GetCourseWithEnrollmentsAsync(enrollmentDto.CourseId);
            if (course == null)
            {
                return ServiceResult<EnrollmentDto>.Failure("Course not found.");
            }

            if (course.MaxCapacity.HasValue)
            {
                var currentEnrollmentCount = await _enrollmentRepository.GetEnrollmentCountByCourseIdAsync(enrollmentDto.CourseId);
                // Nếu số lượng hiện tại đã đủ sức chứa thì không cho đăng ký nữa
                if (currentEnrollmentCount >= course.MaxCapacity.Value)
                {
                    return ServiceResult<EnrollmentDto>.Failure("Course has reached maximum capacity. No additional enrollments can be added.");
                }
            }

            // Tạo entity Enrollment mới từ DTO
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
                // Sau khi thêm thành công, cập nhật lại Id cho DTO trả về
                enrollmentDto.EnrollmentId = enrollment.EnrollmentId;
                return ServiceResult<EnrollmentDto>.Success(enrollmentDto);
            }
            catch (Exception ex)
            {
                // Xảy ra lỗi khi lưu database
                return ServiceResult<EnrollmentDto>.Failure($"Database update failure: {ex.Message}");
            }
        }

        // Tạo enrollment theo logic Premium (SPL Variability Point)
        private async Task<ServiceResult<EnrollmentDto>> CreatePremiumEnrollmentAsync(EnrollmentDto enrollmentDto)
        {
            // Premium enrollment: Có thể có logic khác như auto-assign mentor, send welcome package, etc.
            // Hiện tại delegate về Standard logic, có thể mở rộng sau
            Logger.LogInformation("Creating Premium enrollment for user {UserId} in course {CourseId}", 
                enrollmentDto.UserId, enrollmentDto.CourseId);
            
            return await CreateStandardEnrollmentAsync(enrollmentDto);
        }

        // Cập nhật thông tin một bản ghi đăng ký học
        public async Task<ServiceResult<EnrollmentDto>> UpdateEnrollmentAsync(EnrollmentDto enrollmentDto)
        {
            if (!enrollmentDto.EnrollmentId.HasValue)
            {
                // Không có Id thì không thể cập nhật
                return ServiceResult<EnrollmentDto>.Failure("Enrollment ID is required for update.");
            }

            var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentDto.EnrollmentId.Value);
            if (enrollment == null)
            {
                return ServiceResult<EnrollmentDto>.Failure("Enrollment not found.");
            }

            // Kiểm tra hợp lệ cho dữ liệu đăng ký học (BR-03)
            var validationResult = await ValidateEnrollmentAsync(enrollmentDto);
            if (!validationResult)
            {
                return ServiceResult<EnrollmentDto>.ValidationFailure(new List<string> { "Enrollment validation failed" });
            }

            // Kiểm tra trùng đăng ký nếu thay đổi user hoặc course
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

            // Cập nhật các trường dữ liệu cho entity
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
                // Xảy ra lỗi khi cập nhật vào database
                return ServiceResult<EnrollmentDto>.Failure($"Database update failure: {ex.Message}");
            }
        }

        //Xóa bản ghi đăng ký học dựa vào mã
        public async Task<ServiceResult<bool>> DeleteEnrollmentAsync(int enrollmentId)
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId);
            if (enrollment == null)
            {
                // Không tìm thấy để xóa
                return ServiceResult<bool>.Failure("Enrollment not found.");
            }

            try
            {
                await _enrollmentRepository.DeleteAsync(enrollment);
                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                // Xảy ra lỗi khi xoá khỏi database
                return ServiceResult<bool>.Failure($"Database update failure: {ex.Message}");
            }
        }

        // Kiểm tra tính hợp lệ của dữ liệu đăng ký học
        public async Task<bool> ValidateEnrollmentAsync(EnrollmentDto enrollmentDto)
        {
            // Kiểm tra người dùng có tồn tại không
            var user = await _userRepository.GetByIdAsync(enrollmentDto.UserId);
            if (user == null)
            {
                return false;
            }

            // Kiểm tra khoá học có tồn tại không
            var course = await _courseRepository.GetByIdAsync(enrollmentDto.CourseId);
            if (course == null)
            {
                return false;
            }

            // Kiểm tra trạng thái hợp lệ ("Active", "Completed", "Dropped", "Pending")
            var validStatuses = new[] { "Active", "Completed", "Dropped", "Pending" };
            if (!validStatuses.Contains(enrollmentDto.Status))
            {
                return false;
            }

            // Kiểm tra tiến trình học phải nằm trong khoảng 0-100 (%)
            if (enrollmentDto.ProgressPercent < 0 || enrollmentDto.ProgressPercent > 100)
            {
                return false;
            }

            return true;
        }
    }
}
