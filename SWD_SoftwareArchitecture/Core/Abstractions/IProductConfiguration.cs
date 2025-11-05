namespace SWD_SoftwareArchitecture.Core.Abstractions
{
    /// <summary>
    /// Product configuration interface for SPL
    /// Defines configuration for different product variants
    /// </summary>
    public interface IProductConfiguration
    {
        string ProductName { get; }
        string ProductVariant { get; }
        IEnumerable<string> EnabledFeatures { get; }
        bool IsFeatureEnabled(string featureName);
    }
}

