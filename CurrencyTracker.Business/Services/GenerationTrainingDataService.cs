using CurrencyTracker.Business.Models;
using CurrencyTracker.Business.Services.Interfaces;

namespace CurrencyTracker.Business.Services
{
    public class GenerationTrainingDataService : IGenerationTrainingDataService
    {
        private readonly ICandlestickPatternAnalyzer сandlestickPatternAnalyzer;

        public GenerationTrainingDataService(ICandlestickPatternAnalyzer сandlestickPatternAnalyzer)
        {
            this.сandlestickPatternAnalyzer = сandlestickPatternAnalyzer;
        }

        public IEnumerable<BearishAdvanceBlockData> PrepareBearishAdvanceBlockTrainingData(List<Candlestick> candles)
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

            dataset = GenerateBearishAdvanceBlockBalancedData(dataset).ToList();
            int positiveCount = dataset.Count(x => x.IsBearishAdvanceBlock);
            int negativeCount = dataset.Count(x => !x.IsBearishAdvanceBlock);
            Console.WriteLine($"Result Positive Samples: {positiveCount}, Result Negative Samples: {negativeCount}");

            return dataset;
        }

        private IEnumerable<BearishAdvanceBlockData> GenerateBearishAdvanceBlockBalancedData(
            IEnumerable<BearishAdvanceBlockData> dataset,
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

            dataset = dataset.Concat(augmentedData);

            return dataset.OrderBy(_ => random.Next()).ToList();
        }
    }
}
