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
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Threading;

namespace WebApplication1
{
    public enum VertexShapes { box, polygon, ellipse, oval, circle, triangle, diamond, pentagon, hexagon, note, square };
    public enum ArrowShapes { box, crow, curve, diamond, dot, inv, none, normal, tee, vee };
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">Metavertex content Type</typeparam>
    /// <typeparam name="K">Edge content Type</typeparam>
    public class Visualizator<T, K>
    {
        public EdgeMetagraph<T, K> Graph { get; set; }
        private List<TextBlock> textElements;

        private Canvas canvas;
        private const float defaultDiametr = 10;
        private const float defaultMvDiametr = 20;
        private const int arrowDiameter = 5;
        private bool textNamesEnabled;
        public bool TextNamesEnabled
        {
            get { return textNamesEnabled; }
            set
            {
                System.Diagnostics.Debug.WriteLine(textNamesEnabled);
                System.Diagnostics.Debug.WriteLine(value);
                if (value == true)
                {
                    if (textElements == null)
                        CreateTextElements();
                    else if (textElements.Count > 0)
                    {
                        RefreshTextElements();
                        for (int i = 0; i < Graph.Vertices.Count; i++)
                        {
                            textElements[i].Visibility = Visibility.Visible;
                        }
                    }
                }
                else if (textElements != null)
                {
                    for (int i = 0; i < textElements.Count; i++)
                    {
                        textElements[i].Visibility = Visibility.Hidden;
                    }
                }
                textNamesEnabled = value;
            }
        }

        public Visualizator(EdgeMetagraph<T, K> graph = null)
        {
            TextNamesEnabled = false;

            if (graph == null)
                Graph = new EdgeMetagraph<T, K>();
            else Graph = graph;

            СreateAllShapes();
        }

        public void VisualizeGraph()
        {
            for (int i = 0; i < Graph.Vertices.Count; i++)
            {
                if (Graph.Vertices[i].IncludedVertices.Count == 1)
                    Graph.Vertices[i].Coordinates = Graph.Vertices[i].IncludedVertices[0].Coordinates;
                if (Graph.Vertices[i].IncludedVertices.Count == 2)
                    Graph.Vertices[i].Coordinates =
                        new Vector((Graph.Vertices[i].IncludedVertices[0].Coordinates.X
                                    + Graph.Vertices[i].IncludedVertices[1].Coordinates.X) / 2,
                                    (Graph.Vertices[i].IncludedVertices[0].Coordinates.Y
                                    + Graph.Vertices[i].IncludedVertices[1].Coordinates.Y) / 2);
            }

            //canvas.Children.Clear();
            RecreateAllShapes();
        }

        private void СreateAllShapes()
        {
            //Создание вершин
            for (int i = 0; i < Graph.Vertices.Count; i++)
            {
                if (Graph.Vertices[i].IsMetavertex)
                {
                    Graph.Vertices[i].DrawingShape = GetMetaVertexShape(Graph.Vertices[i],
                        new SolidColorBrush(Graph.Vertices[i].Color), new SolidColorBrush(Graph.Vertices[i].Color));
                }
                else
                    Graph.Vertices[i].DrawingShape = GetEllipseShape((float)Graph.Vertices[i].Coordinates.X,
                            (float)Graph.Vertices[i].Coordinates.Y, defaultDiametr, defaultDiametr,
                            new SolidColorBrush(Graph.Vertices[i].Color));

                //canvas.Children.Add(Graph.Vertices[i].DrawingShape);
            }

            //создание линий
            for (int i = 0; i < Graph.Edges.Count; ++i)
            {
                SetEdgeLine(Graph.Edges[i], Brushes.Black);
                //canvas.Children.Add(Graph.Edges[i].DrawingShape);
                //canvas.Children.Add(Graph.Edges[i].ArrowShape);
            }

            //textElements
            if (TextNamesEnabled)
            {
                CreateTextElements();
            }
            //System.Diagnostics.Debug.WriteLine("textElements Count: " + textElements.Count + "canvas Children.Count: " + canvas.Children.Count);

        }

