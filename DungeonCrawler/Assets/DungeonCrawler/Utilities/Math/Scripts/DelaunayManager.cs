using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
namespace DungeonCrawler.Utilities.Math
{
    public class DelaunayManager : MonoBehaviour
    {
        private static DelaunayManager _instance;

        public static DelaunayManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<DelaunayManager>();
                return _instance;
            }
        }

        [HideInInspector] public bool delaunayGenerated = false;

        public Transform incrementalContainer;
        public Transform flipContainer;
        public Transform updateContainer;

        public LineRenderer lrPrefab;
        public LineRenderer lrFlipPrefab;
        public LineRenderer lrEdgePrefab;
        private List<Point> sortedPoints;

        private LineRenderer t1LR;
        private LineRenderer t2LR;
        private LineRenderer edgeLR;

        private List<LineRenderer> triangleLineRenderers = new List<LineRenderer>();

        public bool displayProcess = false;
        public bool debugRendering = false;
        public bool debugAuto = false;
        public float autoSpeed = 0.2f;

        private bool triangulating = false;

        private Action<Graph> _onFlipFinished = null;

        public void FlipStart(Graph graph, Action<Graph> callback)
        {
            _onFlipFinished = callback;
            
            if (debugRendering)
            {
                if (!triangulating)
                {
                    incrementalContainer.gameObject.SetActive(false);
                    triangulating = true;
                    for (int i = 0; i < graph.triangles.Count; i++)
                    {
                        triangleLineRenderers.Add(Instantiate(lrPrefab, Vector3.zero, Quaternion.identity, flipContainer));
                    }

                    StartCoroutine(FlippingEdgesStepByStep(graph));
                }
            }
            else
            {
                FlippingEdges(graph);
            }
        }

        public void ResetManager()
        {
            if(sortedPoints != null) sortedPoints.Clear();
            foreach (var tr in triangleLineRenderers)
            {
                Destroy(tr.gameObject);
            }

            ClearFlipDisplay();
            for (int i = 0; i < updateContainer.childCount; i++)
            {
                Destroy(updateContainer.GetChild(i).gameObject);
            }
        }
        public void IncrementalTriangulation(Graph graph)
        {
            if (graph.points == null) return;
            if (graph.points.Count <= 0) return;

            //1 - Les points sont triés par abscisse croissante
            sortedPoints = sortPoints(graph.points);

            //2 - On initialise le processus en calculant une 2-triangulation initiale
            //2a -On construit une suite de k - 1 arêtes colinéaires [P1,P2],..., [Pk-1, Pk] avec les k(>=2) points alignés
            float initialX = sortedPoints[0].Position.x;
            List<Point> pointsPassed = new List<Point> { sortedPoints[0] };
            for (int i = 1; i < sortedPoints.Count; i++)
            {
                if (System.Math.Abs(sortedPoints[i].Position.x - initialX) < 0.001)
                {
                    pointsPassed.Add(sortedPoints[i]);
                }
                else
                {
                    break;
                }
            }

            //2b - Avec le point Pk+1, on construit la triangulation initiale Tk+1 dont le bord est le polygone triangulaire POk+1 = [P1P2...Pk+1]
            int k = pointsPassed.Count;
            if (k >= 2)
            {
                Point kplus1 = sortedPoints[k + 1];
                for (int i = 0; i < k; i++)
                {
                    Triangulate(graph, sortedPoints[i], sortedPoints[i + 1], kplus1);
                }

                pointsPassed.Add(kplus1);
            }
            else
            {
                Triangulate(graph,sortedPoints[0], sortedPoints[1], sortedPoints[2]);
                pointsPassed = new List<Point>() { sortedPoints[0], sortedPoints[1], sortedPoints[2] };
            }

            //3 - A l'étape q, k+1<=q<n, on ajoute le point Pq+1 à la triangulation Tq afin d'obtenir la triangulation Tq+1
            // Le point Pq+1 n'appartenant pas au polygone POq, on va rechercher les arêtes de POq vues par Pq+1
            // puis construire les nouveaux triangles à partir de celles-ci et du Point Pq+1
            //3a - On recherche les arêtes de Pq "vues" depuis Pq+1 : une arête A est vue de Pq+1 si et seulement si 
            //le point Pq+1 appartient à int(E_), E_ étant le demi-espace délimité par A et ne contenant pas Pq+1
            //Avec un produit scalaire, on relie les points si l'angle est aigu
            Point pqplus1 = new Point();
            for (int i = pointsPassed.Count; i < sortedPoints.Count; i++)
            {
                pqplus1 = sortedPoints[i];
                List<Point> polygon = new List<Point>();
                polygon = GrahamScanStatic.Compute(pointsPassed);

                for (int p = 0; p < polygon.Count; p++)
                {
                    int indexNext = p + 1 < polygon.Count ? p + 1 : 0;
                    Vector3 segment = polygon[indexNext].Position - polygon[p].Position;
                    Vector3 normal = Vector3.Cross(segment.normalized, -Vector3.forward);
                    Vector3 vectorToPoint = pqplus1.Position - polygon[p].Position;
                    float dotProduct = Vector3.Dot(normal.normalized, vectorToPoint);
                    if (dotProduct > 0)
                    {
                        pointsPassed.Add(pqplus1);
                        Triangulate(graph, polygon[p], polygon[indexNext], pqplus1);
                    }
                }
            }

            if(displayProcess)DisplayIncrementation(graph, new Color(0.3f, 0.3f, 0.3f), incrementalContainer);
        }

        private List<Point> sortPoints(List<Point> points)
        {
            Point[] list = new Point[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                list[i] = points[i];
            }

            Array.Sort(list);
            return list.ToList();
        }

        private void FlippingEdges(Graph graph)
        {
            int count = 0;
            List<Edge> Ac = graph.edges;
            Edge A = new Edge();
            Edge A1 = new Edge();
            Edge A2 = new Edge();
            Edge A3 = new Edge();
            Edge A4 = new Edge();
            int T1 = 0;
            int T2 = 0;
            Point S3 = new Point();
            Point S4 = new Point();
            bool testTriangle = true;
            while (Ac.Count > 0 && count < 1000)
            {
                A = Ac[0];
                Ac.Remove(A);
                (T1, T2) = A.BelongsToTriangles(graph.triangles);
                if (T1 >= 0 && T2 >= 0 && !graph.triangles[T1].VerifyDelaunayCriteria(graph.triangles[T2].GetLastVertex(A).Position))
                {
                    S3 = graph.triangles[T1].GetLastVertex(A);
                    S4 = graph.triangles[T2].GetLastVertex(A);

                    (A4, A1) = graph.triangles[T1].GetOtherEdges(A);
                    (A3, A2) = graph.triangles[T2].GetOtherEdges(A);

                    A = new Edge(S3, S4);
                    graph.triangles[T1].SetEdges(A, A1, A2);
                    graph.triangles[T2].SetEdges(A, A3, A4);

                    Ac.Add(A1);
                    Ac.Add(A2);
                    Ac.Add(A3);
                    Ac.Add(A4);
                }

                count++;
            }

            if(displayProcess)DisplayIncrementation(graph, new Color(0f, 1f, 0f), flipContainer);
            delaunayGenerated = true;
            _onFlipFinished?.Invoke(graph);
        }


        public IEnumerator FlippingEdgesStepByStep(Graph graph)
        {
            WaitForSeconds wait = new WaitForSeconds(autoSpeed);
            WaitUntil waitKey = new WaitUntil(() => Input.GetKeyDown(KeyCode.A));
            int count = 0;
            List<Edge> Ac = graph.edges;
            Edge A = new Edge();
            Edge A1 = new Edge();
            Edge A2 = new Edge();
            Edge A3 = new Edge();
            Edge A4 = new Edge();
            int T1 = 0;
            int T2 = 0;
            Point S1 = new Point();
            Point S2 = new Point();
            Point S3 = new Point();
            Point S4 = new Point();

            while (Ac.Count > 0 && count < 1000)
            {
                A = Ac[0];
                Ac.Remove(A);
                (T1, T2) = A.BelongsToTriangles(graph.triangles);
                bool flipped = false;
                if (T1 >= 0 && T2 >= 0)
                {
                    //DisplayFlip(triangles[T1], triangles[T2], A, Color.blue);
                    //DebugDisplayIncrementation(triangles, new Color(0.3f,0.3f,0.3f));
                    if (!debugAuto) yield return waitKey;
                    if (!graph.triangles[T1].VerifyDelaunayCriteria(graph.triangles[T2].GetLastVertex(A).Position))
                    {
                        S3 = graph.triangles[T1].GetLastVertex(A);
                        S4 = graph.triangles[T2].GetLastVertex(A);

                        (A4, A1) = graph.triangles[T1].GetOtherEdges(A);
                        (A3, A2) = graph.triangles[T2].GetOtherEdges(A);

                        A = new Edge(S3, S4);
                        graph.triangles[T1].SetEdges(A, A1, A2);
                        graph.triangles[T2].SetEdges(A, A3, A4);

                        Ac.Add(A1);
                        Ac.Add(A2);
                        Ac.Add(A3);
                        Ac.Add(A4);
                        flipped = true;
                    }

                    Color col = flipped ? Color.red : Color.green;
                    if(displayProcess)DisplayFlip(graph.triangles[T1], graph.triangles[T2], A, col);
                    if(displayProcess)DebugDisplayIncrementation(graph, new Color(0.3f, 0.3f, 0.3f));

                    if (debugAuto) yield return wait;
                    else yield return waitKey;
                }

                count++;
            }

            if(displayProcess)DisplayIncrementation(graph, new Color(0f, 1f, 0f), flipContainer);
            ClearFlipDisplay();
            triangulating = false;
            delaunayGenerated = true;

            _onFlipFinished?.Invoke(graph);
        }

        private List<Edge> GetFlippedEdges(Graph graph)
        {
            HashSet<Edge> flippedEdges = new HashSet<Edge>();
            foreach (var tri in graph.triangles)
            {
                foreach (var edge in tri.Edges)
                {
                    flippedEdges.Add(edge);
                }
            }

            return flippedEdges.ToList();
        }

        private void ClearFlipDisplay()
        {
            Destroy(t1LR);
            Destroy(t2LR);
            Destroy(edgeLR);
        }

        private void DisplayFlip(Triangle T1, Triangle T2, Edge A, Color color)
        {
            if (t1LR == null) t1LR = Instantiate(lrFlipPrefab, Vector3.zero, Quaternion.identity);
            if (t2LR == null) t2LR = Instantiate(lrFlipPrefab, Vector3.zero, Quaternion.identity);
            if (edgeLR == null) edgeLR = Instantiate(lrEdgePrefab, Vector3.zero, Quaternion.identity);
            t1LR.positionCount = 3;
            t1LR.SetPosition(0, T1.Edges[0].firstPoint.Position);
            t1LR.SetPosition(1, T1.Edges[1].firstPoint.Position);
            t1LR.SetPosition(2, T1.Edges[2].firstPoint.Position);
            t2LR.positionCount = 3;
            t2LR.SetPosition(0, T2.Edges[0].firstPoint.Position);
            t2LR.SetPosition(1, T2.Edges[1].firstPoint.Position);
            t2LR.SetPosition(2, T2.Edges[2].firstPoint.Position);
            edgeLR.positionCount = 2;
            edgeLR.startColor = color;
            edgeLR.endColor = color;
            Debug.Log("Edge A : " + A.firstPoint.Position + " , " + A.secondPoint.Position);
            edgeLR.SetPosition(0, A.firstPoint.Position);
            edgeLR.SetPosition(1, A.secondPoint.Position);
        }

        private void DebugDisplayIncrementation(Graph graph, Color color)
        {
            for (int i = 0; i < graph.triangles.Count; i++)
            {
                LineRenderer lr = triangleLineRenderers[i]; //Instantiate(lrPrefab, Vector3.zero, Quaternion.identity, flipContainer);
                lr.startColor = color;
                lr.endColor = color;
                lr.positionCount = 3;
                graph.triangles[i].DisplayTriangle(ref lr);
            }
        }

        private void DisplayIncrementation(Graph graph, Color color, Transform container)
        {
            for (int i = 0; i < graph.triangles.Count; i++)
            {
                LineRenderer lr = Instantiate(lrPrefab, Vector3.zero, Quaternion.identity, container);
                lr.startColor = color;
                lr.endColor = color;
                lr.positionCount = 3;
                graph.triangles[i].DisplayTriangle(ref lr);
            }
        }

        private void Triangulate(Graph graph, Point a, Point b, Point c)
        {
            Edge edge1 = new Edge(a, b);
            Edge edge2 = new Edge(b, c);
            Edge edge3 = new Edge(c, a);
            if (!graph.edges.Contains(edge1)) graph.edges.Add(edge1);
            if (!graph.edges.Contains(edge2)) graph.edges.Add(edge2);
            if (!graph.edges.Contains(edge3)) graph.edges.Add(edge3);
            graph.triangles.Add(new Triangle(edge1, edge2, edge3));
        }
    }
}