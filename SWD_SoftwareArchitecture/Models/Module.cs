using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_SoftwareArchitecture.Models
{
    public class Module
    {
        [Key]
        public int ModuleId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public int OrderIndex { get; set; }

        // Navigation Properties
        [ForeignKey("CourseId")]
        public virtual Course? Course { get; set; }

        public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    }
}

