using SWD_SoftwareArchitecture.Core.Abstractions;
using SWD_SoftwareArchitecture.Services;
using SWD_SoftwareArchitecture.DTOs; // Giả định CertificateDto nằm ở đây

namespace SWD_SoftwareArchitecture.Variants.Strategies.Interfaces
{
    public interface ICertificateStrategy : IVariabilityPoint
    {
        Task<ServiceResult<CertificateDto>> GetOrCreateCertificateAsync(int studentId, int courseId);
        bool CanHandle(string certificatingType);
    }
}