        private void CreateTextElements()
        {
            textElements = new List<TextBlock>();
            for (int i = 0; i < Graph.Vertices.Count; i++)
            {
                TextBlock text = new TextBlock();
                text.Text = Graph.Vertices[i].Name;
                text.FontSize = 10;
                text.RenderTransform = new TranslateTransform(Graph.Vertices[i].Coordinates.X, Graph.Vertices[i].Coordinates.Y);
                text.Foreground = Brushes.Black;
                textElements.Add(text);
                //canvas.Children.Add(text);
            }
        }
        private void RefreshTextElements()
        {
            for (int i = 0; i < Graph.Vertices.Count; i++)
            {
                textElements[i].RenderTransform = new TranslateTransform(Graph.Vertices[i].Coordinates.X, Graph.Vertices[i].Coordinates.Y);
            }
        }

        private void RecreateAllShapes()
        {
            //Создание вершин
            for (int i = 0; i < Graph.Vertices.Count; i++)
            {
                if (Graph.Vertices[i].IsMetavertex)
                {
                    if (Graph.Vertices[i].IncludedVertices.Count > 2)
                    {
                        List<Point> shapePoints = GrahamAlgorithm.ConvexHull(Graph.Vertices[i].IncludedVertices.Select(e => (Point)e.Coordinates).ToList());
                        double x = 0, y = 0;
                        for (int j = 0; j < shapePoints.Count; j++)
                        {
                            x += shapePoints[j].X;
                            y += shapePoints[j].Y;
                        }
                        x /= shapePoints.Count;
                        y /= shapePoints.Count;

                        List<LineSegment> lineSegments = new List<LineSegment>();

                        Graph.Vertices[i].Coordinates = new Vector(x, y);
                        PathFigure myPathFigure = new PathFigure();
                        Graph.Vertices[i].PF.StartPoint = shapePoints[0];

                        for (int j = 0; j < shapePoints.Count; j++)
                        {
                            lineSegments.Add(new LineSegment(shapePoints[j], true));
                        }
                        lineSegments.Add(new LineSegment(shapePoints[0], true));

                        Graph.Vertices[i].PF.Segments.Clear();

                        PathSegmentCollection myPathSegmentCollection = new PathSegmentCollection();

                        for (int j = 0; j < lineSegments.Count; j++)
                        {
                            myPathSegmentCollection.Add(lineSegments[j]);
                        }

                        Graph.Vertices[i].PF.Segments = myPathSegmentCollection;
                    }
                    if (Graph.Vertices[i].IncludedVertices.Count == 2)
                    {
                        Graph.Vertices[i].Coordinates = new Vector((Graph.Vertices[i].IncludedVertices[0].Coordinates.X
                            + Graph.Vertices[i].IncludedVertices[1].Coordinates.X) / 2,
                            (Graph.Vertices[i].IncludedVertices[0].Coordinates.Y + Graph.Vertices[i].IncludedVertices[1].Coordinates.Y) / 2);
                        double height = (Graph.Vertices[i].IncludedVertices[1].Coordinates
                            - Graph.Vertices[i].IncludedVertices[0].Coordinates).Length + defaultDiametr * 2.5;
                        double angle = Vector.AngleBetween(Graph.Vertices[i].IncludedVertices[0].Coordinates - Graph.Vertices[i].Coordinates,
                            new Vector(0, 1));
                        Ellipse myEllipse = (Ellipse)Graph.Vertices[i].DrawingShape;
                        myEllipse.Width = defaultMvDiametr;
                        myEllipse.Height = height;

                        TransformGroup trGr = new TransformGroup();
                        trGr.Children.Add(new RotateTransform(-angle, defaultDiametr, height / 2));
                        trGr.Children.Add(new TranslateTransform(Graph.Vertices[i].Coordinates.X + -defaultDiametr,
                                                                 Graph.Vertices[i].Coordinates.Y + -height / 2));
                        myEllipse.RenderTransform = trGr;
                    }
                }
                else
                {
                    //Graph.Vertices[i].DrawingShape = GetEllipseShape((float)Graph.Vertices[i].Coordinates.X,
                    //        (float)Graph.Vertices[i].Coordinates.Y, defaultDiametr, defaultDiametr,
                    //        new SolidColorBrush(Graph.Vertices[i].Color));

                    Graph.Vertices[i].DrawingShape.RenderTransform = new TranslateTransform(
                        (float)Graph.Vertices[i].Coordinates.X + -defaultDiametr / 2,
                        (float)Graph.Vertices[i].Coordinates.Y + -defaultDiametr / 2
                        );
                }
            }

            //создание линий
            for (int i = 0; i < Graph.Edges.Count; ++i)
            {
                RefreshLinePosition(Graph.Edges[i]);

            }

            //textElements
            if (TextNamesEnabled)
            {
                if (textElements.Count == 0)
                    CreateTextElements();
                else
                    RefreshTextElements();
            }
        }

