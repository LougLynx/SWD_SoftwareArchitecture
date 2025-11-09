using SWD_SoftwareArchitecture.Models;

namespace SWD_SoftwareArchitecture.Repositories.Interfaces
{
    // Repositories/Interfaces/ICertificationRepository.cs
    public interface ICertificateRepository : IRepository<Certification>
    {
        Task<Certification?> GetCertificateAsync(int studentId, int courseId);
    }
}