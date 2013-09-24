using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using FinTA.Models;
using FinTA.Overlays;
using Logger;

namespace FinTA.Indicators
{
    public class Trix
    {
        private readonly List<MarketData> marketdata;
        public readonly DataTable Data = new DataTable();
        private readonly int period;
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();
       
        public Trix(List<MarketData> marketdata, int period)
        {
            this.marketdata = marketdata;
            this.period = period;
         
        }

        public List<IndicatorsData> Calculate(string mode)
        {
            List<double> closedPrice = new List<double>();
            List<DateTime> dates = new List<DateTime>(); 

            foreach (MarketData mdata in marketdata)
            {
                closedPrice.Add(mdata.ClosePrice);
                dates.Add(mdata.Date);
            }

            SimpleMovingAverage sma = new SimpleMovingAverage();
            ExponentialMovingAverage ema = new ExponentialMovingAverage();
            List<double> singleSmoothedEma = ema.CalculateList(closedPrice, sma.Calculate(closedPrice, period), 2/((double) period + 1),period);
            List<double> doubleSmoothedEma = ema.CalculateList(singleSmoothedEma, sma.Calculate(singleSmoothedEma, period, period * 2 - 1), 2 / ((double)period + 1), period * 2 - 1);
            List<double> tripleSmoothedEma = ema.CalculateList(doubleSmoothedEma, sma.Calculate(doubleSmoothedEma, period, period * 3 - 2), 2 / ((double)period + 1), period * 3 - 2);

            double[] trix = new double[marketdata.Count];

            for (int i = mode.Equals("0") ? 0 : marketdata.Count - 1 ; i < marketdata.Count; i++)
            {
                trix[i] = (i == 0 || tripleSmoothedEma[i - 1]==0) ? 0 : (tripleSmoothedEma[i] / tripleSmoothedEma[i - 1] - 1) * 100;

                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "Trix",
                    Value = trix[i]
                });

                //FileLogWriter looger = new FileLogWriter();
                //looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2},{3}", singleSmoothedEma[i],
                //              doubleSmoothedEma[i],
                //              tripleSmoothedEma[i],
                //              trix[i]), "FinTA");

            }

             return resultData;
        }
        
    }
}
