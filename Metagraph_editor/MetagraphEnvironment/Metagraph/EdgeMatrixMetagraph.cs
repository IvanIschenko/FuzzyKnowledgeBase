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

namespace WebApplication1
{
    [Serializable]
    public class EdgeMatrixMetagraph<T, K> : MetaGraph<T, K>
    {
        private List<List<Edge<T, K>>> edgeMatrix;
        public List<List<Edge<T, K>>> EdgeMatrix
        {
            get { return edgeMatrix; }
            set
            {
                for (int i = 0; i < value.Count; i++)
                {
                    if (value[i].Count != value.Count)
                        throw new Exception("Edge matrics is not square!");
                }
                edgeMatrix = value;
            }
        }

        public EdgeMatrixMetagraph()
        {
            this.EdgeMatrix = new List<List<Edge<T, K>>>();
        }
        public EdgeMatrixMetagraph(List<List<Edge<T, K>>> edgeMatrix)
        {
            this.EdgeMatrix = edgeMatrix;
        }
        ////заполнение матрицы EdgeMatrix - матрица инцидентности  в которой элементомом является ребро, которое соединяет вершины
        //public void FillEdgeMatrix()
        //{
        //    EdgeMatrix.Clear();

        //    for (int i = 0; i < Vertices.Count; i++)
        //    {
        //        EdgeMatrix.Add(new List<Edge<T,K>>());

        //        for (int j = 0; j < Vertices.Count; j++)
        //        {
        //            Edge<T,K> edge = GetEdgeBetweenVertices(Vertices[i], Vertices[j]);

        //            EdgeMatrix[i].Add(edge);
        //        }
        //    }
        //}
        public override void Clear()
        {
            this.EdgeMatrix.Clear();
            base.Clear();
        }
        override public int CountIncedentEdgesForVertex(MetaVertex<T> v)
        {
            int result = 0;
            int index = Vertices.IndexOf(v);
            for (int i = 0; i < edgeMatrix.Count; i++)
            {
                if (edgeMatrix[index][i] != null)
                {
                    result++;
                }
            }
            return result;
        }
        override public List<List<int>> GenerateIncidenceMatrix() { throw new NotImplementedException(); }
        override public List<List<MetaVertex<T>>> GenerateAdjacencyMatrix() { throw new NotImplementedException(); }
        override public Dictionary<MetaVertex<T>, List<MetaVertex<T>>> GenerateAdjacencyList() { throw new NotImplementedException(); }

        //сделать граф полным, список ребер очищается и добавляются неориентированные
        //ребра между каждой парой вершин (между всеми вершинами), 
        //вес ребра вычисляется как расстояние между его вершинами
        override public void MakeFullyMeshed() { throw new NotImplementedException(); }
        //вычисление длины пути проходящего по заданным вершинам по весам ребер
        override public float GetDistancePathFromEdgesWeight(List<int> verticesOrder, bool closedPath) { throw new NotImplementedException(); }

        //обход метаграфа в ширину
        override public void BFS(MetaVertex<T> startVertex) { throw new NotImplementedException(); }
        //обход метаграфа в глубину
        override public void DFS(MetaVertex<T> startVertex) { throw new NotImplementedException(); }
        //заполнение матрицы EdgeMatrix - матрица инцидентности  в которой элементомом является ребро, которое соединяет вершины
        //private void FillEdgeMatrix()
        //{
        //    EdgeMatrix.Clear();

        //    for (int i = 0; i < Vertices.Count; i++)
        //    {
        //        EdgeMatrix.Add(new List<Edge<T,K>>());

        //        for (int j = 0; j < Vertices.Count; j++)
        //        {
        //            Edge<T,K> edge = GetEdgeBetweenVertices(Vertices[i], Vertices[j]);

