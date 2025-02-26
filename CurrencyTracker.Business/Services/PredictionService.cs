using CurrencyTracker.Business.Enums;
using CurrencyTracker.Business.Helpers;
using CurrencyTracker.Business.Models;
using CurrencyTracker.Business.Services.Interfaces;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace CurrencyTracker.Business.Services
{
    public class PredictionService : IPredictionService
    {
        public PatternPrediction PredictPattern(IEnumerable<Candlestick> candles, CandlestickPattern pattern)
        {
            var patternInfo = PatternHelper.GetPatternCheckers()[pattern];
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
    }
}
