using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using WebApplication1.FuzzyLogicBase;
using System.Drawing;
using System.Data.OleDb; // XSSFWorkbook, XSSFSheet
using System.IO; // File.Exists()
using System.Web.UI.WebControls;
using L.DataStructures.Matrix;
using L.DataStructures.Geometry; // File.Exists()
using WebApplication1;

using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using L.Algorithms.Clustering;
using System.Diagnostics;

using System.Xml;
using NPOI.HSSF.Model;
using System.Windows;

namespace Functions
{
    public class Program
    {
        public static Guid ID = new Guid();
        public static List<string> NameOfLinguisticVariables = new List<string>();
        public static List<MultiDimensionalVector> ElementsMulti = new List<MultiDimensionalVector>();
        public static double[,] ElementsMatrix;
        public static int ClusterCount, countColumnData = 0, NumbersOfZonesOneLP = 0, counterFoRowDataFromFile = 0, RecommendCountOfMaxClusterCount = 0;
        public static double[,] FindMinMaxValueTerm;
        public static double[,] ValueIntervalTerm;
        public static double[,] ValueTermVithoutRepeat;
        public static int ostanovkaLP = 0;
        public static int ostanovkaTM = 0;
        //ublic static FuzzyKnowledgeBase FKB = new FuzzyKnowledgeBase();

