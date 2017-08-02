using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets
{
    public class PolygonWithSpike : DestructablePolygon
    {
        public int Steps = 3;
        public float Width = 0.3f;
        // Use this for initialization
        void Start ()
        {
            Init();
        }

        public override void Init()
        {
            var points = new List<Vector2>();
            points.Add(new Vector3(40, 0) * Scale);

            for (var i = 0; i < Steps + 1; i++)
            {
                points.Add(new Vector3((Steps - i) * Width, 0 + i) * Scale);
                points.Add(new Vector3((Steps - i) * Width, 1 + i) * Scale);
            }

            for (var i = 0; i < Steps + 1; i++)
            {
                points.Add(new Vector3((0 - i) * Width, Steps - i + 1) * Scale);
                points.Add(new Vector3((0 - i) * Width, Steps - i) * Scale);
            }


            points.Add(new Vector2(-40, 0) * Scale);
            points.Add(new Vector2(-40, -40) * Scale);
            points.Add(new Vector2(40, -40) * Scale);
            if(!DrawLines.PointsPerPolygon.ContainsKey(this))
                DrawLines.PointsPerPolygon.Add(this, points);
            else
                DrawLines.PointsPerPolygon[this] = points;
            Bounds.UpdateFromPoints(points);
        }

        // Update is called once per frame
        //void Update () {

        //}
    }
}
