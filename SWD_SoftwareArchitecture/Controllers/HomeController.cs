using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SWD_SoftwareArchitecture.Models;
using SWD_SoftwareArchitecture.Repositories.Interfaces;
using SWD_SoftwareArchitecture.Services.Interfaces;
using SWD_SoftwareArchitecture.ViewModels;

namespace SWD_SoftwareArchitecture.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICourseRepository _courseRepo;
        private readonly ILessonRepository _lessonRepo;
        private readonly IProgressService _progressService; // === CẦN THIẾT ===

        public HomeController(
            ICourseRepository courseRepo, 
            ILessonRepository lessonRepo,
            IProgressService progressService) // === CẦN THIẾT ===
        {
            _courseRepo = courseRepo;
            _lessonRepo = lessonRepo;
            _progressService = progressService; // === CẦN THIẾT ===
        }

        public async Task<IActionResult> Index()
        {
            // === FIX CỨNG (Theo yêu cầu) ===
            int courseId = 1; // Demo Course "Introduction to Programming"
            int studentInProgressId = 6; // Student Alice (đang học)
            int studentCompletedId = 7; // Student Bob (đã hoàn thành)
            // ===============================

            // 1. Lấy dữ liệu Course (từ DB)
            var course = await _courseRepo.GetByIdAsync(courseId);
            
            // 2. Lấy dữ liệu Lessons (từ DB)
            var allLessons = await _lessonRepo.GetLessonsByCourseIdAsync(courseId);

            // 3. Lấy dữ liệu Progress cho cả 2 SV (từ DB)
            var progressInProgressResult = await _progressService.GetCourseProgressAsync(studentInProgressId, courseId);
            var progressCompletedResult = await _progressService.GetCourseProgressAsync(studentCompletedId, courseId);

            // 4. Build ViewModel
            var viewModel = new CourseDetailViewModel
            {
                Course = course,

                // Nhóm các bài học (lessons) theo Module
                Modules = allLessons
                    .GroupBy(l => l.Module)
                    .Select(g => new ModuleDto
                    {
                        ModuleId = g.Key.ModuleId,
                        Title = g.Key.Title,
                        OrderIndex = g.Key.OrderIndex,
                        Lessons = g.OrderBy(l => l.OrderIndex).ToList()
                    })
                    .OrderBy(m => m.OrderIndex)
                    .ToList(),

                // Đưa dữ liệu tiến độ vào ViewModel
                StudentInProgress = progressInProgressResult.Data,
                StudentCompleted = progressCompletedResult.Data
            };

            return View(viewModel);
        }
    }
}
