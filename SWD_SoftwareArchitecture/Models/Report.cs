using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWD_SoftwareArchitecture.Models
{
    public class Report
    {
        [Key]
        public int ReportId { get; set; }

        [Required]
        public int UserId { get; set; }

        public DateTime GeneratedAt { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}

