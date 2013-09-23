using System;
using System.Collections.Generic;
using FinTA.Models;
using Logger;

namespace FinTA.Overlays
{
    class BollingerBands
    {
        private readonly List<MarketData> marketdata;
        private readonly int numberOfStd;
        private readonly int daysToGoBack;
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();

        public BollingerBands(List<MarketData> marketdata, int numberOfStd, int daysToGoBack)
        {
            this.marketdata = marketdata;
            this.numberOfStd = numberOfStd;
            this.daysToGoBack = daysToGoBack;
        }

        public List<IndicatorsData> Calculate(string mode)
        {

            List<DateTime> dates = new List<DateTime>();    
            List<double> closedPrice = new List<double>();

            foreach (MarketData mdata in marketdata)
            {
                dates.Add(mdata.Date);
                closedPrice.Add(mdata.ClosePrice); 
            }

            SimpleMovingAverage simpleMovingAverage = new SimpleMovingAverage();
            double[] sma = simpleMovingAverage.Calculate(closedPrice, daysToGoBack);
            StandardDeviation standardDeviationcs = new StandardDeviation();
            double[] std = standardDeviationcs.Calculate(closedPrice, sma, daysToGoBack);
            double[] ubsma = CalcUbSma(sma, std);
            double[] lbsma = CalcLbSma(sma, std);
            double[] bandWidth = CalcBandWidth(ubsma, lbsma);

            for ( int i = mode.Equals("0") ? 0 : marketdata.Count - 1 ; i < marketdata.Count; i++)
            {
                resultData.Add( new IndicatorsData
                                            {
                                                Instrument = marketdata[i].Instrument,
                                                Date = dates[i],
                                                Indicatore = "BollingerBands",
                                                Value = bandWidth[i]
                                            });

                FileLogWriter looger = new FileLogWriter();

                //looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2},{3},{4},{5}",
                //                                                marketdata[i].Date,
                //                                                sma[i],
                //                                                std[i],
                //                                                ubsma[i],
                //                                                lbsma[i],
                //                                                bandWidth[i]), "FinTA");
            }

            return resultData;
        }
            

        private double[] CalcUbSma(double[] sma, double[] std)
        {
            double[] ubsma = new double[sma.Length];

            for (int i = 0; i < sma.Length ; i++)
                ubsma[i] = sma[i] + (std[i] * numberOfStd);

            return ubsma;
        }

        private double[] CalcLbSma(double[] sma, double[] std)
        {
            double[] lbsma = new double[sma.Length];

            for (int i = 0; i < sma.Length; i++)
                lbsma[i] = sma[i] - (std[i] * numberOfStd);

            return lbsma;
        }

        private double[] CalcBandWidth(double[] ubsma, double[] lbsma)
        {
            double[] bandWidth = new double[ubsma.Length];

            for (int i = 0; i < ubsma.Length; i++)
                bandWidth[i] = ubsma[i] - lbsma[i];

            return bandWidth;
        }
    }
}
