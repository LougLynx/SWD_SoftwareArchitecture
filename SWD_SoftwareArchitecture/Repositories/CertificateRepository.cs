using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWD_SoftwareArchitecture.Models;
using SWD_SoftwareArchitecture.Repositories.Interfaces;

namespace SWD_SoftwareArchitecture.Repositories
{
    // Repositories/CertificateRepository.cs
    public class CertificateRepository : Repository<Certification>, ICertificateRepository
    {
        public CertificateRepository(ApplicationDbContext context, ILogger<CertificateRepository> logger)
            : base(context, logger) { }


        public async Task<Certification?> GetCertificateAsync(int studentId, int courseId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.UserId == studentId);
        }
    }
}

