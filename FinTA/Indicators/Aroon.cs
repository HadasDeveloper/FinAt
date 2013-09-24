using System;
using System.Collections.Generic;
using System.Data;
using FinTA.Helper;
using FinTA.Models;
using Logger;

namespace FinTA.Indicators
{
    public class Aroon
    {

        private readonly List<MarketData> marketdata;
        private readonly int period;
        public readonly DataTable Data = new DataTable();
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();

        public Aroon(List<MarketData> marketdata, int period)
        {
            this.marketdata = marketdata;
            this.period = period;
        }

        public List<IndicatorsData> Calculate(string mode)
        {
            List<double> highPrice = new List<double>();
            List<double> lowPrice = new List<double>();
            List<DateTime> dates = new List<DateTime>();  

            foreach (MarketData mdata in marketdata)
            {
                highPrice.Add(mdata.HighPrice);
                lowPrice.Add(mdata.LowPrice);
                dates.Add(mdata.Date);                
            }

            double[] periodHigh = new double[marketdata.Count];
            double[] periodLow = new double[marketdata.Count];
            double[] highIndex = new double[marketdata.Count];
            double[] lowIndex = new double[marketdata.Count];
            double[] aroomUp = new double[marketdata.Count];
            double[] aroonDown = new double[marketdata.Count];

            MathHelper mhelper = new MathHelper();

            for (int i = mode.Equals("0") ? 0 : marketdata.Count - 1 ; i < marketdata.Count; i++)
            {
                if( i >=  period - 1)
                {
                    double[] max = mhelper.GetMaxIndex(highPrice.GetRange(i - period + 1, period));
                    periodHigh[i] = max[0];
                    highIndex[i] = max[1];
                    double[] min = mhelper.GetMinIndex(lowPrice.GetRange(i - period + 1, period));
                    periodLow[i] = min[0];
                    lowIndex[i] = min[1];
                    aroomUp[i] = (period - highIndex[i]) / period * 100;
                    aroonDown[i] = (period - lowIndex[i]) / period * 100;
                }


                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "AroonUP",
                    Value = aroomUp[i]
                });

                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "AroonDown",
                    Value = aroonDown[i]
                });

                //FileLogWriter looger = new FileLogWriter();
                //looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2},{3},{4},{5}",
                //            periodHigh[i],
                //            periodLow[i],
                //            highIndex[i],
                //            lowIndex[i],
                //            aroomUp[i],
                //            aroonDown[i]), "FinTA");
            }

             return resultData;
        }

    }

}

