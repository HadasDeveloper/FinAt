using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FinTA.Models;
using FinTA.Overlays;
using Logger;

namespace FinTA.Indicators
{
    public class StandardDeviationVolatility
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
                    for (int i = marketdata.Count - daysToGoBack ; i < marketdata.Count; i++)
                    {
                        dates.Add(marketdata[i].Date);
                        closedPrice.Add(marketdata[i].ClosePrice);
                    }
                    break;
            }

            double[] periodAverage = new double[dates.Count];
            double[] deviation = new double[dates.Count];
            List<double> deviationSquared = new List<double>();
           
            int startSmaIndex = 0;
            for (int i = 0; i < dates.Count; i++)
            {
                periodAverage[i] = closedPrice.GetRange(startSmaIndex, daysToGoBack).Average();
                deviation[i] = closedPrice[i] - periodAverage[i];
                deviationSquared.Add(deviation[i]*deviation[i]);

                if((i  + 1) % daysToGoBack == 0)
                    startSmaIndex = startSmaIndex + daysToGoBack;
            }

            SimpleMovingAverage sma = new SimpleMovingAverage();
            double[] periodAverageOfDeviationSquared = sma.Calculate(deviationSquared, daysToGoBack);

            double[] standardDeviation = new double[dates.Count];

            for (int i = 0 ; i < dates.Count; i++)
            {
                standardDeviation[i] = Math.Sqrt(periodAverageOfDeviationSquared[i]);

                if (mode.Equals("0") || (mode.Equals("1") && i == dates.Count - 1))
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

