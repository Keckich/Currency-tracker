using CurrencyTracker.Business.Models;

namespace CurrencyTracker.Business.Services.Interfaces
{
    public interface IPredictionService
    {
        PatternPrediction PredictHammerPattern(Candlestick candle);

        PatternPrediction PredictThreeWhiteSoldiersPattern(IEnumerable<Candlestick> candle);

        PatternPrediction PredictBearishAdvanceBlock(IEnumerable<Candlestick> candles);
    }
}
