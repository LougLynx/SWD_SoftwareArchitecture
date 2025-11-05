using SWD_SoftwareArchitecture.Variants.Strategies.Interfaces;

namespace SWD_SoftwareArchitecture.Variants.Strategies
{
    /// <summary>
    /// Factory for creating enrollment strategies - Variability Point
    /// </summary>
    public class EnrollmentStrategyFactory
    {
        private readonly IEnumerable<IEnrollmentStrategy> _strategies;

        public EnrollmentStrategyFactory(IEnumerable<IEnrollmentStrategy> strategies)
        {
            _strategies = strategies;
        }

        /// <summary>
        /// Get appropriate strategy based on enrollment type
        /// </summary>
        public IEnrollmentStrategy GetStrategy(string enrollmentType = "Standard")
        {
            var strategy = _strategies.FirstOrDefault(s => s.CanHandle(enrollmentType));
            return strategy ?? _strategies.First(); // Fallback to first strategy
        }
    }
}