        //public static string[,] NameOfTermsByWords, NameOfTermsByWordsWhithoutRepeat;
        public static List<string> NameOfTerms = new List<string>();
        public static List<int> WeightOfTerms = new List<int>();
        static HSSFWorkbook wb;
        static HSSFSheet sh;
        static double s;
        public static List<string> PoshukLP(string LP)
        {
            List<string> LpZnat = new List<string>();
            for (int i = 0; i < WebApplication1.WebForm1.FKB.ListOfRule.Count; i++)
            {
                if (WebApplication1.WebForm1.FKB.ListOfRule[i].Cоnsequens.NameLP == LP)
                {
                    for (int j = 0; j < WebApplication1.WebForm1.FKB.ListOfRule[i].Antecedents.Count; j++)
                    {
                        LpZnat.Add(WebApplication1.WebForm1.FKB.ListOfRule[i].Antecedents[j].NameLP);
                    }

                }
            }
            return LpZnat;
        }
        public static string Conected(string SourceFile, string InputFile)
        {
            string rez = "";
            string dger = SourceFile;
            string plass = InputFile;
            int lengdger = dger.Length;
            int lengplass = plass.Length;
            if (dger == "")
            {
                rez = plass;
            }
            else
            {
                string s1 = dger.Substring(0, dger.IndexOf("ListOfRule") - 4);
                string s1_2 = s1.Substring(s1.Length - 4, 4);
                string s2 = plass.Substring(12, plass.IndexOf("ListOfRule") - 4 - 12);
                string s2_2 = s2.Substring(s2.Length - 4, 3);
                string s3 = dger.Substring(dger.IndexOf("ListOfRule") - 4, dger.Length - 3 - dger.IndexOf("ListOfRule") - 4 + 6);
                string s3_2 = s3.Substring(s3.Length - 4, 3);
                string s4 = plass.Substring(plass.IndexOf("ListOfRule") + 13, plass.Length - 1 - (plass.IndexOf("ListOfRule") + 13));
                string s4_2 = s4.Substring(s4.Length - 4, 3);

                rez = dger.Substring(0, dger.IndexOf("ListOfRule") - 4) + "}," + plass.Substring(12, plass.IndexOf("ListOfRule") - 4 - 12) + dger.Substring(dger.IndexOf("ListOfRule") - 4, dger.Length - 3 - dger.IndexOf("ListOfRule") - 4 + 6) + "}," + plass.Substring(plass.IndexOf("ListOfRule") + 13, plass.Length - 1 - (plass.IndexOf("ListOfRule") + 13)) + "}" ;
            }
            return rez;

        }
        public static double Fazufikachiya1(string LP, string term, double ZnachArgFp)
        {
            for (int i = 0; i < WebApplication1.WebForm1.FKB.ListVar.Count; i++)
            {
                if (WebApplication1.WebForm1.FKB.ListVar[i].Name == LP)
                {
                    for (int j = 0; j < WebApplication1.WebForm1.FKB.ListVar[i].terms.Count; j++)
                    {
                        if (WebApplication1.WebForm1.FKB.ListVar[i].terms[j].Name == term)
                        {
                            WebApplication1.WebForm1.FKB.ListVar[i].terms[j].ZnachFp = WebApplication1.WebForm1.FKB.ListVar[i].terms[j].Fp(ZnachArgFp);
                            return WebApplication1.WebForm1.FKB.ListVar[i].terms[j].ZnachFp;
                        }
                    }
                    break;
                }
            }
            return 0;

        }
        public static void Vuvid(string LPZikav, string Lphuk, double ZnachArgFp)
        {

            for (int i = 0; i < WebApplication1.WebForm1.FKB.ListOfRule.Count; i++)
            {
                WebApplication1.WebForm1.FKB.ListOfRule[i].MinZnach = Double.PositiveInfinity;
                if (WebApplication1.WebForm1.FKB.ListOfRule[i].Cоnsequens.NameLP == LPZikav)
                {
                    for (int j = 0; j < WebApplication1.WebForm1.FKB.ListOfRule[i].Antecedents.Count; j++)
                    {
                        if (WebApplication1.WebForm1.FKB.ListOfRule[i].Antecedents[j].NameLP == Lphuk)
                        {
                            if (WebApplication1.WebForm1.FKB.ListOfRule[i].MinZnach > WebApplication1.WebForm1.FKB.ListOfRule[i].Antecedents[j].Fp(ZnachArgFp))
                            {
                                WebApplication1.WebForm1.FKB.ListOfRule[i].MinZnach = WebApplication1.WebForm1.FKB.ListOfRule[i].Antecedents[j].Fp(ZnachArgFp);
                            }
                        }
                    }
                }
            }
        }
        public static string Vuvid2()
        {
            int IndexMaxPrav = 0;
            double MaxPrav = 0;
            for (int i = 0; i < WebApplication1.WebForm1.FKB.ListOfRule.Count; i++)
            {
                if (WebApplication1.WebForm1.FKB.ListOfRule[i].MinZnach > MaxPrav)
                {
                    MaxPrav = WebApplication1.WebForm1.FKB.ListOfRule[i].MinZnach;
                    IndexMaxPrav = i;
                }

            }
            return WebApplication1.WebForm1.FKB.ListOfRule[IndexMaxPrav].Cоnsequens.NameLP + " " + WebApplication1.WebForm1.FKB.ListOfRule[IndexMaxPrav].Cоnsequens.Name;
        }
        public static double Y(double p, int CheckerOfPart, double aGaus, double sigmGaus) // Нахождения координаты точки а
        {
            if (p == 0) // посмотреть детально
            {
                return 0;
            }
            else if (CheckerOfPart == 0)
            {
                return (aGaus - sigmGaus * (Math.Sqrt(-Math.Log(p)))) * p;
            }
            else if (CheckerOfPart == 1)
            {
                return (aGaus + sigmGaus * (Math.Sqrt(-Math.Log(p)))) * p;
            }
            else if (CheckerOfPart == 2)
            {
                return aGaus - sigmGaus * (Math.Sqrt(-Math.Log(p)));
            }
            else if (CheckerOfPart == 3)
            {
                return aGaus + sigmGaus * (Math.Sqrt(-Math.Log(p)));
            }
            else
                return 0;
        }
        public static double Y(double p, int CheckerOfPart, int a, double aGaus, double sigmGaus)// Нахождения координаты точки b
        {
            if (p == 0)
            {
                return 0;
            }
            else if (CheckerOfPart == 0)
            {
                return aGaus - sigmGaus * (Math.Sqrt(-Math.Log(p)));
            }
            else if (CheckerOfPart == 1)
            {
                return aGaus + sigmGaus * (Math.Sqrt(-Math.Log(p)));
            }
            else if (CheckerOfPart == 2)
            {
                return (aGaus - sigmGaus * (Math.Sqrt(-Math.Log(p)))) * p;
            }
            else if (CheckerOfPart == 3)
            {
                return (aGaus + sigmGaus * (Math.Sqrt(-Math.Log(p)))) * p;
            }
            else
                return 0;
        }
        public static double Y(double p, int CheckerOfPart, int a, int d, double aGaus, double sigmGaus) // Нахождения координаты точки c
        {
            if (p == 0)
            {
                return 0;
            }
            else if (CheckerOfPart == 0)
            {
                return aGaus - sigmGaus * (Math.Sqrt(-Math.Log(p)));
            }
            else if (CheckerOfPart == 1)
            {
                return aGaus + sigmGaus * (Math.Sqrt(-Math.Log(p)));
            }
            else if (CheckerOfPart == 2)
            {
                return (aGaus - sigmGaus * (Math.Sqrt(-Math.Log(p)))) * p;
            }
            else if (CheckerOfPart == 3)
            {
                return (aGaus + sigmGaus * (Math.Sqrt(-Math.Log(p)))) * p;
            }
            else
                return 0;
        }
        public static void SimpsonsMethodFindingIntegrall(double aGaus, double sigmGaus, int ClusterCount, int countColumnDataNow, int countColumnData, FuzzyKnowledgeBase FKB)//интегрирует любые данные (для точек фп)
        {
            double a = 0, b = 1, eps = 0.0001, result = 0; //Нижний и верхний пределы интегрирования (a, b), погрешность (eps).
            double I = eps + 1, I1 = 0;//I-предыдущее вычисленное значение интеграла, I1-новое, с большим N.

            //Запись новой точки (  a  )
            for (int i = 0; i < 4; i++)
            {
                a = 0; b = 1; eps = 0.0001; //Нижний и верхний пределы интегрирования (a, b), погрешность (eps).
                I = eps + 1; I1 = 0;//I-предыдущее вычисленное значение интеграла, I1-новое, с большим N.
                for (int N = 2; (N <= 4) || (Math.Abs(I1 - I) > eps); N *= 2)
                {
                    double h, sum2 = 0, sum4 = 0, sum = 0;
                    h = (b - a) / (2 * N);//Шаг интегрирования.
                    for (int ind = 1; ind <= 2 * N - 1; ind += 2)
                    {
                        sum4 += Y(a + h * ind, i, aGaus, sigmGaus);//Значения с нечётными индексами, которые нужно умножить на 4.
                        sum2 += Y(a + h * (ind + 1), i, aGaus, sigmGaus);//Значения с чётными индексами, которые нужно умножить на 2.
                    }
                    sum = Y(a, i, aGaus, sigmGaus) + 4 * sum4 + 2 * sum2 - Y(b, i, aGaus, sigmGaus);//Отнимаем значение f(b) так как ранее прибавили его дважды. 
                    I = I1;
                    I1 = (h / 3) * sum;
                }
                if (i < 1)
                    result = 3 * I1;
                else if (i == 1)
                    result += 3 * I1;
                else if (i > 1 && i <= 3)
                    result -= I1;
            }
            if (countColumnDataNow + 1 == countColumnData)
                FKB.ListOfRule[FKB.ListOfRule.Count() - Program.ClusterCount + ClusterCount].Cоnsequens.a = result;
            else
                FKB.ListOfRule[FKB.ListOfRule.Count() - Program.ClusterCount + ClusterCount].Antecedents[countColumnDataNow].a = result;

            /*//Запись новой точки (  b  )
            result = 0;
            for (int i = 0; i < 4; i++)
            {
                a = 0; b = 1; eps = 0.0001; //Нижний и верхний пределы интегрирования (a, b), погрешность (eps).
                I = eps + 1; I1 = 0;//I-предыдущее вычисленное значение интеграла, I1-новое, с большим N.
                for (int N = 2; (N <= 4) || (Math.Abs(I1 - I) > eps); N *= 2)
                {
                    double h, sum2 = 0, sum4 = 0, sum = 0;
                    h = (b - a) / (2 * N);//Шаг интегрирования.
                    for (int ind = 1; ind <= 2 * N - 1; ind += 2)
                    {
                        sum4 += Y(a + h * ind, i, i, aGaus, sigmGaus);//Значения с нечётными индексами, которые нужно умножить на 4.
                        sum2 += Y(a + h * (ind + 1), i, i, aGaus, sigmGaus);//Значения с чётными индексами, которые нужно умножить на 2.
                    }
                    sum = Y(a, i, i, aGaus, sigmGaus) + 4 * sum4 + 2 * sum2 - Y(b, i, i, aGaus, sigmGaus);//Отнимаем значение f(b) так как ранее прибавили его дважды. 
                    I = I1;
                    I1 = (h / 3) * sum;
                }
                if (i == 0)
                    result = (7 / 2) * I1;
                else if (i == 1)
                    result += (1 / 2) * I1;
                else if (i == 2)
                    result -= (9 / 2) * I1;
                else if (i == 3)
                    result -= (3 / 2) * I1;
            }
            if (countColumnDataNow + 1 == countColumnData) //часть для точки b
                FKB.ListOfRule[FKB.ListOfRule.Count() - WebApplication1.FKB.Program.ClusterCount + ClusterCount].Cоnsequens.a = result;
            else
                FKB.ListOfRule[FKB.ListOfRule.Count() - WebApplication1.FKB.Program.ClusterCount + ClusterCount].Antecedents[countColumnDataNow].a = result;*/

            //Запись новой точки (  c  )
            result = 0;
            for (int i = 0; i < 4; i++)
            {
                a = 0; b = 1; eps = 0.0001; //Нижний и верхний пределы интегрирования (a, b), погрешность (eps).
                I = eps + 1; I1 = 0;//I-предыдущее вычисленное значение интеграла, I1-новое, с большим N.
                for (int N = 2; (N <= 4) || (Math.Abs(I1 - I) > eps); N *= 2)
                {
                    double h, sum2 = 0, sum4 = 0, sum = 0;
                    h = (b - a) / (2 * N);//Шаг интегрирования.
                    for (int ind = 1; ind <= 2 * N - 1; ind += 2)
                    {
                        sum4 += Y(a + h * ind, i, i, i, aGaus, sigmGaus);//Значения с нечётными индексами, которые нужно умножить на 4.
                        sum2 += Y(a + h * (ind + 1), i, i, i, aGaus, sigmGaus);//Значения с чётными индексами, которые нужно умножить на 2.
                    }
                    sum = Y(a, i, i, i, aGaus, sigmGaus) + 4 * sum4 + 2 * sum2 - Y(b, i, i, i, aGaus, sigmGaus);//Отнимаем значение f(b) так как ранее прибавили его дважды. 
                    I = I1;
                    I1 = (h / 3) * sum;
                }
                if (i == 0)
                    result = (1 / 2) * I1;
                else if (i == 1)
                    result += (7 / 2) * I1;
                else if (i == 2)
                    result -= (3 / 2) * I1;
                else if (i == 3)
                    result -= (9 / 2) * I1;
            }
            if (countColumnDataNow + 1 == countColumnData) //часть для точки b
                FKB.ListOfRule[FKB.ListOfRule.Count() - Program.ClusterCount + ClusterCount].Cоnsequens.c = result;
            else
                FKB.ListOfRule[FKB.ListOfRule.Count() - Program.ClusterCount + ClusterCount].Antecedents[countColumnDataNow].c = result;

            double tempTerm = 0;
            if (countColumnDataNow + 1 == countColumnData) //часть для точки b
                FKB.ListOfRule[FKB.ListOfRule.Count() - Program.ClusterCount + ClusterCount].Cоnsequens.b = (FKB.ListOfRule[FKB.ListOfRule.Count() - Program.ClusterCount + ClusterCount].Cоnsequens.a + FKB.ListOfRule[FKB.ListOfRule.Count() - Program.ClusterCount + ClusterCount].Cоnsequens.c) / 2;
            else
                FKB.ListOfRule[FKB.ListOfRule.Count() - Program.ClusterCount + ClusterCount].Antecedents[countColumnDataNow].b = (FKB.ListOfRule[FKB.ListOfRule.Count() - Program.ClusterCount + ClusterCount].Antecedents[countColumnDataNow].a + FKB.ListOfRule[FKB.ListOfRule.Count() - Program.ClusterCount + ClusterCount].Antecedents[countColumnDataNow].c) / 2;

            if (countColumnDataNow + 1 == countColumnData)
            {
                if (FKB.ListOfRule[FKB.ListOfRule.Count() - Program.ClusterCount + ClusterCount].Cоnsequens.a > FKB.ListOfRule[FKB.ListOfRule.Count() - Program.ClusterCount + ClusterCount].Cоnsequens.c)
                {
                    tempTerm = FKB.ListOfRule[FKB.ListOfRule.Count() - Program.ClusterCount + ClusterCount].Cоnsequens.c;
                    FKB.ListOfRule[FKB.ListOfRule.Count() - Program.ClusterCount + ClusterCount].Cоnsequens.c = FKB.ListOfRule[FKB.ListOfRule.Count() - Program.ClusterCount + ClusterCount].Cоnsequens.a;
                    FKB.ListOfRule[FKB.ListOfRule.Count() - Program.ClusterCount + ClusterCount].Cоnsequens.a = tempTerm;
                }
            }
            else
            {
                if (FKB.ListOfRule[FKB.ListOfRule.Count() - Program.ClusterCount + ClusterCount].Antecedents[countColumnDataNow].a > FKB.ListOfRule[FKB.ListOfRule.Count() - Program.ClusterCount + ClusterCount].Antecedents[countColumnDataNow].c)
                {
                    tempTerm = FKB.ListOfRule[FKB.ListOfRule.Count() - Program.ClusterCount + ClusterCount].Antecedents[countColumnDataNow].c;
                    FKB.ListOfRule[FKB.ListOfRule.Count() - Program.ClusterCount + ClusterCount].Antecedents[countColumnDataNow].c = FKB.ListOfRule[FKB.ListOfRule.Count() - Program.ClusterCount + ClusterCount].Antecedents[countColumnDataNow].a;
                    FKB.ListOfRule[FKB.ListOfRule.Count() - Program.ClusterCount + ClusterCount].Antecedents[countColumnDataNow].a = tempTerm;
                }
            }
        }
        public static List<string> PreLevelSearch(string Cоnsequens)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < WebApplication1.WebForm1.FKB.ListOfRule.Count; i++)
            {
                if (WebApplication1.WebForm1.FKB.ListOfRule[i].Cоnsequens.NameLP == Cоnsequens)
                {
                    for (int j = 0; j < WebApplication1.WebForm1.FKB.ListOfRule[i].Antecedents.Count; j++)
                    {
                        result.Add(WebApplication1.WebForm1.FKB.ListOfRule[i].Antecedents[j].NameLP);
                    }
                }
            }
            return result;
        }
        public static void ReadFromXLS(string path) // Function for reading data from the file .xls
        {
            HSSFWorkbook hssfwb;

            using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))  // Data2
            {
                hssfwb = new HSSFWorkbook(file);
            }
            ISheet sheet = hssfwb.GetSheet("FirstList");
            List<string> Elements = new List<string>();
            int column = 1, Row = 1;
            countColumnData = 0;
            Elements.Clear();

            for (column = 1; sheet.GetRow(0).GetCell(column) != null; column++) // подсчет количества колонок в файле, а также запись названия ЛП
            {
                List<Term> t = new List<Term>();
                NameOfLinguisticVariables.Add(string.Format("{0: 0.0}", sheet.GetRow(0).GetCell(column)));
                LinguisticVariable LP = new LinguisticVariable(ID, string.Format("{0: 0.0}", sheet.GetRow(0).GetCell(column)), t, 0, 1);
                //FKB.ListVar.Add(LP);
                countColumnData += 1;
            }

            if (counterFoRowDataFromFile == 0)
            {
                for (Row = 1; sheet.GetRow(Row).GetCell(0) != null; Row++)  // подсчет количества строк в файле
                {
                    counterFoRowDataFromFile++;
                }
            }
            column = 1;
            ElementsMatrix = new double[counterFoRowDataFromFile, countColumnData];
            for (int row = 1; row <= counterFoRowDataFromFile; row++)  // запись построчно с файла данных в список ElementsMulti -MultiDimensionalVector-
            {
                MultiDimensionalVector h = new MultiDimensionalVector();
                while (sheet.GetRow(row).GetCell(column) != null)
                {
                    Elements.Add(string.Format("{0: 0.0}", sheet.GetRow(row).GetCell(column)));
                    column += 1;
                }
                List<double> result = Elements.Select(x => double.Parse(x)).ToList();
                int integer = 0;
                foreach (double x in result)
                {
                    var newVector = x;
                    h.Add(newVector);
                    ElementsMatrix[row - 1, integer] = x;
                    integer++;
                }
                ElementsMulti.Add(h);
                column = 1;
                Elements.Clear();
            }
            ClusterCount = (counterFoRowDataFromFile / 2) + 3;
            if (ClusterCount > 10)
            {
                ClusterCount = 7;
            }
        }

        public static void ProcessedDataFromFile(List<Cluster> Clusters)  // найти термы, возможные их значения 
        {
            GiveNameToTerms(ClusterCount, counterFoRowDataFromFile);  // выбор количества названий термов

            int CheckerCount;  // Логическая проверка
            if (ClusterCount > countColumnData)
            {
                CheckerCount = ClusterCount;
            }
            else
            {
                CheckerCount = countColumnData;
            }
            FindMinMaxValueTerm = new double[CheckerCount, 2];  /// ClusterCount -- countColumnData что большее 
            ValueIntervalTerm = new double[CheckerCount, NumbersOfZonesOneLP];  /// countColumnData --- ClusterCount

            //NameOfTermsByWords = new string[counterFoRowDataFromFile, countColumnData];
            //NameOfTermsByWordsWhithoutRepeat = new string[counterFoRowDataFromFile, countColumnData];
            ValueTermVithoutRepeat = new double[CheckerCount, countColumnData];
            int column = 1;
            double MaxValTerm = Clusters.ElementAt(0).Centroid.ElementAt(0), MinValTerm = Clusters.ElementAt(0).Centroid.ElementAt(0);
            //Clusters.ElementAt(i).Centroid.ElementAt(j)
            for (column = 0; column < countColumnData; column++) // найти мин и макс элемент одного ЛП (сравнения значений по колонкам)
            {
                MaxValTerm = Clusters.ElementAt(0).Centroid.ElementAt(column);
                MinValTerm = Clusters.ElementAt(0).Centroid.ElementAt(column);
                for (int rown = 0; rown < ClusterCount; rown++)
                {
                    if (Clusters.ElementAt(rown).Centroid.ElementAt(column) > MaxValTerm)
                    {
                        MaxValTerm = Clusters.ElementAt(rown).Centroid.ElementAt(column);
                    }
                    if (Clusters.ElementAt(rown).Centroid.ElementAt(column) < MinValTerm)
                    {
                        MinValTerm = Clusters.ElementAt(rown).Centroid.ElementAt(column);
                    }
                }
                FindMinMaxValueTerm[column, 0] = MinValTerm;
                FindMinMaxValueTerm[column, 1] = MaxValTerm;
            }

            double interval = 0;
            for (int i = 0; i < CheckerCount; i++)  // разбиение на промежутки значений. Нахождения возможных зон для значений термов одной ЛП 
            {
                interval = (FindMinMaxValueTerm[i, 1] - FindMinMaxValueTerm[i, 0]) / (NumbersOfZonesOneLP - 1);
                ValueIntervalTerm[i, 0] = FindMinMaxValueTerm[i, 0];
                ValueIntervalTerm[i, NumbersOfZonesOneLP - 1] = FindMinMaxValueTerm[i, 1];
                for (int j = 1; j < NumbersOfZonesOneLP; j++)
                {
                    ValueIntervalTerm[i, j] = ValueIntervalTerm[i, j - 1] + interval;
                }
            }

            for (column = 0; column < countColumnData; column++)  // Запись возможных значений (числами) термов у ЛП. Для вывода.
            {
                ValueTermVithoutRepeat[0, column] = Clusters.ElementAt(0).Centroid.ElementAt(column);
                int countElements = 1;
                for (int rown = 1; rown < ClusterCount; rown++)
                {
                    for (int j = 0; j < countElements; j++)
                    {
                        if (Clusters.ElementAt(rown).Centroid.ElementAt(column) == ValueTermVithoutRepeat[j, column])
                        {
                            break;
                        }
                        if (Clusters.ElementAt(rown).Centroid.ElementAt(column) != ValueTermVithoutRepeat[j, column] && j + 1 == countElements)
                        {
                            ValueTermVithoutRepeat[j + 1, column] = Clusters.ElementAt(rown).Centroid.ElementAt(column);
                            countElements++;
                        }
                    }
                }
            }
        }
        public static void GiveNameToTerms(int ClusterCount, int counterFoRowDataFromFile)  // функция для определения просранства имен термов, а также количества зон (возможных значений термов) одной ЛП
        {
            /*if(ClusterCount <= 4 && ClusterCount > 0)
            {
                NameOfTerms.Add("якість низька");
                NameOfTerms.Add("якість середня");
                NameOfTerms.Add("якість висока");
                WeightOfTerms.Add(1);
                WeightOfTerms.Add(3);
                WeightOfTerms.Add(5);
                NumbersOfZonesOneLP = 3;
           }
            else if(ClusterCount > 4 && ClusterCount <=6)
            {*/
            NameOfTerms.Add("очень маленькая");
            NameOfTerms.Add("маленькая");
            NameOfTerms.Add("средняя");
            NameOfTerms.Add("большая");
            NameOfTerms.Add("очень большая");
            WeightOfTerms.Add(0);
            WeightOfTerms.Add(1);
            WeightOfTerms.Add(3);
            WeightOfTerms.Add(4);
            WeightOfTerms.Add(5);
            NumbersOfZonesOneLP = 5;
            /*}
             else if (ClusterCount >= 7 && ClusterCount <= (counterFoRowDataFromFile / 2) + 3) 
             {
                     NameOfTerms.Add("якість дуже низька");
                     NameOfTerms.Add("якість низька");
                     NameOfTerms.Add("якість не низька");
                     NameOfTerms.Add("якість середня");
                     NameOfTerms.Add("якість не дуже висока");
                     NameOfTerms.Add("якість висока");
                     NameOfTerms.Add("якість дуже висока");
                 WeightOfTerms.Add(0);
                 WeightOfTerms.Add(1);
                 WeightOfTerms.Add(2);
                 WeightOfTerms.Add(3);
                 WeightOfTerms.Add(4);
                 WeightOfTerms.Add(5);
                 WeightOfTerms.Add(6);
                 NumbersOfZonesOneLP = 7;
             }*/
        }
        public static void ReadDataFromXLSFileOneWatch(string path) // Function for reading data from the file .xls
        {
            HSSFWorkbook hssfwb;

            using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))  // Data2
            {
                hssfwb = new HSSFWorkbook(file);
            }
            ISheet sheet = hssfwb.GetSheet("FirstList");
            List<string> Elements = new List<string>();
            int column = 1, Row = 1;
            countColumnData = 0;
            ElementsMulti.Clear();

            for (column = 1; sheet.GetRow(0).GetCell(column) != null; column++) // подсчет количества колонок в файле, а также запись названия ЛП
            {
                List<Term> t = new List<Term>();
                NameOfLinguisticVariables.Add(string.Format("{0: 0.0}", sheet.GetRow(0).GetCell(column)));
                LinguisticVariable LP = new LinguisticVariable(ID, string.Format("{0: 0.0}", sheet.GetRow(0).GetCell(column)), t, 0, 1);
                //FKB.ListVar.Add(LP);
                countColumnData += 1;
            }

            if (counterFoRowDataFromFile == 0)
            {
                for (Row = 1; sheet.GetRow(Row).GetCell(0) != null; Row++)  // подсчет количества строк в файле
                {
                    counterFoRowDataFromFile++;
                }
            }
            column = 1;
            ElementsMatrix = new double[counterFoRowDataFromFile, countColumnData];

            for (int row = 1; row <= counterFoRowDataFromFile; row++)  // запись построчно с файла данных в список ElementsMulti -MultiDimensionalVector-
            {
                MultiDimensionalVector h = new MultiDimensionalVector();
                while (sheet.GetRow(row).GetCell(column) != null)
                {
                    Elements.Add(string.Format("{0: 0.0}", sheet.GetRow(row).GetCell(column)));
                    column += 1;
                }
                List<double> result = Elements.Select(x => double.Parse(x)).ToList();
                int integer = 0;
                foreach (double x in result)
                {
                    var newVector = x;
                    h.Add(newVector);
                    ElementsMatrix[row - 1, integer] = x;
                    integer++;
                }
                ElementsMulti.Add(h);
                column = 1;
                Elements.Clear();
            }
            ClusterCount = (counterFoRowDataFromFile / 2) + 3;
            if (ClusterCount > 10)
            {
                ClusterCount = 7;
            }
            ///  Console.WriteLine("Максимальное количество кластеров: " + RecommendCountOfMaxClusterCount);
        }
        public static void ReadAndConvertDataFromXMLFile(string pathToCheck)
        {
            XmlTextReader reader = new XmlTextReader(pathToCheck);  // Оцінка якості води  -- Расход газа
            int CountNodesInXMLfile = 0;  // переменная принимающая число 2, то заканчиваеться запись ЛПшек
            string NameOFViewFromFileXML = "";
            List<string> NameOFtermsForOneLVFromFileXML = new List<string>();
            countColumnData = 0;  // число колонок в создаваемом файле

            while (reader.Read() && CountNodesInXMLfile != 2)  // цыкл, для записи всех ЛПшек
            {
                if (reader.Name == "Node")
                {
                    CountNodesInXMLfile++;
                }
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "data")
                {
                    NameOfLinguisticVariables.Add(reader.GetAttribute("tclass"));
                    countColumnData++;
                }
            }
            wb = HSSFWorkbook.Create(InternalWorkbook.CreateWorkbook());
            sh = (HSSFSheet)wb.CreateSheet("FirstList");// create sheet

            for (int i = 0; i < 1; i++) // запись названий лингвистических переменных с xml -> xls
            {
                var r = sh.CreateRow(i);
                for (int j = 0; j <= NameOfLinguisticVariables.Count; j++)
                {
                    r.CreateCell(j);
                    if (j != 0)
                        sh.GetRow(i).GetCell(j).SetCellValue(NameOfLinguisticVariables.ElementAt(j - 1));
                }
            }
            reader = new XmlTextReader(pathToCheck);  // заново открываем файл, полностью считываются названия термов
            CountNodesInXMLfile = 0;
            counterFoRowDataFromFile = 0;  // для создания новых строчек в xls файле

            while (reader.Read())  // цыкл, происходит запись всех значений исследования 
            {
                if (reader.Name == "Node")
                {
                    if (reader.GetAttribute("nodeName") != null)
                        NameOFViewFromFileXML = reader.GetAttribute("nodeName");
                    CountNodesInXMLfile++;
                }
                if (reader.NodeType == XmlNodeType.Text)  // reader.Name == "data"
                {
                    NameOFtermsForOneLVFromFileXML.Add(reader.Value);
                }
                if (CountNodesInXMLfile == 2)
                {
                    counterFoRowDataFromFile++;
                    var rr = sh.CreateRow(counterFoRowDataFromFile);
                    for (int j = 0; j <= NameOfLinguisticVariables.Count; j++) // запись в xls файл значений термов
                    {
                        rr.CreateCell(j);
                        if (j == 0)
                            sh.GetRow(counterFoRowDataFromFile).GetCell(j).SetCellValue(NameOFViewFromFileXML);
                        else
                        {
                            if (j <= NameOFtermsForOneLVFromFileXML.Count)
                                sh.GetRow(counterFoRowDataFromFile).GetCell(j).SetCellValue(NameOFtermsForOneLVFromFileXML.ElementAt(j - 1));
                            else
                                sh.GetRow(counterFoRowDataFromFile).GetCell(j).SetCellValue(0);
                        }
                    }
                    CountNodesInXMLfile = 0;
                    NameOFtermsForOneLVFromFileXML.Clear();
                    NameOFViewFromFileXML = "";
                }
            }

            using (var fs = new FileStream("test.xls", FileMode.Create, FileAccess.Write)) // сохранения данных в файле xls
            {
                wb.Write(fs);
            }

            // Дописать транспортировку данных
        }
        public static void ReadListVar(int ClusterCount, int countColumnDataNow)
        {
            for (int i = 0; i < WebApplication1.WebForm1.FKB.ListVar.Count; i++)
            {
                if (WebApplication1.WebForm1.FKB.ListVar[i].Name == WebApplication1.WebForm1.FKB.ListOfRule[ClusterCount].Antecedents[countColumnDataNow].NameLP)
                {
                    for (int j = 0; j < WebApplication1.WebForm1.FKB.ListVar[i].terms.Count; j++)
                    {
                        if (WebApplication1.WebForm1.FKB.ListVar[i].terms[j].Name == WebApplication1.WebForm1.FKB.ListOfRule[ClusterCount].Antecedents[countColumnDataNow].Name)
                        {
                            WebApplication1.WebForm1.FKB.ListVar[i].terms[j].a = WebApplication1.WebForm1.FKB.ListOfRule[ClusterCount].Antecedents[countColumnDataNow].a;
                            WebApplication1.WebForm1.FKB.ListVar[i].terms[j].c = WebApplication1.WebForm1.FKB.ListOfRule[ClusterCount].Antecedents[countColumnDataNow].c;
                            WebApplication1.WebForm1.FKB.ListVar[i].terms[j].b = (WebApplication1.WebForm1.FKB.ListVar[i].terms[j].c + WebApplication1.WebForm1.FKB.ListVar[i].terms[j].a) / 2;
                            //WebApplication1.WebForm1.FKB.ListVar[i].terms[j].ProverGaus = WebApplication1.WebForm1.FKB.ListOfRule[ClusterCount].Antecedents[countColumnDataNow].ProverGaus;
                            //WebApplication1.WebForm1.FKB.ListVar[i].terms[j].ProverkTrap = WebApplication1.WebForm1.FKB.ListOfRule[ClusterCount].Antecedents[countColumnDataNow].ProverkTrap;
                            //WebApplication1.WebForm1.FKB.ListVar[i].terms[j].ProverkTruk = WebApplication1.WebForm1.FKB.ListOfRule[ClusterCount].Antecedents[countColumnDataNow].ProverkTruk;
                            break;
                        }
                    }
                    break;
                }
            }
        }

        public static void ReadListVarLast(int ClusterCount, int countColumnDataNow)
        {
            for (int i = 0; i < WebApplication1.WebForm1.FKB.ListVar.Count; i++)
            {
                if (WebApplication1.WebForm1.FKB.ListVar[i].Name == WebApplication1.WebForm1.FKB.ListOfRule[ClusterCount].Cоnsequens.NameLP)
                {
                    for (int j = 0; j < WebApplication1.WebForm1.FKB.ListVar[i].terms.Count; j++)
                    {
                        if (WebApplication1.WebForm1.FKB.ListVar[i].terms[j].Name == WebApplication1.WebForm1.FKB.ListOfRule[ClusterCount].Cоnsequens.Name)
                        {
                            WebApplication1.WebForm1.FKB.ListVar[i].terms[j].a = WebApplication1.WebForm1.FKB.ListOfRule[ClusterCount].Cоnsequens.a;
                            WebApplication1.WebForm1.FKB.ListVar[i].terms[j].c = WebApplication1.WebForm1.FKB.ListOfRule[ClusterCount].Cоnsequens.c;
                            WebApplication1.WebForm1.FKB.ListVar[i].terms[j].b = (WebApplication1.WebForm1.FKB.ListVar[i].terms[j].c + WebApplication1.WebForm1.FKB.ListVar[i].terms[j].a) / 2;
                            //WebApplication1.WebForm1.FKB.ListVar[i].terms[j].ProverGaus = WebApplication1.WebForm1.FKB.ListOfRule[ClusterCount].Antecedents[countColumnDataNow].ProverGaus;
                            //WebApplication1.WebForm1.FKB.ListVar[i].terms[j].ProverkTrap = WebApplication1.WebForm1.FKB.ListOfRule[ClusterCount].Antecedents[countColumnDataNow].ProverkTrap;
                            //WebApplication1.WebForm1.FKB.ListVar[i].terms[j].ProverkTruk = WebApplication1.WebForm1.FKB.ListOfRule[ClusterCount].Antecedents[countColumnDataNow].ProverkTruk;
                            break;
                        }
                    }
                    break;
                }
            }
        }
        public static void FunctionForClearingDataForHierarchy()
        {
            ElementsMulti.Clear();
            NameOfLinguisticVariables.Clear();
            Program.ElementsMatrix = null;
            FindMinMaxValueTerm = null;
            ValueIntervalTerm = null;
            ValueTermVithoutRepeat = null;
            ClusterCount = 0; countColumnData = 0; NumbersOfZonesOneLP = 0; counterFoRowDataFromFile = 0; RecommendCountOfMaxClusterCount = 0;
            NameOfTerms.Clear();
            K_means.Clusters.Clear();
            //WebApplication1.WebForm1.FKB.ListVar.Clear();
            //WebApplication1.WebForm1.FKB.ListOfRule.Clear();
        }
        
        public static void WithRullToVar(FuzzyKnowledgeBase FKB)
        {
            ostanovkaLP = 0;
            for (int rule = ostanovkaLP; rule < FKB.ListOfRule.Count; rule++)
            {
                for (int anc = 0; anc < FKB.ListOfRule[rule].Antecedents.Count; anc++)
                {
                    List<Term> spusokTermans = new List<Term>();
                    for (int termforlist = ostanovkaLP; termforlist < FKB.ListOfRule.Count; termforlist++)
                    {
                        bool provlistterms = true;
                        Term tm = new Term(ID, FKB.ListOfRule[termforlist].Antecedents[anc].Name, FKB.ListOfRule[termforlist].Antecedents[anc].NameLP);
                        if (spusokTermans.Count == 0)
                        {
                            tm.a = FKB.ListOfRule[termforlist].Antecedents[anc].a;
                            tm.b = FKB.ListOfRule[termforlist].Antecedents[anc].b;
                            tm.c = FKB.ListOfRule[termforlist].Antecedents[anc].c;
                            tm.d = FKB.ListOfRule[termforlist].Antecedents[anc].d;
                            tm.ProverkTruk = FKB.ListOfRule[termforlist].Antecedents[anc].ProverkTruk;
                            tm.WeightOfTerm = FKB.ListOfRule[termforlist].Antecedents[anc].WeightOfTerm;
                            spusokTermans.Add(tm);
                        }
                        else
                        {
                            for (int t = 0; t < spusokTermans.Count; t++)
                            {
                                if (spusokTermans[t].Name == tm.Name)
                                {
                                    provlistterms = false;
                                    break;
                                }
                            }
                            if (provlistterms == true)
                            {
                                tm.a = FKB.ListOfRule[termforlist].Antecedents[anc].a;
                                tm.b = FKB.ListOfRule[termforlist].Antecedents[anc].b;
                                tm.c = FKB.ListOfRule[termforlist].Antecedents[anc].c;
                                tm.d = FKB.ListOfRule[termforlist].Antecedents[anc].d;
                                tm.ProverkTruk = FKB.ListOfRule[termforlist].Antecedents[anc].ProverkTruk;
                                tm.WeightOfTerm = FKB.ListOfRule[termforlist].Antecedents[anc].WeightOfTerm;
                                spusokTermans.Add(tm);
                            }
                        }


                    }
                    LinguisticVariable lpans = new LinguisticVariable(ID, FKB.ListOfRule[rule].Antecedents[anc].NameLP, spusokTermans, 0, 1);
                    if (FKB.ListVar.Count == 0)
                    {
                        FKB.ListVar.Add(lpans);
                    }
                    else
                    {
                        bool provirkaLP = true;
                        for (int n = 0; n < FKB.ListVar.Count; n++)
                        {
                            if (FKB.ListVar[n].Name == lpans.Name)
                            {
                                for (int termlpj = 0; termlpj < lpans.terms.Count; termlpj++)
                                {
                                    bool provtermlp = true;
                                    for (int termlpi = 0; termlpi < FKB.ListVar[n].terms.Count; termlpi++)
                                    {
                                        if (FKB.ListVar[n].terms[termlpi].Name == lpans.terms[termlpj].Name)
                                        {
                                            provtermlp = false;
                                        }
                                    }
                                    if (provtermlp == true)
                                    {
                                        FKB.ListVar[n].terms.Add(lpans.terms[termlpj]);
                                    }
                                }
                                provirkaLP = false;
                                break;
                            }
                        }
                        if (provirkaLP == true)
                        {
                            FKB.ListVar.Add(lpans);
                        }
                    }

                }

                List<Term> spusokTerm = new List<Term>();
                for (int termforlist = ostanovkaLP; termforlist < FKB.ListOfRule.Count; termforlist++, ostanovkaLP++)
                {
                    bool provlistterms = true;
                    Term tm = new Term(ID, FKB.ListOfRule[termforlist].Cоnsequens.Name, FKB.ListOfRule[termforlist].Cоnsequens.NameLP);
                    if (spusokTerm.Count == 0)
                    {
                        tm.a = FKB.ListOfRule[termforlist].Cоnsequens.a;
                        tm.b = FKB.ListOfRule[termforlist].Cоnsequens.b;
                        tm.c = FKB.ListOfRule[termforlist].Cоnsequens.c;
                        tm.d = FKB.ListOfRule[termforlist].Cоnsequens.d;
                        tm.ProverkTruk = FKB.ListOfRule[termforlist].Cоnsequens.ProverkTruk;
                        tm.WeightOfTerm = FKB.ListOfRule[termforlist].Cоnsequens.WeightOfTerm;
                        spusokTerm.Add(tm);
                    }
                    else
                    {
                        for (int t = 0; t < spusokTerm.Count; t++)
                        {
                            if (spusokTerm[t].Name == tm.Name)
                            {
                                provlistterms = false;
                                break;
                            }
                        }
                        if (provlistterms == true)
                        {
                            tm.a = FKB.ListOfRule[termforlist].Cоnsequens.a;
                            tm.b = FKB.ListOfRule[termforlist].Cоnsequens.b;
                            tm.c = FKB.ListOfRule[termforlist].Cоnsequens.c;
                            tm.d = FKB.ListOfRule[termforlist].Cоnsequens.d;
                            tm.ProverkTruk = FKB.ListOfRule[termforlist].Cоnsequens.ProverkTruk;
                            tm.WeightOfTerm = FKB.ListOfRule[termforlist].Cоnsequens.WeightOfTerm;
                            spusokTerm.Add(tm);
                        }
                    }
                }
                LinguisticVariable lp = new LinguisticVariable(ID, FKB.ListOfRule[rule].Cоnsequens.NameLP, spusokTerm, 0, 1);
                if (FKB.ListVar.Count == 0)
                {
                    FKB.ListVar.Add(lp);
                }
                else
                {
                    bool provlp = true;
                    for (int n = 0; n < FKB.ListVar.Count; n++)
                    {

                        if (FKB.ListVar[n].Name == lp.Name)
                        {
                            for (int termlpj = 0; termlpj < lp.terms.Count; termlpj++)
                            {
                                bool provtermlp = true;
                                for (int termlpi = 0; termlpi < FKB.ListVar[n].terms.Count; termlpi++)
                                {
                                    if (FKB.ListVar[n].terms[termlpi].Name == lp.terms[termlpj].Name)
                                    {
                                        provtermlp = false;
                                    }
                                }
                                if (provtermlp == true)
                                {
                                    FKB.ListVar[n].terms.Add(lp.terms[termlpj]);
                                }
                            }
                            provlp = false;
                            break;
                        }
                    }
                    if (provlp == true)
                    {
                        FKB.ListVar.Add(lp);
                    }
                }
                break;
            }
        }

        public static void Save_BNZ(FuzzyKnowledgeBase FKB, string path2)  // path2 где сохраняем файлик с БНЗ
        {
            using (StreamWriter sw = File.CreateText(path2))
            {
                sw.WriteLine(System.Web.Helpers.Json.Encode(FKB));
            }
        }
        public static FuzzyKnowledgeBase WithJsonToBNZ(string path)
        {

            return System.Web.Helpers.Json.Decode<FuzzyKnowledgeBase>(File.ReadAllText(path));
        }
    }
}