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
using System.Windows;

namespace WebApplication1
{
    public enum EdgeType { In, Out, All }

    /// <summary>
    /// Represents Metagraph
    /// </summary>
    /// <typeparam name="T">Metavertex content Type</typeparam>
    /// <typeparam name="K">Edge content Type</typeparam>
    [Serializable]
    public abstract class MetaGraph<T, K>
    {
        public String Name { get; set; }
        // public Dictionary<MetaVertex<T>, List<MetaVertex<T>>> AdjacencyList { get; set; }
        public List<MetaVertex<T>> Vertices { get; set; }
        public const float nullDistance = float.PositiveInfinity;
        //public List<List<Vector>> EdgeIntersectionMatrix { get; set; }
        //   public Dictionary<Edge<T,K>, Dictionary<Edge<T,K>, Vector>> EdgeIntersectionMatrix { get; set; }
        //  public int Dictionary<<Vector> IntersectionMatrix { get; set; }
        public String logDFS { get; set; }

        public MetaGraph()
        {
            this.Name = "LibraryGraph_MetaGraph";
            // this.Vertices = new SortedSet<Vertex<T>>();
            this.Vertices = new List<MetaVertex<T>>();

            //  this.AdjacencyList = new Dictionary<MetaVertex<T>, List<MetaVertex<T>>>();
            // this.DistanceMatrix = new List<List<float>>();
            //this.AdjacencyMatrix = new List<List<MetaVertex<T>>>();
            //this.IncidenceMatrix = new List<List<int>>();
            this.logDFS = "";

            // this.EdgeIntersectionMatrix = new Dictionary<Edge<T,K>, Dictionary<Edge<T,K>, Vector>>();
        }
        public abstract int CountIncedentEdgesForVertex(MetaVertex<T> v);

        public int CountMetavertices()
        {
            int result = 0;
            foreach (MetaVertex<T> item in Vertices)
            {
                if (item.IsMetavertex)
                {
                    result++;
                }
            }
            return result;
        }
        public int CountVertices()
        {
            int result = 0;
            foreach (MetaVertex<T> item in Vertices)
            {
                if (item.IsMetavertex == false)
                {
                    result++;
                }
            }
            return result;
        }
        public abstract List<List<int>> GenerateIncidenceMatrix();
        public abstract List<List<MetaVertex<T>>> GenerateAdjacencyMatrix();
        public abstract Dictionary<MetaVertex<T>, List<MetaVertex<T>>> GenerateAdjacencyList();
        public abstract Edge<T, K> GetEdgeBetweenVertices(MetaVertex<T> startVertex, MetaVertex<T> endVertex);
        //сделать граф полным, список ребер очищается и добавляются неориентированные
        //ребра между каждой парой вершин (между всеми вершинами), 
        //вес ребра вычисляется как расстояние между его вершинами
        public abstract void MakeFullyMeshed();
        //вычисление длины пути проходящего по заданным вершинам по весам ребер
        public abstract float GetDistancePathFromEdgesWeight(List<int> verticesOrder, bool closedPath);
        //определение списка исходящих ребер/дуг
        public abstract List<Edge<T, K>> GetOutEdges(MetaVertex<T> vertex);
        //определение списка входящих ребер/дуг
        public abstract List<Edge<T, K>> GetIncomingEdges(MetaVertex<T> vertex);
        //обход метаграфа в ширину
        public abstract void BFS(MetaVertex<T> startVertex);
        //обход метаграфа в глубину
        public abstract void DFS(MetaVertex<T> startVertex);

        virtual public void Clear()
        {
            //this.Vertices.Clear();
            //
            //this.AdjacencyList.Clear();
            //this.DistanceMatrix.Clear();

            //this.AdjacencyMatrix.Clear();
            //this.IncidenceMatrix.Clear();
            this.logDFS = "";
        }

