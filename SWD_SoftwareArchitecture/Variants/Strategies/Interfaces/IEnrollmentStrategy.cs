using SWD_SoftwareArchitecture.Core.Abstractions;
using SWD_SoftwareArchitecture.DTOs;
using SWD_SoftwareArchitecture.Services;

namespace SWD_SoftwareArchitecture.Variants.Strategies.Interfaces
{
    /// <summary>
    /// Strategy interface for enrollment variability point
    /// Allows different enrollment strategies for different product variants
    /// Implements IVariabilityPoint for SPL architecture
    /// </summary>
    public interface IEnrollmentStrategy : IVariabilityPoint
    {
        Task<ServiceResult<EnrollmentDto>> ProcessEnrollmentAsync(EnrollmentDto enrollmentDto);
        bool CanHandle(string enrollmentType);
    }
}

