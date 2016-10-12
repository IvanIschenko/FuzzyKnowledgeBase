using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using System.Web.Mvc;
using Newtonsoft.Json;
using System.Runtime.Serialization;
//using System.Web.Helpers;
using System.Drawing;
using PagedList;

using C__04._07._2015.Algorithms.Clustering;// progr_V.cs
using L.DataStructures.Matrix;// multidim..cs
using L.Algorithms.Clustering;// k_means.cs
using L.DataStructures.Geometry; // cluster.cs
using System.Web.Script.Serialization;
using System.Threading;
using System.IO;
using Functions;
using WebApplication1.FuzzyLogicBase;

using System.Xml;
using System.Text;

namespace WebApplication1
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        public static FuzzyKnowledgeBase FKB = new FuzzyKnowledgeBase();
        public static List<string> short_nameLP = new List<string>();
        public static string proba;
        public static TreeView TreVie = new TreeView();
        public static TreeNode Tree = new TreeNode();
        public TreeNodeTypes ShowCheckBoxes { get; set; }
        public static int proverka = 0;
        public static int indexLp = 0;
        public static int indexTerm = 0;
        public static List<LinguisticVariable> ListVarforCheck = new List<LinguisticVariable>();
        public static List<LinguisticVariable> ListVarFromFile = new List<LinguisticVariable>(); // file xls
        public static List<Term> ListTermIF = new List<Term>();
        public static Term termThen;
        public static Rule rul;
        public static List<double> Point = new List<double>();
        public static Guid ID = new Guid();
        public static List<Term> LiTermsvuvid = new List<Term>();
        public static string TypeFP = "";
        public static int minAx = 0, maxAx = 0;
        public static bool FirstCheck = false;


        public static string rule_n = "";
        public static List<Label> MinMax = new List<Label>();
        protected static List<TextBox> if_tb = new List<TextBox>();
        public static MainWindowVM Editor = new MainWindowVM();
        public static string jsonstring = "";
        public static Thread t;
        public static int rah = 0;
        public static List<TextBox> list_Text_Box = new List<TextBox>();
        public static List<Label> list_label = new List<Label>();

        public static List<string> LP_List = new List<string>();

        public static bool edit = false;
        public static string selectedLP_Tree = null;
        public static string selectedTerm_Tree = null;

        public static string BNZInJSONString = "";
        public static string BNZInJSONStringMain = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            TabContainer1.ActiveTabChanged += TabMetagraphOnClick;
            ListOfRulesBox.SelectedIndexChanged += ListOfRulesBox_SelectedIndexChanged;
            if (list_Text_Box.Count != 0)
            {
                list_Text_Box.Clear();
                SetTableOfDynamicTextBoxes();
            }
        }

        protected void TabMetagraphOnClick(object sender, EventArgs e)
        {
            if(TabContainer1.ActiveTabIndex == 1)
            {
                RefreshCommand();
                Response.Redirect("http://localhost:46930/Home/Index");
            }
        }

        protected void SetTableOfDynamicTextBoxes()
        {
            for (int i = 0; i < rah; ++i)
            {
                TextBox tb = new TextBox();
                list_Text_Box.Add(tb);
                Table tab = new Table();
                TableRow row = new TableRow();
                TableCell cell = new TableCell();
                TableCell cell2 = new TableCell();
                cell.Text = list_label[i].Text + MinMax[i].Text;
                cell.Width = 200;
                cell2.Controls.Add(tb);
                row.Controls.Add(cell);
                row.Controls.Add(cell2);
                tab.Controls.Add(row);
                PanelToAddDynamicTextBox.Controls.Add(tab);
                ConcludeUpdatePanel.Update();
            }
        }
        protected void FormTreeFromFKB()
        {
            TreeViewBNZ.Nodes.Clear();
            for (int i = 0; i < FKB.ListVar.Count; i++)
            {
                TreeNode nod = new TreeNode();
                nod.Text = FKB.ListVar[i].Name;
                TreeViewBNZ.Nodes.Add(nod);
                for (int j = 0; j < FKB.ListVar[i].terms.Count; j++)
                {
                    TreeNode tm = new TreeNode();
                    tm.Text = FKB.ListVar[i].terms[j].Name;
                    TreeViewBNZ.Nodes[i].ChildNodes.Add(tm);
                    TreeViewBNZ.Nodes[i].ChildNodes[j].ShowCheckBox = true;
                }
            }
            TreePan.Update();
        }
        protected void FormListOfRules()
        {
            for(int i = 0; i < FKB.ListOfRule.Count; ++i)
            {
                string antecedent = "";
                for (int j = 0; j < FKB.ListOfRule[i].Antecedents.Count; ++j) {
                    if (j < FKB.ListOfRule[i].Antecedents.Count - 1)
                    {
                        antecedent += FKB.ListOfRule[i].Antecedents[j].NameLP + " = " + FKB.ListOfRule[i].Antecedents[j].Name + " ТА ";
                    }
                    else
                    {
                        antecedent += FKB.ListOfRule[i].Antecedents[j].NameLP + " = " + FKB.ListOfRule[i].Antecedents[j].Name;
                    }
                }
                ListOfRulesBox.Items.Add("ЯКЩО " + antecedent + " ТО " + FKB.ListOfRule[i].Cоnsequens.NameLP + " = " + FKB.ListOfRule[i].Cоnsequens.Name);
            }
        }
        protected void DefineLPPossibleToConclude()
        {
            for (int i = 0; i < Editor.metagraph.Vertices.Count; i++)
            {
                if (Editor.metagraph.Vertices[i].IncomingEdgesCount != 0)
                {
                    bool IsLPCorrect = true;
                    if (ConcludeLP.Items.Count == 0)
                    {
                        ConcludeLP.Items.Add(Editor.metagraph.Vertices[i].NameLP);
                    }
                    else
                    {
                        for (int j = 0; j < ConcludeLP.Items.Count; j++)
                        {
                            if (ConcludeLP.Items[j].Text == Editor.metagraph.Vertices[i].NameLP)
                            {
                                IsLPCorrect = false;
                                break;
                            }
                        }
                        if (IsLPCorrect)
                        {
                            ConcludeLP.Items.Add(Editor.metagraph.Vertices[i].NameLP);
                        }
                    }

                }
            }
        }
        protected void UploadFileAddToOld(object sender, EventArgs e) {
            string pathToCheck = "";
            string fileName = "";
            string format = "";
            if (FileUploadOne.HasFile)
            {
                FileUploadOne.BorderWidth = 0;
                string savePath = @"D:\home\site\doc\";
                if (FileUploadOne.HasFile)
                {
                    fileName = FileUploadOne.FileName;

                    for (int i = 0; i < fileName.Length; i++)
                    {
                        if (fileName[i] == '.')
                        {
                            for (int j = i + 1; j < fileName.Length; j++)
                            {
                                format += fileName[j];
                            }
                            break;
                        }
                    }
                }
                pathToCheck = savePath + fileName;
                FKB = FKB.FormFKBFromFile(format, pathToCheck);
                BNZInJSONString = System.Web.Helpers.Json.Encode(FKB);

                //BNZInJSONString = File.ReadAllText(@"C:\Metagraph_docs\FKB.txt");
                BNZInJSONStringMain = File.ReadAllText(@"D:\home\site\doc\BNZ.txt");
                using (StreamWriter sw = File.CreateText(@"D:\home\site\doc\BNZ.txt"))
                {
                    sw.WriteLine(Program.Conected(BNZInJSONStringMain, BNZInJSONString));
                }
                FKB = Program.WithJsonToBNZ(@"C:\Metagraph_docs\BNZ.txt");
            }
            else
            {
                FileUploadOne.BorderWidth = 2;
                FileUploadOne.BorderColor = Color.Red;
            }
            //ConvertMetagraphToGraph();
            
        }
        protected void UploadFileCreateNew(object sender, EventArgs e)
        {
            string pathToCheck = "";
            string fileName = "";
            string format = "";
            if (FileUploadOne.HasFile)
            {
                FileUploadOne.BorderWidth = 0;
                string savePath = @"C:\Metagraph_docs\";
                if (FileUploadOne.HasFile)
                {
                    fileName = FileUploadOne.FileName;
                    
                    for(int i = 0; i < fileName.Length; i++)
                    { 
                        if (fileName[i] == '.')
                        {
                            for(int j =i+1; j < fileName.Length; j++)
                            {
                                format += fileName[j];
                            }
                            break;
                        }                   
                    }
                }
                pathToCheck = savePath + fileName;
                FKB = FKB.FormFKBFromFile(format, pathToCheck);
                FKB.SetShortNameLV();
                LevelRules();
                RefreshCommand();
                Program.FunctionForClearingDataForHierarchy();
                Term.Sort(FKB);
                FormTreeFromFKB();
                FormListOfRules();
                DefineLPPossibleToConclude();               
            }
            else
            {
                FileUploadOne.BorderWidth = 2;
                FileUploadOne.BorderColor = Color.Red;
            }
            Program.Save_BNZ(FKB, @"C:\Metagraph_docs\BNZ.txt");
            //ConvertMetagraphToGraph();
        }

        public static void ConvertMetagraphToGraph()
        {
            XmlTextWriter textWritter = new XmlTextWriter(@"C:\Metagraph_docs\xml\MetaToGraph.xml", Encoding.GetEncoding("windows-1251")); //Создание начала документа
            textWritter.WriteStartDocument();
            textWritter.WriteStartElement("Graph");
            textWritter.WriteEndElement();
            textWritter.Close(); // Конец создания начала документа

            XmlDocument document = new XmlDocument();
            document.Load(@"C:\Metagraph_docs\xml\MetaToGraph.xml");
            XmlAttribute nodesize = document.CreateAttribute("nodesize");
            nodesize.Value = "10";
            document.DocumentElement.Attributes.Append(nodesize);
            XmlAttribute vspacing = document.CreateAttribute("vspacing");
            vspacing.Value = "3";
            document.DocumentElement.Attributes.Append(vspacing);
            XmlAttribute hspacing = document.CreateAttribute("hspacing");
            hspacing.Value = "20";
            document.DocumentElement.Attributes.Append(hspacing);
            XmlAttribute padding = document.CreateAttribute("padding");
            padding.Value = "4";
            XmlAttribute showarrows = document.CreateAttribute("showarrows");
            showarrows.Value = "true";
            document.DocumentElement.Attributes.Append(showarrows);
            XmlAttribute guidG = document.CreateAttribute("guid");
            guidG.Value = "CD8B0884-73D9-FDA2-1206-72A17502C457"; //!!!
            document.DocumentElement.Attributes.Append(guidG);
            XmlAttribute searchurl = document.CreateAttribute("searchurl");
            searchurl.Value = "https://www.google.com.ua/#output=search%26q=###SEARCH###"; //!!!
            document.DocumentElement.Attributes.Append(searchurl);

            //Наче зайве
            //XmlNode datagroups = document.CreateElement("datagroups"); // create element datagroups
            //document.DocumentElement.AppendChild(datagroups); // указываем родителя

            XmlNode Nodes = document.CreateElement("Nodes");
            document.DocumentElement.AppendChild(Nodes);

            for (int i = 0; i < WebApplication1.WebForm1.FKB.ListOfRule.Count; i++)
            {

                XmlNode Node = document.CreateElement("Node"); // даём имя
                //Node.InnerText = "значение!!!!"; // и значение
                Nodes.AppendChild(Node); // и указываем кому принадлежит
                XmlAttribute guidID = document.CreateAttribute("guid"); // создаём атрибут
                guidID.Value = "" + i;
                Node.Attributes.Append(guidID); // добавляем атрибут
                XmlAttribute nodeName = document.CreateAttribute("nodeName");
                nodeName.Value = "M" + i;
                Node.Attributes.Append(nodeName);
                XmlAttribute nclass = document.CreateAttribute("nclass");
                nclass.Value = "";
                Node.Attributes.Append(nclass);
                XmlAttribute shape = document.CreateAttribute("shape");
                shape.Value = "square";
                Node.Attributes.Append(shape);
                XmlAttribute color = document.CreateAttribute("color");
                color.Value = "13421772";
                Node.Attributes.Append(color);
                XmlAttribute xPos = document.CreateAttribute("xPos");
                xPos.Value = "" + WebApplication1.WebForm1.Editor.metagraph.Vertices[i].Coordinates.X;
                Node.Attributes.Append(xPos);
                XmlAttribute yPos = document.CreateAttribute("yPos");
                yPos.Value = "" + WebApplication1.WebForm1.Editor.metagraph.Vertices[i].Coordinates.Y;
                Node.Attributes.Append(yPos);
                XmlAttribute font = document.CreateAttribute("font");
                font.Value = "Verdana";
                Node.Attributes.Append(font);
                XmlAttribute fontsize = document.CreateAttribute("fontsize");
                fontsize.Value = "10";
                Node.Attributes.Append(fontsize);

                for (int j = 0; j < WebApplication1.WebForm1.Editor.metagraph.Vertices[i].IncludedVertices.Count; ++j)
                {
                    XmlNode data = document.CreateElement("data");
                    data.InnerText = "" + WebApplication1.WebForm1.Editor.metagraph.Vertices[i].IncludedVertices[j].NameLP + ": " + WebApplication1.WebForm1.Editor.metagraph.Vertices[i].IncludedVertices[j].Name;
                    Node.AppendChild(data);
                }
            }
            for (int i = 0; i < WebApplication1.WebForm1.FKB.ListOfRule.Count; i++)
            {

                XmlNode Node = document.CreateElement("Node"); // даём имя
                //Node.InnerText = "значение!!!!"; // и значение
                Nodes.AppendChild(Node); // и указываем кому принадлежит
                XmlAttribute guidID = document.CreateAttribute("guid"); // создаём атрибут
                guidID.Value = "" + WebApplication1.WebForm1.Editor.MetaVertexCount + i;
                Node.Attributes.Append(guidID); // добавляем атрибут
                XmlAttribute nodeName = document.CreateAttribute("nodeName");
                nodeName.Value = "R" + i;
                Node.Attributes.Append(nodeName);
                XmlAttribute nclass = document.CreateAttribute("nclass");
                nclass.Value = "";
                Node.Attributes.Append(nclass);
                XmlAttribute shape = document.CreateAttribute("shape");
                shape.Value = "square";
                Node.Attributes.Append(shape);
                XmlAttribute color = document.CreateAttribute("color");
                color.Value = "13421772";
                Node.Attributes.Append(color);
                XmlAttribute xPos = document.CreateAttribute("xPos");
                xPos.Value = "" + WebApplication1.WebForm1.Editor.metagraph.Edges[i].EndVertex.Coordinates.X;
                Node.Attributes.Append(xPos);
                XmlAttribute yPos = document.CreateAttribute("yPos");
                yPos.Value = "" + WebApplication1.WebForm1.Editor.metagraph.Edges[i].EndVertex.Coordinates.Y;
                Node.Attributes.Append(yPos);
                XmlAttribute font = document.CreateAttribute("font");
                font.Value = "Verdana";
                Node.Attributes.Append(font);
                XmlAttribute fontsize = document.CreateAttribute("fontsize");
                fontsize.Value = "10";
                Node.Attributes.Append(fontsize);
                XmlNode data = document.CreateElement("data");
                data.InnerText = "" + WebApplication1.WebForm1.Editor.metagraph.Edges[i].EndVertex.NameLP + ": " + WebApplication1.WebForm1.Editor.metagraph.Edges[i].EndVertex.Name;
                Node.AppendChild(data);
            }

            XmlNode Linkgroups = document.CreateElement("Linkgroups");
            document.DocumentElement.AppendChild(Linkgroups);
            XmlNode Group = document.CreateElement("Group");
            Group.InnerText = "значение!!!!";
            Linkgroups.AppendChild(Group);
            XmlAttribute name = document.CreateAttribute("name");
            name.Value = "Default";
            Group.Attributes.Append(name);
            XmlAttribute colorl = document.CreateAttribute("colorl");
            colorl.Value = "10066329";
            Group.Attributes.Append(colorl);

            XmlNode Edges = document.CreateElement("Edges");
            document.DocumentElement.AppendChild(Edges);
            for (int i = 0; i < WebApplication1.WebForm1.Editor.metagraph.Edges.Count; ++i)
            {
                XmlNode Edge = document.CreateElement("Edge");
                Edges.AppendChild(Edge);
                XmlAttribute guid = document.CreateAttribute("guid");
                guid.Value = "12222ad" + i;
                Edge.Attributes.Append(guid);
                XmlAttribute edgeName = document.CreateAttribute("edgeName");
                edgeName.Value = "link_" + (i + 1);
                Edge.Attributes.Append(edgeName);
                XmlAttribute node1 = document.CreateAttribute("node1");
                node1.Value = "" + WebApplication1.WebForm1.Editor.metagraph.Edges[i].StartVertex.Name;
                Edge.Attributes.Append(node1);
                XmlAttribute node2 = document.CreateAttribute("node2");
                node2.Value = "R" + i; //!!!
                Edge.Attributes.Append(node2);
                XmlAttribute group = document.CreateAttribute("group");
                group.Value = "Default";
                Edge.Attributes.Append(group);
                XmlAttribute istwoway = document.CreateAttribute("istwoway");
                istwoway.Value = "false";
                Edge.Attributes.Append(istwoway);
            }
            document.Save(@"C:\Metagraph_docs\xml\MetaToGraph.xml");  // Сохранение изменений

        }
        protected void Next_Click(object sender, EventArgs e)
        {
            if (FKB.ListVar.Count == 0)
            {
                FileUploadOne.BorderColor = Color.Red;
                return;
            }
            LP_List.Clear();
            list_Text_Box.Clear();
            MinMax.Clear();
            list_label.Clear();
            rah = 0;
            PlusTermButton.BorderWidth = 0;
            DefineLPNeededToConclude(sender, e, ConcludeLP.Text);
            if (PanelToAddDynamicTextBox.Controls.Count == 0)
            {
                SetTableOfDynamicTextBoxes();
            }
        }
        protected bool IsTextBoxEmpty(TextBox textBoxToValidate)
        {
            if (textBoxToValidate.Text == "")
            {
                textBoxToValidate.BorderColor = Color.Red;
                return true;
            }
            else
            {
                return false;
            }
        }
        protected void AddLPFromInterface_Click(object sender, EventArgs e)
        {
            if (IsTextBoxEmpty(NameLP)|| IsTextBoxEmpty(ShortNameLP)|| IsTextBoxEmpty(MinValueLP)|| IsTextBoxEmpty(MaxValueLP))
            {
                return;
            }
            double minim = 0;
            double maxim = 0;
            try
            {
                minim = Convert.ToDouble(MinValueLP.Text);
                maxim = Convert.ToDouble(MaxValueLP.Text);
                if (minim >= maxim)
                {
                    NameLP.BorderColor = Color.Black;
                    MinValueLP.BorderColor = Color.Red;
                    MaxValueLP.BorderColor = Color.Red;
                    return;
                }
            }
            catch
            {
                MinValueLP.BorderColor = Color.Red;
                MaxValueLP.BorderColor = Color.Red;
            }

            if (proverka == 0)
            {
                Tree.Text = NameLP.Text;
                TreeViewBNZ.Nodes.Add(Tree);;
            }
            else
            {
                Tree.Text = NameLP.Text;
                TreeViewBNZ.Nodes.Add(Tree);
                TreeViewBNZ.Nodes[indexLp].ShowCheckBox = false;
            }
            List<Term> ListTe = new List<Term>();
            LinguisticVariable LP = new LinguisticVariable(ID, NameLP.Text, ListTe, minim, maxim);
            if (ShortNameLP.Text != null)
            {
                short_nameLP.Add(ShortNameLP.Text);
            }
            else
            {
                short_nameLP.Add("");
            }
            indexLp++;
            NameLP.Text = "";

            TreePan.Update();
        }
        protected void TreeViewBNZ_TreeNodeCheckChanged(object sender, TreeNodeEventArgs e)
        {
            for (int i = 0; i < e.Node.Parent.ChildNodes.Count; ++i)
            {
                if (e.Node.Parent.ChildNodes[i].Checked == true)
                {
                    e.Node.Parent.ChildNodes[i].Checked = false;
                }
            }
            e.Node.Checked = true;
            TreePan.Update();
        }
        protected void DefineLPNeededToConclude(object sender, EventArgs e, string LPName)
        {
            bool proverka = true;
            for (int i = 0; i < FKB.ListOfRule.Count; i++)
            {
                if (FKB.ListOfRule[i].Cоnsequens.NameLP == LPName)
                {

                    for (int n = 0; n < FKB.ListOfRule[i].Antecedents.Count; n++)
                    {
                        proverka = true;
                        for (int k = 0; k < FKB.ListOfRule.Count; k++)
                        {
                            try
                            {
                                if (FKB.ListOfRule[k].Cоnsequens.NameLP == FKB.ListOfRule[i].Antecedents[n].NameLP && FKB.ListOfRule[i].Antecedents[n].NameLP != LPName)
                                {
                                    if (FKB.ListOfRule[i].Antecedents[n].ProverOut == false)
                                    {
                                        DefineLPNeededToConclude(sender, e, FKB.ListOfRule[i].Antecedents[n].NameLP);
                                        FKB.ListOfRule[i].Antecedents[n].ProverOut = true;
                                    }
                                    proverka = false;
                                }
                            }
                            catch
                            {
                                return;
                            }
                        }
                        if (proverka == true)
                        {
                            if (LP_List.Count == 0)
                            {
                                LP_List.Add(FKB.ListOfRule[i].Antecedents[n].NameLP);
                                Label lb = new Label();
                                Label mm = new Label();
                                for (int s = 0; s < LP_List.Count; ++s)
                                {
                                    for (int z = 0; z < FKB.ListVar.Count; ++z)
                                    {
                                        if (FKB.ListVar[z].Name == LP_List[s])
                                        {
                                            double MaxC = double.NegativeInfinity;
                                            double MinC = double.PositiveInfinity;
                                            for (int r = 0; r < FKB.ListVar[z].terms.Count; ++r)
                                            {
                                                if (MaxC < FKB.ListVar[z].terms[r].c)
                                                {
                                                    MaxC = FKB.ListVar[z].terms[r].c;
                                                }
                                                if (MinC > FKB.ListVar[z].terms[r].a)
                                                {
                                                    MinC = FKB.ListVar[z].terms[r].a;
                                                }
                                            }
                                            if (Convert.ToString(MinC).Length >= 5 && Convert.ToString(MaxC).Length >= 5)
                                            {
                                                mm.Text = "(" + Convert.ToString(MinC).Remove(4) + ";" + Convert.ToString(MaxC).Remove(4) + ")";
                                            }
                                            else if (Convert.ToString(MinC).Length >= 5 && Convert.ToString(MaxC).Length < 5)
                                            {
                                                mm.Text = "(" + Convert.ToString(MinC).Remove(4) + ";" + MaxC + ")";
                                            }
                                            else if (Convert.ToString(MinC).Length < 5 && Convert.ToString(MaxC).Length >= 5)
                                            {
                                                mm.Text = "(" + MinC + ";" + Convert.ToString(MaxC).Remove(4) + ")";
                                            }
                                            else
                                            {
                                                mm.Text = "(" + MinC + ";" + MaxC + ")";
                                            }
                                            break;
                                        }
                                    }
                                }
                                MinMax.Add(mm);
                                lb.Text = FKB.ListOfRule[i].Antecedents[n].NameLP;
                                list_label.Add(lb);
                                rah++;
                            }
                            bool proverka2 = true;
                            for (int m = 0; m < LP_List.Count; m++)
                            {
                                if (LP_List[m] == FKB.ListOfRule[i].Antecedents[n].NameLP)
                                {
                                    proverka2 = false;
                                    break;
                                }
                            }
                            if (proverka2 == true)
                            {
                                LP_List.Add(FKB.ListOfRule[i].Antecedents[n].NameLP);
                                Label mm = new Label();
                                for (int s = 0; s < LP_List.Count; ++s)
                                {
                                    for (int z = 0; z < FKB.ListVar.Count; ++z)
                                    {
                                        if (FKB.ListVar[z].Name == LP_List[s])
                                        {
                                            double MaxC = double.NegativeInfinity;
                                            double MinC = double.PositiveInfinity;
                                            for (int r = 0; r < FKB.ListVar[z].terms.Count; ++r)
                                            {
                                                if (MaxC < FKB.ListVar[z].terms[r].c)
                                                {
                                                    MaxC = FKB.ListVar[z].terms[r].c;
                                                }
                                                if (MinC > FKB.ListVar[z].terms[r].a)
                                                {
                                                    MinC = FKB.ListVar[z].terms[r].a;
                                                }
                                            }

                                            if (Convert.ToString(MinC).Length >= 5 && Convert.ToString(MaxC).Length >= 5)
                                            {
                                                mm.Text = "(" + Convert.ToString(MinC).Remove(4) + ";" + Convert.ToString(MaxC).Remove(4) + ")";
                                            }
                                            else if (Convert.ToString(MinC).Length >= 5 && Convert.ToString(MaxC).Length < 5)
                                            {
                                                mm.Text = "(" + Convert.ToString(MinC).Remove(4) + ";" + MaxC + ")";
                                            }
                                            else if (Convert.ToString(MinC).Length < 5 && Convert.ToString(MaxC).Length >= 5)
                                            {
                                                mm.Text = "(" + MinC + ";" + Convert.ToString(MaxC).Remove(4) + ")";
                                            }
                                            else
                                            {
                                                mm.Text = "(" + MinC + ";" + MaxC + ")";
                                            }
                                            break;
                                        }
                                    }
                                }

                                MinMax.Add(mm);
                                Label lb = new Label();
                                lb.Text = FKB.ListOfRule[i].Antecedents[n].NameLP;
                                list_label.Add(lb);
                                rah++;
                            }
                        }
                    }

                }
            }
        }
        protected void WithRullToVar()
        {
            for (int rule = Program.ostanovkaLP; rule < FKB.ListOfRule.Count; rule++)
            {
                for (int anc = 0; anc < FKB.ListOfRule[rule].Antecedents.Count; anc++)
                {
                    List<Term> spusokTermans = new List<Term>();
                    for (int termforlist = Program.ostanovkaLP; termforlist < FKB.ListOfRule.Count; termforlist++)
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
                for (int termforlist = Program.ostanovkaLP; termforlist < FKB.ListOfRule.Count; termforlist++, Program.ostanovkaLP++)
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
        
        //уровни
        protected void LevelRules()
        {
            for (int i = 0; i < FKB.ListOfRule.Count; i++)
            {
                FKB.ListOfRule[i].Level = -1;
            }
            int indexMaxLevelVar = 0;
            for (int i = 0; i < FKB.ListVar.Count; i++)
            {
                bool depending_test = true;
                for (int j = 0; j < FKB.ListOfRule.Count; j++)
                {
                    for (int k = 0; k < FKB.ListOfRule[j].Antecedents.Count; k++)
                    {
                        if (FKB.ListOfRule[j].Antecedents[k].NameLP == FKB.ListVar[i].Name)
                        {
                            depending_test = false;
                            break;
                        }

                    }
                    if (depending_test == false)
                    {
                        break;
                    }
                }
                if (depending_test == true)
                {
                    indexMaxLevelVar = i;
                    break;
                }
            }
            DataforLoghortVersion(FKB.ListVar[indexMaxLevelVar].Name);

        }
        public static int IdexLevel = -1;
        protected void DataforLoghortVersion(string LPName)
        {
            bool provrek = true;
            for (int i = 0; i < FKB.ListOfRule.Count; i++)
            {
                if (FKB.ListOfRule[i].Level == -1)
                {
                    provrek = false;
                    break;
                }
            }
            if (provrek)
            {
                return;
            }
            for (int i = 0; i < FKB.ListOfRule.Count; i++)
            {
                if (FKB.ListOfRule[i].Cоnsequens.NameLP == LPName)
                {
                    bool z = true;
                    for (int p = 0; p < FKB.ListOfRule.Count; p++)
                    {
                        if (FKB.ListOfRule[p].Cоnsequens.NameLP == LPName)
                        {
                            if (FKB.ListOfRule[p].Level == -1)
                            {
                                if (z == true)
                                {
                                    ++IdexLevel;
                                }
                                FKB.ListOfRule[p].Level = IdexLevel + 1;
                                z = false;
                            }
                        }
                    }
                    for (int n = 0; n < FKB.ListOfRule[i].Antecedents.Count; n++)
                    {

                        for (int k = 0; k < FKB.ListOfRule.Count; k++)
                        {
                            try
                            {
                                if (FKB.ListOfRule[k].Cоnsequens.NameLP == FKB.ListOfRule[i].Antecedents[n].NameLP)
                                {
                                    DataforLoghortVersion(FKB.ListOfRule[i].Antecedents[n].NameLP);

                                }
                            }
                            catch
                            {
                                return;
                            }
                        }
                    }

                }
            }
        }
        
        protected void TreeViewBNZ_SelectedNodeChanged(object sender, EventArgs e)
        {
            xyz.Visible = false;
            EditRuleTextBox.Text = TreeViewBNZ.SelectedNode.Text;
            if (TreeViewBNZ.SelectedNode.Parent == null)
            {
                for (int i = 0; i < FKB.ListVar.Count; i++)
                {
                    if (FKB.ListVar[i].Name == TreeViewBNZ.SelectedNode.Text)
                    {
                        selectedLP_Tree = FKB.ListVar[i].Name;
                        if (Type_FP_List.SelectedItem.Text == "Трикутна")
                        {
                            double Max = double.NegativeInfinity;
                            double Min = double.PositiveInfinity;
                            for (int j = 0; j < FKB.ListVar[i].terms.Count; j++)
                            {
                                if (FKB.ListVar[i].terms[j].c > Max)
                                {
                                    Max = FKB.ListVar[i].terms[j].c;
                                }
                                if (FKB.ListVar[i].terms[j].a < Min)
                                {
                                    Min = FKB.ListVar[i].terms[j].a;
                                }
                            }
                            xyz.Visible = true;
                            for (int j = 0; j < FKB.ListVar[i].terms.Count; j++)
                            {
                                Chart1.Visible = true;
                                Chart1.Series.Add(FKB.ListVar[i].terms[j].Name);
                                Chart1.Legends.Add(FKB.ListVar[i].terms[j].Name);
                                Chart1.ChartAreas[0].AxisX.Maximum = Max + 1;
                                Chart1.ChartAreas[0].AxisX.Minimum = Min - 1;
                                Chart1.ChartAreas[0].AxisX.LabelStyle.Format = "{0:0.00}";
                                Chart1.ChartAreas[0].AxisY.Maximum = 1.2;
                                Chart1.Series[FKB.ListVar[i].terms[j].Name].ChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.Line;
                                if (FKB.ListVar[i].terms[j].ProverLast == true)
                                {
                                    Chart1.Series[FKB.ListVar[i].terms[j].Name].Points.AddXY(FKB.ListVar[i].terms[j].a, 0);
                                    Chart1.Series[FKB.ListVar[i].terms[j].Name].Points.AddXY(FKB.ListVar[i].terms[j].b, 1);
                                    Chart1.Series[FKB.ListVar[i].terms[j].Name].Points.AddXY(FKB.ListVar[i].terms[j].c, 1);
                                    Chart1.Series[FKB.ListVar[i].terms[j].Name].Points.AddXY(FKB.ListVar[i].terms[j].c + 5, 1);
                                }
                                else if (FKB.ListVar[i].terms[j].ProverFirst == true)
                                {
                                    Chart1.Series[FKB.ListVar[i].terms[j].Name].Points.AddXY(FKB.ListVar[i].terms[j].a - 5, 1);
                                    Chart1.Series[FKB.ListVar[i].terms[j].Name].Points.AddXY(FKB.ListVar[i].terms[j].a, 1);
                                    Chart1.Series[FKB.ListVar[i].terms[j].Name].Points.AddXY(FKB.ListVar[i].terms[j].b, 1);
                                    Chart1.Series[FKB.ListVar[i].terms[j].Name].Points.AddXY(FKB.ListVar[i].terms[j].c, 0);
                                }
                                else
                                {
                                    Chart1.Series[FKB.ListVar[i].terms[j].Name].Points.AddXY(FKB.ListVar[i].terms[j].a, 0);
                                    Chart1.Series[FKB.ListVar[i].terms[j].Name].Points.AddXY(FKB.ListVar[i].terms[j].b, 1);
                                    Chart1.Series[FKB.ListVar[i].terms[j].Name].Points.AddXY(FKB.ListVar[i].terms[j].c, 0);
                                }

                                UpdatePanel1.Update();
                            }

                        }
                        else if (Type_FP_List.SelectedItem.Text == "Гаусівська")
                        {
                            K_means v = new K_means(Program.ElementsMulti, null, Program.ClusterCount, Program.ElementsMatrix);

                            double Max = v.MaxValueGausFunction(Program.countColumnData, i);
                            for (int j = 0; j < FKB.ListVar[i].terms.Count; j++)
                            {
                                if (FKB.ListVar[i].terms[j].c > Max)
                                {
                                    Max = FKB.ListVar[i].terms[j].c;
                                }
                            }
                            for (int j = 0; j < FKB.ListVar[i].terms.Count; j++)
                            {
                                Chart1.Series.Add(FKB.ListVar[i].terms[j].Name);
                                Chart1.Series[FKB.ListVar[i].terms[j].Name].ChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.Spline;
                                for (double k = 0.0; k < Max; k = k + 0.1)
                                {
                                    K_means point = new K_means(Program.ElementsMulti, null, Program.ClusterCount, Program.ElementsMatrix);
                                    Chart1.Series[FKB.ListVar[i].terms[j].Name].Points.AddXY(k, point.GausFunction(Program.countColumnData, k, i, j));
                                }
                            }
                        }
                    }
                }
            }
            if (TreeViewBNZ.SelectedNode.Parent != null)
            {
                string nazvaTerma = TreeViewBNZ.SelectedNode.Text;
                TreeViewBNZ.SelectedNode.Parent.Select();
                TreeViewBNZ_SelectedNodeChanged(sender, e);
                for (int k = 0; k < TreeViewBNZ.SelectedNode.ChildNodes.Count; k++)
                {
                    if (TreeViewBNZ.SelectedNode.ChildNodes[k].Text == nazvaTerma)
                    {
                        TreeViewBNZ.SelectedNode.ChildNodes[k].Select();
                        break;
                    }
                }
                selectedTerm_Tree = TreeViewBNZ.SelectedNode.Text;
                for (int i = 0; i < FKB.ListVar.Count(); ++i)
                {
                    if (FKB.ListVar[i].Name == selectedLP_Tree)
                    {
                        for (int k = 0; k < FKB.ListVar[i].terms.Count(); ++k)
                        {
                            if (FKB.ListVar[i].terms[k].Name == TreeViewBNZ.SelectedNode.Text)
                            {
                                Chart1.Series[TreeViewBNZ.SelectedNode.Text].BorderWidth = 4;
                                X.Text = Convert.ToString(FKB.ListVar[i].terms[k].a);
                                Y.Text = Convert.ToString(FKB.ListVar[i].terms[k].b);
                                Z.Text = Convert.ToString(FKB.ListVar[i].terms[k].c);
                                xyz.Update();
                            }
                        }
                    }
                }
            }
        }
        
        protected void EditRule_onTextChange(object sender, EventArgs e)
        {
            ListOfRulesBox.Items.Clear();
            string name = TreeViewBNZ.SelectedNode.Text;
            TreeViewBNZ.SelectedNode.Text = EditRuleTextBox.Text;
            if (TreeViewBNZ.SelectedNode.Parent == null)
            {
                for (int i = 0; i < FKB.ListVar.Count; i++)
                {
                    if (FKB.ListVar[i].Name == name)
                    {
                        FKB.ListVar[i].Name = EditRuleTextBox.Text;
                    }
                }
                for (int i = 0; i < FKB.ListOfRule.Count; i++)
                {
                    if (FKB.ListOfRule[i].Cоnsequens.NameLP == name)
                    {
                        FKB.ListOfRule[i].Cоnsequens.NameLP = EditRuleTextBox.Text;
                    }
                    for (int j = 0; j < FKB.ListOfRule[i].Antecedents.Count; j++)
                    {
                        if (FKB.ListOfRule[i].Antecedents[j].NameLP == name)
                        {
                            FKB.ListOfRule[i].Antecedents[j].NameLP = EditRuleTextBox.Text;
                        }

                    }
                }
            }
            else
            {

                for (int i = 0; i < FKB.ListVar.Count; i++)
                {
                    if (FKB.ListVar[i].Name == TreeViewBNZ.SelectedNode.Parent.Text)
                    {
                        for (int j = 0; j < FKB.ListVar[i].terms.Count; j++)
                        {
                            if (FKB.ListVar[i].terms[j].Name == name)
                            {
                                FKB.ListVar[i].terms[j].Name = EditRuleTextBox.Text;
                            }
                        }
                    }
                }
                for (int i = 0; i < FKB.ListOfRule.Count; i++)
                {
                    if (FKB.ListOfRule[i].Cоnsequens.NameLP == TreeViewBNZ.SelectedNode.Parent.Text && FKB.ListOfRule[i].Cоnsequens.Name == name)
                    {
                        FKB.ListOfRule[i].Cоnsequens.Name = EditRuleTextBox.Text;
                    }
                    for (int j = 0; j < FKB.ListOfRule[i].Antecedents.Count; j++)
                    {
                        if (FKB.ListOfRule[i].Antecedents[j].NameLP == TreeViewBNZ.SelectedNode.Parent.Text && FKB.ListOfRule[i].Antecedents[j].Name == name)
                        {
                            FKB.ListOfRule[i].Antecedents[j].Name = EditRuleTextBox.Text;
                        }

                    }
                }
            }
            for (int i = 0; i < FKB.ListOfRule.Count; ++i)
            {
                string temp = "";
                for (int j = 0; j < FKB.ListOfRule[i].Antecedents.Count(); ++j)
                {
                    temp = temp + FKB.ListOfRule[i].Antecedents[j].NameLP + " " + FKB.ListOfRule[i].Antecedents[j].Name + " i ";
                }
                ListOfRulesBox.Items.Add("Якщо" + " " + temp + " то " + FKB.ListOfRule[i].Cоnsequens.NameLP + " " + FKB.ListOfRule[i].Cоnsequens.Name);
                ListOfRulesBox.Items.Add("");
            }
            UpdateRules.Update();
        }

        protected void Metagraph_Click(object sender, EventArgs e)
        {
            if (FKB.ListVar.Count == 0)
            {
                FileUploadOne.BorderColor = Color.Red;
                return;
            }
            RefreshCommand();
            Response.Redirect("http://localhost:46930/Home/Index");
        }
        protected void EditRulesImageButton_Click(object sender, ImageClickEventArgs e) //карандаш
        {
            if (FKB.ListVar.Count == 0)
            {
                FileUploadOne.BorderColor = Color.Red;
                return;
            }
            edit = true;
            TreePan.Update();
            xyz.Visible = false;
            UpdatePanel1.Update();
        }
        protected void ListOfRulesBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < FKB.ListOfRule.Count; ++i)
            {
                string temp = "";
                for (int j = 0; j < FKB.ListOfRule[i].Antecedents.Count(); ++j)
                {
                    for (int k = 0; k < FKB.ListVar.Count; ++k)
                    {
                        if (FKB.ListVar[k].Name == FKB.ListOfRule[i].Antecedents[j].NameLP)
                        {
                            for (int n = 0; n < FKB.ListVar[k].terms.Count; ++n)
                            {
                                if (FKB.ListVar[k].terms[n].Name == FKB.ListOfRule[i].Antecedents[j].Name)
                                {
                                    if (j != FKB.ListOfRule[i].Antecedents.Count() - 1)
                                    {
                                        temp = temp + FKB.ListVar[k].ShortName + " = " + FKB.ListVar[k].terms[n].ShortNameTerm + " ТА ";
                                    }
                                    else
                                    {
                                        temp = temp + FKB.ListVar[k].ShortName + " = " + FKB.ListVar[k].terms[n].ShortNameTerm + " ";
                                    }
                                }
                            }
                        }
                    }
                }
                for (int j = 0; j < FKB.ListVar.Count; ++j)
                {
                    if (FKB.ListVar[j].Name == FKB.ListOfRule[i].Cоnsequens.NameLP)
                    {
                        for (int k = 0; k < FKB.ListVar[j].terms.Count; ++k)
                        {
                            if (FKB.ListVar[j].terms[k].Name == FKB.ListOfRule[i].Cоnsequens.Name)
                            {
                                if ("ЯКЩО" + " " + temp + " ТО " + FKB.ListVar[j].ShortName + " " + FKB.ListVar[j].terms[k].ShortNameTerm == ListOfRulesBox.SelectedItem.Text)
                                {
                                    IfListToEditRule.Items.Clear();
                                    for (int n = 0; n < FKB.ListOfRule[i].Antecedents.Count; ++n)
                                    {
                                        IfListToEditRule.Visible = true;
                                        IfListToEditRule.Width = 327;
                                        MinusIfToEditRule.Visible = true;
                                        IfListToEditRule.Items.Add(FKB.ListOfRule[i].Antecedents[n].NameLP + " = " + FKB.ListOfRule[i].Antecedents[n].Name);
                                        for (int l = 0; l < TreeViewBNZ.Nodes.Count; ++l)
                                        {
                                            for (int m = 0; m < TreeViewBNZ.Nodes[l].ChildNodes.Count; ++m)
                                            {
                                                for (int z = 0; z < FKB.ListOfRule[i].Antecedents.Count; ++z)
                                                {
                                                    if (TreeViewBNZ.Nodes[l].Text == FKB.ListOfRule[i].Antecedents[z].NameLP && TreeViewBNZ.Nodes[l].ChildNodes[m].Text == FKB.ListOfRule[i].Antecedents[z].Name)
                                                    {
                                                        TreeViewBNZ.Nodes[l].Expand();
                                                        TreeViewBNZ.Nodes[l].ChildNodes[m].Checked = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    ThenLabelToEditRule.Text = FKB.ListOfRule[i].Cоnsequens.NameLP + " = " + FKB.ListOfRule[i].Cоnsequens.Name;
                                    break;
                                }
                            }
                        }
                    }
                }

            }
        }
        public void RefreshCommand(object sender, EventArgs e, FuzzyKnowledgeBase FKB)
        {

            Editor.metagraph = MetagraphGenerator.GenerateRandomMetagraph(FKB, FKB.ListVar.Count, FKB.ListOfRule.Count, 1000, 400, 0.03f
                        , 0.03f);
            Editor.RefreshMetagraph();
        }
        public static void RefreshCommand()
        {
            Editor.metagraph = MetagraphGenerator.GenerateRandomMetagraph(FKB, FKB.ListVar.Count, FKB.ListOfRule.Count, 1200, 800, 0.03f
                        , 0.03f);
            Editor.RefreshMetagraph(); //поправить координаты в функции изменения алгоритма из 1500 на норм!!!!
            t = new Thread(Editor.Visinit);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }
        [STAThread]
        public static string Getjsonstring()
        {
            if (FKB.ListOfRule.Count == 0)
            {
                return (null);
            }

            Editor.RunAlgorithmWithTask();
            MainWindowVM EditorCopy = Editor;
            int Vertices = Editor.metagraph.CountVertices() + Editor.metagraph.CountMetavertices();
            for (int i = 0; i < Vertices; ++i)
            {
                EditorCopy.metagraph.Vertices[i].DrawingShape = null;
                EditorCopy.metagraph.Vertices[i].shape = null;
                EditorCopy.metagraph.Vertices[i].PF = null;
            }
            int Edges = Editor.metagraph.Edges.Count;
            for (int i = 0; i < Edges; ++i)
            {
                EditorCopy.metagraph.Edges[i].DrawingShape = null;
                EditorCopy.metagraph.Edges[i].ArrowShape = null;
            }
            jsonstring = new JavaScriptSerializer().Serialize(Editor.metagraph);
            return (jsonstring);
        }
        protected void X_TextChanged(object sender, EventArgs e)
        {
            X.BorderColor = Color.Black;
            X.BorderWidth = 1;
            for (int i = 0; i < FKB.ListVar.Count(); ++i)
            {
                if (FKB.ListVar[i].Name == selectedLP_Tree)
                {
                    for (int j = 0; j < FKB.ListVar[i].terms.Count(); ++j)
                    {
                        if (FKB.ListVar[i].terms[j].Name == selectedTerm_Tree)
                        {
                            try
                            {
                                FKB.ListVar[i].terms[j].a = Convert.ToDouble(X.Text);
                            }
                            catch
                            {
                                X.BorderColor = Color.Red;
                                return;
                            }
                            TreeViewBNZ.SelectedNode.Parent.Select();
                            TreeViewBNZ_SelectedNodeChanged(sender, e);
                            UpdatePanel1.Update();
                            break;
                        }
                    }
                    break;
                }
            }
        }

        protected void Y_TextChanged(object sender, EventArgs e)
        {
            Y.BorderColor = Color.Black;
            Y.BorderWidth = 1;
            for (int i = 0; i < FKB.ListVar.Count(); ++i)
            {
                if (FKB.ListVar[i].Name == selectedLP_Tree)
                {
                    for (int j = 0; j < FKB.ListVar[i].terms.Count(); ++j)
                    {
                        if (FKB.ListVar[i].terms[j].Name == TreeViewBNZ.SelectedNode.Text)
                        {
                            try
                            {
                                FKB.ListVar[i].terms[j].b = Convert.ToDouble(Y.Text);
                            }
                            catch
                            {
                                Y.BorderColor = Color.Red;
                                return;
                            }
                            TreeViewBNZ.SelectedNode.Parent.Select();
                            TreeViewBNZ_SelectedNodeChanged(sender, e);
                            UpdatePanel1.Update();
                            break;
                        }
                    }
                    break;
                }
            }
        }

        protected void Z_TextChanged(object sender, EventArgs e)
        {
            Z.BorderColor = Color.Black;
            Z.BorderWidth = 1;
            for (int i = 0; i < FKB.ListVar.Count(); ++i)
            {
                if (FKB.ListVar[i].Name == selectedLP_Tree)
                {
                    for (int j = 0; j < FKB.ListVar[i].terms.Count(); ++j)
                    {
                        if (FKB.ListVar[i].terms[j].Name == TreeViewBNZ.SelectedNode.Text)
                        {
                            try
                            {
                                FKB.ListVar[i].terms[j].c = Convert.ToDouble(Z.Text);
                            }
                            catch
                            {
                                Z.BorderColor = Color.Red;
                                return;
                            }
                            TreeViewBNZ.SelectedNode.Parent.Select();
                            TreeViewBNZ_SelectedNodeChanged(sender, e);
                            UpdatePanel1.Update();
                            break;
                        }
                    }
                }
                break;
            }
        }
        protected void MinusIfToEditRule_Click(object sender, EventArgs e)
        {
            
            for (int i = 0; i < TreeViewBNZ.Nodes.Count; ++i)
            {
                if (IfListToEditRule.Items.Count == 0)
                {
                    return;
                }
                if (IfListToEditRule.SelectedItem == null)
                {
                    return;
                }
                if (IfListToEditRule.SelectedItem.Text.StartsWith(TreeViewBNZ.Nodes[i].Text))
                {
                    TreeViewBNZ.CheckedNodes.Remove(TreeViewBNZ.Nodes[i]);
                    for (int j = 0; j < TreeViewBNZ.CheckedNodes.Count; ++j)
                    {
                        if (TreeViewBNZ.CheckedNodes[j].Parent.Text == TreeViewBNZ.Nodes[i].Text)
                        {
                            TreeViewBNZ.CheckedNodes[j].Checked = false;
                        }
                    }
                }
            }
            TreePan.Update();
            IfListToEditRule.Items.Remove(IfListToEditRule.SelectedItem);
            UpdatePanel1.Update();
        }

        protected void SaveRule_Click(object sender, EventArgs e)
        {
            string if_text = "";
            for (int i = 0; i < IfListToEditRule.Items.Count; ++i)
            {
                if_text += IfListToEditRule.Items[i].Text;
            }
            IfListToEditRule.Items.Clear();
            IfListToEditRule.Visible = false;
            MinusIfToEditRule.Visible = false;
            if (edit)
            {
                ListOfRulesBox.SelectedItem.Text = "ЯКЩО " + if_text + " ТО " + ThenLabelToEditRule.Text + " ";
            }
            else
            {
                ListOfRulesBox.Items.Add("ЯКЩО " + if_text + " ТО " + ThenLabelToEditRule.Text + " ");
            }
            UpdateRules.Update();
            int kount = TreeViewBNZ.CheckedNodes.Count;
            for (int i = 0; i < kount; i++)
            {
                if (TreeViewBNZ.CheckedNodes[i].Checked == true)
                {
                    for (int j = 0; j < TreeViewBNZ.CheckedNodes[i].Parent.ChildNodes.Count; ++j)
                    {
                        TreeViewBNZ.CheckedNodes[i].Parent.ChildNodes[j].ShowCheckBox = false;
                    }
                    TreeViewBNZ.CheckedNodes[i].Parent.Collapse();
                    TreePan.Update();
                }
            }
            List<Term> cop = new List<Term>();
            for (int i = 0; i < ListTermIF.Count; i++)
            {
                cop.Add(ListTermIF[i]);
            }
            rul = new Rule(ID, termThen, cop);
            FKB.ListOfRule.Add(rul);
            ThenLabelToEditRule.Text = "";
            UpdatePanel1.Update();
            ListTermIF.Clear();
            for (int i = 0; i < TreeViewBNZ.Nodes.Count; ++i)
            {
                for (int j = 0; j < TreeViewBNZ.Nodes[i].ChildNodes.Count; ++j)
                {
                    TreeViewBNZ.Nodes[i].ChildNodes[j].ShowCheckBox = true;
                    TreeViewBNZ.Nodes[i].ChildNodes[j].Checked = false;
                }
            }
            edit = false;
            TreePan.Update();
        }
        protected void SaveBNZ_Click(object sender, EventArgs e)
        {
            Program.Save_BNZ(FKB, @"C:\Metagraph_docs\BNZ.txt");
        }
        protected void WithJsonToBNZ(object sender, EventArgs e)
        {
            //Program.WithJsonToBNZ();
        }
        protected void Ready_Click(object sender, EventArgs e)
        {
            List<string> ListLPToArgs = new List<string>();
            for (int i = 0; i < list_label.Count; ++i)
            {
                ListLPToArgs.Add(list_label[i].Text);
            }
            List<string> ListValuesOfLPToArgs = new List<string>();
            for (int i = 0; i < list_Text_Box.Count; ++i)
            {
                ListValuesOfLPToArgs.Add(list_Text_Box[i].Text);
            }
            ArgumentsForConclusion args = new ArgumentsForConclusion();
            args.ListNamesLinguisticVar = ListLPToArgs;
            args.ListValuesLinguisticVar = ListValuesOfLPToArgs;
            args.nameResultedLinguisticVar = ConcludeLP.Text;
            Label1.Text = args.nameResultedLinguisticVar + " " + FKB.MakeConclusion(ConcludeLP.Text, args, FKB);
        }

        protected void IfButton_Click(object sender, EventArgs e)
        {
            if (FKB.ListVar.Count == 0)
            {
                FileUploadOne.BorderColor = Color.Red;
                return;
            }
            if (TreeViewBNZ.CheckedNodes.Count == 0)
            {
                return;
            }
            IfListToEditRule.Visible = true;
            MinusIfToEditRule.Visible = true;
            TreeNodeCollection ColecIF = new TreeNodeCollection();
            ColecIF = TreeViewBNZ.CheckedNodes;
            ListTermIF.Clear();
            IfListToEditRule.Items.Clear();
            for (int i = 0; i < ColecIF.Count; i++)
            {
                IfListToEditRule.Items.Add(ColecIF[i].Parent.Text + " = " + ColecIF[i].Text);
            }
            int kount = TreeViewBNZ.CheckedNodes.Count;
            for (int i = 0; i < kount; i++)
            {
                if (TreeViewBNZ.CheckedNodes[i].Checked == true)
                {
                    for (int j = 0; j < TreeViewBNZ.CheckedNodes[i].Parent.ChildNodes.Count; ++j)
                    {
                        TreeViewBNZ.CheckedNodes[i].Parent.ChildNodes[j].ShowCheckBox = false;
                    }
                    TreeViewBNZ.CheckedNodes[i].Parent.Collapse();
                    TreePan.Update();
                }
            }
            for (int i = 0; i < TreeViewBNZ.Nodes.Count; ++i)
            {
                for (int j = 0; j < TreeViewBNZ.Nodes[i].ChildNodes.Count; ++j)
                {
                    TreeViewBNZ.Nodes[i].ChildNodes[j].Checked = false;
                }
            }
        }

        protected void ThenButton_Click(object sender, EventArgs e)
        {
            ThenLabelToEditRule.Text = "";
            if (FKB.ListVar.Count == 0)
            {
                FileUploadOne.BorderColor = Color.Red;
                return;
            }
            if (TreeViewBNZ.CheckedNodes.Count == 0)
            {
                return;
            }
            TreeNodeCollection ColecThen = new TreeNodeCollection();
            ColecThen = TreeViewBNZ.CheckedNodes;
            termThen = new Term(ID, ColecThen[0].Text, ColecThen[0].Parent.Text);
            string result = " " + ColecThen[0].Parent.Text + " " + ColecThen[0].Text;
            ThenLabelToEditRule.Text = result;
            UpdatePanel1.Update();
        }

        protected void AddTermFromInterface_Click(object sender, EventArgs e)
        {
            if (FKB.ListVar.Count == 0)
            {
                FileUploadOne.BorderColor = Color.Red;
                return;
            }
            int i = 0;
            Tree.Text = NameTerm.Text;
            TreeViewBNZ.SelectedNode.ChildNodes.Add(Tree);
            proverka = 1;
            for (i = 0; i < FKB.ListVar.Count; i++)
            {
                if (FKB.ListVar[i].Name == TreeViewBNZ.SelectedNode.Text)
                {
                    Term term = new Term(ID, NameTerm.Text, FKB.ListVar[i].Name);
                    term.a = FKB.ListVar[i].min;
                    term.b = FKB.ListVar[i].max - FKB.ListVar[i].min;
                    term.c = FKB.ListVar[i].max;
                    FKB.ListVar[i].terms.Add(term);
                    TreeViewBNZ.SelectedNode.ChildNodes[FKB.ListVar[i].terms.Count - 1].ShowCheckBox = true;
                    break;
                }
            }        
            
            NameTerm.Text = "";
            TreeViewBNZ.TreeNodeCheckChanged += Page_Load;
            TreePan.Update();
        }
    }
}
