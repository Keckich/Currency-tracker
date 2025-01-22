namespace CurrencyTracker.Models
{
    public class Candlestick
    {
        public DateTime OpenTime { get; set; }

        public decimal Open { get; set; }

        public decimal High { get; set; }

        public decimal Low { get; set; }

        public decimal Close { get; set; }

        public decimal Volume { get; set; }

        public bool IsBear => Close < Open;

        public bool IsBull => Close > Open;
    }
}
