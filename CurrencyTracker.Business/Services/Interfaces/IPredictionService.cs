using CurrencyTracker.Business.Models.Enums;
using CurrencyTracker.Business.Models;
using CurrencyTracker.Business.Models.Indicators;

namespace CurrencyTracker.Business.Services.Interfaces
{
    public interface IPredictionService
    {
        PatternPrediction PredictPattern(IEnumerable<Candlestick> candles, CandlestickPattern pattern);

        TradeSignal GenerateTradeSignal(IList<Candlestick> candles);

        IEnumerable<IndicatorSnapshot> GenerateSignals(IEnumerable<Candlestick> candles);

        TradeSignalType GenerateSignal(IEnumerable<Candlestick> candles);
    }
}
