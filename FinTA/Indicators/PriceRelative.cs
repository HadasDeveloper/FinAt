using System;
using System.Collections.Generic;
using System.Data;
using FinTA.Helper;
using FinTA.Models;
using Logger;

namespace FinTA.Indicators
{
    public class PriceRelative
    {
    
        private readonly List<MarketData> marketdata;
        private readonly List<MarketData> comparativeMarketdata;
        private readonly string comparativeInstrument;
        public readonly DataTable Data = new DataTable();
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();


        public PriceRelative(List<MarketData> marketdata, string symbol)
        {
            this.marketdata = marketdata;
            comparativeInstrument = symbol;
            comparativeMarketdata = DataContext.GetOneInstrumentMarketData(comparativeInstrument); 
        }

        public List<IndicatorsData> Calculate(string mode)
        {
            
            List<double> closedPrice = new List<double>();
            List<DateTime> date = new List<DateTime>();
            List<double> comparativeClosedPrice = new List<double>();

            int j = 0;
            for (int i = 0; i < marketdata.Count; i++)
                for ( ; j < comparativeMarketdata.Count; j++)
                {
                    if (marketdata[i].Date.Equals(comparativeMarketdata[j].Date))
                    {
                        closedPrice.Add(marketdata[i].ClosePrice);
                        comparativeClosedPrice.Add(comparativeMarketdata[j].ClosePrice);
                        date.Add(marketdata[i].Date);
                        break;
                    }
                }

            double[] priceRelative = new double[marketdata.Count];
            double[] changeInPriceRelative = new double[marketdata.Count];
            double[] instrumentChange = new double[marketdata.Count];
            double[] comparativeInstrumentChange = new double[marketdata.Count];
            double[] difference = new double[marketdata.Count];

            for (int i = mode.Equals("0") ? 0 : marketdata.Count - 1 ; i < closedPrice.Count; i++)
            {
                priceRelative[i] = closedPrice[i] == 0 ? 0 : closedPrice[i]/comparativeClosedPrice[i];
                changeInPriceRelative[i] = (i == 0)  || (priceRelative[i - 1] == 0)? 0 : (priceRelative[i] - priceRelative[i - 1])/priceRelative[i - 1];  
                instrumentChange[i] = (i == 0)  || (closedPrice[i - 1] == 0)? 0 : (closedPrice[i] - closedPrice[i - 1])/closedPrice[i - 1];
                comparativeInstrumentChange[i] = (i == 0)  || (comparativeClosedPrice[i - 1] == 0)? 0 : (comparativeClosedPrice[i] - comparativeClosedPrice[i - 1])/comparativeClosedPrice[i - 1];
                difference[i] = instrumentChange[i] - comparativeInstrumentChange[i]; 
  
                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = date[i],
                    Indicatore = "PriceRelative",
                    Value = difference[i]
                });

                //FileLogWriter looger = new FileLogWriter();
                //looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2},{3},{4},{5},{6}", date[i],comparativeClosedPrice[i],
                //              priceRelative[i],
                //              changeInPriceRelative[i],
                //              instrumentChange[i],
                //              comparativeInstrumentChange[i],
                //              difference[i]),
                //              "FinTA");
            }

             return resultData;
        }
    }
}


