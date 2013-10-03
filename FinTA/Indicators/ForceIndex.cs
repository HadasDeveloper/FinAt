using System;
using System.Collections.Generic;
using System.Data;
using FinTA.Models;
using FinTA.Overlays;
using Logger;

namespace FinTA.Indicators
{
    public class ForceIndex
    {
        private readonly List<MarketData> marketdata;
        private readonly int daysToGoBack;
        public readonly DataTable Data = new DataTable();
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();


        public ForceIndex(List<MarketData> marketdata, int daysToGoBack)
        {
            this.marketdata = marketdata;
            this.daysToGoBack = daysToGoBack;
        }

        public List<IndicatorsData> Calculate(string mode)
        {
            List<double> closedPrice = new List<double>();
            List<double> volume = new List<double>();
            List<DateTime> dates = new List<DateTime>();

            switch (mode)
            {

                case "0":
                    foreach (MarketData mdata in marketdata)
                    {
                        dates.Add(mdata.Date);
                        volume.Add(mdata.Volume);                        
                        closedPrice.Add(mdata.ClosePrice);
                    }
                    break;
                case "1":
                    for (int i = marketdata.Count - daysToGoBack - 1; i < marketdata.Count; i++)
                    {
                        dates.Add(marketdata[i].Date);
                        volume.Add(marketdata[i].Volume);
                        closedPrice.Add(marketdata[i].ClosePrice);
                    }
                    break;
            }


            double[] upDown = new double[dates.Count];
            double[] extent = new double[dates.Count];
            List<double> forceIndex = new List<double>();

            for(int i=0 ; i< dates.Count ; i++)
            {
                upDown[i] = i == 0 ? 0 : (closedPrice[i] - closedPrice[i - 1]) > 0 ? 1 : -1; 
                extent[i] =  i == 0 ? 0 : closedPrice[i] - closedPrice[i - 1];
                forceIndex.Add(extent[i]*volume[i]);
            }

            SimpleMovingAverage sma = new SimpleMovingAverage();
            double[] forceIndexSma = sma.Calculate(forceIndex, daysToGoBack);

            ExponentialMovingAverage ema = new ExponentialMovingAverage();
            double[] periodForceIndex = ema.Calculate(forceIndex, forceIndexSma, 2 / ((double)daysToGoBack + 1), daysToGoBack);


            for (int i = mode.Equals("0") ? 0 : dates.Count - 1 ; i < dates.Count; i++)
            {
                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "ForceIndex",
                    Value = periodForceIndex[i]
                });

                //FileLogWriter looger = new FileLogWriter();
                //looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2},{3},{4}",
                //            upDown[i],
                //            extent[i],
                //            forceIndex[i],
                //            forceIndexSma[i],
                //            periodForceIndex[i]), "FinTA");
            }

             return resultData;
        }

    }
}
