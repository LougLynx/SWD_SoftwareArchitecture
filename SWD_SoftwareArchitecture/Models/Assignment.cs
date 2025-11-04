using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_SoftwareArchitecture.Models
{
    public class Assignment
    {
        [Key]
        public int AssignmentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public DateTime DueDate { get; set; }

        public double MaxScore { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        [ForeignKey("CourseId")]
        public virtual Course? Course { get; set; }

        public virtual ICollection<AssignmentSubmission> AssignmentSubmissions { get; set; } = new List<AssignmentSubmission>();
    }
}

