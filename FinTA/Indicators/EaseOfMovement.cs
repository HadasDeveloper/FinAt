﻿using System;
using System.Collections.Generic;
using System.Data;
using FinTA.Models;
using FinTA.Overlays;
using Logger;

namespace FinTA.Indicators
{
    public class EaseOfMovement
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

            
            switch (mode)
            {

                case "0":
                    foreach (MarketData mdata in marketdata)
                    {
                        dates.Add(mdata.Date);
                        lowPrice.Add(mdata.LowPrice);
                        highPrice.Add(mdata.HighPrice);
                        volume.Add(mdata.Volume);
                    }
                    break;
                case "1":
                    for (int i = marketdata.Count - daysToGoBack; i < marketdata.Count; i++)
                    {
                        dates.Add(marketdata[i].Date);
                        lowPrice.Add(marketdata[i].LowPrice);
                        highPrice.Add(marketdata[i].HighPrice);
                        volume.Add(marketdata[i].Volume);
                    }
                    break;
            }


            double[] distanceMoved = new double[dates.Count];
            double[] boxRatio = new double[dates.Count];

            for(int i=0 ; i< dates.Count ; i++)
            {
                distanceMoved[i] = i == 0 ? 0 : (highPrice[i] + lowPrice[i])/2 - (highPrice[i - 1] + lowPrice[i - 1])/2;
                boxRatio[i] = (highPrice[i] - lowPrice[i])==0 ? 0 : (volume[i] / 100000000) / (highPrice[i] - lowPrice[i]);
                emv.Add(boxRatio[i]==0? 0 : distanceMoved[i]/boxRatio[i]); 
            }

            SimpleMovingAverage sma = new SimpleMovingAverage();
            double[] smaEmv = sma.Calculate(emv, daysToGoBack);

            for (int i = mode.Equals("0") ? 0 : dates.Count - 1 ; i < dates.Count; i++)
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
