using System.Collections.Generic;

namespace FinTA.Overlays
{
    class ExponentialMovingAverage
    {
        public double[] Calculate(List<double> value, double[] sma, double multiplier, int rang)
        {
            double[] ema = new double[value.Count];

            for (int i = 0; i < value.Count; i++)
                ema[i] = i < rang - 1 ? 0 : (i == rang -1 ? sma[i] : (value[i] - ema[i - 1]) * multiplier + ema[i - 1]);
                
            return ema;
        }

        public List<double> CalculateList(List<double> value, double[] sma, double multiplier, int rang)
        {
            List<double> ema = new List<double>();

            for (int i = 0; i < value.Count; i++)
                ema.Add(i < rang - 1 ? 0 : (i == rang - 1 ? sma[i] : (value[i] - ema[i - 1]) * multiplier + ema[i - 1]));

            return ema;
        }
    }
}
