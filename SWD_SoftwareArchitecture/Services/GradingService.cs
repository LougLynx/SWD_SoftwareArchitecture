using Microsoft.Extensions.Logging;
using SWD_SoftwareArchitecture.Core.Common;
using SWD_SoftwareArchitecture.DTOs;
using SWD_SoftwareArchitecture.Models;
using SWD_SoftwareArchitecture.Repositories.Interfaces;
using SWD_SoftwareArchitecture.Services.Interfaces;

namespace SWD_SoftwareArchitecture.Services
{
    /// <summary>
    /// Grading service implementation
    /// Implements business logic for grading submissions following Grade Submissions use case requirements
    /// Inherits from BaseService for SPL Core functionality
    /// </summary>
    public class GradingService : BaseService, IGradingService
    {
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IAssignmentRepository _assignmentRepository;

        public GradingService(
            ILogger<GradingService> logger,
            ISubmissionRepository submissionRepository,
            IAssignmentRepository assignmentRepository)
            : base(logger)
        {
            _submissionRepository = submissionRepository;
            _assignmentRepository = assignmentRepository;
        }

        public async Task<IEnumerable<SubmissionListDto>> GetSubmissionsByAssignmentAsync(int assignmentId)
        {
            var submissions = await _submissionRepository.GetSubmissionsByAssignmentIdAsync(assignmentId);
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

        public async Task<GradeSubmissionDto?> GetSubmissionForGradingAsync(int submissionId)
        {
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

        public async Task<ServiceResult<GradeSubmissionDto>> GradeSubmissionAsync(GradeSubmissionDto gradeDto)
        {
            // Validate grade input (BR-01: Grades must fall within the valid scoring range)
            var validationResult = await ValidateGradeAsync(gradeDto);
            if (!validationResult)
            {
                return ServiceResult<GradeSubmissionDto>.ValidationFailure(
                    new List<string> { "Invalid grade input. Score is missing, non-numeric, or outside the allowed grading range." });
            }

            // Get submission
            var submission = await _submissionRepository.GetByIdAsync(gradeDto.SubmissionId);
            if (submission == null)
            {
                return ServiceResult<GradeSubmissionDto>.Failure("Submission not found or is no longer accessible.");
            }

            // Check if already graded (BR-02: A submission can be graded only once, unless manually overridden)
            // Note: This implementation allows re-grading. In production, you might want to add a flag or check.
            // For now, we allow instructors to update grades.

            // Update grade and feedback (BR-03: Feedback should be saved together with the score)
            submission.Grade = gradeDto.Grade;
            submission.Feedback = gradeDto.Feedback;

            try
            {
                await _submissionRepository.UpdateAsync(submission);
                return ServiceResult<GradeSubmissionDto>.Success(gradeDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<GradeSubmissionDto>.Failure($"Database update failure: {ex.Message}");
            }
        }

        public async Task<bool> ValidateGradeAsync(GradeSubmissionDto gradeDto)
        {
            // Get assignment to check max score
            var assignment = await _assignmentRepository.GetByIdAsync(gradeDto.AssignmentId);
            if (assignment == null)
            {
                return false;
            }

            // Validate grade range (BR-01: Grades must fall within the valid scoring range)
            if (gradeDto.Grade < 0 || gradeDto.Grade > assignment.MaxScore)
            {
                return false;
            }

            // Validate feedback length
            if (!string.IsNullOrEmpty(gradeDto.Feedback) && gradeDto.Feedback.Length > 1000)
            {
                return false;
            }

            return true;
        }
    }
}

