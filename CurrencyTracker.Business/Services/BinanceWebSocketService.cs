using CurrencyTracker.Business.Hubs;
using CurrencyTracker.Business.Models;
using CurrencyTracker.Business.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Net.WebSockets;
using System.Text;

namespace CurrencyTracker.Business.Services
{
    public class BinanceWebSocketService : IBinanceWebSocketService
    {
        private readonly Dictionary<string, ClientWebSocket> sockets = new();
        private readonly IHubContext<CryptoHub> hubContext;

        public BinanceWebSocketService(IHubContext<CryptoHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        public async Task ConnectToStreamAsync(BinanceSocketRequest data)
        {
            var cryptoPair = data.Symbol;
            string url = data.Type switch
            {
                "ticker" => $"wss://stream.binance.com:9443/ws/{cryptoPair.ToLower()}@ticker",
                "kline" => $"wss://stream.binance.com:9443/ws/{cryptoPair.ToLower()}@kline_{data.Interval ?? "1m"}",
                "depth" => $"wss://stream.binance.com:9443/ws/{cryptoPair.ToLower()}@depth",
                _ => throw new ArgumentException("Unknown stream type", nameof(data.Type))
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

                    await hubContext.Clients.All.SendAsync($"Receive_{data.Type}", message);
                }
            });
        }

        public async Task DisconnectFromStreamAsync(BinanceSocketRequest data)
        {
            var cryptoPair = data.Symbol;
            string url = data.Type switch
            {
                "ticker" => $"wss://stream.binance.com:9443/ws/{cryptoPair.ToLower()}@ticker",
                "kline" => $"wss://stream.binance.com:9443/ws/{cryptoPair.ToLower()}@kline_${data.Interval ?? "1m"}",
                "depth" => $"wss://stream.binance.com:9443/ws/{cryptoPair.ToLower()}@depth",
                _ => throw new ArgumentException("Unknown stream type", nameof(data.Type))
            };

            if (sockets.TryGetValue(url, out var socket))
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnected", CancellationToken.None);
                sockets.Remove(url);
            }
        }
    }
}
