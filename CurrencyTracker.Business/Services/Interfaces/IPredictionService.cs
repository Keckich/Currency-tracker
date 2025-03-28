using CurrencyTracker.Business.Enums;
using CurrencyTracker.Business.Models;

namespace CurrencyTracker.Business.Services.Interfaces
{
    public interface IPredictionService
    {
        PatternPrediction PredictPattern(IEnumerable<Candlestick> candles, CandlestickPattern pattern);

        TradeSignal GenerateTradeSignal(IList<Candlestick> candles);
    }
}
