namespace CurrencyTracker.Services.Interfaces
{
    public interface IBinanceService
    {
        Task<Dictionary<string, decimal>> GetPricesAsync(IEnumerable<string>? currencies = null);
    }
}
