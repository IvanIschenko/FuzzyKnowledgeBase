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
    public static class MathHelper
    {
        public static Vector UP = new Vector(0, 1);
        public static Vector DOWN = new Vector(0, -1);
        public static Vector RIGHT = new Vector(1, 0);
        public static Vector LEFT = new Vector(-1, 0);
        public static float DistanceBetweenVectors(Vector p1, Vector p2)
        {
            double x = p1.X - p2.X;
            double y = p1.Y - p2.Y;
            return (float)Math.Sqrt(x * x + y * y);
        }
        public static double Norm(Vector p1, Vector p2)
        {
            return Math.Sqrt(Math.Pow(p1.X + p2.X, 2) + Math.Pow(p1.Y + p2.Y, 2));
        }
        public static double Norm(double startX, double startY, double endX, double endY)
        {
            return Math.Sqrt(Math.Pow(startX - endX, 2) + Math.Pow(startY - endY, 2));
        }
        public static double NormSquared(double startX, double startY, double endX, double endY)
        {
            return Math.Pow(startX - endX, 2) + Math.Pow(startY - endY, 2);
        }

        public static double Scalar(Vector p1, Vector p2)
        {
            return p1.X * p2.X + p1.Y * p2.Y;
        }
        static Random rnd = new Random();
        public static Vector GetDirection(Vector start, Vector end)
        {
            Vector temp = Vector.Subtract(end, start);
            double tempLenSqv = temp.LengthSquared;

            if (tempLenSqv < 0.0000001d)
            {
                temp = new Vector(rnd.NextDouble() * 2 - 1, rnd.NextDouble() * 2 - 1);
                tempLenSqv = temp.LengthSquared;
            }

            return temp / Math.Sqrt(tempLenSqv);
        }
        public static Vector GetDirection(double startX, double startY, double endX, double endY)
        {
            Vector dir = new Vector(endX - startX, endY - startY);
            dir.Normalize();
            return dir;
        }
        /// <summary>
        /// Радиус эллипса в данной точке (расстояние от его центра до данной точки)
        /// </summary>
        /// <param name="a">большая полуось</param>
        /// <param name="b">малая полуось</param>
        /// <param name="angle">угол в градусах между радиус-вектором данной точки и осью абсцисс gradus</param>
        /// <returns></returns>
        public static double GetRadiusToPointOnEllipse(double a, double b, double angle)
        {
            angle *= Math.PI / 180;

            return a * b / Math.Sqrt(b * b * Math.Pow(Math.Cos(angle), 2)
                + a * a * Math.Pow(Math.Sin(angle), 2));
        }
        /// <summary>
        /// в градусах ретурн
        /// </summary>
        /// <param name="a"></param>
        /// <returns>в градусах</returns>
        public static double Fi(Vector a)
        {
            double al = a.X * a.X + a.Y * a.Y;
            al = Math.Sqrt(al);
            if (a.Y >= 0)
                return Math.Acos(a.X / al) * 180 / Math.PI;
            else
                return -Math.Acos(a.X / al) * 180 / Math.PI;
        }

    }
}
