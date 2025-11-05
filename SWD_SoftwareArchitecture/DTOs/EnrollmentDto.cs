using System.ComponentModel.DataAnnotations;

namespace SWD_SoftwareArchitecture.DTOs
{
    /// <summary>
    /// Data Transfer Object for Enrollment operations
    /// Used for transferring enrollment data between layers
    /// </summary>
    public class EnrollmentDto
    {
        public int? EnrollmentId { get; set; }

        [Required(ErrorMessage = "Student is required")]
        [Display(Name = "Student")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Course is required")]
        [Display(Name = "Course")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Enrollment date is required")]
        [Display(Name = "Enrollment Date")]
        [DataType(DataType.Date)]
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Status is required")]
        [StringLength(50)]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Active";

        [Range(0, 100, ErrorMessage = "Progress must be between 0 and 100")]
        [Display(Name = "Progress (%)")]
        public double ProgressPercent { get; set; } = 0;

        // Display properties
        [Display(Name = "Student Name")]
        public string? StudentName { get; set; }

        [Display(Name = "Course Title")]
        public string? CourseTitle { get; set; }
    }
}

