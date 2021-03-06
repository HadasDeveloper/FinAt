﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FinTA.Models;
using Logger;

namespace FinTA.Indicators
{
    public class PercentageVolumeOscillator
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
            List<double> volume = new List<double>();
            List<DateTime> dates = new List<DateTime>();

            switch (mode)
            {

                case "0":
                    foreach (MarketData mdata in marketdata)
                    {
                        dates.Add(mdata.Date);                     
                        volume.Add(mdata.Volume);
                    }
                    break;
                case "1":
                    for (int i = marketdata.Count - period2*2 + 1; i < marketdata.Count; i++)
                    {
                        dates.Add(marketdata[i].Date);
                        volume.Add(marketdata[i].Volume);
                    }
                    break;
            }

            PercentageOscillator po = new PercentageOscillator(volume, period1, period2, period3);
            DataTable pvo =  po.Calculate("0");

            for (int i = mode.Equals("0") ? 0 : dates.Count - 1; i < dates.Count; i++)
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
