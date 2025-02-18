using CurrencyTracker.Business.Models;

namespace CurrencyTracker.Business.Services.Interfaces
{
    public interface IIndicatorService
    {
        decimal CalculateRSI(IList<Candlestick> candles, int period = 14);

        /*void AddCandleAndUpdateRSI(Candlestick candlestick);

        void UpdateRSI(IEnumerable<Candlestick> candles);

        decimal GetRSI();*/
    }
}
