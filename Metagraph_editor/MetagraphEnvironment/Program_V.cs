using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.OleDb; //XSSFWorkbook, XSSFSheet
using L.DataStructures.Matrix;
using L.DataStructures.Geometry;
using System.IO; // File.Exists()

using NPOI.SS.UserModel;
using System.Web;
using NPOI.HSSF.UserModel;
using L.Algorithms.Clustering;

//using System.IO; -- for csv file reading

namespace C__04._07._2015.Algorithms.Clustering
{
    public class ProgramV
    {
        static List<MultiDimensionalVector> ElementsMulti = new List<MultiDimensionalVector>();
        static List<string> NamesOfTermsDataFromFile = new List<string>();
        static double[,] ElementsMatrix;
        static double[,] FindMinMaxValueTerm;
        static double[,] ValueIntervalTerm;
        static int ClusterCount, counterFoRowDataFromFile = 0;
        static List<string> NameOfTerms = new List<string>();

        public static void Main(string[] args)
        {
            double epsilon = 0.05;
            double x = 0; // NEED THIS VALUE FROM ANDREY!
            ReadDataFromXLSFileOneWatch();
            K_means Clustering = new K_means(ElementsMulti, null, ClusterCount, ElementsMatrix); 
            Clustering.Clustering(ClusterCount, epsilon);
            //Clustering.GausFunction(x); 
            //Clustering.DrawClusters(ClusterCount, @"c:\Users\Іван\Desktop");
            //Clustering.FindRulesModelTypeMamdani(NamesOfTermsDataFromFile, ValueIntervalTerm, NameOfTerms);

            // ReadDataFromCSVFile();
            // ReadDataFromXLSFile();

            Console.ReadKey();
        }

        public static void ReadDataFromXLSFileOneWatch() // Function for reading data from the file .xls
        {
            HSSFWorkbook hssfwb;

            using (FileStream file = new FileStream(@"c:\Data2.xls", FileMode.Open, FileAccess.Read))
            {
                hssfwb = new HSSFWorkbook(file);
            }
            ISheet sheet = hssfwb.GetSheet("FirstList");
            List<string> Elements = new List<string>();
            int column = 1;

            for (int row = 1; sheet.GetRow(row).GetCell(0) != null; row++)
            {
                counterFoRowDataFromFile++;
            }
            //ClusterCount = (counterFoRowDataFromFile + 3) / 2;

            for (column = 1; sheet.GetRow(0).GetCell(column) != null; column++)
            {
                NamesOfTermsDataFromFile.Add(string.Format("{0: 0.0}", sheet.GetRow(0).GetCell(column)));
                ClusterCount = ClusterCount + 1;
            }
            GiveNameToTerms(ClusterCount);
            FindMinMaxValueTerm = new double[ClusterCount, 2];
            ElementsMatrix = new double[counterFoRowDataFromFile, ClusterCount];
            column = 1;

            for (int row = 1; sheet.GetRow(row).GetCell(0) != null; row++)
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
            double MaxValTerm = ElementsMatrix[0, 0], MinValTerm = ElementsMatrix[0, 0];

            for (column = 0; column < ClusterCount; column++)
            {
                MaxValTerm = ElementsMatrix[0, column];
                MinValTerm = ElementsMatrix[0, column];
                for (int rown = 0; rown < counterFoRowDataFromFile; rown++)
                {
                    if (ElementsMatrix[rown, column] > MaxValTerm)
                    {
                        MaxValTerm = ElementsMatrix[rown, column];
                    }
                    if (ElementsMatrix[rown, column] < MinValTerm)
                    {
                        MinValTerm = ElementsMatrix[rown, column];
                    }
                }
                FindMinMaxValueTerm[column, 0] = MinValTerm;
                FindMinMaxValueTerm[column, 1] = MaxValTerm;
            }
            ValueIntervalTerm = new double[ClusterCount, ClusterCount];
            double interval = 0;

            for (int i = 0; i < ClusterCount; i++)
            {
                interval = (FindMinMaxValueTerm[i, 1] - FindMinMaxValueTerm[i, 0]) / (ClusterCount - 1);
                ValueIntervalTerm[i, 0] = FindMinMaxValueTerm[i, 0];
                ValueIntervalTerm[i, ClusterCount-1] = FindMinMaxValueTerm[i, 1];
                for (int j = 1; j < ClusterCount - 1; j++)
                {
                    ValueIntervalTerm[i, j] = ValueIntervalTerm[i, j - 1] + interval;
                }
            }
            int a = 0;
        }

        public static void GiveNameToTerms(int ClusterCount)
        {
            switch (ClusterCount)
            { 
                case 1:
                case 2:
                case 3:
                    Console.WriteLine("Использовано 3 названия термов");
                    NameOfTerms.Add("много");
                    NameOfTerms.Add("среднее");
                    NameOfTerms.Add("малое");
                    break;
                case 4:
                case 5:
                    Console.WriteLine("Использовано 5 названия термов");
                    NameOfTerms.Add("большое");
                    NameOfTerms.Add("меньше большого");
                    NameOfTerms.Add("среднее");
                    NameOfTerms.Add("больше малого");
                    NameOfTerms.Add("малое");
                    break;
                case 6:
                case 7:
                    Console.WriteLine("Использовано 7 названия термов");
                    NameOfTerms.Add("большое");
                    NameOfTerms.Add("меньше большого");
                    NameOfTerms.Add("больше среднего");
                    NameOfTerms.Add("среднее");
                    NameOfTerms.Add("меньше среднего");
                    NameOfTerms.Add("больше малого");
                    NameOfTerms.Add("малое");
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }
        }

        public static void ReadDataFromXLSFile() // Function for reading data from the file .xls  // (object sender, EventArgs e)
        {
            HSSFWorkbook hssfwb;

            using (FileStream file = new FileStream(@"c:\Data.xls", FileMode.Open, FileAccess.Read))
            {
                hssfwb = new HSSFWorkbook(file);
            }

            ISheet sheet = hssfwb.GetSheet("FirstList");
            List<string> ListLinguisticVariables = new List<string>();
            int weight = 10, height = 10;
            string[,] ListTerms = new string[weight, height]; // not correct saving terms

            for (int column = 1; sheet.GetRow(0).GetCell(column) != null; column++)
            {
                ListLinguisticVariables.Add(string.Format("{0: 0.0}", sheet.GetRow(0).GetCell(column)));
                for (int row = 1, count = 0; row < sheet.LastRowNum; row++, count = 0)
                {
                    while (true)
                    {
                        if (ListTerms[column, count] == "")
                        {
                            break;
                        }
                        else if (ListTerms[column, count] == null)
                        {
                            ListTerms[column, count] = string.Format("{0: 0.0}", sheet.GetRow(row).GetCell(column));
                            break;
                        }
                        else if (ListTerms[column, count] == string.Format("{0: 0.0}", sheet.GetRow(row).GetCell(column)))
                        {
                            break;
                        }
                        else if (ListTerms[column, count] != string.Format("{0: 0.0}", sheet.GetRow(row).GetCell(column)))
                        {
                            count++;
                        }
                    }
                }
            }
            Console.ReadKey();
        }

        public static void ReadDataFromCSVFile() // Function for reading data from the file .csv
        {
            var reader = new StreamReader(File.OpenRead(@"C:\DATAA.txt"));
            List<string> listA = new List<string>();
            List<string> listB = new List<string>();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(';');

                listA.Add(values[0]);
                listB.Add(values[1]);
            }
            Console.ReadKey();
        }
    }
    

}   