using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebApplication1;

namespace WebApplication1
{
    public class WindowGenerateMetagraphVM
    {
        String sbVert;
        String sbEdge;
        EdgeMetagraph<int, int> graph;
        int maxCoordBoxWidth;
        int maxCoordBoxHeight;
        public Command Generate { get; set; }
        public String EdgesList
        {
            get { return sbEdge; }
            set { sbEdge = value; }
        }
        public String VerticesList
        {
            get { return sbVert; }
            set { sbVert = value; }
        }
        public WindowGenerateMetagraphVM(EdgeMetagraph<int, int> graph, int maxCoordBoxWidth, int maxCoordBoxHeight)
        {
            Generate = new Command(GenerateMetagraphFromText);
            this.graph = graph;
            this.maxCoordBoxHeight = maxCoordBoxHeight;
            this.maxCoordBoxWidth = maxCoordBoxWidth;
        }
        private void GenerateMetagraphFromText(object o)
        {
            graph = MetagraphGenerator.GenerateMetagraphFromText(sbVert + sbEdge, maxCoordBoxWidth, maxCoordBoxHeight);
        }
    }
}
