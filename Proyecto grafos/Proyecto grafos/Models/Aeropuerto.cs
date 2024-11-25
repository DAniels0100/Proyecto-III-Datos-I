using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Proyecto_grafos.Models
{
    internal class Aeropuerto
    {
        int avionesHangar;
        bool esTerrestre;
        string identificador;
        public int CombustibleDisponible;

        private Graph grafo = new Graph();
        private List<Avion> aviones = new List<Avion>();

        public Aeropuerto(int avionesHangar, string identificador, bool esTerrestre)
        {
            this.avionesHangar = avionesHangar;
            this.esTerrestre = true;
            this.identificador = identificador;
        }

        //crear
        private void CrearAvion(Random random, List<Graph.Node> aeropuertosYPortaviones)
        {
            if (MaxCantidadAviones()) 
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
                    Source = new BitmapImage(new Uri("C:\\Users\\sealc\\OneDrive\\Escritorio\\ProyectoDatos\\Proyecto-III-Datos-I\\Proyecto grafos\\Proyecto grafos\\Assets\\avion.png"))
                };

                // Posicionar la imagen del avión
                Canvas.SetLeft(avionVisual, startNode.X - avionVisual.Width / 2);
                Canvas.SetTop(avionVisual, startNode.Y - avionVisual.Height / 2);
            }
        }

        private bool MaxCantidadAviones()
        {
            int cantidadAviones = aviones.Count;
            if (cantidadAviones <= this.avionesHangar)
            {
                return true;
            }
            return false;
            
        }

        public int SuministrarCombustible(int cantidadSolicitada)
        {
            int combustibleEntregado = Math.Min(cantidadSolicitada, CombustibleDisponible);
            CombustibleDisponible -= combustibleEntregado;
            return combustibleEntregado;
        }
    }
}
