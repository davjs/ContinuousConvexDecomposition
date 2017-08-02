using UnityEngine;

namespace Assets
{
    public struct Line
    {
        public Vector2 Begin;
        public Vector2 End;

        public Line(Vector2 begin, Vector2 end) : this()
        {
            Begin = begin;
            End = end;
        }
    }
}