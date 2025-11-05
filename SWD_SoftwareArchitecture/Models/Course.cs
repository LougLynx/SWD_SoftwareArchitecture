using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SWD_SoftwareArchitecture.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = string.Empty;

        [StringLength(100)]
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Maximum capacity for course enrollments (BR-02: Course maximum capacity must not be exceeded)
        /// </summary>
        public int? MaxCapacity { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public virtual ICollection<Module> Modules { get; set; } = new List<Module>();
        public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
        public virtual ICollection<ForumThread> ForumThreads { get; set; } = new List<ForumThread>();
    }
}

