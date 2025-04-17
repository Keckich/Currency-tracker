using CurrencyTracker.Business.Services.Interfaces;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.OnnxRuntime;
using CurrencyTracker.Business.Models;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace CurrencyTracker.Business.Services
{
    public class OnnxPredictionService : IOnnxPredictionService, IDisposable
    {
        private readonly InferenceSession session;

        private readonly float min;
        private readonly float max;

        public OnnxPredictionService(IOptions<OnnxSettings> options)
        {
            string modelPath = options.Value.ModelPath;
            session = new InferenceSession(modelPath);

            var scalerLines = File.ReadAllText(options.Value.ScalerParams).Split(',');
            min = float.Parse(scalerLines[0], CultureInfo.InvariantCulture);
            max = float.Parse(scalerLines[1], CultureInfo.InvariantCulture);
        }

        private float[] Normalize(float[] input)
        {
            return input.Select(x => (x - min) / (max - min)).ToArray();
        }

        private float Denormalize(float y)
        {
            return y * (max - min) + min;
        }

        public float Predict(float[] inputRaw)
        {
            var input = Normalize(inputRaw);
            var inputShape = new[] { 1, 200, 1 };
            var inputTensor = new DenseTensor<float>(input, inputShape);
            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor(session.InputMetadata.Keys.First(), inputTensor)
            };

            using var results = session.Run(inputs);
            var rawPrediction = results.First().AsEnumerable<float>().First();

            return Denormalize(rawPrediction);
        }

        public void Dispose()
        {
            session.Dispose();
        }
    }
}
