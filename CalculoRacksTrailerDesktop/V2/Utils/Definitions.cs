using System;
using System.Collections.Generic;
using System.Text;

namespace CalculoRacksTrailerDesktop.V2.Utils
{
    public enum PlacementStrategy
    {
        /// <summary>
        /// Ordenar por ancho (más anchas primero)
        /// </summary>
        GreedyByWidth,
        /// <summary>
        /// Ordenar por largo (más largas primero)
        /// </summary>
        GreedyByLength,
        /// <summary>
        /// Ordenar por área (más grandes primero)
        /// </summary>
        GreedyByArea,
        /// <summary>
        /// Probar múltiples estrategias y elegir la mejor
        /// </summary>
        BestFit
    }
}
