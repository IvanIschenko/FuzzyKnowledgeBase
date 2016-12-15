using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Functions;
using L.DataStructures.Matrix;
using L.DataStructures.Geometry;
//using System.MarshalByRefObject;
//using System.Drawing.Image;
//using System.Drawing.Bitmap;
//using System.Drawing.Drawing2D.GraphicsPath;

using WebApplication1.FuzzyLogicBase;


namespace L.Algorithms.Clustering
{
    public class K_means
    {
        public static Guid ID = new Guid();
        public List<MultiDimensionalVector> Elements { get; set; }
        public int ClusterCount;
        public float ExponentWeight;
        public List<MultiDimensionalVector> Centroids { get; set; }
        public static List<Cluster> Clusters { get; set; }

        private double[,] DistanceMatrix;   //матрица расстояний от элементов до центроидов по (i) - элементы, по (j) - центроиды


        /// матрица принадлежности элементов кластерам (матрица нечеткого разбиения) по (i) - элементы, по (j) - кластеры
        /// условия для матрицы:
        /// сумма в столбце = 1 - каждый элемент распределен между всеми кластерами 
        /// 0 < сумма в строке < число элементов  - ни один кластер не должен быть пустым или содержать все элементы
        public double[,] MembershipMatrix;
        private double[,] MembershipMatrixTemp;
        private double[,] ElementsMatrix;  // data from the file
        public double[,] ValueGausFPForEachTerm;
        private double[,] ValueLaplassFPForEachTerm;

        public static string[,] NameOfTermsByWords, NameOfTermsByWordsWhithoutRepeat;

        private Random rand = new Random();
        //public K_means(List<MultiDimensionalVector> Elements) : this(Elements, null) { }
        public K_means(List<MultiDimensionalVector> Elements, List<MultiDimensionalVector> Centroids, int ClusterCount, double[,] ElementsMatrix)
        {
            this.Elements = Elements;
            this.ClusterCount = ClusterCount;
            this.Centroids = Centroids;
            Clusters = new List<Cluster>();
            this.ElementsMatrix = ElementsMatrix;
        }

        /*#region InitMembershipMatrix222
        public void InitMembershipMatrix222() //инициализация матрицы нечеткого разбиения (рандомом определяются элементы для матрицы, матрицы генерируеться согласно условиям, которые на нее накладываются)
        {
            MembershipMatrix = new double[Elements.Count, ClusterCount];
            // double rowSum = 0; //дописать проверку суммы
            double columnSum = 0;

            for (int j = 0; j < Elements.Count; ++j)
            {
                for (int i = 0; i < ClusterCount; ++i)
                {
                    while (true)
                    {
                        double nextMatrixElement = rand.NextDouble();
                        if (columnSum == 1) 
                        { 
                            nextMatrixElement = 0; 
                        }
                        columnSum += nextMatrixElement;
                        if (columnSum <= 1)
                        {
                            MembershipMatrix[i, j] = nextMatrixElement;
                            break;
                        }
                        else
                        {
                            MembershipMatrix[i, j] = 1 - columnSum;
                        }
                    }
                }

            }
        }
        #endregion*/

        public void InitCentroids()  //инициализация центроидов для  кластеров (рандомом выбираются элементы из выборки, которая кластеризуеться Elements)
        {
            Centroids = new List<MultiDimensionalVector>();
            Centroids.Clear();
            for (int i = 0; i < ClusterCount; ++i)
            {
                Centroids.Add(Elements.ElementAt(rand.Next(Elements.Count - 1)));
            }
        }

        public void FillDistanceMatrix()  //заполнение матрицы расстояний - расстояния от элементов до центроидов по (i) - элементы, по (j) - центроиды
        {
            DistanceMatrix = new double[Elements.Count, Centroids.Count];
            for (int i = 0; i < Elements.Count; ++i)
            {
                for (int j = 0; j < Centroids.Count; ++j)
                {
                    DistanceMatrix[i, j] = Elements.ElementAt(i).EuclideanDistance(Centroids.ElementAt(j));
                }
            }
        }

        public void InitMembershipMatrix() //для генерироваиня случайной матрицы нечеткого разбиения, случайно выбираются центроиды и матрица рассчитывается по формуле для ее вычисления
        {
            InitCentroids();
            MembershipMatrix = new double[Elements.Count, Centroids.Count]; // changed from [Centroids.Count, Elements.Count]
            MembershipMatrixTemp = new double[Elements.Count, Centroids.Count]; ///
            FillDistanceMatrix();
            ReCalculateMembershipMatrix();
        }

