using System;
using System.Collections.Generic;
using System.Text;

namespace CalculoRacksTrailerDesktop.V2.Utils
{
    public enum PlacementStrategy
    {
        GreedyByWidth,      // Ordenar por ancho (más anchas primero)
        GreedyByLength,     // Ordenar por largo (más largas primero)
        GreedyByArea,       // Ordenar por área (más grandes primero)
        BestFit             // Probar múltiples estrategias y elegir la mejor
    }
}
