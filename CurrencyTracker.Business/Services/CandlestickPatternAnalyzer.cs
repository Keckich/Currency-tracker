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

        public PatternPrediction PredictHammerPattern(Candlestick candle)
        {
            var context = new MLContext();
            var model = context.Model.Load("hammerPatternModel.zip", out _);
            var predictionEngine = context.Model.CreatePredictionEngine<Candlestick, PatternPrediction>(model);

            return predictionEngine.Predict(candle);
        }

        public PatternPrediction PredictThreeWhiteSoldiersPattern(IEnumerable<Candlestick> candles)
        {
            var context = new MLContext();

            var model = context.Model.Load("threeWhiteSoldiersModel.zip", out _);
            var lastCandles = candles.TakeLast(3).ToList();
            if (lastCandles.Count < 3)
            {
                throw new ArgumentException("Not enougth data for pattern analyzing");
            }

            var input = new ThreeWhiteSoldiersInput
            {
                Body1 = lastCandles[0].Body,
                Body2 = lastCandles[1].Body,
                Body3 = lastCandles[2].Body,
            };

            var predictionEngine = context.Model.CreatePredictionEngine<ThreeWhiteSoldiersInput, PatternPrediction>(model);
            return predictionEngine.Predict(input);
        }

        public void AnalyzePatterns(IEnumerable<Candlestick> candlesticks)
        {
            var ohlcList = candlesticks
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

            int bullishCount = signals.GetFiboBullishSignalsCount(ohlcList);
            Console.WriteLine($"Bullish signals count: {bullishCount}");

            int bearishCount = signals.GetFiboBearishSignalsCount(ohlcList);
            Console.WriteLine($"Bearish signals count: {bearishCount}");

            LogAccuracyResult(ohlcList, "Bearish 3 BlackCrows");
            LogAccuracyResult(ohlcList, "Bearish 3 Inside Down");
            LogAccuracyResult(ohlcList, "Bearish 3 Outside Down");
            LogAccuracyResult(ohlcList, "Bearish 3 Line Strike");
            LogAccuracyResult(ohlcList, "Bearish Advance Block");
            LogAccuracyResult(ohlcList, "Bearish Belt Hold");
            LogAccuracyResult(ohlcList, "Bearish Black Closing Marubozu");
            LogAccuracyResult(ohlcList, "Bearish Black Marubozu");
            LogAccuracyResult(ohlcList, "Bearish Black Opening Marubozu");
            LogAccuracyResult(ohlcList, "Bearish Breakaway");
            LogAccuracyResult(ohlcList, "Bearish Deliberation");
            LogAccuracyResult(ohlcList, "Bearish Dark Cloud Cover");
            LogAccuracyResult(ohlcList, "Bearish Doji Star");
            LogAccuracyResult(ohlcList, "Bearish Downside Gap 3 Methods");
            LogAccuracyResult(ohlcList, "Bearish Dragonfly Doji");
            LogAccuracyResult(ohlcList, "Bearish Engulfing");
            LogAccuracyResult(ohlcList, "Bearish Evening Doji Star");
            LogAccuracyResult(ohlcList, "Bearish Evening Star");
            LogAccuracyResult(ohlcList, "Bearish Falling 3 Methods");
            LogAccuracyResult(ohlcList, "Bullish Inverted Hammer");
            LogAccuracyResult(ohlcList, "Bearish Thrusting");

            var accuracyAverPositive = accuracy.GetPositiveAccuracyToAverPatterns(ohlcList);
            Console.WriteLine("Patterns with positive accuracy rate comaring to aver. close price: {0}", string.Join(",", accuracyAverPositive));

            var accuracyEndPositive = accuracy.GetPositiveAccuracyToEndPatterns(ohlcList);
            Console.WriteLine("Patterns with positive accuracy rate comaring to end close price: {0}", string.Join(",", accuracyEndPositive));

            var accuracyBest = accuracy.GetBestAccuracyPatterns(ohlcList, 25);
            Console.WriteLine("25% of best patterns comparing to end and aver. close price: {0}", string.Join(",", accuracyBest));

            var accuracyBest30CandlesAhead = accuracy.GetBestAccuracyPatterns(ohlcList, 25, 30);
            Console.WriteLine("25% of best patterns 30 candles ahead comparing to end and aver. close price: {0}\n", string.Join(",", accuracyBest30CandlesAhead));
        }

        private void LogAccuracyResult(List<OhlcvObject> ohlcList, string pattern)
        {
            var accuracyPercentageSummary = accuracy.GetAverPercentAccuracy(ohlcList, pattern);
            Console.WriteLine($"{pattern}:");
            Console.WriteLine($"Accuracy percentage summary comparing to end of data set result: {0}", accuracyPercentageSummary.AccuracyToEndClose);
            Console.WriteLine($"Accuracy percentage summary comparing to average close result: {0}", accuracyPercentageSummary.AccuracyToAverageClose);

            var accuracyForSelectedFibo30CandlesAhead = accuracy.GetAverPercentAccuracy(ohlcList, pattern, 30);
            Console.WriteLine($"Accuracy percentage summary 30 candles ahead comparing to end of data set result: {0}", accuracyForSelectedFibo30CandlesAhead.AccuracyToEndClose);
            Console.WriteLine($"Accuracy percentage summary 30 candles ahead comparing to average close result: {0}\n", accuracyForSelectedFibo30CandlesAhead.AccuracyToAverageClose);
        }
    }
}
