using Microsoft.ML.Data;

namespace CurrencyTracker.Business.Models
{
    public class PatternPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool IsPattern { get; set; }

        public float Probability { get; set; }
    }
}
