using Microsoft.AspNetCore.Mvc;
using SWD_SoftwareArchitecture.DTOs;
using SWD_SoftwareArchitecture.Services.Interfaces;
using SWD_SoftwareArchitecture.Repositories.Interfaces;

namespace SWD_SoftwareArchitecture.Controllers
{
    // Chấm bài nộp
    public class GradingController : Controller
    {
        private readonly IGradingService _gradingService;
        private readonly IAssignmentRepository _assignmentRepository;

        public GradingController(
            IGradingService gradingService,
            IAssignmentRepository assignmentRepository)
        {
            _gradingService = gradingService;
            _assignmentRepository = assignmentRepository;
        }

        /// <summary>
        /// GET: Grading/Index?assignmentId={assignmentId}
        /// Step 1-2: Opens Grade Submissions screen and displays list of submissions
        /// If no assignmentId, shows list of assignments to select
        /// Hiển thị màn hình chọn bài tập hoặc danh sách bài nộp để chấm điểm
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(int? assignmentId)
        {
            if (!assignmentId.HasValue)
            {
                // Nếu chưa nhận assignmentId => hiện danh sách bài tập để chọn
                var assignments = await _assignmentRepository.GetAllAsync();
                return View("SelectAssignment", assignments);
            }

            // Lấy thông tin bài tập từ repository
            var assignment = await _assignmentRepository.GetByIdAsync(assignmentId.Value);
            if (assignment == null)
            {
                // Nếu không tìm thấy, báo lỗi
                TempData["ErrorMessage"] = "Assignment not found."; // Bài tập không tồn tại
                return RedirectToAction("Index", "Home");
            }

            // Lấy danh sách bài nộp của assignment này
            var submissions = await _gradingService.GetSubmissionsByAssignmentAsync(assignmentId.Value);
            // Truyền thêm các thông tin cần thiết cho View
            ViewBag.AssignmentId = assignmentId.Value;
            ViewBag.AssignmentTitle = assignment.Title;
            ViewBag.MaxScore = assignment.MaxScore;

            return View(submissions);
        }

        /// <summary>
        /// GET: Grading/Grade/{id}
        /// Step 3-4: Displays submission details for grading
        /// Hiển thị chi tiết bài nộp để chấm điểm
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Grade(int? id)
        {
            if (!id.HasValue)
            {
                // Nếu không có id bài nộp, báo lỗi về Home
                TempData["ErrorMessage"] = "Submission ID is required."; // Cần có ID bài nộp
                return RedirectToAction("Index", "Home");
            }

            // Lấy thông tin bài nộp (GradeSubmissionDto)
            var gradeDto = await _gradingService.GetSubmissionForGradingAsync(id.Value);
            if (gradeDto == null)
            {
                // A3.1: Submission Not Available
                // Nếu không có bài nộp (đã bị xóa, ...), báo lỗi
                TempData["ErrorMessage"] = "Submission not found or is no longer accessible."; // Bài nộp không tồn tại hoặc không truy cập được
                return RedirectToAction("Index", "Home");
            }

            // Hiển thị view nhập điểm và nhận xét
            return View(gradeDto);
        }

        /// <summary>
        /// POST: Grading/Grade
        /// Step 5-8: Handles grade submission
        /// Xử lý việc gửi điểm và nhận xét bài nộp (post form chấm điểm)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Grade(GradeSubmissionDto gradeDto)
        {
            if (!ModelState.IsValid)
            {
                // A7.1: Invalid Grade Input
                // Nếu dữ liệu nhập không hợp lệ, load lại thông tin bài nộp để hiện lỗi trên giao diện
                var submission = await _gradingService.GetSubmissionForGradingAsync(gradeDto.SubmissionId);
                if (submission != null)
                {
                    // Điền lại dữ liệu phụ trợ (tên sv, tiêu đề bài,...)
                    gradeDto.StudentName = submission.StudentName;
                    gradeDto.AssignmentTitle = submission.AssignmentTitle;
                    gradeDto.SubmittedAt = submission.SubmittedAt;
                    gradeDto.MaxScore = submission.MaxScore;
                }
                return View(gradeDto);
            }

            // Gọi dịch vụ xử lý chấm điểm
            var result = await _gradingService.GradeSubmissionAsync(gradeDto);

            if (!result.IsSuccess)
            {
                // A3.1: Submission Not Available
                // A7.1: Invalid Grade Input
                // E8.1: Database update failure
                // Nếu chấm điểm thất bại, báo lỗi
                TempData["ErrorMessage"] = result.ErrorMessage ?? "Failed to save grade."; // Không lưu được điểm

                // Nạp lại thông tin cần thiết cho view
                var submission = await _gradingService.GetSubmissionForGradingAsync(gradeDto.SubmissionId);
                if (submission != null)
                {
                    gradeDto.StudentName = submission.StudentName;
                    gradeDto.AssignmentTitle = submission.AssignmentTitle;
                    gradeDto.SubmittedAt = submission.SubmittedAt;
                    gradeDto.MaxScore = submission.MaxScore;
                }
                return View(gradeDto);
            }

            // Step 8: Success confirmation
            // Nếu thành công, thông báo thành công cho người dùng
            TempData["SuccessMessage"] = "Grade and feedback saved successfully."; // Chấm điểm và nhận xét thành công

            // Lấy assignmentId để chuyển về trang danh sách bài nộp của assignment đó
            var submissionDetails = await _gradingService.GetSubmissionForGradingAsync(gradeDto.SubmissionId);
            var assignmentId = submissionDetails?.AssignmentId ?? 0;

            // Quay lại trang danh sách bài nộp sau khi chấm xong
            return RedirectToAction(nameof(Index), new { assignmentId = assignmentId });
        }

        /// <summary>
        /// GET: Grading/View/{id}
        /// View submission details without editing
        /// Xem chi tiết bài nộp (không cho chấm, chỉ xem)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> View(int? id)
        {
            if (!id.HasValue)
            {
                // Nếu không truyền id => báo lỗi
                TempData["ErrorMessage"] = "Submission ID is required."; // Cần có ID bài nộp
                return RedirectToAction("Index", "Home");
            }

            // Lấy dữ liệu chi tiết bài nộp
            var gradeDto = await _gradingService.GetSubmissionForGradingAsync(id.Value);
            if (gradeDto == null)
            {
                TempData["ErrorMessage"] = "Submission not found."; // Không tìm thấy bài nộp
                return RedirectToAction("Index", "Home");
            }

            // Hiển thị view xem chi tiết bài nộp
            return View(gradeDto);
        }
    }
}
