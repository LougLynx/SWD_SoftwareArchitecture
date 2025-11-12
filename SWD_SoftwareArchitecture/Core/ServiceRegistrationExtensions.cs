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
            services.AddScoped<IEnrollmentRepository, EnrollmentRepository>(); 
            services.AddScoped<ISubmissionRepository, SubmissionRepository>(); 
            services.AddScoped<ICourseRepository, CourseRepository>();         
            services.AddScoped<IUserRepository, UserRepository>();            
            services.AddScoped<IAssignmentRepository, AssignmentRepository>();

            return services;
        }

        /// Sử dụng Singleton Pattern (FeatureManager) + SPL Architecture với conditional logic
        public static IServiceCollection AddFeatureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Tạo FeatureManager instance để check features (sẽ được đăng ký Singleton ở AddCoreServices)
            var featureManager = new FeatureManager(configuration);

            services.AddScoped<IEnrollmentService, EnrollmentService>(); 
            services.AddScoped<IGradingService, GradingService>();       
            services.AddScoped<IUserService, UserService>();             

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
