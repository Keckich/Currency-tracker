using CurrencyTracker.Business.Enums;
using CurrencyTracker.Business.Helpers;
using CurrencyTracker.Business.Models;
using CurrencyTracker.Business.Services.Interfaces;
using System.Data;

namespace CurrencyTracker.Business.Services
{
    public class GenerationTrainingDataService : IGenerationTrainingDataService
    {
        private readonly ICandlestickPatternAnalyzer candlestickPatternAnalyzer;

        public GenerationTrainingDataService(ICandlestickPatternAnalyzer candlestickPatternAnalyzer)
        {
            this.candlestickPatternAnalyzer = candlestickPatternAnalyzer;
        }

        public IEnumerable<CandlePatternData> PreparePatternTrainingData(List<Candlestick> candles, CandlestickPattern pattern)
        {
            var dataset = new List<CandlePatternData>();
            var patternInfo = PatternHelper.GetPatternCheckers()[pattern];
            var patternSize = patternInfo.PatternSize;

            for (int i = patternSize - 1; i < candles.Count; i++)
            {
                var sample = new CandlePatternData
                {
                    Opens = candles.Skip(i - (patternSize - 1)).Take(patternSize).Select(c => c.Open).ToArray(),
                    Highs = candles.Skip(i - (patternSize - 1)).Take(patternSize).Select(c => c.High).ToArray(),
                    Lows = candles.Skip(i - (patternSize - 1)).Take(patternSize).Select(c => c.Low).ToArray(),
                    Closes = candles.Skip(i - (patternSize - 1)).Take(patternSize).Select(c => c.Close).ToArray(),
                    Volumes = candles.Skip(i - (patternSize - 1)).Take(patternSize).Select(c => c.Volume).ToArray(),
                    IsPattern = PatternHelper.GetPatternCheckers()[pattern].Method(candles.GetRange(i - (patternSize - 1), patternSize))
                };

                dataset.Add(sample);
            }

            dataset = GenerateBalancedPatternData(dataset, patternInfo.Method, patternSize).ToList();
            int positiveCount = dataset.Count(x => x.IsPattern);
            int negativeCount = dataset.Count(x => !x.IsPattern);
            Console.WriteLine($"Result Positive Samples: {positiveCount}, Result Negative Samples: {negativeCount}");

            return dataset;
        }

        private IEnumerable<CandlePatternData> GenerateBalancedPatternData(
            IEnumerable<CandlePatternData> dataset,
            Func<IList<Candlestick>, bool> isPattern,
            int patternSize,
            float noisePercent = 1.5f)
        {
            var random = new Random();
            var positiveSamples = dataset.Where(x => x.IsPattern).ToList();
            var negativeSamples = dataset.Where(x => !x.IsPattern).ToList();

            int negativeCount = negativeSamples.Count;
            int positiveTargetCount = (int)((0.7 * negativeCount) / 0.3);
            int requiredNewPositives = positiveTargetCount - positiveSamples.Count;

            Console.WriteLine($"Negative Samples: {negativeCount}, Initial Positive Samples: {positiveSamples.Count}");
            Console.WriteLine($"Target Positive Samples: {positiveTargetCount}, Need to Generate: {requiredNewPositives}");

            var augmentedData = new List<CandlePatternData>();

            while (augmentedData.Count < requiredNewPositives)
            {
                var original = positiveSamples[random.Next(positiveSamples.Count)];
                var candles = new List<Candlestick>();

                for (int i = 0; i < patternSize; i++)
                {
                    candles.Add(new Candlestick
                    {
                        Open = original.Opens[i] * (1 + (float)(random.NextDouble() * noisePercent / 100)),
                        High = original.Highs[i] * (1 + (float)(random.NextDouble() * noisePercent / 100)),
                        Low = original.Lows[i] * (1 + (float)(random.NextDouble() * noisePercent / 100)),
                        Close = original.Closes[i] * (1 + (float)(random.NextDouble() * noisePercent / 100)),
                        Volume = original.Volumes[i]
                    });
                }

                if (isPattern(candles))
                {
                    var newSample = new CandlePatternData
                    {
                        Opens = candles.Select(c => c.Open).ToArray(),
                        Highs = candles.Select(c => c.High).ToArray(),
                        Lows = candles.Select(c => c.Low).ToArray(),
                        Closes = candles.Select(c => c.Close).ToArray(),
                        Volumes = candles.Select(c => c.Volume).ToArray(),
                        IsPattern = true
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