        //            EdgeMatrix[i].Add(edge);
        //        }
        //    }
        //}
        //определение списка входящих ребер/дуг с помощью матрицы EdgeMatrix
        override public List<Edge<T, K>> GetIncomingEdges(MetaVertex<T> vertex)
        {
            List<Edge<T, K>> edges = new List<Edge<T, K>>();
            Edge<T, K> edge;
            int j = Vertices.IndexOf(vertex);
            for (int i = 0; i < Vertices.Count; ++i)
            {
                edge = EdgeMatrix[i][j];
                if (edge != null)
                {
                    edges.Add(edge);
                }
            }
            return edges;
        }
        //определение списка достижимых вершин с помощью матрицы EdgeMatrix
        override public List<MetaVertex<T>> GetAccessibleVertices(MetaVertex<T> vertex, List<MetaVertex<T>> excludeVertices = null)
        {
            List<MetaVertex<T>> accessibleVerticesList = new List<MetaVertex<T>>();
            MetaVertex<T> accessibleVertex;
            if (vertex != null)
            {
                int vertexIndex = Vertices.IndexOf(vertex);

                for (int i = 0; i < Vertices.Count; ++i)
                {
                    accessibleVertex = null;
                    Edge<T, K> outEdge = EdgeMatrix[vertexIndex][i];

                    if (outEdge != null)
                    {
                        if (outEdge.Oriented == true)
                        {
                            accessibleVertex = outEdge.EndVertex;
                        }
                        else //если ребро не ориентированное(дуга), то достижимой сделать начальную или конечную вершину ребра, которая не равна исходной вершине
                        {
                            if (outEdge.StartVertex != vertex)
                            {
                                accessibleVertex = outEdge.StartVertex;
                            }
                            else
                            {
                                accessibleVertex = outEdge.EndVertex;
                            }
                        }
                    }
                    //если достижимая вершина не входит в список исключения и не равна null добавить ее в список достижимых вершин
                    if ((excludeVertices.FirstOrDefault(v => v == accessibleVertex) == null) && (accessibleVertex != null))
                    {
                        accessibleVerticesList.Add(accessibleVertex);
                    }
                }
            }

            return accessibleVerticesList;
        }
        //нахождение ребра между вершинами по матрице EdgeMatrix
        override public Edge<T, K> GetEdgeBetweenVertices(MetaVertex<T> startVertex, MetaVertex<T> endVertex)
        {
            Edge<T, K> edge = EdgeMatrix[Vertices.IndexOf(startVertex)][Vertices.IndexOf(endVertex)];
            //if (edge == null)
            //{
            //    edge = EdgeMatrix[Vertices.IndexOf(endVertex)][Vertices.IndexOf(startVertex)];
            //    if (edge.Oriented == true) { edge = null; }
            //}

            return edge;
        }
        public bool VerticeAjacency(int startVertex, int endVertex)
        {
            return EdgeMatrix[startVertex][endVertex] != null;
        }
        //вычисление длины пути проходящего по заданным вершинам по матрице EdgeMatrix
        public float GetDistancePathFromEdgesWeight(List<MetaVertex<T>> vertices, bool closedPath)
        {
            float distance = 0;

            for (int i = 0; i < vertices.Count - 1; ++i)
            {
                //Edge<T,K> edge = Edges.FirstOrDefault(e => e.StartVertex == vertices[i] && e.EndVertex == vertices[i + 1] || e.Oriented == false && e.StartVertex == vertices[i + 1] && e.EndVertex == vertices[i]);
                Edge<T, K> edge = GetEdgeBetweenVertices(vertices[i], vertices[i + 1]);
                if (edge == null) { return nullDistance; }
                distance += edge.Weight;
            }
            if (closedPath)
            {
                // Edge<T,K> edge = Edges.FirstOrDefault(e => e.StartVertex == vertices[vertices.Count - 1] && e.EndVertex == vertices[0] || e.Oriented == false && e.StartVertex == vertices[0] && e.EndVertex == vertices[vertices.Count - 1]);
                Edge<T, K> edge = GetEdgeBetweenVertices(vertices[vertices.Count - 1], vertices[0]);
                if (edge == null) { return nullDistance; }
                distance += edge.Weight;
            }
            return distance;
        }

        //определение списка исходящих ребер/дуг с помощью матрицы EdgeMatrix
        override public List<Edge<T, K>> GetOutEdges(MetaVertex<T> vertex)
        {
            List<Edge<T, K>> edges = new List<Edge<T, K>>();
            Edge<T, K> edge;
            int i = Vertices.IndexOf(vertex);
            for (int j = 0; j < Vertices.Count; ++j)
            {
                edge = EdgeMatrix[i][j];
                if (edge != null)
                {
                    edges.Add(edge);
                }
            }
            return edges;
        }

    }
}
