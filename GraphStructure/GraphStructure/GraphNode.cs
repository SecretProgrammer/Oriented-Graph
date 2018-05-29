using System;

namespace GraphStructure
{
    class GraphNode<T> : ICloneable
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

        public GraphNode<T> Clone()
        {
            return ( GraphNode<T> )MemberwiseClone();
        }

        object ICloneable.Clone()
        {
            return MemberwiseClone();
        }
    }
}
