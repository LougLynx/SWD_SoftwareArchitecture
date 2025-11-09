using System.ComponentModel.DataAnnotations;

namespace SWD_SoftwareArchitecture.DTOs
{
    /// <summary>
    /// Data Transfer Object for Enrollment operations
    /// Used for transferring enrollment data between layers
    /// </summary>
    public class CertificateDto
    {
        public string CertificateId { get; set; }
        public string CertificateUrl { get; set; } // Link PDF
        public DateTime IssueDate { get; set; }
    }
}

