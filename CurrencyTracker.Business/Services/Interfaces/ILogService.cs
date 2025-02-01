using Microsoft.ML;
using Microsoft.ML.Data;

namespace CurrencyTracker.Business.Services.Interfaces
{
    public interface ILogService
    {
        void LogMetrics(CalibratedBinaryClassificationMetrics metrics);

        void CheckIsModelRetrained(MLContext context, IDataView trainingData, IEstimator<ITransformer> pipeline);
    }
}
