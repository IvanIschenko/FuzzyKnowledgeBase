using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using WebApplication1.FuzzyLogicBase;
using C__04._07._2015.Algorithms.Clustering;// progr_V.cs
using L.DataStructures.Matrix;// multidim..cs
using L.Algorithms.Clustering;// k_means.cs
using L.DataStructures.Geometry; // cluster.cs
using WebApplication1;

namespace WebApplication1
{
    public class MetaVertex<T> : IComparable
    {
        public Guid ID { get; set; }
        public String Name { get; set; }
        public String NameLP { get; set; }
        public bool IsIncluded { get; set; }
        public bool IsLevelSet { get; set; }
        public List<MetaVertex<T>> IncludedVertices
        {
            get;
            set;
        }
        public bool IsMetavertex
        {
            get
            {
                return IncludedVertices.Count != 0;
            }
        }
        public Shape shape;
        public Shape DrawingShape
        {
            get { return shape; }
            set { shape = value; }
        }
        public const float defaultNumber = -1;
        public T Content { get; set; }
        public float Number { get; set; }
        public Color Color { get; set; }
        public float Weight { get; set; }
        public Vector Coordinates;
        public double ZnachFaz { get; set; }
        public int Level { get; set; }
        //public VectorPolar PolarCoordinates  {get; set;}
        //public VertexUsed Used { get; set; }
        public int IncomingEdgesCount { get; set; }
        public int OutcomingEdgesCount { get; set; }
        public PathFigure PF { get; set; }


        public MetaVertex()
        {
            this.IncludedVertices = new List<MetaVertex<T>>();
            this.ID = Guid.NewGuid();
            this.Weight = defaultNumber;
            this.Color = Colors.White;
        }
        public MetaVertex(String Name, String NameLP, T Content, float Number = defaultNumber,
             Vector Coordinates = default(Vector), Color color = default(Color), Shape shape = null)
        {
            this.IncludedVertices = new List<MetaVertex<T>>();
            this.ID = Guid.NewGuid();
            this.Name = Name;
            this.NameLP = NameLP;
            this.Coordinates = Coordinates;
            this.Color = color;
            this.Content = Content;
            this.Number = Number;
            this.Weight = defaultNumber;
            if (Color == default(Color))
                Color = Colors.White;
            this.DrawingShape = shape;
            this.Level = 0;
            this.IsIncluded = false;
            this.IsLevelSet = false;
        }

        //Получение входящих в метавершину элементов 
        public static List<MetaVertex<T>> GetIncludedVertices(MetaVertex<T> vertex)
        {
            List<MetaVertex<T>> allIncludedVertices = new List<MetaVertex<T>>();

            if (vertex.IncludedVertices.Count != 0)
            {
                foreach (var v in vertex.IncludedVertices)
                {
                    allIncludedVertices.Add(v);
                    //рекурсия, чтобы добавить все входящие элементы, ПРОВЕРИТЬ БУДЕТ ЛИ РАБОТАТЬ
                    allIncludedVertices.AddRange(GetIncludedVertices(v));
                }
            }


            return allIncludedVertices;
        }
        public List<MetaVertex<T>> GetIncludedVertices()
        {
            return GetIncludedVertices(this);
        }

        public double GetMaxDistForInnerVert()
        {
            if (IncludedVertices.Count == 0)
                return 0;
            // System.Diagnostics.Debug.WriteLine("--------------");
            double result = 0, temp = 0;
            for (int i = 0; i < IncludedVertices.Count; i++)
            {
                // temp = (IncludedVertices[i].Coordinates - Coordinates).Length;
                temp = MathHelper.DistanceBetweenVectors(Coordinates, IncludedVertices[i].Coordinates);

                if (temp > result)
                    result = temp;
                //System.Diagnostics.Debug.WriteLine(Name + " temp :" + temp + " result " + result); 
            }
            //System.Diagnostics.Debug.WriteLine(Name + " result " + result); 
            return result;

        }
        public MetaVertex<T> GetMostDistantInnerVert()
        {
            if (IncludedVertices.Count == 0)
                return null;
            double maxDistance = 0, tempDistance = 0;
            MetaVertex<T> tempdistVertex = IncludedVertices[0];
            MetaVertex<T> resultVertex = IncludedVertices[0];
            for (int i = 0; i < IncludedVertices.Count; i++)
            {
                tempDistance = MathHelper.DistanceBetweenVectors(Coordinates, IncludedVertices[i].Coordinates);

                if (tempDistance > maxDistance)
                {
                    maxDistance = tempDistance;
                    resultVertex = tempdistVertex;
                }
            }

            return resultVertex;
        }

        public float Distance(MetaVertex<T> v)
        {
            return MathHelper.DistanceBetweenVectors(v.Coordinates, this.Coordinates);
        }

        public int CompareTo(object obj)
        {
            MetaVertex<T> n = obj as MetaVertex<T>;
            if (n != null)
                return this.Number.CompareTo(n.Number);
            else
                throw new ArgumentException("Вершины графа не сравнимы!");
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (IsMetavertex)
            {
                sb.Append(" include<");
                this.IncludedVertices.ForEach(v => sb.Append(v.Name + ","));

                sb.Append(">");
            }
            if (this.Name != "")
            { return this.Name + sb.ToString(); }
            else
            {
                if (this.Content != null)
                {
                    return this.Content.ToString();
                }
                else { return "NoName"; }
            }
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            MetaVertex<T> temp = obj as MetaVertex<T>;
            if (temp == null)
                return false;
            return this.ID == temp.ID;
        }
    }
}
