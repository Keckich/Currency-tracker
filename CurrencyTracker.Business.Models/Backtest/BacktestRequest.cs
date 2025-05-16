namespace CurrencyTracker.Business.Models.Backtest
{
    public class BacktestRequest
    {
        public string Symbol { get; set; } = string.Empty;

        public string Interval { get; set; } = string.Empty;

        public DateTime Start { get; set; }

        public DateTime End { get; set; }
    }
}
