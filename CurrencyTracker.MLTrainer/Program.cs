﻿using CurrencyTracker.Business.Data;
using CurrencyTracker.Business.Services;
using CurrencyTracker.Business.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CurrencyTracker.Business.Models;
using CurrencyTracker.Business.Models.Enums;
using CurrencyTracker.Common;

namespace MLTrainer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            DependencyResolver.Initialize(serviceProvider);

            var binanceService = serviceProvider.GetRequiredService<IBinanceService>();
            var modelTrainer = serviceProvider.GetRequiredService<IModelTrainer>();
            var patternAnalyzer = serviceProvider.GetRequiredService<ICandlestickPatternAnalyzer>();
            var predictionService = serviceProvider.GetRequiredService<IPredictionService>();
            var dataGenerationService = serviceProvider.GetRequiredService<IGenerationTrainingDataService>();
            var indicatorService = serviceProvider.GetRequiredService<IIndicatorService>();
            var onnxPredictionService = serviceProvider.GetRequiredService<IOnnxPredictionService>();

            //var candleDataXRP = (await binanceService.GetHistoricalData("XRPUSDC", "1h", 10000)).ToList();
            //var pattern = CandlestickPattern.BearishAdvanceBlock;
            /*
            var preparedData = dataGenerationService.PreparePatternTrainingData(candleDataXRP, pattern);
            modelTrainer.TrainPatternModel(preparedData, pattern);*/

            /*var rsi = indicatorService.CalculateRSI(candleDataXRP);
            Console.WriteLine(indicatorService.AnalyzeMarket(candleDataXRP));
            var prediction = predictionService.GenerateTradeSignal(candleDataXRP);

            Console.WriteLine($"{prediction.Type.ToString()} - {prediction.Confidence}%");*/
            var inputData = (await binanceService.GetHistoricalData("BTCUSDC", "1d", 200)).Select(c => c.Close).ToArray();
            var output = onnxPredictionService.Predict(inputData);

            Console.WriteLine($"Результат предсказания: {string.Join(", ", output)}");
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure<OnnxSettings>(configuration.GetSection("OnnxSettings"));
            services.AddSingleton<IOnnxPredictionService, OnnxPredictionService>();
            services.AddScoped<IModelTrainer, ModelTrainer>();
            services.AddScoped<IBinanceService, BinanceService>();
            services.AddHttpClient<IBinanceService, BinanceService>();
            services.AddScoped<ICandlestickPatternAnalyzer, CandlestickPatternAnalyzer>();
            services.AddScoped<IGenerationTrainingDataService, GenerationTrainingDataService>();
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<IPredictionService, PredictionService>();
            services.AddScoped<IIndicatorService, IndicatorService>();
        }
    }
}
