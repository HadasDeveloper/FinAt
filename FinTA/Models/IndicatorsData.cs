using System;

namespace FinTA.Models
{
    public class IndicatorsData
    {
        public string Instrument { get; set; }
        public DateTime Date { get; set; }
        public string Indicatore { get; set; }
        public double Value { get; set; }
    }
}
