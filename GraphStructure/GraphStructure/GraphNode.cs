namespace GraphStructure
{
    class GraphNode<T>
    {
        public string Name { get; }
        public T Value { get; set; }

        public GraphNode(string name)
        {
            Name = name;
        }

        public GraphNode(string name, T value)
        {
            Name = name;
            Value = value;
        }
    }
}
