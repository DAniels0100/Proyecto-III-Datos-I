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
        private DispatcherTimer pausaTimer; // Temporizador para la pausa de 3 segundos

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
            movimientoTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(600) };
            movimientoTimer.Tick += MovimientoAvion;

            // Inicializar el timer para la pausa de 3 segundos
            pausaTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            pausaTimer.Tick += PausarAntesDeDespegar;
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
                CurrentNode = DestinationNode;
                DestinationNode = null; // Resetea el destino después de llegar
                movimientoTimer.Stop();

                // Iniciar la pausa de 3 segundos antes de elegir un nuevo destino
                pausaTimer.Start();
            }
        }

        // Método privado del timer para mover el avión
        private void MovimientoAvion(object? sender, EventArgs e)
        {
            MoveToDestination();
        }

        // Método privado para manejar la pausa de 3 segundos
        private void PausarAntesDeDespegar(object? sender, EventArgs e)
        {
            pausaTimer.Stop(); // Detener el temporizador de pausa
            SetRandomDestination(); // Elegir un nuevo destino
            StartMoving(); // Iniciar el movimiento hacia el nuevo destino
        }
    }
}
