using CurrencyTracker.Data;
using CurrencyTracker.Models;
using CurrencyTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTracker.Services
{
    public class TradeService : ITradeService
    {
        private readonly ApplicationDbContext context;

        private readonly IBinanceService binanceService;

        public TradeService(ApplicationDbContext context, IBinanceService binanceService)
        {
            this.context = context;
            this.binanceService = binanceService;
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

        public async Task<IEnumerable<PnLData>> GetPnLDataAsync()
        {
            var trades = await context.Trades.ToListAsync();
            var prices = await binanceService.GetPricesAsync();

            var groupedTrades = trades
                .GroupBy(t => new { Date = t.Date.Date, t.Currency })
                .Select(group =>
                {
                    var totalAmount = (decimal)group.Sum(t => t.Amount);
                    var totalSpent = group.Sum(t => t.Value);
                    var currency = group.Key.Currency;

                    var currentPrice = prices.TryGetValue(currency, out var price) ? price : 0;
                    var avgPrice = totalSpent / totalAmount;
                    var pnl = (currentPrice - avgPrice) * totalAmount;

                    return new PnLData
                    {
                        Date = group.Key.Date,
                        Currency = currency,
                        PnL = pnl
                    };
                });

            return groupedTrades;
        }
    }
}
