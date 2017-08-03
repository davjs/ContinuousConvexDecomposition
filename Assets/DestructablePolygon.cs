using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Assets
{
    public struct RayPolygonEdgeIntersection
    {
        public int Index;
        public Vector2 Location;
        public float DistanceFromOrigin;

        public RayPolygonEdgeIntersection(int index, Vector2 location, float distanceFromOrigin) : this()
        {
            Index = index;
            Location = location;
            DistanceFromOrigin = distanceFromOrigin;
        }
    }
    public struct RayPolygonIntersection
    {
        public RayPolygonEdgeIntersection Enter;
        public RayPolygonEdgeIntersection Exit;

        public RayPolygonIntersection(RayPolygonEdgeIntersection enter, RayPolygonEdgeIntersection exit)
        {
            Enter = enter;
            Exit = exit;
        }
    }

    public abstract class DestructablePolygon : MonoBehaviour
    {
        public float Scale = 0.2f;
        public float _holeWidth = 0.2f;
        public float HoleDepth = 10f;
        public int BestCollide;
        public int LeftEnter;
        public int LeftExit;
        public int RightEnter;
        public int RightExit;
        public DestructablePolygon PolygonPrefab;
        public Bounds2D Bounds = new Bounds2D();

        // Update is called once per frame

        public bool RayIntersectBBox(Ray ray)
        {
            return Bounds.CollidesWithRay(Point(transform.position), ray);
        }

        public bool TestForIntersections(Ray ray,Vector2 RayOrigin,bool ShouldBeRightFromOrigin, out RayPolygonIntersection intersection)
        {
            var collides = new List<KeyValuePair<int, Vector2>>();
            Vector2 col;
            if (LineCollisionStuff.RayCollidesWithSegment(ray, Points[Points.Count-1], Points[0], out col))
                collides.Add(new KeyValuePair<int, Vector2>(0, col));
            //Find all lines colliding with ray, should be either 2 or zero
            for (var i = 1; i < Points.Count; i += 1)
            {
                if (LineCollisionStuff.RayCollidesWithSegment(ray, Points[i - 1], Points[i], out col))
                    collides.Add(new KeyValuePair<int, Vector2>(i, col));
            }
            if (collides.Count >= 2) // //@Design, change to == to only support convex polygons
            {
                var collidesWithDistance = collides
                    .Select(x => new RayPolygonEdgeIntersection(x.Key,x.Value, (x.Value - RayOrigin).sqrMagnitude));
                var orderedByDistance = collidesWithDistance.OrderBy(x => x.DistanceFromOrigin);
                var enter = orderedByDistance.First();
                var exit = orderedByDistance.ElementAt(1);

                //@Design, include disttorigin2 in struct, it is recreated to check if holedepth is greater than diff
                // Alternatively do the depth check here and add a boolean ("Goes through")
                intersection = new RayPolygonIntersection(enter, exit);
                return true;
            }

            intersection = new RayPolygonIntersection();
            return false;

        }

        //public void Shoot(bool debug)
        //{
        //    try
        //    { 
        //        var mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //        var player = GameObject.Find("Player");
        //        var delta = mouseWorld - player.transform.position;
        //        var angle = Mathf.Atan2(delta.y, delta.x);
        //        float _;

        //        var collides = new List<KeyValuePair<int, Vector2>>();
        //        for (var i = 1; i < Points.Count; i += 1)
        //        {
        //            Vector2 col;
        //            if (SegmentIntersectSegment(
        //                player.transform.position,
        //                Point(player.transform.position) + LengthDir(10000, angle),
        //                Points[i - 1],
        //                Points[i], out _, out col))
        //            {
        //                collides.Add(new KeyValuePair<int, Vector2>(i, col));
        //            }
        //        }
        //        if (collides.Any())
        //        {
        //            var bestCollide = new KeyValuePair<int, Vector2>();
        //            var minDist = float.MaxValue;
        //            foreach (var collide in collides)
        //            {
        //                var distFromPlayer =
        //                    (player.transform.position - new Vector3(collide.Value.x, collide.Value.y)).sqrMagnitude;
        //                if (distFromPlayer < minDist)
        //                {
        //                    minDist = distFromPlayer;
        //                    bestCollide = collide;
        //                }
        //            }

        //            var i = bestCollide.Key;
        //            var holePos = bestCollide.Value + LengthDir(HoleDepth, angle);
        //            var old = Points[i];
        //            var leftDir = angle + Mathf.PI - _holeWidth;
        //            var rightDir = angle + Mathf.PI + _holeWidth;
        //            var leftGapStartEnd = holePos + LengthDir(HoleDepth * 100, leftDir);
        //            var rightGapStartEnd = holePos + LengthDir(HoleDepth * 100, rightDir);
        //            Vector2 leftGapStart;
        //            Vector2 rightGapStart;
        //            Vector2 leftGapExit = new Vector2();
        //            Vector2 rightGapExit = new Vector2();
        //            bool wholeWentThrough = false;
        //            var exitLineIndex = 0;
        //            var leftExit = 0;
        //            var rightExit = 0;
        //            var leftEnter = 0;
        //            var rightEnter = 0;
        //            var totalPoints = Points.Count;


        //            var indexToRemove = new List<int>();


        //            // If holedepth is less than distance towards a line on the other side, make hole go through
        //            {
        //                foreach(var collide in collides.Where( x => x.Key != bestCollide.Key))
        //                {
        //                    var dist = (bestCollide.Value - collide.Value).magnitude;
        //                    if(dist <= HoleDepth)
        //                    {
        //                        SegmentIntersectSegment(holePos, leftGapStartEnd, Points[collide.Key - 1], Points[collide.Key], out _, out leftGapExit);
        //                        SegmentIntersectSegment(holePos, rightGapStartEnd, Points[collide.Key - 1], Points[collide.Key], out _, out rightGapExit);
        //                        wholeWentThrough = true;
        //                        indexToRemove.Add(collide.Key-1);
        //                        leftExit = rightExit = collide.Key;

        //                        break;
        //                    }
        //                }
        //            }
        //            var found = false;
        //            if (!SegmentIntersectSegment(holePos, leftGapStartEnd, Points[i - 1], Points[i], out _, out leftGapStart))
        //            {
        //                int ii;
        //                for (ii = i - 1; ; ii--)
        //                {
        //                    if (ii == 0)
        //                        ii = Points.Count - 1;

        //                    indexToRemove.Add(ii);
        //                    if (SegmentIntersectSegment(holePos, leftGapStartEnd, Points[ii - 1], Points[ii], out _,
        //                        out leftGapStart))
        //                    {

        //                        leftEnter = ii;
        //                        found = true;
        //                        break;
        //                    }
        //                    // We have gone around
        //                    if (ii == i)
        //                        return;
        //                }
        //                if (!found)
        //                    return;
        //                //if (i < ii)
        //                //    indexToRemove.AddRange(Enumerable.Range(i, ii - i));
        //                //else
        //                //    indexToRemove.AddRange(Enumerable.Range(ii, i - ii));
        //            }
        //            else
        //            {
        //                leftEnter = i;
        //            }
        //            if (
        //                !SegmentIntersectSegment(holePos, rightGapStartEnd, Points[i - 1], Points[i], out _,
        //                    out rightGapStart))
        //            {
        //                int ii;
        //                for (ii = i + 1; ii != i; ii++)
        //                {
        //                    if (ii == Points.Count)
        //                        ii = 1;
        //                    indexToRemove.Add(ii-1);
        //                    if (SegmentIntersectSegment(holePos, rightGapStartEnd, Points[ii - 1], Points[ii], out _,
        //                        out rightGapStart))
        //                    {
        //                        rightEnter = ii;
        //                        found = true;
        //                        break;
        //                    }

        //                    // We have gone around
        //                    if (ii == i)
        //                        return;
        //                }
        //                if (!found)
        //                    return;
        //                //if (i < ii)
        //                //    indexToRemove.AddRange(Enumerable.Range(i, ii - i));
        //                //else
        //                //    indexToRemove.AddRange(Enumerable.Range(ii, i - ii));
        //            }
        //            else
        //            {
        //                rightEnter = i;
        //            }

        //            if (debug)
        //            {
        //                LeftEnter = leftEnter;
        //                LeftExit = leftExit;
        //                RightEnter = rightEnter;
        //                RightExit = rightExit;
        //                BestCollide = i;
        //                GameObject.Find("Debugg").transform.position = new Vector3(holePos.x, holePos.y,
        //                    transform.position.z);
        //                GameObject.Find("Debugg2").transform.position = new Vector3(leftGapStart.x, leftGapStart.y,
        //                    transform.position.z);
        //                GameObject.Find("Debugg3").transform.position = new Vector3(rightGapStart.x, rightGapStart.y,
        //                    transform.position.z);


        //                foreach (var debugObject in toRemoveDebugObjects)
        //                {
        //                    debugObject.SetActive(false);
        //                }
        //                for (var toAdd = toRemoveDebugObjects.Count; toAdd < indexToRemove.Count; toAdd++)
        //                {
        //                    toRemoveDebugObjects.Add(Instantiate(toRemoveDebugPrefab));
        //                }
        //                var obj = 0;
        //                foreach (var index in indexToRemove)
        //                {
        //                    var point = Points[index];
        //                    toRemoveDebugObjects[obj].transform.position = point;
        //                    toRemoveDebugObjects[obj].SetActive(true);
        //                    obj++;
        //                }

        //                if(wholeWentThrough)
        //                {
        //                    toRemoveDebugObjects[obj].transform.position = leftGapExit;
        //                    toRemoveDebugObjects[obj].SetActive(true);
        //                    toRemoveDebugObjects[obj+1].transform.position = rightGapExit;
        //                    toRemoveDebugObjects[obj+1].SetActive(true);
        //                }
        //            }
        //            else
        //            {
        //                if (wholeWentThrough)
        //                {
        //                    var polygon = Instantiate(PolygonPrefab);
        //                    //Right polygon
        //                    var poly1 = CreatePolygon(rightEnter, rightExit, totalPoints, rightGapStart, rightGapExit)
        //                        .ToList();
        //                    var poly2 = CreatePolygon(leftExit, leftEnter, totalPoints, leftGapExit, leftGapStart)
        //                        .ToList();
        //                    poly1.Add(poly1.First());
        //                    poly2.Add(poly2.First());
        //                    DrawLines.PointsPerPolygon[this] = poly1;
        //                    DrawLines.PointsPerPolygon[polygon] = poly2;
        //                    this.Bounds.UpdateFromPoints(poly1);
        //                    polygon.Bounds.UpdateFromPoints(poly2);
        //                }
        //                else
        //                {
                        

        //                    MakeHole(indexToRemove.Distinct(), i, holePos, leftGapStart, rightGapStart);
        //                }
        //            }
        //        }
        //    }
        //    catch(Exception e)
        //    {

        //    }

        //}

        private void MakeHole(IEnumerable<int> indexToRemove, int enterIndex, Vector2 holePos, Vector2 leftGapStart, Vector2 rightGapStart)
        {
            // Remove points
            indexToRemove = indexToRemove.ToList();
            var removed = 0;
            foreach (var index in indexToRemove.OrderBy(x => x))
            {
                DrawLines.PointsPerPolygon[this].RemoveAt(index - removed);
                removed++;
            }

            // Insert new points
            int startIndex;
            if (indexToRemove.Any())
                startIndex = indexToRemove.Min();
            else
                startIndex = enterIndex;

            InsertPoint(startIndex, new Vector3(leftGapStart.x, leftGapStart.y));
            InsertPoint(startIndex + 1, new Vector3(holePos.x, holePos.y));
            InsertPoint(startIndex + 2, new Vector3(rightGapStart.x, rightGapStart.y));
        }



        private List<Vector2> CreatePolygon(int start, int stop, int totalPoints, Vector2 rightGapStart, Vector2 rightGapExit)
        {
            List<int> poly1;
            if (start > stop)
            {
                poly1 = PolygonIterating.RangeFromTo(start, totalPoints - 1)
                    .Concat(PolygonIterating.RangeFromTo(0, stop - 1)).ToList();
            }
            else
            {
                poly1 = PolygonIterating.RangeFromTo(start, stop - 1).ToList();
            }
            var polPoints = poly1.Select(index => Points[index]).ToList();
            var poly1Points = new List<Vector2>();
            poly1Points.Add(rightGapStart);
            poly1Points.AddRange(polPoints);
            poly1Points.Add(rightGapExit);
            return poly1Points;
        }

        public List<GameObject> toRemoveDebugObjects = new List<GameObject>();
        public GameObject toRemoveDebugPrefab;
        public float currentDepth;

        //void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.R))
        //    {
        //        Init();
        //    }
        //    if (Input.GetMouseButtonDown(0))
        //        Shoot(false);
        //    else
        //    {
        //        Shoot(true);
        //    }
        //}

        public abstract void Init();

        private Vector2 LengthDir(float length, float angle)
        {
            return new Vector2(
                Mathf.Cos(angle) * length,
                Mathf.Sin(angle) * length);
        }


        static float Signed2DTriArea(Vector2 a, Vector2 b, Vector2 c)
        {
            return (a.x - c.x) * (b.y - c.y) - (a.y - c.y) * (b.x - c.x);
        }

        public static bool SegmentIntersectSegment(Vector2 a, Vector2 b, Vector2 c, Vector2 d, out float t, out Vector2 p)
        {
            // signs of areas correspond to which side of ab points c and d are
            var a1 = Signed2DTriArea(a, b, d); // Compute winding of abd (+ or -)
            var a2 = Signed2DTriArea(a, b, c); // To intersect, must have sign opposite of a1

            // If c and d are on different sides of ab, areas have different signs
            if (a1 * a2 < 0.0f) // require unsigned x & y values.
            {
                var a3 = Signed2DTriArea(c, d, a); // Compute winding of cda (+ or -)
                var a4 = a3 + a2 - a1; // Since area is constant a1 - a2 = a3 - a4, or a4 = a3 + a2 - a1

                // Points a and b on different sides of cd if areas have different signs
                if (a3 * a4 < 0.0f)
                {
                    // Segments intersect. Find intersection point along L(t) = a + t * (b - a).
                    t = a3 / (a3 - a4);
                    p = a + t * (b - a); // the point of intersection
                    return true;
                }
            }
            t = 0;
            p = new Vector2();
            // Segments not intersecting or collinear
            return false;
        }

        private List<Vector2> Points
        {
            get
            {
                return DrawLines.PointsPerPolygon[this]
                    .Select(x => Point(transform.position) + x).ToList();
            }
            //set { DrawLines.PointsPerPolygon[this] = 
            //        value
            //        .Select(x => new Vector3(x))
            //        .ToList(); }
        }

        public Vector2 Point(Vector3 vec)
        {
            return new Vector2(vec.x, vec.y);
        }

        public void InsertPoint(int index, Vector2 vec2)
        {
            DrawLines.PointsPerPolygon[this].Insert(index, ((new Vector3(vec2.x, vec2.y)) - transform.position));
        }

        public void Shoot(Ray ray,RayPolygonIntersection intersection, bool debug)
        {
            var enterInterSection = intersection.Enter;
            var angle = Mathf.Atan2(ray.Direction.y, ray.Direction.x);
            var holePos = enterInterSection.Location + LengthDir(HoleDepth, angle);
            var leftDir = angle + Mathf.PI - _holeWidth;
            var rightDir = angle + Mathf.PI + _holeWidth;
            var leftGapRay = Ray.FromLine(holePos, holePos + LengthDir(HoleDepth * 100, leftDir));
            var rightGapRay = Ray.FromLine(holePos, holePos + LengthDir(HoleDepth * 100, rightDir));

            DrawLines.DrawDebugLine(holePos, leftGapRay.Line.End, Color.gray);
            DrawLines.DrawDebugLine(holePos, rightGapRay.Line.End, Color.gray);
            // Go through all possible lines starting from the line we collided with and by going clockwise (negative1)
            // Until we find a line to collide with, this is leftEnter

            Vector2 leftGapPoint;
            if (Input.GetMouseButtonDown(0))
            {
                int x = 4;
            }
            int leftGapLineColIndex;
            if(!SearchForLineCollision(enterInterSection.Index,-1,leftGapRay, out leftGapPoint,out leftGapLineColIndex))
                leftGapLineColIndex = enterInterSection.Index;

            Vector2 rightGapPoint;
            int rightGapLineColIndex;
            if (!SearchForLineCollision(enterInterSection.Index, 1, rightGapRay, out rightGapPoint, out rightGapLineColIndex))
                rightGapLineColIndex = enterInterSection.Index;

            //if(leftGapPoint.x == 0 && leftGapPoint.y == 0 && rightGapPoint.x == 0 && rightGapPoint.y == 0)
            //    throw new Exception("DAR");

            //var ToRemoveForLeftGap = PolygonIterating.RangeRoundReverse(enterInterSection.Index, leftGapLineColIndex , Points.Count).ToList();
            //var ToRemoveForRightGap = PolygonIterating.RangeRound(enterInterSection.Index, rightGapLineColIndex, Points.Count).ToList();

            var toRemoveFinal = PolygonIterating.RangeRound(leftGapLineColIndex-1, rightGapLineColIndex, Points.Count);

            foreach (var index in toRemoveFinal)
            {
                //@Design, currently draws an extra line for [points.count-2...points.count-1]
                var i = index;
                if (i == 0)
                    i = Points.Count -1;
                DrawLines.DrawDebugLine(Points[i - 1 ], Points[i],Color.blue);
            }
            debug = false;
            currentDepth = (enterInterSection.Location - intersection.Exit.Location).magnitude;
            if ((enterInterSection.Location - intersection.Exit.Location).magnitude < HoleDepth)
            {
                Vector2 leftExitPoint;
                int leftExitIndex;
                if (!SearchForLineCollision(intersection.Exit.Index, 1, leftGapRay, out leftExitPoint, out leftExitIndex))
                    leftExitIndex = intersection.Exit.Index;

                Vector2 rightExitPoint;
                int rightExitIndex;
                if (!SearchForLineCollision(intersection.Exit.Index, -1, rightGapRay, out rightExitPoint, out rightExitIndex))
                    rightExitIndex = intersection.Exit.Index;


                DrawLines.DrawDebugLine(holePos, leftGapPoint, Color.red);
                DrawLines.DrawDebugLine(holePos, rightGapPoint, Color.red);
                DrawLines.DrawDebugLine(leftGapPoint, leftExitPoint, Color.yellow);
                DrawLines.DrawDebugLine(rightGapPoint, rightExitPoint, Color.yellow);
                //// Hole went through
                //var polygon = Instantiate(PolygonPrefab);
                ////Right polygon
                //var poly1 = CreatePolygon(rightGapLineColIndex, rightExit, totalPoints, rightGapStart, rightGapExit)
                //    .ToList();
                //var poly2 = CreatePolygon(leftGapLineColIndex, leftEnter, totalPoints, leftGapExit, leftGapStart)
                //    .ToList();
                //poly1.Add(poly1.First());
                //poly2.Add(poly2.First());
                //DrawLines.PointsPerPolygon[this] = poly1;
                //DrawLines.PointsPerPolygon[polygon] = poly2;
                //this.Bounds.UpdateFromPoints(poly1);
                //polygon.Bounds.UpdateFromPoints(poly2);
            }
            else
            {
                DrawLines.DrawDebugLine(holePos, leftGapPoint, Color.red);
                DrawLines.DrawDebugLine(holePos, rightGapPoint, Color.red);
                if (Input.GetMouseButtonDown(0))
                {
                    MakeHole(toRemoveFinal.Distinct().ToList(),
                    enterInterSection.Index,
                    holePos, leftGapPoint, rightGapPoint);
                }
            }

        }


        public bool SearchForLineCollision(int indexToStartFrom, int direction, Ray ray,
            out Vector2 intersectionPoint,
            out int intersectionIndex)
        {
            {
                var start = indexToStartFrom - 1;
                var stop = indexToStartFrom;

                if (stop == 0 || stop == Points.Count)
                {
                    start = Points.Count - 1;
                    stop = 0;
                }
                var line = new Line(Points[start], Points[stop]);
                if (ray.CollidesWithSegment(line, out intersectionPoint))
                {
                    intersectionIndex = stop;
                    return true;
                }
            }


            var x = 0;
            for (var i = indexToStartFrom + direction; ; )
            {
                if (x++ > 10000)
                {
                    throw new Exception();
                }
                var start = i - 1;
                var stop = i;
                if (i == 0 || i == Points.Count) //Todo: gets -2
                {
                    start = Points.Count -1;
                    stop = 0;
                }
                var line = new Line(Points[start], Points[stop]);
                if (ray.CollidesWithSegment(line, out intersectionPoint))
                {
                    intersectionIndex = i;
                    return true;
                }


                if (i == 0 && direction == -1)
                {
                    i = Points.Count - 1;
                    if (indexToStartFrom == 0)
                    {
                        intersectionPoint = new Vector2();
                        intersectionIndex = 0;
                        return false;
                    }
                }
                else if (i == Points.Count)
                {
                    i = 1;
                    if (indexToStartFrom == 0)
                    {
                        intersectionPoint = new Vector2();
                        intersectionIndex = 0;
                        return false;
                    }
                }
                else
                {
                    i += direction;
                }

                if (i == indexToStartFrom)
                {
                    intersectionPoint = new Vector2();
                    intersectionIndex = 0;
                    return false;
                }
            }
        }
    }
}