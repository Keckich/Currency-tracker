namespace CurrencyTracker.Business.Models
{
    public class PatternInfo
    {
        public int PatternSize { get; set; }

        public Func<IList<Candlestick>, bool> Method { get; set; }
    }
}
