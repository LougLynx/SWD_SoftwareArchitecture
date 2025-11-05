using Microsoft.Extensions.Configuration;

namespace SWD_SoftwareArchitecture.Features
{
    /// <summary>
    /// Feature Manager for SPL Architecture
    /// Manages feature flags and feature availability
    /// </summary>
    public class FeatureManager
    {
        private readonly IConfiguration _configuration;
        private readonly Dictionary<string, bool> _features;

        public FeatureManager(IConfiguration configuration)
        {
            _configuration = configuration;
            _features = new Dictionary<string, bool>();
            LoadFeatures();
        }

        private void LoadFeatures()
        {
            var featuresSection = _configuration.GetSection("Features");
            if (featuresSection.Exists())
            {
                foreach (var feature in featuresSection.GetChildren())
                {
                    _features[feature.Key] = feature.Get<bool>();
                }
            }

            // Load default features
            LoadDefaultFeatures();
        }

        private void LoadDefaultFeatures()
        {
            // Set default values if not configured
            if (!_features.ContainsKey(FeatureFlags.EnrollmentManagement))
                _features[FeatureFlags.EnrollmentManagement] = true;

            if (!_features.ContainsKey(FeatureFlags.GradingSystem))
                _features[FeatureFlags.GradingSystem] = true;

            if (!_features.ContainsKey(FeatureFlags.AdvancedReporting))
                _features[FeatureFlags.AdvancedReporting] = false;

            if (!_features.ContainsKey(FeatureFlags.ForumDiscussion))
                _features[FeatureFlags.ForumDiscussion] = false;

            if (!_features.ContainsKey(FeatureFlags.CertificationSystem))
                _features[FeatureFlags.CertificationSystem] = false;
        }

        /// <summary>
        /// Check if a feature is enabled
        /// </summary>
        public bool IsEnabled(string featureName)
        {
            return _features.ContainsKey(featureName) && _features[featureName];
        }

        /// <summary>
        /// Get all enabled features
        /// </summary>
        public IEnumerable<string> GetEnabledFeatures()
        {
            return _features.Where(f => f.Value).Select(f => f.Key);
        }

        /// <summary>
        /// Get all features with their status
        /// </summary>
        public Dictionary<string, bool> GetAllFeatures()
        {
            return new Dictionary<string, bool>(_features);
        }
    }

    /// <summary>
    /// Feature flags constants for SPL variability points
    /// </summary>
    public static class FeatureFlags
    {
        // Core Features (Common)
        public const string EnrollmentManagement = "EnrollmentManagement";
        public const string GradingSystem = "GradingSystem";
        
        // Optional Features (Variants)
        public const string AdvancedReporting = "AdvancedReporting";
        public const string ForumDiscussion = "ForumDiscussion";
        public const string CertificationSystem = "CertificationSystem";
        public const string MessagingSystem = "MessagingSystem";
        public const string QuizSystem = "QuizSystem";
    }
}

