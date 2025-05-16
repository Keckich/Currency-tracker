using CurrencyTracker.Business.Data;
using CurrencyTracker.Business.Helpers;
using CurrencyTracker.Business.Models;
using CurrencyTracker.Business.Services.Interfaces;
using System;
using System.Net.Http.Json;
using System.Text.Json;

namespace CurrencyTracker.Business.Services
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

        public async Task<string> GetOrderBookData(string symbol, int limit)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync($"https://api.binance.com/api/v3/depth?symbol={symbol.ToUpper()}&limit={limit}");

            if (!response.IsSuccessStatusCode)
                throw new Exception(await response.Content.ReadAsStringAsync());

            var content = await response.Content.ReadAsStringAsync();

            return content;
        }

        public async Task<IEnumerable<Candlestick>> GetHistoricalData(string symbol, string interval)
        {
            var httpClient = new HttpClient();
            var url = $"https://api.binance.com/api/v3/klines?symbol={symbol}&interval={interval}";
            var response = await httpClient.GetStringAsync(url);
            var json = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<object>>>(response);

            return json?.Select(data => new Candlestick
            {
                OpenTime = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(data[0])).DateTime,
                Open = ParseHelper.TryParseFloat(data[1]),
                High = ParseHelper.TryParseFloat(data[2]),
                Low = ParseHelper.TryParseFloat(data[3]),
                Close = ParseHelper.TryParseFloat(data[4]),
                Volume = ParseHelper.TryParseFloat(data[5]),
            }).ToList() ?? Enumerable.Empty<Candlestick>();
        }

        /*public async Task<IEnumerable<Candlestick>> GetHistoricalData(string symbol, string interval, int limit = 5000)
        {
            var httpClient = new HttpClient();
            var allCandlesticks = new List<Candlestick>();
            long? endTime = null;

            while (allCandlesticks.Count < limit)
            {
                var url = $"https://api.binance.com/api/v3/klines?symbol={symbol}&interval={interval}&limit=1000";
                if (endTime.HasValue)
                {
                    url += $"&endTime={endTime}";
                }

                var response = await httpClient.GetStringAsync(url);
                var json = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<object>>>(response);

                if (json == null || !json.Any())
                {
                    break;
                }

                var candlesticks = json.Select(data => new Candlestick
                {
                    OpenTime = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(data[0])).DateTime,
                    Open = ParseHelper.TryParseFloat(data[1]),
                    High = ParseHelper.TryParseFloat(data[2]),
                    Low = ParseHelper.TryParseFloat(data[3]),
                    Close = ParseHelper.TryParseFloat(data[4]),
                    Volume = ParseHelper.TryParseFloat(data[5]),
                }).ToList();

                allCandlesticks.AddRange(candlesticks);
                endTime = ToUnixTimestamp(candlesticks.First().OpenTime) - 1;
                if (candlesticks.Count < 1000)
                {
                    break;
                }
            }

            return allCandlesticks.Take(limit).OrderBy(c => c.OpenTime);
        }*/

        public async Task<IEnumerable<Candlestick>> GetHistoricalData(string symbol, string interval, DateTime? start = null, DateTime? end = null, int limit = 5000)
        {
            var httpClient = new HttpClient();
            var allCandlesticks = new List<Candlestick>();

            long? startTime = start.HasValue
                ? new DateTimeOffset(start.Value).ToUnixTimeMilliseconds()
                : null;

            long? endTime = end.HasValue
                ? new DateTimeOffset(end.Value).ToUnixTimeMilliseconds()
                : DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            while (allCandlesticks.Count < limit)
            {
                int fetchLimit = Math.Min(1000, limit - allCandlesticks.Count);
                var url = $"https://api.binance.com/api/v3/klines?symbol={symbol}&interval={interval}&limit={fetchLimit}&endTime={endTime}";

                var response = await httpClient.GetStringAsync(url);
                var json = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<object>>>(response);

                if (json == null || !json.Any())
                {
                    break;
                }

                var candlesticks = json.Select(data => new Candlestick
                {
                    OpenTime = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(data[0])).DateTime,
                    Open = ParseHelper.TryParseFloat(data[1]),
                    High = ParseHelper.TryParseFloat(data[2]),
                    Low = ParseHelper.TryParseFloat(data[3]),
                    Close = ParseHelper.TryParseFloat(data[4]),
                    Volume = ParseHelper.TryParseFloat(data[5]),
                }).ToList();

                if (start.HasValue)
                {
                    candlesticks = candlesticks.Where(c => c.OpenTime >= start.Value).ToList();
                }

                allCandlesticks.InsertRange(0, candlesticks);

                var oldestCandle = candlesticks.FirstOrDefault();
                if (oldestCandle == null || (start.HasValue && oldestCandle.OpenTime <= start.Value))
                    break;

                endTime = ToUnixTimestamp(candlesticks.First().OpenTime) - 1;

                if (candlesticks.Count < fetchLimit)
                {
                    break;
                }
            }

            return allCandlesticks.TakeLast(limit).OrderBy(c => c.OpenTime);
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
