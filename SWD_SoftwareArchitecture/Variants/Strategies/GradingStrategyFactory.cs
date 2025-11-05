using SWD_SoftwareArchitecture.Variants.Strategies.Interfaces;

namespace SWD_SoftwareArchitecture.Variants.Strategies
{
    /// <summary>
    /// Factory for creating grading strategies - Variability Point
    /// </summary>
    public class GradingStrategyFactory
    {
        private readonly IEnumerable<IGradingStrategy> _strategies;

        public GradingStrategyFactory(IEnumerable<IGradingStrategy> strategies)
        {
            _strategies = strategies;
        }

        /// <summary>
        /// Get appropriate strategy based on grading type
        /// </summary>
        public IGradingStrategy GetStrategy(string gradingType = "Standard")
        {
            var strategy = _strategies.FirstOrDefault(s => s.CanHandle(gradingType));
            return strategy ?? _strategies.First(); // Fallback to first strategy
        }
    }
}

