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
using System.Windows.Media;

namespace WebApplication1
{
    public static class MetagraphGenerator
    {
        //ublic static FuzzyKnowledgeBase FKB = new FuzzyKnowledgeBase();
        static Random rnd;
        static MetagraphGenerator()
        {
            rnd = new Random();
        }
        public static EdgeMetagraph<int, int> GenerateRandomMetagraph(FuzzyKnowledgeBase FKB, int simpleVertNumber, int metaVertNumber,
            int maxCoordBoxWidth, int maxCoordBoxHeight, float sparseness, float includeChance)  // ListVarforCheck -- > ListVar
        {
            int vertNumber = metaVertNumber;
            for (int i = 0; i < FKB.ListVar.Count; ++i)
            {
                vertNumber += FKB.ListVar[i].terms.Count;
            }
            EdgeMetagraph<int, int> graph = new EdgeMetagraph<int, int>();
            string name = "M";
            string nameLP = "M";
            int num = 0;
            for (int i = 0, j = 0, t = 0; i < vertNumber; i++)
            {
                if (i < metaVertNumber)
                {
                    name = "M" + num.ToString();
                }
                num++;
                if (i >= metaVertNumber)
                {
                    nameLP = FKB.ListVar[j].Name;
                    name = FKB.ListVar[j].terms[t].Name;
                    ++t;
                    if (t == FKB.ListVar[j].terms.Count)
                    {
                        ++j;
                        t = 0;
                    }

                }

                graph.Vertices.Add(new MetaVertex<int>(name, nameLP, i, Coordinates: new Vector(rnd.Next(maxCoordBoxWidth), rnd.Next(maxCoordBoxHeight))));
            }

            //генерация включенных в метавершины вершин
            for (int i = 0, t = 0; i < metaVertNumber; ++i)
            {
                graph.Vertices[i].IncludedVertices.Clear();
                t = 0;
                while (graph.Vertices[i].IncludedVertices.Count < FKB.ListOfRule[i].Antecedents.Count )
                {
                    graph = findVertex(metaVertNumber, vertNumber, t, i, graph, FKB);
                    ++t;
                }
            }
            //levels
            for (int i = 0; i < metaVertNumber; i++)
            {
                graph.Vertices[i].Level = FKB.ListOfRule[i].Level;
                graph.Vertices[i].IsLevelSet = true;
                for (int k = 0; k < graph.Vertices[i].IncludedVertices.Count; k++)
                {
                    graph.Vertices[i].IncludedVertices[k].Level = FKB.ListOfRule[i].Level;
                    graph.Vertices[i].IncludedVertices[k].IsLevelSet = true;
                }
            }
            graph = NotincludedVertexLevelSet(metaVertNumber, vertNumber, graph);
            // генерация ребер
            for (int i = 0; i < metaVertNumber; i++)
            {
                string tempname = (FKB.ListOfRule[i].Cоnsequens.NameLP + FKB.ListOfRule[i].Cоnsequens.Name);
                for (int j = metaVertNumber; j < vertNumber; j++)
                {
                    string tempnamevert = (graph.Vertices[j].NameLP + graph.Vertices[j].Name);
                    if (tempname == tempnamevert)
                    {
                        graph.Edges.Add(new Edge<int, int>("E" + (graph.Edges.Count + 1).ToString(),
                            graph.Vertices[i], graph.Vertices[j]));
                        graph.Vertices[i].OutcomingEdgesCount++;
                        graph.Vertices[j].IncomingEdgesCount++;
                        break;
                    }
                }
            }

            return GenerateRandomColorsPlusOptimisation(graph);
        }

        private static EdgeMetagraph<int, int> findVertex(int metaVertNumber, int vertNumber, int t, int i, EdgeMetagraph<int, int> graph, FuzzyKnowledgeBase FKB)
        {
            string tempname = (FKB.ListOfRule[i].Antecedents[t].NameLP + FKB.ListOfRule[i].Antecedents[t].Name);
            for (int j = metaVertNumber; j < vertNumber; ++j)
            {

                string tempnamevert = (graph.Vertices[j].NameLP + graph.Vertices[j].Name);
                if (tempname == tempnamevert)
                {
                    graph.Vertices[i].IncludedVertices.Add(graph.Vertices[j]);
                    graph.Vertices[j].IsIncluded = true;
                    break;

                }
            }
            return (graph);
        }

        private static EdgeMetagraph<int, int> NotincludedVertexLevelSet(int metaVertNumber, int vertNumber, EdgeMetagraph<int, int> graph)
        {
            for (int i = metaVertNumber; i < vertNumber; ++i)
            {
                if (graph.Vertices[i].IsIncluded == false)
                {
                    for (int j = metaVertNumber; j < vertNumber; ++j)
                    {
                        if (graph.Vertices[j].IsIncluded == true)
                        {
                            if (graph.Vertices[i].NameLP == graph.Vertices[j].NameLP)
                            {
                                graph.Vertices[i].Level = graph.Vertices[j].Level;
                                graph.Vertices[i].IsLevelSet = true;
                                break;
                            }
                        }
                        else continue;
                    }
                    if (graph.Vertices[i].IsLevelSet == false)
                    {
                        graph.Vertices[i].Level = 0;
                        graph.Vertices[i].IsLevelSet = true;
                    }
                }
            }

            return (graph);
        }

        public static EdgeMetagraph<int, int> GenerateMetagraphFromText(string text, int maxCoordBoxWidth, int maxCoordBoxHeight)
        {
            IMetagraphTextParser<int, int> parser = new EdgeMetagraphTextParser<int, int>();
            EdgeMetagraph<int, int> graph = (EdgeMetagraph<int, int>)parser.GenerateMetagraph(text);
            graph.Vertices.ForEach(v => v.Coordinates = new Vector(rnd.Next(maxCoordBoxWidth), rnd.Next(maxCoordBoxHeight)));
            return GenerateRandomColorsPlusOptimisation(graph);
        }

        public static EdgeMetagraph<int, int> GenerateRandomColorsPlusOptimisation(EdgeMetagraph<int, int> graph)
        {
            graph.Optimisation();

            Random rnd = new Random();
            int metaVertNumber = graph.CountMetavertices();
            int vertNumber = graph.CountVertices();
            //генерация цветов вершин
            Color tempColor;
            for (int i = metaVertNumber; i < vertNumber; i++)
            {
                if (graph.Vertices[i].IncludedVertices == null)
                    graph.Vertices[i].Color = Color.FromRgb((byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255));

            }
            for (int i = 0; i < metaVertNumber; i++)
            {
                if (graph.Vertices[i].IncludedVertices != null)
                {
                    tempColor = Color.FromArgb(100, (byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255));
                    graph.Vertices[i].Color = tempColor;
                    foreach (MetaVertex<int> item in graph.Vertices[i].IncludedVertices)
                    {
                        item.Color = tempColor;//Color.FromRgb(tempColor.R, tempColor.G, tempColor.B);
                    }
                }
            }
            return graph;
        }
    }
}
