
using System;

namespace Proyecto_grafos.Models
{
    public class Aeropuerto
    {
        public string Name { get; }
        public int CombustibleDisponible { get; private set; }

        public Aeropuerto(string name, int combustibleInicial)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("El nombre del aeropuerto no puede estar vacío.", nameof(name));
            }

            Name = name;
            CombustibleDisponible = combustibleInicial;
        }

        public int SuministrarCombustible(int cantidadSolicitada)
        {
            int combustibleEntregado = Math.Min(cantidadSolicitada, CombustibleDisponible);
            CombustibleDisponible -= combustibleEntregado;
            return combustibleEntregado;
        }
    }
}
