﻿using System;
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
using System.Diagnostics;

namespace WebApplication1
{
    public enum VisualizationOrientation { Horizontal, Vertical }

    public class TreeVisualizationAlgorithm<T, K> : VisualizAlgorithm<T, K>
    {
        private const float koeffCompression = 5f;
        private int _maxIterationCount;
        private float area, verticeRadius;
        private Vector[] areaCenters;
        private Vector areaCorner;
        private double areaHeight;
        private double areaWidth, segmentSize;
        private double graphVerticesNumb, graphMetaverticesNumb, averageInnerVertNumber;
        private double ideallength;
        private double ideallength2;
        private double Kmetavert, KinnerVertToMv, KinnerToInner, Kgrav;
        private double[][] KrMatrix, KaMatrix;
        private double[][] KrMatrixWithoutFormula, KaMatrixWithoutFormula;
        private int levelsCount;
        private Vector magneticFieldOrientation, magneticFieldOrientationNegative;
        private double magneticStrength;
        private double maxTemperature;
        private int stepCount = 0;
        private double temperature;
        private Vector[] transforms;
        private int verticesCount;
        private VisualizationOrientation orientation;

        public TreeVisualizationAlgorithm(EdgeMetagraph<T, K> graph, Vector areaCorner, double areaWidth, double areaHeight,
          double maxTemperature = 5, float verticeRadius = 10,
          double Kmetavert = 10, double KinnerVertToMv = 1, double KinnerToInner = 1, double Kgrav = 5, int maxIterationCount = 1500,
          double magneticStrength = 0, VisualizationOrientation orientation = VisualizationOrientation.Vertical)
        {
            if (graph == null)
                Graph = new EdgeMetagraph<T, K>();
            else Graph = graph;
            CalculateLevels();
            this.verticeRadius = verticeRadius;

            verticesCount = graph.Vertices.Count;
            MaxTemperature = maxTemperature;
            MaxIterationCount = maxIterationCount;
            Area = (float)(areaWidth * areaHeight);
            KrMatrix = new double[verticesCount][];
            KaMatrix = new double[verticesCount][];

            for (int i = 0; i < verticesCount; i++)
            {
                KrMatrix[i] = new double[verticesCount];
                KaMatrix[i] = new double[verticesCount];
            }

            this.areaCorner = areaCorner;
            this.areaWidth = areaWidth;
            this.orientation = orientation;
            this.segmentSize = (this.orientation == VisualizationOrientation.Horizontal) ? areaWidth / levelsCount : areaHeight / levelsCount;
            this.areaHeight = areaHeight;
            this.areaCenters = new Vector[levelsCount];
            System.Diagnostics.Debug.WriteLine("area size {0} , {1}, {2}", areaWidth, areaHeight, segmentSize);
            for (int i = 0; i < levelsCount; i++)
            {
                areaCenters[i] = (this.orientation == VisualizationOrientation.Horizontal)
                ? new Vector((areaCorner.X + segmentSize * (i + 1) - segmentSize / 2), (areaCorner.Y + areaHeight) / 2)
                : new Vector((areaCorner.X + areaWidth) / 2, (areaCorner.Y + segmentSize * (i + 1)) - segmentSize / 2);
                System.Diagnostics.Debug.WriteLine("area center {0} = {1}", i, areaCenters[i]);

            }
            this.Kmetavert = Kmetavert;
            this.KinnerVertToMv = KinnerVertToMv;
            this.KinnerToInner = KinnerToInner;
            this.Kgrav = Kgrav;

            graphMetaverticesNumb = Graph.CountMetavertices();
            graphVerticesNumb = Graph.CountVertices();
            CalculateAverageInnerVertNumber();
            CalculateKoeffMatrix();
            magneticFieldOrientation = new Vector(0, 1);
            magneticFieldOrientationNegative = new Vector(0, -1);
            this.MagneticStrength = magneticStrength;
            System.Diagnostics.Debug.WriteLine("IL" + ideallength);
        }
        public float Area
        {
            get { return area; }
            set
            {
                CalculateIdealLength(area, value);
                area = value;
            }
        }

