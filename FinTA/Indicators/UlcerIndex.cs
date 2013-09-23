using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FinTA.Helper;
using FinTA.Models;
using FinTA.Overlays;
using Logger;

namespace FinTA.Indicators
{
    class UlcerIndex
    {
        private readonly List<MarketData> marketdata;
        private readonly int daysToGoBack;
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();

        public readonly DataTable Data = new DataTable();

        public UlcerIndex(List<MarketData> marketdata, int daysToGoBack)
        {
            this.marketdata = marketdata;
            this.daysToGoBack = daysToGoBack;
        }

        public List<IndicatorsData> Calculate(string mode)
        {
            List<double> closedPrice = marketdata.Select(mdata => mdata.ClosePrice).ToList();
            List<DateTime> dates = marketdata.Select(mdata => mdata.Date).ToList();

            List<double> maxClosePrice = new List<double>();
            List<double> percentDrawDown= new List<double>();
            List<double> percentDrawdownSquared = new List<double>();
            
            MathHelper mathHalper = new MathHelper();

            for (int i = 0; i < marketdata.Count; i++)
            {
                maxClosePrice.Add(i < daysToGoBack - 1 ? 0 : mathHalper.FindMax(closedPrice.GetRange(i - daysToGoBack + 1, daysToGoBack)));
                percentDrawDown.Add( i < daysToGoBack -1 ? 0 : (closedPrice[i] - maxClosePrice[i])/maxClosePrice[i]*100 );
                percentDrawdownSquared.Add(percentDrawDown[i]*percentDrawDown[i]);
            }

            SimpleMovingAverage sma = new SimpleMovingAverage();
            double[] percentDrawdownSquaredAvg = sma.Calculate(percentDrawdownSquared, daysToGoBack, daysToGoBack*2 - 1);
            double[] ulcerIndex = new double[marketdata.Count];

            for (int i = mode.Equals("0") ? 0 : marketdata.Count - 1; i < marketdata.Count; i++)
            {
                ulcerIndex[i] = (double)Math.Sqrt((double)percentDrawdownSquaredAvg[i]);

                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "UlcerIndex",
                    Value = ulcerIndex[i]
                });

                //FileLogWriter looger = new FileLogWriter();
                //looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2},{3}", maxClosePrice[i],
                //              percentDrawDown[i],
                //              percentDrawdownSquaredAvg[i],
                //              ulcerIndex[i]),
                //                  "FinTA"
                //    );
            }

             return resultData;
        }
    }
}
