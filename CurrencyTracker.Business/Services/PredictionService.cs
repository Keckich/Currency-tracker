using CurrencyTracker.Business.Models;
using CurrencyTracker.Business.Services.Interfaces;
using Microsoft.ML;

namespace CurrencyTracker.Business.Services
{
    public class PredictionService : IPredictionService
    {
        public PatternPrediction PredictHammerPattern(Candlestick candle)
        {
            var context = new MLContext();
            var model = context.Model.Load("hammerPatternModel.zip", out _);
            var predictionEngine = context.Model.CreatePredictionEngine<Candlestick, PatternPrediction>(model);

            return predictionEngine.Predict(candle);
        }

        public PatternPrediction PredictThreeWhiteSoldiersPattern(IEnumerable<Candlestick> candles)
        {
            if (candles.Count() < 3) return new PatternPrediction { IsPattern = false, Probability = 0 };

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

        public PatternPrediction PredictBearishAdvanceBlock(IEnumerable<Candlestick> candles)
        {
            if (candles.Count() < 3) return new PatternPrediction { IsPattern = false, Probability = 0 };

            var context = new MLContext();

            var model = context.Model.Load("bearishAdvanceBlockModel.zip", out _);
            var lastCandles = candles.TakeLast(3).ToList();

            var input = new ThreeCandlePatternData
            {
                Open1 = lastCandles[^3].Open,
                High1 = lastCandles[^3].High,
                Low1 = lastCandles[^3].Low,
                Close1 = lastCandles[^3].Close,
                Volume1 = lastCandles[^3].Volume,
                Open2 = lastCandles[^2].Open,
                High2 = lastCandles[^2].High,
                Low2 = lastCandles[^2].Low,
                Close2 = lastCandles[^2].Close,
                Volume2 = lastCandles[^2].Volume,
                Open3 = lastCandles[^1].Open,
                High3 = lastCandles[^1].High,
                Low3 = lastCandles[^1].Low,
                Close3 = lastCandles[^1].Close,
                Volume3 = lastCandles[^1].Volume
            };

            var predictionEngine = context.Model.CreatePredictionEngine<ThreeCandlePatternData, PatternPrediction>(model);
            return predictionEngine.Predict(input);
        }
    }
}
