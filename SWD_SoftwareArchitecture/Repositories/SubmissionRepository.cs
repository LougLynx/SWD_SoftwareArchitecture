using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWD_SoftwareArchitecture.Models;
using SWD_SoftwareArchitecture.Repositories.Interfaces;

namespace SWD_SoftwareArchitecture.Repositories
{
    /// <summary>
    /// Submission repository implementation
    /// Provides data access operations for submission and grading management
    /// </summary>
    public class SubmissionRepository : Repository<AssignmentSubmission>, ISubmissionRepository
    {
        public SubmissionRepository(ApplicationDbContext context, ILogger<SubmissionRepository> logger) 
            : base(context, logger)
        {
        }

        public async Task<IEnumerable<AssignmentSubmission>> GetSubmissionsByAssignmentIdAsync(int assignmentId)
        {
            return await _dbSet
                .Include(s => s.User)
                .Include(s => s.Assignment)
                .Where(s => s.AssignmentId == assignmentId)
                .ToListAsync();
        }

        public async Task<IEnumerable<AssignmentSubmission>> GetSubmissionsByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(s => s.User)
                .Include(s => s.Assignment)
                .Where(s => s.UserId == userId)
                .ToListAsync();
        }

        public async Task<AssignmentSubmission?> GetSubmissionWithDetailsAsync(int submissionId)
        {
            return await _dbSet
                .Include(s => s.User)
                .Include(s => s.Assignment)
                    .ThenInclude(a => a!.Course)
                .FirstOrDefaultAsync(s => s.SubmissionId == submissionId);
        }

        public async Task<bool> HasSubmissionAsync(int assignmentId, int userId)
        {
            return await _dbSet
                .AnyAsync(s => s.AssignmentId == assignmentId && s.UserId == userId);
        }
    }
}

