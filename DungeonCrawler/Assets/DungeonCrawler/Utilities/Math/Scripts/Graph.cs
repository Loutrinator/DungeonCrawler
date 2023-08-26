using System.Collections.Generic;

namespace DungeonCrawler.Utilities.Math
{
    public class Graph
    {
        public List<Edge> edges = null;
        public List<Point> points = null;
        public List<Triangle> triangles = null;

        public Graph()
        {
            edges = new List<Edge>();
            points = new List<Point>();
            triangles = new List<Triangle>();
        }
    }
}