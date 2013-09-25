using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FinTA.Helper;
using FinTA.Indicators;
using Logger;

namespace FinTA.Models
{
    class ChandelierExit
    {
        private readonly List<MarketData> marketdata;
        private readonly int daysToGoBack;
        private readonly int multiplier;
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();

        public ChandelierExit(List<MarketData> marketdata, int daysToGoBack, int multiplier)
        {
            this.marketdata = marketdata;
            this.daysToGoBack = daysToGoBack;
            this.multiplier = multiplier;

        }

        public List<IndicatorsData> Calculate(string mode)
        {
            List<double> closedPrice = new List<double>();
            List<double> lowPrice = new List<double>();
            List<double> highPrice = new List<double>();
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
                        highPrice.Add(marketdata[i].LowPrice);
                        closedPrice.Add(marketdata[i].ClosePrice);
                    }
                    break;
            }

            double[] lowestLow = new double[marketdata.Count];
            double[] chandelierExitShort = new double[marketdata.Count];
            double[] highestHigh = new double[marketdata.Count];
            double[] chandelierExitLong = new double[marketdata.Count];

            MathHelper mhalper = new MathHelper();

            double[] atr = new double[marketdata.Count];

            AverageTrueRange averageTrueRange = new AverageTrueRange(marketdata, daysToGoBack);
            List<IndicatorsData> atrData = averageTrueRange.Calculate("0");

            for (int  i = mode.Equals("0") ? 0 : marketdata.Count - 1; i < marketdata.Count; i++)
            {              
                atr[i] = atrData[i].Value;
                
                lowestLow[i] = i < daysToGoBack - 1
                                   ? 0
                                   : mhalper.FindMin(lowPrice.GetRange(i - daysToGoBack + 1, daysToGoBack));

                chandelierExitShort[i] = i < daysToGoBack - 1 ? 0 : lowestLow[i] + atr[i]*multiplier;

                highestHigh[i] = i < daysToGoBack - 1
                                     ? 0
                                     : mhalper.FindMax(highPrice.GetRange(i - daysToGoBack + 1, daysToGoBack));

                chandelierExitLong[i] = i < daysToGoBack - 1 ? 0 : highestHigh[i] - atr[i] * multiplier;

                resultData.Add(new IndicatorsData
                                   {
                                       Instrument = marketdata[i].Instrument,
                                       Date = dates[i],
                                       Indicatore = "ChandelierExitShort",
                                       Value = chandelierExitShort[i]
                                   });

                resultData.Add(new IndicatorsData
                                   {
                                       Instrument = marketdata[i].Instrument,
                                       Date = dates[i],
                                       Indicatore = "ChandelierExitLong",
                                       Value = chandelierExitLong[i]
                                   });

                //FileLogWriter looger = new FileLogWriter();
                //looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2},{3},{4}",
                //                                              lowestLow[i],
                //                                              highestHigh[i],
                //                                              atr[i],
                //                                              chandelierExitShort[i],
                //                                              chandelierExitLong[i]), "FinTA");


            }

            return resultData;

        }
    }
}
