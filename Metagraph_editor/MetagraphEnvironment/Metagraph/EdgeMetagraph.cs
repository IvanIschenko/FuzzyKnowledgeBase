using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebApplication1.FuzzyLogicBase;
using C__04._07._2015.Algorithms.Clustering;// progr_V.cs
using L.DataStructures.Matrix;// multidim..cs
using L.Algorithms.Clustering;// k_means.cs
using L.DataStructures.Geometry; // cluster.cs
using WebApplication1;
using System.Windows.Media;

namespace WebApplication1
{
    public class EdgeMetagraph<T, K> : MetaGraph<T, K>
    {
        public List<Edge<T, K>> Edges { get; set; }
        public EdgeMetagraph()
        {
            this.Edges = new List<Edge<T, K>>();
        }

        #region CountIncedentEdgesForVertex Optimisation
        Dictionary<MetaVertex<T>, int> incEdg;

        public void Optimisation()
        {
            this.incEdg = new Dictionary<MetaVertex<T>, int>();

            foreach (var item in Vertices)
            {
                incEdg.Add(item,
                    Edges.FindAll(e => object.ReferenceEquals(e.StartVertex, item)
                    || object.ReferenceEquals(e.EndVertex, item)).Count);
            }


        }
        #endregion

        override public int CountIncedentEdgesForVertex(MetaVertex<T> v)
        {
            return incEdg[v];
        }
        override public List<List<int>> GenerateIncidenceMatrix() { throw new NotImplementedException(); }
        override public List<List<MetaVertex<T>>> GenerateAdjacencyMatrix() { throw new NotImplementedException(); }

        override public void Clear()
        {
            this.Edges.Clear();
            base.Clear();
        }
        override public void MakeFullyMeshed()
        {
            Edges = new List<Edge<T, K>>();

            for (int i = 0; i < Vertices.Count; ++i)
            {
                for (int j = i + 1; j < Vertices.Count; ++j)
                {
                    Edge<T, K> e = new Edge<T, K>("(" + i.ToString() + "," + j.ToString() + ")", Vertices[i],
                        Vertices[j], false, MathHelper.DistanceBetweenVectors(Vertices[i].Coordinates, Vertices[j].Coordinates), 1, Color.FromRgb(0, 40, 255));
                    Edges.Add(e);
                }
            }
        }
        //нахождение ребра между вершинами
        override public Edge<T, K> GetEdgeBetweenVertices(MetaVertex<T> startVertex, MetaVertex<T> endVertex)
        {
            Edge<T, K> edge = Edges.Find(e => e.StartVertex == startVertex && e.EndVertex == endVertex
                 || e.Oriented == false && e.StartVertex == endVertex && e.EndVertex == startVertex);
            return edge;
        }

