using CurrencyTracker.Models;
using CurrencyTracker.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TradesController : ControllerBase
    {
        private readonly ITradeService tradeService;

        public TradesController(ITradeService tradeService)
        {
            this.tradeService = tradeService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var trades = await tradeService.GetTradesAsync();
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
