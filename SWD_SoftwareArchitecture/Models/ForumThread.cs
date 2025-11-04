using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_SoftwareArchitecture.Models
{
    public class ForumThread
    {
        [Key]
        public int ThreadId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        [ForeignKey("CourseId")]
        public virtual Course? Course { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public virtual ICollection<ForumPost> ForumPosts { get; set; } = new List<ForumPost>();
    }
}

