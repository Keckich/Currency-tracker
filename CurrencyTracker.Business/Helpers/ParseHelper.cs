using System.Globalization;

namespace CurrencyTracker.Business.Helpers
{
    public static class ParseHelper
    {
        public static float TryParseFloat(object value)
        {
            return float.TryParse(value?.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var result)
                ? result
                : 0f;
        }
    }
}
