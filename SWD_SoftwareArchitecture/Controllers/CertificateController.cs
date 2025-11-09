using Microsoft.AspNetCore.Mvc;
using SWD_SoftwareArchitecture.DTOs;
using SWD_SoftwareArchitecture.Services.Interfaces;
using SWD_SoftwareArchitecture.Repositories.Interfaces;
using SWD_SoftwareArchitecture.Variants.Strategies;
using System.Text;

namespace SWD_SoftwareArchitecture.Controllers
{
    /// <summary>
    /// Grading Controller following MVC pattern
    /// Implements Grade Submissions use case
    /// </summary>
    // ...
    public class CertificateController : Controller
    {
        private readonly CertificateStrategyFactory _certificateFactory;
        // Không cần IUserService vì chúng ta fix cứng ID

        public CertificateController(CertificateStrategyFactory certificateFactory)
        {
            _certificateFactory = certificateFactory;
        }

        // UC-05: Trigger
        [HttpGet]
        public async Task<IActionResult> DownloadCertificate(int courseId, int studentId)
        {
            // 1. "The student clicks the 'Download Certificate' button"
            var strategy = _certificateFactory.GetStrategy();

            // 2. "The system automatically checks..." (gọi service thật)
            var result = await strategy.GetOrCreateCertificateAsync(studentId, courseId);

            if (!result.IsSuccess)
            {
                // (Chưa hoàn thành, hoặc feature tắt)
                TempData["ErrorMessage"] = result.ErrorMessage;
                // Quay lại trang chủ (Course Detail)
                return RedirectToAction("Index", "Home");
            }

            // 3. "The system generates a certificate file..."
            // 4. "The browser begins downloading the certificate file..."

            // TẠO MỘT FILE PDF (GIẢ LẬP BẰNG TEXT) ĐỂ DEMO DOWNLOAD
            var message = $"Đây là chứng chỉ (Bản Demo) cho:\n\n" +
                          $"Student ID: {studentId}\n" +
                          $"Course ID: {courseId}\n" +
                          $"Ngày cấp: {result.Data.IssueDate}\n" +
                          $"Mã chứng chỉ: {result.Data.CertificateId}";

            var fileBytes = Encoding.UTF8.GetBytes(message);

            // Trả về file, trình duyệt sẽ tự động tải xuống
            return File(fileBytes, "application/pdf", $"ChungChi_Demo_SID{studentId}_CID{courseId}.pdf");
        }
    }
}

