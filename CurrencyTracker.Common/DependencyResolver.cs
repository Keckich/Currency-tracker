using Microsoft.Extensions.DependencyInjection;

namespace CurrencyTracker.Common
{
    public static class DependencyResolver
    {
        private static IServiceProvider _serviceProvider;

        public static void Initialize(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public static T GetService<T>() where T : class
        {
            if (_serviceProvider == null)
                throw new InvalidOperationException("DependencyResolver is not initialized. Call Initialize() in Program.cs.");

            return _serviceProvider.GetRequiredService<T>();
        }
    }
}