        public void SetEdgeLine(Edge<T, K> edge, System.Windows.Media.Brush color = default(System.Windows.Media.Brush))
        {
            Line line = new Line();
            line.Stroke = color;
            line.HorizontalAlignment = HorizontalAlignment.Left;
            line.VerticalAlignment = VerticalAlignment.Top;
            line.StrokeThickness = 1;
            edge.DrawingShape = line;
            edge.ArrowShape = GetEllipseShape((float)edge.EndVertex.Coordinates.X,
                           (float)edge.EndVertex.Coordinates.Y, arrowDiameter, arrowDiameter,
                           Brushes.Black);
            RefreshLinePosition(edge);

        }

        private Vector Calc(Vector center, Vector outerCenter, IEnumerable<Vector> corners)
        {
            if (corners.Count() < 3)
                throw new ArgumentException("corners have to be >= 3");
            //normalize
            outerCenter -= center;
            List<Vector> normCorner = new List<Vector>();
            foreach (var item in corners)
            {
                normCorner.Add(item - center);
            }
            //find 2 points
            double needingFi = MathHelper.Fi(outerCenter);
            double temp;
            double min1 = double.MaxValue;
            int i1 = 0;
            //Console.WriteLine("need "+needingFi);
            for (int i = 0; i < normCorner.Count; i++)
            {
                temp = MathHelper.Fi(normCorner[i]) - needingFi;
                //Console.WriteLine(i+" "+(temp+needingFi));

                if (temp < 0)
                    temp *= -1;
                if (min1 > temp)
                {
                    i1 = i;
                    min1 = temp;
                }
            }
            //Console.WriteLine("take " + i1);
            Vector p1 = normCorner[i1];

            normCorner.RemoveAt(i1);
            double min2 = double.MaxValue;
            int i2 = 0;
            for (int i = 0; i < normCorner.Count; i++)
            {
                temp = MathHelper.Fi(normCorner[i]) - needingFi;
                if (temp < 0)
                    temp *= -1;
                if (min2 > temp)
                {
                    i2 = i;
                    min2 = temp;
                }
            }
            Vector p2 = normCorner[i2];

            //equation
            double x, y;
            x = (p1.X * (p2.Y - p1.Y) - p1.Y * (p2.X - p1.X)) /
                ((p2.Y - p1.Y) - outerCenter.Y / outerCenter.X * (p2.X - p1.X));
            y = x * outerCenter.Y / outerCenter.X;
            Vector v = new Vector(x, y);
            //System.Diagnostics.Debug.WriteLine("v " + v);
            //System.Diagnostics.Debug.WriteLine("p1 " + p1);
            //System.Diagnostics.Debug.WriteLine("p2 " + p2);
            outerCenter.Normalize();
            return v.Length * outerCenter;
        }

        public void RefreshLinePosition(Edge<T, K> edge)
        {
            Line line = (Line)edge.DrawingShape;
            line.X1 = edge.StartVertex.Coordinates.X;
            line.Y1 = edge.StartVertex.Coordinates.Y;
            line.X2 = edge.EndVertex.Coordinates.X;
            line.Y2 = edge.EndVertex.Coordinates.Y;
            if (edge.EndVertex.IncludedVertices.Count > 2)
            {
                edge.ArrowShape.RenderTransform = new TranslateTransform(
                       edge.EndVertex.Coordinates.X - arrowDiameter / 2,
                       edge.EndVertex.Coordinates.Y - arrowDiameter / 2);
            }
            Vector newPointTemp = new Vector(0, 0);

            if (edge.StartVertex.IncludedVertices.Count <= 2)
            {
                newPointTemp = GetLineEndForEllipseVert(edge.StartVertex, edge.EndVertex);
                //else
                //    newPointTemp = Calc(startVertex.Coordinates, endVertex.Coordinates, startVertex.IncludedVertices.Select<MetaVertex<T>, Vector>(e => e.Coordinates));
                line.X1 += newPointTemp.X;
                line.Y1 += newPointTemp.Y;

            }
            if (edge.EndVertex.IncludedVertices.Count <= 2)
            {
                newPointTemp = GetLineEndForEllipseVert(edge.EndVertex, edge.StartVertex);
                //else
                //    newPointTemp = Calc(endVertex.Coordinates, startVertex.Coordinates, endVertex.IncludedVertices.Select<MetaVertex<T>, Vector>(e => e.Coordinates));
                line.X2 += newPointTemp.X;
                line.Y2 += newPointTemp.Y;
                edge.ArrowShape.RenderTransform = new TranslateTransform(
                    newPointTemp.X + edge.EndVertex.Coordinates.X - arrowDiameter / 2,
                    newPointTemp.Y + edge.EndVertex.Coordinates.Y - arrowDiameter / 2);

            }

        }

