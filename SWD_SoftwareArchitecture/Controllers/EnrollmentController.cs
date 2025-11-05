using Microsoft.AspNetCore.Mvc;
using SWD_SoftwareArchitecture.DTOs;
using SWD_SoftwareArchitecture.Services.Interfaces;
using SWD_SoftwareArchitecture.Repositories.Interfaces;

namespace SWD_SoftwareArchitecture.Controllers
{
    /// <summary>
    /// Enrollment Controller following MVC pattern
    /// Implements UC-08: Manage Enrollments
    /// </summary>
    public class EnrollmentController : Controller
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly ICourseRepository _courseRepository;
        private readonly IUserRepository _userRepository;

        public EnrollmentController(
            IEnrollmentService enrollmentService,
            ICourseRepository courseRepository,
            IUserRepository userRepository)
        {
            _enrollmentService = enrollmentService;
            _courseRepository = courseRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// GET: Enrollment/Index
        /// Displays list of courses to select for enrollment management
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var courses = await _courseRepository.GetAllAsync();
            return View(courses);
        }

        /// <summary>
        /// GET: Enrollment/Manage?courseId={courseId}
        /// Step 1-2: Opens Manage Enrollments screen and displays enrollment list
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Manage(int? courseId)
        {
            if (!courseId.HasValue)
            {
                TempData["ErrorMessage"] = "Course ID is required.";
                return RedirectToAction("Index");
            }

            var enrollments = await _enrollmentService.GetEnrollmentsByCourseAsync(courseId.Value);
            ViewBag.CourseId = courseId.Value;
            ViewBag.CourseTitle = (await _courseRepository.GetByIdAsync(courseId.Value))?.Title ?? "Unknown Course";

            return View(enrollments);
        }

        /// <summary>
        /// GET: Enrollment/Create?courseId={courseId}
        /// Step 3-4: Displays form for adding new enrollment
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Create(int? courseId)
        {
            if (!courseId.HasValue)
            {
                TempData["ErrorMessage"] = "Course ID is required.";
                return RedirectToAction("Index", "Home");
            }

            var course = await _courseRepository.GetByIdAsync(courseId.Value);
            if (course == null)
            {
                TempData["ErrorMessage"] = "Course not found.";
                return RedirectToAction("Index", "Home");
            }

            var students = await _userRepository.GetUsersByRoleAsync("Student");
            ViewBag.Students = students;
            ViewBag.CourseId = courseId.Value;
            ViewBag.CourseTitle = course.Title;

            var enrollmentDto = new EnrollmentDto
            {
                CourseId = courseId.Value,
                EnrollmentDate = DateTime.Now,
                Status = "Active"
            };

            return View(enrollmentDto);
        }

        /// <summary>
        /// POST: Enrollment/Create
        /// Step 5-8: Handles enrollment creation (Add action)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EnrollmentDto enrollmentDto)
        {
            if (!ModelState.IsValid)
            {
                // E6.1: Invalid or missing enrollment information
                var students = await _userRepository.GetUsersByRoleAsync("Student");
                ViewBag.Students = students;
                var course = await _courseRepository.GetByIdAsync(enrollmentDto.CourseId);
                ViewBag.CourseTitle = course?.Title ?? "Unknown";
                return View(enrollmentDto);
            }

            var result = await _enrollmentService.CreateEnrollmentAsync(enrollmentDto);

            if (!result.IsSuccess)
            {
                // A6.1: Duplicate Enrollment Detected
                // A6.2: Course Capacity Reached
                // E7.1: Database update failure
                TempData["ErrorMessage"] = result.ErrorMessage ?? "Failed to create enrollment.";
                var students = await _userRepository.GetUsersByRoleAsync("Student");
                ViewBag.Students = students;
                var course = await _courseRepository.GetByIdAsync(enrollmentDto.CourseId);
                ViewBag.CourseTitle = course?.Title ?? "Unknown";
                return View(enrollmentDto);
            }

            // Step 8: Success confirmation
            TempData["SuccessMessage"] = "Enrollment created successfully.";
            return RedirectToAction(nameof(Manage), new { courseId = enrollmentDto.CourseId });
        }

