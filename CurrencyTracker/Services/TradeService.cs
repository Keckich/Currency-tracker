using CurrencyTracker.Data;
using CurrencyTracker.Models;
using CurrencyTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTracker.Services
{
    public class TradeService : ITradeService
    {
        private readonly ApplicationDbContext context;

        public TradeService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IQueryable<Trade> GetTrades()
        {
            return context.Trades.OrderByDescending(t => t.Id);
        }

        public async Task<IEnumerable<Trade>> GetPaginatedTradesAsync(CancellationToken cancellationToken, PaginationInfo paginationInfo)
        {
            var trades = GetTrades();
            return paginationInfo.PageSize.HasValue
                ? await GetTrades()
                    .Skip(paginationInfo.Page * paginationInfo.PageSize.Value)
                    .Take(paginationInfo.PageSize.Value)
                    .ToListAsync(cancellationToken)
                : trades;
        }

        public async Task AddTradeAsync(Trade trade)
        {
            context.Trades.Add(trade);
            await context.SaveChangesAsync();
        }
    }
}
