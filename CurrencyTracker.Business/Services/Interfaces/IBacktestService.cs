using CurrencyTracker.Business.Models;
using CurrencyTracker.Business.Models.Backtest;

namespace CurrencyTracker.Business.Services.Interfaces
{
    public interface IBacktestService
    {
        BacktestResult Run(IList<Candlestick> candles, decimal initialBalance = 1000m);
    }
}
