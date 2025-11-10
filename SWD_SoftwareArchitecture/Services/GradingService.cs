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
    /// Service chấm điểm bài nộp
    /// Xử lý logic nghiệp vụ cho chấm điểm submission theo yêu cầu của use case liên quan
    /// Kế thừa từ BaseService để dùng chung chức năng SPL Core
    /// </summary>
    public class GradingService : BaseService, IGradingService
    {
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly FeatureManager _featureManager; // ✅ Singleton - SPL Architecture

        public GradingService(
            ILogger<GradingService> logger,
            ISubmissionRepository submissionRepository,
            IAssignmentRepository assignmentRepository,
            FeatureManager featureManager) // ✅ Inject Singleton
            : base(logger)
        {
            // Khởi tạo repository cho bài nộp và bài tập
            _submissionRepository = submissionRepository;
            _assignmentRepository = assignmentRepository;
            _featureManager = featureManager; // ✅ SPL Architecture
        }

        // Lấy danh sách bài nộp theo mã bài tập
        public async Task<IEnumerable<SubmissionListDto>> GetSubmissionsByAssignmentAsync(int assignmentId)
        {
            // Lấy danh sách các submission theo assignmentId
            var submissions = await _submissionRepository.GetSubmissionsByAssignmentIdAsync(assignmentId);
            // Đổi sang dạng DTO trả về cho client
            return submissions.Select(s => new SubmissionListDto
            {
                SubmissionId = s.SubmissionId,
                AssignmentId = s.AssignmentId,
                AssignmentTitle = s.Assignment?.Title ?? "Unknown", 
                UserId = s.UserId,
                StudentName = s.User?.FullName ?? "Unknown",        
                StudentEmail = s.User?.Email ?? "",
                SubmittedAt = s.SubmittedAt,
                Grade = s.Grade,
                Feedback = s.Feedback,
                MaxScore = s.Assignment?.MaxScore ?? 0            
            });
        }

        // Lấy thông tin bài nộp chi tiết để chấm điểm
        public async Task<GradeSubmissionDto?> GetSubmissionForGradingAsync(int submissionId)
        {
            // Lấy submission có kèm thông tin assignment, user
            var submission = await _submissionRepository.GetSubmissionWithDetailsAsync(submissionId);
            if (submission == null) return null;

            return new GradeSubmissionDto
            {
                SubmissionId = submission.SubmissionId,
                AssignmentId = submission.AssignmentId,
                UserId = submission.UserId,
                StudentName = submission.User?.FullName,
                AssignmentTitle = submission.Assignment?.Title,
                SubmittedAt = submission.SubmittedAt,
                MaxScore = submission.Assignment?.MaxScore ?? 0,
                Grade = submission.Grade ?? 0,
                Feedback = submission.Feedback
            };
        }

        // Chấm điểm cho một bài nộp
        // SPL Architecture: Sử dụng FeatureManager (Singleton) để chọn logic dựa trên product variant
        public async Task<ServiceResult<GradeSubmissionDto>> GradeSubmissionAsync(GradeSubmissionDto gradeDto)
        {
            // ✅ SPL Variability Point: Chọn logic dựa trên product variant
            var productVariant = _featureManager.GetProductVariant();

            // Conditional logic thay vì Strategy Pattern
            if (productVariant == "Premium" && _featureManager.IsEnabled("PremiumGrading"))
            {
                return await GradeSubmissionPremiumAsync(gradeDto);
            }

            // Standard grading (default)
            return await GradeSubmissionStandardAsync(gradeDto);
        }

        // Chấm điểm theo logic Standard (default)
        private async Task<ServiceResult<GradeSubmissionDto>> GradeSubmissionStandardAsync(GradeSubmissionDto gradeDto)
        {
            // Kiểm tra hợp lệ đầu vào điểm số (BR-01: Điểm phải nằm trong phạm vi hợp lệ)
            var validationResult = await ValidateGradeAsync(gradeDto);
            if (!validationResult)
            {
                return ServiceResult<GradeSubmissionDto>.ValidationFailure(
                    new List<string> { "Điểm nhập vào không hợp lệ. Điểm bị thiếu, không phải số, hoặc nằm ngoài phạm vi cho phép." });
            }

            // Lấy submission cần chấm điểm
            var submission = await _submissionRepository.GetByIdAsync(gradeDto.SubmissionId);
            if (submission == null)
            {
                // Không tìm thấy bài nộp
                return ServiceResult<GradeSubmissionDto>.Failure("Không tìm thấy bài nộp hoặc không còn khả dụng.");
            }

            // Kiểm tra đã chấm điểm chưa (BR-02: Mỗi bài chỉ được chấm một lần, trừ trường hợp override thủ công)
            // Lưu ý: Ở đây cho phép giáo viên cập nhật lại điểm (có thể mở thêm cờ nếu muốn hạn chế)
            // => Tạm thời cho phép cập nhật điểm tự do

            // Cập nhật điểm và nhận xét (BR-03: Feedback phải được lưu cùng điểm số)
            submission.Grade = gradeDto.Grade;
            submission.Feedback = gradeDto.Feedback;

            try
            {
                // Cập nhật dữ liệu vào DB
                await _submissionRepository.UpdateAsync(submission);
                return ServiceResult<GradeSubmissionDto>.Success(gradeDto);
            }
            catch (Exception ex)
            {
                // Bắt lỗi cập nhật vào DB
                return ServiceResult<GradeSubmissionDto>.Failure($"Lỗi khi cập nhật database: {ex.Message}");
            }
        }

        // Chấm điểm theo logic Premium (SPL Variability Point)
        private async Task<ServiceResult<GradeSubmissionDto>> GradeSubmissionPremiumAsync(GradeSubmissionDto gradeDto)
        {
            // Premium grading: Có thể có logic khác như auto-feedback, detailed analytics, etc.
            // Hiện tại delegate về Standard logic, có thể mở rộng sau
            Logger.LogInformation("Grading Premium submission {SubmissionId} for assignment {AssignmentId}",
                gradeDto.SubmissionId, gradeDto.AssignmentId);
            return await GradeSubmissionStandardAsync(gradeDto);
        }

        // Hàm kiểm tra hợp lệ điểm chấm cho một bài nộp
        public async Task<bool> ValidateGradeAsync(GradeSubmissionDto gradeDto)
        {
            // Lấy bài tập để kiểm tra điểm tối đa
            var assignment = await _assignmentRepository.GetByIdAsync(gradeDto.AssignmentId);
            if (assignment == null)
            {
                // Không tìm thấy bài tập
                return false;
            }

            // Kiểm tra điểm có nằm trong phạm vi cho phép không (BR-01)
            if (gradeDto.Grade < 0 || gradeDto.Grade > assignment.MaxScore)
            {
                return false;
            }

            // Kiểm tra độ dài nhận xét feedback (không quá 1000 ký tự)
            if (!string.IsNullOrEmpty(gradeDto.Feedback) && gradeDto.Feedback.Length > 1000)
            {
                return false;
            }

            // Nếu mọi kiểm tra đều qua -> hợp lệ
            return true;
        }
    }
}
