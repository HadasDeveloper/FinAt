using System;
using System.Collections.Generic;
using FinTA.Helper;
using FinTA.Indicators;
using FinTA.Models;
using Logger;

namespace FinTA.Overlays
{
    public class ChandelierExitShort
    {
        private readonly List<MarketData> marketdata;
        private readonly int daysToGoBack;
        private readonly int multiplier;
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();

        public ChandelierExitShort(List<MarketData> marketdata, int daysToGoBack, int multiplier)
        {
            this.marketdata = marketdata;
            this.daysToGoBack = daysToGoBack;
            this.multiplier = multiplier;

        }

        public List<IndicatorsData> Calculate(string mode)
        {
            List<double> closedPrice = new List<double>();
            List<double> lowPrice = new List<double>();
            List<DateTime> dates = new List<DateTime>();

            foreach (MarketData mdata in marketdata)
            {
                closedPrice.Add(mdata.ClosePrice);
                lowPrice.Add(mdata.LowPrice);
                dates.Add(mdata.Date);
            }

            double[] lowestLow = new double[marketdata.Count];
            double[] chandelierExitShort = new double[marketdata.Count];

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

                resultData.Add(new IndicatorsData
                                   {
                                       Instrument = marketdata[i].Instrument,
                                       Date = dates[i],
                                       Indicatore = "ChandelierExitShort",
                                       Value = chandelierExitShort[i]
                                   });

                //FileLogWriter looger = new FileLogWriter();
                //looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2}",
                //                                              lowestLow[i],
                //                                              atr[i],
                //                                              chandelierExitShort[i]), "FinTA");


            }

            return resultData;

        }
    }
}
