using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FinTA.Models;
using FinTA.Overlays;
using Logger;

namespace FinTA.Indicators
{
    class TrueStrengthIndex
    {
        private readonly List<MarketData> marketdata;
        private readonly int period1;
        private readonly int period2;
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();

        public readonly DataTable Data = new DataTable();

        public TrueStrengthIndex(List<MarketData> marketdata, int period1, int period2 )
        {
            this.marketdata = marketdata;
            this.period1 = period1;
            this.period2 = period2;
        }

        public List<IndicatorsData> Calculate(string mode)
        {
            List<double> closedPrice = marketdata.Select(mdata => mdata.ClosePrice).ToList();
            List<DateTime> dates = marketdata.Select(mdata => mdata.Date).ToList();
            List<double> change = new List<double>();
            List<double> abcChange = new List<double>();
            
            for (int i = 0; i < marketdata.Count; i++)
            {
                change.Add(i == 0 ? 0 : closedPrice[i] - closedPrice[i - 1]);
                abcChange.Add(Math.Abs(change[i]));
            }

            SimpleMovingAverage sma = new SimpleMovingAverage();
            ExponentialMovingAverage ema = new ExponentialMovingAverage();
            List<double> emaChange = ema.CalculateList(change, sma.Calculate(change, period1,period1 + 1), 2/((double)period1 + 1), period1 +1);
            List<double> emaAbcChange = ema.CalculateList(abcChange, sma.Calculate(abcChange, period1, period1 + 1), 2 / ((double)period1 + 1), period1 +1);

            double[] doubleEmaChange = ema.Calculate(emaChange,
                                                      sma.Calculate(emaChange, period2, period1 + period2 - 1),
                                                      2 / ((double)period2 + 1), period1 + period2);
            
            double[] doulleAbcEmaChange = ema.Calculate(emaAbcChange,
                                                      sma.Calculate(emaAbcChange, period2, period1 + period2 - 1),
                                                      2/((double)period2 + 1), period1 + period2);

            double[] tsi = new double[marketdata.Count];

            for (int i = mode.Equals("0") ? 0 : marketdata.Count - 1 ; i < marketdata.Count; i++)
            {
                tsi[i] = doulleAbcEmaChange[i]==0 ? 0 : (doubleEmaChange[i] / doulleAbcEmaChange[i]) * 100;

                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "TrueStrengthIndex",
                    Value = tsi[i]
                });

                //FileLogWriter looger = new FileLogWriter();
                //looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2},{3},{4},{5},{6}", change[i],
                //              abcChange[i],
                //              emaChange[i],
                //              emaAbcChange[i],
                //              doubleEmaChange[i],
                //              doulleAbcEmaChange[i],
                //              tsi[i]),
                //                  "FinTA"
                //    );
            }

             return resultData;
        }
    }
}
