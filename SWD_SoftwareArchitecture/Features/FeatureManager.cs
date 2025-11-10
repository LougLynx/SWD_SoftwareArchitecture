using Microsoft.Extensions.Configuration;

namespace SWD_SoftwareArchitecture.Features
{
    /// <summary>
    /// Quản lý tính năng cho kiến trúc SPL
    /// Quản lý các tính năng và khả năng khả dụng của tính năng
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

        /// Tải các tính năng từ cấu hình và thiết lập giá trị mặc định
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

            // Tải các tính năng mặc định (nếu chưa cấu hình)
            LoadDefaultFeatures();
        }

        /// Thiết lập giá trị mặc định cho các tính năng nếu chưa có trong cấu hình
        private void LoadDefaultFeatures()
        {
            // Thiết lập giá trị mặc định nếu chưa cấu hình trong file cấu hình
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

        /// Kiểm tra một tính năng có đang bật hay không
        public bool IsEnabled(string featureName)
        {
            return _features.ContainsKey(featureName) && _features[featureName];
        }

        /// Lấy danh sách các tính năng đang bật
        public IEnumerable<string> GetEnabledFeatures()
        {
            return _features.Where(f => f.Value).Select(f => f.Key);
        }

        /// Lấy tất cả các tính năng cùng trạng thái bật/tắt của chúng
        public Dictionary<string, bool> GetAllFeatures()
        {
            return new Dictionary<string, bool>(_features);
        }

        /// Lấy product variant từ configuration (SPL Architecture)
        public string GetProductVariant()
        {
            return _configuration["Product:Variant"] ?? "Standard";
        }
    }

    /// Các hằng số cờ tính năng cho các điểm biến đổi trong SPL
    public static class FeatureFlags
    {
        // Tính năng cốt lõi (chung)
        public const string EnrollmentManagement = "EnrollmentManagement";
        public const string GradingSystem = "GradingSystem";
        
        // Tính năng tùy chọn (biến thể)
        public const string AdvancedReporting = "AdvancedReporting";
        public const string ForumDiscussion = "ForumDiscussion";
        public const string CertificationSystem = "CertificationSystem";
        public const string MessagingSystem = "MessagingSystem";
        public const string QuizSystem = "QuizSystem";
    }
}

