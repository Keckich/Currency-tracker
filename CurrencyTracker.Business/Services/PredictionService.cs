﻿using CurrencyTracker.Business.Enums;
using CurrencyTracker.Business.Extensions;
using CurrencyTracker.Business.Helpers;
using CurrencyTracker.Business.Models;
using CurrencyTracker.Business.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace CurrencyTracker.Business.Services
{
    public class PredictionService : IPredictionService
    {
        private readonly IIndicatorService indicatorService;

        private readonly IServiceScopeFactory scopeFactory;

        public PredictionService(IIndicatorService indicatorService, IServiceScopeFactory scopeFactory)
        {
            this.indicatorService = indicatorService;
            this.scopeFactory = scopeFactory;
        }

        public PatternPrediction PredictPattern(IEnumerable<Candlestick> candles, CandlestickPattern pattern)
        {
            var patternInfo = PatternHelper.GetPatternCheckers(scopeFactory)[pattern];
            var patternSize = patternInfo.PatternSize;

            if (candles.Count() < patternSize)
                return new PatternPrediction { IsPattern = false, Probability = 0 };

            var context = new MLContext();
            var modelPath = $"{pattern.ToString()}Model.zip";
            var model = context.Model.Load(modelPath, out _);

            var lastCandles = candles.TakeLast(patternSize).ToList();

            var input = new CandlePatternData
            {
                Opens = lastCandles.Select(c => c.Open).ToArray(),
                Highs = lastCandles.Select(c => c.High).ToArray(),
                Lows = lastCandles.Select(c => c.Low).ToArray(),
                Closes = lastCandles.Select(c => c.Close).ToArray(),
                Volumes = lastCandles.Select(c => c.Volume).ToArray(),
                IsPattern = patternInfo.Method(lastCandles)
            };

            var schemaDefinition = SchemaDefinition.Create(typeof(CandlePatternData));
            schemaDefinition[nameof(CandlePatternData.Opens)].ColumnType = new VectorDataViewType(NumberDataViewType.Single, patternSize);
            schemaDefinition[nameof(CandlePatternData.Highs)].ColumnType = new VectorDataViewType(NumberDataViewType.Single, patternSize);
            schemaDefinition[nameof(CandlePatternData.Lows)].ColumnType = new VectorDataViewType(NumberDataViewType.Single, patternSize);
            schemaDefinition[nameof(CandlePatternData.Closes)].ColumnType = new VectorDataViewType(NumberDataViewType.Single, patternSize);
            schemaDefinition[nameof(CandlePatternData.Volumes)].ColumnType = new VectorDataViewType(NumberDataViewType.Single, patternSize);

            var predictionEngine = context.Model.CreatePredictionEngine<CandlePatternData, PatternPrediction>(model, inputSchemaDefinition: schemaDefinition);

            return predictionEngine.Predict(input);
        }

        private PatternPrediction DetectSingleCandlePatterns(Candlestick candle)
        {
            if (candle.IsDoji())
                return new PatternPrediction { PatternName = "Doji", Probability = 100 };

            if (candle.IsHammer())
                return new PatternPrediction { PatternName = "Hammer", Probability = 100 };

            if (candle.IsInvertedHammer())
                return new PatternPrediction { PatternName = "Inverted Hammer", Probability = 100 };

            return new PatternPrediction { PatternName = "Unknown", Probability = 0 };
        }

        public TradeSignal GenerateTradeSignal(IList<Candlestick> candles)
        {
            if (candles.Count < 26)
                return new TradeSignal { Type = TradeSignalType.Neutral, Confidence = 0 };

            var detectedPatterns = new List<(CandlestickPattern Pattern, double Probability)>();

            foreach (var pattern in Enum.GetValues<CandlestickPattern>())
            {
                var prediction = PredictPattern(candles, pattern);
                if (prediction.IsPattern && prediction.Probability > 0.7)
                {
                    detectedPatterns.Add((pattern, prediction.Probability));
                }
            }

            if (!detectedPatterns.Any())
                return new TradeSignal { Type = TradeSignalType.Neutral, Confidence = 0 };

            var rsi = indicatorService.CalculateRSI(candles, 14);
            var (macd, signal) = indicatorService.CalculateMACD(candles);
            var (upperBand, lowerBand) = indicatorService.CalculateBollingerBands(candles);
            double macdLast = macd.Last();
            double signalLast = signal.Last();
            double closePrice = candles.Last().Close;

            bool rsiOversold = rsi < 30;
            bool rsiOverbought = rsi > 70;
            bool macdBullishCross = macdLast > signalLast && macd[macd.Count - 2] <= signal[signal.Count - 2];
            bool macdBearishCross = macdLast < signalLast && macd[macd.Count - 2] >= signal[signal.Count - 2];
            bool priceNearLowerBand = closePrice <= lowerBand.Last();
            bool priceNearUpperBand = closePrice >= upperBand.Last();

            int confidence = 0;
            int maxConfidence = 100;
            int maxFactors = 4;
            int weightPerFactor = maxConfidence / maxFactors;

            bool buySignal = false;
            bool sellSignal = false;

            foreach (var (pattern, probability) in detectedPatterns)
            {
                if (pattern.IsBullish() && (rsiOversold || priceNearLowerBand || macdBullishCross))
                {
                    buySignal = true;
                    confidence += (int)(probability * weightPerFactor);
                }
                if (pattern.IsBearish() && (rsiOverbought || priceNearUpperBand || macdBearishCross))
                {
                    sellSignal = true;
                    confidence += (int)(probability * weightPerFactor);
                }
            }

            if (macdBullishCross) confidence += weightPerFactor;
            if (macdBearishCross) confidence += weightPerFactor;

            confidence = Math.Min(confidence, maxConfidence);

            if (buySignal) return new TradeSignal { Type = TradeSignalType.Buy, Confidence = confidence };
            if (sellSignal) return new TradeSignal { Type = TradeSignalType.Sell, Confidence = confidence };

            return new TradeSignal { Type = TradeSignalType.Neutral, Confidence = 0 };
        }
    }
}
