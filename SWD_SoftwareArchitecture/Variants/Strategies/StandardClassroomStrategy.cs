using SWD_SoftwareArchitecture.Core.Common;
using SWD_SoftwareArchitecture.DTOs;
using SWD_SoftwareArchitecture.Models;
using SWD_SoftwareArchitecture.Repositories.Interfaces;
using SWD_SoftwareArchitecture.Services;
using SWD_SoftwareArchitecture.Services.Interfaces; // Cho IProgressService
using SWD_SoftwareArchitecture.Variants.Strategies.Interfaces;

namespace SWD_SoftwareArchitecture.Variants.Strategies
{
    public class StandardClassroomStrategy : IClassroomStrategy
    {
        private readonly ILessonService _LessonService;

        public StandardClassroomStrategy(ILessonService LessonService)
        {
            _LessonService = LessonService;
        }

        public string VariabilityPointName => "ClassroomStrategy";

        public bool CanHandle(string ClassroomType)
        {
            return ClassroomType == "Standard" || string.IsNullOrEmpty(ClassroomType);
        }

        public async Task<ServiceResult<string>> GetOnlineClassLinkAsync(int lessonId, int studentId)
        {
            return await _LessonService.GetOnlineClassLinkAsync(lessonId, studentId);
        }
    }
}