﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FinTA.Models;
using Logger;

namespace FinTA.Indicators
{
    public class RelativeStrengthIndex
    {
        private readonly List<MarketData> marketdata;
        private readonly int daysToGoBack;
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();

        public readonly DataTable Data = new DataTable();

        public RelativeStrengthIndex(List<MarketData> marketdata, int daysToGoBack)
        {
            this.marketdata = marketdata;
            this.daysToGoBack = daysToGoBack;
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
                    for (int i = marketdata.Count - daysToGoBack*2 ; i < marketdata.Count; i++)
                    {
                        dates.Add(marketdata[i].Date);
                        closedPrice.Add(marketdata[i].ClosePrice);
                    }
                    break;
            }

            double[] change = new double[dates.Count];
            List<double> loss = new List<double>();
            List<double> gain = new List<double>();
            double[] avgGain = new double[dates.Count];
            double[] avgLoss = new double[dates.Count];
            double[] rs = new double[dates.Count];
            double[] rsi = new double[dates.Count];

            for (int i = 0  ; i < dates.Count; i++)
            {
                change[i] = i == 0 ? 0 : closedPrice[i] - closedPrice[i - 1];
                gain.Add(change[i] > 0 ? change[i] : 0);
                loss.Add(change[i] > 0 ? 0 : -change[i]);
            }
   
            for (int i = 0; i < dates.Count; i++)
            {
                
                avgGain[i] = i <= daysToGoBack - 1
                                 ? 0
                                 : (i == daysToGoBack
                                        ? gain.GetRange(i - daysToGoBack + 1, daysToGoBack).Average()
                                        : (avgGain[i - 1]*(daysToGoBack - 1) + gain[i])/daysToGoBack);
                
                avgLoss[i] = i <= daysToGoBack - 1
                                 ? 0
                                 : (i == daysToGoBack
                                        ? loss.GetRange(i - daysToGoBack + 1, daysToGoBack).Average()
                                        : (avgLoss[i - 1] * (daysToGoBack - 1) + loss[i]) / daysToGoBack);
                
                rs[i] = avgLoss[i]==0 ? 0 : avgGain[i]/avgLoss[i];
                rsi[i] =  i < daysToGoBack - 1 ? 0 : avgLoss[i] == 0 ? 100 : 100 - (100 / (1 + rs[i]));

                if (mode.Equals("0") || (mode.Equals("1") && i == dates.Count - 1))
                    resultData.Add(new IndicatorsData
                    {
                        Instrument = marketdata[i].Instrument,
                        Date = dates[i],
                        Indicatore = "RelativeStrengthIndex",
                        Value = rsi[i]
                    });

                //FileLogWriter looger = new FileLogWriter();
                //looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2},{3},{4},{5},{6}", change[i],
                //              gain[i],
                //              loss[i],
                //              avgGain[i],
                //              avgLoss[i],
                //              rs[i],
                //              rsi[i]),
                //              "FinTA"
                    //);
            }

             return resultData;
        }
    }
}