        //вычисление длины пути проходящего по заданным вершинам по расстоянию между этими вершинами (по координатам)
        public static float GetDistancePathFromCoordinates(List<MetaVertex<T>> vertices, bool closedPath)
        {
            float distance = 0;
            for (int i = 0; i < vertices.Count - 1; i++)
            {
                distance += MathHelper.DistanceBetweenVectors(vertices[i].Coordinates, vertices[i + 1].Coordinates);
            }
            if (closedPath)
                distance += MathHelper.DistanceBetweenVectors(vertices[vertices.Count].Coordinates, vertices[0].Coordinates);
            return distance;
        }
        //нумерация метавершины и всех входящих в нее вершин
        public static List<MetaVertex<T>> SetMetaVertexAndIncludedVertexNumber(MetaVertex<T> metaVertex, ref float number)
        {
            List<MetaVertex<T>> numberedVertices = new List<MetaVertex<T>>();
            //нумерация метавершины
            if (metaVertex.Number == MetaVertex<T>.defaultNumber)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("set metaVertexList " + metaVertex.ToString() + " number=" + number);
#endif
                //logDFS += "set metaVertexList " + metaVertex.ToString() + " number=" + number + "\n";
                metaVertex.Number = number;
                ++number;
                numberedVertices.Add(metaVertex);
                //нумерация входящих в метавершину вершин
                foreach (var vertexIncluded in metaVertex.IncludedVertices)
                {
                    if (vertexIncluded.Number == MetaVertex<T>.defaultNumber)
                    {
                        vertexIncluded.Number = number;
                        ++number;
                        numberedVertices.Add(vertexIncluded);
                    }
                }
            }
            return numberedVertices;
        }


        //создание матрицы растояний, если вершины заданы координатами и расстояние между ними вычисляется как расстояние между точками, которые заданы этими координатами
        //при вычислении растояния учитывается есть ли ребро между вершинами и его направление
        public List<List<float>> CreateDistanceMatrixFromVertexVector()
        {
            List<List<float>> distanceMatrix = new List<List<float>>();
            float distanceBetweenVertices = 0;
            for (int i = 0; i < Vertices.Count; i++)
            {
                distanceMatrix.Add(new List<float>());

                for (int j = 0; j < Vertices.Count; j++)
                {
                    Edge<T, K> edge = GetEdgeBetweenVertices(Vertices[i], Vertices[j]);

                    if (edge != null)
                    {
                        distanceBetweenVertices = MathHelper.DistanceBetweenVectors(Vertices[i].Coordinates, Vertices[j].Coordinates);
                    }
                    else
                    { distanceBetweenVertices = nullDistance; }
                    distanceMatrix[i].Add(distanceBetweenVertices);
                }
            }
            return distanceMatrix;
        }
        //нормирование матрицы расстояний
        public List<List<float>> GetNormDistanceMatrix(List<List<float>> distanceMatrix)
        {
            List<List<float>> normMatrix = new List<List<float>>();
            float normDistance = 0;
            //List<float> listMaxDistanceForVertices = DistanceMatrix.Select(e => e.Max()).ToList();
            //listMaxDistanceForVertices.Remove(float.PositiveInfinity);
            float maxDistance = 0;

            for (int i = 0; i < distanceMatrix.Count; ++i)
                for (int j = 0; j < distanceMatrix.Count; ++j)
                {
                    if (distanceMatrix[i][j] != nullDistance && maxDistance < distanceMatrix[i][j]) { maxDistance = distanceMatrix[i][j]; }
                }


            for (int i = 0; i < Vertices.Count; i++)
            {
                normMatrix.Add(new List<float>());

                for (int j = 0; j < Vertices.Count; j++)
                {
                    if (distanceMatrix[i][j] == nullDistance)
                    {
                        normDistance = nullDistance;
                    }
                    else
                    {
                        normDistance = distanceMatrix[i][j] / maxDistance;
                    }

                    normMatrix[i].Add(normDistance);
                }
            }

            return normMatrix;
        }
        //вычисление длины пути проходящего по заданным вершинам по весам ребер
        //??????????????????????????????????????????????????????????/
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
        //определение списка достижимых вершин
        public virtual List<MetaVertex<T>> GetAccessibleVertices(MetaVertex<T> vertex, List<MetaVertex<T>> excludeVertices = null)
        {
            List<MetaVertex<T>> accessibleVerticesList = new List<MetaVertex<T>>();
            List<Edge<T, K>> outEdges = GetOutEdges(vertex);
            MetaVertex<T> accessibleVertex;

            foreach (var e in outEdges)
            {
                if (e.StartVertex != vertex) { accessibleVertex = e.StartVertex; }
                else { accessibleVertex = e.EndVertex; }

                if (excludeVertices.FirstOrDefault(v => v == accessibleVertex) == null)
                { accessibleVerticesList.Add(accessibleVertex); }
            }

            return accessibleVerticesList;
        }
        public List<MetaVertex<T>> GetMetaVertexForVertex(MetaVertex<T> vertexIn)
        {
            List<MetaVertex<T>> metaVertexList = new List<MetaVertex<T>>();
            foreach (var v in Vertices)
            {
                if (v.IncludedVertices.Contains(vertexIn))
                {
                    metaVertexList.Add(v);
                }
            }

            logDFS += "Count vertex in " + vertexIn.ToString() + "=" + metaVertexList.Count + "\n";
            return metaVertexList;
        }
        public List<MetaVertex<T>> SetVertexNumber(MetaVertex<T> vertex, ref float number)
        {
            List<MetaVertex<T>> numberedVertices = new List<MetaVertex<T>>();

            //если вершина является частью метавершин, то нумеруем все метавершины, частью которых она является, и потом все входящие в метавершины вершины
            List<MetaVertex<T>> metaVertexIncludingList = GetMetaVertexForVertex(vertex);
            if (metaVertexIncludingList.Count != 0)
            {
                foreach (var metaVertexIncluding in metaVertexIncludingList)
                {
                    List<MetaVertex<T>> nv = SetMetaVertexAndIncludedVertexNumber(metaVertexIncluding, ref number);
                    numberedVertices.AddRange(nv);
                }
                logDFS += "vertex " + vertex.ToString() + " include in " + metaVertexIncludingList.Count + " metavertex\n";
            }
            else
            {
                logDFS += "vertex " + vertex.ToString() + " do not included in any metaVertexList \n";
                //если вершина является метавершиной, то нумеруем ее и все, что в нее входит
                if (vertex.IncludedVertices.Count != 0)
                {
                    List<MetaVertex<T>> nv = SetMetaVertexAndIncludedVertexNumber(vertex, ref number);
                    numberedVertices.AddRange(nv);
                }
                else //если это просто вершина нумеруем ее
                {
                    if (vertex.Number == MetaVertex<T>.defaultNumber)
                    {
                        logDFS += "set Vertex " + vertex.ToString() + " number=" + number + "\n";
                        vertex.Number = number;
                        ++number;
                        numberedVertices.Add(vertex);
                    }
                }
            }
            return numberedVertices;
        }
        public Vector GetCenterOfGravity()
        {
            double x = 0, y = 0;
            for (int i = 0; i < Vertices.Count; i++)
            {
                x += Vertices[i].Coordinates.X;
                y += Vertices[i].Coordinates.Y;
            }
            x /= Vertices.Count;
            y /= Vertices.Count;
            return new Vector(x, y);
        }