        public double GravitationCoefficient
        {
            get { return Kgrav; }
            set { Kgrav = value; }
        }

        public double[][] KAttrMatrix
        {
            get { return KaMatrix; }
            set { KaMatrix = value; }
        }

        public double[][] KAttrMatrixWithoutFormula1
        {
            get { return KaMatrixWithoutFormula; }
            set { KaMatrixWithoutFormula = value; }
        }

        public double[][] KRepMatrixWithoutFormula
        {
            get { return KrMatrixWithoutFormula; }
            set { KrMatrixWithoutFormula = value; }
        }

        public double[][] KRepullMatrix
        {
            get { return KrMatrix; }
            set { KrMatrix = value; }
        }

        public double MagneticStrength
        {
            get { return magneticStrength; }
            set { magneticStrength = value; }
        }

        public int MaxIterationCount
        {
            get { return _maxIterationCount; }
            set { _maxIterationCount = value; }
        }
        public double MaxTemperature
        {
            get { return maxTemperature; }
            set { maxTemperature = value; }
        }
        public void CalculateLevels()
        {
            foreach (MetaVertex<T> v in Graph.Vertices)
                if (v.Level > levelsCount)
                    levelsCount = v.Level;
            levelsCount++;
            System.Diagnostics.Debug.WriteLine("Levels " + levelsCount);
        }

