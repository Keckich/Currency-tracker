using CurrencyTracker.Business.Models.Enums;

namespace CurrencyTracker.Business.Extensions
{
    public static class PatternExtensions
    {
        public static bool IsBullish(this CandlestickPattern pattern)
        {
            return pattern switch
            {
                CandlestickPattern.ThreeWhiteSoldiers => true,
                CandlestickPattern.MorningStar => true,
                CandlestickPattern.BullishAbandonedBaby => true,
                CandlestickPattern.ThreeInsideUp => true,
                CandlestickPattern.BullishThreeLineStrike => true,
                CandlestickPattern.RisingThreeMethods => true,
                CandlestickPattern.ThreeStarsInTheSouth => true,
                CandlestickPattern.FourSoldiers => true,
                CandlestickPattern.BullishDeliberationBlock => true,
                _ => false
            };
        }

        public static bool IsBearish(this CandlestickPattern pattern)
        {
            return pattern switch
            {
                CandlestickPattern.ThreeBlackCrows => true,
                CandlestickPattern.EveningStar => true,
                CandlestickPattern.BearishAbandonedBaby => true,
                CandlestickPattern.ThreeInsideDown => true,
                CandlestickPattern.BearishThreeLineStrike => true,
                CandlestickPattern.FallingThreeMethods => true,
                CandlestickPattern.FourBlackCrows => true,
                CandlestickPattern.BearishAdvanceBlock => true,
                _ => false
            };
        }
    }
}