        /// <summary>
        /// GET: Enrollment/Edit/{id}
        /// Step 3-4: Displays form for updating enrollment (Update action)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                TempData["ErrorMessage"] = "Enrollment ID is required.";
                return RedirectToAction("Index", "Home");
            }

            var enrollmentDto = await _enrollmentService.GetEnrollmentByIdAsync(id.Value);
            if (enrollmentDto == null)
            {
                TempData["ErrorMessage"] = "Enrollment not found.";
                return RedirectToAction("Index", "Home");
            }

            var students = await _userRepository.GetUsersByRoleAsync("Student");
            ViewBag.Students = students;
            var course = await _courseRepository.GetByIdAsync(enrollmentDto.CourseId);
            ViewBag.CourseTitle = course?.Title ?? "Unknown";

            return View(enrollmentDto);
        }

        /// <summary>
        /// POST: Enrollment/Edit
        /// Step 5-8: Handles enrollment update
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EnrollmentDto enrollmentDto)
        {
            if (!ModelState.IsValid)
            {
                // E6.1: Invalid or missing enrollment information
                var students = await _userRepository.GetUsersByRoleAsync("Student");
                ViewBag.Students = students;
                var course = await _courseRepository.GetByIdAsync(enrollmentDto.CourseId);
                ViewBag.CourseTitle = course?.Title ?? "Unknown";
                return View(enrollmentDto);
            }

            var result = await _enrollmentService.UpdateEnrollmentAsync(enrollmentDto);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage ?? "Failed to update enrollment.";
                var students = await _userRepository.GetUsersByRoleAsync("Student");
                ViewBag.Students = students;
                var course = await _courseRepository.GetByIdAsync(enrollmentDto.CourseId);
                ViewBag.CourseTitle = course?.Title ?? "Unknown";
                return View(enrollmentDto);
            }

            // Step 8: Success confirmation
            TempData["SuccessMessage"] = "Enrollment updated successfully.";
            return RedirectToAction(nameof(Manage), new { courseId = enrollmentDto.CourseId });
        }

        /// <summary>
        /// GET: Enrollment/Delete/{id}
        /// Step 3: Displays confirmation for removing enrollment (Remove action)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                TempData["ErrorMessage"] = "Enrollment ID is required.";
                return RedirectToAction("Index", "Home");
            }

            var enrollmentDto = await _enrollmentService.GetEnrollmentByIdAsync(id.Value);
            if (enrollmentDto == null)
            {
                TempData["ErrorMessage"] = "Enrollment not found.";
                return RedirectToAction("Index", "Home");
            }

            return View(enrollmentDto);
        }

        /// <summary>
        /// POST: Enrollment/Delete/{id}
        /// Step 5-8: Handles enrollment deletion
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrollmentDto = await _enrollmentService.GetEnrollmentByIdAsync(id);
            if (enrollmentDto == null)
            {
                TempData["ErrorMessage"] = "Enrollment not found.";
                return RedirectToAction("Index", "Home");
            }

            var result = await _enrollmentService.DeleteEnrollmentAsync(id);

            if (!result.IsSuccess)
            {
                // E7.1: Database update failure
                TempData["ErrorMessage"] = result.ErrorMessage ?? "Failed to delete enrollment.";
                return View(enrollmentDto);
            }

            // Step 8: Success confirmation
            TempData["SuccessMessage"] = "Enrollment deleted successfully.";
            return RedirectToAction(nameof(Manage), new { courseId = enrollmentDto.CourseId });
        }

        /// <summary>
        /// GET: Enrollment/Details/{id}
        /// Step 3: View enrollment details (View action)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
            {
                TempData["ErrorMessage"] = "Enrollment ID is required.";
                return RedirectToAction("Index", "Home");
            }

            var enrollmentDto = await _enrollmentService.GetEnrollmentByIdAsync(id.Value);
            if (enrollmentDto == null)
            {
                TempData["ErrorMessage"] = "Enrollment not found.";
                return RedirectToAction("Index", "Home");
            }

            return View(enrollmentDto);
        }
    }
}