        public override void RunAlgorithm()
        {
            Stopwatch sw = Stopwatch.StartNew();
            Vector centerOfGravity = new Vector((areaCorner.X + areaWidth) / 2, (areaCorner.Y + areaHeight) / 2);
            bool stop = false;
            double epsilon = 0.00000001;
            List<MetaVertex<T>> gv = Graph.Vertices;
            int vc = Graph.Vertices.Count;
            while (!stop && stepCount <= _maxIterationCount)
            {
                ++stepCount;

                transforms = new Vector[Graph.Vertices.Count];

                //Parallel.For(0, Graph.Vertices.Count, i =>
                for (int i = 0; i < vc; i++)
                {
                    for (int j = i; j < vc; j++)
                    {
                        bool isKR0 = KrMatrix[i][j] == 0 && KrMatrix[j][i] == 0;
                        bool isKA0 = KaMatrix[i][j] == 0 && KaMatrix[j][i] == 0;
                        if (!(isKR0 && isKA0))
                        {
                            Vector dir = gv[j].Coordinates - gv[i].Coordinates;
                            double dirLenSqv = dir.LengthSquared;
                            if (dirLenSqv < 0.0000001d)
                            {
                                Random rnd = new Random();
                                dir = new Vector(rnd.NextDouble() * 2 - 1, rnd.NextDouble() * 2 - 1);
                                dirLenSqv = dir.LengthSquared;
                            }
                            else if (dirLenSqv < 900)
                            {
                                Graph.Vertices[i].Coordinates.X += 10;
                                Graph.Vertices[i].Coordinates.Y += 10;
                                Graph.Vertices[j].Coordinates.X -= 10;
                                Graph.Vertices[j].Coordinates.Y -= 10;
                            }

                            double tt, tt2;
                            //repulsion
                            if (!isKR0 /*&& gv[i].Level == gv[i].Level*/)
                            {
                                tt = KrMatrix[i][j] / dirLenSqv;
                                tt2 = KrMatrix[j][i] / dirLenSqv;

                                transforms[i] = new Vector(
                                    transforms[i].X - dir.X * tt,
                                    transforms[i].Y - dir.Y * tt);
                                transforms[j] = new Vector(
                                    transforms[j].X + dir.X * tt2,
                                    transforms[j].Y + dir.Y * tt2);
                            }
                            //attraction
                            if (!isKA0)
                            {
                                double dirLen = Math.Sqrt(dirLenSqv);
                                tt = KaMatrix[i][j] * dirLen;
                                tt2 = KaMatrix[j][i] * dirLen;
                                transforms[i] = new Vector(
                                    transforms[i].X + dir.X * tt,
                                    transforms[i].Y + dir.Y * tt);
                                transforms[j] = new Vector(
                                    transforms[j].X - dir.X * tt2,
                                    transforms[j].Y - dir.Y * tt2);
                            }
                        }
                    }
                    if (transforms[i].LengthSquared <= epsilon)
                        stop = true;
                    //gravitation
                    transforms[i] += CalculateInhibition(Graph.Vertices[i]) * Kgrav *
                           MathHelper.GetDirection(Graph.Vertices[i].Coordinates, areaCenters[Graph.Vertices[i].Level]);

                }//);
                //magnetic force
                foreach (var edge in Graph.Edges)
                {
                    double angle = Vector.AngleBetween(MathHelper.GetDirection(edge.EndVertex.Coordinates, edge.StartVertex.Coordinates), magneticFieldOrientation);
                    if (angle > 180) angle -= 180;
                    angle *= Math.PI / 180;
                    double magneticForce = magneticStrength * angle * (edge.EndVertex.Coordinates - edge.StartVertex.Coordinates).Length;
                    int i = Graph.Vertices.IndexOf(edge.StartVertex);
                    int j = Graph.Vertices.IndexOf(edge.EndVertex);
                    transforms[i] += magneticForce * magneticFieldOrientation;
                    transforms[j] += magneticForce * magneticFieldOrientationNegative;
                }
                for (int i = 0; i < Graph.Vertices.Count; i++)
                {
                    if (orientation == VisualizationOrientation.Horizontal)
                        Graph.Vertices[i].Coordinates = new Vector(
                            Math.Min(
                                      areaCorner.X + segmentSize * (Graph.Vertices[i].Level + 1), Math.Max(areaCorner.X + segmentSize * Graph.Vertices[i].Level, Graph.Vertices[i].Coordinates.X
                                      + (transforms[i] / transforms[i].Length).X * Math.Min(temperature, Math.Abs(transforms[i].X)))
                                    ),
                            Math.Min(
                                     areaCorner.Y + areaHeight, Math.Max(areaCorner.Y, Graph.Vertices[i].Coordinates.Y
                                     + (transforms[i] / transforms[i].Length).Y * Math.Min(temperature, Math.Abs(transforms[i].Y)))
                                    )
                          );
                    if (orientation == VisualizationOrientation.Vertical)
                        Graph.Vertices[i].Coordinates = new Vector(
                            Math.Min(
                                      areaCorner.X + areaWidth, Math.Max(areaCorner.X, Graph.Vertices[i].Coordinates.X
                                      + (transforms[i] / transforms[i].Length).X * Math.Min(temperature, Math.Abs(transforms[i].X)))
                                    ),
                            Math.Min(
                                     areaCorner.Y + segmentSize * (Graph.Vertices[i].Level + 1), Math.Max(areaCorner.Y + segmentSize * (Graph.Vertices[i].Level), Graph.Vertices[i].Coordinates.Y
                                     + (transforms[i] / transforms[i].Length).Y * Math.Min(temperature, Math.Abs(transforms[i].Y)))
                                    )
                          );
                }

                //Cool
                temperature = Math.Max(0, temperature - maxTemperature / _maxIterationCount);
                //остановка алгоритма
                if (temperature == 0)
                    stop = true;
            }
            sw.Stop();
            //for (int i = 0; i < Graph.Vertices.Count; i++)
            //{
            //    System.Diagnostics.Debug.WriteLine(Graph.Vertices[i].Coordinates);
            //}
            //System.Diagnostics.Debug.WriteLine(String.Format("steps ={0}, temperature = {1} center = {2}", stepCount, temperature, centerOfGravity));
            stepCount = 0;
            temperature = maxTemperature;
            sw.Stop();
            System.Diagnostics.Debug.WriteLine(String.Format("TOTAL TIME: {0:N0} {1}", sw.ElapsedMilliseconds, MaxIterationCount));

        }

