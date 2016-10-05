using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WebApplication1.FuzzyLogicBase;
using C__04._07._2015.Algorithms.Clustering;// progr_V.cs
using L.DataStructures.Matrix;// multidim..cs
using L.Algorithms.Clustering;// k_means.cs
using L.DataStructures.Geometry; // cluster.cs
using WebApplication1;

namespace WebApplication1
{
    public class EdgeMetagraphTextParser<T, K> : IMetagraphTextParser<T, K>
    {
        public static FuzzyKnowledgeBase FKB = new FuzzyKnowledgeBase();
        String patternMetaVertice = @"\w+<([\w,]*)>,l\d+";
        String patternEdge = @"\w+\(\w+,\w+\)";
        EdgeMetagraph<T, K> graph;
        public EdgeMetagraphTextParser()
        {
            graph = new EdgeMetagraph<T, K>();
        }
        public MetaGraph<T, K> GenerateMetagraph(string text)
        {
            graph = new EdgeMetagraph<T, K>();
            //all vertices names
            MatchCollection matchAllVertices = Regex.Matches(text, patternMetaVertice);
            foreach (Match matchVert in matchAllVertices)
            {
                graph.Vertices.Add(new MetaVertex<T>() { Name = Regex.Match(matchVert.Value, @"^\w+").Value });
            }

            //metavertices
            MetaVertex<T> mv;
            for (int i = 0; i < matchAllVertices.Count; i++)
            {
                MatchCollection innerSymbols = Regex.Matches(matchAllVertices[i].Value, @"\w+");
                if (graph.Vertices[i].Name == innerSymbols[0].Value)
                {
                    if (innerSymbols.Count > 2)
                    {
                        graph.Vertices[i].IncludedVertices = new List<MetaVertex<T>>();

                        for (int j = 1; j < innerSymbols.Count - 1; j++)
                        {
                            mv = graph.Vertices.Find(v => v.Name == innerSymbols[j].Value);
                            if (mv != null)
                                FKB.ListOfRule.Count();//ne to chto bilo
                            else throw new ArgumentException(String.Format("Error in text:{0}. Inner vertice {1} does not exist", matchAllVertices[i].Value, innerSymbols[j].Value));
                        }
                    }
                    try
                    {
                        graph.Vertices[i].Level = Int32.Parse(Regex.Match(innerSymbols[innerSymbols.Count - 1].Value, @"(?<=l)\d+").Value);
                    }
                    catch (FormatException)
                    {
                        throw new FormatException(String.Format("Error in text: {0}. Level has a wrong format", matchAllVertices[i].Value));
                    }

                }
                else throw new ArgumentException("Error in text");

            }
            //edges
            MatchCollection allEdges = Regex.Matches(text, patternEdge);
            foreach (Match matchEdge in allEdges)
            {
                MatchCollection innerSymbols = Regex.Matches(matchEdge.Value, @"\w+");
                Edge<T, K> edge = new Edge<T, K>() { Name = innerSymbols[0].Value };
                edge.StartVertex = graph.Vertices.Find(v => v.Name == innerSymbols[1].Value);
                if (edge.StartVertex == null)
                    throw new FormatException(String.Format("Error in text:{0}, vertice {1} does not exist", matchEdge.Value, innerSymbols[1].Value));
                edge.EndVertex = graph.Vertices.Find(v => v.Name == innerSymbols[2].Value);
                if (edge.EndVertex == null)
                    throw new FormatException(String.Format("Error in text:{0}, vertice {1} does not exist", matchEdge.Value, innerSymbols[2].Value));
                graph.Edges.Add(edge);
            }
            return graph;
        }
    }
}
