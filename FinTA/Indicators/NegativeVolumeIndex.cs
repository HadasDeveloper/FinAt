﻿using System;
using System.Collections.Generic;
using System.Data;
using FinTA.Models;
using Logger;

namespace FinTA.Indicators
{
    public class NegativeVolumeIndex
    {
        private readonly List<MarketData> marketdata;
        public readonly DataTable Data = new DataTable();
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();


        public NegativeVolumeIndex(List<MarketData> marketdata)
        {
            this.marketdata = marketdata;            
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
                        closedPrice.Add(mdata.ClosePrice);
                        volume.Add(mdata.Volume);
                    }
                    break;
                case "1":
                    for (int i = marketdata.Count - 2; i < marketdata.Count; i++)
                    {
                        dates.Add(marketdata[i].Date);
                        closedPrice.Add(marketdata[i].ClosePrice); 
                        volume.Add(marketdata[i].Volume);
                    }
                    break;
            }


            double[] spxChange = new double[marketdata.Count];
            double[] volumeChange = new double[marketdata.Count];
            double[] nviValue = new double[marketdata.Count];
            double[] nviCumulative = new double[marketdata.Count];

            for (int i = mode.Equals("0") ? 0 : dates.Count - 1 ; i < dates.Count; i++)
            {
                spxChange[i] = i < 1 ? 0 : ((closedPrice[i] - closedPrice[i - 1])/closedPrice[i - 1]*100);
                volumeChange[i] = i < 1 ? 0 : ((volume[i] - volume[i - 1])/volume[i - 1]*100);
                nviValue[i] = volumeChange[i] < 0 ? spxChange[i] : 0;
                nviCumulative[i] = i == 0 ? 1000 : nviCumulative[i - 1] + nviValue[i];


                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "NegativeVolumeIndex",
                    Value = nviCumulative[i]
                });

                //FileLogWriter looger = new FileLogWriter();
                //looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2},{3}", spxChange[i],
                //                                              volumeChange[i],
                //                                              nviValue[i],
                //                                              nviCumulative[i]), "FinTA");
            }

             return resultData;
        }
    }
}
