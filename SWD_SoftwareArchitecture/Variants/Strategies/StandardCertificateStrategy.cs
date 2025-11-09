using SWD_SoftwareArchitecture.Core.Common;
using SWD_SoftwareArchitecture.DTOs;
using SWD_SoftwareArchitecture.Models;
using SWD_SoftwareArchitecture.Repositories.Interfaces;
using SWD_SoftwareArchitecture.Services;
using SWD_SoftwareArchitecture.Services.Interfaces; // Cho IProgressService
using SWD_SoftwareArchitecture.Variants.Strategies.Interfaces;

namespace SWD_SoftwareArchitecture.Variants.Strategies
{
    public class StandardCertificateStrategy : ICertificateStrategy
    {
        private readonly ICertificateService _CertificateService;

        public StandardCertificateStrategy(ICertificateService CertificateService)
        {
            _CertificateService = CertificateService;
        }

        public string VariabilityPointName => "CertificateStrategy";

        public bool CanHandle(string CertificateType)
        {
            return CertificateType == "Standard" || string.IsNullOrEmpty(CertificateType);
        }

        public async Task<ServiceResult<CertificateDto>> GetOrCreateCertificateAsync(int studentId, int courseId)
        {
            return await _CertificateService.GetOrCreateCertificateAsync(studentId, courseId);
        }
    }
}