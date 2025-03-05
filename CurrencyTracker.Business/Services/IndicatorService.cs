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

        public (IList<float> macd, IList<float> signal) CalculateMACD(IList<Candlestick> candles, int shortPeriod = 12, int longPeriod = 26, int signalPeriod = 9)
        {
            var prices = candles.Select(c => c.Close).ToList();
            var shortEMA = CalculateEMA(prices, shortPeriod);
            var longEMA = CalculateEMA(prices, longPeriod);
            var macd = shortEMA.Zip(longEMA, (s, l) => s - l).ToList();
            var signal = CalculateEMA(macd, signalPeriod);

            return (macd, signal);
        }

        public IList<float> CalculateEMA(IList<float> prices, int period)
        {
            var emaValues = new List<float>();
            float multiplier = 2.0f / (period + 1);
            float ema = prices.Take(period).Average();
            emaValues.Add(ema);

            for (int i = period; i < prices.Count; i++)
            {
                ema = (prices[i] - ema) * multiplier + ema;
                emaValues.Add(ema);
            }

            return emaValues;
        }

        public (IEnumerable<double> upperBand, IEnumerable<double> lowerBand) CalculateBollingerBands(IEnumerable<Candlestick> candles, int period = 20, double stdDevMultiplier = 2.0)
        {
            var prices = candles.Select(c => c.Close).ToList();
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

        public IEnumerable<float> CalculateATR(IList<Candlestick> candles, int period = 14)
        {
            var atrValues = new List<float>();

            for (int i = 1; i < candles.Count; i++)
            {
                float highLow = candles[i].High - candles[i].Low;
                float highClose = Math.Abs(candles[i].High - candles[i - 1].Close);
                float lowClose = Math.Abs(candles[i].Low - candles[i - 1].Close);

                float trueRange = Math.Max(highLow, Math.Max(highClose, lowClose));
                atrValues.Add(trueRange);
            }

            var smoothedATR = CalculateEMA(atrValues, period);
            return smoothedATR;
        }

        public string AnalyzeMarket(IList<Candlestick> candles)
        {
            var prices = candles.Select(c => c.Close).ToList();

            var rsi = CalculateRSI(candles);
            var (macd, signal) = CalculateMACD(candles);
            var atr = CalculateATR(candles);
            var (upperBand, lowerBand) = CalculateBollingerBands(candles);

            bool isUptrend = macd.Last() > signal.Last();
            bool isDowntrend = macd.Last() < signal.Last();

            double latestATR = atr.Last();
            double bollingerWidth = upperBand.Last() - lowerBand.Last();
            bool highVolatility = bollingerWidth > prices.Last() * 0.02;

            bool highVolume = candles.Last().Volume > candles.SkipLast(1).Average(c => c.Volume) * 1.5;

            return $"Trend: {(isUptrend ? "Uptrend" : isDowntrend ? "Downtrend" : "Flat")}, " +
                   $"Volatility: {(highVolatility ? "High" : "Low")}, " +
                   $"Volume: {(highVolume ? "High" : "Normal")}.";
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
