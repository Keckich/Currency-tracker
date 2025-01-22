using CurrencyTracker.Models;
using CurrencyTracker.Services.Interfaces;

namespace CurrencyTracker.Services
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

            return body < lowerShadow && upperShadow < body * 0.1m && lowerShadow > 2 * body;
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

            return body < range * 0.1m;
        }

        //TODO: correct these methods later to analyze patterns more accurately
        /*public void TrainHammerModel(IEnumerable<Candlestick> historicalData)
        {
            var context = new MLContext();

            var trainingData = historicalData.Select(c => new
            {
                Body = (float)c.Body,
                LowerShadow = (float)c.LowerShadow,
                UpperShadow = (float)c.UpperShadow,
                Label = c.IsHammer
            }).ToList();

            var data = context.Data.LoadFromEnumerable(trainingData);

            var pipeline = context.Transforms.Concatenate("Features", nameof(Candlestick.Body), nameof(Candlestick.LowerShadow), nameof(Candlestick.UpperShadow))
                .Append(context.BinaryClassification.Trainers.SdcaLogisticRegression());

            var model = pipeline.Fit(data);
            context.Model.Save(model, data.Schema, "hammerPatternModel.zip");
        }

        public HammerPrediction PredictHammerPattern(Candlestick candle)
        {
            var context = new MLContext();
            var model = context.Model.Load("hammerPatternModel.zip", out _);
            var predictionEngine = context.Model.CreatePredictionEngine<Candlestick, HammerPrediction>(model);

            return predictionEngine.Predict(candle);
        }*/
    }
}
