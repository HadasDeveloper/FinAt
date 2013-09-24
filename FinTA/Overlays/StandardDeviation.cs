using System;
using System.Collections.Generic;

namespace FinTA.Overlays
{
    public class StandardDeviation
    {
        public double[] Calculate(List<double> values, double[] sma, int range)
        {
            double[] dtdv = new double[values.Count];

            for (int i = 0; i < values.Count; i++)
            {
                double average = (double)sma[i];
                double sumOfDerivation = 0;

                if (i < range - 1)
                    dtdv[i] = 0;
                else
                {
                    foreach (double value in values.GetRange(i - range + 1, range))
                        sumOfDerivation += (((double)value - average) * ((double)value - average));

                    dtdv[i] = (double)Math.Sqrt(sumOfDerivation / range);
                }
            }

            return dtdv;
        }
    }
}
