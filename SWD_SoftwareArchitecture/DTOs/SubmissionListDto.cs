namespace SWD_SoftwareArchitecture.DTOs
{
    /// <summary>
    /// DTO for displaying submission list with assignment information
    /// </summary>
    public class SubmissionListDto
    {
        public int SubmissionId { get; set; }
        public int AssignmentId { get; set; }
        public string AssignmentTitle { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentEmail { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public float? Grade { get; set; }
        public string? Feedback { get; set; }
        public double MaxScore { get; set; }
        public bool IsGraded => Grade.HasValue;
    }
}

