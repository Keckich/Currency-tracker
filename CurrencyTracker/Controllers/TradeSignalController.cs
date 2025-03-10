using CurrencyTracker.Business.Models;
using CurrencyTracker.Business.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TradeSignalController : ControllerBase
    {
        private readonly ITradeSignalWebSocket tradeSignalWebSocket;

        public TradeSignalController(ITradeSignalWebSocket tradeSignalWebSocket)
        {
            this.tradeSignalWebSocket = tradeSignalWebSocket;
        }

        [HttpGet("/trade-signals")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await tradeSignalWebSocket.HandleWebSocketAsync(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendSignal([FromBody] TradeSignalRequest request)
        {
            await tradeSignalWebSocket.SendTradeSignalAsync(request.Signal);
            return Ok(new { message = "Trade signal sent", signal = request.Signal });
        }
    }
}
