using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

            //OrientedGraph graph = OrientedGraph.Load("Kaliningrad.txt", GraphFileFormat.PlainTextAdjacencyMatrix);
            OrientedGraph<int> graph = OrientedGraph<int>.Load("graph01.txt");

            Random rnd = new Random();
            for ( int i = 0; i < graph.VerticesCount; i++ )
            {
                graph[i].Value = rnd.Next(-1000, 1001);
            }

            //int soughtValue = graph[rnd.Next(graph.VerticesCount)].Value;
            int soughtValue = graph[6].Value;

            MessageBox.Show(graph.DFS(soughtValue).ToString(), "DFS");
            MessageBox.Show(graph.BFS(soughtValue).ToString(), "BFS");
        }
    }
}
