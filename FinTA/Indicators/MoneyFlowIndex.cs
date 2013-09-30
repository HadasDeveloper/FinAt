using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FinTA.Models;
using Logger;

namespace FinTA.Indicators
{
    public class MoneyFlowIndex
    { 
        private readonly List<MarketData> marketdata;
        private readonly int period;
        public readonly DataTable Data = new DataTable();
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();


        public MoneyFlowIndex(List<MarketData> marketdata, int period)
        {
            this.marketdata = marketdata;
            this.period = period;
            
        }

        public List<IndicatorsData> Calculate(string mode)
        {
            List<double> closedPrice = new List<double>();
            List<double> highPrice = new List<double>();
            List<double> lowPrice = new List<double>();
            List<double> volume = new List<double>();
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
                        volume.Add(mdata.Volume);
                    }
                    break;
                case "1":
                    for (int i = marketdata.Count - period; i < marketdata.Count; i++)
                    {
                        dates.Add(marketdata[i].Date);
                        lowPrice.Add(marketdata[i].LowPrice);
                        highPrice.Add(marketdata[i].LowPrice);
                        closedPrice.Add(marketdata[i].ClosePrice);
                        volume.Add(marketdata[i].Volume);
                    }
                    break;
            }


            double[] typicalPrice = new double[marketdata.Count];
            double[] upOrDown = new double[marketdata.Count];
            double[] rawMoneyFlow = new double[marketdata.Count];
            List<double> positiveMoneyFlow = new List<double>();
            List<double> negativeMoneyFlow = new List<double>();
            double[] periodPositiveMoneyFlow = new double[marketdata.Count];
            double[] periodNegativeMoneyFlow = new double[marketdata.Count];
            double[] periodMoneyFlowRatio = new double[marketdata.Count];
            double[] periodMoneyFlowIndex = new double[marketdata.Count];

            for (int i = 0 ; i < dates.Count; i++)
            {
                typicalPrice[i] = (highPrice[i] + lowPrice[i] + closedPrice[i])/3;

                upOrDown[i] = i < 1 ? 0 : (typicalPrice[i] - typicalPrice[i - 1] > 0 ? 1 : -1);
                rawMoneyFlow[i] = i < 1 ? 0 : volume[i]*typicalPrice[i];
                positiveMoneyFlow.Add(upOrDown[i] > 0 ? rawMoneyFlow[i] : 0);
                negativeMoneyFlow.Add(upOrDown[i] < 0 ? rawMoneyFlow[i] : 0);
            }

            for (int i = mode.Equals("0") ? 0 : dates.Count - 1; i < dates.Count; i++)
            {
                periodPositiveMoneyFlow[i] = i < period ? 0 : positiveMoneyFlow.GetRange(i - period + 1, period).Sum();
                periodNegativeMoneyFlow[i] = i < period ? 0 : negativeMoneyFlow.GetRange(i - period + 1, period).Sum();
                periodMoneyFlowRatio[i] =periodNegativeMoneyFlow[i]==0 ? 0 :  i < period ? 0 : periodPositiveMoneyFlow[i]/periodNegativeMoneyFlow[i];
                periodMoneyFlowIndex[i] = 100 - (100/(1 + periodMoneyFlowRatio[i]));


                resultData.Add(new IndicatorsData
                {
                    Instrument = marketdata[i].Instrument,
                    Date = dates[i],
                    Indicatore = "MoneyFlowIndex",
                    Value = periodMoneyFlowIndex[i]
                });


                //FileLogWriter looger = new FileLogWriter();
                //looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}", typicalPrice[i] , upOrDown[i],
                //             rawMoneyFlow[i],
                //             positiveMoneyFlow[i],
                //             negativeMoneyFlow[i],
                //             periodPositiveMoneyFlow[i],
                //             periodNegativeMoneyFlow[i],
                //             periodMoneyFlowRatio[i],
                //             periodMoneyFlowIndex[i]), "FinTA");
            }

             return resultData;
        }
    }
}
