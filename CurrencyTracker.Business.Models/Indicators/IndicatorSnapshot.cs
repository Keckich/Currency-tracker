using CurrencyTracker.Business.Models.Enums;

namespace CurrencyTracker.Business.Models.Indicators
{
    public class IndicatorSnapshot
    {
        public DateTime Date { get; set; }

        public double? Rsi { get; set; }

        public double? Ema { get; set; }

        public double? Macd { get; set; }

        public double? MacdSignal { get; set; }

        public double? BollingerUpper { get; set; }

        public double? BollingerLower { get; set; }

        public double? Momentum { get; set; }

        public double? ClosePrice { get; set; }

        public TradeSignalType? Signal { get; set; }
    }
}
