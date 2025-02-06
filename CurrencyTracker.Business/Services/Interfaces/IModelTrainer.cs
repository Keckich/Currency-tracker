using CurrencyTracker.Business.Enums;
using CurrencyTracker.Business.Models;

namespace CurrencyTracker.Business.Services.Interfaces
{
    public interface IModelTrainer
    {
        void TrainHammerModel(IEnumerable<Candlestick> historicalData);

        void TrainThreeWhiteSoldiersModel(IEnumerable<Candlestick> allCandles);

        void TrainThreeCandlePatternModel(IEnumerable<ThreeCandlePatternData> preparedData, CandlestickPattern pattern);
    }
}
