using CurrencyTracker.Business.Services;
using Microsoft.AspNetCore.Http;
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
    }
}
