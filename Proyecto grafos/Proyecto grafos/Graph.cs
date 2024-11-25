using System;
using System.Collections.Generic;
using System.Linq;

namespace Proyecto_grafos
{
    public class Graph
    {
        public class Node
        {
            public string Name { get; }
            public LinkedList<Edge> Edges { get; } = new LinkedList<Edge>();
            public double X { get; } // X coordinate of the node
            public double Y { get; } // Y coordinate of the node

            public Node(string name, double x, double y)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentException("Node name cannot be null or empty", nameof(name));
                }
                Name = name;
                X = x;
                Y = y;
            }
        }

        public class Edge
        {
            public Node Destination { get; }
            public double Weight { get; }

            public Edge(Node destination, double weight)
            {
                Destination = destination ?? throw new ArgumentNullException(nameof(destination));
                Weight = weight;
            }
        }

        private readonly Dictionary<string, Node> nodes = new Dictionary<string, Node>();

        // Add a node to the dictionary
        public void AddNode(string name, double x = 0, double y = 0)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Node name cannot be null or empty", nameof(name));
            }

            if (!nodes.ContainsKey(name))
            {
                nodes[name] = new Node(name, x, y);
            }
        }

        // Add an edge between two nodes with a weight
        public void AddEdge(string fromNode, string toNode, double weight, bool isDirected = false)
        {
            if (nodes.ContainsKey(fromNode) && nodes.ContainsKey(toNode))
            {
                Node from = nodes[fromNode];
                Node to = nodes[toNode];
                from.Edges.AddLast(new Edge(to, weight));

                if (!isDirected)
                {
                    to.Edges.AddLast(new Edge(from, weight));
                }
            }
            else
            {
                throw new ArgumentException("Both nodes must exist to add an edge");
            }
        }

        // Get and display the connections of each node
        public string GetGraphRepresentation()
        {
            var sb = new System.Text.StringBuilder();
            foreach (var nodePair in nodes)
            {
                Node node = nodePair.Value;
                sb.Append($"{node.Name}: ");
                foreach (Edge edge in node.Edges)
                {
                    sb.Append($"-> {edge.Destination.Name} (weight: {edge.Weight}) ");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        // Get all node names
        public IEnumerable<string> GetAllNodes()
        {
            return nodes.Keys;
        }

        // Get all Node objects
        public IEnumerable<Node> GetAllNodeObjects()
        {
            return nodes.Values;
        }

        // Get a node by its name
        public Node GetNodeByName(string name)
        {
            if (nodes.TryGetValue(name, out var node))
            {
                return node;
            }
            throw new KeyNotFoundException($"Node with name {name} does not exist");
        }
    }
}
