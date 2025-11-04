using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_SoftwareArchitecture.Models
{
    public class Quiz
    {
        [Key]
        public int QuizId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public int TimeLimit { get; set; }

        public int TotalQuestion { get; set; }

        // Navigation Properties
        [ForeignKey("CourseId")]
        public virtual Course? Course { get; set; }

        public virtual ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}

