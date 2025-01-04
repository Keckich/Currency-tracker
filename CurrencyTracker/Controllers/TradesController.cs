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
        [Route("paginated")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken, [FromQuery] PaginationInfo paginationInfo)
        {
            var trades = await tradeService.GetPaginatedTradesAsync(cancellationToken, paginationInfo);
            var tradesCount = tradeService.GetTrades().Count();
            
            var result = new
            {
                Data = trades,
                TotalItems = tradesCount,
            };

            return Ok(result);
        }

        [HttpGet]
        public IActionResult Get()
        {
            var trades = tradeService.GetTrades();
            return Ok(trades);
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
