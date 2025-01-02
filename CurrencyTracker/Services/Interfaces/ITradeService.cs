using CurrencyTracker.Models;

namespace CurrencyTracker.Services.Interfaces
{
    public interface ITradeService
    {
        Task<IEnumerable<Trade>> GetTradesAsync();

        Task AddTradeAsync(Trade trade);
    }
}
