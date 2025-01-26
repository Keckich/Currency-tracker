using CurrencyTracker.Business.Models;

namespace CurrencyTracker.Business.Services.Interfaces
{
    public interface IModelTrainer
    {
        void TrainHammerModel(IEnumerable<Candlestick> historicalData);

        void TrainThreeWhiteSoldiersModel(IEnumerable<Candlestick> allCandles);
    }
}
