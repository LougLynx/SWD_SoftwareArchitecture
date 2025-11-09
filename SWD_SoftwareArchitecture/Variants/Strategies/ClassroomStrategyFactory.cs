using SWD_SoftwareArchitecture.Variants.Strategies.Interfaces;

namespace SWD_SoftwareArchitecture.Variants.Strategies
{
    /// <summary>
    /// Factory for creating Classroom strategies - Variability Point
    /// </summary>
    public class ClassroomStrategyFactory
    {
        private readonly IEnumerable<IClassroomStrategy> _strategies;

        public ClassroomStrategyFactory(IEnumerable<IClassroomStrategy> strategies)
        {
            _strategies = strategies;
        }

        /// <summary>
        /// Get appropriate strategy based on Classroom type
        /// </summary>
        public IClassroomStrategy GetStrategy(string ClassroomType = "Standard")
        {
            var strategy = _strategies.FirstOrDefault(s => s.CanHandle(ClassroomType));
            return strategy ?? _strategies.First(); // Fallback to first strategy
        }
    }
}

