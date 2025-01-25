using CurrencyTracker.Business.Services.Interfaces;
using CurrencyTracker.Business.Services;

namespace CurrencyTracker
{
    public static class DependencyInjectionConfig
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ITradeService, TradeService>();
            services.AddScoped<IBinanceService, BinanceService>();
            services.AddScoped<ICandlestickPatternAnalyzer, CandlestickPatternAnalyzer>();
        }
    }
}
