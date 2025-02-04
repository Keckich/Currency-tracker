using Candlestick_Patterns;
using CurrencyTracker.Business.Models;
using CurrencyTracker.Business.Services.Interfaces;
using Microsoft.ML;
using Newtonsoft.Json;
using OHLC_Candlestick_Patterns;

namespace CurrencyTracker.Business.Services
{
    public class CandlestickPatternAnalyzer : ICandlestickPatternAnalyzer
    {
        private readonly IBinanceService binanceService;

        private readonly Signals signals;

        private readonly IAccuracyTrials accuracy;

        public CandlestickPatternAnalyzer(IBinanceService binanceService)
        {
            this.binanceService = binanceService;
            signals = new Signals();
            accuracy = new AccuracyTrials();
        }

        public bool IsHammer(Candlestick candle)
        {
            var body = Math.Abs(candle.Close - candle.Open);
            var lowerShadow = Math.Min(candle.Open, candle.Close) - candle.Low;
            var upperShadow = candle.High - Math.Max(candle.Open, candle.Close);

            return body < lowerShadow && upperShadow < body * 0.1 && lowerShadow > 2 * body;
        }

        public bool IsHangingMan(Candlestick candle)
        {
            return IsHammer(candle) && candle.IsBear;
        }

        public bool IsBullishEngulfing(Candlestick previous, Candlestick current)
        {
            return previous.IsBear && current.IsBull && current.Open < previous.Close && current.Close > previous.Open;
        }

        public bool IsBearishEngulfing(Candlestick previous, Candlestick current)
        {
            return previous.IsBull && current.IsBear && current.Open > previous.Close && current.Close < previous.Open;
        }

        public bool IsDoji(Candlestick candle)
        {
            var body = Math.Abs(candle.Close - candle.Open);
            var range = candle.High - candle.Low;

            return body < range * 0.1;
        }

        public bool IsThreeWhiteSoldiers(IList<Candlestick> candles)
        {
            return candles[0].Close > candles[0].Open &&
                   candles[1].Close > candles[1].Open &&
                   candles[2].Close > candles[2].Open &&
                   candles[1].Open > candles[0].Close &&
                   candles[2].Open > candles[1].Close;
        }

        public bool IsBearishAdvanceBlock(IList<Candlestick> candles)
        {
            if (candles.Count < 3) return false;

            var c1 = candles[candles.Count - 3];
            var c2 = candles[candles.Count - 2];
            var c3 = candles[candles.Count - 1];

            bool isBullishSequence = c1.IsBull && c2.IsBull && c3.IsBull;
            bool isDecreasingBody = c1.Body > c2.Body && c2.Body > c3.Body;
            bool hasLongUpperShadows = c1.UpperShadow > c1.Body * 0.5f &&
                                       c2.UpperShadow > c2.Body * 0.5f &&
                                       c3.UpperShadow > c3.Body * 0.5f;

            return isBullishSequence && isDecreasingBody && hasLongUpperShadows;
        }

        public bool IsEveningStar(IList<Candlestick> candles)
        {
            if (candles.Count != 3) return false;

            var first = candles[0];
            var second = candles[1];
            var third = candles[2];

            return first.IsBull &&
                   second.Body < (first.Body * 0.5) &&
                   third.Close < (first.Open + first.Close) / 2 &&
                   third.IsBear;
        }

