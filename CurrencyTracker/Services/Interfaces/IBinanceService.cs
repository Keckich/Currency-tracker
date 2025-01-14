namespace CurrencyTracker.Services.Interfaces
{
    public interface IBinanceService
    {
        Task<Dictionary<string, decimal>> GetPricesAsync(IEnumerable<string>? currencies = null);

        Task<Dictionary<string, Dictionary<DateTime, decimal>?>> GetDailyClosingPricesAsync(IEnumerable<string> currencies, int days, CancellationToken cancellationToken);

        Task<string> GetOrderBookData(string symbol, int limit);
    }
}
