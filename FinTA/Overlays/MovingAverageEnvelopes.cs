using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FinTA.Models;
using Logger;

namespace FinTA.Overlays
{
    class MovingAverageEnvelopes
    {
        private readonly List<MarketData> marketdata;
        private readonly int daysToGoBack;
        private readonly double[] envelope;
        public readonly DataTable Data = new DataTable();
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();

        public MovingAverageEnvelopes(List<MarketData> marketdata, int daysToGoBack, double envelope )
        {
            this.marketdata = marketdata;
            this.daysToGoBack = daysToGoBack;
            this.envelope = new double[marketdata.Count];

            for (int i = 0; i < marketdata.Count; i++)
                this.envelope[i] = i < daysToGoBack - 1 ? 0 :(double)envelope / 100;
            
        }

        public List<IndicatorsData> Calculate(string mode)
        {
            List<double> closedPrice = marketdata.Select(mdata => mdata.ClosePrice).ToList();
            List<DateTime> dates = marketdata.Select(mdata => mdata.Date).ToList();

            SimpleMovingAverage simpleMovingAverage = new SimpleMovingAverage();
            double[] sma = simpleMovingAverage.Calculate(closedPrice, daysToGoBack);
            double[] uEnvelope = new double[marketdata.Count];
            double[] lEnvelope = new double[marketdata.Count];

            for (int i = mode.Equals("0") ? 0 : marketdata.Count - 1 ; i < marketdata.Count; i++)
            {
                uEnvelope[i] = sma[i] + (sma[i]*envelope[i]);
                lEnvelope[i] = sma[i] - (sma[i]*envelope[i]);

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
