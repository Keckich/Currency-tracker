﻿using Microsoft.ML.Data;

namespace CurrencyTracker.Business.Models
{
    public class ThreeWhiteSoldiersInput
    {
        public float Body1 { get; set; }

        public float Body2 { get; set; }

        public float Body3 { get; set; }

        [ColumnName("Label")]
        public bool IsThreeWhiteSoldiers { get; set; }
    }
}
