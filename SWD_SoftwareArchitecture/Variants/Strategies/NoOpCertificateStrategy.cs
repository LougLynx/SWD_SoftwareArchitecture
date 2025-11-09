using SWD_SoftwareArchitecture.DTOs;
using SWD_SoftwareArchitecture.Services;
using SWD_SoftwareArchitecture.Variants.Strategies.Interfaces;

namespace SWD_SoftwareArchitecture.Variants.Strategies
{
    public class NoOpCertificateStrategy : ICertificateStrategy
    {
        public string VariabilityPointName => "CertificateStrategy";

        public bool CanHandle(string certificatingType)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult<CertificateDto>> GetOrCreateCertificateAsync(int studentId, int courseId)
        {
            return Task.FromResult(
                ServiceResult<CertificateDto>.Failure("Tính năng cấp chứng chỉ không được kích hoạt.")
            );
        }
    }
}