        public List<MetaVertex<T>> GetIncidentVertices(MetaVertex<T> vertex, EdgeType edgeType, bool withIncludedVertex)
        {
            List<MetaVertex<T>> incidentVertices = new List<MetaVertex<T>>();
            List<Edge<T, K>> incidentEdges = new List<Edge<T, K>>();
            if (edgeType == EdgeType.In || edgeType == EdgeType.All)
            {
                //Получение всех инцидентных вершин по входящим ребрам
                incidentEdges = Edges.FindAll(e => e.EndVertex == vertex);

                foreach (var e in incidentEdges)
                {
                    incidentVertices.Add(e.StartVertex);
                    if (withIncludedVertex == true)
                    {
                        //incidentVertices.AddRange(e.StartVertex.GetIncludedVertices());
                    }
                }
            }

            if (edgeType == EdgeType.Out || edgeType == EdgeType.All)
            {
                //Получение всех инцидентных вершин по исходящим ребрам
                incidentEdges = Edges.FindAll(e => e.StartVertex == vertex);

                foreach (var e in incidentEdges)
                {
                    incidentVertices.Add(e.EndVertex);
                    if (withIncludedVertex == true)
                    {
                        //incidentVertices.AddRange(e.EndVertex.GetIncludedVertices());
                    }
                }
            }

            return incidentVertices;
        }
        public bool VerticeAjacency(MetaVertex<T> v1, MetaVertex<T> v2)
        {
            return GetEdgeBetweenVertices(v1, v2) != null;
        }
        //определение списка входящих ребер/дуг
        override public List<Edge<T, K>> GetIncomingEdges(MetaVertex<T> vertex)
        {
            return Edges.FindAll(e => e.EndVertex == vertex || e.Oriented == false && e.StartVertex == vertex);
        }
        //определение списка исходящих ребер/дуг
        override public List<Edge<T, K>> GetOutEdges(MetaVertex<T> vertex)
        {
            return Edges.FindAll(e => e.StartVertex == vertex || e.Oriented == false && e.EndVertex == vertex);
        }
        override public Dictionary<MetaVertex<T>, List<MetaVertex<T>>> GenerateAdjacencyList()
        {
            Dictionary<MetaVertex<T>, List<MetaVertex<T>>> AdjacencyList = new Dictionary<MetaVertex<T>, List<MetaVertex<T>>>();
            foreach (var e in Edges)
            {
                if (AdjacencyList.ContainsKey(e.StartVertex))
                {
                    AdjacencyList[e.StartVertex].Add(e.EndVertex);
                }
                else
                {
                    AdjacencyList.Add(e.StartVertex, new List<MetaVertex<T>>());
                    AdjacencyList[e.StartVertex].Add(e.EndVertex);
                }
            }
            return AdjacencyList;
        }
        override public float GetDistancePathFromEdgesWeight(List<int> verticesOrder, bool closedPath)
        {
            float distance = 0;

            for (int i = 0; i < verticesOrder.Count - 1; ++i)
            {
                //Edge<T,K> edge = Edges.FirstOrDefault(e => e.StartVertex == vertices[i] && e.EndVertex == vertices[i + 1] || e.Oriented == false && e.StartVertex == vertices[i + 1] && e.EndVertex == vertices[i]);
                Edge<T, K> edge = GetEdgeBetweenVertices(Vertices[verticesOrder[i]], Vertices[verticesOrder[i + 1]]);
                if (edge == null) { return nullDistance; }
                distance += edge.Weight;
            }
            if (closedPath)
            {
                // Edge<T,K> edge = Edges.FirstOrDefault(e => e.StartVertex == vertices[vertices.Count - 1] && e.EndVertex == vertices[0] || e.Oriented == false && e.StartVertex == vertices[0] && e.EndVertex == vertices[vertices.Count - 1]);
                Edge<T, K> edge = GetEdgeBetweenVertices(Vertices[verticesOrder[verticesOrder.Count - 1]], Vertices[verticesOrder[0]]);
                if (edge == null) { return nullDistance; }
                distance += edge.Weight;
            }
            return distance;
        }
        //обход метаграфа в ширину
        override public void BFS(MetaVertex<T> startVertex)
        {
            float number = 1;
            Queue<MetaVertex<T>> queue = new Queue<MetaVertex<T>>();
            MetaVertex<T> currentVertex = startVertex;
            //List<Vertex<T>> incidenceVertices = new List<Vertex<T>>();
            //currentVertex.Number = number;
            //++number;
            queue.Enqueue(currentVertex);

            while (queue.Count != 0)
            {
                currentVertex = queue.Dequeue();
                if (currentVertex.Number == MetaVertex<T>.defaultNumber)
                {
                    List<MetaVertex<T>> numberedVertices = SetVertexNumber(currentVertex, ref number);
                    foreach (var v in numberedVertices)
                    {
                        queue.Enqueue(v);
                        //все смежные вершины тоже в очередь
                        List<Edge<T, K>> incidenceEdges = Edges.FindAll(e => e.StartVertex == v);
                        foreach (var e in incidenceEdges)
                        {
                            queue.Enqueue(e.EndVertex);
                        }
                    }
                }
            }
        }

        //обход метаграфа в глубину
        override public void DFS(MetaVertex<T> startVertex)
        {
            float number = 1;
            Stack<MetaVertex<T>> stack = new Stack<MetaVertex<T>>();
            MetaVertex<T> currentVertex = startVertex;
            List<Edge<T, K>> incidenceEdges = new List<Edge<T, K>>();
            currentVertex.Number = number;
            ++number;
            stack.Push(currentVertex);
            while (stack.Count != 0)
            {
                incidenceEdges = Edges.FindAll(e => (e.StartVertex == stack.Peek()) && (e.IsUsed == false));

                if (incidenceEdges.Count != 0)
                {
                    Edge<T, K> currentEdge = incidenceEdges.First();    //Find(e => e.IsUsed == false);
                    if (currentEdge != null)
                    {
                        currentEdge.IsUsed = true;
                        currentVertex = currentEdge.EndVertex;

                        List<MetaVertex<T>> numberedVertices = SetVertexNumber(currentVertex, ref number);
                        foreach (var v in numberedVertices)
                        {
                            stack.Push(v);
                        }
                    }
                    else
                    {
                        stack.Pop();
                    }
                }
                else
                {
                    stack.Pop();
                }
            }
        }

    }
}
