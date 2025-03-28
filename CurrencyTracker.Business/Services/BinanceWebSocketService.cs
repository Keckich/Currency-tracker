using CurrencyTracker.Business.Helpers;
using CurrencyTracker.Business.Hubs;
using CurrencyTracker.Business.Models;
using CurrencyTracker.Business.Services.Interfaces;
using Ecng.Common;
using Ecng.Net;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;

namespace CurrencyTracker.Business.Services
{
    public class BinanceWebSocketService : IBinanceWebSocketService
    {
        private readonly ConcurrentDictionary<string, ClientWebSocket> sockets = new();
        private readonly List<Candlestick> candles = new();
        private readonly IHubContext<CryptoHub> hubContext;
        private readonly object candlesLock = new();

        private readonly IPredictionService predictionService;

        public BinanceWebSocketService(IHubContext<CryptoHub> hubContext, IPredictionService predictionService)
        {
            this.hubContext = hubContext;
            this.predictionService = predictionService;
        }

        public async Task ConnectToStreamAsync(BinanceSocketRequest data)
        {
            var cryptoPair = data.Symbol;
            string url = data.Type switch
            {
                "ticker" => $"wss://stream.binance.com:9443/ws/{cryptoPair.ToLower()}@ticker",
                "kline" => $"wss://stream.binance.com:9443/ws/{cryptoPair.ToLower()}@kline_{data.Interval ?? "1s"}",
                "depth" => $"wss://stream.binance.com:9443/ws/{cryptoPair.ToLower()}@depth",
                _ => throw new ArgumentException("Unknown stream type", nameof(data.Type))
            };

            var key = $"{url}_data";

            if (sockets.ContainsKey(key))
                return;

            var socket = new ClientWebSocket();
            await socket.ConnectAsync(new Uri(url), CancellationToken.None);
            sockets[key] = socket;

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

        public async Task ConnectToTradeSignalAsync(BinanceSocketRequest data)
        {
            var cryptoPair = data.Symbol;
            var url = $"wss://stream.binance.com:9443/ws/{cryptoPair.ToLower()}@kline_{data.Interval ?? "1s"}";

            var key = $"{url}_signal";

            var socket = new ClientWebSocket();
            await socket.ConnectAsync(new Uri(url), CancellationToken.None);
            sockets[key] = socket;

            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var klineData = JsonConvert.DeserializeObject<BinanceKlineMessage>(message);

                    if (klineData?.Kline != null)
                    {
                        var candle = new Candlestick
                        {
                            OpenTime = DateTimeOffset.FromUnixTimeMilliseconds(klineData.Kline.OpenTime).UtcDateTime,
                            Open = ParseHelper.TryParseFloat(klineData.Kline.Open),
                            High = ParseHelper.TryParseFloat(klineData.Kline.High),
                            Low = ParseHelper.TryParseFloat(klineData.Kline.Low),
                            Close = ParseHelper.TryParseFloat(klineData.Kline.Close),
                            Volume = ParseHelper.TryParseFloat(klineData.Kline.Volume),
                            CloseTime = DateTimeOffset.FromUnixTimeMilliseconds(klineData.Kline.CloseTime).UtcDateTime
                        };

                        lock (candlesLock)
                        {
                            candles.Add(candle);
                            if (candles.Count > 100)
                                candles.RemoveAt(0);
                        }

                        var tradeSignal = predictionService.GenerateTradeSignal(new List<Candlestick>(candles));
                        var signalJson = JsonConvert.SerializeObject(tradeSignal);

                        await hubContext.Clients.All.SendAsync("ReceiveTradeSignal", signalJson);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error WebSocket: {ex.Message}");
                }
            }
        }

        public async Task DisconnectFromStreamAsync(BinanceSocketRequest data, string keyPart)
        {
            var cryptoPair = data.Symbol;
            string url = data.Type switch
            {
                "ticker" => $"wss://stream.binance.com:9443/ws/{cryptoPair.ToLower()}@ticker",
                "kline" => $"wss://stream.binance.com:9443/ws/{cryptoPair.ToLower()}@kline_{data.Interval ?? "1s"}",
                "depth" => $"wss://stream.binance.com:9443/ws/{cryptoPair.ToLower()}@depth",
                _ => throw new ArgumentException("Unknown stream type", nameof(data.Type))
            };

            var key = $"{url}_${keyPart}";

            if (sockets.TryGetValue(url, out var socket))
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnected", CancellationToken.None);
                sockets.Remove(url, out _);
            }
        }
    }
}
