using System;
using System.Collections.Generic;
using System.Data;
using FinTA.Helper;
using FinTA.Models;
using Logger;

namespace FinTA.Overlays
{
    class PriceChannels
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

            foreach (MarketData mdata in marketdata)
            {
                highPrice.Add(mdata.HighPrice);
                lowPrice.Add(mdata.LowPrice);
                dates.Add(mdata.Date);
            }

            double[] upperLine = new double[marketdata.Count];
            double[] lowerLine = new double[marketdata.Count];
            double[] centerLine = new double[marketdata.Count];

            MathHelper mhalper = new MathHelper();

            for (int i = mode.Equals("0") ? 0 : marketdata.Count - 1 ; i < marketdata.Count; i++)
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
