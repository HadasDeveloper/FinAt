using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FinTA.Helper;
using FinTA.Models;
using Logger;

namespace FinTA.Indicators
{
    class WilliamR
    {
        private readonly List<MarketData> marketdata;
        private readonly int daysToGoBack;
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();

        public readonly DataTable Data = new DataTable();

        public WilliamR(List<MarketData> marketdata, int daysToGoBack)
        {
            this.marketdata = marketdata;
            this.daysToGoBack = daysToGoBack;

        }

        public List<IndicatorsData> Calculate(string mode)
        {
            List<double> closedPrice = marketdata.Select(mdata => mdata.ClosePrice).ToList();
            List<double> highPrice = marketdata.Select(mdata => mdata.HighPrice).ToList();
            List<double> lowPrice = marketdata.Select(mdata => mdata.LowPrice).ToList();
            List<DateTime> dates = marketdata.Select(mdata => mdata.Date).ToList();

            double highestHigh;
            double lowestLow;
            double williamR;

            MathHelper mathHalper = new MathHelper();

            for (int i = mode.Equals("0") ? 0 : marketdata.Count - 1; i < marketdata.Count; i++)
            {
                highestHigh = i < daysToGoBack - 1 ? 0 : mathHalper.FindMax(highPrice.GetRange(i - daysToGoBack + 1, daysToGoBack));
                lowestLow = i < daysToGoBack - 1 ? 0 : mathHalper.FindMin(lowPrice.GetRange(i - daysToGoBack + 1, daysToGoBack));
                williamR = i < daysToGoBack - 1 ? 0 : (highestHigh - lowestLow) == 0 ? 0 : ((highestHigh - closedPrice[i]) / (highestHigh - lowestLow) * (-100));

                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "WilliamR",
                    Value = williamR
                });

                //FileLogWriter looger = new FileLogWriter();
                //looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2}", highestHigh,
                //              lowestLow,
                //              williamR),
                //              "FinTA"
                //    );
            }

             return resultData;
        }

    }
}
