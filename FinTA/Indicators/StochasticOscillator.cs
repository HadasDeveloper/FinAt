using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FinTA.Helper;
using FinTA.Models;
using Logger;

namespace FinTA.Indicators
{
    public class StochasticOscillator
    {
        private readonly List<MarketData> marketdata;
        private readonly int daysToGoBack;
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();
        public readonly DataTable Data = new DataTable();

        public StochasticOscillator(List<MarketData> marketdata, int daysToGoBack)
        {
            this.marketdata = marketdata;
            this.daysToGoBack = daysToGoBack;
        }

        public List<IndicatorsData> Calculate(string mode)
        {
            List<double> closedPrice = new List<double>();
            List<double> highprice = new List<double>();
            List<double> lowprice = new List<double>();
            List<DateTime> dates = new List<DateTime>();

            switch (mode)
            {

                case "0":
                    foreach (MarketData mdata in marketdata)
                    {
                        dates.Add(mdata.Date);
                        lowprice.Add(mdata.LowPrice);
                        highprice.Add(mdata.HighPrice);
                        closedPrice.Add(mdata.ClosePrice);                       
                    }
                    break;
                case "1":
                    for (int i = marketdata.Count - daysToGoBack; i < marketdata.Count; i++)
                    {
                        dates.Add(marketdata[i].Date);
                        lowprice.Add(marketdata[i].LowPrice);
                        highprice.Add(marketdata[i].LowPrice);
                        closedPrice.Add(marketdata[i].ClosePrice);
                    }
                    break;
            }
            
            double[] highestHigh = new double[marketdata.Count];
            double[] lowestLow = new double[marketdata.Count];
            double[] stochasticOscillator = new double[marketdata.Count];

            MathHelper mhelper = new MathHelper();

            for (int i = mode.Equals("0") ? 0 : dates.Count - 1 ; i < dates.Count; i++)
            {
                highestHigh[i] = i < daysToGoBack - 1
                                     ? 0
                                     : mhelper.FindMax(highprice.GetRange(i - daysToGoBack + 1, daysToGoBack));
                lowestLow[i] = i < daysToGoBack - 1
                                     ? 0
                                     : mhelper.FindMin(lowprice.GetRange(i - daysToGoBack + 1, daysToGoBack));
                stochasticOscillator[i] = i < daysToGoBack - 1
                                              ? 0
                                              : (highestHigh[i] - lowestLow[i])==0 ? 0 : (closedPrice[i] - lowestLow[i])/(highestHigh[i] - lowestLow[i])*100;

                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "StochasticOscillator",
                    Value = stochasticOscillator[i]
                });

                //FileLogWriter looger = new FileLogWriter();
                //looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2}", highestHigh[i],
                //              lowestLow[i],
                //              stochasticOscillator[i]),
                //              "FinTA"
                //    );
            }

             return resultData;
        }
    }
}
