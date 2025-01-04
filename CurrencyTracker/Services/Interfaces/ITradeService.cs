using CurrencyTracker.Models;

namespace CurrencyTracker.Services.Interfaces
{
    public interface ITradeService
    {
        IQueryable<Trade> GetTrades();

        Task<IEnumerable<Trade>> GetPaginatedTradesAsync(CancellationToken cancellationToken, int page = 0, int pageSize = 10);

        Task AddTradeAsync(Trade trade);
    }
}