        private Vector GetLineEndForEllipseVert(MetaVertex<T> startVertex, MetaVertex<T> endVertex)
        {
            double l = 0;
            Vector direction = endVertex.Coordinates - startVertex.Coordinates;
            if (direction.Length == 0)
                return endVertex.Coordinates;

            direction.Normalize();
            switch (startVertex.IncludedVertices.Count)
            {
                case 0:
                    l = defaultDiametr / 2;
                    break;
                case 1:
                    l = defaultMvDiametr / 2;
                    break;
                case 2:

                    Vector ellipseLane = startVertex.IncludedVertices[0].Coordinates - startVertex.Coordinates;
                    ellipseLane.Normalize();
                    double resultAngle = Vector.AngleBetween(direction, ellipseLane);
                    Vector temp = new Vector(Math.Max(startVertex.DrawingShape.Height / 2, startVertex.DrawingShape.Width / 2),
                        Math.Min(startVertex.DrawingShape.Height / 2, startVertex.DrawingShape.Width / 2));
                    l = MathHelper.GetRadiusToPointOnEllipse(
                        temp.X, temp.Y,
                        resultAngle)
                        //- new Vector(startVertex.DrawingShape.Width / 2, startVertex.DrawingShape.Height / 2).Length
                        ;
                    //System.Diagnostics.Debug.WriteLine("temp" + temp);
                    //System.Diagnostics.Debug.WriteLine("dir " + direction);
                    //System.Diagnostics.Debug.WriteLine("eL  " + ellipseLane);
                    //System.Diagnostics.Debug.WriteLine("ang " + resultAngle);
                    //System.Diagnostics.Debug.WriteLine("l   " + l);
                    break;
            }

            return direction * l;
        }

        private void RefreshEllipsePosition(Ellipse el, double newX, double newY, double newWidth, double newHeight)
        {
            el.RenderTransform = new TranslateTransform(newX + -newWidth / 2, newY + -newHeight / 2);
            el.Width = newWidth;
            el.Height = newHeight;
        }

        private void RefreshVertexShape(Shape vertShape, MetaVertex<T> v, Brush strokeColor = default(Brush))
        {
            if (v.IncludedVertices.Count != 0)
            {
                canvas.Children.Remove(vertShape);
                vertShape = GetMetaVertexShape(v, vertShape.Fill, strokeColor);
                canvas.Children.Add(vertShape);
            }
            else
            {
                RefreshEllipsePosition((Ellipse)vertShape, v.Coordinates.X, v.Coordinates.Y,
                    defaultDiametr, defaultDiametr);
            }
        }

