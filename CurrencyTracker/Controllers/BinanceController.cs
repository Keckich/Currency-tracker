using CurrencyTracker.Business.Models;
using CurrencyTracker.Business.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BinanceController : ControllerBase
    {
        private readonly IBinanceWebSocketService binanceWebSocketService;

        public BinanceController(IBinanceWebSocketService binanceWebSocketService)
        {
            this.binanceWebSocketService = binanceWebSocketService;
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe(
            [FromBody] BinanceSocketRequest data)
        {
            if (string.IsNullOrWhiteSpace(data.Symbol) || string.IsNullOrWhiteSpace(data.Type))
                return BadRequest(new { message = "Symbol and type are required" });

            if (data.Type == "kline" && string.IsNullOrWhiteSpace(data.Interval))
                return BadRequest(new { message = "Interval is required for kline" });
            
            await binanceWebSocketService.ConnectToStreamAsync(data);

            return Ok(new { message = $"Subscribed to {data.Symbol} {data.Type} updates" });
        }

        [HttpPost("unsubscribe")]
        public async Task<IActionResult> Unsubscribe(
            [FromBody] BinanceSocketRequest data)
        {
            if (string.IsNullOrWhiteSpace(data.Symbol) || string.IsNullOrWhiteSpace(data.Type))
                return BadRequest(new { message = "Symbol and type are required" });

            await binanceWebSocketService.DisconnectFromStreamAsync(data, "data");

            return Ok(new { message = $"Unsubscribed from {data.Symbol} {data.Type} updates" });
        }
    }
}
