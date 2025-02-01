using CurrencyTracker.Business.Models;
using CurrencyTracker.Business.Services.Interfaces;
using Microsoft.ML;
using Microsoft.ML.Trainers.FastTree;
using System.Data;

namespace CurrencyTracker.Business.Services
{
    public class ModelTrainer : IModelTrainer
    {
        private readonly ICandlestickPatternAnalyzer сandlestickPatternAnalyzer;

        private readonly IGenerationTrainingDataService generationTrainingDataService;

        private readonly ILogService logService;

        public ModelTrainer(
            ICandlestickPatternAnalyzer сandlestickPatternAnalyzer,
            IGenerationTrainingDataService generationTrainingDataService,
            ILogService logService)
        {
            this.сandlestickPatternAnalyzer = сandlestickPatternAnalyzer;
            this.generationTrainingDataService = generationTrainingDataService;
            this.logService = logService;
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

        public void TrainBearishAdvanceBlockModel(IEnumerable<Candlestick> allCandles)
        {
            var context = new MLContext();

            var trainer = context.BinaryClassification.Trainers.FastTree(
                new FastTreeBinaryTrainer.Options
                {
                    NumberOfTrees = 50,
                    NumberOfLeaves = 10,  // 10, 20, 50
                    MinimumExampleCountPerLeaf = 10,
                    LearningRate = 0.05  // 0.05, 0.1, 0.2
                });
            var pipeline = context.Transforms.Concatenate("Features",
                    nameof(BearishAdvanceBlockData.Open1), nameof(BearishAdvanceBlockData.High1), nameof(BearishAdvanceBlockData.Low1), nameof(BearishAdvanceBlockData.Close1), nameof(BearishAdvanceBlockData.Volume1),
                    nameof(BearishAdvanceBlockData.Open2), nameof(BearishAdvanceBlockData.High2), nameof(BearishAdvanceBlockData.Low2), nameof(BearishAdvanceBlockData.Close2), nameof(BearishAdvanceBlockData.Volume2),
                    nameof(BearishAdvanceBlockData.Open3), nameof(BearishAdvanceBlockData.High3), nameof(BearishAdvanceBlockData.Low3), nameof(BearishAdvanceBlockData.Close3), nameof(BearishAdvanceBlockData.Volume3))
                .Append(context.Transforms.NormalizeMinMax("Features"))
                .Append(trainer);

            var data = generationTrainingDataService.PrepareBearishAdvanceBlockTrainingData(allCandles.ToList());

            var trainData = context.Data.LoadFromEnumerable(data);

            var partitions = context.Data.TrainTestSplit(trainData, testFraction: 0.2);
            var trainingData = partitions.TrainSet;
            var testData = partitions.TestSet;

            var model = pipeline.Fit(trainingData);
            var predictions = model.Transform(testData);
            
            var metrics = context.BinaryClassification.Evaluate(predictions, "Label", "Score");
            logService.LogMetrics(metrics);
            logService.CheckIsModelRetrained(context, trainingData, pipeline);
            
            context.Model.Save(model, trainData.Schema, "bearishAdvanceBlockModel.zip");
        }
    }
}