        private void ApplyKoeffFormulas()
        {
            //for (int i = 0; i < KrMatrix.Count; i++)
            //{
            //    for (int j = 0; j < KrMatrix.Count; j++)
            //    {
            //применение формулы
            //if (KrMatrix[i][j] != 0)
            //   KrMatrix[i][j] = 1 / KrMatrix[i][j];

            //if (KaMatrix[i][j] != 0)
            //    KaMatrix[i][j] = KaMatrix[i][j] * KaMatrix[i][j];
            //    }
            //}
            double maxKr = 0, maxKa = 0;
            for (int i = 0; i < verticesCount; i++)
            {
                for (int j = 0; j < verticesCount; j++)
                {
                    if (KrMatrix[i][j] > maxKr)
                        maxKr = KrMatrix[i][j];

                    if (KaMatrix[i][j] > maxKa)
                        maxKa = KaMatrix[i][j];
                }
            }
            for (int i = 0; i < verticesCount; i++)
            {
                for (int j = 0; j < verticesCount; j++)
                {
                    //Нормирование матриц
                    if (maxKr != 0)
                        KrMatrix[i][j] /= maxKr;
                    if (maxKa != 0)
                        KaMatrix[i][j] /= maxKa;
                }
            }
        }

        private void CalculateAverageInnerVertNumber()
        {
            int sumInner = 0;
            for (int i = 0; i < Graph.Vertices.Count; i++)
            {
                if (Graph.Vertices[i].IsMetavertex)
                {
                    sumInner += Graph.Vertices[i].IncludedVertices.Count;
                }
            }
            averageInnerVertNumber = sumInner / graphMetaverticesNumb;
        }

        private void CalculateIdealLength(double oldArea, double newArea)
        {
            ideallength = Math.Sqrt(newArea / koeffCompression / Graph.Vertices.Count);
            ideallength2 = ideallength * ideallength;

            if (oldArea == 0)
                return;

            double oideallength = Math.Sqrt(oldArea / koeffCompression / Graph.Vertices.Count);
            double oideallength2 = ideallength * ideallength;

            RefreshKMatrices(oideallength, oideallength2);
        }

        private float CalculateInhibition(MetaVertex<T> v)
        { return (1 + Graph.CountIncedentEdgesForVertex(v) / 2); }

        private double CalculateKinnerToInner(MetaVertex<T> parentMetavertex)
        {
            //System.ServerProxy.Instance.UserInfo.Userics.Debug.WriteLine("averageInnerVertNumber= " + averageInnerVertNumber);
            return KinnerToInner
                * averageInnerVertNumber
                ;
        }

        private double CalculateKinnerVertToMv(MetaVertex<T> v)
        {
            return KinnerVertToMv
                * (v.IncludedVertices.Count + (Graph.CountIncedentEdgesForVertex(v) + 1))
                ;
        }

