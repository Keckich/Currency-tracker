namespace CurrencyTracker.Models
{
    public class PnLData
    {
        public DateTime Date { get; set; }

        public string Currency { get; set; } = string.Empty;

        public decimal Balance { get; set; }
    }
}
