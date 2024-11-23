using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Input;

namespace Proyecto_grafos
{
    public partial class MainWindow : Window
    {
        private double bateriaPosX = 375; // Posición inicial
        private double direccion = 5; // Velocidad de movimiento
        private DispatcherTimer movimientoTimer;

        public MainWindow()
        {
            InitializeComponent();
            GenerarMapa();
            IniciarMovimientoBateria();

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
            }
        }

        private void IniciarMovimientoBateria()
        {
            movimientoTimer = new DispatcherTimer {
                Interval = TimeSpan.FromMilliseconds(50)
            };
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
