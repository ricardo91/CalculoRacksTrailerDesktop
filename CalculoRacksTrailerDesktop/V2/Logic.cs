using System;
using System.Collections.Generic;
using System.Text;

namespace CalculoRacksTrailerDesktop.V2
{
    public class Group
    {
        public double Largo { get; }
        public double Ancho { get; }

        public List<double> UnitHeights { get; } = new List<double>();
        public List<string> Codes { get; } = new List<string>();

        public Group(double largo, double ancho)
        {
            Largo = largo;
            Ancho = ancho;
        }
    }

    public class Tower
    {
        public double Largo { get; }
        public double Ancho { get; }
        public double Alto { get; }
        public Tower(double largo, double ancho, double alto)
        {
            Largo = largo;
            Ancho = ancho;
            Alto = alto;
        }
    }

    public enum PlacementStrategy
    {
        GreedyByWidth,      // Ordenar por ancho (más anchas primero)
        GreedyByLength,     // Ordenar por largo (más largas primero)
        GreedyByArea,       // Ordenar por área (más grandes primero)
        BestFit             // Probar múltiples estrategias y elegir la mejor
    }

    public static class TrailerCalculator
    {
        public static bool UnitFitsSingle(double largo, double ancho, double alto, double trailerLargo, double trailerAncho, double trailerAlto)
            => largo <= trailerLargo && ancho <= trailerAncho && alto <= trailerAlto;

        public static Dictionary<string, Group> CloneGroups(Dictionary<string, Group> original)
        {
            var clone = new Dictionary<string, Group>();

            foreach (var kv in original)
            {
                var g = new Group(kv.Value.Largo, kv.Value.Ancho);
                g.UnitHeights.AddRange(kv.Value.UnitHeights);
                g.Codes.AddRange(kv.Value.Codes.Distinct());
                clone[kv.Key] = g;
            }

            return clone;
        }

        public static bool TryPlaceAllGroupsOptimized(
            Dictionary<string, Group> groups,
            double trailerLargo,
            double trailerAncho,
            double trailerAlto,
            out string reason,
            PlacementStrategy strategy = PlacementStrategy.GreedyByWidth)
        {
            reason = string.Empty;

            // 1. Convertir grupos en torres
            var towers = CreateTowers(groups, trailerAlto);

            if (towers.Count == 0)
            {
                return true; // No hay nada que colocar
            }

            // 2. Aplicar estrategia seleccionada
            switch (strategy)
            {
                case PlacementStrategy.GreedyByWidth:
                    return TryPlaceWithOrdering(towers.OrderByDescending(t => t.Ancho).ThenByDescending(t => t.Largo).ToList(), trailerLargo, trailerAncho, out reason);

                case PlacementStrategy.GreedyByLength:
                    return TryPlaceWithOrdering(towers.OrderByDescending(t => t.Largo).ThenByDescending(t => t.Ancho).ToList(), trailerLargo, trailerAncho, out reason);

                case PlacementStrategy.GreedyByArea:
                    return TryPlaceWithOrdering(towers.OrderByDescending(t => t.Largo * t.Ancho).ToList(), trailerLargo, trailerAncho, out reason);

                case PlacementStrategy.BestFit:
                    return TryBestFitStrategy(towers, trailerLargo, trailerAncho, out reason);

                default:
                    reason = "Estrategia no reconocida.";
                    return false;
            }
        }

        private static List<Tower> CreateTowers(Dictionary<string, Group> groups, double trailerAlto)
        {
            var towers = new List<Tower>();

            foreach (var g in groups.Values)
            {
                var sortedHeights = g.UnitHeights.OrderByDescending(h => h).ToList();
                double currentHeight = 0;

                foreach (var h in sortedHeights)
                {
                    if (currentHeight + h > trailerAlto)
                    {
                        towers.Add(new Tower(g.Largo, g.Ancho, currentHeight));
                        currentHeight = h;
                    }
                    else
                    {
                        currentHeight += h;
                    }
                }

                if (currentHeight > 0)
                    towers.Add(new Tower(g.Largo, g.Ancho, currentHeight));
            }

            return towers;
        }

        private static bool TryPlaceWithOrdering(
            List<Tower> orderedTowers,
            double trailerLargo,
            double trailerAncho,
            out string reason)
        {
            reason = string.Empty;

            double usedLength = 0;
            double rowAccumAncho = 0;
            double rowMaxLargo = 0;

            foreach (var t in orderedTowers)
            {
                if (rowAccumAncho + t.Ancho <= trailerAncho)
                {
                    rowAccumAncho += t.Ancho;
                    rowMaxLargo = Math.Max(rowMaxLargo, t.Largo);
                }
                else
                {
                    usedLength += rowMaxLargo;

                    if (usedLength > trailerLargo)
                    {
                        reason = $"Supera el largo del tráiler ({usedLength:F0} > {trailerLargo:F0} mm).";
                        return false;
                    }

                    rowAccumAncho = t.Ancho;
                    rowMaxLargo = t.Largo;
                }
            }

            usedLength += rowMaxLargo;

            if (usedLength > trailerLargo)
            {
                reason = $"Supera el largo del tráiler ({usedLength:F0} > {trailerLargo:F0} mm).";
                return false;
            }

            return true;
        }

        private static bool TryBestFitStrategy(
            List<Tower> towers,
            double trailerLargo,
            double trailerAncho,
            out string reason)
        {
            // Probar múltiples estrategias y elegir la mejor (la que usa menos largo)
            var strategies = new[]
            {
                ("Por Ancho", towers.OrderByDescending(t => t.Ancho).ThenByDescending(t => t.Largo).ToList()),
                ("Por Largo", towers.OrderByDescending(t => t.Largo).ThenByDescending(t => t.Ancho).ToList()),
                ("Por Área", towers.OrderByDescending(t => t.Largo * t.Ancho).ToList())
            };

            double bestUsedLength = double.MaxValue;
            string bestStrategy = string.Empty;
            string bestReason = string.Empty;

            foreach (var (strategyName, orderedTowers) in strategies)
            {
                if (TryPlaceWithOrdering(orderedTowers, trailerLargo, trailerAncho, out string tempReason))
                {
                    // Calcular cuánto largo se usó
                    double usedLength = CalculateUsedLength(orderedTowers, trailerAncho);

                    if (usedLength < bestUsedLength)
                    {
                        bestUsedLength = usedLength;
                        bestStrategy = strategyName;
                        bestReason = $"Mejor ajuste encontrado (estrategia: {strategyName}, largo usado: {usedLength:F0} mm).";
                    }
                }
            }

            if (bestUsedLength < double.MaxValue)
            {
                reason = bestReason;
                return true;
            }

            reason = "Ninguna estrategia encontró una solución válida.";
            return false;
        }

        private static double CalculateUsedLength(List<Tower> orderedTowers, double trailerAncho)
        {
            double usedLength = 0;
            double rowAccumAncho = 0;
            double rowMaxLargo = 0;

            foreach (var t in orderedTowers)
            {
                if (rowAccumAncho + t.Ancho <= trailerAncho)
                {
                    rowAccumAncho += t.Ancho;
                    rowMaxLargo = Math.Max(rowMaxLargo, t.Largo);
                }
                else
                {
                    usedLength += rowMaxLargo;
                    rowAccumAncho = t.Ancho;
                    rowMaxLargo = t.Largo;
                }
            }

            usedLength += rowMaxLargo;
            return usedLength;
        }
    }
}