﻿namespace CurrencyTracker.Models
{
    public class Candlestick
    {
        public DateTime OpenTime { get; set; }

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
    }
}
