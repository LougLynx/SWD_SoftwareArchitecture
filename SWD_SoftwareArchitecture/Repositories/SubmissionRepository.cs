using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWD_SoftwareArchitecture.Models;
using SWD_SoftwareArchitecture.Repositories.Interfaces;

namespace SWD_SoftwareArchitecture.Repositories
{
    public class SubmissionRepository : Repository<AssignmentSubmission>, ISubmissionRepository
    {
        // Hàm khởi tạo, inject context và logger
        public SubmissionRepository(ApplicationDbContext context, ILogger<SubmissionRepository> logger) 
            : base(context, logger)
        {
        }

        // Lấy danh sách bài nộp theo mã Assignment (bài tập)
        public async Task<IEnumerable<AssignmentSubmission>> GetSubmissionsByAssignmentIdAsync(int assignmentId)
        {
            // Trả về danh sách AssignmentSubmission có AssignmentId tương ứng
            return await _dbSet
                .Include(s => s.User)           // Bao gồm thông tin người dùng nộp bài
                .Include(s => s.Assignment)     // Bao gồm thông tin bài tập
                .Where(s => s.AssignmentId == assignmentId)
                .ToListAsync();
        }

        // Lấy danh sách bài nộp theo mã người dùng
        public async Task<IEnumerable<AssignmentSubmission>> GetSubmissionsByUserIdAsync(int userId)
        {
            // Trả về danh sách AssignmentSubmission có UserId tương ứng
            return await _dbSet
                .Include(s => s.User)           // Bao gồm thông tin người dùng nộp bài
                .Include(s => s.Assignment)     // Bao gồm thông tin bài tập
                .Where(s => s.UserId == userId)
                .ToListAsync();
        }

        // Lấy thông tin chi tiết bài nộp theo mã submission
        public async Task<AssignmentSubmission?> GetSubmissionWithDetailsAsync(int submissionId)
        {
            // Trả về bài nộp bao gồm thông tin người dùng, bài tập và khóa học
            return await _dbSet
                .Include(s => s.User)                                       // Bao gồm thông tin người dùng nộp bài
                .Include(s => s.Assignment)                                 // Bao gồm thông tin bài tập
                    .ThenInclude(a => a!.Course)                           // Bao gồm thông tin khóa học
                .FirstOrDefaultAsync(s => s.SubmissionId == submissionId);
        }

        // Kiểm tra xem một người dùng đã nộp bài cho một assignment hay chưa
        public async Task<bool> HasSubmissionAsync(int assignmentId, int userId)
        {
            // Trả về true nếu đã có bài nộp tương ứng với AssignmentId và UserId, ngược lại trả về false
            return await _dbSet
                .AnyAsync(s => s.AssignmentId == assignmentId && s.UserId == userId);
        }
    }
}
