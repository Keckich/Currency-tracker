using CurrencyTracker.Business.Models;
using CurrencyTracker.Business.Services.Interfaces;
using Microsoft.ML;

namespace CurrencyTracker.Business.Services
{
    public class ModelTrainer : IModelTrainer
    {
        private readonly ICandlestickPatternAnalyzer сandlestickPatternAnalyzer;

        public ModelTrainer(ICandlestickPatternAnalyzer сandlestickPatternAnalyzer)
        {
            this.сandlestickPatternAnalyzer = сandlestickPatternAnalyzer;
        }

        public void TrainHammerModel(IEnumerable<Candlestick> historicalData)
        {
            var context = new MLContext();

            var trainingData = historicalData.Select(c => new
            {
                Body = (float)c.Body,
                LowerShadow = (float)c.LowerShadow,
                UpperShadow = (float)c.UpperShadow,
                Label = сandlestickPatternAnalyzer.IsHammer(c),
            }).ToList();

            var data = context.Data.LoadFromEnumerable(trainingData);

            var pipeline = context.Transforms.Concatenate("Features", nameof(Candlestick.Body), nameof(Candlestick.LowerShadow), nameof(Candlestick.UpperShadow))
                .Append(context.BinaryClassification.Trainers.SdcaLogisticRegression());

            var model = pipeline.Fit(data);
            context.Model.Save(model, data.Schema, "hammerPatternModel.zip");
        }

        public void TrainThreeWhiteSoldiersModel(IEnumerable<Candlestick> allCandles)
        {
            var context = new MLContext();

            var trainingData = allCandles
                .Skip(2)
                .Select((_, index) =>
                {
                    var segment = allCandles.Skip(index).Take(3).ToList();
                    return segment.Count == 3
                        ? new
                        {
                            Body1 = Math.Abs(segment[0].Close - segment[0].Open),
                            Body2 = Math.Abs(segment[1].Close - segment[1].Open),
                            Body3 = Math.Abs(segment[2].Close - segment[2].Open),
                            Label = сandlestickPatternAnalyzer.IsThreeWhiteSoldiers(segment)
                        }
                        : null;
                })
                .Where(data => data != null)
                .ToList();

            var data = context.Data.LoadFromEnumerable(trainingData!);
            var pipeline = context.Transforms.Concatenate("Features", nameof(ThreeWhiteSoldiersInput.Body1), nameof(ThreeWhiteSoldiersInput.Body2), nameof(ThreeWhiteSoldiersInput.Body3))
                .Append(context.BinaryClassification.Trainers.SdcaLogisticRegression());
            var model = pipeline.Fit(data);

            context.Model.Save(model, data.Schema, "threeWhiteSoldiersModel.zip");
        }
    }
}
