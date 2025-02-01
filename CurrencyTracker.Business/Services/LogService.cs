using CurrencyTracker.Business.Services.Interfaces;
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyTracker.Business.Services
{
    public class LogService : ILogService
    {
        public void LogMetrics(CalibratedBinaryClassificationMetrics metrics)
        {
            Console.WriteLine($"Log-loss: {metrics.LogLoss}");
            Console.WriteLine($"Accuracy: {metrics.Accuracy}");
            Console.WriteLine($"AUC: {metrics.AreaUnderRocCurve}");
            Console.WriteLine($"F1 Score: {metrics.F1Score}");
            Console.WriteLine($"Precision: {metrics.PositivePrecision}");
            Console.WriteLine($"Recall: {metrics.PositiveRecall}");
            Console.WriteLine(metrics.ConfusionMatrix.GetFormattedConfusionTable());
        }

        public void CheckIsModelRetrained(MLContext context, IDataView trainingData, IEstimator<ITransformer> pipeline)
        {
            var cvResults = context.BinaryClassification.CrossValidate(trainingData, pipeline, numberOfFolds: 5);
            foreach (var result in cvResults)
            {
                Console.WriteLine($"Fold Accuracy: {result.Metrics.Accuracy}");
            }
        }
    }
}
