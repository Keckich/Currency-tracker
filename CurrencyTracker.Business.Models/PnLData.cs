namespace CurrencyTracker.Business.Models
{
    public class PnLData
    {
        public DateTime Date { get; set; }

        public string Currency { get; set; } = string.Empty;

        public decimal Balance { get; set; }

        public decimal PnL { get; set; }
    }
}
