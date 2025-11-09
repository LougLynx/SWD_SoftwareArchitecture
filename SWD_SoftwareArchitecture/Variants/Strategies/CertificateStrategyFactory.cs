using SWD_SoftwareArchitecture.Features;
using SWD_SoftwareArchitecture.Variants.Strategies.Interfaces;

namespace SWD_SoftwareArchitecture.Variants.Strategies
{
    public class CertificateStrategyFactory
    {
        private readonly IEnumerable<ICertificateStrategy> _strategies;

        public CertificateStrategyFactory(IEnumerable<ICertificateStrategy> strategies)
        {
            _strategies = strategies;
        }

        /// <summary>
        /// Get appropriate strategy based on Certificate type
        /// </summary>
        public ICertificateStrategy GetStrategy(string CertificateType = "Standard")
        {
            var strategy = _strategies.FirstOrDefault(s => s.CanHandle(CertificateType));
            return strategy ?? _strategies.First(); // Fallback to first strategy
        }
    }
}