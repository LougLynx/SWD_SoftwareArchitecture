using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_SoftwareArchitecture.Models
{
    public class Progress
    {
        [Key]
        public int ProgressId { get; set; }

        [Required]
        public int EnrollmentId { get; set; }

        [Required]
        public int LessonId { get; set; }

        public int CompletedLessons { get; set; }

        public DateTime? LastAccessTime { get; set; }

        // Navigation Properties
        [ForeignKey("EnrollmentId")]
        public virtual Enrollment? Enrollment { get; set; }

        [ForeignKey("LessonId")]
        public virtual Lesson? Lesson { get; set; }
    }
}

