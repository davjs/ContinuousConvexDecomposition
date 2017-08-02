using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public class LineCollisionStuff
    {
        public static  bool RayCollidesWithSegment(Ray ray, Vector2 p1, Vector2 p2,out Vector2 collidePosition)
        {
            return GetRayToLineSegmentIntersection(ray, new Line(p1, p2), out collidePosition);
        }

        static float CrossProduct(Vector2 v1, Vector2 v2)
        {
            return (v1.x* v2.y) - (v1.y* v2.x);
        }

    /// <summary>
    /// Returns intersection point on ray or null if there is no intersection.
    /// </summary>
        public static bool GetRayToLineSegmentIntersection(Ray ray, Line line, out Vector2 collidePosition)
        {
            var v1 = ray.Line.Begin - line.Begin;
            var v2 = line.End - line.Begin;
            var v3 = new Vector2(-ray.Direction.y, ray.Direction.x);


            var dot = Vector2.Dot(v2 ,v3);
            if (!(Math.Abs(dot) < 0.000001))
            {
                var t1 = CrossProduct(v2, v1)/dot;
                var t2 = Vector2.Dot(v1, v3)/dot;

                if (t1 >= 0.0 && (t2 >= 0.0 && t2 <= 1.0))
                {
                    collidePosition = ray.Line.Begin + t1 * (ray.Direction); // the point of intersection
                    return true;
                }

            }
            collidePosition = new Vector2();
            return false;
        }
    }
}
