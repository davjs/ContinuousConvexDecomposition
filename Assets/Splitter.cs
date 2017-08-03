using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    public class Splitter
    {
        public static KeyValuePair<List<int>, List<int>> SplitPolygon(int enter1, int enter2, int exit1, int exit2, int end)
        {
            var enterIsMinimum = enter1 < exit1;
            int min;
            int max;
            int maxOfMin;
            int minOfMax;
            if (enterIsMinimum)
            {
                if (enter1 < enter2)
                {
                    min = enter1;
                    maxOfMin = enter2;
                }
                else
                {
                    min = enter2;
                    maxOfMin = enter1;
                }
                if (exit1 > exit2)
                {
                    max = exit1;
                    minOfMax = exit2;
                }
                else
                {
                    max = exit2;
                    minOfMax = exit1;
                }
            }
            else
            {
                if (exit1 < exit2)
                {
                    min = exit1;
                    maxOfMin = exit2;
                }
                else
                {
                    min = exit2;
                    maxOfMin = exit1;
                }
                if (enter1 > enter2)
                {
                    max = enter1;
                    minOfMax = enter2;
                }
                else
                {
                    max = enter2;
                    minOfMax = enter1;
                }
            }

            var zeroToMin = Enumerable.Range(0, min + 1);
            var maxToEnd = Enumerable.Range(max, end - max + 1);

            var maxOfMinToMinOfMax = Enumerable.Range(maxOfMin, minOfMax - maxOfMin + 1);

            var poly1 = zeroToMin.Union(maxToEnd);
            var poly2 = maxOfMinToMinOfMax;

            return new KeyValuePair<List<int>, List<int>>(
                poly1.ToList(),
                poly2.ToList());
        }
    }
}
