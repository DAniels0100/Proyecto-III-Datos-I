using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_grafos
{
    public class Graph
    {
        
        public class Node
        {
            public string Name { get; }
            public List<Edge> Edges { get; } = new List<Edge>();

            public Node(string name)
            {
                Name = name;
            }
        }

        
        public class Edge
        {
            public Node Destination { get; }
            public double Weight { get; }  

            public Edge(Node destination, double weight)
            {
                Destination = destination;
                Weight = weight;
            }
        }

        private readonly Dictionary<string, Node> nodes = new Dictionary<string, Node>();

        // Agregar nodo al diccionario
        public void AddNode(string name)
        {
            if (!nodes.ContainsKey(name))
            {
                nodes[name] = new Node(name);
            }
        }

        // Agregar una arista entre dos nodos con un peso
        public void AddEdge(string fromNode, string toNode, double weight)
        {
            if (nodes.ContainsKey(fromNode) && nodes.ContainsKey(toNode))
            {
                Node from = nodes[fromNode];
                Node to = nodes[toNode];
                from.Edges.Add(new Edge(to, weight));
                to.Edges.Add(new Edge(from, weight));  // Si es un grafo no dirigido
            }
        }

        // Obtener y mostrar las conexiones de cada nodo
        public void PrintGraph()
        {
            foreach (var nodePair in nodes)
            {
                Node node = nodePair.Value;
                Console.Write($"{node.Name}: ");
                foreach (Edge edge in node.Edges)
                {
                    Console.Write($"-> {edge.Destination.Name} (peso: {edge.Weight}) ");
                }
                Console.WriteLine();
            }
        }
    }
}
