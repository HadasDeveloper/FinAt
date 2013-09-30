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
        private readonly int daysToGoBack;
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();

        public AccumulationDistributionLine(List<MarketData> marketdata, int daysToGoBack)
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
                    for (int i = marketdata.Count - daysToGoBack; i < marketdata.Count; i++)
                    {
                        dates.Add(marketdata[i].Date);
                        lowPrice.Add(marketdata[i].LowPrice);
                        highPrice.Add(marketdata[i].LowPrice);
                        closedPrice.Add(marketdata[i].ClosePrice);
                        volume.Add(marketdata[i].Volume);
                    }
                    break;
            }

            double[] moneyFolowMultiplier = new double[marketdata.Count];
            double[] moneyFolowVolum = new double[marketdata.Count];
            double[] adl = new double[marketdata.Count];

            for (int i = mode.Equals("0") ? 0 : dates.Count - 1 ; i < dates.Count; i++)
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