        public void ReCalculateMembershipMatrix()
        {
            //  double stepen = 2; //http://matlab.exponenta.ru/fuzzylogic/book1/12.php
            //  double stepen = 2/(ExponentWeight-1); //http://www.iis.nsk.su/files/articles/sbor_kas_13_taraskina.pdf
            double sum = 0;

            for (int i = 0; i < Elements.Count; ++i)  //changed for Elements.Count
            {
                for (int j = 0; j < ClusterCount; ++j)
                {
                    if (DistanceMatrix[i, j] > 0)
                    {
                        for (int k = 0; k < ClusterCount; ++k)
                        {
                            sum = sum + 1 / Math.Pow(DistanceMatrix[k, j], 2);
                        }
                        MembershipMatrixTemp[i, j] = 1 / (Math.Pow(DistanceMatrix[i, j], 2) * sum);
                        sum = 0;
                    }
                    else
                    {
                        if (i == j)
                        {
                            MembershipMatrixTemp[i, j] = 0;
                        }
                        else
                        {
                            MembershipMatrixTemp[i, j] = 1;
                        }
                    }
                }
            }
        }


        public MultiDimensionalVector FindClosestCentroid(MultiDimensionalVector element)  //нахождение ближайшего центроида для элемента
        {
            int indexElement = Elements.IndexOf(element);
            int indexClosestCentroid = 0;
            double min = double.MaxValue;

            for (int j = 0; j < Centroids.Count; ++j)
            {
                if (DistanceMatrix[indexElement, j] < min)
                {
                    min = DistanceMatrix[indexElement, j];
                    indexClosestCentroid = j;
                }
            }
            MultiDimensionalVector ClosestCentroid = Centroids.ElementAt(indexClosestCentroid);
            return ClosestCentroid;
        }

        public void FillClusters()  //отнесение каждого элемента к кластеру по признаку к ближайшему центроиду
        {
            Clusters.Clear();
            foreach (var c in Centroids)
            {
                Cluster currentCluster = new Cluster(c);
                Clusters.Add(currentCluster);
            }

            foreach (var e in Elements)  //проход по всем элементам и добавление каждого элемента в кластер к самому ближнему центроиду
            {
                Clusters.First(t => t.Centroid == FindClosestCentroid(e)).Elements.Add(e);
            }
        }

        public MultiDimensionalVector CalculateCentroid(Cluster cluster)  //вычисление центроида для кластера и обновление значения центроида в кластере
        {
            MultiDimensionalVector centroid = new MultiDimensionalVector();
            for (int i = 0; i < cluster.Centroid.Count; ++i)
            {
                centroid.Add(0.0);
            }

            if (cluster.Elements.Count > 0)
            {
                foreach (var e in cluster.Elements)
                {
                    centroid = centroid + e;
                }
                centroid /= cluster.Elements.Count;
                cluster.Centroid = centroid;
            }
            else { centroid = this.Elements.ElementAt(rand.Next(Elements.Count - 1)); }
            return centroid;
        }

        public void ReCalculateCentroids()  //вычисление новых центроидов для всех кластеров на основе элементов кластеров
        {
            foreach (var currentCluster in Clusters)
            {
                CalculateCentroid(currentCluster);
            }
        }

        public bool EqualPrevAndNewCentroids(double epsilon)  //сравнение новых вычисленных центроидов для каждого кластера Cluster.Centroid с имеющимися центроидами (Centroids), вычисление с погрешностью epsilon
        {
            bool equivalent = true;
            for (int i = 0; i < Centroids.Count; ++i)
            {
                //if (Centroids.ElementAt(i) == Clusters.ElementAt(i).Centroid)
                MultiDimensionalVector differentVector = Centroids.ElementAt(i) - Clusters.ElementAt(i).Centroid;
                foreach (var coord in differentVector)
                {
                    if (Math.Abs(coord) > epsilon)
                    {
                        equivalent = false;
                        break;
                    }
                }
            }
            return equivalent;
        }

        public void UpdateCentroids()  // обновление списка центроидов - запись центроидов из каждого кластера в Centroids
        {
            Centroids.Clear();
            foreach (var c in Clusters)
            {
                Centroids.Add(c.Centroid);
            }
        }

