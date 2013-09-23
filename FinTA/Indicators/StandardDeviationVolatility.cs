﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FinTA.Models;
using FinTA.Overlays;
using Logger;

namespace FinTA.Indicators
{
    class StandardDeviationVolatility
    {
        
        private readonly List<MarketData> marketdata;
        private readonly int daysToGoBack; 
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();
        public readonly DataTable Data = new DataTable();

        public StandardDeviationVolatility(List<MarketData> marketdata, int daysToGoBack)
        {
            this.marketdata = marketdata;
            this.daysToGoBack = daysToGoBack;
        }

        public List<IndicatorsData> Calculate(string mode)
        {
           
            List<double> closedPrice = marketdata.Select(mdata => mdata.ClosePrice).ToList();
            List<DateTime> dates = marketdata.Select(mdata => mdata.Date).ToList();
            double[] periodAverage = new double[marketdata.Count];
            double[] deviation = new double[marketdata.Count];
            List<double> deviationSquared = new List<double>();
           
            int startSmaIndex = 0;
            for (int i = 0; i < marketdata.Count - 10; i++)
            {
                periodAverage[i] = closedPrice.GetRange(startSmaIndex, daysToGoBack).Average();
                deviation[i] = closedPrice[i] - periodAverage[i];
                deviationSquared.Add(deviation[i]*deviation[i]);

                if((i  + 1) % daysToGoBack == 0)
                    startSmaIndex = startSmaIndex + daysToGoBack;
            }

            SimpleMovingAverage sma = new SimpleMovingAverage();
            double[] periodAverageOfDeviationSquared = sma.Calculate(deviationSquared, daysToGoBack);

            double[] standardDeviation = new double[marketdata.Count];

            for (int i = mode.Equals("0") ? 0 : marketdata.Count - 1 ; i < marketdata.Count - 10; i++)
            {
                standardDeviation[i] = (double)Math.Sqrt((double)periodAverageOfDeviationSquared[i]);

                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "StandardDeviationVolatility",
                    Value = standardDeviation[i]
                });

                //FileLogWriter looger = new FileLogWriter();
                //looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2},{3},{4}",
                //                periodAverage[i],
                //                deviation[i],
                //                deviationSquared[i],
                //                periodAverageOfDeviationSquared[i],
                //                standardDeviation[i]),
                //              "FinTA");

              }

                 return resultData;
           }          
    }
}

