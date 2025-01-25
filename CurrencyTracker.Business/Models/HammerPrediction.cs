using Microsoft.ML.Data;

namespace CurrencyTracker.Business.Models
{
    public class HammerPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool IsHammer { get; set; }
        public float Probability { get; set; }
    }
}
