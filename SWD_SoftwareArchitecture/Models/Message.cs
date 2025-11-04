using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_SoftwareArchitecture.Models
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }

        [Required]
        public int SenderId { get; set; }

        [Required]
        public int ReceiverId { get; set; }

        [Required]
        [StringLength(2000)]
        public string Content { get; set; } = string.Empty;

        public DateTime SendTime { get; set; }

        // Navigation Properties
        [ForeignKey("SenderId")]
        public virtual User? Sender { get; set; }

        [ForeignKey("ReceiverId")]
        public virtual User? Receiver { get; set; }
    }
}

