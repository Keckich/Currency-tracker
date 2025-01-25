using CurrencyTracker.Business.Models;

namespace CurrencyTracker.Business.Services.Interfaces
{
    public interface ICandlestickPatternAnalyzer
    {
        bool IsHammer(Candlestick candle);

        bool IsHangingMan(Candlestick candle);

        bool IsBullishEngulfing(Candlestick previous, Candlestick current);

        bool IsBearishEngulfing(Candlestick previous, Candlestick current);

        bool IsDoji(Candlestick candle);

        /*void TrainHammerModel(IEnumerable<Candlestick> historicalData);*/

        HammerPrediction PredictHammerPattern(Candlestick candle);
    }
}
