﻿using System;
using System.Collections.Generic;
using System.Data;
using FinTA.Models;
using FinTA.Overlays;
using Logger;

namespace FinTA.Indicators
{
    class ForceIndex
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

            foreach (MarketData mdata in marketdata)
            {
                closedPrice.Add(mdata.ClosePrice);
                volume.Add(mdata.Volume);
                dates.Add(mdata.Date);
            }

            double[] upDown = new double[marketdata.Count];
            double[] extent = new double[marketdata.Count];
            List<double> forceIndex = new List<double>();

            for(int i=0 ; i< marketdata.Count ; i++)
            {
                upDown[i] = i == 0 ? 0 : (closedPrice[i] - closedPrice[i - 1]) > 0 ? 1 : -1; 
                extent[i] =  i == 0 ? 0 : closedPrice[i] - closedPrice[i - 1];
                forceIndex.Add(extent[i]*(double)volume[i]);
            }

            SimpleMovingAverage sma = new SimpleMovingAverage();
            double[] forceIndexSma = sma.Calculate(forceIndex, daysToGoBack);

            ExponentialMovingAverage ema = new ExponentialMovingAverage();
            double[] periodForceIndex = ema.Calculate(forceIndex, forceIndexSma, 2 / ((double)daysToGoBack + 1), daysToGoBack);


            for (int i = mode.Equals("0") ? 0 : marketdata.Count - 1 ; i < marketdata.Count; i++)
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