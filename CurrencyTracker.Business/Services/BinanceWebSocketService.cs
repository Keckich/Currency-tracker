using CurrencyTracker.Business.Services.Interfaces;
using System.Net.WebSockets;
using System.Text;

namespace CurrencyTracker.Business.Services
{
    public class BinanceWebSocketService : IBinanceWebSocketService
    {
        private readonly Dictionary<string, ClientWebSocket> sockets = new();

        public event Action<string, string>? OnMessageReceived;

        public async Task ConnectToStreamAsync(string cryptoPair, string streamType)
        {
            string url = streamType switch
            {
                "ticker" => $"wss://stream.binance.com:9443/ws/{cryptoPair.ToLower()}@ticker",
                "kline_1m" => $"wss://stream.binance.com:9443/ws/{cryptoPair.ToLower()}@kline_1m",
                "depth" => $"wss://stream.binance.com:9443/ws/{cryptoPair.ToLower()}@depth",
                _ => throw new ArgumentException("Unknown stream type", nameof(streamType))
            };

            if (sockets.ContainsKey(url))
                return;

            var socket = new ClientWebSocket();
            await socket.ConnectAsync(new Uri(url), CancellationToken.None);
            sockets[url] = socket;

            _ = Task.Run(async () =>
            {
                var buffer = new byte[4096];
                while (socket.State == WebSocketState.Open)
                {
                    var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    OnMessageReceived?.Invoke(streamType, message);
                }
            });
        }
    }
}
