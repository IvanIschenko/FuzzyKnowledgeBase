using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using WebApplication1.FuzzyLogicBase;
using C__04._07._2015.Algorithms.Clustering;// progr_V.cs
using L.DataStructures.Matrix;// multidim..cs
using L.Algorithms.Clustering;// k_means.cs
using L.DataStructures.Geometry; // cluster.cs
using WebApplication1;

namespace WebApplication1
{
    public class GrahamAlgorithm
    {
        public static List<Point> ConvexHull(List<Point> p)
        {
            int n = p.Count;
            if (n <= 1)
                return p;
            int k = 0;
            p.Sort((e1, e2) => Math.Sign(e2.X - e1.X));
            List<Point> q = new List<Point>();
            for (int i = 0; i < n * 2; i++)
            {
                q.Add(default(Point));
            }
            for (int i = 0; i < n; q[k++] = p[i++])
                for (; k >= 2 && !cw(q[k - 2], q[k - 1], p[i]); --k)
                    ;
            for (int i = n - 2, t = k; i >= 0; q[k++] = p[i--])
                for (; k > t && !cw(q[k - 2], q[k - 1], p[i]); --k)
                    ;
            int a = q[0] == q[1] ? 1 : 0;
            q.RemoveRange(k - a, n * 2 - k - a);
            return q;
        }
        static bool cw(Point a, Point b, Point c)
        {
            return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X) < 0;
        }
    }
}
