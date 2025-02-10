using Microsoft.ML.Data;

namespace CurrencyTracker.Business.Models
{
    public class CandlePatternData
    {
        [VectorType(4)]
        public float[] Opens { get; set; }

        [VectorType(4)]
        public float[] Highs { get; set; }

        [VectorType(4)]
        public float[] Lows { get; set; }

        [VectorType(4)]
        public float[] Closes { get; set; }

        [VectorType(4)]
        public float[] Volumes { get; set; }

        [ColumnName("Label")]
        public bool IsPattern { get; set; }
    }
}
