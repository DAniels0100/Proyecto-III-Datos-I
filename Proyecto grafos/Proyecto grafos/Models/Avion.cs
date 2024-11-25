
using System;
using System.Collections.Generic;
using System.Linq;
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
        public int Combustible { get; private set; }

        public Avion(string name, Graph.Node startNode, Graph graph)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Avion name cannot be null or empty", nameof(name));
            }

            Name = name;
            CurrentNode = startNode ?? throw new ArgumentNullException(nameof(startNode));
            this.graph = graph ?? throw new ArgumentNullException(nameof(graph));
            Combustible = 100; // Combustible inicial

            // Inicializar el timer para el movimiento del avión
            movimientoTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            movimientoTimer.Tick += MovimientoAvion;
        }

        public void SetRandomDestination()
        {
            var nodes = graph.GetAllNodeObjects().Where(n => n != CurrentNode).ToList();
            if (nodes.Count == 0)
            {
                throw new InvalidOperationException("No hay nodos disponibles en el grafo para seleccionar un destino.");
            }

            Random random = new Random();
            DestinationNode = nodes[random.Next(nodes.Count)];
        }

        public void StartMoving()
        {
            if (DestinationNode == null)
            {
                throw new InvalidOperationException("El destino no está configurado.");
            }
            movimientoTimer.Start();
        }

        public void MoveToDestination()
        {
            if (DestinationNode == null)
            {
                throw new InvalidOperationException("El destino no está configurado.");
            }

            Console.WriteLine($"{Name} se está moviendo de {CurrentNode.Name} a {DestinationNode.Name}");
            CurrentNode = DestinationNode;
            DestinationNode = null; // Resetea el destino después de llegar
            movimientoTimer.Stop();
        }

        private void MovimientoAvion(object? sender, EventArgs e)
        {
            MoveToDestination();
            RecargarCombustible();
            SetRandomDestination();
            StartMoving();
        }

        private void RecargarCombustible()
        {
            Console.WriteLine($"{Name} está recargando combustible en {CurrentNode.Name}.");
            Combustible = Math.Min(Combustible + 50, 100); // Recarga hasta un máximo de 100
        }
    }
}
