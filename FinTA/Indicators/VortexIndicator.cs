using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FinTA.Models;
using Logger;

namespace FinTA.Indicators
{
    class VortexIndicator
    {
        private readonly List<MarketData> marketdata;
        private readonly int period;
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();

        public readonly DataTable Data = new DataTable();

        public VortexIndicator(List<MarketData> marketdata, int period)
        {
            this.marketdata = marketdata;
            this.period = period;
        }

        public List<IndicatorsData> Calculate(string mode)
        {
            List<double> closedPrice = marketdata.Select(mdata => mdata.ClosePrice).ToList();
            List<double> highPrice = marketdata.Select(mdata => mdata.HighPrice).ToList();
            List<double> lowPrice = marketdata.Select(mdata => mdata.LowPrice).ToList();
            List<DateTime> dates = marketdata.Select(mdata => mdata.Date).ToList();

            List<double> positiveMovement = new List<double>();
            List<double> negativeMovement = new List<double>();
            List<double> periodicPositiveMovement = new List<double>();
            List<double> periodicNegativeMovement = new List<double>();
            List<double> trueRange = new List<double>();
            List<double> periodicTrueRange = new List<double>();
            double normalizedPositiveMovement;
            double normalizedNegativeMovement;

            for (int i = 0 ; i < marketdata.Count; i++)
            {
                positiveMovement.Add(i == 0 ? 0 : Math.Abs(highPrice[i] - lowPrice[i - 1]));
                negativeMovement.Add(i == 0 ? 0 : Math.Abs(lowPrice[i] - highPrice[i - 1]));
                periodicPositiveMovement.Add(i < period ? 0 : positiveMovement.GetRange(i - period + 1, period).Sum());
                periodicNegativeMovement.Add(i < period ? 0 : negativeMovement.GetRange(i - period + 1, period).Sum());
                trueRange.Add(i == 0
                                  ? 0
                                  : Math.Max(highPrice[i] - lowPrice[i],
                                             Math.Max(Math.Abs(highPrice[i] - closedPrice[i - 1]),
                                                      Math.Abs(lowPrice[i] - closedPrice[i - 1]))));
                periodicTrueRange.Add(i < period ? 0 : trueRange.GetRange(i-period + 1,period).Sum());
            }
            
            for (int i = mode.Equals("0") ? 0 : marketdata.Count - 1; i < marketdata.Count ; i++)
            {
               
                normalizedPositiveMovement = (periodicTrueRange[i]== 0 ? 0 : periodicPositiveMovement[i] / periodicTrueRange[i]);
                normalizedNegativeMovement = (periodicTrueRange[i] == 0 ? 0 : periodicNegativeMovement[i] / periodicTrueRange[i]);

                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "PositiveVortexIndicator",
                    Value = normalizedPositiveMovement
                });

                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "NegativeVortexIndicator",
                    Value = normalizedNegativeMovement
                });

                //FileLogWriter looger = new FileLogWriter();
                //looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", positiveMovement[i],
                //              negativeMovement[i],
                //              periodicPositiveMovement[i],
                //              periodicNegativeMovement[i],
                //              trueRange[i],
                //              periodicTrueRange[i],
                //              normalizedPositiveMovement[i],
                //              normalizedNegativeMovement[i]),
                //              "FinTA"
                //    );
            }

             return resultData;
        }

    }
}
