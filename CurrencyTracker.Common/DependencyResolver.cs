using Microsoft.Extensions.DependencyInjection;
using System;

namespace CurrencyTracker.Common
{
    public static class DependencyResolver
    {
        private static readonly IServiceProvider serviceProvider;

        public static void Initialize(IServiceProvider serviceProvider)
        {
            Console.WriteLine($"_serviceProvider в DependencyResolver: {serviceProvider?.GetHashCode() ?? 0}");
            serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public static T GetService<T>() where T : class
        {
            Console.WriteLine($"_serviceProvider в DependencyResolver: {serviceProvider?.GetHashCode() ?? 0}");
            if (serviceProvider == null)
                throw new InvalidOperationException("DependencyResolver is not initialized. Call Initialize() in Program.cs.");

            return serviceProvider.GetRequiredService<T>();
        }
    }
}
