using SWD_SoftwareArchitecture.DTOs;
using SWD_SoftwareArchitecture.Services;

namespace SWD_SoftwareArchitecture.Services.Interfaces
{
    // Services/Interfaces/ICertificateService.

    public interface ICertificateService
    {
        // Lấy hoặc tạo mới chứng chỉ
        Task<ServiceResult<CertificateDto>> GetOrCreateCertificateAsync(int studentId, int courseId);
    }
}

