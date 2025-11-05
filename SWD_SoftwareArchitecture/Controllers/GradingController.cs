using Microsoft.AspNetCore.Mvc;
using SWD_SoftwareArchitecture.DTOs;
using SWD_SoftwareArchitecture.Services.Interfaces;
using SWD_SoftwareArchitecture.Repositories.Interfaces;

namespace SWD_SoftwareArchitecture.Controllers
{
    /// <summary>
    /// Grading Controller following MVC pattern
    /// Implements Grade Submissions use case
    /// </summary>
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
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(int? assignmentId)
        {
            if (!assignmentId.HasValue)
            {
                // Show list of assignments to select
                var assignments = await _assignmentRepository.GetAllAsync();
                return View("SelectAssignment", assignments);
            }

            var assignment = await _assignmentRepository.GetByIdAsync(assignmentId.Value);
            if (assignment == null)
            {
                TempData["ErrorMessage"] = "Assignment not found.";
                return RedirectToAction("Index", "Home");
            }

            var submissions = await _gradingService.GetSubmissionsByAssignmentAsync(assignmentId.Value);
            ViewBag.AssignmentId = assignmentId.Value;
            ViewBag.AssignmentTitle = assignment.Title;
            ViewBag.MaxScore = assignment.MaxScore;

            return View(submissions);
        }

        /// <summary>
        /// GET: Grading/Grade/{id}
        /// Step 3-4: Displays submission details for grading
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Grade(int? id)
        {
            if (!id.HasValue)
            {
                TempData["ErrorMessage"] = "Submission ID is required.";
                return RedirectToAction("Index", "Home");
            }

            var gradeDto = await _gradingService.GetSubmissionForGradingAsync(id.Value);
            if (gradeDto == null)
            {
                // A3.1: Submission Not Available
                TempData["ErrorMessage"] = "Submission not found or is no longer accessible.";
                return RedirectToAction("Index", "Home");
            }

            return View(gradeDto);
        }

        /// <summary>
        /// POST: Grading/Grade
        /// Step 5-8: Handles grade submission
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Grade(GradeSubmissionDto gradeDto)
        {
            if (!ModelState.IsValid)
            {
                // A7.1: Invalid Grade Input
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

            var result = await _gradingService.GradeSubmissionAsync(gradeDto);

            if (!result.IsSuccess)
            {
                // A3.1: Submission Not Available
                // A7.1: Invalid Grade Input
                // E8.1: Database update failure
                TempData["ErrorMessage"] = result.ErrorMessage ?? "Failed to save grade.";
                
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
            TempData["SuccessMessage"] = "Grade and feedback saved successfully.";
            
            // Get assignment ID to redirect back to submissions list
            var submissionDetails = await _gradingService.GetSubmissionForGradingAsync(gradeDto.SubmissionId);
            var assignmentId = submissionDetails?.AssignmentId ?? 0;
            
            return RedirectToAction(nameof(Index), new { assignmentId = assignmentId });
        }

        /// <summary>
        /// GET: Grading/View/{id}
        /// View submission details without editing
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> View(int? id)
        {
            if (!id.HasValue)
            {
                TempData["ErrorMessage"] = "Submission ID is required.";
                return RedirectToAction("Index", "Home");
            }

            var gradeDto = await _gradingService.GetSubmissionForGradingAsync(id.Value);
            if (gradeDto == null)
            {
                TempData["ErrorMessage"] = "Submission not found.";
                return RedirectToAction("Index", "Home");
            }

            return View(gradeDto);
        }
    }
}

