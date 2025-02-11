using CurrencyTracker.Business.Enums;
using CurrencyTracker.Business.Models;
using CurrencyTracker.Business.Services.Interfaces;
using CurrencyTracker.Common;

namespace CurrencyTracker.Business.Helpers
{
    public static class PatternHelper
    {
        private static Lazy<Dictionary<CandlestickPattern, PatternInfo>> _patternCheckers =
        new(() =>
        {
            var patternAnalyzer = DependencyResolver.GetService<ICandlestickPatternAnalyzer>();
            return new Dictionary<CandlestickPattern, PatternInfo>
            {
                { CandlestickPattern.ThreeWhiteSoldiers, new PatternInfo { Method = patternAnalyzer.IsThreeWhiteSoldiers, PatternSize = 3 } },
                { CandlestickPattern.ThreeBlackCrows, new PatternInfo { Method = patternAnalyzer.IsThreeBlackCrows, PatternSize = 3 } },
                { CandlestickPattern.EveningStar, new PatternInfo { Method = patternAnalyzer.IsEveningStar, PatternSize = 3 } },
                { CandlestickPattern.MorningStar, new PatternInfo { Method = patternAnalyzer.IsMorningStar, PatternSize = 3 } },
                { CandlestickPattern.BearishAdvanceBlock, new PatternInfo { Method = patternAnalyzer.IsBearishAdvanceBlock, PatternSize = 3 } },
                { CandlestickPattern.BullishDeliberationBlock, new PatternInfo { Method = patternAnalyzer.IsBullishDeliberationBlock, PatternSize = 3 } },
                { CandlestickPattern.BearishAbandonedBaby, new PatternInfo { Method = patternAnalyzer.IsBearishAbandonedBaby, PatternSize = 3 } },
                { CandlestickPattern.BullishAbandonedBaby, new PatternInfo { Method = patternAnalyzer.IsBullishAbandonedBaby, PatternSize = 3 } },
                { CandlestickPattern.ThreeInsideUp, new PatternInfo { Method = patternAnalyzer.IsThreeInsideUp, PatternSize = 3 } },
                { CandlestickPattern.ThreeInsideDown, new PatternInfo { Method = patternAnalyzer.IsThreeInsideDown, PatternSize = 3 } },
            };
        });

        public static Dictionary<CandlestickPattern, PatternInfo> GetPatternCheckers()
            => _patternCheckers.Value;
    }
}
