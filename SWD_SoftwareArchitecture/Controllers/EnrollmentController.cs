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

        /// GET: Enrollment/Index
        /// Hiển thị danh sách các khóa học để chọn quản lý đăng ký
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách tất cả các khóa học
            var courses = await _courseRepository.GetAllAsync();
            return View(courses);
        }

        /// GET: Enrollment/Manage?courseId={courseId}
        /// Mở màn hình quản lý đăng ký và hiển thị danh sách đăng ký
        [HttpGet]
        public async Task<IActionResult> Manage(int? courseId)
        {
            // Kiểm tra nếu chưa truyền courseId thì báo lỗi
            if (!courseId.HasValue)
            {
                TempData["ErrorMessage"] = "Course ID is required.";
                return RedirectToAction("Index");
            }

            // Lấy danh sách đăng ký của khóa học này
            var enrollments = await _enrollmentService.GetEnrollmentsByCourseAsync(courseId.Value);
            ViewBag.CourseId = courseId.Value;
            // Lấy tên khoá học để hiển thị trên giao diện, nếu không có thì hiển thị "Unknown Course"
            ViewBag.CourseTitle = (await _courseRepository.GetByIdAsync(courseId.Value))?.Title ?? "Unknown Course";

            return View(enrollments);
        }

        /// GET: Enrollment/Create?courseId={courseId}
        /// Hiển thị form thêm đăng ký mới
        [HttpGet]
        public async Task<IActionResult> Create(int? courseId)
        {
            // Kiểm tra đầu vào
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

            // Lấy danh sách sinh viên để chọn khi đăng ký
            var students = await _userRepository.GetUsersByRoleAsync("Student");
            ViewBag.Students = students;
            ViewBag.CourseId = courseId.Value;
            ViewBag.CourseTitle = course.Title;

            // Khởi tạo đối tượng đăng ký mới với các giá trị mặc định
            var enrollmentDto = new EnrollmentDto
            {
                CourseId = courseId.Value,
                EnrollmentDate = DateTime.Now,
                Status = "Active"
            };

            return View(enrollmentDto);
        }

        /// POST: Enrollment/Create
        /// Xử lý tạo mới đăng ký (Add action)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EnrollmentDto enrollmentDto)
        {
            // Kiểm tra dữ liệu hợp lệ
            if (!ModelState.IsValid)
            {
                // E6.1: Thông tin đăng ký không hợp lệ hoặc thiếu
                var students = await _userRepository.GetUsersByRoleAsync("Student");
                ViewBag.Students = students;
                var course = await _courseRepository.GetByIdAsync(enrollmentDto.CourseId);
                ViewBag.CourseTitle = course?.Title ?? "Unknown";
                return View(enrollmentDto);
            }

            // Thực hiện tạo mới đăng ký
            var result = await _enrollmentService.CreateEnrollmentAsync(enrollmentDto);

            if (!result.IsSuccess)
            {
                // A6.1: Trùng đăng ký
                // A6.2: Đã đủ số lượng tối đa
                // E7.1: Lỗi cập nhật cơ sở dữ liệu
                TempData["ErrorMessage"] = result.ErrorMessage ?? "Failed to create enrollment.";
                var students = await _userRepository.GetUsersByRoleAsync("Student");
                ViewBag.Students = students;
                var course = await _courseRepository.GetByIdAsync(enrollmentDto.CourseId);
                ViewBag.CourseTitle = course?.Title ?? "Unknown";
                return View(enrollmentDto);
            }

            // Tạo thành công, chuyển về trang quản lý đăng ký của khoá học đó
            TempData["SuccessMessage"] = "Enrollment created successfully.";
            return RedirectToAction(nameof(Manage), new { courseId = enrollmentDto.CourseId });
        }

        /// GET: Enrollment/Edit/{id}
        /// Hiển thị form cập nhật đăng ký
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            // Kiểm tra đầu vào
            if (!id.HasValue)
            {
                TempData["ErrorMessage"] = "Enrollment ID is required.";
                return RedirectToAction("Index", "Home");
            }

            // Lấy thông tin đăng ký cần sửa
            var enrollmentDto = await _enrollmentService.GetEnrollmentByIdAsync(id.Value);
            if (enrollmentDto == null)
            {
                TempData["ErrorMessage"] = "Enrollment not found.";
                return RedirectToAction("Index", "Home");
            }

            // Lấy danh sách sinh viên để chọn
            var students = await _userRepository.GetUsersByRoleAsync("Student");
            ViewBag.Students = students;
            var course = await _courseRepository.GetByIdAsync(enrollmentDto.CourseId);
            ViewBag.CourseTitle = course?.Title ?? "Unknown";

            return View(enrollmentDto);
        }

        /// POST: Enrollment/Edit
        /// Xử lý cập nhật đăng ký
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EnrollmentDto enrollmentDto)
        {
            // Kiểm tra dữ liệu hợp lệ
            if (!ModelState.IsValid)
            {
                // E6.1: Thông tin đăng ký không hợp lệ hoặc thiếu
                var students = await _userRepository.GetUsersByRoleAsync("Student");
                ViewBag.Students = students;
                var course = await _courseRepository.GetByIdAsync(enrollmentDto.CourseId);
                ViewBag.CourseTitle = course?.Title ?? "Unknown";
                return View(enrollmentDto);
            }

            // Cập nhật thông tin đăng ký
            var result = await _enrollmentService.UpdateEnrollmentAsync(enrollmentDto);

            if (!result.IsSuccess)
            {
                // Xảy ra lỗi khi cập nhật
                TempData["ErrorMessage"] = result.ErrorMessage ?? "Failed to update enrollment.";
                var students = await _userRepository.GetUsersByRoleAsync("Student");
                ViewBag.Students = students;
                var course = await _courseRepository.GetByIdAsync(enrollmentDto.CourseId);
                ViewBag.CourseTitle = course?.Title ?? "Unknown";
                return View(enrollmentDto);
            }

            // Cập nhật thành công, điều hướng về trang quản lý đăng ký
            TempData["SuccessMessage"] = "Enrollment updated successfully.";
            return RedirectToAction(nameof(Manage), new { courseId = enrollmentDto.CourseId });
        }

        /// GET: Enrollment/Delete/{id}
        /// Hiển thị xác nhận xoá đăng ký
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            // Kiểm tra đầu vào
            if (!id.HasValue)
            {
                TempData["ErrorMessage"] = "Enrollment ID is required.";
                return RedirectToAction("Index", "Home");
            }

            // Lấy thông tin đăng ký cần xoá
            var enrollmentDto = await _enrollmentService.GetEnrollmentByIdAsync(id.Value);
            if (enrollmentDto == null)
            {
                TempData["ErrorMessage"] = "Enrollment not found.";
                return RedirectToAction("Index", "Home");
            }

            return View(enrollmentDto);
        }

        /// POST: Enrollment/Delete/{id}
        /// Xử lý xoá đăng ký
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Lấy thông tin đăng ký cần xoá
            var enrollmentDto = await _enrollmentService.GetEnrollmentByIdAsync(id);
            if (enrollmentDto == null)
            {
                TempData["ErrorMessage"] = "Enrollment not found.";
                return RedirectToAction("Index", "Home");
            }

            // Xoá khỏi hệ thống
            var result = await _enrollmentService.DeleteEnrollmentAsync(id);

            if (!result.IsSuccess)
            {
                // E7.1: Lỗi cập nhật cơ sở dữ liệu
                TempData["ErrorMessage"] = result.ErrorMessage ?? "Failed to delete enrollment.";
                return View(enrollmentDto);
            }

            // Xoá thành công, quay về trang quản lý đăng ký khoá học
            TempData["SuccessMessage"] = "Enrollment deleted successfully.";
            return RedirectToAction(nameof(Manage), new { courseId = enrollmentDto.CourseId });
        }

        /// GET: Enrollment/Details/{id}
        /// Hiển thị chi tiết đăng ký
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            // Kiểm tra đầu vào
            if (!id.HasValue)
            {
                TempData["ErrorMessage"] = "Enrollment ID is required.";
                return RedirectToAction("Index", "Home");
            }

            // Lấy thông tin chi tiết đăng ký
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
