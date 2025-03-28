using CurrencyTracker.Business.Helpers;
using CurrencyTracker.Business.Models;
using CurrencyTracker.Business.Services;
using CurrencyTracker.Business.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TradeSignalController : ControllerBase
    {
        private readonly IBinanceWebSocketService binanceWebSocketService;

        private readonly ICandlestickPatternAnalyzer candlestickPatternAnalyzer;

        public TradeSignalController(IBinanceWebSocketService binanceWebSocketService, ICandlestickPatternAnalyzer candlestickPatternAnalyzer)
        {
            this.binanceWebSocketService = binanceWebSocketService;
            this.candlestickPatternAnalyzer = candlestickPatternAnalyzer;
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe(
            [FromBody] BinanceSocketRequest data)
        {
            if (string.IsNullOrWhiteSpace(data.Symbol) || string.IsNullOrWhiteSpace(data.Type))
                return BadRequest(new { message = "Symbol and type are required" });

            if (data.Type == "kline" && string.IsNullOrWhiteSpace(data.Interval))
                return BadRequest(new { message = "Interval is required for kline" });

            //TODO: create BackgroundService
            _ = Task.Run(async () => await binanceWebSocketService.ConnectToTradeSignalAsync(data));

            return Ok(new { message = $"Subscribed to {data.Symbol} {data.Type} trade signal updates" });
        }

        [HttpPost("unsubscribe")]
        public async Task<IActionResult> Unsubscribe(
            [FromBody] BinanceSocketRequest data)
        {
            if (string.IsNullOrWhiteSpace(data.Symbol) || string.IsNullOrWhiteSpace(data.Type))
                return BadRequest(new { message = "Symbol and type are required" });

            //TODO: remove keyPart param
            _ = Task.Run(async () => await binanceWebSocketService.DisconnectFromStreamAsync(data, "signal"));

            return Ok(new { message = $"Unsubscribed from {data.Symbol} {data.Type} trade signal updates" });
        }
    }
}
