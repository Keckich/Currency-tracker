using Microsoft.ML.Data;

namespace CurrencyTracker.Business.Models
{
    public class BaseCandlePatternData
    {
        [ColumnName("Label")]
        public bool IsPattern { get; set; }
    }
}
