using SWD_SoftwareArchitecture.Models;

namespace SWD_SoftwareArchitecture.Repositories.Interfaces
{
    /// <summary>
    /// Submission-specific repository interface
    /// Extends generic repository for submission-specific operations
    /// </summary>
    public interface ISubmissionRepository : IRepository<AssignmentSubmission>
    {
        Task<IEnumerable<AssignmentSubmission>> GetSubmissionsByAssignmentIdAsync(int assignmentId);
        Task<IEnumerable<AssignmentSubmission>> GetSubmissionsByUserIdAsync(int userId);
        Task<AssignmentSubmission?> GetSubmissionWithDetailsAsync(int submissionId);
        Task<bool> HasSubmissionAsync(int assignmentId, int userId);
    }
}

