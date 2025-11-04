using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_SoftwareArchitecture.Models
{
    public class Enrollment
    {
        [Key]
        public int EnrollmentId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int CourseId { get; set; }

        public DateTime EnrollmentDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = string.Empty;

        public double ProgressPercent { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course? Course { get; set; }

        public virtual ICollection<Progress> Progresses { get; set; } = new List<Progress>();
    }
}

