namespace SWD_SoftwareArchitecture.Core.Abstractions
{
    /// <summary>
    /// Marker interface for variability points in SPL architecture
    /// </summary>
    public interface IVariabilityPoint
    {
        /// <summary>
        /// Name of the variability point
        /// </summary>
        string VariabilityPointName { get; }
    }
}

