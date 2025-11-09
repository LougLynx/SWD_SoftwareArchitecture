using SWD_SoftwareArchitecture.Features;
using SWD_SoftwareArchitecture.Repositories;
using SWD_SoftwareArchitecture.Repositories.Interfaces;
using SWD_SoftwareArchitecture.Services;
using SWD_SoftwareArchitecture.Services.Interfaces;
using SWD_SoftwareArchitecture.Variants.Strategies;
using SWD_SoftwareArchitecture.Variants.Strategies.Interfaces;

namespace SWD_SoftwareArchitecture.Core
{
    /// <summary>
    /// Service registration extensions for SPL architecture
    /// Provides feature-based service registration
    /// </summary>
    public static class ServiceRegistrationExtensions
    {
        /// <summary>
        /// Register core services (always available)
        /// </summary>
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            // Register Feature Manager
            services.AddSingleton<FeatureManager>();

            // Register Product Configuration
            services.AddScoped<Core.Abstractions.IProductConfiguration, ProductConfiguration>();

            // Register Repositories (Core)
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
            services.AddScoped<ISubmissionRepository, SubmissionRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAssignmentRepository, AssignmentRepository>();
            services.AddScoped<IProgressRepository, ProgressRepository>();
            

            return services;
        }

        /// <summary>
        /// Register feature-based services (conditional registration)
        /// </summary>
        public static IServiceCollection AddFeatureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var featureManager = new FeatureManager(configuration);

            // Core Services (always enabled)
            services.AddScoped<IEnrollmentService, EnrollmentService>();
            services.AddScoped<IGradingService, GradingService>();
            services.AddScoped<IProgressService, ProgressService>();

            services.AddScoped<ILessonService, LessonService>();

            // Enrollment Strategy Pattern (Variability Point)
            services.AddScoped<IEnrollmentStrategy, StandardEnrollmentStrategy>();
            services.AddScoped<EnrollmentStrategyFactory>();

            // Grading Strategy Pattern (Variability Point)
            services.AddScoped<IGradingStrategy, StandardGradingStrategy>();
            services.AddScoped<GradingStrategyFactory>();

            // Conditional feature registration
            if (featureManager.IsEnabled(FeatureFlags.AdvancedReporting))
            {
                // Register advanced reporting services if enabled
                // services.AddScoped<IAdvancedReportingService, AdvancedReportingService>();
            }

            if (featureManager.IsEnabled(FeatureFlags.ForumDiscussion))
            {
                // Register forum services if enabled
                // services.AddScoped<IForumService, ForumService>();
            }

            if (featureManager.IsEnabled(FeatureFlags.CertificationSystem))
            {
                services.AddScoped<ICertificateRepository, CertificateRepository>();
                services.AddScoped<ICertificateService, CertificateService>();
                services.AddScoped<ICertificateStrategy, StandardCertificateStrategy>();
                services.AddScoped<ICertificateStrategy, NoOpCertificateStrategy>();
                services.AddScoped<CertificateStrategyFactory>();
            }

            if (featureManager.IsEnabled(FeatureFlags.VirtualClassroom))
            {
                services.AddScoped<ILessonRepository, LessonRepository>();
                services.AddScoped<ILessonService, LessonService>();
                services.AddScoped<IClassroomStrategy, StandardClassroomStrategy>();
                services.AddScoped<ClassroomStrategyFactory>();
            }

            return services;
        }
    }
}

