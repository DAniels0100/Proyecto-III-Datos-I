using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_grafos.Models
{
    public static class OrdenadorModulosAI
    {
        public static void SelectionSort(List<ModuloAI> modulos, Func<ModuloAI, IComparable> criterio)
        {
            for (int i = 0; i < modulos.Count - 1; i++)
            {
                int minIndex = i;

                for (int j = i + 1; j < modulos.Count; j++)
                {
                    if (criterio(modulos[j]).CompareTo(criterio(modulos[minIndex])) < 0)
                    {
                        minIndex = j;
                    }
                }

                if (minIndex != i)
                {
                    var temp = modulos[i];
                    modulos[i] = modulos[minIndex];
                    modulos[minIndex] = temp;
                }
            }
        }
    }
}
