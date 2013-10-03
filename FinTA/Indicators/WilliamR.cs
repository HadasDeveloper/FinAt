using System;
using System.Collections.Generic;
using System.Data;
using FinTA.Helper;
using FinTA.Models;
using Logger;

namespace FinTA.Indicators
{
    public class WilliamR
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
            List<double> closedPrice = new List<double>();
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
                        closedPrice.Add(mdata.ClosePrice);
                    }
                    break;
                case "1":
                    for (int i = marketdata.Count - daysToGoBack; i < marketdata.Count; i++)
                    {
                        dates.Add(marketdata[i].Date);
                        lowPrice.Add(marketdata[i].LowPrice);
                        highPrice.Add(marketdata[i].HighPrice);
                        closedPrice.Add(marketdata[i].ClosePrice);
                    }
                    break;
            }


            MathHelper mathHalper = new MathHelper();

            for (int i = mode.Equals("0") ? 0 : dates.Count - 1; i < dates.Count; i++)
            {
                double highestHigh = i < daysToGoBack - 1 ? 0 : mathHalper.FindMax(highPrice.GetRange(i - daysToGoBack + 1, daysToGoBack));
                double lowestLow = i < daysToGoBack - 1 ? 0 : mathHalper.FindMin(lowPrice.GetRange(i - daysToGoBack + 1, daysToGoBack));
                double williamR = i < daysToGoBack - 1 ? 0 : (highestHigh - lowestLow) == 0 ? 0 : ((highestHigh - closedPrice[i]) / (highestHigh - lowestLow) * (-100));

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
