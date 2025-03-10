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
            services.AddScoped<IModelTrainer, ModelTrainer>();
            services.AddScoped<IGenerationTrainingDataService, GenerationTrainingDataService>();
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<IPredictionService, PredictionService>();
            services.AddScoped<IIndicatorService, IndicatorService>();
            services.AddScoped<ITradeSignalWebSocket, TradeSignalWebSocket>();
            services.AddScoped<IBinanceWebSocketService, BinanceWebSocketService>();
        }
    }
}
