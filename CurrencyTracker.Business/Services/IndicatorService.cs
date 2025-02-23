using CurrencyTracker.Business.Models;
using CurrencyTracker.Business.Services.Interfaces;
using StockSharp.Algo.Indicators;
using StockSharp.Messages;

namespace CurrencyTracker.Business.Services
{
    public class IndicatorService : IIndicatorService
    {
        private const int MaxCandles = 14;

        private readonly RelativeStrengthIndex rsi;

        private readonly Queue<TimeFrameCandleMessage> candleBuffer;

        public IndicatorService()
        {
            rsi = new RelativeStrengthIndex { Length = MaxCandles };
            candleBuffer = new Queue<TimeFrameCandleMessage>(MaxCandles);
        }

        public decimal CalculateRSI(IList<Candlestick> candles, int period = MaxCandles)
        {
            if (candles.Count < period + 1) return -1;

            decimal gainSum = 0, lossSum = 0;

            for (int i = 1; i <= period; i++)
            {
                decimal change = (decimal)(candles[i].Close - candles[i - 1].Close);
                if (change > 0) gainSum += change;
                else lossSum += Math.Abs(change);
            }

            decimal avgGain = gainSum / period;
            decimal avgLoss = lossSum / period;

            for (int i = period + 1; i < candles.Count; i++)
            {
                decimal change = (decimal)(candles[i].Close - candles[i - 1].Close);
                decimal gain = change > 0 ? change : 0;
                decimal loss = change < 0 ? Math.Abs(change) : 0;

                avgGain = (avgGain * (period - 1) + gain) / period;
                avgLoss = (avgLoss * (period - 1) + loss) / period;
            }

            if (avgLoss == 0) return 100;

            decimal rs = avgGain / avgLoss;
            return 100 - (100 / (1 + rs));
        }

        public (IEnumerable<double> macd, IEnumerable<double> signal) CalculateMACD(IList<double> prices, int shortPeriod = 12, int longPeriod = 26, int signalPeriod = 9)
        {
            var shortEMA = CalculateEMA(prices, shortPeriod);
            var longEMA = CalculateEMA(prices, longPeriod);
            var macd = shortEMA.Zip(longEMA, (s, l) => s - l).ToList();
            var signal = CalculateEMA(macd, signalPeriod);

            return (macd, signal);
        }

        public IEnumerable<double> CalculateEMA(IList<double> prices, int period)
        {
            var emaValues = new List<double>();
            double multiplier = 2.0 / (period + 1);
            double ema = prices.Take(period).Average();
            emaValues.Add(ema);

            for (int i = period; i < prices.Count; i++)
            {
                ema = (prices[i] - ema) * multiplier + ema;
                emaValues.Add(ema);
            }

            return emaValues;
        }

        public (IEnumerable<double> upperBand, IEnumerable<double> lowerBand) CalculateBollingerBands(IEnumerable<double> prices, int period = 20, double stdDevMultiplier = 2.0)
        {
            var upperBand = new List<double>();
            var lowerBand = new List<double>();

            for (int i = period - 1; i < prices.Count(); i++)
            {
                var window = prices.Skip(i - period + 1).Take(period).ToList();
                double mean = window.Average();
                double stdDev = Math.Sqrt(window.Sum(p => Math.Pow(p - mean, 2)) / period);

                upperBand.Add(mean + stdDevMultiplier * stdDev);
                lowerBand.Add(mean - stdDevMultiplier * stdDev);
            }

            return (upperBand, lowerBand);
        }

        // For some reason StockSharp is not forming RSI value
        /*public void AddCandleAndUpdateRSI(Candlestick candlestick)
        {
            if (candleBuffer.Count == MaxCandles)
            {
                candleBuffer.Dequeue();
            }

            var candleMessage = ConvertToTimeFrameCandleMessage(candlestick);
            candleBuffer.Enqueue(candleMessage);
            UpdateRSI(candleMessage);
        }

        private void UpdateRSI(TimeFrameCandleMessage candleMessage)
        {
            if (candleBuffer.Count < MaxCandles)
            {
                Console.WriteLine("Not enough data for RSI calculating.");
                return;
            }

            rsi.Process(candleMessage);
        }

        public void UpdateRSI(IEnumerable<Candlestick> candles)
        {
            if (candles.Count() < rsi.Length)
            {
                Console.WriteLine("Not enough data for RSI calculating.");
                return;
            }

            var orderedCandles = candles.OrderBy(c => c.OpenTime).ToList();

            foreach (var candle in orderedCandles)
            {
                var candleMessage = ConvertToTimeFrameCandleMessage(candle);
                rsi.Process(candleMessage);
            }

            if (!rsi.IsFormed)
            {
                Console.WriteLine(" RSI is not formed.");
                return;
            }

            Console.WriteLine($"RSI is formed. Current value: {rsi.GetCurrentValue()}");
        }

        public decimal GetRSI()
        {
            return rsi.IsFormed ? rsi.GetCurrentValue() : 0;
        }

        private TimeFrameCandleMessage ConvertToTimeFrameCandleMessage(Candlestick candlestick)
        {
            return new TimeFrameCandleMessage
            {
                OpenTime = candlestick.OpenTime,
                CloseTime = candlestick.OpenTime.AddHours(4), // TODO: pass interval as param
                OpenPrice = (decimal)candlestick.Open,
                HighPrice = (decimal)candlestick.High,
                LowPrice = (decimal)candlestick.Low,
                ClosePrice = (decimal)candlestick.Close,
                TotalVolume = (decimal)candlestick.Volume,
                SecurityId = new SecurityId { SecurityCode = "XRPUSDC" },
            };
        }*/
    }
}
