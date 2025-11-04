using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_SoftwareArchitecture.Models
{
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }

        [Required]
        public int QuizId { get; set; }

        [Required]
        [StringLength(2000)]
        public string Content { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string CorrectAnswer { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string QuestionType { get; set; } = string.Empty;

        // Navigation Properties
        [ForeignKey("QuizId")]
        public virtual Quiz? Quiz { get; set; }
    }
}

