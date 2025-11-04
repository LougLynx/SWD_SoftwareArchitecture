using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_SoftwareArchitecture.Models
{
    public class QuizAttempt
    {
        [Key]
        public int AttemptId { get; set; }

        [Required]
        public int QuizId { get; set; }

        [Required]
        public int UserId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public double? Score { get; set; }

        // Navigation Properties
        [ForeignKey("QuizId")]
        public virtual Quiz? Quiz { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}

