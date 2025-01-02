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

        public async Task<IEnumerable<Trade>> GetTradesAsync()
        {
            return await context.Trades.ToListAsync();
        }

        public async Task AddTradeAsync(Trade trade)
        {
            context.Trades.Add(trade);
            await context.SaveChangesAsync();
        }
    }
}
