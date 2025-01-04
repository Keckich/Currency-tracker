using CurrencyTracker.Models;
using CurrencyTracker.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TradesController : ControllerBase
    {
        private readonly ITradeService tradeService;

        public TradesController(ITradeService tradeService)
        {
            this.tradeService = tradeService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken, [FromQuery] int page = 0, [FromQuery] int pageSize = 10)
        {
            var trades = await tradeService.GetPaginatedTradesAsync(cancellationToken, page, pageSize);
            var tradesCount = tradeService.GetTrades().Count();
            
            var result = new
            {
                Data = trades,
                TotalItems = tradesCount,
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddTrade([FromBody] Trade trade)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await tradeService.AddTradeAsync(trade);
            return Ok(trade);
        }
    }
}
