using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_grafos.Models
{
    public static class OrdenadorAviones
    {
        public static List<Avion> MergeSort(List<Avion> aviones)
        {
            if (aviones.Count <= 1)
                return aviones;

            int mid = aviones.Count / 2;
            var izquierda = MergeSort(aviones.GetRange(0, mid));
            var derecha = MergeSort(aviones.GetRange(mid, aviones.Count - mid));

            return Merge(izquierda, derecha);
        }

        private static List<Avion> Merge(List<Avion> izquierda, List<Avion> derecha)
        {
            var resultado = new List<Avion>();
            int i = 0, j = 0;

            while (i < izquierda.Count && j < derecha.Count)
            {
                if (string.Compare(izquierda[i].Name, derecha[j].Name) <= 0)
                {
                    resultado.Add(izquierda[i]);
                    i++;
                }
                else
                {
                    resultado.Add(derecha[j]);
                    j++;
                }
            }

            resultado.AddRange(izquierda.GetRange(i, izquierda.Count - i));
            resultado.AddRange(derecha.GetRange(j, derecha.Count - j));

            return resultado;
        }
    }
}
