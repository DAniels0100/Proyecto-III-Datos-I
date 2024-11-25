using Proyecto_grafos.Models;
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
        private DispatcherTimer pausaTimer;
        public int Combustible { get; private set; }
        public List<ModuloAI> ModulosAI { get; private set; } // Propiedad para almacenar los módulos AI

        // Constructor
        public Avion(Graph.Node startNode, Graph graph)
        {
            Name = Guid.NewGuid().ToString(); // Genera un GUID como nombre único

            CurrentNode = startNode ?? throw new ArgumentNullException(nameof(startNode));
            this.graph = graph ?? throw new ArgumentNullException(nameof(graph));

            // Inicializar combustible con un valor aleatorio entre 60 y 100
            Random random = new Random();
            Combustible = random.Next(60, 101);

            // Inicializar el timer para el movimiento del avión
            movimientoTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(600) };
            movimientoTimer.Tick += MovimientoAvion;

            // Inicializar el timer para la pausa de 3 segundos
            pausaTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            pausaTimer.Tick += PausarAntesDeDespegar;

            // Inicializar los módulos AI
            ModulosAI = CrearModulosAI();
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

        private void RecargarCombustible()
        {
            Combustible = Math.Min(Combustible + 50, 100); // Recarga hasta un máximo de 100
        }

        //Modulos AI
        private List<ModuloAI> CrearModulosAI()
        {
            return new List<ModuloAI>
            {
                new ModuloAI("Pilot"),
                new ModuloAI("Copilot"),
                new ModuloAI("Manteinance"),
                new ModuloAI("Space Awarness")
            };
        }
    }
}
