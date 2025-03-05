using CurrencyTracker.Business.Enums;

namespace CurrencyTracker.Business.Models
{
    public class TradeSignal
    {
        public TradeSignalType Type { get; set; }

        public double Confidence { get; set; }
    }
}
