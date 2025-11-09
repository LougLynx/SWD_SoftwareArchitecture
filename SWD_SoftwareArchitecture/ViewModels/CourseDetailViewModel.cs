using SWD_SoftwareArchitecture.DTOs;
using SWD_SoftwareArchitecture.Models;

namespace SWD_SoftwareArchitecture.ViewModels
{
    // DTO này chỉ dùng cho View
    public class ModuleDto
    {
        public int ModuleId { get; set; }
        public string Title { get; set; }
        public int OrderIndex { get; set; }
        public List<Lesson> Lessons { get; set; } = new();
    }
    
    public class CourseDetailViewModel
    {
        public Course Course { get; set; }
        public List<ModuleDto> Modules { get; set; } = new();
        public CourseProgressDto StudentInProgress { get; set; }
        public CourseProgressDto StudentCompleted { get; set; }
    }
}