using CurrencyTracker.Business.Models;

namespace CurrencyTracker.Business.Services.Interfaces
{
    public interface IIndicatorService
    {
        void AddCandleAndUpdateRSI(Candlestick candlestick);

        void UpdateRSI(IEnumerable<Candlestick> candles);

        decimal GetRSI();
    }
}
