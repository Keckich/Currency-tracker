using CurrencyTracker.Business.Models.Enums;
using CurrencyTracker.Business.Models;
using CurrencyTracker.Business.Services.Interfaces;
using CurrencyTracker.Common;
using Microsoft.Extensions.DependencyInjection;

namespace CurrencyTracker.Business.Helpers
{
    public static class PatternHelper
    {
        public static Dictionary<CandlestickPattern, PatternInfo> GetPatternCheckers(IServiceScopeFactory scopeFactory)
        {
            using var scope = scopeFactory.CreateScope();
            var patternAnalyzer = scope.ServiceProvider.GetRequiredService<ICandlestickPatternAnalyzer>();

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
            { CandlestickPattern.BullishThreeLineStrike, new PatternInfo { Method = patternAnalyzer.IsBullishThreeLineStrike, PatternSize = 4 } },
            { CandlestickPattern.BearishThreeLineStrike, new PatternInfo { Method = patternAnalyzer.IsBearishThreeLineStrike, PatternSize = 4 } },
            { CandlestickPattern.FallingThreeMethods, new PatternInfo { Method = patternAnalyzer.IsFallingThreeMethods, PatternSize = 5 } },
            { CandlestickPattern.RisingThreeMethods, new PatternInfo { Method = patternAnalyzer.IsRisingThreeMethods, PatternSize = 5 } },
            { CandlestickPattern.ThreeStarsInTheSouth, new PatternInfo { Method = patternAnalyzer.IsThreeStarsInTheSouth, PatternSize = 3 } },
            { CandlestickPattern.FourSoldiers, new PatternInfo { Method = patternAnalyzer.IsFourSoldiers, PatternSize = 4 } },
            { CandlestickPattern.FourBlackCrows, new PatternInfo { Method = patternAnalyzer.IsFourBlackCrows, PatternSize = 4 } },
        };
        }

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
                { CandlestickPattern.BullishThreeLineStrike, new PatternInfo { Method = patternAnalyzer.IsBullishThreeLineStrike, PatternSize = 4 } },
                { CandlestickPattern.BearishThreeLineStrike, new PatternInfo { Method = patternAnalyzer.IsBearishThreeLineStrike, PatternSize = 4 } },
                { CandlestickPattern.FallingThreeMethods, new PatternInfo { Method = patternAnalyzer.IsFallingThreeMethods, PatternSize = 5 } },
                { CandlestickPattern.RisingThreeMethods, new PatternInfo { Method = patternAnalyzer.IsRisingThreeMethods, PatternSize = 5 } },
                { CandlestickPattern.ThreeStarsInTheSouth, new PatternInfo { Method = patternAnalyzer.IsThreeStarsInTheSouth, PatternSize = 3 } },
                { CandlestickPattern.FourSoldiers, new PatternInfo { Method = patternAnalyzer.IsFourSoldiers, PatternSize = 4 } },
                { CandlestickPattern.FourBlackCrows, new PatternInfo { Method = patternAnalyzer.IsFourBlackCrows, PatternSize = 4 } },
            };
        });

        public static Dictionary<CandlestickPattern, PatternInfo> GetPatternCheckers()
            => _patternCheckers.Value;
    }
}
