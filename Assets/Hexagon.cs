using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets
{
    public class Hexagon : DestructablePolygon
    {

        // Use this for initialization

        void Start()
        {
            Init();
        }

        public override void Init()
        {
            var points = new List<Vector2>()
            {
                new Vector2(-5, 0) * Scale,
                new Vector2(0, -5) * Scale,
                new Vector2(5, 0) * Scale,
                new Vector2(3, 5) * Scale,
                new Vector2(-3, 5) * Scale,
                new Vector2(-5, 0) * Scale,
            };
            DrawLines.PointsPerPolygon.Add(this, points);
            if (!DrawLines.PointsPerPolygon.ContainsKey(this))
                DrawLines.PointsPerPolygon.Add(this, points);
            else
                DrawLines.PointsPerPolygon[this] = points;
        }
    }
}