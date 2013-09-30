using System;
using System.Collections.Generic;

namespace FinTA.Helper
{
    class MathHelper
    {
        public double FindMax(List<double> values)
        {
            double max = values[0];

            foreach (var val in values)
                max = Math.Max(max, val);

            return max;
        }

        public double FindMin(List<double> values)
        {
            double min = values[0];

            foreach (var val in values)
                min = Math.Min(min, val);

            return min;
        }

        public double[] GetMaxIndex(List<double> values)
        {
            double[] max = new double[2];  
            max[0] =   values[0];

            for (int i = 0; i < values.Count; i++ )
            {
                max[1] = max[0] < values[i] ? i : max[1];
                max[0] = Math.Max(max[0], values[i]);
            }

            return max;
        }

        public double[] GetMinIndex(List<double> values)
        {
            double[] min = new double[2];
            min[0] = values[0];

           for (int i = 0; i < values.Count; i++)
           {
               min[1] = min[0] > values[i] ? i : min[1];
               min[0] = Math.Min(min[0], values[i]);
           }


            return min;
        }

    }
}
