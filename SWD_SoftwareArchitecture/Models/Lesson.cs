using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_SoftwareArchitecture.Models
{
    public class Lesson
    {
        [Key]
        public int LessonId { get; set; }

        [Required]
        public int ModuleId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(50)]
        public string? ContentType { get; set; }

        [StringLength(500)]
        public string? ContentUrl { get; set; }

        public int OrderIndex { get; set; }

        // Navigation Properties
        [ForeignKey("ModuleId")]
        public virtual Module? Module { get; set; }

        public virtual ICollection<Progress> Progresses { get; set; } = new List<Progress>();
    }
}

