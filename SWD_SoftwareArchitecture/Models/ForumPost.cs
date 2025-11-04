using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_SoftwareArchitecture.Models
{
    public class ForumPost
    {
        [Key]
        public int PostId { get; set; }

        [Required]
        public int ThreadId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(5000)]
        public string Content { get; set; } = string.Empty;

        public DateTime PostDate { get; set; }

        // Navigation Properties
        [ForeignKey("ThreadId")]
        public virtual ForumThread? ForumThread { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}

