using CurrencyTracker.Business.Enums;
using CurrencyTracker.Business.Helpers;
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

        private readonly IBinanceService binanceService;

        public ModelTrainer(
            ICandlestickPatternAnalyzer candlestickPatternAnalyzer,
            IGenerationTrainingDataService generationTrainingDataService,
            ILogService logService)
        {
            this.candlestickPatternAnalyzer = candlestickPatternAnalyzer;
            this.generationTrainingDataService = generationTrainingDataService;
            this.logService = logService;
        }

        public void TrainPatternModel(IEnumerable<CandlePatternData> preparedData, CandlestickPattern pattern)
        {
            var context = new MLContext();
            var patternSize = PatternHelper.GetPatternCheckers()[pattern].PatternSize;
            var trainer = GetFastTreeTrainer(context);

            var schemaDefinition = SchemaDefinition.Create(typeof(CandlePatternData));
            schemaDefinition[nameof(CandlePatternData.Opens)].ColumnType = new VectorDataViewType(NumberDataViewType.Single, patternSize);
            schemaDefinition[nameof(CandlePatternData.Highs)].ColumnType = new VectorDataViewType(NumberDataViewType.Single, patternSize);
            schemaDefinition[nameof(CandlePatternData.Lows)].ColumnType = new VectorDataViewType(NumberDataViewType.Single, patternSize);
            schemaDefinition[nameof(CandlePatternData.Closes)].ColumnType = new VectorDataViewType(NumberDataViewType.Single, patternSize);
            schemaDefinition[nameof(CandlePatternData.Volumes)].ColumnType = new VectorDataViewType(NumberDataViewType.Single, patternSize);

            var trainData = context.Data.LoadFromEnumerable(preparedData, schemaDefinition);

            var pipeline = context.Transforms
                .Concatenate("Features", nameof(CandlePatternData.Opens), nameof(CandlePatternData.Highs),
                                         nameof(CandlePatternData.Lows), nameof(CandlePatternData.Closes),
                                         nameof(CandlePatternData.Volumes))
                .Append(context.Transforms.NormalizeMinMax("Features"))
                .Append(trainer)
                .Append(context.BinaryClassification.Calibrators.Platt(
                    labelColumnName: "Label",
                    scoreColumnName: "Score"));

            var partitions = context.Data.TrainTestSplit(trainData, testFraction: 0.2);
            var trainingData = partitions.TrainSet;
            var testData = partitions.TestSet;

            var model = pipeline.Fit(trainingData);
            var predictions = model.Transform(testData);

            var metrics = context.BinaryClassification.Evaluate(predictions, "Label", "Score");
            logService.LogMetrics(metrics);
            logService.CheckIsModelRetrained(context, trainingData, pipeline);

            context.Model.Save(model, trainData.Schema, $"{pattern.ToString()}Model.zip");
        }

        public async Task TrainModels()
        {
            var candleDataXRP = (await binanceService.GetHistoricalData("XRPUSDC", "15m", 22000)).ToList();
            foreach (var pattern in Enum.GetValues<CandlestickPattern>())
            {
                var preparedData = generationTrainingDataService.PreparePatternTrainingData(candleDataXRP, pattern);
                TrainPatternModel(preparedData, pattern);
            }
        }

        private FastTreeBinaryTrainer GetFastTreeTrainer(MLContext context)
        {
            var trainer = context.BinaryClassification.Trainers.FastTree(
                new FastTreeBinaryTrainer.Options
                {
                    NumberOfTrees = 300,
                    NumberOfLeaves = 20,  // 10, 20, 50
                    MinimumExampleCountPerLeaf = 15,
                    LearningRate = 0.3  // 0.05, 0.1, 0.2
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
