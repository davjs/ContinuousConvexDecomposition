using System;
using UnityEngine;

namespace Assets
{
    public struct Ray
    {
        public float K;
        public float M;
        public Line Line;
        public Vector2 Direction;

        public static Ray FromLine(Vector2 begin, Vector2 end)
        {
            var k = (begin.y - end.y) / (begin.x - end.x);
            float m;
            if (float.IsInfinity(k))
            {
                m = 0;
            }
            else
            {
                m = begin.y - begin.x * k;
            }
            return new Ray
            {
                K = k, M = m,Line =  new Line(begin,end),
                Direction = end - begin
            };
        }


        public bool Intersects(Ray ray, out Vector2 intersection)
        {
            // kx + m = ray.kx + ray.m
            // kx + m - ray.kx = ray.m
            // x * (k -ray.k) = m - ray.m
            // x = (m - ray.m) / (k - ray.k)
            if (Math.Abs(K - ray.K) > 0.001) //TODO: dont know if faster to remove, benchmark
            {
                var collidingX = (M - ray.M) / (K - ray.K);
                if (float.IsInfinity(ray.K)) // 
                {
                    intersection = new Vector2(collidingX, collidingX * K + M);
                }
                else
                {
                    intersection = new Vector2(collidingX, collidingX * ray.K + ray.M);
                }
                return true;
            }
            intersection = new Vector2();
            return false;
        }

        public bool CollidesWithSegment(Line line, out Vector2 collidePosition)
        {
            return LineCollisionStuff.GetRayToLineSegmentIntersection(this, line, out collidePosition);
        }
    }
}