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

        bool IsThreeWhiteSoldiers(IList<Candlestick> candles);

        bool IsBearishAdvanceBlock(IList<Candlestick> candles);

        bool IsEveningStar(IList<Candlestick> candles);

        bool IsThreeBlackCrows(IList<Candlestick> candles);

        bool IsMorningStar(IList<Candlestick> candles);

        void AnalyzePatterns(IEnumerable<Candlestick> candlesticks);
    }
}
