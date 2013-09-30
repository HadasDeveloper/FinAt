using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FinTA.Helper;
using FinTA.Models;
using Logger;

namespace FinTA.Indicators
{
    public class StochRSI
    {
        private readonly List<MarketData> marketdata;
        private readonly int daysToGoBack;
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();

        public readonly DataTable Data = new DataTable();

        public StochRSI(List<MarketData> marketdata, int daysToGoBack)
        {
            this.marketdata = marketdata;
            this.daysToGoBack = daysToGoBack;
        }

        public List<IndicatorsData> Calculate(string mode)
        {
            RelativeStrengthIndex rsiData = new RelativeStrengthIndex(marketdata , 14);
            List<IndicatorsData> rsiTable = rsiData.Calculate("0");

            List<DateTime> dates = new List<DateTime>();

            switch (mode)
            {
                case "0":
                    foreach (MarketData mdata in marketdata)
                    {
                        dates.Add(mdata.Date);
                    }
                    break;
                case "1":
                    for (int i = marketdata.Count - daysToGoBack; i < marketdata.Count; i++)
                    {
                        dates.Add(marketdata[i].Date);
                    }
                    break;
            }


            List<double> rsi = rsiTable.Select(data => data.Value).ToList();

            double[] highestHigh = new double[marketdata.Count];
            double[] lowestLow = new double[marketdata.Count];
            double[] stochasticRsi = new double[marketdata.Count];
             
            MathHelper mhelper = new MathHelper();

            for (int i = mode.Equals("0") ? 0 : dates.Count - 1 ; i < dates.Count; i++)
            {
                highestHigh[i] = i < daysToGoBack - 1
                                     ? 0
                                     : mhelper.FindMax(rsi.GetRange(i - daysToGoBack + 1, daysToGoBack));
                lowestLow[i] = i < daysToGoBack - 1
                                     ? 0
                                     : mhelper.FindMin(rsi.GetRange(i - daysToGoBack + 1, daysToGoBack));
                stochasticRsi[i] = i < daysToGoBack - 1
                                              ? 0
                                              : (highestHigh[i] - lowestLow[i])==0? 0 : (rsi[i] - lowestLow[i]) / (highestHigh[i] - lowestLow[i]);

                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "StochRSI",
                    Value = stochasticRsi[i]
                });

                //FileLogWriter looger = new FileLogWriter();
                //looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2},{3}", rsi[i], highestHigh[i],
                //              lowestLow[i],
                //              stochasticRsi[i]),
                //              "FinTA"
                //    );
            }

             return resultData;
        }
    }
}
