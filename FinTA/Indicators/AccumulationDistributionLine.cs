using System;
using System.Collections.Generic;
using System.Data;
using FinTA.Models;
using Logger;

namespace FinTA.Indicators
{
    public class AccumulationDistributionLine
    {
        private readonly List<MarketData> marketdata;
        public readonly DataTable Data = new DataTable();
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();

        public AccumulationDistributionLine(List<MarketData> marketdata)
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

            double[] moneyFolowMultiplier = new double[marketdata.Count];
            double[] moneyFolowVolum = new double[marketdata.Count];
            double[] adl = new double[marketdata.Count];

            for (int i = mode.Equals("0") ? 0 : marketdata.Count - 1 ; i < marketdata.Count; i++)
            {
                moneyFolowMultiplier[i] = (highPrice[i] - lowPrice[i])==0 ? 0 : ((closedPrice[i] - lowPrice[i]) - (highPrice[i] - closedPrice[i])) / (highPrice[i] - lowPrice[i]);
                moneyFolowVolum[i] = moneyFolowMultiplier[i] * (double)volume[i];
                adl[i] = i == 0 ? adl[i] = moneyFolowVolum[i] : moneyFolowVolum[i] + adl[i - 1];

                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "AccumulationDistributionLine",
                    Value = adl[i]
                });

                //FileLogWriter looger = new FileLogWriter();
                //looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2},{3}", volume[i], moneyFolowMultiplier[i], moneyFolowVolum[i], adl[i]), "FinTA");
            }

             return resultData;
        }
 
    }
}
