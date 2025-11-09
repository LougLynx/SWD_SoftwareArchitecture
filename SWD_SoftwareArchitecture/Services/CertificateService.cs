using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWD_SoftwareArchitecture.Core.Common;
using SWD_SoftwareArchitecture.DTOs;
using SWD_SoftwareArchitecture.Models;
using SWD_SoftwareArchitecture.Repositories.Interfaces;
using SWD_SoftwareArchitecture.Services.Interfaces;

namespace SWD_SoftwareArchitecture.Services
{
    /// <summary>
    /// Enrollment service implementation
    /// Implements business logic for enrollment management following UC-08 requirements
    /// Inherits from BaseService for SPL Core functionality
    /// </summary>
    // Services/CertificateService.cs
    public class CertificateService : BaseService, ICertificateService
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IProgressService _progressService; // Dùng để check 100%
        private readonly IEnrollmentRepository _enrollmentRepository;
        // private readonly IPdfGenerator _pdfGenerator; // (Giả lập)

        public CertificateService(
            ILogger<CertificateService> logger,
            ICertificateRepository certificateRepository,
            IProgressService progressService,
            IEnrollmentRepository enrollmentRepository) : base(logger)
        {
            _certificateRepository = certificateRepository;
            _progressService = progressService;
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<ServiceResult<CertificateDto>> GetOrCreateCertificateAsync(int studentId, int courseId)
        {
            return await ExecuteWithErrorHandlingAsync(async () =>
            {
                // PRE-2: Kiểm tra 100%Í
                var progressResult = await _progressService.GetCourseProgressAsync(studentId, courseId);
                if (!progressResult.IsSuccess || progressResult.Data.CompletionPercentage < 100.0)
                {
                    return ServiceResult<CertificateDto>.Failure("Course is not yet completed.");
                }

                // Kiểm tra xem đã cấp chưa
                var existingCert = await _certificateRepository.GetCertificateAsync(studentId, courseId);
                if (existingCert != null)
                {
                    return ServiceResult<CertificateDto>.Success(new CertificateDto
                    {
                        CertificateId = existingCert.CertificateId,
                        CertificateUrl = existingCert.CertificateUrl,
                        IssueDate = existingCert.IssueDate
                    });
                }

                // Normal Flow: Tạo mới chứng chỉ
                // var pdfUrl = await _pdfGenerator.Generate(studentId, courseId); // Giả lập
                var pdfUrl = $"/certificates/{Guid.NewGuid()}.pdf"; // [cite: 196]

                var newCertificate = new Certification
                {
                    UserId = studentId,
                    IssueDate = DateTime.UtcNow,
                    CertificateUrl = pdfUrl,
                };

                await _certificateRepository.AddAsync(newCertificate);

                // Cập nhật trạng thái "Completed" cho Enrollment
                var enrollment = (await _enrollmentRepository
                    .FindAsync(e => e.UserId == studentId && e.CourseId == courseId)).First();
                enrollment.Status = "Completed"; // [cite: 192]
                await _enrollmentRepository.UpdateAsync(enrollment);

                return ServiceResult<CertificateDto>.Success(new CertificateDto
                {
                    CertificateId = newCertificate.CertificateId,
                    CertificateUrl = newCertificate.CertificateUrl,
                    IssueDate = newCertificate.IssueDate
                });
            }, "GetOrCreateCertificateAsync");
        }
    }
}

