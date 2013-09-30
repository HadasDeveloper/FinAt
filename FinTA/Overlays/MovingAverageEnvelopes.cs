using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FinTA.Models;
using Logger;

namespace FinTA.Overlays
{
    public class MovingAverageEnvelopes
    {
        private readonly List<MarketData> marketdata;
        private readonly int daysToGoBack;
        private readonly double envelope;
        public readonly DataTable Data = new DataTable();
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();

        public MovingAverageEnvelopes(List<MarketData> marketdata, int daysToGoBack, double envelope )
        {
            this.marketdata = marketdata;
            this.daysToGoBack = daysToGoBack;
            this.envelope = envelope / 100; ;                      
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
                    for (int i = marketdata.Count - daysToGoBack ; i < marketdata.Count ; i++)
                    {
                        dates.Add(marketdata[i].Date);                       
                        closedPrice.Add(marketdata[i].ClosePrice);
                    }
                    break;
            }



            SimpleMovingAverage simpleMovingAverage = new SimpleMovingAverage();
            double[] sma = simpleMovingAverage.Calculate(closedPrice, daysToGoBack);
            double[] uEnvelope = new double[marketdata.Count];
            double[] lEnvelope = new double[marketdata.Count];

            for (int i = mode.Equals("0") ? 0 : dates.Count - 1; i < dates.Count; i++)
            {
                uEnvelope[i] = sma[i] + (sma[i]*envelope);
                lEnvelope[i] = sma[i] - (sma[i]*envelope);

                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "UpperMovingAverageEnvelopes",
                    Value = uEnvelope[i]
                });

                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "LowerMovingAverageEnvelopes",
                    Value = lEnvelope[i]
                });

                //FileLogWriter looger = new FileLogWriter();
                //looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2},{3},{4}",marketdata[i].Date, sma[i], envelope[i], uEnvelope[i], lEnvelope[i]), "FinTA");
             }
            return resultData;
        }

    }
}
