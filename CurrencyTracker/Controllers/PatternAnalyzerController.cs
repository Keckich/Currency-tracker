using CurrencyTracker.Business.Models;
using CurrencyTracker.Business.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
            var prediction = patternAnalyzer.PredictHammerPattern(currCandel);

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
