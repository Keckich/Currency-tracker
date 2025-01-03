namespace CurrencyTracker.Models
{
    public class Trade
    {
        public int Id { get; set; }

        public int? Position { get; set; }

        public decimal Price { get; set; }

        public DateTime Date { get; set; }

        public required string Currency { get; set; }

        public double Amount { get; set; }

        public decimal Value { get; set; }

        public decimal? TakeProfit { get; set; }

        public decimal? StopLoss { get; set; }
    }
}