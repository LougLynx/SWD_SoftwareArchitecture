using SWD_SoftwareArchitecture.Features;
using SWD_SoftwareArchitecture.Repositories;
using SWD_SoftwareArchitecture.Repositories.Interfaces;
using SWD_SoftwareArchitecture.Services;
using SWD_SoftwareArchitecture.Services.Interfaces;
using SWD_SoftwareArchitecture.Variants.Strategies;
using SWD_SoftwareArchitecture.Variants.Strategies.Interfaces;
using System.Reflection;

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
                // Register certification services if enabled
                // services.AddScoped<ICertificationService, CertificationService>();
            }

            return services;
        }
        public static void RegisterRepositories(this IServiceCollection services)
        {
            // Quét và đăng ký tất cả các repository
            var repositoryInterfaces = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsInterface && t.Name.EndsWith("Repository") && t.Name != "IRepository");

            foreach (var repoInterface in repositoryInterfaces)
            {
                var repoImplementation = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .FirstOrDefault(t => t.IsClass && !t.IsAbstract && repoInterface.IsAssignableFrom(t));

                if (repoImplementation != null)
                {
                    services.AddScoped(repoInterface, repoImplementation);
                }
            }
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        }

        public static void RegisterServices(this IServiceCollection services)
        {
            // Đăng ký các service
            services.AddScoped<IEnrollmentService, EnrollmentService>();
            services.AddScoped<IGradingService, GradingService>();

            // Thêm đăng ký cho UserService
            services.AddScoped<IUserService, UserService>();
        }
    }
}

