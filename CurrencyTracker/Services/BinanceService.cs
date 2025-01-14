﻿using CurrencyTracker.Data;
using CurrencyTracker.Helpers;
using CurrencyTracker.Models;
using CurrencyTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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

        public async Task<Dictionary<string, Dictionary<DateTime, decimal>?>> GetDailyClosingPricesAsync(IEnumerable<string> currencies, int days, CancellationToken cancellationToken)
        {
            var tasks = currencies.Select(async symbol =>
            {
                var url = GetDailyClosingPricesUrl(symbol, days);

                using var client = new HttpClient();
                var response = await client.GetAsync(url, cancellationToken);

                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Failed to fetch prices for {symbol}");

                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var data = JsonSerializer.Deserialize<List<JsonElement[]>>(content);

                return new
                {
                    Symbol = symbol,
                    Prices = data?.ToDictionary(
                        k => DateTimeOffset.FromUnixTimeMilliseconds(JsonHelper.ConvertJsonElementToLong(k[0])).DateTime,
                        v => JsonHelper.ConvertJsonElementToDecimal(v[4])
                    )
                };
            });

            var results = await Task.WhenAll(tasks);

            return results.ToDictionary(
                r => r.Symbol,
                r => r.Prices
            );
        }

        private string GetDailyClosingPricesUrl(string symbol, int days)
        {
            var intervalParams = days == 1 ? $"interval=1h&limit=24" : $"interval=1d&limit={days}";
            var url = $"https://api.binance.com/api/v3/klines?symbol={symbol}&{intervalParams}";

            return url;
        }

        private long ToUnixTimestamp(DateTime date)
        {
            return new DateTimeOffset(date).ToUnixTimeMilliseconds();
        }
    }
}
