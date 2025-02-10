using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyTracker.Business.Models
{
    public class FourCandlePatternData : ThreeCandlePatternData
    {
        public float Open4 { get; set; }

        public float High4 { get; set; }

        public float Low4 { get; set; }

        public float Close4 { get; set; }

        public float Volume4 { get; set; }

        public float Body4 => Math.Abs(Close4 - Open4);

        public float UpperShadow4 => High4 - Math.Max(Open4, Close4);

        public float LowerShadow4 => Math.Min(Open4, Close4) - Low4;

        public float BodyRatio34 => Body3 / Body4;
    }
}
