using CurrencyTracker.Data;
using CurrencyTracker.Models;
using CurrencyTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTracker.Services
{
    public class BinanceService : IBinanceService
    {
        private readonly ApplicationDbContext context;

        private readonly HttpClient httpClient;

        public BinanceService(ApplicationDbContext context, HttpClient httpClient)
        {
            this.context = context;
            this.httpClient = httpClient;
        }

        public async Task<Dictionary<string, decimal>> GetPricesAsync(IEnumerable<string>? currencies = null)
        {
            var response = await httpClient.GetFromJsonAsync<IEnumerable<BinancePrice>>("https://api.binance.com/api/v3/ticker/price");

            if (response == null)
            {
                throw new Exception("Failed to fetch prices from Binance.");
            }

            var filteredPrices = response
                .Where(price => currencies != null && currencies.Contains(price.Currency))
                .ToDictionary(price => price.Currency, price => price.Price);

            return filteredPrices;
        }
    }
}
