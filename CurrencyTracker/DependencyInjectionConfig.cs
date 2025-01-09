using CurrencyTracker.Services.Interfaces;
using CurrencyTracker.Services;

namespace CurrencyTracker
{
    public static class DependencyInjectionConfig
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ITradeService, TradeService>();
            services.AddScoped<IBinanceService, BinanceService>();
        }
    }
}