        #region NonWorkingCode
        ////удаление данных о ребрах из матрицы пересечений ребер, которые были удалены из метаграфа
        //public void DeleteEdgeFromEdgeIntersectionMatrix(Edge<T,K> edge)
        //{
        //    EdgeIntersectionMatrix.Remove(edge);
        //    foreach (var e in EdgeIntersectionMatrix)
        //    {
        //        e.Value.Remove(edge);
        //    }
        //}
        /*  public void FillEdgeIntersectionMatrix()
   {
       EdgeIntersectionMatrix.Clear();
       int k = 0;
       //for (int i = 0; i < Edges.Count; i++)
       foreach (var i in Edges)
       {
           EdgeIntersectionMatrix.Add(new List<Vector>());

           //for (int j = 0; j < Edges.Count; j++)
           foreach (var j in Edges)
           {
               Segment edge_i = new Segment(i.StartVertex.Coordinates, i.EndVertex.Coordinates);
               Segment edge_j = new Segment(j.StartVertex.Coordinates, j.EndVertex.Coordinates);
               Vector intersectionVector = Intersection.VectorOfIntersectionSegments(edge_i, edge_j, false);

               EdgeIntersectionMatrix[k].Add(intersectionVector);
           }
           ++k;
       }
   }*/
        /*  public List<Vertex<T>> GetIncidentVerticesWithIncluded(Vertex<T> vertex)
          {
              List<Edge<T,K>> incidentEdges = Edges.FindAll(e => e.StartVertex == vertex);
              List<Vertex<T>> incidentVertices = new List<Vertex<T>>();
              foreach (var e in incidentEdges)
              {
                  incidentVertices.Add(e.EndVertex);
                  if (e.EndVertex.MetaVertex.Count != 0) //добавление входящих в метавершину вершин
                  {
                      foreach (var v in e.EndVertex.MetaVertex)
                      {
                          incidentVertices.Add(v);
                      }
                  }
              }

              return incidentVertices;
          }
          */
        /*  public void FillEdgeIntersectionMatrix()
          {
              EdgeIntersectionMatrix.Clear();
              int k = 0;
              //for (int i = 0; i < Edges.Count; i++)
              foreach (var i in Edges)
              {
                  EdgeIntersectionMatrix.Add(new List<Vector>());

                  //for (int j = 0; j < Edges.Count; j++)
                  foreach (var j in Edges)
                  {
                      Segment edge_i = new Segment(i.StartVertex.Coordinates, i.EndVertex.Coordinates);
                      Segment edge_j = new Segment(j.StartVertex.Coordinates, j.EndVertex.Coordinates);
                      Vector intersectionVector = Intersection.VectorOfIntersectionSegments(edge_i, edge_j, false);

                      EdgeIntersectionMatrix[k].Add(intersectionVector);
                  }
                  ++k;
              }
          }*/
        ////определяет имеет ли ребро пересечения с другими ребрами метаграфа
        //public bool IsIntersection(Edge<T,K> edge)
        //{
        //    bool isIntersection = false;
        //    if (EdgeIntersectionMatrix.ContainsKey(edge))
        //    {
        //        Dictionary<Edge<T,K>, Vector> intersections = EdgeIntersectionMatrix[edge];
        //        int countIntersection = 0;