        public Ellipse GetEllipseShape(float x, float y, float height, float width,
            System.Windows.Media.Brush color = default(System.Windows.Media.Brush))
        {
            Ellipse myEllipse = new Ellipse();
            // mySolidColorBrush.Color = Color.FromArgb(255, 255, 255, 0);
            myEllipse.Fill = color;
            myEllipse.StrokeThickness = 1;
            myEllipse.Stroke = Brushes.Black;
            myEllipse.HorizontalAlignment = HorizontalAlignment.Left;
            myEllipse.VerticalAlignment = VerticalAlignment.Top;
            myEllipse.Width = width;
            myEllipse.Height = height;
            myEllipse.RenderTransform = new TranslateTransform(x + -width / 2, y + -height / 2);

            return myEllipse;
        }
        public Shape GetMetaVertexShape(MetaVertex<T> v, Brush color = default(Brush),
            Brush strokeColor = default(Brush))
        {
            Ellipse myEllipse;
            if (v.IncludedVertices.Count > 2)
            {
                List<Point> shapePoints = GrahamAlgorithm.ConvexHull(v.IncludedVertices.Select(e => (Point)e.Coordinates).ToList());
                //определение центра многоугольника и перенос координаты метавершины
                double x = 0, y = 0;
                for (int i = 0; i < shapePoints.Count; i++)
                {
                    x += shapePoints[i].X;
                    y += shapePoints[i].Y;
                }
                x /= shapePoints.Count;
                y /= shapePoints.Count;
                v.Coordinates = new Vector(x, y);
                PathFigure myPathFigure = new PathFigure();
                myPathFigure.StartPoint = shapePoints[0];
                List<LineSegment> lineSegments = new List<LineSegment>();

                for (int i = 0; i < shapePoints.Count; i++)
                {
                    lineSegments.Add(new LineSegment(shapePoints[i], true));
                }

                lineSegments.Add(new LineSegment(shapePoints[0], true));

                PathSegmentCollection myPathSegmentCollection = new PathSegmentCollection();

                for (int i = 0; i < lineSegments.Count; i++)
                {
                    myPathSegmentCollection.Add(lineSegments[i]);
                }

                myPathFigure.Segments = myPathSegmentCollection;
                v.PF = myPathFigure;
                PathFigureCollection myPathFigureCollection = new PathFigureCollection();
                myPathFigureCollection.Add(myPathFigure);

                PathGeometry myPathGeometry = new PathGeometry();
                myPathGeometry.Figures = myPathFigureCollection;
                TransformGroup trgr = new TransformGroup();
                trgr.Children.Add(new ScaleTransform(1, 1, x, y));
                myPathGeometry.Transform = trgr;

                Path myPath = new Path();
                myPath.Stroke = strokeColor;
                myPath.StrokeThickness = 3;
                myPath.Fill = color;
                myPath.Data = myPathGeometry;

                return myPath;
            }
            else if (v.IncludedVertices.Count == 2)
            {
                v.Coordinates = new Vector((v.IncludedVertices[0].Coordinates.X
                    + v.IncludedVertices[1].Coordinates.X) / 2,
                    (v.IncludedVertices[0].Coordinates.Y + v.IncludedVertices[1].Coordinates.Y) / 2);
                double height = (v.IncludedVertices[1].Coordinates
                    - v.IncludedVertices[0].Coordinates).Length + defaultDiametr * 2.5;
                double angle = Vector.AngleBetween(v.IncludedVertices[0].Coordinates - v.Coordinates,
                    new Vector(0, 1));
                myEllipse = new Ellipse();
                myEllipse.Fill = color;
                myEllipse.StrokeThickness = 3;
                myEllipse.Stroke = strokeColor;
                myEllipse.HorizontalAlignment = HorizontalAlignment.Left;
                myEllipse.VerticalAlignment = VerticalAlignment.Top;
                myEllipse.Width = defaultMvDiametr;
                myEllipse.Height = height;

                TransformGroup trGr = new TransformGroup();
                trGr.Children.Add(new RotateTransform(-angle, defaultDiametr, height / 2));
                trGr.Children.Add(new TranslateTransform(v.Coordinates.X + -defaultDiametr,
                                                         v.Coordinates.Y + -height / 2));
                myEllipse.RenderTransform = trGr;

                return myEllipse;
            }
            //double height = v.GetMaxDistForInnerVert() * 2 + defaultDiametr * 2;
            //double angle = Vector.AngleBetween(v.IncludedVertices[0].Coordinates 
            //- v.Coordinates, new Vector(0,1));
            myEllipse = new Ellipse();
            myEllipse.Fill = color;
            myEllipse.StrokeThickness = 3;
            myEllipse.Stroke = strokeColor;
            myEllipse.HorizontalAlignment = HorizontalAlignment.Left;
            myEllipse.VerticalAlignment = VerticalAlignment.Top;
            myEllipse.Width = defaultMvDiametr;
            myEllipse.Height = myEllipse.Width = defaultMvDiametr;
            //myEllipse.Width = defaultDiametr * 3;
            //myEllipse.Height = height;
            //TransformGroup trGr = new TransformGroup();
            //trGr.Children.Add(new RotateTransform(-angle, defaultDiametr * 2, height/2));
            //trGr.Children.Add(new TranslateTransform(v.Coordinates.X +  - defaultDiametr * 2,
            //                                         v.Coordinates.Y +  - height/2));
            //myEllipse.RenderTransform = trGr;
            v.Coordinates = v.IncludedVertices[0].Coordinates;
            myEllipse.RenderTransform = new TranslateTransform(v.Coordinates.X + -defaultDiametr,
                                                    v.Coordinates.Y + -defaultDiametr);
            return myEllipse;
        }
    }
}
