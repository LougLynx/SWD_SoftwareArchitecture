using SWD_SoftwareArchitecture.Models;

namespace SWD_SoftwareArchitecture.Repositories.Interfaces
{
    /// <summary>
    /// Assignment repository interface
    /// </summary>
    public interface IAssignmentRepository : IRepository<Assignment>
    {
        Task<IEnumerable<Assignment>> GetAssignmentsByCourseIdAsync(int courseId);
    }
}

