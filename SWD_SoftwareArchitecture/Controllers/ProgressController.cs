using Microsoft.AspNetCore.Mvc;
using SWD_SoftwareArchitecture.DTOs;
using SWD_SoftwareArchitecture.Services.Interfaces;
using SWD_SoftwareArchitecture.Repositories.Interfaces;
using SWD_SoftwareArchitecture.Variants.Strategies;

namespace SWD_SoftwareArchitecture.Controllers
{
    /// <summary>
    /// Grading Controller following MVC pattern
    /// Implements Grade Submissions use case
    /// </summary>
    // ...
    // ...
    public class ProgressController : Controller
    {
        private readonly IProgressService _progressService;
        // Xóa IUserService

        public ProgressController(IProgressService progressService)
        {
            _progressService = progressService;
        }
        
        [HttpGet]
        public async Task<IActionResult> MyProgress(int courseId, int studentId) // Nhận studentId từ link
        {
            var result = await _progressService.GetCourseProgressAsync(studentId, courseId);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            return View(result.Data);
        }
    }
}

