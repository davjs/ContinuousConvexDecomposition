using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public struct Bounds2D
    {
        public float Top;
        public float Left;
        public float Bottom;
        public float Right;

        public void UpdateFromPoints(IEnumerable<Vector2> points)
        {
            Top = Left = Bottom = Right = 0;
            foreach (var point in points)
            {
                if (-point.x < -Left)
                    Left = -point.x;
                if (-point.y < -Bottom)
                    Bottom = -point.y;
                if (point.y > Top)
                    Top = point.y;
                if (point.x > Right)
                    Right = point.x;
            }
        }

        public bool CollidesWithRay(Vector2 position,Ray ray)
        {
            var actualTop = position.y + Top;
            var actualBot = position.y - Bottom;
            var actualLeft = position.x - Left;
            var actualRight = position.x + Right;

            if (ray.K != 0)
            {
                //to find x where y is top
                // k*x + m = top
                // k*x     = top - m
                // x       = (top - m) / k
                var topColXpos = (actualTop - ray.M) / ray.K;
                var botColXpos = (actualBot - ray.M) / ray.K;
                if (topColXpos >= actualLeft && topColXpos <= actualRight)
                    return true;
                if (botColXpos >= actualLeft && botColXpos <= actualRight)
                    return true;
            }

            var yLeft = actualLeft *ray.K + ray.M;
            if (yLeft >= actualBot && yLeft <= actualTop)
                return true;

            var yRight = actualRight * ray.K + ray.M;
            if (yRight >= actualBot && yRight <= actualTop)
                return true;
           

            return false;
        }

    }
}