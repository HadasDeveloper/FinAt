using System;
using System.Collections.Generic;
using System.Data;
using FinTA.Models;
using FinTA.Overlays;
using Logger;

namespace FinTA.Indicators
{
    public class KnowSureThing
    {
        private readonly List<MarketData> marketdata;
        public readonly DataTable Data = new DataTable();
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();


        readonly Timeframes frames = new Timeframes().GetShortTermDaily();

        public KnowSureThing(List<MarketData> marketdata)
        {
            this.marketdata = marketdata;
        }

        public List<IndicatorsData> Calculate(string mode)
        {
            List<double> closedPrice = new List<double>();
            List<DateTime> dates = new List<DateTime>();


            switch (mode)
            {

                case "0":
                    foreach (MarketData mdata in marketdata)
                    {
                        dates.Add(mdata.Date);                      
                        closedPrice.Add(mdata.ClosePrice);
                    }
                    break;
                case "1":
                    for (int i = marketdata.Count - frames.Roc4 - frames.Sma4 ; i < marketdata.Count; i++)
                    {
                        dates.Add(marketdata[i].Date);
                        closedPrice.Add(marketdata[i].ClosePrice);
                    }
                    break;
            }


            List<double> roc1 = new List<double>();
            List<double> roc2 = new List<double>();
            List<double> roc3 = new List<double>();
            List<double> roc4 = new List<double>();
           
            for(int i=0 ; i< marketdata.Count ; i++)
            {
                roc1.Add(i < frames.Roc1 ? 0 : (closedPrice[i] - closedPrice[i - frames.Roc1]) / closedPrice[i - frames.Roc1] * 100);
                roc2.Add(i < frames.Roc2 ? 0 : (closedPrice[i] - closedPrice[i - frames.Roc2]) / closedPrice[i - frames.Roc2] * 100);
                roc3.Add(i < frames.Roc3 ? 0 : (closedPrice[i] - closedPrice[i - frames.Roc3]) / closedPrice[i - frames.Roc3] * 100);
                roc4.Add(i < frames.Roc4 ? 0 : (closedPrice[i] - closedPrice[i - frames.Roc4]) / closedPrice[i - frames.Roc4] * 100); 
            }

            SimpleMovingAverage sma = new SimpleMovingAverage();

            double[] smaRoc1 = sma.Calculate(roc1, frames.Sma1, frames.Roc4 + frames.Sma1 - 1);
            double[] smaRoc2 = sma.Calculate(roc2, frames.Sma2, frames.Roc4 + frames.Sma2 - 1);
            double[] smaRoc3 = sma.Calculate(roc3, frames.Sma3, frames.Roc4 + frames.Sma3 - 1);
            double[] smaRoc4 = sma.Calculate(roc4, frames.Sma4, frames.Roc4 + frames.Sma4);

            double[] kst = new double[marketdata.Count];

            for (int i = mode.Equals("0") ? 0 : marketdata.Count - 1 ; i < marketdata.Count; i++)
            {
                int startPoint = frames.Roc4 + frames.Sma4 - 1;

                kst[i] = i < startPoint ? 0 : smaRoc1[i]*4 + smaRoc2[i]*3 + smaRoc3[i]*2 + smaRoc4[i]*1;

                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "KnowSureThing",
                    Value = kst[i]
                });

                //FileLogWriter looger = new FileLogWriter();
                //looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}",
                //            roc1[i],
                //            roc2[i],
                //            roc3[i],
                //            roc4[i],
                //            smaRoc1[i],
                //            smaRoc2[i],
                //            smaRoc3[i],
                //            smaRoc4[i],
                //            kst[i]), "FinTA");
            }
             return resultData;
        }

    }    

}
