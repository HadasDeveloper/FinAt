using System;
using System.Collections.Generic;
using System.Data;
using FinTA.Models;
using FinTA.Overlays;
using Logger;

namespace FinTA.Indicators
{
    public class TrueStrengthIndex
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
            List<double> closedPrice = new List<double>();
            List<DateTime> dates = new List<DateTime>();
            List<double> change = new List<double>();
            List<double> abcChange = new List<double>();


            switch (mode)
            {

                case "0":
                    foreach (MarketData mdata in marketdata)
                    {
                        dates.Add(mdata.Date);
                        closedPrice.Add(mdata.ClosePrice);
                        change.Add(closedPrice.Count == 1 ? 0 : closedPrice[closedPrice.Count - 1] - closedPrice[closedPrice.Count - 2]);
                        abcChange.Add(Math.Abs(change[change.Count-1]));

                    }
                    break;
                case "1":
                    for (int i = marketdata.Count - period1*2 - 1 ; i < marketdata.Count; i++)
                    {
                        dates.Add(marketdata[i].Date);
                        closedPrice.Add(marketdata[i].ClosePrice);
                        change.Add(closedPrice.Count == 1 ? 0 : closedPrice[closedPrice.Count - 1] - closedPrice[closedPrice.Count - 2]);
                        abcChange.Add(Math.Abs(change[change.Count - 1]));
                    }
                    break;
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

            double[] tsi = new double[dates.Count];

            for (int i = mode.Equals("0") ? 0 : dates.Count - 1 ; i < dates.Count; i++)
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
