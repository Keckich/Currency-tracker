using CurrencyTracker.Models;

namespace CurrencyTracker.Services.Interfaces
{
    public interface ITradeService
    {
        IQueryable<Trade> GetTrades();

        Task<IEnumerable<Trade>> GetPaginatedTradesAsync(CancellationToken cancellationToken, PaginationInfo paginationInfo);

        Task AddTradeAsync(Trade trade);

        Task<IEnumerable<PnLData>> CalculatePnLAsync();
    }
}
