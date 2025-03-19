using Microsoft.AspNetCore.SignalR;

namespace CurrencyTracker.Business.Hubs
{
    public class CryptoHub : Hub
    {
        /*public async Task SubscribeToPriceUpdates(string symbol)
        {
            await Clients.Caller.SendAsync("ReceivePriceUpdate", $"Subscribed to {symbol} price updates.");
        }

        public async Task SubscribeToCandleData(string symbol, string interval)
        {
            await Clients.Caller.SendAsync("ReceiveCandleData", $"Subscribed to {symbol} candle data.");
        }

        public async Task SubscribeToOrderBook(string symbol)
        {
            await Clients.Caller.SendAsync("ReceiveOrderBook", $"Subscribed to {symbol} order book.");
        }*/
    }
}
