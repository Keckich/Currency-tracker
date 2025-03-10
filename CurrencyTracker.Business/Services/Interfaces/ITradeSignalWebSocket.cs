using System.Net.WebSockets;

namespace CurrencyTracker.Business.Services.Interfaces
{
    public interface ITradeSignalWebSocket
    {
        Task HandleWebSocketAsync(WebSocket webSocket);

        Task SendTradeSignalAsync(string signal);
    }
}
