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

        bool IsBullishDeliberationBlock(IList<Candlestick> candles);

        bool IsEveningStar(IList<Candlestick> candles);

        bool IsThreeBlackCrows(IList<Candlestick> candles);

        bool IsMorningStar(IList<Candlestick> candles);

        bool IsBearishAbandonedBaby(IList<Candlestick> candles);

        bool IsBullishAbandonedBaby(IList<Candlestick> candles);

        bool IsThreeInsideUp(IList<Candlestick> candles);

        bool IsThreeInsideDown(IList<Candlestick> candles);

        bool IsBullishThreeLineStrike(IList<Candlestick> candles);

        bool IsBearishThreeLineStrike(IList<Candlestick> candles);

        bool IsFallingThreeMethods(IList<Candlestick> candles);

        bool IsRisingThreeMethods(IList<Candlestick> candles);

        bool IsThreeStarsInTheSouth(IList<Candlestick> candles);

        bool IsFourSoldiers(IList<Candlestick> candles);

        bool IsFourBlackCrows(IList<Candlestick> candles);

        void AnalyzePatterns(IEnumerable<Candlestick> candlesticks);
    }
}
