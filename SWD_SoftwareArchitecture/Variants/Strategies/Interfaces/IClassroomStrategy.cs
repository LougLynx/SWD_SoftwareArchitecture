using SWD_SoftwareArchitecture.Core.Abstractions;
using SWD_SoftwareArchitecture.DTOs;
using SWD_SoftwareArchitecture.Services;

namespace SWD_SoftwareArchitecture.Variants.Strategies.Interfaces
{
    /// <summary>
    /// Strategy interface for Classroom variability point
    /// Allows different Classroom strategies for different product variants
    /// Implements IVariabilityPoint for SPL architecture
    /// </summary>
    public interface IClassroomStrategy : IVariabilityPoint
    {
        Task<ServiceResult<string>> GetOnlineClassLinkAsync(int lessonId, int studentId);
        bool CanHandle(string classroomType);
    }
}