        //рисование кластеров для двухмерного случая
        //для одномерного задать у-константой
        /*public void DrawClusters(int iteration, string filePath)
        {
            int radius = 3;
            int x = 0;
            const int y = 100;


            Bitmap img = new Bitmap(1000, 1000);

            Graphics ClusterPath = Graphics.FromImage(img);
            ClusterPath.Clear(Color.SkyBlue); //skyBlue

            //  Pen PenCentroid = new Pen(FillColor.FromArgb((byte)rand.Next(255), (byte)rand.Next(255), (byte)rand.Next(255), 0), 5);
            Pen PenCentroid = new Pen(Color.FromArgb(0, 255, 255, 0), 5);

            foreach (var c in this.Clusters)
            {
                System.Drawing.Drawing2D.GraphicsPath points = new System.Drawing.Drawing2D.GraphicsPath();
                Pen PenElements = new Pen(c.color, 5);
                x = (int)c.Centroid.ElementAt(0);
                //y = (int)c.Centroid.ElementAt(1);
                //y = 100;
                points.AddEllipse(x, y, radius + 5, radius + 5);
                ClusterPath.DrawPath(PenCentroid, points);
                points.Reset();
                foreach (var el in c.Elements)
                {
                    x = (int)el.ElementAt(0);
                    //y = (int)el.ElementAt(1);
                    // y = 100;
                    points.AddEllipse(x, y, radius, radius);
                }
                ClusterPath.DrawPath(PenElements, points);
            }

            String imagePathFile = filePath + "/" + iteration + ".png";
            //    Uri imageUri = new Uri(imagePathFile);
            img.Save(imagePathFile, System.Drawing.Imaging.ImageFormat.Png);

        }*/

        public void Clustering(int ClusterCount, double epsilon)  //КЛАСТЕРИЗАЦИЯ
        {
            int iteration = 0;
            //this.ClusterCount = ClusterCount;
            InitMembershipMatrix();

            if (Centroids == null)
            {
                InitCentroids();
            }
            while (true)
            {
                FillDistanceMatrix();
                FillClusters();
                ReCalculateCentroids();

                if (EqualPrevAndNewCentroids(epsilon))
                {
                    break;
                }
                UpdateCentroids();
                ++iteration;
            }
            Program.ProcessedDataFromFile(Clusters);
        }

        public void GausFunction(int countColumnData, FuzzyKnowledgeBase FKB)
        {
            double SummDenominator = 0;
            double SummNumeral = 0;
            double sigm = 0;
            ValueGausFPForEachTerm = new double[ClusterCount, countColumnData - 1]; // [ClusterCount, countColumnData];


            for (int i = 0; i < ClusterCount; i++)
            {
                for (int j = 0; j < countColumnData; j++)  // countColumnData 
                {
                    double a = Clusters.ElementAt(i).Centroid.ElementAt(j);
                    for (int k = 0; k < Elements.Count; ++k)
                    {
                        SummDenominator += Math.Pow(MembershipMatrixTemp[k, i], 2);
                        SummNumeral += Math.Pow(MembershipMatrixTemp[k, i], 2) * Math.Pow(ElementsMatrix[k, j] - Clusters.ElementAt(i).Centroid.ElementAt(j), 2);
                    }
                    sigm = Math.Sqrt(SummNumeral / SummDenominator);
                    Program.SimpsonsMethodFindingIntegrall(a, sigm, i, j, countColumnData, FKB);
                }
            }
            //sigm = FuzzyLogicBase.ListOfRule.Count;
        }
        public double MaxValueGausFunction(int countColumnData, int NumbLP)
        {
            double SummDenominator = 0;
            double SummNumeral = 0;
            double sigm = 0;
            double ValueGausFP = 0;
            double MaxValue = Double.NegativeInfinity;
            for (int j = 0; j < countColumnData - 1; j++)
            {
                double a = Clusters.ElementAt(NumbLP).Centroid.ElementAt(j);
                for (int k = 0; k < Elements.Count; ++k)
                {
                    SummDenominator += Math.Pow(MembershipMatrixTemp[k, NumbLP], 2);
                    SummNumeral += Math.Pow(MembershipMatrixTemp[k, NumbLP], 2) * Math.Pow(ElementsMatrix[k, j] - Clusters.ElementAt(NumbLP).Centroid.ElementAt(j), 2);
                }
                sigm = SummNumeral / SummDenominator;
                if (MaxValue < ValueGausFP)
                {
                    MaxValue = ValueGausFP;
                }
            }

            return MaxValue;
        }

