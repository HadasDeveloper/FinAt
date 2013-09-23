using System;
using System.Collections.Generic;
using System.Data;
using FinTA.Models;
using Logger;

namespace FinTA.Overlays
{
    class MovingAverages
    {
        private readonly List<MarketData> marketdata;
        private readonly int daysToGoBack;
        public readonly DataTable Data = new DataTable();
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();

        public MovingAverages(List<MarketData> marketdata, int daysToGoBack)
        {
            this.marketdata = marketdata;
            this.daysToGoBack = daysToGoBack;
        }

        public List<IndicatorsData> Calculate(string mode)
        {
            List<double> closedPrice = new List<double>();
            List<DateTime> dates = new List<DateTime>();    

            for(int i = 0 ; i < marketdata.Count ; i++)
            {
                closedPrice.Add(marketdata[i].ClosePrice);
                dates.Add(marketdata[i].Date);
            }

            double multiplier =  (2 / ((double)daysToGoBack + 1));

            SimpleMovingAverage simpleMovingAverage = new SimpleMovingAverage();
            double[] sma = simpleMovingAverage.Calculate(closedPrice, daysToGoBack);

            ExponentialMovingAverage exponentialMovingAverage = new ExponentialMovingAverage();
            double[] ema = exponentialMovingAverage.Calculate(closedPrice, sma, multiplier, daysToGoBack);
           
            for (int i = mode.Equals("0") ? 0 : marketdata.Count - 1 ; i < marketdata.Count; i++)
            {
                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "SimpleMovingAverage",
                    Value = sma[i]
                });

                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "ExponentialMovingAverage",
                    Value = ema[i]
                });


               //FileLogWriter looger = new FileLogWriter();
               //looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2},{3}",marketdata[i].Date,sma[i],multiplier,ema[i]), "FinTA");
            }
            return resultData;
        }       
    }
}