        //        foreach (var i in intersections)
        //        {
        //            if (i.Value.X == nullDistance && i.Value.Y == nullDistance) { ++countIntersection; }
        //        }

        //        if (countIntersection == intersections.Count)
        //        {
        //            isIntersection = false;
        //        }
        //        else
        //        {
        //            isIntersection = true;
        //        }
        //    }
        //    return isIntersection;
        //}
        ////заполнение матрицы пересечений ребер
        //public void FillEdgeIntersectionMatrix()
        //{
        //    EdgeIntersectionMatrix.Clear();
        //    int k = 0;
        //    //for (int i = 0; i < Edges.Count; i++)
        //    foreach (var i in Edges)
        //    {

        //        Dictionary<Edge<T,K>, Vector> intersections = new Dictionary<Edge<T,K>, Vector>();
        //        //for (int j = 0; j < Edges.Count; j++)
        //        foreach (var j in Edges)
        //        {
        //            Segment edge_i = new Segment(i.StartVertex.Coordinates, i.EndVertex.Coordinates);
        //            Segment edge_j = new Segment(j.StartVertex.Coordinates, j.EndVertex.Coordinates);
        //            Vector intersectionVector = Intersection.VectorOfIntersectionSegments(edge_i, edge_j, false);

        //            intersections.Add(j, intersectionVector);
        //        }
        //        EdgeIntersectionMatrix.Add(i, intersections);
        //        ++k;
        //    }
        //}
        //public void GraphVizMetaGraph(String FromWhat, String mapPath, String fileName)
        //{
        //    String dotFile = "";
        //    //String mapPath="";
        //    switch (FromWhat)
        //    {
        //        case "Edge":
        //            {
        //                //List<Vertex<T>> VertexList = new List<Vertex<T>>();
        //                // foreach(var i in Vertices)
        //                // {
        //                // VertexList.Add(i);
        //                // }
        //                dotFile = GraphViz<T>.GenerateDotFile(this.Vertices, this.Edges, GraphViz<T>.ArrowShapes.vee);
        //                break;
        //            }
        //        case "AdjacencyList":
        //            {
        //                break;
        //            }
        //        default:
        //            {
        //                dotFile = "Error";
        //                break;
        //            }
        //    }

        //    //так если для ASP.net приложения   GraphViz<T>.WriteDotFile(HttpContext.Current.Server.MapPath("~") + mapPath + "/" + fileName, dotFile);
        //    //так если для ASP.net приложения     GraphViz<T>.runGraphViz(HttpContext.Current.Server.MapPath("~") + mapPath, fileName);

        //    GraphViz<T>.WriteDotFile(mapPath + "/" + fileName, dotFile);
        //    GraphViz<T>.runGraphViz(mapPath, fileName);
        //}
        #endregion
        public void RandomizeCoordinates(int minX, int maxX, int minY, int maxY)
        {
            Random rnd = new Random();
            foreach (MetaVertex<T> v in Vertices)
            {
                v.Coordinates = new Vector(rnd.Next(minX, maxX), rnd.Next(minY, maxY));
            }
        }
        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            int i = 0;
            Vertices.ForEach(e => str.Append("v" + i++ + "(" + e.Coordinates.X + "," + e.Coordinates.Y + ")\t"));

            return str.ToString();
        }
    }
}
