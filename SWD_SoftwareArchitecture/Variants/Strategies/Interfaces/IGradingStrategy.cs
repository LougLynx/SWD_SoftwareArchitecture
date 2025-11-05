using SWD_SoftwareArchitecture.Core.Abstractions;
using SWD_SoftwareArchitecture.DTOs;
using SWD_SoftwareArchitecture.Services;

namespace SWD_SoftwareArchitecture.Variants.Strategies.Interfaces
{
    /// <summary>
    /// Strategy interface for grading variability point
    /// Allows different grading strategies for different product variants
    /// Implements IVariabilityPoint for SPL architecture
    /// </summary>
    public interface IGradingStrategy : IVariabilityPoint
    {
        Task<ServiceResult<GradeSubmissionDto>> ProcessGradingAsync(GradeSubmissionDto gradeDto);
        bool CanHandle(string gradingType);
    }
}

