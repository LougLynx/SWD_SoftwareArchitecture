using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_SoftwareArchitecture.Models
{
    public class Certification
    {
        [Key]
        [StringLength(100)]
        public string CertificateId { get; set; } = string.Empty;

        [Required]
        public int UserId { get; set; }

        public DateTime IssueDate { get; set; }

        [StringLength(500)]
        public string? CertificateUrl { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}

