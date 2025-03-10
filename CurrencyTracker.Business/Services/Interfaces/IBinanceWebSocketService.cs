namespace CurrencyTracker.Business.Services.Interfaces
{
    public interface IBinanceWebSocketService
    {
        event Action<string, string>? OnMessageReceived;

        Task ConnectToStreamAsync(string cryptoPair, string streamType);
    }
}
