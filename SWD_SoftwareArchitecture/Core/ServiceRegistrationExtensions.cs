using SWD_SoftwareArchitecture.Features;
using SWD_SoftwareArchitecture.Repositories;
using SWD_SoftwareArchitecture.Repositories.Interfaces;
using SWD_SoftwareArchitecture.Services;
using SWD_SoftwareArchitecture.Services.Interfaces;

namespace SWD_SoftwareArchitecture.Core
{
    /// <summary>
    /// Service registration extensions for SPL architecture
    /// </summary>
    public static class ServiceRegistrationExtensions
    {

        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            // Đăng ký FeatureManager dưới dạng singleton (duy nhất trong toàn bộ ứng dụng)
            services.AddSingleton<FeatureManager>();

            // Đăng ký ProductConfiguration
            services.AddScoped<Core.Abstractions.IProductConfiguration, ProductConfiguration>();

            // Đăng ký các repository dùng cho chức năng cơ bản
            // Tạo generic repository cho các entity (IRepository<>)
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IEnrollmentRepository, EnrollmentRepository>(); // Đăng ký repository ghi danh
            services.AddScoped<ISubmissionRepository, SubmissionRepository>(); // Đăng ký repository bài nộp
            services.AddScoped<ICourseRepository, CourseRepository>();         // Đăng ký repository môn học
            services.AddScoped<IUserRepository, UserRepository>();             // Đăng ký repository người dùng
            services.AddScoped<IAssignmentRepository, AssignmentRepository>(); // Đăng ký repository bài tập

            return services;
        }

        /// <summary>
        /// Register feature-based services (conditional registration)
        /// Sử dụng Singleton Pattern (FeatureManager) + SPL Architecture với conditional logic
        /// </summary>
        public static IServiceCollection AddFeatureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Tạo FeatureManager instance để check features (sẽ được đăng ký Singleton ở AddCoreServices)
            var featureManager = new FeatureManager(configuration);

            // Đăng ký các dịch vụ lõi (luôn bật)
            services.AddScoped<IEnrollmentService, EnrollmentService>(); // Dịch vụ ghi danh
            services.AddScoped<IGradingService, GradingService>();       // Dịch vụ chấm điểm

            // ✅ SPL Architecture: Conditional feature registration dựa trên FeatureManager (Singleton)
            if (featureManager.IsEnabled(FeatureFlags.AdvancedReporting))
            {
                // services.AddScoped<IAdvancedReportingService, AdvancedReportingService>();
            }

            if (featureManager.IsEnabled(FeatureFlags.ForumDiscussion))
            {
                // services.AddScoped<IForumService, ForumService>();
            }

            if (featureManager.IsEnabled(FeatureFlags.CertificationSystem))
            {
                // services.AddScoped<ICertificationService, CertificationService>();
            }

            return services;
        }
    }
}