        private void CalculateKoeffMatrix()
        {
            for (int i = 0; i < Graph.Vertices.Count; i++)
            {
                for (int j = 0; j < Graph.Vertices.Count; ++j)
                {
                    if (i != j && !Graph.Vertices[i].IncludedVertices.Contains(Graph.Vertices[j])
                        && !Graph.Vertices[j].IncludedVertices.Contains(Graph.Vertices[i]))
                    {
                        //if (Graph.Vertices[i].Level == Graph.Vertices[j].Level)
                        KrMatrix[i][j] = CalculateWeightKoeff(Graph.Vertices[i], Graph.Vertices[j]);
                        if (Graph.VerticeAjacency(Graph.Vertices[i], Graph.Vertices[j]))
                        {
                            KaMatrix[i][j] = CalculateWeightKoeff(Graph.Vertices[i], Graph.Vertices[j]);
                        }
                    }
                    else KrMatrix[i][j] = 0;
                }
                // Для всех вершин, у которых есть вложенные
                if (Graph.Vertices[i].IsMetavertex)
                {
                    for (int j = 0; j < Graph.Vertices[i].IncludedVertices.Count; j++)
                    {
                        int m = Graph.Vertices.IndexOf(Graph.Vertices[i].IncludedVertices[j]);
                        KaMatrix[i][m] = CalculateKinnerVertToMv(Graph.Vertices[i]);
                        //если есть ребро от внутренней к любой внешней
                        //for (int k = 0; k < Graph.Vertices.Count; k++)
                        //{
                        //    if (Graph.VerticeAjacency(m, k))
                        //    {
                        //        KaMatrix[m][k] = 1 / averageInnerVertNumber;
                        //        KrMatrix[m][k] = 1 / averageInnerVertNumber;
                        //        if (Graph.Vertices[k].IsMetavertex)
                        //        {
                        //            KaMatrix[m][k] = (Graph.Vertices[k].IncludedVertices.Count + 1) / 2;

                        //        }
                        //    }

                        //}
                        for (int k = j + 1; k < Graph.Vertices[i].IncludedVertices.Count; k++)
                        {
                            int n = Graph.Vertices.IndexOf(Graph.Vertices[i].IncludedVertices[k]);
                            KaMatrix[m][n] = CalculateKinnerToInner(Graph.Vertices[i]);
                            KaMatrix[n][m] = CalculateKinnerToInner(Graph.Vertices[i]);
                            KrMatrix[m][n] = CalculateKinnerToInner(Graph.Vertices[i]);
                            KrMatrix[n][m] = CalculateKinnerToInner(Graph.Vertices[i]);
                        }
                    }
                }
            }
            KaMatrixWithoutFormula = new double[verticesCount][];
            KrMatrixWithoutFormula = new double[verticesCount][];
            for (int i = 0; i < verticesCount; i++)
            {
                KaMatrixWithoutFormula[i] = new double[verticesCount];
                KrMatrixWithoutFormula[i] = new double[verticesCount];

                for (int j = 0; j < verticesCount; j++)
                {
                    KaMatrixWithoutFormula[i][j] = KaMatrix[i][j];
                    KrMatrixWithoutFormula[i][j] = KrMatrix[i][j];
                }
                //KaMatrixWithoutFormula.Add(new List<double>());
                //KaMatrix[i].CopyTo(KaMatrixWithoutFormula[i].ToArray());
                //KrMatrixWithoutFormula.Add(new List<double>());
                //KrMatrix[i].CopyTo(KaMatrixWithoutFormula[i].ToArray());
            }

            ApplyKoeffFormulas();

            RefreshKMatrices();
        }

        private double CalculateWeightKoeff(MetaVertex<T> sourse, MetaVertex<T> target)
        {

            double res = Kmetavert
                * (sourse.IsMetavertex ? sourse.IncludedVertices.Count : 1
                + (target.IsMetavertex ? target.IncludedVertices.Count : 1)) / 2;
            if (sourse.IsMetavertex && target.IsMetavertex && sourse.Level == target.Level && target.Level != 0)
                res *= 2;
            return res;
        }

        private void RefreshKMatrices(double oideallength = 1, double oideallength2 = 1)
        {
            for (int i = 0; i < verticesCount; i++)
            {
                for (int j = 0; j < verticesCount; j++)
                {
                    KrMatrix[i][j] = KrMatrix[i][j] / oideallength2 * ideallength2;
                    KrMatrixWithoutFormula[i][j] = KrMatrixWithoutFormula[i][j] / oideallength2 * ideallength2;
                    KaMatrix[i][j] = KaMatrix[i][j] * oideallength / ideallength / CalculateInhibition(Graph.Vertices[i]);
                    KaMatrixWithoutFormula[i][j] = KaMatrixWithoutFormula[i][j] * oideallength / ideallength / CalculateInhibition(Graph.Vertices[i]);
                }
            }
        }
    }
}
