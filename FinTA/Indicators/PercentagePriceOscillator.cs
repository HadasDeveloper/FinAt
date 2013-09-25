using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FinTA.Models;
using Logger;

namespace FinTA.Indicators
{
    public class PercentagePriceOscillator
    {
        private readonly List<MarketData> marketdata;
        private readonly int period1;
        private readonly int period2;
        private readonly int period3;
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();
        public readonly DataTable Data = new DataTable();

        public PercentagePriceOscillator(List<MarketData> marketdata, int period1, int period2, int period3)
        {
            this.marketdata = marketdata;
            this.period1 = period1;
            this.period2 = period2;
            this.period3 = period3;
        }

        public List<IndicatorsData> Calculate(string mode)
        {
            List<double> closedPrice = new List<double>();
            List<DateTime> dates = new List<DateTime>();

            switch (mode)
            {

                case "0":
                    foreach (MarketData mdata in marketdata)
                    {
                        dates.Add(mdata.Date);                     
                        closedPrice.Add(mdata.ClosePrice);                       
                    }
                    break;
                case "1":
                    for (int i = marketdata.Count - period2 - period3; i < marketdata.Count; i++)
                    {
                        dates.Add(marketdata[i].Date);
                        closedPrice.Add(marketdata[i].ClosePrice);
                    }
                    break;
            }
            
            PercentageOscillator po = new PercentageOscillator(closedPrice, period1, period2, period3);
            DataTable ppo =  po.Calculate("0");

            for (int i = mode.Equals("0") ? 0 : marketdata.Count - 1 ; i < marketdata.Count; i++)
            {

                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "PercentagePriceOscillator",
                    Value = ppo.Rows[i].Field<double>("SignalLine")
                });

                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "PercentagePriceOscillatorHistogram",
                    Value = ppo.Rows[i].Field<double>("Histogram")
                });


                //FileLogWriter looger = new FileLogWriter();
                //looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2},{3},{4},{5}", ppo.Rows[i].Field<double>("EMAPeriod1"),
                //              ppo.Rows[i].Field<double>("EMAPeriod2"),
                //              ppo.Rows[i].Field<double>("MACD"),
                //              ppo.Rows[i].Field<double>("PO"), 
                //              ppo.Rows[i].Field<double>("SignalLine"),
                //              ppo.Rows[i].Field<double>("Histogram")),
                //              "FinTA"
                //    );
            }

             return resultData;
        }
    }
}
