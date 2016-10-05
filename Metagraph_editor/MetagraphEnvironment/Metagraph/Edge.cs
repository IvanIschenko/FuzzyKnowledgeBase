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
using System.Windows.Shapes;
using System.Windows.Media;

namespace WebApplication1
{
    public class Edge<T, K> : IComparable
    {
        ///<value><c>Name</c> название ребра </value> 
        public String Name { get; set; }
        public MetaVertex<T> StartVertex { get; set; }
        public MetaVertex<T> EndVertex { get; set; }
        public K Content { get; set; }
        ///<value><c>Weight</c> вес ребра </value> 
        public float Weight { get; set; }
        public Color Сolor { get; set; }
        public Shape DrawingShape { get; set; }
        public Shape ArrowShape { get; set; }
        public bool Oriented { get; set; }
        public bool IsUsed { get; set; }
        public double ZnachFaz { get; set; }
        public float Pheromone { get; set; }

        public Edge()
        {

        }
        public Edge(String Name, MetaVertex<T> StartVertex, MetaVertex<T> EndVertex, bool Oriented = false, float Weight = -1,
            float Pheromone = 0, Color Color = default(Color), Shape shape = null)
        {
            this.Name = Name;
            this.Weight = Weight;
            this.Oriented = Oriented;
            this.Сolor = Color;
            this.StartVertex = StartVertex;
            this.EndVertex = EndVertex;
            this.IsUsed = false;
            this.Pheromone = Pheromone;
            this.DrawingShape = shape;
            if (Сolor == default(Color)) { Сolor = Color.FromRgb(255, 255, 255); }
        }
        public Edge(String Name, K content, MetaVertex<T> StartVertex, MetaVertex<T> EndVertex, bool Oriented = false, float Weight = -1,
            float Pheromone = 0, Color Color = default(Color), Shape shape = null)
        {
            this.Name = Name;
            this.Content = content;
            this.Weight = Weight;
            this.Oriented = Oriented;
            this.Сolor = Color;
            this.StartVertex = StartVertex;
            this.EndVertex = EndVertex;
            this.IsUsed = false;
            this.Pheromone = Pheromone;
            this.DrawingShape = shape;
            if (Сolor == default(Color)) { Сolor = Color.FromRgb(255, 255, 255); }
        }

        public override string ToString()
        {
            return String.Format("{0}({1},{2})", Name, this.StartVertex.Name, this.EndVertex.Name);
        }

        public int CompareTo(object obj)
        {
            Edge<T, K> n = obj as Edge<T, K>;
            if (n != null)
            {
                return this.Weight.CompareTo(n.Weight);
            }
            else { throw new ArgumentException("Ребра графа не сравнимы!"); }
        }
    }
}
