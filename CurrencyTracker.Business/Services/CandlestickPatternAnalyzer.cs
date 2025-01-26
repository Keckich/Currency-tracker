using CurrencyTracker.Business.Models;
using CurrencyTracker.Business.Services.Interfaces;
using Microsoft.ML;

namespace CurrencyTracker.Business.Services
{
    public class CandlestickPatternAnalyzer : ICandlestickPatternAnalyzer
    {
        private readonly IBinanceService binanceService;

        public CandlestickPatternAnalyzer(IBinanceService binanceService)
        {
            this.binanceService = binanceService;
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
    }
}
