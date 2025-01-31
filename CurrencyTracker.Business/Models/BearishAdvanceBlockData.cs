using Microsoft.ML.Data;

namespace CurrencyTracker.Business.Models
{
    public class BearishAdvanceBlockData
    {
        public float Open1 { get; set; }

        public float High1 { get; set; }

        public float Low1 { get; set; }

        public float Close1 { get; set; }

        public float Volume1 { get; set; }

        public float Open2 { get; set; }

        public float High2 { get; set; }

        public float Low2 { get; set; }

        public float Close2 { get; set; }

        public float Volume2 { get; set; }

        public float Open3 { get; set; }

        public float High3 { get; set; }

        public float Low3 { get; set; }

        public float Close3 { get; set; }

        public float Volume3 { get; set; }

        public float Body1 => Math.Abs(Close1 - Open1);

        public float Body2 => Math.Abs(Close2 - Open2);

        public float Body3 => Math.Abs(Close3 - Open3);

        public float UpperShadow1 => High1 - Math.Max(Open1, Close1);

        public float UpperShadow2 => High2 - Math.Max(Open2, Close2);

        public float UpperShadow3 => High3 - Math.Max(Open3, Close3);

        public float LowerShadow1 => Math.Min(Open1, Close1) - Low1;

        public float LowerShadow2 => Math.Min(Open2, Close2) - Low2;

        public float LowerShadow3 => Math.Min(Open3, Close3) - Low3;

        public float BodyRatio12 => Body1 / Body2;

        public float BodyRatio23 => Body2 / Body3;

        public float TrendAngle => (Close3 - Open1) / Open1;

        [ColumnName("Label")]
        public bool IsBearishAdvanceBlock { get; set; }
    }
}
