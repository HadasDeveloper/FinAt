using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FinTA.Models;
using Logger;

namespace FinTA.Indicators
{
    class RateOfChange
    {
        private readonly List<MarketData> marketdata;
        private readonly int daysToGoBack;
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();
        public readonly DataTable Data = new DataTable();
       
        public RateOfChange(List<MarketData> marketdata, int daysToGoBack)
        {
            this.marketdata = marketdata;
            this.daysToGoBack = daysToGoBack;    
        }

        public List<IndicatorsData> Calculate(string mode)
        {
            List<double> closedPrice = marketdata.Select(mdata => mdata.ClosePrice).ToList();
            List<DateTime> dates = marketdata.Select(mdata => mdata.Date).ToList();
            double[] roc = new double[marketdata.Count];

            for (int i = mode.Equals("0") ? 0 : marketdata.Count - 1; i < marketdata.Count; i++)
            {
                roc[i] = i < daysToGoBack
                             ? 0
                             : ((closedPrice[i] - closedPrice[i - daysToGoBack])/closedPrice[i - daysToGoBack])*100;

                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "RateOfChange",
                    Value = roc[i]
                });

                //FileLogWriter looger = new FileLogWriter();
                //looger.WriteToLog(DateTime.Now, string.Format("{0}", roc[i]),
                //              "FinTA"
                //    );
            }

             return resultData;
        }
    }
}
