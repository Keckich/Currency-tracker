namespace CurrencyTracker.Business.Models
{
    public class BinancePrice
    {
        public string Currency { get; set; } = string.Empty;

        public decimal Price { get; set; }
    }
}
