using CurrencyTracker.Business.Models;

namespace CurrencyTracker.Business.Services.Interfaces
{
    public interface IBinanceService
    {
        Task<Dictionary<string, decimal>> GetPricesAsync(IEnumerable<string>? currencies = null);

        Task<Dictionary<string, Dictionary<DateTime, decimal>?>> GetDailyClosingPricesAsync(IEnumerable<string> currencies, int days, CancellationToken cancellationToken);

        Task<string> GetOrderBookData(string symbol, int limit);

        Task<IEnumerable<Candlestick>> GetHistoricalData(string symbol, string interval);

        Task<IEnumerable<Candlestick>> GetHistoricalData(string symbol, string interval, DateTime? start = null, DateTime? end = null, int limit = 5000);
    }
}
