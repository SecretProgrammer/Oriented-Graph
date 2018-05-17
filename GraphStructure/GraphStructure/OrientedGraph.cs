using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GraphStructure
{
    class OrientedGraph<T>
    {
        /// <summary>
        /// Матрица смежности
        /// </summary>
        bool[,] adjacencyMatrix;
        GraphNode<T>[] verticesInfo;

        public OrientedGraph(int verticesCount)
        {
            adjacencyMatrix = new bool[verticesCount, verticesCount];
            verticesInfo = new GraphNode<T>[verticesCount];
        }

        public int VerticesCount => adjacencyMatrix.GetLength(0);

        public GraphNode<T> this[int vertex]
        {
            get { return verticesInfo[vertex]; }
            set { verticesInfo[vertex] = value; }
        }

        // TODO
        // public GraphNode<T> this[string name]
        // throw new KeyNotFoundException

        public bool this[int vertexFrom, int vertexTo]
        {
            get { return adjacencyMatrix[vertexFrom, vertexTo]; }
            set { adjacencyMatrix[vertexFrom, vertexTo] = value; }
        }

        public bool DFS(T soughtValue)
        {
            bool[] hasVisited = new bool[VerticesCount];

            for ( int p = 0; p < VerticesCount; p++ )
            {
                if ( !hasVisited[p] )
                {
                    if ( DFS(p, hasVisited, soughtValue) )
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool DFS(int vertexIndex, bool[] hasVisited, T soughtValue)
        {
            hasVisited[vertexIndex] = true;
            Debug.WriteLine($"(DFS) Посещаем вершину {verticesInfo[vertexIndex].Name}");

            if ( verticesInfo[vertexIndex].Value.Equals(soughtValue) )
            {
                return true;
            }
            for ( int p = 0; p < VerticesCount; p++ )
            {
                if ( adjacencyMatrix[vertexIndex, p] && !hasVisited[p] )
                {
                    if ( DFS(p, hasVisited, soughtValue) )
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool BFS(T soughtValue)
        {
            bool[] hasVisited = new bool[VerticesCount];
            Queue<int> queue = new Queue<int>();
            for ( int p = 0; p < VerticesCount; p++ )
            {
                if ( hasVisited[p] )
                    continue;

                queue.Enqueue(p);

                while ( queue.Count != 0 )
                {
                    int index = queue.Dequeue();

                    hasVisited[index] = true;
                    Debug.WriteLine($"(BFS) Посещаем вершину {verticesInfo[index].Name}");

                    if ( verticesInfo[index].Value.Equals(soughtValue) )
                    {
                        return true;
                    }

                    for ( int t = 0; t < VerticesCount; t++ )
                    {
                        if ( adjacencyMatrix[index, t] && !hasVisited[t] && !queue.Contains(t) )
                        {
                            queue.Enqueue(t);
                        }
                    }
                }
            }
            return false;
        }

        #region Загрузка и сохранение

        const string GRAPHML_XMLNS = "http://graphml.graphdrawing.org/xmlns";

        public static OrientedGraph<T> Load(string path)
        {
            string extension = Path.GetExtension(path);
            if ( ".txt".Equals(extension, StringComparison.OrdinalIgnoreCase) )
            {
                return Load(path, GraphFileFormat.PlainTextAdjacencyMatrix);
            }
            else if ( ".xml".Equals(extension, StringComparison.OrdinalIgnoreCase) )
            {
                return Load(path, GraphFileFormat.GraphML);
            }
            throw new NotSupportedException($"Unsupported extension '{extension}'");
        }

        public static OrientedGraph<T> Load(string path, GraphFileFormat fileFormat)
        {
            using ( var fstream = new StreamReader(path) )
            {
                return Load(fstream, fileFormat);
            }
        }

        public static OrientedGraph<T> Load(TextReader reader, GraphFileFormat fileFormat)
        {
            switch ( fileFormat )
            {
                case GraphFileFormat.PlainTextAdjacencyMatrix:
                    return LoadFromPlainTextAdjacencyMatrix(reader);

                case GraphFileFormat.GraphML:
                    return LoadFromGraphML(reader);

                default:
                    throw new NotImplementedException($"Support for '{fileFormat}' has not been implemented yet");
            }
        }

        private static OrientedGraph<T> LoadFromPlainTextAdjacencyMatrix(TextReader reader)
        {
            string[] splittedLine = reader.ReadLine().Split(' ');
            var orgraph = new OrientedGraph<T>(splittedLine.Length);
            int lineIndex = 0;

            while ( lineIndex < orgraph.VerticesCount )
            {
                orgraph[lineIndex] = new GraphNode<T>("node" + lineIndex);
                for ( int p = 0; p < splittedLine.Length; p++ )
                {
                    if ( splittedLine[p] == "1" )
                    {
                        orgraph[lineIndex, p] = true;
                    }
                }
                splittedLine = reader.ReadLine()?.Split(' ');
                lineIndex++;
            }
            return orgraph;
        }

        private static OrientedGraph<T> LoadFromGraphML(TextReader reader)
        {
            XNamespace xmlns = GRAPHML_XMLNS;
            XDocument xdoc = XDocument.Load(reader);
            var xgraph = xdoc.Element(xmlns + "graphml").Element(xmlns + "graph");
            Dictionary<string, int> id2index = xgraph.Elements(xmlns + "node")
                .Select(x => x.Attribute("id").Value)
                .Select((id, index) => new KeyValuePair<string, int>(id, index))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            var orgraph = new OrientedGraph<T>(id2index.Count);

            foreach ( KeyValuePair<string, int> kvp in id2index )
            {
                orgraph[kvp.Value] = new GraphNode<T>(kvp.Key);
            }

            foreach ( XElement xedge in xgraph.Elements(xmlns + "edge") )
            {
                int source = id2index[xedge.Attribute("source").Value];
                int target = id2index[xedge.Attribute("target").Value];
                orgraph[source, target] = true;
            }

            return orgraph;
        }

        public void Save(string path, GraphFileFormat fileFormat)
        {
            using ( var writer = new StreamWriter(path) )
            {
                Save(writer, fileFormat);
            }
        }

        public void Save(TextWriter writer, GraphFileFormat fileFormat)
        {
            switch ( fileFormat )
            {
                case GraphFileFormat.GraphML:
                    SaveAsGraphML(writer);
                    break;

                case GraphFileFormat.PlainTextAdjacencyMatrix:
                    SaveAsPlainTextAdjacencyMatrix(writer);
                    break;

                default:
                    throw new NotImplementedException($"Support for '{fileFormat}' has not been implemented yet");
            }
        }

        private void SaveAsPlainTextAdjacencyMatrix(TextWriter writer)
        {
            int nodeCount = adjacencyMatrix.GetLength(0);
            char[] buffer = new string(' ', 2 * nodeCount - 1).ToCharArray();
            for ( int r = 0; r < nodeCount; r++ )
            {
                if ( r > 0 )
                    writer.WriteLine();
                for ( int c = 0, idx = 0; c < nodeCount; c++, idx += 2 )
                {
                    buffer[idx] = adjacencyMatrix[r, c] ? '1' : '0';
                }
                writer.Write(buffer);
            }
        }

        private void SaveAsGraphML(TextWriter writer)
        {
            XNamespace xmlns = GRAPHML_XMLNS;
            XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
            const string schemaLocation = "http://graphml.graphdrawing.org/xmlns/1.0/graphml.xsd";

            XElement xgraphml = new XElement(xmlns + "graphml",
                new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),
                new XAttribute(xsi + "schemaLocation", schemaLocation),
                new XElement(xmlns + "graph",
                    new XAttribute("id", "G"),
                    new XAttribute("edgedefault", "directed")
                )
            );
            XElement xgraph = xgraphml.Elements().First();

            // Узлы
            for ( int p = 0; p < adjacencyMatrix.GetLength(0); p++ )
            {
                xgraph.Add(new XElement(xmlns + "node",
                    new XAttribute("id", "n" + p)
                ));
            }

            // Ребра
            for ( int r = 0, e = 1; r < adjacencyMatrix.GetLength(0); r++ )
            {
                for ( int c = 0; c < adjacencyMatrix.GetLength(1); c++ )
                {
                    if ( adjacencyMatrix[r, c] )
                    {
                        xgraph.Add(new XElement(xmlns + "edge",
                            new XAttribute("id", "e" + e),
                            new XAttribute("source", "n" + r),
                            new XAttribute("target", "n" + c)
                        ));
                        e++;
                    }
                }
            }

            xgraphml.Save(writer);
        }

        #endregion
    }
}
