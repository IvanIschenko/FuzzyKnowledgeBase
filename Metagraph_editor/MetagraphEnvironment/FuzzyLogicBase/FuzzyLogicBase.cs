using System;
using System.Collections.Generic;
using Functions;
using L.Algorithms.Clustering;
using WebApplication1;
using System.Xml;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Model;
using System.Linq;

namespace WebApplication1.FuzzyLogicBase
{
    public class FuzzyKnowledgeBase
    {
        public List<LinguisticVariable> ListVar = new List<LinguisticVariable>();
        public List<Rule> ListOfRule = new List<Rule>();
        public static MainWindowVM Editor = new MainWindowVM();
        public static double Defazification (Term term, double y)
        {
            return y * (term.b - term.a) + term.a;
        }
        public FuzzyKnowledgeBase FormFKBFromFile(string FileType, string PathToFile)
        {
            FuzzyKnowledgeBase FKB = new FuzzyKnowledgeBase();
            FKB.ListOfRule.Clear();
            FKB.ListVar.Clear();
            if (FileType == "xls")
            {
                Program.ReadDataFromXLSFileOneWatch(PathToFile);
                K_means k = new K_means(Program.ElementsMulti, null, Program.ClusterCount, Program.ElementsMatrix);
                double epsilon = 0.05;
                k.Clustering(Program.ClusterCount, epsilon);
                k.FindRulesModelTypeMamdani(Program.NameOfLinguisticVariables, Program.ValueIntervalTerm, Program.NameOfTerms, Program.countColumnData, Program.NumbersOfZonesOneLP, Program.counterFoRowDataFromFile, "Трикутна", Program.WeightOfTerms, FKB);
                k.GausFunction(Program.countColumnData, FKB);
                Program.WithRullToVar(FKB);
            }
            else if (FileType == "txt")
            {
                FKB = Program.WithJsonToBNZ(PathToFile);
            }

            else if (FileType == "xml")
            {
                Program.ReadAndConvertDataFromXMLFile(PathToFile);
                Program.ReadDataFromXLSFileOneWatch(@"C:\Program Files (x86)\IIS Express\test.xls");
                K_means k = new K_means(Program.ElementsMulti, null, Program.ClusterCount, Program.ElementsMatrix);
                double epsilon = 0.05;
                k.Clustering(Program.ClusterCount, epsilon);
                k.FindRulesModelTypeMamdani(Program.NameOfLinguisticVariables, Program.ValueIntervalTerm, Program.NameOfTerms, Program.countColumnData, Program.NumbersOfZonesOneLP, Program.counterFoRowDataFromFile, "Трикутна", Program.WeightOfTerms, FKB);
                k.GausFunction(Program.countColumnData, FKB);
                Program.WithRullToVar(FKB);
            }
            return FKB;
        }

