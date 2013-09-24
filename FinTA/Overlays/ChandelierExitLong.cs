using System;
using System.Collections.Generic;
using FinTA.Helper;
using FinTA.Indicators;
using FinTA.Models;
using Logger;

namespace FinTA.Overlays
{
    public class ChandelierExitLong
    {
        private readonly List<MarketData> marketdata;
        private readonly int daysToGoBack;
        private readonly int multiplier;
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();

        public ChandelierExitLong(List<MarketData> marketdata, int daysToGoBack, int multiplier)
        {
            this.marketdata = marketdata;
            this.daysToGoBack = daysToGoBack;
            this.multiplier = multiplier;

        }

        public List<IndicatorsData> Calculate(string mode)
        {
            List<double> closedPrice = new List<double>();
            List<double> highPrice = new List<double>();
            List<DateTime> dates = new List<DateTime>();

            foreach (MarketData mdata in marketdata)
            {
                closedPrice.Add(mdata.ClosePrice);
                highPrice.Add(mdata.HighPrice);
                dates.Add(mdata.Date);
            }

            double[] highestHigh = new double[marketdata.Count];
            double[] chandelierExitLong = new double[marketdata.Count];

            MathHelper mhalper = new MathHelper();

            double[] atr = new double[marketdata.Count];

            AverageTrueRange averageTrueRange = new AverageTrueRange(marketdata, daysToGoBack);
            List<IndicatorsData> atrData = averageTrueRange.Calculate("0");

            
            for (int i = mode.Equals("0") ? 0 : marketdata.Count - 1 ; i < marketdata.Count; i++)
            {
                atr[i] = atrData[i].Value;

                highestHigh[i] = i < daysToGoBack - 1
                                     ? 0
                                     : mhalper.FindMax(highPrice.GetRange(i - daysToGoBack + 1, daysToGoBack));
   
                chandelierExitLong[i] = i < daysToGoBack - 1 ? 0 : highestHigh[i] - atr[i] * multiplier;
            
                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "ChandelierExitLong",
                    Value = chandelierExitLong[i]
                });

                //FileLogWriter looger = new FileLogWriter();
                
                //looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2}", highestHigh[i],
                //                                              atr[i],
                //                                              chandelierExitLong[i]), "FinTA");
            
            }

          
            return resultData;

        }
    }
}
