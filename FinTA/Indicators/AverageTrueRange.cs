using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FinTA.Models;
using Logger;

namespace FinTA.Indicators
{
    public class AverageTrueRange
    {
        private readonly List<MarketData> marketdata;
        private readonly int daysToGoBack;
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();
        public readonly DataTable Data = new DataTable();

        public AverageTrueRange(List<MarketData> marketdata, int daysToGoBack)
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

            switch (mode)
            {

                case "0":
                    foreach (MarketData mdata in marketdata)
                    {
                        dates.Add(mdata.Date);
                        lowPrice.Add(mdata.LowPrice);
                        highPrice.Add(mdata.HighPrice);
                        closedPrice.Add(mdata.ClosePrice);
                    }
                    break;
                case "1":
                    for (int i = marketdata.Count - daysToGoBack - 1; i < marketdata.Count; i++)
                    {
                        dates.Add(marketdata[i].Date);
                        lowPrice.Add(marketdata[i].LowPrice);
                        highPrice.Add(marketdata[i].HighPrice);
                        closedPrice.Add(marketdata[i].ClosePrice);
                    }
                    break;
            }

            double[] atr = new double[dates.Count];
            List<double> tr = CalcTr(highPrice, lowPrice, closedPrice);

            for (int  i=0 ; i < dates.Count; i++)
            { 
                atr[i] = i < (daysToGoBack - 1) ? 0 : (i == daysToGoBack - 1 ? tr.GetRange(0, daysToGoBack).Average() : (atr[i - 1] * (daysToGoBack - 1) + tr[i]) / daysToGoBack);


                if (mode.Equals("0") || (mode.Equals("1") && i == dates.Count - 1))
                    resultData.Add( new IndicatorsData
                                                {
                                                    Instrument = marketdata[i].Instrument,
                                                    Date = dates[i],
                                                    Indicatore = "AverageTrueRange",
                                                    Value = atr[i]
                                                });

                //FileLogWriter looger = new FileLogWriter();
                //looger.WriteToLog(DateTime.Now, string.Format("{0},{1}", tr[i], atr[i]), "FinTA");

            }
            
            return resultData;
        }

        public List<double> CalcTr(List<double> highPrice, List<double> lowPrice, List<double> closedPrice)
        {
            double[] highLow = new double[closedPrice.Count];
            double[] highPreviosClose = new double[closedPrice.Count];
            double[] lowPreviosClose = new double[closedPrice.Count];

            for (int i = 0; i < closedPrice.Count; i++)
            {

                highLow[i] = highPrice[i] - lowPrice[i];
                highPreviosClose[i] = i == 0 ? 0 : Math.Abs(highPrice[i] - closedPrice[i - 1]);
                lowPreviosClose[i] = i == 0 ? 0 : Math.Abs(lowPrice[i] - closedPrice[i - 1]);
            }
            return highLow.Select((t, i) => Math.Max(t, Math.Max(highPreviosClose[i], lowPreviosClose[i]))).ToList();
        }

    }
}