using System;
using System.Collections.Generic;
using System.Data;
using FinTA.Models;
using FinTA.Overlays;
using Logger;

namespace FinTA.Indicators
{
    class EaseOfMovement
    {
        private readonly List<MarketData> marketdata;
        private readonly int daysToGoBack;
        public readonly DataTable Data = new DataTable();
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();


        public EaseOfMovement(List<MarketData> marketdata, int daysToGoBack)
        {
            this.marketdata = marketdata;
            this.daysToGoBack = daysToGoBack;   
        }

        public List<IndicatorsData> Calculate(string mode)
        {
            List<double> volume = new List<double>();
            List<double> highPrice = new List<double>();
            List<double> lowPrice = new List<double>();
            List<double> emv = new List<double>();
            List<DateTime> dates = new List<DateTime>();    

            foreach (MarketData mdata in marketdata)
            {
                volume.Add(mdata.Volume);
                highPrice.Add(mdata.HighPrice);
                lowPrice.Add(mdata.LowPrice);
                dates.Add(mdata.Date); 
            }


            double[] distanceMoved = new double[marketdata.Count];
            double[] boxRatio = new double[marketdata.Count];

            for(int i=0 ; i< marketdata.Count ; i++)
            {
                distanceMoved[i] = i == 0 ? 0 : (highPrice[i] + lowPrice[i])/2 - (highPrice[i - 1] + lowPrice[i - 1])/2;
                boxRatio[i] = (highPrice[i] - lowPrice[i])==0 ? 0 : (volume[i] / 100000000) / (highPrice[i] - lowPrice[i]);
                emv.Add(boxRatio[i]==0? 0 : distanceMoved[i]/boxRatio[i]); 
            }

            SimpleMovingAverage sma = new SimpleMovingAverage();
            double[] smaEmv = sma.Calculate(emv, daysToGoBack);

            for (int i = mode.Equals("0") ? 0 : marketdata.Count - 1 ; i < marketdata.Count; i++)
            {
                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "EaseOfMovement",
                    Value = smaEmv[i]
                });

                //FileLogWriter looger = new FileLogWriter();
                //looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2}", daysToGoBack,emv[i],smaEmv[i]),"FinTA");

            }

             return resultData;
        }
    }
}