        public double GausFunction(int countColumnData, double x, int NomberLinguisticVar, int NomberTerm)
        {
            double SummDenominator = 0;
            double SummNumeral = 0;
            double sigm = 0;
            double ValueGausFP = 0;
            ValueGausFPForEachTerm = new double[ClusterCount, countColumnData - 1];

            double a = Clusters.ElementAt(NomberLinguisticVar).Centroid.ElementAt(NomberTerm);
            for (int k = 0; k < Elements.Count; ++k)
            {
                SummDenominator += Math.Pow(MembershipMatrixTemp[k, NomberLinguisticVar], 2);
                SummNumeral += Math.Pow(MembershipMatrixTemp[k, NomberLinguisticVar], 2) * Math.Pow(ElementsMatrix[k, NomberTerm] - Clusters.ElementAt(NomberLinguisticVar).Centroid.ElementAt(NomberTerm), 2);
            }
            sigm = SummNumeral / SummDenominator;
            ValueGausFP = Math.Exp(-((Math.Pow((x - a), 2)) / (2 * Math.Pow(sigm, 2))));

            return ValueGausFP;
        }

        //public void LaplasFunction(double x, int countRowData)
        //{
        //    double SummDenominator = 0;
        //    double SummNumeral = 0;
        //    double sigm = 0;
        //    double ValueLaplassFP = 0;
        //    ValueLaplassFPForEachTerm = new double[ClusterCount, countRowData - 1];

        //    for (int i = 0; i < ClusterCount; i++)
        //    {
        //        for (int j = 0; j < countRowData - 1; j++)
        //        {
        //            double a = Clusters.ElementAt(i).Centroid.ElementAt(j);
        //            for (int k = 0; k < Elements.Count; ++k)
        //            {
        //                SummDenominator += Math.Pow(MembershipMatrixTemp[k, i], 2);
        //                SummNumeral += Math.Pow(MembershipMatrixTemp[k, i], 2) * Math.Pow(ElementsMatrix[k, j] - Clusters.ElementAt(i).Centroid.ElementAt(j), 2);
        //            }
        //            sigm = SummNumeral / SummDenominator;
        //            //Console.WriteLine("Значение sigm: " + sigm);
        //            ValueLaplassFP = Math.Exp(-((x - a) / sigm));
        //            //Console.WriteLine("Значение Гаус ФП: " + ValueGausFP);
        //            ValueLaplassFPForEachTerm[i, j] = ValueLaplassFP;
        //        }
        //    }
        //}

