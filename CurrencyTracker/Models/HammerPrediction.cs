using Microsoft.ML.Data;

namespace CurrencyTracker.Models
{
    public class HammerPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool IsHammer { get; set; }
        public float Probability { get; set; }
    }
}
