using System;
using System.Collections.Generic;
using System.Data;
using FinTA.Models;
using FinTA.Overlays;
using Logger;

namespace FinTA.Indicators
{
    public class BIndicator
    {
        private readonly List<MarketData> marketdata;
        private readonly int daysToGoBack;
        private readonly int numberOfStd;
        public readonly DataTable Data = new DataTable();
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();


        public BIndicator(List<MarketData> marketdata, int numberOfStd, int daysToGoBack)
        {
            this.marketdata = marketdata;
            this.numberOfStd = numberOfStd;
            this.daysToGoBack = daysToGoBack;         
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
            double[] middleBand = sma.Calculate(closedPrice,daysToGoBack);

            StandardDeviation standardDeviationcs = new StandardDeviation();
            double[] std = standardDeviationcs.Calculate(closedPrice, middleBand, daysToGoBack);

            double[] upperBand = new double[marketdata.Count];
            double[] lowerBand = new double[marketdata.Count];
            double[] bIndicator = new double[marketdata.Count];


            for (int i = mode.Equals("0") ? 0 : marketdata.Count - 1 ; i < marketdata.Count; i++)
            {
                upperBand[i] = middleBand[i] + std[i] * numberOfStd;
                lowerBand[i] = middleBand[i] - std[i] * numberOfStd;
                bIndicator[i] = (upperBand[i] - lowerBand[i]) == 0 ? 0 : (closedPrice[i] - lowerBand[i]) / (upperBand[i] - lowerBand[i]);

                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "BIndicator",
                    Value = bIndicator[i]
                });

                //FileLogWriter looger = new FileLogWriter();
                //looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2},{3},{4},{5}", daysToGoBack,
                //            numberOfStd,
                //            middleBand[i],
                //            upperBand[i],
                //            lowerBand[i],
                //            bIndicator[i]
                //    ), "FinTA");
 
            }
             return resultData;
        }
    }
}
