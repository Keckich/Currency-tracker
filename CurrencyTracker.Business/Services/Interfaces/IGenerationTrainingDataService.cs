using CurrencyTracker.Business.Enums;
using CurrencyTracker.Business.Models;

namespace CurrencyTracker.Business.Services.Interfaces
{
    public interface IGenerationTrainingDataService
    {
        IEnumerable<CandlePatternData> PreparePatternTrainingData(List<Candlestick> candles, CandlestickPattern pattern);
    }
}
