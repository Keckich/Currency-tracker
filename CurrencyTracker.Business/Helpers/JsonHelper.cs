using System.Globalization;
using System.Text.Json;

namespace CurrencyTracker.Business.Helpers
{
    public static class JsonHelper
    {
        public static long ConvertJsonElementToLong(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.Number => element.GetInt64(),
                JsonValueKind.String => long.Parse(element.GetString()!, CultureInfo.InvariantCulture),
                _ => throw new InvalidOperationException($"Unexpected JSON value kind: {element.ValueKind}")
            };
        }

        public static decimal ConvertJsonElementToDecimal(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.Number => element.GetDecimal(),
                JsonValueKind.String => decimal.Parse(element.GetString()!, CultureInfo.InvariantCulture),
                _ => throw new InvalidOperationException($"Unexpected JSON value kind: {element.ValueKind}")
            };
        }
    }
}
