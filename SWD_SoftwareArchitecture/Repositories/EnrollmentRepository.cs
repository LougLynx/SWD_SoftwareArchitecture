using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWD_SoftwareArchitecture.Models;
using SWD_SoftwareArchitecture.Repositories.Interfaces;

namespace SWD_SoftwareArchitecture.Repositories {
    public class EnrollmentRepository : Repository<Enrollment>, IEnrollmentRepository
    {
        public EnrollmentRepository(ApplicationDbContext context, ILogger<EnrollmentRepository> logger) 
            : base(context, logger)
        {
        }

        //Lấy danh sách ghi danh theo mã khóa học
        public async Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseIdAsync(int courseId)
        {
            // Trả về danh sách Enrollment có CourseId tương ứng
            return await _dbSet
                .Include(e => e.User)   
                .Include(e => e.Course)
                .Where(e => e.CourseId == courseId)
                .ToListAsync();
        }

        // Lấy danh sách ghi danh theo mã người dùng
        public async Task<IEnumerable<Enrollment>> GetEnrollmentsByUserIdAsync(int userId)
        {
            // Trả về danh sách Enrollment có UserId tương ứng
            return await _dbSet
                .Include(e => e.User)   
                .Include(e => e.Course) 
                .Where(e => e.UserId == userId)
                .ToListAsync();
        }

        // Lấy thông tin ghi danh bằng UserId và CourseId
        public async Task<Enrollment?> GetEnrollmentByUserAndCourseAsync(int userId, int courseId)
        {
            return await _dbSet
                .Include(e => e.User)   
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);
        }

        // Đếm số lượng học viên theo mã khóa học
        public async Task<int> GetEnrollmentCountByCourseIdAsync(int courseId)
        {
            // Trả về số lượng Enrollment theo CourseId
            return await _dbSet
                .Where(e => e.CourseId == courseId)
                .CountAsync();
        }

        // Kiểm tra học viên đã ghi danh vào khóa học hay chưa
        public async Task<bool> IsStudentEnrolledAsync(int userId, int courseId)
        {
            // Trả về true nếu User đã ghi danh vào khóa học, ngược lại trả về false
            return await _dbSet
                .AnyAsync(e => e.UserId == userId && e.CourseId == courseId);
        }
    }
}
