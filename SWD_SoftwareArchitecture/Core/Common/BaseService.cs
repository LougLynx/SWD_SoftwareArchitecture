using Microsoft.Extensions.Logging;

namespace SWD_SoftwareArchitecture.Core.Common
{
    /// <summary>
    /// Base service class for SPL Core components
    /// Provides common functionality shared across all products
    /// </summary>
    public abstract class BaseService
    {
        protected readonly ILogger Logger;

        protected BaseService(ILogger logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// Common validation method
        /// </summary>
        protected bool ValidateRequired<T>(T? value, string fieldName)
        {
            if (value == null)
            {
                Logger.LogWarning($"Validation failed: {fieldName} is required");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Common error handling wrapper
        /// </summary>
        protected async Task<T> ExecuteWithErrorHandlingAsync<T>(
            Func<Task<T>> operation,
            string operationName)
        {
            try
            {
                return await operation();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error executing {operationName}");
                throw;
            }
        }
    }
}

