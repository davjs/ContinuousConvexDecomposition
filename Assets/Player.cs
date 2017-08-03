using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets
{
    public class Player : MonoBehaviour
    {
        public int Intersections = 0;
        // Use this for initialization
        void Start () {
	
        }

        // Update is called once per frame
        void Update ()
        {
            var point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.eulerAngles = new Vector3(0,0,0);
            transform.Translate(new Vector3(Input.GetAxis("Horizontal")/10, Input.GetAxis("Vertical")/10));
            transform.LookAt(new Vector3(point.x, point.y, transform.position.z));

            if (Input.GetKeyDown(KeyCode.R))
            {
            }
           
            var polygons = Object.FindObjectsOfType<DestructablePolygon>();
            var mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var player = GameObject.Find("Player");
            var playerPos = player.transform.position;
            var ray = Ray.FromLine(new Vector2(playerPos.x, playerPos.y), new Vector2(mouseWorld.x, mouseWorld.y));
            var mouseIsRightOfPlayer = playerPos.x > mouseWorld.x;
            var intersections = new Dictionary<DestructablePolygon,RayPolygonIntersection>();
            foreach (var polygon in polygons)
            {
                RayPolygonIntersection intersection;
                if (polygon.TestForIntersections(ray, playerPos, mouseIsRightOfPlayer, out intersection))
                {
                    intersections[polygon] = intersection;
                }
            }

            Intersections = intersections.Count;
            if (intersections.Any())
            {
                var closestIntersection = intersections.OrderBy(x => x.Value.Enter.DistanceFromOrigin).Last();
                DrawLines.DrawDebugLine(playerPos, closestIntersection.Value.Enter.Location,Color.green);
                DrawLines.DrawDebugLine(closestIntersection.Value.Enter.Location, closestIntersection.Value.Exit.Location, Color.blue);
                closestIntersection.Key.Shoot(ray,closestIntersection.Value,true);
            }
            //var intersectingBox = polygons.Where(polygon => polygon.RayIntersectBBox(ray));
            //var distToBestBoxSquared = float.MaxValue;
            //DestructablePolygon closestPolygon = null;
            //foreach (var boxIntersected in intersectingBox)
            //{
            //    var dist = (boxIntersected.transform.position - player.transform.position).magnitude;
            //    if (dist < distToBestBoxSquared)
            //    {
            //        distToBestBoxSquared = dist;
            //        closestPolygon = boxIntersected;
            //    }
            //}
            //if (closestPolygon != null)
            //{
            //    closestPolygon.Shoot(!Input.GetMouseButtonDown(0));
            //}

        }
    }
}
