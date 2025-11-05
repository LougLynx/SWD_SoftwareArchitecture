using System.ComponentModel.DataAnnotations;

namespace SWD_SoftwareArchitecture.DTOs
{
    /// <summary>
    /// Data Transfer Object for grading submissions
    /// Used for transferring grading data between layers
    /// </summary>
    public class GradeSubmissionDto
    {
        public int SubmissionId { get; set; }

        [Required(ErrorMessage = "Grade is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Grade must be a valid number")]
        [Display(Name = "Score")]
        public float Grade { get; set; }

        [StringLength(1000, ErrorMessage = "Feedback cannot exceed 1000 characters")]
        [Display(Name = "Feedback")]
        public string? Feedback { get; set; }

        // Display properties
        [Display(Name = "Student Name")]
        public string? StudentName { get; set; }

        [Display(Name = "Assignment Title")]
        public string? AssignmentTitle { get; set; }

        [Display(Name = "Submitted At")]
        public DateTime SubmittedAt { get; set; }

        [Display(Name = "Max Score")]
        public double MaxScore { get; set; }

        public int AssignmentId { get; set; }
        public int UserId { get; set; }
    }
}

