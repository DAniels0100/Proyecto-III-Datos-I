using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_grafos.Models
{
    public class ModuloAI
    {
        public string ID { get; private set; } // Identificador único de tres letras
        public string Rol { get; private set; } // Rol del módulo (Pilot, Copilot, etc.)
        public int HorasDeVuelo { get; set; } // Horas de vuelo asociadas al módulo

        private static Random random = new Random();

        public ModuloAI(string rol)
        {
            ID = GenerarID();
            Rol = rol;
            HorasDeVuelo = 0; // Inicializa las horas de vuelo en 0
        }

        private string GenerarID()
        {
            char letra1 = (char)random.Next('A', 'Z' + 1);
            char letra2 = (char)random.Next('A', 'Z' + 1);
            char letra3 = (char)random.Next('A', 'Z' + 1);
            return $"{letra1}{letra2}{letra3}";
        }
    }
}
