using System;
using System.Collections.Generic;
using System.Linq;

static internal class PolygonIterating
{
    // 0,5 -> 0,1,2,3,4
    // 1,5 -> 1,2,3,4
    // 3,3 -> []
    public static IEnumerable<int> RangeFromTo(int start, int end)
    {
        return Enumerable.Range(start, end - start);
    }

    // Returns [from+1...to], goes around zero
    public static IEnumerable<int> RangeRound(int @from, int to, int size)
    {
        if (from == to)
            return new List<int>();
        if (@from > to)
        {
            return RangeFromTo(@from + 1, size)
                .Concat(RangeFromTo(0, to)).ToList();
        }
        else
        {
            return RangeFromTo(@from + 1, to).ToList();
        }
    }


    // Returns [from-1...to] reverse, goes around zero
    public static IEnumerable<int> RangeRoundReverse(int @from, int to, int size)
    {
        if (from == to)
            return new List<int>();
        if (@from < to)
        {
            return RangeFromTo(0, @from).Reverse()
                .Concat(RangeFromTo(to + 1, size).Reverse()).ToList();
        }
        else // from > to
        {
            return RangeFromTo(to + 1, @from).Reverse().ToList();
        }
    }
}