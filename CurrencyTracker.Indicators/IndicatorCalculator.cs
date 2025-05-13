using CurrencyTracker.Business.Models;
using Skender.Stock.Indicators;

namespace CurrencyTracker.Indicators
{
    public class IndicatorCalculator
    {
        public static IEnumerable<Quote> ConvertToQuotes(IEnumerable<Candlestick> candles)
        {
            return candles.Select(c => new Quote
            {
                Date = c.CloseTime,
                Open = (decimal)c.Open,
                High = (decimal)c.High,
                Low = (decimal)c.Low,
                Close = (decimal)c.Close,
                Volume = (decimal)c.Volume
            }).ToList();
        }

        public static IEnumerable<RsiResult> CalculateRsi(IEnumerable<Candlestick> candles, int period = 14)
        {
            var quotes = ConvertToQuotes(candles);
            return quotes.GetRsi(period);
        }

        public static IEnumerable<EmaResult> CalculateEma(IEnumerable<Candlestick> candles, int period = 20)
        {
            var quotes = ConvertToQuotes(candles);
            return quotes.GetEma(period);
        }

        public static IEnumerable<MacdResult> CalculateMacd(IEnumerable<Candlestick> candles)
        {
            var quotes = ConvertToQuotes(candles);
            return quotes.GetMacd(); // default params: 12, 26, 9
        }

        public static IEnumerable<BollingerBandsResult> CalculateBollingerBands(IEnumerable<Candlestick> candles, int period = 20, double stdDev = 2)
        {
            var quotes = ConvertToQuotes(candles);
            return quotes.GetBollingerBands(period, stdDev);
        }

        public static IEnumerable<RocResult> CalculateMomentumAsRoc(IEnumerable<Candlestick> candles, int period = 10)
        {
            var quotes = ConvertToQuotes(candles);
            return quotes.GetRoc(period);
        }
    }
}
