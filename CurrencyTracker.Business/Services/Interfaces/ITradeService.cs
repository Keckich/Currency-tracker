using CurrencyTracker.Business.Models;

namespace CurrencyTracker.Business.Services.Interfaces
{
    public interface ITradeService
    {
        IQueryable<Trade> GetTrades();

        Task<IEnumerable<Trade>> GetPaginatedTradesAsync(CancellationToken cancellationToken, PaginationInfo paginationInfo);

        Task AddTradeAsync(Trade trade);

        Task<IEnumerable<PnLData>> CalculatePnLAsync(CancellationToken cancellationToken, int days = 7);
    }
}
