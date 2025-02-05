using CurrencyTracker.Business.Models;

namespace CurrencyTracker.Business.Services.Interfaces
{
    public interface IGenerationTrainingDataService
    {
        IEnumerable<ThreeCandlePatternData> PrepareBearishAdvanceBlockTrainingData(List<Candlestick> candles);

        IEnumerable<ThreeCandlePatternData> PrepareThreeWhiteSoldiersTrainingData(List<Candlestick> candles);

        IEnumerable<ThreeCandlePatternData> PrepareThreeCandlePatternTrainingData(List<Candlestick> candles, Func<IList<Candlestick>, bool> isPattern);
    }
}
