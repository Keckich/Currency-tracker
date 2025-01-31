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

        private IEnumerable<BearishAdvanceBlockData> PrepareBearishAdvanceBlockTrainingData(List<Candlestick> candles)
        {
            var dataset = new List<BearishAdvanceBlockData>();

            for (int i = 2; i < candles.Count; i++)
            {
                var sample = new BearishAdvanceBlockData
                {
                    Open1 = candles[i - 2].Open,
                    High1 = candles[i - 2].High,
                    Low1 = candles[i - 2].Low,
                    Close1 = candles[i - 2].Close,
                    Volume1 = candles[i - 2].Volume,

                    Open2 = candles[i - 1].Open,
                    High2 = candles[i - 1].High,
                    Low2 = candles[i - 1].Low,
                    Close2 = candles[i - 1].Close,
                    Volume2 = candles[i - 1].Volume,

                    Open3 = candles[i].Open,
                    High3 = candles[i].High,
                    Low3 = candles[i].Low,
                    Close3 = candles[i].Close,
                    Volume3 = candles[i].Volume,
                    IsBearishAdvanceBlock = сandlestickPatternAnalyzer.IsBearishAdvanceBlock(candles.GetRange(i - 2, 3))
                };

                dataset.Add(sample);
            }

            dataset = GenerateBalancedData(dataset);
            int positiveCount = dataset.Count(x => x.IsBearishAdvanceBlock);
            int negativeCount = dataset.Count(x => !x.IsBearishAdvanceBlock);
            Console.WriteLine($"Result Positive Samples: {positiveCount}, Result Negative Samples: {negativeCount}");
            return dataset;
        }

        private List<BearishAdvanceBlockData> GenerateBalancedData(
            List<BearishAdvanceBlockData> dataset,
            float noisePercent = 1.5f)
        {
            var random = new Random();
            var positiveSamples = dataset.Where(x => x.IsBearishAdvanceBlock).ToList();
            var negativeSamples = dataset.Where(x => !x.IsBearishAdvanceBlock).ToList();

            int negativeCount = negativeSamples.Count;
            int positiveTargetCount = (int)((0.7 * negativeCount) / 0.3);
            int requiredNewPositives = positiveTargetCount - positiveSamples.Count;

            Console.WriteLine($"Negative Samples: {negativeCount}, Initial Positive Samples: {positiveSamples.Count}");
            Console.WriteLine($"Target Positive Samples: {positiveTargetCount}, Need to Generate: {requiredNewPositives}");

            var augmentedData = new List<BearishAdvanceBlockData>();

            while (augmentedData.Count < requiredNewPositives)
            {
                var original = positiveSamples[random.Next(positiveSamples.Count)];

                // Generating new data with noise
                var candles = new List<Candlestick>
                {
                    new Candlestick
                    {
                        Open = original.Open1 * (1 + (float)(random.NextDouble() * noisePercent / 100)),
                        High = original.High1 * (1 + (float)(random.NextDouble() * noisePercent / 100)),
                        Low = original.Low1 * (1 + (float)(random.NextDouble() * noisePercent / 100)),
                        Close = original.Close1 * (1 + (float)(random.NextDouble() * noisePercent / 100)),
                        Volume = original.Volume1
                    },
                    new Candlestick
                    {
                        Open = original.Open2 * (1 + (float)(random.NextDouble() * noisePercent / 100)),
                        High = original.High2 * (1 + (float)(random.NextDouble() * noisePercent / 100)),
                        Low = original.Low2 * (1 + (float)(random.NextDouble() * noisePercent / 100)),
                        Close = original.Close2 * (1 + (float)(random.NextDouble() * noisePercent / 100)),
                        Volume = original.Volume2
                    },
                    new Candlestick
                    {
                        Open = original.Open3 * (1 + (float)(random.NextDouble() * noisePercent / 100)),
                        High = original.High3 * (1 + (float)(random.NextDouble() * noisePercent / 100)),
                        Low = original.Low3 * (1 + (float)(random.NextDouble() * noisePercent / 100)),
                        Close = original.Close3 * (1 + (float)(random.NextDouble() * noisePercent / 100)),
                        Volume = original.Volume3
                    }
                };

                if (сandlestickPatternAnalyzer.IsBearishAdvanceBlock(candles))
                {
                    var newSample = new BearishAdvanceBlockData
                    {
                        Open1 = candles[0].Open,
                        High1 = candles[0].High,
                        Low1 = candles[0].Low,
                        Close1 = candles[0].Close,
                        Volume1 = candles[0].Volume,

                        Open2 = candles[1].Open,
                        High2 = candles[1].High,
                        Low2 = candles[1].Low,
                        Close2 = candles[1].Close,
                        Volume2 = candles[1].Volume,

                        Open3 = candles[2].Open,
                        High3 = candles[2].High,
                        Low3 = candles[2].Low,
                        Close3 = candles[2].Close,
                        Volume3 = candles[2].Volume,

                        IsBearishAdvanceBlock = true
                    };

                    augmentedData.Add(newSample);
                }
            }

            Console.WriteLine($"Generated {augmentedData.Count} new positive samples!");

            dataset.AddRange(augmentedData);

            return dataset.OrderBy(_ => random.Next()).ToList();
        }

        public void TrainBearishAdvanceBlockModel(IEnumerable<Candlestick> allCandles)
        {
            var context = new MLContext();
            /*var trainer = context.MulticlassClassification.Trainers.LightGbm(
                new LightGbmMulticlassTrainer.Options
                {
                    NumberOfIterations = 100,
                    LearningRate = 0.1,         // (0.05 - 0.2)
                    NumberOfLeaves = 30,        // (the more, the difficult it is)
                    MinimumExampleCountPerLeaf = 20,
                    //L2CategoricalRegularization = 0.1f,
                });*/
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

            var data = PrepareBearishAdvanceBlockTrainingData(allCandles.ToList());

            var trainData = context.Data.LoadFromEnumerable(data);

            var partitions = context.Data.TrainTestSplit(trainData, testFraction: 0.2);
            var trainingData = partitions.TrainSet;
            var testData = partitions.TestSet;

            var model = pipeline.Fit(trainingData);
            var predictions = model.Transform(testData);
            
            var metrics = context.BinaryClassification.Evaluate(predictions, "Label", "Score");

            Console.WriteLine($"Log-loss: {metrics.LogLoss}");
            Console.WriteLine($"Accuracy: {metrics.Accuracy}");
            Console.WriteLine($"AUC: {metrics.AreaUnderRocCurve}");
            Console.WriteLine($"F1 Score: {metrics.F1Score}");
            Console.WriteLine($"Precision: {metrics.PositivePrecision}");
            Console.WriteLine($"Recall: {metrics.PositiveRecall}");
            Console.WriteLine(metrics.ConfusionMatrix.GetFormattedConfusionTable());
            var cvResults = context.BinaryClassification.CrossValidate(trainingData, pipeline, numberOfFolds: 5);
            foreach (var result in cvResults)
            {
                Console.WriteLine($"Fold Accuracy: {result.Metrics.Accuracy}");
            }
            context.Model.Save(model, trainData.Schema, "bearishAdvanceBlockModel.zip");
        }
    }
}
