using Newtonsoft.Json;

namespace CurrencyTracker.Business.Models
{
    public class BinanceKline
    {
        [JsonProperty("t")]
        public long OpenTime { get; set; }

        [JsonProperty("o")]
        public string Open { get; set; }

        [JsonProperty("h")]
        public string High { get; set; }

        [JsonProperty("l")]
        public string Low { get; set; }

        [JsonProperty("c")]
        public string Close { get; set; }

        [JsonProperty("v")]
        public string Volume { get; set; }

        [JsonProperty("T")]
        public long CloseTime { get; set; }
    }
}
