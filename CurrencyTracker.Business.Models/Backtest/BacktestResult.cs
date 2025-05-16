namespace CurrencyTracker.Business.Models.Backtest
{
    public class BacktestResult
    {
        public decimal FinalBalance { get; set; }

        public int TotalTrades { get; set; }

        public int WinningTrades { get; set; }

        public int LosingTrades { get; set; }

        public decimal MaxDrawdown { get; set; }

        public decimal ProfitPercent => StartBalance == 0 ? 0 : (FinalBalance - StartBalance) / StartBalance * 100;

        public decimal StartBalance { get; set; }
    }
}
