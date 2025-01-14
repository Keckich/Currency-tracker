using CurrencyTracker.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderBookController : ControllerBase
    {
        private readonly IBinanceService binanceService;

        public OrderBookController(IBinanceService binanceService)
        {
            this.binanceService = binanceService;
        }

        [HttpGet("{symbol}")]
        public async Task<IActionResult> GetOrderBook(string symbol, [FromQuery] int limit = 50)
        {
            var content = await binanceService.GetOrderBookData(symbol, limit);
            return Content(content, "application/json");
        }
    }
}
