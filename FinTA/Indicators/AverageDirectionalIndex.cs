using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FinTA.Models;
using Logger;

namespace FinTA.Indicators
{
    class AverageDirectionalIndex
    {
        private readonly List<MarketData> marketdata;
        private readonly int daysToGoBack;
        public readonly DataTable Data = new DataTable();
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();

        public AverageDirectionalIndex(List<MarketData> marketdata, int daysToGoBack)
        {
            this.marketdata = marketdata;
            this.daysToGoBack = daysToGoBack;
        }

        public List<IndicatorsData> Calculate(string mode)
        {
            List<double> closedPrice = new List<double>();
            List<double> highPrice = new List<double>();
            List<double> lowPrice = new List<double>();
            List<DateTime> dates = new List<DateTime>();

            foreach (MarketData mdata in marketdata)
            {
                closedPrice.Add(mdata.ClosePrice);
                highPrice.Add(mdata.HighPrice);
                lowPrice.Add(mdata.LowPrice);
                dates.Add(mdata.Date);
            }

            AverageTrueRange atr = new AverageTrueRange(marketdata, daysToGoBack);
            List<double> tr = atr.CalcTr(highPrice, lowPrice, closedPrice);
            
            List<double> plusDm = new List<double>();
            List<double> minusDm = new List<double>();

            double[] periodicTr = new double[marketdata.Count];

            for (int i = 0; i < marketdata.Count; i++)
            {
                periodicTr[i] = i < daysToGoBack ? 0 : tr.GetRange(i - daysToGoBack + 1, daysToGoBack).Sum();

                double diffHigh = i == 0 ? 0 : highPrice[i] - highPrice[i - 1];
                double diffLow = i == 0 ? 0 : lowPrice[i - 1] - lowPrice[i];

                plusDm.Add(i == 0 ? 0 : diffHigh > diffLow ? Math.Max(diffHigh, 0) : 0);
                minusDm.Add(i == 0 ? 0 : diffLow > diffHigh ? Math.Max(diffLow, 0) : 0);
            }

            double[] plusPeriodicDm = CalcWilderSmoothing(plusDm);
            double[] minusPeriodicDm = CalcWilderSmoothing(minusDm);

            List<double> plusDi = CalcDirectionalIndicator(periodicTr, plusPeriodicDm);
            List<double> minusDi = CalcDirectionalIndicator(periodicTr, minusPeriodicDm);

            double[] diDiff = new double[marketdata.Count];
            double[] diSum = new double[marketdata.Count];

            double[] adx = new double[marketdata.Count];

            for (int i = 0; i < marketdata.Count; i++)
            {
                diDiff[i] = Math.Abs(plusDi[i] - minusDi[i]);
                diSum[i] = plusDi[i] + minusDi[i];
            }

            List<double> dx = CalcDirectionalIndicator(diSum, diDiff);

            for (int i = mode.Equals("0") ? 0 : marketdata.Count - 1 ; i < marketdata.Count; i++)
            {
                adx[i] = i < 2 * daysToGoBack - 1
                             ? 0
                             : i == 2 * daysToGoBack - 1
                                   ? dx.GetRange(i - daysToGoBack + 1, daysToGoBack).Average()
                                   : (adx[i - 1] * (daysToGoBack - 1) + dx[i]) / daysToGoBack;

                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "AverageDirectionalIndex",
                    Value = adx[i]
                });


                //FileLogWriter looger = new FileLogWriter();
                //looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", tr[i],
                //            plusDm[i],
                //            minusDm[i],
                //            periodicTr[i],
                //            plusPeriodicDm[i],
                //            minusPeriodicDm[i],
                //            plusDi[i],
                //            minusDi[i],
                //            diDiff[i],
                //            diSum[i],
                //            dx[i],
                //            adx[i]), "FinTA");

            }

            return resultData;
        }

        private double[] CalcWilderSmoothing(List<double> list)
        {

            double[] smoothedList = new double[marketdata.Count];

            for (int i = 0; i < marketdata.Count; i++)
            {
                smoothedList[i] = i < daysToGoBack
                                    ? 0
                                    : i == daysToGoBack
                                          ? list.GetRange(i - daysToGoBack + 1, daysToGoBack).Sum()
                                          : smoothedList[i - 1] - smoothedList[i - 1] / daysToGoBack + list[i];
            }

            return smoothedList;
        }

        private List<double> CalcDirectionalIndicator(double[] tr, double[] dm)
        {
            return marketdata.Select((t, i) => tr[i] == 0 ? 0 : (dm[i] / tr[i]) * 100).ToList();
        }
    }
}

