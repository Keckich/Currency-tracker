using CurrencyTracker.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyTracker.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BinanceController : ControllerBase
    {
        private readonly BinanceWebSocketService _binanceWebSocketService;

        public BinanceController(BinanceWebSocketService binanceWebSocketService)
        {
            _binanceWebSocketService = binanceWebSocketService;
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromQuery] string cryptoPair, [FromQuery] string streamType)
        {
            await _binanceWebSocketService.ConnectToStreamAsync(cryptoPair, streamType);
            return Ok(new { message = "Subscribed successfully" });
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe(
            [FromQuery] string symbol,
            [FromQuery] string type,
            [FromQuery] string? interval = null)
        {
            if (string.IsNullOrWhiteSpace(symbol) || string.IsNullOrWhiteSpace(type))
                return BadRequest(new { message = "Symbol and type are required" });

            if (type == "kline" && string.IsNullOrWhiteSpace(interval))
                return BadRequest(new { message = "Interval is required for kline" });

            string streamType = type == "kline" ? $"{type}_{interval}" : type;
            await _binanceWebSocketService.ConnectToStreamAsync(symbol, streamType);

            return Ok(new { message = $"Subscribed to {symbol} {streamType} updates" });
        }

        [HttpPost("unsubscribe")]
        public async Task<IActionResult> Unsubscribe(
            [FromQuery] string symbol,
            [FromQuery] string type,
            [FromQuery] string? interval = null)
        {
            if (string.IsNullOrWhiteSpace(symbol) || string.IsNullOrWhiteSpace(type))
                return BadRequest(new { message = "Symbol and type are required" });

            string streamType = type == "kline" ? $"{type}_{interval}" : type;
            await _binanceWebSocketService.DisconnectFromStreamAsync(symbol, streamType);

            return Ok(new { message = $"Unsubscribed from {symbol} {streamType} updates" });
        }
    }
}