        public void FindRulesModelTypeMamdani(List<string> NamesOfTermsDataFromFile, double[,] ValueIntervalTerm, List<string> NameOfTerms, int countColumnData, int NumbersOfZonesOneLP, int counterFoRowDataFromFile, string typeFP, List<int> WeightOfTerms, FuzzyKnowledgeBase FKB)
        {
            NameOfTermsByWords = new string[counterFoRowDataFromFile, countColumnData];
            NameOfTermsByWordsWhithoutRepeat = new string[counterFoRowDataFromFile, countColumnData];

            for (int i = 0; i < ClusterCount; i++)
            {
                List<Term> term = new List<Term>();
                Rule OneRule = new Rule(ID, null, term, 0);

                //Console.WriteLine((i + 1) + "st rule: ");
                for (int j = 0; j < countColumnData - 1; j++)
                {
                    Term t = new Term(ID, null, null);
                    //Console.Write(NamesOfTermsDataFromFile.ElementAt(j) + " = ");
                    t.NameLP = NamesOfTermsDataFromFile.ElementAt(j);  // write LP
                    if (typeFP == "Трикутна")
                    {
                        t.ProverkTruk = true;
                    }
                    else if (typeFP == "Гаусівська")
                    {
                        t.ProverGaus = true;
                    }
                    else if (typeFP == "Трапеція")
                    {
                        t.ProverkTrap = true;
                    }
                    for (int k = 0; k < NumbersOfZonesOneLP; k++)
                    {
                        if (k + 1 >= NumbersOfZonesOneLP)
                        {
                            //Console.Write(NameOfTerms.ElementAt(NumbersOfZonesOneLP - 1));
                            t.Name = NameOfTerms.ElementAt(NumbersOfZonesOneLP - 1);  // write term;
                            t.WeightOfTerm = WeightOfTerms.ElementAt(NumbersOfZonesOneLP - 1);
                            OneRule.Antecedents.Add(t);
                            NameOfTermsByWords[k, j] = NameOfTerms.ElementAt(NumbersOfZonesOneLP - 1);
                            break;
                        }
                        else if (ValueIntervalTerm[j, k] <= Clusters.ElementAt(i).Centroid.ElementAt(j) && ValueIntervalTerm[j, k + 1] >= Clusters.ElementAt(i).Centroid.ElementAt(j))
                        {
                            //Console.Write(NameOfTerms.ElementAt(k));
                            t.Name = NameOfTerms.ElementAt(k);  // write term;
                            t.WeightOfTerm = WeightOfTerms.ElementAt(k);
                            OneRule.Antecedents.Add(t);
                            NameOfTermsByWords[k, j] = NameOfTerms.ElementAt(k);
                            break;
                        }
                    }
                    //Console.Write(" and ");
                }

                Term term_conseq = new Term(ID, null, null);
                term_conseq.NameLP = NamesOfTermsDataFromFile.ElementAt(countColumnData - 1);

                //Console.WriteLine("\n than " + NamesOfTermsDataFromFile.ElementAt(countColumnData - 1) + ": ");
                for (int p = 0; p < countColumnData; p++)
                {
                    double a = Clusters.ElementAt(i).Centroid.ElementAt(countColumnData - 1);
                    if (p + 2 > NumbersOfZonesOneLP)
                    {
                        //Console.Write(NameOfTerms.ElementAt(p - 1));
                        term_conseq.Name = NameOfTerms.ElementAt(p - 1);
                        term_conseq.WeightOfTerm = WeightOfTerms.ElementAt(p - 1);
                        NameOfTermsByWords[p, countColumnData - 1] = NameOfTerms.ElementAt(p - 1);
                        break;
                    }
                    else if (p + 1 == countColumnData)
                    {
                        //Console.Write(NameOfTerms.ElementAt(p)); // Clusters.ElementAt(i).Centroid.ElementAt(Clusters.Count - 1)
                        term_conseq.Name = NameOfTerms.ElementAt(p);
                        term_conseq.WeightOfTerm = WeightOfTerms.ElementAt(p);
                        NameOfTermsByWords[p, countColumnData - 1] = NameOfTerms.ElementAt(p);
                        break;
                    }
                    else if ((ValueIntervalTerm[countColumnData - 1, p] <= Clusters.ElementAt(i).Centroid.ElementAt(countColumnData - 1) && ValueIntervalTerm[countColumnData - 1, p + 1] >= Clusters.ElementAt(i).Centroid.ElementAt(countColumnData - 1)) && p + 1 == NumbersOfZonesOneLP)
                    {
                       // Console.Write(NameOfTerms.ElementAt(p + 1));
                        term_conseq.Name = NameOfTerms.ElementAt(p + 1);
                        term_conseq.WeightOfTerm = WeightOfTerms.ElementAt(p + 1);
                        NameOfTermsByWords[p, countColumnData - 1] = NameOfTerms.ElementAt(p + 1);
                        break;
                    }
                    else if (ValueIntervalTerm[countColumnData - 1, p] <= Clusters.ElementAt(i).Centroid.ElementAt(countColumnData - 1) && ValueIntervalTerm[countColumnData - 1, p + 1] >= Clusters.ElementAt(i).Centroid.ElementAt(countColumnData - 1))
                    {
                        //Console.Write(NameOfTerms.ElementAt(p));
                        term_conseq.Name = NameOfTerms.ElementAt(p);
                        term_conseq.WeightOfTerm = WeightOfTerms.ElementAt(p);
                        NameOfTermsByWords[p, countColumnData - 1] = NameOfTerms.ElementAt(p);
                        break;
                    }
                }
                OneRule.Cоnsequens = term_conseq;
                FKB.ListOfRule.Add(OneRule);
            }

            for (int index = 0; index < countColumnData; index++)  // Запись без повторений возможных значений (словами) термов у ЛП. Для вывода.
            {
                NameOfTermsByWordsWhithoutRepeat[0, index] = NameOfTermsByWords[0, index];
                int CounterWithoutRepeatInArray = 1, Checker = 0, IndexInArray = 1;

                for (int j = 0; j < counterFoRowDataFromFile; j++)
                {
                    for (int k = 0; k < CounterWithoutRepeatInArray; k++)
                    {
                        if (NameOfTermsByWordsWhithoutRepeat[k, index] == NameOfTermsByWords[j, index])
                        {
                            Checker++;
                            break;
                        }
                    }
                    if (Checker == 0)
                    {
                        NameOfTermsByWordsWhithoutRepeat[IndexInArray, index] = NameOfTermsByWords[j, index];
                        IndexInArray++;
                        CounterWithoutRepeatInArray++;
                    }
                    Checker = 0;
                }
            }
        }
    }
}