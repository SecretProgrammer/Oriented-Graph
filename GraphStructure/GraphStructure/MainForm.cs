using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphStructure
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            ////OrientedGraph graph = OrientedGraph.Load("Kaliningrad.txt", GraphFileFormat.PlainTextAdjacencyMatrix);
            //OrientedGraph<int> graph = OrientedGraph<int>.Load("graph01.txt");

            //Random rnd = new Random();
            //for ( int i = 0; i < graph.VerticesCount; i++ )
            //{
            //    graph[i].Value = rnd.Next(-1000, 1001);
            //}

            ////int soughtValue = graph[rnd.Next(graph.VerticesCount)].Value;
            //int soughtValue = graph[6].Value;

            //MessageBox.Show(graph.DFS(soughtValue).ToString(), "DFS");
            //MessageBox.Show(graph.BFS(soughtValue).ToString(), "BFS");

            //var g1 = OrientedGraph<int>.Load("graph-for-union-01.txt");
            //var g2 = OrientedGraph<int>.Load("graph-for-union-02.txt");
            //var unionResult = g1.Union(g2, (int1, int2) => int1 + int2);
            //StringWriter writer = new StringWriter();
            //unionResult.Save(writer, GraphFileFormat.PlainTextAdjacencyMatrix);
            //string str = writer.ToString();

            //var g1 = OrientedGraph<int>.Load("graph-for-union-01.txt");
            //var temp = g1.DeleteVertex(2);
            //var writer = new StringWriter();
            //temp.Save(writer, GraphFileFormat.PlainTextAdjacencyMatrix);
            //var str = writer.ToString();

            var g1 = OrientedGraph<int>.Load("graph-for-union-01.txt");
            var g2 = OrientedGraph<int>.Load("graph-for-union-02.txt");
            //var intersectResult = g1.Intersect(g2, (int1, int2) => int1 + int2);
            var intersectResult = g1 & g2;
            StringWriter writer = new StringWriter();
            intersectResult.Save(writer, GraphFileFormat.PlainTextAdjacencyMatrix);
            string str = writer.ToString();

        }
    }
}
