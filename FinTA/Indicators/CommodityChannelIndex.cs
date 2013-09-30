using System;
using System.Collections.Generic;
using System.Data;
using FinTA.Models;
using FinTA.Overlays;
using Logger;

namespace FinTA.Indicators
{
    public class CommodityChannelIndex
    {
        private readonly List<MarketData> marketdata;
        private readonly int daysToGoBack;
        public readonly DataTable Data = new DataTable();
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();


        public CommodityChannelIndex(List<MarketData> marketdata, int daysToGoBack)
        {
            this.marketdata = marketdata;
            this.daysToGoBack = daysToGoBack;        
        }

        public List<IndicatorsData> Calculate(string mode)
        {
            List<double> tp = new List<double>();
            List<DateTime> dates = new List<DateTime>();

            switch (mode){

                case "0":
                    foreach (MarketData mdata in marketdata)
                    {
                        dates.Add(mdata.Date);
                        tp.Add((mdata.ClosePrice + mdata.HighPrice + mdata.LowPrice) / 3);
                    }
                    break;
                case "1":
                    for (int i = marketdata.Count - daysToGoBack; i < marketdata.Count; i++)
                    {
                        dates.Add(marketdata[i].Date);
                        tp.Add((marketdata[i].ClosePrice + marketdata[i].HighPrice + marketdata[i].LowPrice) / 3);
                    }
                    break;
            }
            
            SimpleMovingAverage sma = new SimpleMovingAverage();
            double[] tpSma = sma.Calculate(tp, daysToGoBack);

            double[] meanDeviation = new double[marketdata.Count];
            double[] cci = new double[marketdata.Count];

            for (int i = mode.Equals("0") ? 0 : dates.Count - 1; i < dates.Count; i++)
            {
                if(i < daysToGoBack - 1)
                {
                    meanDeviation[i] = 0;
                    cci[i] = 0;
                }
                else
                {
                    double total = 0;
                    foreach (var price in tp.GetRange(i - daysToGoBack + 1, daysToGoBack))
                        total = total + Math.Abs(tpSma[i] - price);

                    meanDeviation[i] = total/daysToGoBack;
                    cci[i] = (0.015 * meanDeviation[i]) ==0 ? 0 : (tp[i] - tpSma[i]) / (0.015 * meanDeviation[i]);
                }

                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "CommodityChannelIndex",
                    Value = cci[i]
                });

                //FileLogWriter looger = new FileLogWriter();
                //looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2},{3},{4}", daysToGoBack,
                //            tp[i],
                //            tpSma[i],
                //            meanDeviation[i],
                //            cci[i]), 
                //            "FinTA");
            }

             return resultData;
        }
    }
}
