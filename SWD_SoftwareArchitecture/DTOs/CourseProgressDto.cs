namespace SWD_SoftwareArchitecture.DTOs
{
    // DTO cho bài học (Lesson)
    public class LessonProgressDto
    {
        public string Title { get; set; }
        public bool IsCompleted { get; set; } // "Identification marks (e.g., tick marks)"
    }

    // DTO cho bài tập (Assignment)
    public class AssignmentProgressDto
    {
        public string Title { get; set; }
        public string Score { get; set; } // "Scores of graded assignments/tests"
    }

    // DTO chính cho trang Progress
    public class CourseProgressDto
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        
        // "Overall completion percentage"
        public double CompletionPercentage { get; set; } 
        
        // "List of chapters, lessons, activities"
        public List<LessonProgressDto> Lessons { get; set; } = new();
        
        // "Scores of graded assignments/tests"
        public List<AssignmentProgressDto> Assignments { get; set; } = new();
        
        // Thuộc tính trợ giúp để hiển thị
        public int TotalLessons { get; set; }
        public int CompletedLessons { get; set; }
    }
}