using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_SoftwareArchitecture.Models
{
    public class AssignmentSubmission
    {
        [Key]
        public int SubmissionId { get; set; }

        [Required]
        public int AssignmentId { get; set; }

        [Required]
        public int UserId { get; set; }

        public DateTime SubmittedAt { get; set; }

        public float? Grade { get; set; }

        [StringLength(1000)]
        public string? Feedback { get; set; }

        // Navigation Properties
        [ForeignKey("AssignmentId")]
        public virtual Assignment? Assignment { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}

