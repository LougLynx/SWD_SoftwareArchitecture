using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_SoftwareArchitecture.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;

        public DateTime SentDate { get; set; }

        public bool IsRead { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}

