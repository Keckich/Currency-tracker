﻿using CurrencyTracker.Business.Models;
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

        public IEnumerable<ThreeCandlePatternData> PrepareBearishAdvanceBlockTrainingData(List<Candlestick> candles)
        {
            var dataset = new List<ThreeCandlePatternData>();

            for (int i = 2; i < candles.Count; i++)
            {
                var sample = new ThreeCandlePatternData
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
                    IsPattern = candlestickPatternAnalyzer.IsBearishAdvanceBlock(candles.GetRange(i - 2, 3))
                };

                dataset.Add(sample);
            }

            dataset = GenerateThreeCandlePatternBalancedData(dataset, candlestickPatternAnalyzer.IsBearishAdvanceBlock).ToList();
            int positiveCount = dataset.Count(x => x.IsPattern);
            int negativeCount = dataset.Count(x => !x.IsPattern);
            Console.WriteLine($"Result Positive Samples: {positiveCount}, Result Negative Samples: {negativeCount}");

            return dataset;
        }

        public IEnumerable<ThreeCandlePatternData> PrepareThreeWhiteSoldiersTrainingData(List<Candlestick> candles)
        {
            var dataset = new List<ThreeCandlePatternData>();

            for (int i = 2; i < candles.Count; i++)
            {
                var sample = new ThreeCandlePatternData
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
                    IsPattern = candlestickPatternAnalyzer.IsThreeWhiteSoldiers(candles.GetRange(i - 2, 3))
                };

                dataset.Add(sample);
            }

            dataset = GenerateThreeCandlePatternBalancedData(dataset, candlestickPatternAnalyzer.IsThreeWhiteSoldiers).ToList();
            int positiveCount = dataset.Count(x => x.IsPattern);
            int negativeCount = dataset.Count(x => !x.IsPattern);
            Console.WriteLine($"Result Positive Samples: {positiveCount}, Result Negative Samples: {negativeCount}");

            return dataset;
        }

        public IEnumerable<ThreeCandlePatternData> PrepareThreeCandlePatternTrainingData(List<Candlestick> candles, Func<IList<Candlestick>, bool> isPattern)
        {
            var dataset = new List<ThreeCandlePatternData>();

            for (int i = 2; i < candles.Count; i++)
            {
                var sample = new ThreeCandlePatternData
                {
                    Open1 = candles[i - 2].Open,
                    High1 = candles[i - 2].High,
                    Low1 = candles[i - 2].Low,
                    Close1 = candles[i - 2].Close,

                    Open2 = candles[i - 1].Open,
                    High2 = candles[i - 1].High,
                    Low2 = candles[i - 1].Low,
                    Close2 = candles[i - 1].Close,

                    Open3 = candles[i].Open,
                    High3 = candles[i].High,
                    Low3 = candles[i].Low,
                    Close3 = candles[i].Close,

                    IsPattern = isPattern(candles.GetRange(i - 2, 3))
                };

                dataset.Add(sample);
            }

            dataset = GenerateThreeCandlePatternBalancedData(dataset, isPattern).ToList();
            int positiveCount = dataset.Count(x => x.IsPattern);
            int negativeCount = dataset.Count(x => !x.IsPattern);
            Console.WriteLine($"Result Positive Samples: {positiveCount}, Result Negative Samples: {negativeCount}");

            return dataset;
        }

        private IEnumerable<ThreeCandlePatternData> GenerateThreeCandlePatternBalancedData(
            IEnumerable<ThreeCandlePatternData> dataset,
            Func<IList<Candlestick>, bool> isPattern,
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

            var augmentedData = new List<ThreeCandlePatternData>();

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

                if (isPattern(candles))
                {
                    var newSample = new ThreeCandlePatternData
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
