using CurrencyTracker.Business.Models.Enums;
using CurrencyTracker.Business.Models;

namespace CurrencyTracker.Business.Services.Interfaces
{
    public interface IModelTrainer
    {
        void TrainPatternModel(IEnumerable<CandlePatternData> preparedData, CandlestickPattern pattern);

        Task TrainModels();
    }
}
