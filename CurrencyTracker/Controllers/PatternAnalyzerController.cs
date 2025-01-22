using CurrencyTracker.Models;
using CurrencyTracker.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CurrencyTracker.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PatternAnalyzerController : ControllerBase
    {
        private readonly IBinanceService binanceService;

        private readonly ICandlestickPatternAnalyzer patternAnalyzer;

        public PatternAnalyzerController(IBinanceService binanceService, ICandlestickPatternAnalyzer patternAnalyzer)
        {
            this.binanceService = binanceService;
            this.patternAnalyzer = patternAnalyzer;
        }

        [HttpGet]
        public async Task<IActionResult> GetPrediction(string currency, string interval)
        {
            //TODO: make it with socket
            var candleData = await binanceService.GetHistoricalData(currency, interval);
            var prevCanle = candleData.First();
            var currCandel = candleData.Last();

            var result = new Prediction
            {
                IsHammer = patternAnalyzer.IsHammer(currCandel),
                IsHangingMan = patternAnalyzer.IsHangingMan(currCandel),
                IsBullishEngulfing = patternAnalyzer.IsBullishEngulfing(prevCanle, currCandel),
                IsBearishEngulfing = patternAnalyzer.IsBearishEngulfing(prevCanle, currCandel),
                IsDoji = patternAnalyzer.IsDoji(currCandel),
            };

            return Ok(result);
        }
    }
}