        public String MakeConclusion(String LPForConclusion, ArgumentsForConclusion ConclArg, FuzzyKnowledgeBase FKB)
        {
            Editor.metagraph = MetagraphGenerator.GenerateRandomMetagraph(FKB, FKB.ListVar.Count, FKB.ListOfRule.Count, 1000, 400, 0.03f, 0.03f);
            for (int i = 0; i < Editor.metagraph.Edges.Count; i++)
            {
                for (int j = 0; j < Editor.metagraph.Edges[i].StartVertex.IncludedVertices.Count; j++)
                {
                    Editor.metagraph.Edges[i].StartVertex.IncludedVertices[j].ZnachFaz = -1;
                }
                Editor.metagraph.Edges[i].EndVertex.ZnachFaz = -1;
                Editor.metagraph.Edges[i].StartVertex.ZnachFaz = -1;
                Editor.metagraph.Edges[i].ZnachFaz = -1;
            }

            int number_TextBox = 0;
            List<double> provirka = new List<double>();
            for (int i = FKB.ListOfRule.Count; i < Editor.metagraph.Vertices.Count; i++)
            {
                if (Editor.metagraph.Vertices[i].IncomingEdgesCount == 0)
                {
                    for (int j = 0; j < ConclArg.ListNamesLinguisticVar.Count; j++)
                    {
                        if (ConclArg.ListNamesLinguisticVar[j] == Editor.metagraph.Vertices[i].NameLP)
                        {
                            number_TextBox = j;
                            break;
                        }
                    }
                }
            }

            List<double> prost = new List<double>();
            for (int i = 0; i < Editor.metagraph.Edges.Count; i++)
            {
                for (int j = 0; j < Editor.metagraph.Edges[i].StartVertex.IncludedVertices.Count; j++)
                {
                    for (int n = 0; n < ConclArg.ListNamesLinguisticVar.Count; n++)
                    {
                        if (Editor.metagraph.Edges[i].StartVertex.IncludedVertices[j].NameLP == ConclArg.ListNamesLinguisticVar[n])
                        {
                            for (int k = 0; k < FKB.ListVar.Count; k++)
                            {
                                if (FKB.ListVar[k].Name == ConclArg.ListNamesLinguisticVar[n])
                                {
                                    for (int p = 0; p < FKB.ListVar[k].terms.Count; p++)
                                    {
                                        if (FKB.ListVar[k].terms[p].Name == Editor.metagraph.Edges[i].StartVertex.IncludedVertices[j].Name)
                                        {
                                            try
                                            {
                                                Editor.metagraph.Edges[i].StartVertex.IncludedVertices[j].ZnachFaz = FKB.ListVar[k].terms[p].Fp(Convert.ToDouble(ConclArg.ListValuesLinguisticVar[n]));
                                            }
                                            catch
                                            {
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }
            double min;
            for (int i = 0; i < Editor.metagraph.Edges.Count; i++)
            {
                min = Double.PositiveInfinity;
                for (int j = 0; j < Editor.metagraph.Edges[i].StartVertex.IncludedVertices.Count; j++)
                {
                    if (Editor.metagraph.Edges[i].StartVertex.IncludedVertices[j].ZnachFaz < min && Editor.metagraph.Edges[i].StartVertex.IncludedVertices[j].ZnachFaz != 0)
                    {
                        min = Editor.metagraph.Edges[i].StartVertex.IncludedVertices[j].ZnachFaz;
                    }
                }
                if (min == Double.PositiveInfinity)
                {
                    min = 0;
                }
                Editor.metagraph.Edges[i].StartVertex.ZnachFaz = min;

            }

            for (int i = 0; i < Editor.metagraph.Edges.Count; i++)
            {
                if (Editor.metagraph.Edges[i].EndVertex.ZnachFaz == -1)
                {
                    for (int j = 0; j < Editor.metagraph.Edges.Count; j++)
                    {
                        if (Editor.metagraph.Edges[j].EndVertex.Name == Editor.metagraph.Edges[i].EndVertex.Name && Editor.metagraph.Edges[j].EndVertex.NameLP == Editor.metagraph.Edges[i].EndVertex.NameLP)
                        {
                            if (Editor.metagraph.Edges[j].StartVertex.ZnachFaz != -1)
                            {
                                if (Editor.metagraph.Edges[j].StartVertex.ZnachFaz > Editor.metagraph.Edges[i].EndVertex.ZnachFaz)
                                {
                                    Editor.metagraph.Edges[i].EndVertex.ZnachFaz = Editor.metagraph.Edges[j].StartVertex.ZnachFaz;
                                    Editor.metagraph.Edges[i].ZnachFaz = Editor.metagraph.Edges[j].StartVertex.ZnachFaz;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                if (i == Editor.metagraph.Edges.Count - 1)
                {
                    bool isvseok = true;
                    for (int j = 0; j < Editor.metagraph.Edges.Count; j++)
                    {
                        if (Editor.metagraph.Edges[j].EndVertex.ZnachFaz == -1)
                        {
                            isvseok = false;
                            break;
                        }
                    }
                    if (!isvseok)
                    {
                        i = 0;
                    }
                }
            }
            int index_max_znach = -1;
            double maxznacterm = Double.NegativeInfinity;
            for (int i = 0; i < Editor.metagraph.Edges.Count; i++)
            {
                if (Editor.metagraph.Edges[i].EndVertex.NameLP == LPForConclusion)
                {
                    if (Editor.metagraph.Edges[i].EndVertex.ZnachFaz > maxznacterm)
                    {
                        maxznacterm = Editor.metagraph.Edges[i].EndVertex.ZnachFaz;
                        index_max_znach = i;
                    }
                }
            }

            double defaz = 0;
            for (int i = 0; i < FKB.ListVar.Count; i++)
            {
                if (FKB.ListVar[i].Name == Editor.metagraph.Edges[index_max_znach].EndVertex.NameLP)
                {
                    for (int j = 0; j < FKB.ListVar[i].terms.Count; j++)
                    {
                        if (FKB.ListVar[i].terms[j].Name == Editor.metagraph.Edges[index_max_znach].EndVertex.Name)
                        {
                            defaz = FuzzyKnowledgeBase.Defazification(FKB.ListVar[i].terms[j], Editor.metagraph.Edges[index_max_znach].EndVertex.ZnachFaz);
                        }
                    }
                    break;
                }
            }
            for (int i = 0; i < FKB.ListVar.Count; i++)
            {
                if (FKB.ListVar[i].Name == Editor.metagraph.Edges[index_max_znach].EndVertex.NameLP)
                {
                    for (int j = 0; j < FKB.ListVar[i].terms.Count; j++)
                    {
                        if (FKB.ListVar[i].terms[j].Name == Editor.metagraph.Edges[index_max_znach].EndVertex.Name)
                        {
                            defaz = FuzzyKnowledgeBase.Defazification(FKB.ListVar[i].terms[j], Editor.metagraph.Edges[index_max_znach].EndVertex.ZnachFaz);
                        }
                    }
                    break;
                }
            }
            double rezult = defaz;
            string rez;
            if (rezult <= 1)
            {
                rez = "підводний об’єкт";
            }
            else if (rezult >= 2 && rezult < 3)
            {
                rez = " надводний об’єкт";
            }
            else
            {
                rez = "повітряний об’єкт";
            }
            return /*"Ймовірність відповіді: " + Editor.metagraph.Edges[index_max_znach].EndVertex.ZnachFaz +*/ " Тип: " + rez;
        }
        public void SetShortNameLV()
        {
            for (int i = 0; i < this.ListVar.Count; ++i)
            {
                this.ListVar[i].ShortName = "A" + i;
                for (int j = 0; j < this.ListVar[i].terms.Count; ++j)
                {
                    this.ListVar[i].terms[j].ShortNameTerm = "X" + j;
                }
            }
        }
        public static String runFuzzy(string resultedLV, string [] ArrayOfNameAllLV, string [] ArrayOfValuesLV, string typeFile, string pathToFile)
        {
            WebApplication1.FuzzyLogicBase.FuzzyKnowledgeBase FKB = new WebApplication1.FuzzyLogicBase.FuzzyKnowledgeBase();
            FKB = FKB.FormFKBFromFile(typeFile, pathToFile);
            Program.FunctionForClearingDataForHierarchy();
            WebApplication1.FuzzyLogicBase.ArgumentsForConclusion arguments = new WebApplication1.FuzzyLogicBase.ArgumentsForConclusion();

            arguments.nameResultedLinguisticVar = resultedLV;
            for(int i = 0; i < ArrayOfNameAllLV.Length; ++i)
            {
                if(ArrayOfNameAllLV[i] == resultedLV)
                {
                    continue;
                }
                arguments.ListNamesLinguisticVar.Add(ArrayOfNameAllLV[i]);
                arguments.ListValuesLinguisticVar.Add(ArrayOfValuesLV[i]);
            }
            Program.FunctionForClearingDataForHierarchy();
            return FKB.MakeConclusion(arguments.nameResultedLinguisticVar, arguments, FKB);
        }

        public void FormFKBfromXML(string path1, string path2) // Формирование БНЗ с xml
        {
            WebApplication1.FuzzyLogicBase.FuzzyKnowledgeBase FKB = new WebApplication1.FuzzyLogicBase.FuzzyKnowledgeBase();
            FKB = FKB.FormFKBFromFile("xml", path1);

            Program.Save_BNZ(FKB, path2);
        }

        public void runFuzzyWithXML(string path2, string path3, string resultedLV)//path3 путь к их xml
        {
            WebApplication1.FuzzyLogicBase.FuzzyKnowledgeBase FKB = new WebApplication1.FuzzyLogicBase.FuzzyKnowledgeBase();
            FKB = Program.WithJsonToBNZ(path2);

            List<string> ValuesLVNode = new List<string>();

           // XmlTextReader read = new XmlTextReader(path3);
            XmlDocument document = new XmlDocument();
            
            document.Load(path3);
                for (int j = 0; j < document.DocumentElement.ChildNodes.Count; j++)
                {
                ArgumentsForConclusion args = new ArgumentsForConclusion();
                for (int i = 0; i < document.DocumentElement.ChildNodes[j].Attributes.Count - 1; i++)
                {                    
                    ValuesLVNode.Add(document.DocumentElement.ChildNodes[j].Attributes[i].Value);
                    args.ListNamesLinguisticVar[i] = ListVar[i].Name;
                    
                }
                args.ListValuesLinguisticVar = ValuesLVNode;
                args.nameResultedLinguisticVar = ListVar[ListVar.Count - 1].Name;
                String Conclusion = MakeConclusion(ListVar[ListVar.Count - 1].Name, args, FKB);//вернуть не терм наслидок, а число!!                   
                document.DocumentElement.ChildNodes[j].Attributes[document.DocumentElement.ChildNodes[j].Attributes.Count - 1].Value = Conclusion;
                }
            }
        }
}