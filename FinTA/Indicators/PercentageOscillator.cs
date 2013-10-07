using System.Collections.Generic;
using System.Data;
using FinTA.Overlays;

namespace FinTA.Indicators
{
    public class PercentageOscillator
    {
        private readonly List<double > data;
        private readonly int period1;
        private readonly int period2;
        private readonly int period3;

        public readonly DataTable Data = new DataTable();

        public PercentageOscillator(List<double > data , int period1, int period2 , int period3)
        {
            this.data = data;
            this.period1 = period1;
            this.period2 = period2;
            this.period3 = period3;

            Data.Columns.Add("EmaPeriod1",typeof(double));
            Data.Columns.Add("EmaPeriod2",typeof(double));
            Data.Columns.Add("Macd", typeof(double));
            Data.Columns.Add("Po", typeof(double));
            Data.Columns.Add("SignalLine", typeof(double));
            Data.Columns.Add("Histogram", typeof(double));

        }

        public DataTable Calculate(string mode)
        {
            SimpleMovingAverage sma = new SimpleMovingAverage();
            ExponentialMovingAverage ema = new ExponentialMovingAverage();

            double[] emaPeriod1 = ema.Calculate(data, sma.Calculate(data, period1), //12 days ema(close peice)
                                                 2/((double) period1 + 1), period1);
            double[] emaPeriod2 = ema.Calculate(data, sma.Calculate(data, period2), //26 days ema(close peice)
                                                 2/((double) period2 + 1), period2);

            double[] macd = new double[data.Count];
            List<double> po = new List<double>();

            for (int i = 0; i < data.Count; i++)
            {
                macd[i] = i < period2 - 1 ? 0 : emaPeriod1[i] - emaPeriod2[i];
                po.Add(emaPeriod2[i] == 0 ? 0 : macd[i]/emaPeriod2[i]*100);
            }

            double[] signalLine = ema.Calculate(po, sma.Calculate(po, period3, po.Count - period3 + 1), 2 / ((double)period3 + 1), po.Count - period3 + 1);

            double[] histogram = new double[data.Count];

            for (int i = 0 ; i < data.Count; i++)
            {
                histogram[i] = i < po.Count - period3  ? 0 : po[i] - signalLine[i];
                Data.Rows.Add(emaPeriod1[i],
                                  emaPeriod2[i],
                                  macd[i],
                                  po[i],
                                  signalLine[i],
                                  histogram[i]
                        );
            }

            return Data;
        }
    }
}
