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
        public readonly DataTable Data = new DataTable();
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();

        public VolumeWeightedAveragePrice(List<MarketData> marketdata)
        {
            this.marketdata = marketdata;
        }

        public List<IndicatorsData> Calculate(string mode)
        {
            List<double> closedPrice = new List<double>();
            List<double> highPrice = new List<double>();
            List<double> lowPrice = new List<double>();
            List<double> volume = new List<double>();
            List<DateTime> dates = new List<DateTime>();

            foreach (MarketData mdata in marketdata)
            {
                closedPrice.Add(mdata.ClosePrice);
                highPrice.Add(mdata.HighPrice);
                lowPrice.Add(mdata.LowPrice);
                volume.Add(mdata.Volume);
                dates.Add(mdata.Date);
            }

            double[] typicalPrice = new double[marketdata.Count];
            double[] pv = new double[marketdata.Count];
            double[] totalPv = new double[marketdata.Count];
            double[] totalV = new double[marketdata.Count];
            double[] vwap = new double[marketdata.Count];


            for (int i = mode.Equals("0") ? 0 : marketdata.Count - 1; i < marketdata.Count; i++)
            {
                typicalPrice[i] = ((double)highPrice[i] + (double)lowPrice[i] + (double)closedPrice[i]) / 3;
                pv[i] = (double)typicalPrice[i]*volume[i];
                totalPv[i] = i == 0 ? pv[i] : pv[i] + totalPv[i - 1];
                totalV[i] = i == 0 ? totalV[i] = volume[i] : volume[i] + totalV[i - 1];
                vwap[i] = totalPv[i]/totalV[i];


                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "VolumeWeightedAveragePrice",
                    Value = vwap[i]
                });

            //    FileLogWriter looger = new FileLogWriter();
            //    looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2},{3},{4},{5}", volume[i], typicalPrice[i], pv[i], totalPv[i],totalV[i] ,vwap[i]), "FinTA");
            }

             return resultData;
        }
    }
}
