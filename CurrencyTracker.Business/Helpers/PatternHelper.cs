﻿using CurrencyTracker.Business.Enums;
using CurrencyTracker.Business.Models;
using CurrencyTracker.Business.Services.Interfaces;
using CurrencyTracker.Common;

namespace CurrencyTracker.Business.Helpers
{
    public static class PatternHelper
    {
        private static Lazy<Dictionary<CandlestickPattern, Func<IList<Candlestick>, bool>>> _patternCheckers =
        new(() =>
        {
            var patternAnalyzer = DependencyResolver.GetService<ICandlestickPatternAnalyzer>();
            return new Dictionary<CandlestickPattern, Func<IList<Candlestick>, bool>>
            {
                { CandlestickPattern.ThreeWhiteSoldiers, patternAnalyzer.IsThreeWhiteSoldiers },
                { CandlestickPattern.ThreeBlackCrows, patternAnalyzer.IsThreeBlackCrows },
                { CandlestickPattern.EveningStar, patternAnalyzer.IsEveningStar },
                { CandlestickPattern.MorningStar, patternAnalyzer.IsMorningStar },
                { CandlestickPattern.BearishAdvanceBlock, patternAnalyzer.IsBearishAdvanceBlock },
                { CandlestickPattern.BullishDeliberationBlock, patternAnalyzer.IsBullishDeliberationBlock },
                { CandlestickPattern.BearishAbandonedBaby, patternAnalyzer.IsBearishAbandonedBaby },
                { CandlestickPattern.BullishAbandonedBaby, patternAnalyzer.IsBullishAbandonedBaby },
                { CandlestickPattern.ThreeInsideUp, patternAnalyzer.IsThreeInsideUp },
                { CandlestickPattern.ThreeInsideDown, patternAnalyzer.IsThreeInsideDown },
            };
        });

        public static Dictionary<CandlestickPattern, Func<IList<Candlestick>, bool>> GetPatternCheckers()
            => _patternCheckers.Value;
    }
}
