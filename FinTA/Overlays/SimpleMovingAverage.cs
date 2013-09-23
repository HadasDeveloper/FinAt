using System.Collections.Generic;
using System.Linq;

namespace FinTA.Overlays
{
    public class SimpleMovingAverage
    {
        public double[] Calculate(List<double> values , int rang)
        {
            double[] sma = new double[values.Count];

            for (int i = 0; i < values.Count; i++)
                sma[i] = i < rang - 1 ? 0 : values.GetRange(i - rang + 1, rang).Average();

            return sma;
        }

        public double[] Calculate(List<double> values, int rang, int startingIndex)
        {
            double[] sma = new double[values.Count];

            for (int i = 0; i < values.Count; i++)
                sma[i] = i < startingIndex - 1 ? 0 : values.GetRange(i - rang + 1, rang).Average();

            return sma;
        }
    }
}
