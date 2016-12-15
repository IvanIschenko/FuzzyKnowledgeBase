using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using System.Web.Mvc;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Drawing;
using PagedList;
using System.Xml;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Model;

namespace WebApplication1
{
    public partial class ViewOfTerms : System.Web.UI.Page
    {

        static List<string> NameOfLinguisticVariables = new List<string>();

        protected void Page_Load(object sender, EventArgs e)
        {
            string pathToCheck = @"C:\Users\96and\Source\Repos\FuzzyKnowledgeBase\Metagraph_editor\MetagraphEnvironment\Saved.xml";
            XmlTextReader reader = new XmlTextReader(pathToCheck); 
            int CountNodesInXMLfile = 0; 
            List<string> NameOFtermsForOneLVFromFileXML = new List<string>();
            int countColumnData = 0;

            while (reader.Read() && CountNodesInXMLfile != 2) 
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
            for (int j = 0; j <= NameOfLinguisticVariables.Count; j++)
            {
                if (j != 0)
                {
                    CheckBoxList1.Items.Add(NameOfLinguisticVariables.ElementAt(j - 1));
                }      
           }
            
        }

    }
}
