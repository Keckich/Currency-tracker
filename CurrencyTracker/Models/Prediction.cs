using Microsoft.ML.Data;

namespace CurrencyTracker.Models
{
    public class Prediction
    {
        public bool IsHammer { get; set; }

        public bool IsHangingMan { get; set; }

        public bool IsBullishEngulfing { get; set; }

        public bool IsBearishEngulfing { get; set; }

        public bool IsDoji { get; set; }
    }
}
