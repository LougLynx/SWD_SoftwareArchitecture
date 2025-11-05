namespace SWD_SoftwareArchitecture.DTOs
{
    /// <summary>
    /// DTO for displaying enrollment list with course information
    /// </summary>
    public class EnrollmentListDto
    {
        public int EnrollmentId { get; set; }
        public int UserId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentEmail { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public double ProgressPercent { get; set; }
    }
}

