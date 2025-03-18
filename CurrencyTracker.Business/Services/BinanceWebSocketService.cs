using CurrencyTracker.Business.Hubs;
using CurrencyTracker.Business.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Net.WebSockets;
using System.Text;

namespace CurrencyTracker.Business.Services
{
    public class BinanceWebSocketService : IBinanceWebSocketService
    {
        private readonly Dictionary<string, ClientWebSocket> sockets = new();
        private readonly IHubContext<CryptoWebSocketHub> _hubContext;

        public BinanceWebSocketService(IHubContext<CryptoWebSocketHub> hubContext)
        {
            _hubContext = hubContext;
        }

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

                    await _hubContext.Clients.All.SendAsync($"Receive{streamType}", message);
                }
            });
        }

        public async Task DisconnectFromStreamAsync(string cryptoPair, string streamType)
        {
            string url = streamType switch
            {
                "ticker" => $"wss://stream.binance.com:9443/ws/{cryptoPair.ToLower()}@ticker",
                "kline_1m" => $"wss://stream.binance.com:9443/ws/{cryptoPair.ToLower()}@kline_1m",
                "depth" => $"wss://stream.binance.com:9443/ws/{cryptoPair.ToLower()}@depth",
                _ => throw new ArgumentException("Unknown stream type", nameof(streamType))
            };

            if (sockets.TryGetValue(url, out var socket))
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnected", CancellationToken.None);
                sockets.Remove(url);
            }
        }
    }
}
