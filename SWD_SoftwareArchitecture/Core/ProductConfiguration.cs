using SWD_SoftwareArchitecture.Features;
using SWD_SoftwareArchitecture.Core.Abstractions;

namespace SWD_SoftwareArchitecture.Core
{
    /// <summary>
    /// Product configuration implementation for SPL
    /// </summary>
    public class ProductConfiguration : IProductConfiguration
    {
        private readonly FeatureManager _featureManager;
        private readonly IConfiguration _configuration;

        public ProductConfiguration(FeatureManager featureManager, IConfiguration configuration)
        {
            _featureManager = featureManager;
            _configuration = configuration;
            ProductName = _configuration["Product:Name"] ?? "LearningManagementSystem";
            ProductVariant = _configuration["Product:Variant"] ?? "Standard";
        }

        public string ProductName { get; }
        public string ProductVariant { get; }

        public IEnumerable<string> EnabledFeatures => _featureManager.GetEnabledFeatures();

        public bool IsFeatureEnabled(string featureName)
        {
            return _featureManager.IsEnabled(featureName);
        }
    }
}

