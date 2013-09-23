using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FinTA.Models;
using Logger;

namespace FinTA.Indicators
{
    class PercentageVolumeOscillator
    {
        private readonly List<MarketData> marketdata;
        private readonly int period1;
        private readonly int period2;
        private readonly int period3;
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();
        public readonly DataTable Data = new DataTable();


        public PercentageVolumeOscillator(List<MarketData> marketdata, int period1, int period2, int period3)
        {
            this.marketdata = marketdata;
            this.period1 = period1;
            this.period2 = period2;
            this.period3 = period3;

        }

        public List<IndicatorsData> Calculate(string mode)
        {
            List<double> volume = marketdata.Select(mdata => (double)mdata.Volume).ToList();
            List<DateTime> dates = marketdata.Select(mdata => mdata.Date).ToList();    

            PercentageOscillator po = new PercentageOscillator(volume, period1, period2, period3);
            DataTable pvo =  po.Calculate("0");

            for (int i = mode.Equals("0") ? 0 : marketdata.Count - 1; i < marketdata.Count; i++)
            {

                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "PercentageVolumeOscillator",
                    Value = pvo.Rows[i].Field<double>("SignalLine")
                });

                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "PercentageVolumeOscillatorHistogram",
                    Value = pvo.Rows[i].Field<double>("Histogram")
                });

                //FileLogWriter looger = new FileLogWriter();
                //looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2},{3},{4},{5}", pvo.Rows[i].Field<double>("EMAPeriod1"),
                //              pvo.Rows[i].Field<double>("EMAPeriod2"),
                //              pvo.Rows[i].Field<double>("MACD"),
                //              pvo.Rows[i].Field<double>("PO"), 
                //              pvo.Rows[i].Field<double>("SignalLine"),
                //              pvo.Rows[i].Field<double>("Histogram")),
                //              "FinTA"
                //   );
            }

             return resultData;
        }
    }
}
