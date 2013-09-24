using System;
using System.Collections.Generic;
using System.Data;
using FinTA.Models;
using FinTA.Overlays;
using Logger;

namespace FinTA.Indicators
{
    public class MovingAverageConvergenceDivergence
    {
        private readonly List<MarketData> marketdata;
        public readonly DataTable Data = new DataTable();
        private readonly int macdDays1;
        private readonly int macdDays2;
        private readonly int signalDays;
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();


        public MovingAverageConvergenceDivergence(List<MarketData> marketdata, int macdDays1, int macdDays2 , int signalDays )
        {
            this.marketdata = marketdata;
            this.macdDays1 = macdDays1;
            this.macdDays2 = macdDays2;
            this.signalDays = signalDays;
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
            ExponentialMovingAverage ema = new ExponentialMovingAverage();
            double[] emaDays1 = ema.Calculate(closedPrice, sma.Calculate(closedPrice, macdDays1),   //12 days ema(close peice)
                                               2/((double) macdDays1 + 1), macdDays1);
            double[] emaDays2 = ema.Calculate(closedPrice, sma.Calculate(closedPrice, macdDays2),   //26 days ema(close peice)
                                               2/((double) macdDays2 + 1), macdDays2);

            List<double> macdLine = new List<double>();

            for (int i = 0; i < marketdata.Count; i++)
                macdLine.Add(i < macdDays2 -1 ? 0 : emaDays1[i] - emaDays2[i]);

            double[] smaMacdLine = sma.Calculate(macdLine, signalDays, macdDays2 + signalDays - 1);
            //signalLine = 9 days ema(macd Line)
            double[] signalLine = ema.Calculate(macdLine, smaMacdLine , 2 / ((double)signalDays + 1), macdDays2 + signalDays - 1);
            double[] macdHistogram = new double[marketdata.Count];

            for (int i = mode.Equals("0") ? 0 : marketdata.Count - 1; i < marketdata.Count; i++)
            {
                macdHistogram[i] = i < macdDays2 + signalDays -2 ? 0 : macdLine[i] - signalLine[i];


                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "MACD",
                    Value = smaMacdLine[i]
                });
                
                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "MACDHistogram",
                    Value = macdHistogram[i]
                });


                //FileLogWriter looger = new FileLogWriter();
                //looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2},{3},{4},{5}", emaDays1[i], emaDays2[i], macdLine[i], smaMacdLine[i], signalLine[i],
                //                                              macdHistogram[i]), "FinTA");

            }

             return resultData;
        }
        
    }
}
