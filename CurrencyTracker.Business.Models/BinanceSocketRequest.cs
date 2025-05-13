namespace CurrencyTracker.Business.Models
{
    public class BinanceSocketRequest
    {
        public string Symbol { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string? Interval { get; set; }
    }
}
