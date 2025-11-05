using SWD_SoftwareArchitecture.DTOs;
using SWD_SoftwareArchitecture.Services;

namespace SWD_SoftwareArchitecture.Services.Interfaces
{
    /// <summary>
    /// Grading service interface
    /// Defines business operations for grading submissions following Service Layer Pattern
    /// </summary>
    public interface IGradingService
    {
        Task<IEnumerable<SubmissionListDto>> GetSubmissionsByAssignmentAsync(int assignmentId);
        Task<GradeSubmissionDto?> GetSubmissionForGradingAsync(int submissionId);
        Task<ServiceResult<GradeSubmissionDto>> GradeSubmissionAsync(GradeSubmissionDto gradeDto);
        Task<bool> ValidateGradeAsync(GradeSubmissionDto gradeDto);
    }
}

