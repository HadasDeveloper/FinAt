using System;
using System.Collections.Generic;

namespace FinTA.Models
{
    class VolumeByPriceBlock
    {
        public List<DateTime> Date { get; set; }
        public List<double> Price { get; set; }
        public List<double> Volume { get; set; }
        public List<double> PosNegVolue { get; set; }
        public int BlockNum { get; set; }
        public double TotalVbp { get; set; }
        public double NegVbp { get; set; }
        public double PosVbp { get; set; }

        public VolumeByPriceBlock()
        {
            Date = new List<DateTime>();
            Price = new List<double>();
            Volume = new List<double>();
            PosNegVolue = new List<double>();
        }
    }

   
}
