namespace CurrencyTracker.Business.Services.Interfaces
{
    public interface IOnnxPredictionService
    {
        float Predict(float[] inputRaw);

        void Dispose();
    }
}
