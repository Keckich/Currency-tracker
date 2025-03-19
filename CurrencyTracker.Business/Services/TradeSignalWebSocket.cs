using CurrencyTracker.Business.Services.Interfaces;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

namespace CurrencyTracker.Business.Services
{
    public class TradeSignalWebSocket : ITradeSignalWebSocket
    {
        private static readonly List<WebSocket> clients = new();
        private readonly IBinanceWebSocketService binanceWebSocketService;

        public TradeSignalWebSocket(IBinanceWebSocketService binanceWebSocketService)
        {
            this.binanceWebSocketService = binanceWebSocketService;
        }

        public async Task HandleWebSocketAsync(WebSocket webSocket)
        {
            clients.Add(webSocket);
            var buffer = new byte[1024 * 4];

            try
            {
                var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                while (!receiveResult.CloseStatus.HasValue)
                {
                    receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebSocket error: {ex.Message}");
            }
            finally
            {
                clients.Remove(webSocket);
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            }
        }

        public async Task SendTradeSignalAsync(string signal)
        {
            var message = JsonConvert.SerializeObject(new { signal });
            var buffer = Encoding.UTF8.GetBytes(message);

            foreach (var client in clients.ToList())
            {
                if (client.State == WebSocketState.Open)
                {
                    await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}
