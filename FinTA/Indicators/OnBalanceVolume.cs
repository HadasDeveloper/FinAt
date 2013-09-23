using System;
using System.Collections.Generic;
using System.Data;
using FinTA.Models;
using Logger;

namespace FinTA.Indicators
{
    class OnBalanceVolume
    {
        private readonly List<MarketData> marketdata;
        public readonly DataTable Data = new DataTable();
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();


        public OnBalanceVolume(List<MarketData> marketdata)
        {
            this.marketdata = marketdata;
        }

        public List<IndicatorsData> Calculate(string mode)
        {
            List<double> closedPrice = new List<double>();
            List<double> volume = new List<double>();
            List<DateTime> dates = new List<DateTime>(); 

            foreach (MarketData mdata in marketdata)
            {
                closedPrice.Add(mdata.ClosePrice);
                volume.Add((double) mdata.Volume);
                dates.Add(mdata.Date);
            }

            double[] upDown = new double[marketdata.Count];
            double[] positiveNegative = new double[marketdata.Count];
            double[] obv = new double[marketdata.Count];

            for (int i = mode.Equals("0") ? 0 : marketdata.Count - 1 ; i < marketdata.Count; i++)
            {
                upDown[i] = i < 1 ? 0 : closedPrice[i] - closedPrice[i - 1] > 0 ? 1 : -1 ;
                positiveNegative[i] = upDown[i] * volume[i];
                obv[i] = i < 1 ? positiveNegative[i] : positiveNegative[i] + obv[i-1];

                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "OnBalanceVolume",
                    Value = obv[i]
                });

                //FileLogWriter looger = new FileLogWriter();
                //looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2}", upDown[i],
                //              positiveNegative[i],
                //              obv[i]), "FinTA");
            }

             return resultData;
        }

    }
}
