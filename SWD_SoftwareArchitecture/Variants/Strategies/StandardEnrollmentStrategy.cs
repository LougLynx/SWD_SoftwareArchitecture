using SWD_SoftwareArchitecture.DTOs;
using SWD_SoftwareArchitecture.Services;
using SWD_SoftwareArchitecture.Services.Interfaces;
using SWD_SoftwareArchitecture.Variants.Strategies.Interfaces;

namespace SWD_SoftwareArchitecture.Variants.Strategies
{
    /// <summary>
    /// Standard enrollment strategy - Core implementation
    /// Implements IVariabilityPoint marker interface for SPL architecture
    /// </summary>
    public class StandardEnrollmentStrategy : IEnrollmentStrategy
    {
        private readonly IEnrollmentService _enrollmentService;

        public StandardEnrollmentStrategy(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        public string VariabilityPointName => "EnrollmentStrategy";

        public async Task<ServiceResult<EnrollmentDto>> ProcessEnrollmentAsync(EnrollmentDto enrollmentDto)
        {
            return await _enrollmentService.CreateEnrollmentAsync(enrollmentDto);
        }

        public bool CanHandle(string enrollmentType)
        {
            return enrollmentType == "Standard" || string.IsNullOrEmpty(enrollmentType);
        }
    }
}

