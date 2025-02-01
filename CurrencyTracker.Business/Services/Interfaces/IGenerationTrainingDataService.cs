using CurrencyTracker.Business.Models;

namespace CurrencyTracker.Business.Services.Interfaces
{
    public interface IGenerationTrainingDataService
    {
        IEnumerable<BearishAdvanceBlockData> PrepareBearishAdvanceBlockTrainingData(List<Candlestick> candles);
    }
}
