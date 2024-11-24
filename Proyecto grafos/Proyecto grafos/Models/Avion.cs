using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace Proyecto_grafos
{
    public class Avion
    {
        public string Name { get; }
        public Graph.Node CurrentNode { get; private set; }
        public Graph.Node? DestinationNode { get; private set; }
        private Graph graph;
        private DispatcherTimer movimientoTimer;

        public Avion(string name, Graph.Node startNode, Graph graph)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Avion name cannot be null or empty", nameof(name));
            }

            Name = name;
            CurrentNode = startNode ?? throw new ArgumentNullException(nameof(startNode));
            this.graph = graph ?? throw new ArgumentNullException(nameof(graph));

            // Inicializar el timer para el movimiento del avión
            movimientoTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            movimientoTimer.Tick += MovimientoAvion;
        }

        // Elegir un destino aleatorio para el avión
        public void SetRandomDestination()
        {
            var nodes = new List<Graph.Node>(graph.GetAllNodeObjects());
            if (nodes.Count == 0)
            {
                throw new InvalidOperationException("No nodes available in the graph");
            }

            Random random = new Random();
            Graph.Node destination;
            do
            {
                destination = nodes[random.Next(nodes.Count)];
            } while (destination == CurrentNode);

            DestinationNode = destination;
        }

        // Iniciar el movimiento del avión hacia el destino
        public void StartMoving()
        {
            if (DestinationNode == null)
            {
                throw new InvalidOperationException("Destination not set for the plane");
            }
            movimientoTimer.Start();
        }

        // Moverse hacia el nodo destino
        public void MoveToDestination()
        {
            if (DestinationNode != null)
            {
                Console.WriteLine($"{Name} is moving from {CurrentNode.Name} to {DestinationNode.Name}");
                CurrentNode = DestinationNode;
                DestinationNode = null; // Resetea el destino después de llegar
                movimientoTimer.Stop();
                SetRandomDestination(); // Elegir un nuevo destino
            }
        }

        // Método privado del timer para mover el avión
        private void MovimientoAvion(object? sender, EventArgs e)
        {
            MoveToDestination();
            StartMoving(); // Reiniciar el movimiento hacia el nuevo destino
        }
    }
}
