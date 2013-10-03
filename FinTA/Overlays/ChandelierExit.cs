using System;
using System.Collections.Generic;
using FinTA.Helper;
using FinTA.Indicators;
using Logger;

namespace FinTA.Models
{
    public class ChandelierExit
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

            AverageTrueRange averageTrueRange = new AverageTrueRange(marketdata, daysToGoBack);
            List<IndicatorsData> atrData = averageTrueRange.Calculate("0");

            List<double> atr = new List<double>();

            switch (mode)
            {
                case "0":
                    for (int i = 0 ; i < marketdata.Count; i++)
                    {
                        dates.Add(marketdata[i].Date);
                        lowPrice.Add(marketdata[i].LowPrice);
                        highPrice.Add(marketdata[i].HighPrice);
                        closedPrice.Add(marketdata[i].ClosePrice);
                        atr.Add(atrData[i].Value);
                    }
                    break;
                case "1":
                    for (int i = marketdata.Count - daysToGoBack-1; i < marketdata.Count; i++)
                    {
                        dates.Add(marketdata[i].Date);
                        lowPrice.Add(marketdata[i].LowPrice);
                        highPrice.Add(marketdata[i].HighPrice);
                        closedPrice.Add(marketdata[i].ClosePrice);
                        atr.Add(atrData[i].Value);
                    }
                    break;
            }

            double[] lowestLow = new double[dates.Count];
            double[] chandelierExitShort = new double[dates.Count];
            double[] highestHigh = new double[dates.Count];
            double[] chandelierExitLong = new double[dates.Count];

            MathHelper mhalper = new MathHelper();

            for (int i = mode.Equals("0") ? 0 : dates.Count - 1; i < dates.Count; i++)
            {              
                
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
