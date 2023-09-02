using System.Collections.Generic;

namespace DungeonCrawler.Utilities.Math
{
    public class Graph
    {
        public List<Edge> edges = null;

        public List<Point> points
        {
            get
            {
                return points;
            }
            set
            {
                points = value;
                for (int p = 0; p < points.Count; p++)
                {
                    points[p].Index = p;
                }
            }
        }

        public List<Triangle> triangles = null;

        public Graph()
        {
            edges = new List<Edge>();
            points = new List<Point>();
            triangles = new List<Triangle>();
        }

        private void AddEdge(Edge edge)
        {
            edges.Add(edge);
            if (!points.Contains(edge.firstPoint))
            {
                points.Add(edge.firstPoint);
            }
            if (!points.Contains(edge.secondPoint))
            {
                points.Add(edge.secondPoint);
            }
        }


        public Graph GetMinnimumSpanningTree()
        {
            int roomCount = points.Count;
            List<Edge> edgesCopy = edges;
            edgesCopy.Sort((a,b) => a.CompareTo(b));

            Queue<Edge> queue = new Queue<Edge>(edgesCopy);

            PointSubset[] subsets = new PointSubset[roomCount];

            for (int i = 0; i < points.Count; i++)
            {
                subsets[i] = new PointSubset() { Parent = points[i] };
            }

            Graph result = new Graph();

            while (result.edges.Count < roomCount - 1)
            {
                Edge edge = queue.Dequeue();
                Point from = GetRoot(subsets, edge.firstPoint);
                Point to = GetRoot(subsets, edge.secondPoint);

                if (from != to)
                {
                    result.AddEdge(edge);
                    Union(subsets, from, to);
                }
            }

            return result;
        }

        private Point GetRoot(PointSubset[] subsets, Point point)
        {
            if (subsets[point.Index].Parent != point)
            {
                subsets[point.Index].Parent = GetRoot(subsets, subsets[point.Index].Parent);
            }

            return subsets[point.Index].Parent;
        }
        private void Union(PointSubset[] subsets, Point from, Point to)
        {
            if (subsets[from.Index].Rank > subsets[to.Index].Rank)
            {
                subsets[to.Index].Parent = from;
            }
            else if (subsets[from.Index].Rank < subsets[to.Index].Rank)
            {
                subsets[from.Index].Parent = to;
            }
            else
            {
                subsets[to.Index].Parent = from;
                subsets[from.Index].Rank++;
            }
        }
    }
}