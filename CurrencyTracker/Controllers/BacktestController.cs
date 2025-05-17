using CurrencyTracker.Business.Models.Backtest;
using CurrencyTracker.Business.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyTracker.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BacktestController : ControllerBase
    {
        private readonly IBacktestService backtestService;

        private readonly IBinanceService binanceService;

        public BacktestController(IBacktestService backtestService, IBinanceService binanceService)
        {
            this.backtestService = backtestService;
            this.binanceService = binanceService;
        }

        [HttpPost]
        public async Task<IActionResult> RunBacktest([FromBody] BacktestRequest request)
        {
            var candles = (await binanceService.GetHistoricalData(request.Symbol, request.Interval, request.Start, request.End)).ToList();
            var result = backtestService.Run(candles);
            return Ok(result);
        }
    }
}
