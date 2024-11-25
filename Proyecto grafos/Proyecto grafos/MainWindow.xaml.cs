using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Proyecto_grafos
{
    public partial class MainWindow : Window
    {
        // Variables de la batería antiaérea
        private double bateriaPosX = 375;
        private double direccion = 5;
        private DispatcherTimer movimientoTimer = new DispatcherTimer();

        // Variables del juego
        private DispatcherTimer gameTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
        private int tiempoRestante = 30; // tiempo cuenta regresiva
        private TextBlock timerTextBlock = new TextBlock(); // TextBlock que muestra el tiempo restante
        private DateTime tiempoInicio;
        private double velocidadBala;

        // Crear grafo y aviones
        private Graph grafo = new Graph();
        private List<Avion> aviones = new List<Avion>();
        private Dictionary<Avion, Image> avionVisuals = new Dictionary<Avion, Image>();

        //Lista de aviones derribados
        private List<Avion> avionesDerribados = new List<Avion>();

        public MainWindow()
        {
            if (tiempoRestante > 0)
            {
                InitializeComponent();
                GenerarMapa();
                GenerarRutas();
                IniciarMovimientoBateria();
                CrearAviones();
                IniciarMovimientoAviones();
            }

            // Mostrar el TextBlock del tiempo restante
            timerTextBlock.FontSize = 16;
            timerTextBlock.Foreground = Brushes.Red;
            timerTextBlock.Margin = new Thickness(10);
            MapaCanvas.Children.Add(timerTextBlock);
            Canvas.SetLeft(timerTextBlock, 275);
            Canvas.SetTop(timerTextBlock, 10);

            // Timer del juego
            gameTimer.Interval = TimeSpan.FromSeconds(1);
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            // Control del click presionado
            this.MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;
            this.MouseLeftButtonUp += MainWindow_MouseLeftButtonUp;
        }

        // Método para actualizar el temporizador
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            tiempoRestante--;
            timerTextBlock.Text = $"Tiempo restante: {tiempoRestante}s";

            if (tiempoRestante <= 0)
            {
                gameTimer.Stop();
                movimientoTimer.Stop();
                foreach (var avionTimer in avionVisuals.Values)
                {
                    avionTimer.Tag = null; // detener movimiento
                }
                timerTextBlock.Text = $"El tiempo se acabó";
            }
        }

        // Métodos para manejar el disparo de la batería antiaérea
        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IniciarDisparo();
        }

        private void MainWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FinalizarDisparo();
        }

        private void IniciarDisparo()
        {
            tiempoInicio = DateTime.Now;
        }

        private void FinalizarDisparo()
        {
            TimeSpan duracionPresionado = DateTime.Now - tiempoInicio;
            velocidadBala = Math.Max(5, duracionPresionado.TotalMilliseconds / 10);
            DispararBala();
        }

        // Método para disparar la bala
        private void DispararBala()
        {
            Rectangle bala = new Rectangle
            {
                Width = 5,
                Height = 15,
                Fill = Brushes.Black
            };

            double posX = Canvas.GetLeft(BateriaAntiaerea) + (BateriaAntiaerea.Width / 2) - (bala.Width / 2);
            double posY = Canvas.GetTop(BateriaAntiaerea) - bala.Height;
            Canvas.SetLeft(bala, posX);
            Canvas.SetTop(bala, posY);
            MapaCanvas.Children.Add(bala);

            DispatcherTimer balaTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(20)
            };

            balaTimer.Tick += (s, args) =>
            {
                posY -= velocidadBala;
                if (posY < 0)
                {
                    balaTimer.Stop();
                    MapaCanvas.Children.Remove(bala);
                }
                else
                {
                    Canvas.SetTop(bala, posY);
                    CheckCollisions(bala, posX, posY, balaTimer);
                }
            };

            balaTimer.Start();
        }

        // Método para verificar las colisiones
        private void CheckCollisions(Rectangle bala, double balaX, double balaY, DispatcherTimer balaTimer)
        {
            var balaRect = new Rect(balaX, balaY, bala.Width, bala.Height);

            foreach (var avionPair in avionVisuals.ToList())
            {
                var avion = avionPair.Key;
                var avionVisual = avionPair.Value;

                double avionX = Canvas.GetLeft(avionVisual);
                double avionY = Canvas.GetTop(avionVisual);
                var avionRect = new Rect(avionX, avionY, avionVisual.Width, avionVisual.Height);

                if (balaRect.IntersectsWith(avionRect))
                {
                    // Si hay colisión, manejarla
                    HandleCollision(avion, avionVisual, bala, balaTimer);
                    break;
                }
            }
        }

        // Método para manejar la colisión entre la bala y el avión
        private void HandleCollision(Avion avion, Image avionVisual, Rectangle bala, DispatcherTimer balaTimer)
        {
            // Eliminar la bala
            balaTimer.Stop();
            MapaCanvas.Children.Remove(bala);

            // Eliminar el avión
            MapaCanvas.Children.Remove(avionVisual);
            aviones.Remove(avion);
            avionVisuals.Remove(avion);

            // Mostrar información del avión destruido en la pantalla
            MostrarInformacionAvion(avion);

            // Mostrar efecto de explosión
            ShowExplosion(Canvas.GetLeft(avionVisual), Canvas.GetTop(avionVisual));
        }

        // Método para mostrar la información del avión destruido
        private void MostrarInformacionAvion(Avion avion)
        {
            TextBlock avionInfo = new TextBlock
            {
                Text = $"Avión destruido: {avion.Name}\nCombustible restante: {avion.Combustible}",
                FontSize = 14,
                Foreground = Brushes.White,
                Background = Brushes.Black,
                Margin = new Thickness(10),
                TextWrapping = TextWrapping.Wrap
            };

            // Posicionar el texto en la esquina superior derecha
            Canvas.SetLeft(avionInfo, 10);
            Canvas.SetTop(avionInfo, 10);

            // Añadir al Canvas
            MapaCanvas.Children.Add(avionInfo);

            // Configurar un timer para eliminar el texto después de unos segundos
            DispatcherTimer infoTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };

            infoTimer.Tick += (s, e) =>
            {
                infoTimer.Stop();
                MapaCanvas.Children.Remove(avionInfo);
            };

            infoTimer.Start();
        }

        // Mostrar la explosión al destruir un avión
        private void ShowExplosion(double x, double y)
        {
            Ellipse explosion = new Ellipse
            {
                Width = 40,
                Height = 40,
                Fill = new RadialGradientBrush
                {
                    GradientStops = new GradientStopCollection
                    {
                        new GradientStop(Colors.Yellow, 0),
                        new GradientStop(Colors.Red, 0.5),
                        new GradientStop(Colors.Orange, 1)
                    }
                }
            };

            Canvas.SetLeft(explosion, x);
            Canvas.SetTop(explosion, y);
            MapaCanvas.Children.Add(explosion);

            DispatcherTimer explosionTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };

            explosionTimer.Tick += (s, e) =>
            {
                explosionTimer.Stop();
                MapaCanvas.Children.Remove(explosion);
            };

            explosionTimer.Start();
        }

        private void GenerarMapa()
        {
            Random random = new Random();
            for (int i = 0; i < 5; i++)
            {
                double x = random.Next(50, 700);
                double y = random.Next(50, 500);

                Image aeropuerto = new Image()
                {
                    Width = 75,
                    Height = 75,
                    Source = new BitmapImage(new Uri("pack://application:,,,/Assets/aeropuerto.png"))
                };

                Canvas.SetLeft(aeropuerto, x);
                Canvas.SetTop(aeropuerto, y);
                MapaCanvas.Children.Add(aeropuerto);

                grafo.AddNode($"Aeropuerto_{i}", x + (aeropuerto.Width / 2), y + (aeropuerto.Height / 2));
            }

            for (int i = 0; i < 3; i++)
            {
                double x = random.Next(50, 700);
                double y = random.Next(50, 500);

                Image portaavion = new Image()
                {
                    Width = 75,
                    Height = 75,
                    Source = new BitmapImage(new Uri("pack://application:,,,/Assets/portaavion.png"))
                };

                Canvas.SetLeft(portaavion, x);
                Canvas.SetTop(portaavion, y);
                MapaCanvas.Children.Add(portaavion);

                grafo.AddNode($"Portaavion_{i}", x + (portaavion.Width / 2), y + (portaavion.Height / 2));
            }
        }

        private void GenerarRutas()
        {
            Random random = new Random();
            var nodos = new List<Graph.Node>(grafo.GetAllNodeObjects());

            for (int i = 0; i < nodos.Count; i++)
            {
                for (int j = i + 1; j < nodos.Count; j++)
                {
                    if (random.NextDouble() > 0.5)
                    {
                        double peso = random.Next(10, 100);
                        grafo.AddEdge(nodos[i].Name, nodos[j].Name, peso);

                        // Obtener las posiciones de los nodos
                        var nodoA = nodos[i];
                        var nodoB = nodos[j];

                        // Crear una línea entre los nodos
                        Line ruta = new Line
                        {
                            X1 = nodoA.X, 
                            Y1 = nodoA.Y, 
                            X2 = nodoB.X, 
                            Y2 = nodoB.Y, 
                            Stroke = Brushes.Black, 
                            StrokeThickness = 2, 
                            StrokeDashArray = new DoubleCollection { 4, 2 }
                        };

                        // Añadir la línea al Canvas
                        MapaCanvas.Children.Add(ruta);
                    }
                }
            }
        }


        private void CrearAviones()
        {
            Random random = new Random();
            var aeropuertosYPortaviones = grafo.GetAllNodeObjects()
                .Where(n => n.Name.Contains("Aeropuerto") || n.Name.Contains("Portaavion"))
                .ToList();  // Filtra solo los aeropuertos y portaaviones

            // Crear los tres aviones iniciales
            for (int i = 0; i < 0; i++)
            {
                CrearAvion(random, aeropuertosYPortaviones);
            }

            // Configurar un timer para generar aviones cada 3 segundos
            DispatcherTimer avionTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };

            avionTimer.Tick += (s, e) =>
            {
                CrearAvion(random, aeropuertosYPortaviones);
            };

            avionTimer.Start();
        }

        // Método auxiliar para crear un avión y añadirlo al juego
        private void CrearAvion(Random random, List<Graph.Node> aeropuertosYPortaviones)
        {
            var startNode = aeropuertosYPortaviones[random.Next(aeropuertosYPortaviones.Count)];
            var avion = new Avion(startNode, grafo);
            avion.SetRandomDestination();
            aviones.Add(avion);

            // Crear la imagen del avión
            Image avionVisual = new Image
            {
                Width = 60,
                Height = 60,
                Source = new BitmapImage(new Uri("pack://application:,,,/Assets/avion.png"))
            };

            // Posicionar la imagen del avión
            Canvas.SetLeft(avionVisual, startNode.X - avionVisual.Width / 2);
            Canvas.SetTop(avionVisual, startNode.Y - avionVisual.Height / 2);

            // Añadir al Canvas y al diccionario de visuales
            MapaCanvas.Children.Add(avionVisual);
            avionVisuals[avion] = avionVisual;

            // Iniciar el movimiento del avión
            IniciarMovimientoAvion(avion, avionVisual);
        }

        // Método para iniciar el movimiento individual de un avión
        private void IniciarMovimientoAvion(Avion avion, Image avionVisual)
        {
            DispatcherTimer avionTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
            avionTimer.Tick += (s, e) =>
            {
                if (avion.DestinationNode != null)
                {
                    double destinoX = avion.DestinationNode.X;
                    double destinoY = avion.DestinationNode.Y;

                    double currentX = Canvas.GetLeft(avionVisual);
                    double currentY = Canvas.GetTop(avionVisual);

                    double deltaX = destinoX - currentX;
                    double deltaY = destinoY - currentY;
                    double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

                    if (distance < 5)
                    {
                        Canvas.SetLeft(avionVisual, destinoX - avionVisual.Width / 2);
                        Canvas.SetTop(avionVisual, destinoY - avionVisual.Height / 2);
                        avion.MoveToDestination(); // Avanzar al siguiente destino
                    }
                    else
                    {
                        double moveX = deltaX / distance * 2; // Velocidad de movimiento
                        double moveY = deltaY / distance * 2;
                        Canvas.SetLeft(avionVisual, currentX + moveX);
                        Canvas.SetTop(avionVisual, currentY + moveY);
                    }
                }
            };
            avionTimer.Start();
        }


        private void IniciarMovimientoAviones()
        {
            foreach (var avion in aviones)
            {
                avion.StartMoving(); // Inicia el movimiento del avión
                var avionVisual = avionVisuals[avion];

                DispatcherTimer avionTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
                avionTimer.Tick += (s, e) =>
                {
                    if (avion.DestinationNode != null)
                    {
                        double destinoX = avion.DestinationNode.X;
                        double destinoY = avion.DestinationNode.Y;

                        double currentX = Canvas.GetLeft(avionVisual);
                        double currentY = Canvas.GetTop(avionVisual);

                        double deltaX = destinoX - currentX;
                        double deltaY = destinoY - currentY;
                        double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

                        if (distance < 5)
                        {
                            // Cuando llega a su destino
                            Canvas.SetLeft(avionVisual, destinoX - avionVisual.Width / 2);
                            Canvas.SetTop(avionVisual, destinoY - avionVisual.Height / 2);
                            avion.MoveToDestination(); // Avanzar al siguiente destino
                            avion.SetRandomDestination(); // Elegir un nuevo destino aleatorio
                        }
                        else
                        {
                            // Mover el avión hacia el destino
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
            movimientoTimer.Interval = TimeSpan.FromMilliseconds(20);
            movimientoTimer.Tick += MovimientoBateriaAntiaerea;
            movimientoTimer.Start();
        }

        private void MovimientoBateriaAntiaerea(object sender, EventArgs e)
        {
            bateriaPosX += direccion;

            if (bateriaPosX <= 0 || bateriaPosX >= MapaCanvas.ActualWidth - BateriaAntiaerea.RenderSize.Width)
            {
                direccion *= -1;
            }

            Canvas.SetLeft(BateriaAntiaerea, bateriaPosX);
        }
    }
}
