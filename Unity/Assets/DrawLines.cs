using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets
{
    public class DrawLines : MonoBehaviour
    {

        // Fill/drag these in from the editor

        // Choose the Unlit/Color shader in the Material Settings
        // You can change that color, to change the color of the connecting lines
        public Material LineMat;
        public static Dictionary<MonoBehaviour, List<Vector2>> PointsPerPolygon = new Dictionary<MonoBehaviour, List<Vector2>>();

        public static Queue<KeyValuePair<Line,Color>> DebugLinesQueue = new Queue<KeyValuePair<Line, Color>>();


        // Connect all of the `points` to the `mainPoint`
        void DrawConnectingLines()
        {

            if (PointsPerPolygon.Any())
            {
                foreach (var polygon in PointsPerPolygon)
                {
                    if(polygon.Key == null)
                        continue;
                    GL.Begin(GL.LINES);
                    LineMat.SetPass(0);
                    var position = polygon.Key.transform.position;
                    var points = polygon.Value;
                    for (var i = 1; i < points.Count; i++)
                    {
                        GL.Color(new Color(0f, 0f, 0f, 1f));
                        GL.Vertex3(
                            position.x + points[i-1].x,
                            position.y + points[i-1].y,0);
                        GL.Vertex3(
                            position.x + points[i].x,
                            position.y + points[i].y,0);
                    }

                    GL.Vertex3(
                        position.x + points[points.Count-1].x,
                        position.y + points[points.Count-1].y, 0);
                    GL.Vertex3(
                        position.x + points[0].x,
                        position.y + points[0].y, 0);

                    //GL.Color(new Color(0f, 0f, 0f, 1f));
                    //GL.Vertex3(
                    //    position.x + points[points.Count -1].x,
                    //    position.y + points[points.Count - 1].y,
                    //    position.z + points[points.Count - 1].z);
                    //GL.Vertex3(
                    //    position.x + points[0].x,
                    //    position.y + points[0].y,
                    //    position.z + points[0].z);
                    GL.End();
                }


                GL.Begin(GL.LINES);
                foreach (var line in DebugLinesQueue)
                {
                    GL.Color(line.Value);
                    GL.Vertex3(line.Key.Begin.x, line.Key.Begin.y, 0);
                    GL.Vertex3(line.Key.End.x, line.Key.End.y, 0);
                }
                GL.End();
                DebugLinesQueue.Clear();
            }
        }

        public static void DrawDebugLine(Vector2 begin,Vector2 end, Color color)
        {
            DebugLinesQueue.Enqueue(new KeyValuePair<Line, Color>(new Line(begin,end),color));
        }

        // To show the lines in the game window whne it is running
        void OnPostRender()
        {
            DrawConnectingLines();
        }

        // To show the lines in the editor
        void OnDrawGizmos()
        {
            DrawConnectingLines();
        }
    }
}
