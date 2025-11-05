using SWD_SoftwareArchitecture.DTOs;
using SWD_SoftwareArchitecture.Services;
using SWD_SoftwareArchitecture.Services.Interfaces;
using SWD_SoftwareArchitecture.Variants.Strategies.Interfaces;

namespace SWD_SoftwareArchitecture.Variants.Strategies
{
    /// <summary>
    /// Standard grading strategy - Core implementation
    /// Implements IVariabilityPoint marker interface for SPL architecture
    /// </summary>
    public class StandardGradingStrategy : IGradingStrategy
    {
        private readonly IGradingService _gradingService;

        public StandardGradingStrategy(IGradingService gradingService)
        {
            _gradingService = gradingService;
        }

        public string VariabilityPointName => "GradingStrategy";

        public async Task<ServiceResult<GradeSubmissionDto>> ProcessGradingAsync(GradeSubmissionDto gradeDto)
        {
            return await _gradingService.GradeSubmissionAsync(gradeDto);
        }

        public bool CanHandle(string gradingType)
        {
            return gradingType == "Standard" || string.IsNullOrEmpty(gradingType);
        }
    }
}

