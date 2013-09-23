using System;

namespace FinTA.Models
{
    public class MarketData
    {
        public string Instrument { get; set; }
        public DateTime Date { get; set; }
        public double OpenPrice { get; set; }
        public double HighPrice { get; set; }
        public double LowPrice { get; set; }
        public double ClosePrice { get; set; }
        public double Volume { get; set; }    
    }
}
