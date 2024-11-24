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
            public List<Edge> Edges { get; } = new List<Edge>();
            public double X { get; } // Coordenada X del nodo
            public double Y { get; } // Coordenada Y del nodo

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

        // Agregar nodo al diccionario
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

        // Agregar una arista entre dos nodos con un peso
        public void AddEdge(string fromNode, string toNode, double weight, bool isDirected = false)
        {
            if (nodes.ContainsKey(fromNode) && nodes.ContainsKey(toNode))
            {
                Node from = nodes[fromNode];
                Node to = nodes[toNode];
                from.Edges.Add(new Edge(to, weight));

                if (!isDirected)
                {
                    to.Edges.Add(new Edge(from, weight));
                }
            }
            else
            {
                throw new ArgumentException("Both nodes must exist to add an edge");
            }
        }

        // Obtener y mostrar las conexiones de cada nodo
        public string GetGraphRepresentation()
        {
            var sb = new System.Text.StringBuilder();
            foreach (var nodePair in nodes)
            {
                Node node = nodePair.Value;
                sb.Append($"{node.Name}: ");
                foreach (Edge edge in node.Edges)
                {
                    sb.Append($"-> {edge.Destination.Name} (peso: {edge.Weight}) ");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        // Método para obtener todos los nombres de los nodos
        public IEnumerable<string> GetAllNodes()
        {
            return nodes.Keys;
        }

        // Método para obtener todos los objetos Node
        public IEnumerable<Node> GetAllNodeObjects()
        {
            return nodes.Values;
        }

        // Método para obtener un nodo por su nombre
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
