using Newtonsoft.Json;

namespace CurrencyTracker.Business.Models
{
    public class BinanceKlineMessage
    {
        [JsonProperty("k")]
        public BinanceKline Kline { get; set; }
    }
}
