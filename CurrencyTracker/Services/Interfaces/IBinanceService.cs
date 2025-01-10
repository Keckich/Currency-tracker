namespace CurrencyTracker.Services.Interfaces
{
    public interface IBinanceService
    {
        Task<Dictionary<string, decimal>> GetPricesAsync(IEnumerable<string>? currencies = null);

        Task<Dictionary<string, Dictionary<DateTime, decimal>?>> GetDailyClosingPricesAsync(IEnumerable<string> currencies, DateTime startDate, DateTime endDate);
    }
}
