using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FinTA.Models;
using FinTA.Overlays;
using Logger;

namespace FinTA.Indicators
{
    public class MassIndex
    {
        private readonly List<MarketData> marketdata;
        public readonly DataTable Data = new DataTable();
        private readonly int daysToGoBack1;
        private readonly int daysToGoBack2;
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();


        public MassIndex(List<MarketData> marketdata, int daysToGoBack1, int daysToGoBack2 )
        {
            this.marketdata = marketdata;
            this.daysToGoBack1 = daysToGoBack1;
            this.daysToGoBack2 = daysToGoBack2;       
        }

        public List<IndicatorsData> Calculate(string mode)
        {
            List<double> highPrice = new List<double>();
            List<double> lowPrice = new List<double>();
            List<double> highLowDiff = new List<double>();
            List<DateTime> dates = new List<DateTime>();


            switch (mode)
            {

                case "0":
                    for (int i = 0; i < marketdata.Count; i++)
                    {
                        dates.Add(marketdata[i].Date);
                        lowPrice.Add(marketdata[i].LowPrice);
                        highPrice.Add(marketdata[i].HighPrice);
                        highLowDiff.Add(highPrice[i] - lowPrice[i]);
                    }
                    break;
                case "1":
                    for (int i = marketdata.Count - daysToGoBack1 * 2 + 1 - daysToGoBack2 ; i < marketdata.Count; i++)
                    {
                        dates.Add(marketdata[i].Date);
                        lowPrice.Add(marketdata[i].LowPrice);
                        highPrice.Add(marketdata[i].LowPrice);
                        highLowDiff.Add(highPrice[i] - lowPrice[i]);
                    }
                    break;
            }
                
            SimpleMovingAverage sma = new SimpleMovingAverage();
            ExponentialMovingAverage ema = new ExponentialMovingAverage();

            double[] singleSma = sma.Calculate(highLowDiff, daysToGoBack1);
            List<double > singleEma = ema.CalculateList(highLowDiff, singleSma, 2 / ((double)daysToGoBack1 +1), daysToGoBack1);

            double[] doubleSma = sma.Calculate(singleEma, daysToGoBack1, daysToGoBack1*2 -1 );
            double[] doubleEma = ema.Calculate(singleEma, doubleSma, 2 / ((double)daysToGoBack1 + 1), daysToGoBack1 * 2 - 1);

            List<double> emaRatio = new List<double>();
            double[] massIndex = new double[marketdata.Count];

            for (int i =  0 ; i < marketdata.Count; i++)
                emaRatio.Add(i < daysToGoBack1 * 2 - 1 ? 0 : doubleEma[i] == 0 ? 0 : singleEma[i]/doubleEma[i] );

            for (int i = mode.Equals("0") ? 0 : marketdata.Count - 1 ; i < marketdata.Count; i++)
            {      
                massIndex[i] = i < daysToGoBack2 + (daysToGoBack1*2) - 2 ? 0 : emaRatio.GetRange(i - daysToGoBack2 + 1, daysToGoBack2).Sum();

                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "MassIndex",
                    Value = massIndex[i]
                });

                //FileLogWriter looger = new FileLogWriter();
                //looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2},{3},{4}", highLowDiff[i],
                //    singleEma[i],
                //    doubleEma[i],
                //    emaRatio[i],
                //    massIndex[i]), "FinTA");

            }

             return resultData;
        }

    }
}

