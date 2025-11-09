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
    public class LessonController : Controller
    {
        private readonly ClassroomStrategyFactory _classroomFactory;
        // Xóa IUserService đi vì chúng ta đang demo
        // private readonly IUserService _userService;

        public LessonController(ClassroomStrategyFactory classroomFactory)
        {
            _classroomFactory = classroomFactory;
        }

        [HttpGet]
        public async Task<IActionResult> JoinOnlineClass(int lessonId, int studentId) // Nhận studentId từ link
        {
            // 1. Lấy Strategy từ Factory
            var strategy = _classroomFactory.GetStrategy();

            // 2. Thực thi (truyền studentId giả lập)
            var result = await strategy.GetOnlineClassLinkAsync(lessonId, studentId);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return RedirectToAction("Index", "Home");
            }
            return Redirect(result.Data);
        }
    }
}