        public void AnalyzePatterns(IEnumerable<Candlestick> candlesticks)
        {
            var dataOhlcv = candlesticks
                .Select(c => new OhlcvObject
                {
                    Open = (decimal)c.Open,
                    High = (decimal)c.High,
                    Low = (decimal)c.Low,
                    Close = (decimal)c.Close,
                    Volume = (decimal)c.Volume,
                })
                .Where(x => x.Open != 0 && x.High != 0 && x.Low != 0 && x.Close != 0)
                .ToList();

            var accuracyAverPositive = accuracy.GetPositiveAccuracyToAverPatterns(dataOhlcv);
            Console.WriteLine("Patterns with positive accuracy rate comaring to aver. close price: {0}", string.Join(",", accuracyAverPositive));

            var accuracyEndPositive = accuracy.GetPositiveAccuracyToEndPatterns(dataOhlcv);
            Console.WriteLine("Patterns with positive accuracy rate comaring to end close price: {0}", string.Join(",", accuracyEndPositive));

            foreach (var pattern in accuracyEndPositive)
            {
                LogAccuracyResult(dataOhlcv, pattern);
            }

            var accuracyBest = accuracy.GetBestAccuracyPatterns(dataOhlcv, 25);
            Console.WriteLine("25% of best patterns comparing to end and aver. close price: {0}", string.Join(",", accuracyBest));

            var accuracyBest30CandlesAhead = accuracy.GetBestAccuracyPatterns(dataOhlcv, 25, 30);
            Console.WriteLine("25% of best patterns 30 candles ahead comparing to end and aver. close price: {0}", string.Join(",", accuracyBest30CandlesAhead));

            //SIGNALS
            var bullishCount = signals.GetPatternsBullishSignalsCount(dataOhlcv);
            Console.WriteLine("Bullish signals count: {0}", bullishCount);

            var bearishCount = signals.GetPatternsBearishSignalsCount(dataOhlcv);
            Console.WriteLine("Bearish signals count: {0}", bearishCount);

            var signalsCountMulti = signals.GetMultiplePatternsSignalsCount(dataOhlcv, new string[] { "Bearish Belt Hold", "Bearish Black Closing Marubozu" });
            Console.WriteLine("Multiple patterns signals count: {0}", signalsCountMulti);

            var signalsCountSingle = signals.GetPatternsSignalsCount(dataOhlcv, "Bearish Black Closing Marubozu");
            Console.WriteLine("Single pattern signals count: {0}", signalsCountSingle);

            var signalsCountMultiWeightened = signals.GetMultiplePatternsSignalsIndex(dataOhlcv, new Dictionary<string, decimal>() { { "Bearish Belt Hold", 0.5M }, { "Bearish Black Closing Marubozu", 0.5M } });
            Console.WriteLine("Weightened index for selected multiple patterns: {0}", signalsCountMultiWeightened);

            var signalsCountSingleWeightened = signals.GetPatternSignalsIndex(dataOhlcv, "Bearish Black Closing Marubozu", 0.5M);
            Console.WriteLine("Weightened index for selected single pattern: {0}", signalsCountSingleWeightened);

            var ohlcSingleSignals = signals.GetPatternsOhlcvWithSignals(dataOhlcv, "Bearish Black Closing Marubozu");
            Console.WriteLine("Signals for single pattern: {0}", ohlcSingleSignals.Where(x => x.Signal == true).Count());

            var ohlcMultiSignals = signals.GetMultiplePatternsOhlcvWithSignals(dataOhlcv, new string[] { "Bearish Belt Hold", "Bearish Black Closing Marubozu" });
            Console.WriteLine("Number of lists returned: {0}", ohlcMultiSignals.Count());
        }

        private void LogAccuracyResult(List<OhlcvObject> ohlcList, string pattern)
        {
            /*var accuracyPercentageSummary = accuracy.GetAverPercentPatternAccuracy(ohlcList, pattern);
            Console.WriteLine($"{pattern}:");
            Console.WriteLine($"Accuracy percentage summary comparing to end of data set result: {0}", accuracyPercentageSummary.AccuracyToEndClose);
            Console.WriteLine($"Accuracy percentage summary comparing to average close result: {0}", accuracyPercentageSummary.AccuracyToAverageClose);

            var accuracyForSelectedPattern30CandlesAhead = accuracy.GetAverPercentPatternAccuracy(ohlcList, pattern, 30);
            Console.WriteLine($"Accuracy percentage summary 30 candles ahead comparing to end of data set result: {0}", accuracyForSelectedPattern30CandlesAhead.AccuracyToEndClose);
            Console.WriteLine($"Accuracy percentage summary 30 candles ahead comparing to average close result: {0}\n", accuracyForSelectedPattern30CandlesAhead.AccuracyToAverageClose);*/

            var accuracyPercentageSummary = accuracy.GetAverPercentPatternAccuracy(ohlcList, pattern);
            Console.WriteLine($"{pattern}:");
            Console.WriteLine("Accuracy percentage summary comparing to end of data set result: {0}", accuracyPercentageSummary.AccuracyToEndClose);
            Console.WriteLine("Accuracy percentage summary comparing to average close result: {0}", accuracyPercentageSummary.AccuracyToAverageClose);

            var accuracyForSelectedPattern30CandlesAhead = accuracy.GetAverPercentPatternAccuracy(ohlcList, pattern, 30);
            Console.WriteLine("Accuracy percentage summary 30 candles ahead comparing to end of data set result: {0}", accuracyForSelectedPattern30CandlesAhead.AccuracyToEndClose);
            Console.WriteLine("Accuracy percentage summary 30 candles ahead comparing to average close result: {0}\n", accuracyForSelectedPattern30CandlesAhead.AccuracyToAverageClose);
        }
    }
}
