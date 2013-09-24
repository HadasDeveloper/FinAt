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

            foreach (MarketData mdata in marketdata)
            {
                closedPrice.Add(mdata.ClosePrice);
                highPrice.Add(mdata.HighPrice);
                lowPrice.Add(mdata.LowPrice);
                dates.Add(mdata.Date);
            }

            double[] atr = new double[marketdata.Count];
            List<double> tr = CalcTr(highPrice, lowPrice, closedPrice);

            for (int i = mode.Equals("0") ? 0 : marketdata.Count - 1; i < marketdata.Count; i++)
            { 
                atr[i] = i < (daysToGoBack - 1) ? 0 : (i == daysToGoBack - 1 ? tr.GetRange(0, daysToGoBack).Average() : (atr[i - 1] * (daysToGoBack - 1) + tr[i]) / daysToGoBack);
                
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
            double[] highLow = new double[marketdata.Count];
            double[] highPreviosClose = new double[marketdata.Count];
            double[] lowPreviosClose = new double[marketdata.Count];

            for (int i = 0; i < marketdata.Count; i++)
            {

                highLow[i] = highPrice[i] - lowPrice[i];
                highPreviosClose[i] = i == 0 ? 0 : Math.Abs(highPrice[i] - closedPrice[i - 1]);
                lowPreviosClose[i] = i == 0 ? 0 : Math.Abs(lowPrice[i] - closedPrice[i - 1]);
            }
            return highLow.Select((t, i) => Math.Max(t, Math.Max(highPreviosClose[i], lowPreviosClose[i]))).ToList();
        }

    }
}