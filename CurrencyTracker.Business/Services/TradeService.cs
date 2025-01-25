using CurrencyTracker.Business.Data;
using CurrencyTracker.Business.Models;
using CurrencyTracker.Business.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTracker.Business.Services
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

        public async Task<IEnumerable<PnLData>> CalculatePnLAsync(CancellationToken cancellationToken, int days = 7)
        {
            var dates = GetPnLDates(days);
            var trades = await context.Trades.ToListAsync(cancellationToken);
            var currencies = trades.Select(t => t.Currency).Distinct();
            var prices = await binanceService.GetDailyClosingPricesAsync(currencies, days, cancellationToken);

            var pnlEntries = new List<PnLData>();
            var balance = new Dictionary<string, decimal>();
            decimal initialBalance = 0;

            foreach (var date in dates.Select((value, i) => new { Index = i, Value = value }))
            {
                foreach (var trade in trades.Where(t => t.Date.Date <= date.Value))
                {
                    if (!balance.ContainsKey(trade.Currency))
                        balance[trade.Currency] = 0;

                    balance[trade.Currency] += (decimal)trade.Amount;
                }

                decimal totalBalance = balance.Sum(b => b.Value * prices[b.Key][days == 1 ? date.Value : date.Value.Date]);
                if (date.Index == 0 || (initialBalance == 0 && totalBalance != 0))
                {
                    initialBalance = totalBalance;
                }

                var pnl = initialBalance != 0 ? totalBalance / initialBalance * 100 - 100 : 0;
                pnlEntries.Add(new PnLData { Date = date.Value, Balance = totalBalance, PnL = pnl });
                balance = new Dictionary<string, decimal>();
            }

            return pnlEntries;
        }

        private IEnumerable<DateTime> GetPnLDates(int days = 7)
        {
            var currentDate = DateTime.UtcNow;
            var dates = Enumerable.Empty<DateTime>();
            if (days == 1)
            {
                dates = Enumerable.Range(0, 24)
                           .Select(h => currentDate.AddHours(-h))
                           .Select(d => new DateTime(d.Year, d.Month, d.Day, d.Hour, 0, 0));
            }
            else
            {
                dates = Enumerable.Range(0, days)
                           .Select(d => currentDate.AddDays(-d));
            }

            return dates.OrderBy(d => d).ToList();
        }
    }
}
