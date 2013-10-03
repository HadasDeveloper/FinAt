using System;
using System.Collections.Generic;
using System.Data;
using FinTA.Helper;
using FinTA.Models;
using Logger;

namespace FinTA.Overlays
{
    public class PriceChannels
    {
        private readonly List<MarketData> marketdata;
        private readonly int daysToGoBack;
        public readonly DataTable Data = new DataTable();
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();

        public PriceChannels(List<MarketData> marketdata, int daysToGoBack)
        {
            this.marketdata = marketdata;
            this.daysToGoBack = daysToGoBack;
        }

        public List<IndicatorsData> Calculate(string mode)
        {
            List<double> highPrice = new List<double>();
            List<double> lowPrice = new List<double>();
            List<DateTime> dates = new List<DateTime>();

            switch (mode)
            {
                case "0":
                    foreach (MarketData mdata in marketdata)
                    {
                        dates.Add(mdata.Date);
                        lowPrice.Add(mdata.LowPrice);
                        highPrice.Add(mdata.HighPrice);                     
                    }
                    break;

                case "1":
                    for (int i = marketdata.Count - daysToGoBack ; i < marketdata.Count ; i++)
                    {
                        dates.Add(marketdata[i].Date);
                        lowPrice.Add(marketdata[i].LowPrice);
                        highPrice.Add(marketdata[i].HighPrice);                    
                    }
                    break;
            }

            double[] upperLine = new double[dates.Count];
            double[] lowerLine = new double[dates.Count];
            double[] centerLine = new double[dates.Count];

            MathHelper mhalper = new MathHelper();

            for (int i = mode.Equals("0") ? 0 : dates.Count - 1 ; i < dates.Count; i++)
            {
                upperLine[i] = i < daysToGoBack - 1
                                   ? 0
                                   : mhalper.FindMax(highPrice.GetRange(i - daysToGoBack + 1, daysToGoBack));
                lowerLine[i] = i < daysToGoBack - 1
                                   ? 0
                                   : mhalper.FindMin(lowPrice.GetRange(i - daysToGoBack + 1, daysToGoBack));

                centerLine[i] = i < daysToGoBack - 1 ? 0 : (upperLine[i] + lowerLine[i])/2;

                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "PriceChannels",
                    Value = centerLine[i]
                });

                //FileLogWriter looger = new FileLogWriter();
                //looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2}", upperLine[i],
                //                                              lowerLine[i],
                //                                              centerLine[i]), "FinTA");

            }

            return resultData;
        }
    }
}
