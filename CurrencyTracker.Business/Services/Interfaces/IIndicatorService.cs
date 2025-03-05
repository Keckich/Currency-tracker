using CurrencyTracker.Business.Models;

namespace CurrencyTracker.Business.Services.Interfaces
{
    public interface IIndicatorService
    {
        decimal CalculateRSI(IList<Candlestick> candles, int period = 14);

        (IList<float> macd, IList<float> signal) CalculateMACD(IList<Candlestick> candles, int shortPeriod = 12, int longPeriod = 26, int signalPeriod = 9);

        IList<float> CalculateEMA(IList<float> prices, int period);

        (IEnumerable<double> upperBand, IEnumerable<double> lowerBand) CalculateBollingerBands(IEnumerable<Candlestick> candles, int period = 20, double stdDevMultiplier = 2.0);

        IEnumerable<float> CalculateATR(IList<Candlestick> candles, int period = 14);

        string AnalyzeMarket(IList<Candlestick> candles);
        /*void AddCandleAndUpdateRSI(Candlestick candlestick);

        void UpdateRSI(IEnumerable<Candlestick> candles);

        decimal GetRSI();*/
    }
}
