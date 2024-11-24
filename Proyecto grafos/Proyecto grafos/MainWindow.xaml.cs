using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Proyecto_grafos
{
    public partial class MainWindow : Window
    {
        private double bateriaPosX = 375; // Posición inicial
        private double direccion = 5; // Velocidad de movimiento
        private DispatcherTimer movimientoTimer = new DispatcherTimer();
        private Graph grafo = new Graph(); // Instancia del grafo
        private List<Avion> aviones = new List<Avion>();
        private Dictionary<Avion, Rectangle> avionVisuals = new Dictionary<Avion, Rectangle>();

        public MainWindow()
        {
            InitializeComponent();
            GenerarMapa();
            GenerarRutas();
            IniciarMovimientoBateria();
            CrearAviones();
            IniciarMovimientoAviones();

            // Añadir el evento de clic para disparar
            this.MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;
        }

        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DispararBala();
        }

        private void DispararBala()
        {
            // Crear una bala representada por un rectángulo
            Rectangle bala = new Rectangle
            {
                Width = 5,
                Height = 15,
                Fill = Brushes.Black
            };

            // Posicionar la bala en la parte superior de la batería antiaérea
            double posX = Canvas.GetLeft(BateriaAntiaerea) + (BateriaAntiaerea.Width / 2) - (bala.Width / 2);
            double posY = Canvas.GetTop(BateriaAntiaerea) - bala.Height;

            Canvas.SetLeft(bala, posX);
            Canvas.SetTop(bala, posY);
            MapaCanvas.Children.Add(bala);

            // Animar la bala para que suba por la pantalla
            DispatcherTimer balaTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(20)
            };

            balaTimer.Tick += (s, args) =>
            {
                posY -= 10; // Mover la bala hacia arriba

                if (posY < 0)
                {
                    // Eliminar la bala si sale de la pantalla
                    balaTimer.Stop();
                    MapaCanvas.Children.Remove(bala);
                }
                else
                {
                    Canvas.SetTop(bala, posY);
                }
            };

            balaTimer.Start();
        }

        private void GenerarMapa()
        {
            Random random = new Random();

            // Generar aeropuertos aleatoriamente
            for (int i = 0; i < 5; i++)
            {
                double x = random.Next(50, 700);
                double y = random.Next(50, 500);

                Ellipse aeropuerto = new Ellipse
                {
                    Width = 20,
                    Height = 20,
                    Fill = Brushes.Green
                };

                Canvas.SetLeft(aeropuerto, x);
                Canvas.SetTop(aeropuerto, y);
                MapaCanvas.Children.Add(aeropuerto);

                // Añadir el aeropuerto como nodo al grafo
                grafo.AddNode($"Aeropuerto_{i}");
            }

            // Generar portaaviones aleatoriamente
            for (int i = 0; i < 3; i++)
            {
                double x = random.Next(50, 700);
                double y = random.Next(50, 500);

                Rectangle portaavion = new Rectangle
                {
                    Width = 30,
                    Height = 15,
                    Fill = Brushes.Blue
                };

                Canvas.SetLeft(portaavion, x);
                Canvas.SetTop(portaavion, y);
                MapaCanvas.Children.Add(portaavion);

                // Añadir el portaaviones como nodo al grafo
                grafo.AddNode($"Portaavion_{i}");
            }
        }

        private void GenerarRutas()
        {
            Random random = new Random();
            var nodos = new List<Graph.Node>(grafo.GetAllNodeObjects());

            // Generar rutas aleatorias entre los nodos
            for (int i = 0; i < nodos.Count; i++)
            {
                for (int j = i + 1; j < nodos.Count; j++)
                {
                    if (random.NextDouble() > 0.5) // Probabilidad del 50% de crear una ruta
                    {
                        double peso = random.Next(10, 100); // Peso aleatorio entre 10 y 100
                        grafo.AddEdge(nodos[i].Name, nodos[j].Name, peso);
                    }
                }
            }
        }

        private void CrearAviones()
        {
            Random random = new Random();
            var nodes = grafo.GetAllNodeObjects().ToList();

            // Crear 3 aviones y asignarles un nodo inicial aleatorio
            for (int i = 0; i < 3; i++)
            {
                var startNode = nodes[random.Next(nodes.Count)];
                var avion = new Avion($"Avion_{i}", startNode, grafo);
                avion.SetRandomDestination();
                aviones.Add(avion);

                // Crear la representación visual del avión
                Rectangle avionVisual = new Rectangle
                {
                    Width = 10,
                    Height = 10,
                    Fill = Brushes.Red
                };

                Canvas.SetLeft(avionVisual, random.Next(50, 700));
                Canvas.SetTop(avionVisual, random.Next(50, 500));
                MapaCanvas.Children.Add(avionVisual);
                avionVisuals[avion] = avionVisual;
            }
        }

        private void IniciarMovimientoAviones()
        {
            foreach (var avion in aviones)
            {
                avion.StartMoving();
                var avionVisual = avionVisuals[avion];

                DispatcherTimer avionTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
                avionTimer.Tick += (s, e) =>
                {
                    if (avion.DestinationNode != null)
                    {
                        // Obtener la posición del nodo destino
                        double destinoX = avion.DestinationNode.X;
                        double destinoY = avion.DestinationNode.Y;

                        // Obtener la posición actual del avión
                        double currentX = Canvas.GetLeft(avionVisual);
                        double currentY = Canvas.GetTop(avionVisual);

                        // Calcular el movimiento hacia el destino
                        double deltaX = destinoX - currentX;
                        double deltaY = destinoY - currentY;
                        double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

                        if (distance < 5)
                        {
                            // Si el avión está cerca del destino, detener el movimiento
                            Canvas.SetLeft(avionVisual, destinoX);
                            Canvas.SetTop(avionVisual, destinoY);
                            avion.MoveToDestination();
                            avion.SetRandomDestination();
                        }
                        else
                        {
                            // Mover el avión gradualmente hacia el destino
                            double moveX = deltaX / distance * 5;
                            double moveY = deltaY / distance * 5;

                            Canvas.SetLeft(avionVisual, currentX + moveX);
                            Canvas.SetTop(avionVisual, currentY + moveY);
                        }
                    }
                };

                avionTimer.Start();
            }
        }

        private void IniciarMovimientoBateria()
        {
            movimientoTimer.Interval = TimeSpan.FromMilliseconds(50);
            movimientoTimer.Tick += MovimientoBateriaAntiaerea;
            movimientoTimer.Start();
        }

        private void MovimientoBateriaAntiaerea(object? sender, EventArgs e)
        {
            bateriaPosX += direccion;

            // Limitar el movimiento dentro de los bordes
            if (bateriaPosX <= 0 || bateriaPosX >= MapaCanvas.ActualWidth - BateriaAntiaerea.RenderSize.Width)
            {
                direccion *= -1; // Cambiar la dirección
            }

            Canvas.SetLeft(BateriaAntiaerea, bateriaPosX);
        }
    }
}
