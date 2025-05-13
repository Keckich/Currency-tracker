namespace CurrencyTracker.Business.Models
{
    public class Candlestick
    {
        public DateTime OpenTime { get; set; }

        public DateTime CloseTime { get; set; }

        //float is for ML model
        public float Open { get; set; }

        public float High { get; set; }

        public float Low { get; set; }

        public float Close { get; set; }

        public float Volume { get; set; }

        public bool IsBear => Close < Open;

        public bool IsBull => Close > Open;

        public float Body => Math.Abs(Close - Open);

        public float LowerShadow => Math.Min(Open, Close) - Low;

        public float UpperShadow => High - Math.Max(Open, Close);

        public float Range => High - Low;

        public float TotalSize => High - Low;

        public bool HasLongUpperShadow(float factor = 1.5f) => UpperShadow > Body * factor;

        public bool HasLongLowerShadow(float factor = 1.5f) => LowerShadow > Body * factor;

        public bool IsDoji() => Body <= (Range * 0.1);

        public bool IsHammer()
            => LowerShadow > (Body * 2) &&
               UpperShadow < (Body * 0.5) &&
               Body < Range * 0.3;

        public bool IsInvertedHammer()
            => UpperShadow > (Body * 2) &&
               LowerShadow < (Body * 0.5) &&
               Body < Range * 0.3;

        public static bool IsBullishEngulfing(Candlestick first, Candlestick second)
            => first.IsBear &&
               second.IsBull &&
               second.Open < first.Close &&
               second.Close > first.Open;

        public static bool IsBearishEngulfing(Candlestick first, Candlestick second)
            => first.IsBull &&
               second.IsBear &&
               second.Open > first.Close &&
               second.Close < first.Open;

        public static bool IsTweezerTop(Candlestick first, Candlestick second)
            => first.High == second.High && first.IsBull && second.IsBear;

        public static bool IsTweezerBottom(Candlestick first, Candlestick second)
            => first.Low == second.Low && first.IsBear && second.IsBull;
    }
}
