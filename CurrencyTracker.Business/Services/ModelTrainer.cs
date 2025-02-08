using CurrencyTracker.Business.Enums;
using CurrencyTracker.Business.Models;
using CurrencyTracker.Business.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Trainers.FastTree;
using Microsoft.ML.Trainers.LightGbm;
using System.Data;

namespace CurrencyTracker.Business.Services
{
    public class ModelTrainer : IModelTrainer
    {
        private readonly ICandlestickPatternAnalyzer candlestickPatternAnalyzer;

        private readonly IGenerationTrainingDataService generationTrainingDataService;

        private readonly ILogService logService;

        public ModelTrainer(
            ICandlestickPatternAnalyzer candlestickPatternAnalyzer,
            IGenerationTrainingDataService generationTrainingDataService,
            ILogService logService)
        {
            this.candlestickPatternAnalyzer = candlestickPatternAnalyzer;
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
                Label = candlestickPatternAnalyzer.IsHammer(c),
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
            var trainer = GetLightGbmBinaryTrainer(context);
            var trainingData = generationTrainingDataService.PrepareThreeWhiteSoldiersTrainingData(allCandles.ToList());

            var data = context.Data.LoadFromEnumerable(trainingData!);
            var pipeline = context.Transforms.Concatenate("Features", nameof(ThreeWhiteSoldiersInput.Body1), nameof(ThreeWhiteSoldiersInput.Body2), nameof(ThreeWhiteSoldiersInput.Body3))
                .Append(trainer);
            var model = pipeline.Fit(data);

            var partitions = context.Data.TrainTestSplit(data, testFraction: 0.2);
            var trainData = partitions.TrainSet;
            var testData = partitions.TestSet;
            var predictions = model.Transform(testData);
            var metrics = context.BinaryClassification.Evaluate(predictions, "Label", "Score");
            logService.LogMetrics(metrics);
            logService.CheckIsModelRetrained(context, trainData, pipeline);

            context.Model.Save(model, data.Schema, "threeWhiteSoldiersModel.zip");
        }

        public void TrainThreeCandlePatternModel(IEnumerable<ThreeCandlePatternData> preparedData, CandlestickPattern pattern)
        {
            var context = new MLContext();

            var trainer = GetFastTreeTrainer(context);
            var pipeline = context.Transforms.Concatenate("Features",
                    nameof(ThreeCandlePatternData.Open1), nameof(ThreeCandlePatternData.High1), nameof(ThreeCandlePatternData.Low1), nameof(ThreeCandlePatternData.Close1), nameof(ThreeCandlePatternData.Volume1),
                    nameof(ThreeCandlePatternData.Open2), nameof(ThreeCandlePatternData.High2), nameof(ThreeCandlePatternData.Low2), nameof(ThreeCandlePatternData.Close2), nameof(ThreeCandlePatternData.Volume2),
                    nameof(ThreeCandlePatternData.Open3), nameof(ThreeCandlePatternData.High3), nameof(ThreeCandlePatternData.Low3), nameof(ThreeCandlePatternData.Close3), nameof(ThreeCandlePatternData.Volume3))
                .Append(context.Transforms.NormalizeMinMax("Features"))
                .Append(trainer)
                .Append(context.BinaryClassification.Calibrators.Platt(
                    labelColumnName: "Label",
                    scoreColumnName: "Score"));

            var trainData = context.Data.LoadFromEnumerable(preparedData);

            var partitions = context.Data.TrainTestSplit(trainData, testFraction: 0.2);
            var trainingData = partitions.TrainSet;
            var testData = partitions.TestSet;

            var model = pipeline.Fit(trainingData);
            var predictions = model.Transform(testData);
            var probabilities = predictions.GetColumn<float>("Score").Select(score => 1.0f / (1.0f + (float)Math.Exp(-score))).ToList();

            var metrics = context.BinaryClassification.Evaluate(predictions, "Label", "Score");
            logService.LogMetrics(metrics);
            logService.CheckIsModelRetrained(context, trainingData, pipeline);
            
            context.Model.Save(model, trainData.Schema, $"{pattern.ToString()}Model.zip");
        }

        private FastTreeBinaryTrainer GetFastTreeTrainer(MLContext context)
        {
            var trainer = context.BinaryClassification.Trainers.FastTree(
                new FastTreeBinaryTrainer.Options
                {
                    NumberOfTrees = 500,
                    NumberOfLeaves = 50,  // 10, 20, 50
                    MinimumExampleCountPerLeaf = 15,
                    LearningRate = 0.6  // 0.05, 0.1, 0.2
                });

            return trainer;
        }

        private SdcaLogisticRegressionBinaryTrainer GetSdcaLogisticRegressionBinaryTrainer(MLContext context)
        {
            var trainer = context.BinaryClassification.Trainers.SdcaLogisticRegression(
                labelColumnName: "Label",
                featureColumnName: "Features");

            return trainer;
        }

        private LbfgsLogisticRegressionBinaryTrainer GetLbfgsLogisticRegressionBinaryTrainer(MLContext context)
        {
            var trainer = context.BinaryClassification.Trainers.LbfgsLogisticRegression(
                labelColumnName: "Label",
                featureColumnName: "Features");

            return trainer;
        }

        private LightGbmBinaryTrainer GetLightGbmBinaryTrainer(MLContext context)
        {
            var lightGbmOptions = new LightGbmBinaryTrainer.Options
            {
                NumberOfLeaves = 50,
                NumberOfIterations = 500,
                LearningRate = 0.2f,
                MinimumExampleCountPerLeaf = 15,
                UseCategoricalSplit = false,
                EarlyStoppingRound = 10,
            };

            var trainer = context.BinaryClassification.Trainers.LightGbm(lightGbmOptions);

            return trainer;
        }
    }
}
