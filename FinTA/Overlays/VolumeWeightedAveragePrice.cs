using System;
using System.Collections.Generic;
using System.Data;
using FinTA.Models;
using Logger;

namespace FinTA.Overlays
{
    public class VolumeWeightedAveragePrice
    {
        private readonly List<MarketData> marketdata;
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();
        private readonly int daysToGoBack;
        public readonly DataTable Data = new DataTable();
        

        public VolumeWeightedAveragePrice(List<MarketData> marketdata, int daysToGoBack)
        {
            this.marketdata = marketdata;
            this.daysToGoBack = daysToGoBack;
        }

        public List<IndicatorsData> Calculate(string mode)
        {
            List<double> closedPrice = new List<double>();
            List<double> highPrice = new List<double>();
            List<double> lowPrice = new List<double>();
            List<double> volume = new List<double>();
            List<DateTime> dates = new List<DateTime>();
      
            switch (mode)
            {

                case "0":
                    foreach (MarketData mdata in marketdata)
                    {
                        dates.Add(mdata.Date);
                        lowPrice.Add(mdata.LowPrice);
                        highPrice.Add(mdata.HighPrice);
                        closedPrice.Add(mdata.ClosePrice);
                        volume.Add(mdata.Volume);
                    }
                    break;
                case "1":
                    for (int i = marketdata.Count - daysToGoBack; i < marketdata.Count ; i++)
                    {
                        dates.Add(marketdata[i].Date);
                        lowPrice.Add(marketdata[i].LowPrice);
                        highPrice.Add(marketdata[i].HighPrice);
                        closedPrice.Add(marketdata[i].ClosePrice);
                        volume.Add(marketdata[i].Volume);
                    }
                    break;
            }

            List<double> pv = new List<double>();
            List<double> totalPv = new List<double>();
            double[] totalV = new double[dates.Count];


            for (int i = 0; i < dates.Count; i++)
            {
                double typicalPrice = (highPrice[i] + lowPrice[i] + closedPrice[i]) / 3;
                pv.Add(typicalPrice*volume[i]);
                totalPv.Add( i == 0 ? pv[i] : pv[i] + totalPv[i - 1]);
                totalV[i] = i == 0 ? totalV[i] = volume[i] : volume[i] + totalV[i - 1];
                double vwap = totalPv[i]/totalV[i];

                if(mode.Equals("0")||(mode.Equals("1")&& i==dates.Count-1))
                    resultData.Add(new IndicatorsData
                    {
                        Instrument = marketdata[i].Instrument,
                        Date = dates[i],
                        Indicatore = "VolumeWeightedAveragePrice",
                        Value = vwap
                    });

            //    FileLogWriter looger = new FileLogWriter();
            //    looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2},{3},{4},{5}", volume[i], typicalPrice[i], pv[i], totalPv[i],totalV[i] ,vwap[i]), "FinTA");
            }

             return resultData;
        }
    }
}
