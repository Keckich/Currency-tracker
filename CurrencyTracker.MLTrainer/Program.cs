using CurrencyTracker.Business.Data;
using CurrencyTracker.Business.Services;
using CurrencyTracker.Business.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CurrencyTracker.Business.Models;

namespace MLTrainer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var binanceService = serviceProvider.GetRequiredService<IBinanceService>();
            var modelTrainer = serviceProvider.GetRequiredService<IModelTrainer>();
            var patternAnalyzer = serviceProvider.GetRequiredService<ICandlestickPatternAnalyzer>();

            var candleData = await binanceService.GetHistoricalData("BTCUSDC", "1h");
            modelTrainer.TrainThreeWhiteSoldiersModel(candleData);
            var prediction = patternAnalyzer.PredictThreeWhiteSoldiersPattern(candleData);
            Console.WriteLine(prediction.Probability);
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

            services.AddScoped<IModelTrainer, ModelTrainer>();
            services.AddScoped<IBinanceService, BinanceService>();
            services.AddHttpClient<IBinanceService, BinanceService>();
            services.AddScoped<ICandlestickPatternAnalyzer, CandlestickPatternAnalyzer>();
        }
    }
}
