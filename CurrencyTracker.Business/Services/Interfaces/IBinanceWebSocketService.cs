namespace CurrencyTracker.Business.Services.Interfaces
{
    public interface IBinanceWebSocketService
    {
        Task ConnectToStreamAsync(string cryptoPair, string streamType);

        Task DisconnectFromStreamAsync(string cryptoPair, string streamType);
    }
}
