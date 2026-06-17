using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CalculoRacksTrailerDesktop.V2.Models;

namespace CalculoRacksTrailerDesktop.V2.Utils
{
    public static class TrailerCalculator
    {
        #region Métodos públicos

        public static bool UnitFitsSingle(double largo, double ancho, double alto, double trailerLargo, double trailerAncho, double trailerAlto)
        {
            if (alto > trailerAlto) return false;
            // Acepta cualquiera de las dos orientaciones (normal o rotada 90°)
            return (largo <= trailerLargo && ancho <= trailerAncho)
                || (ancho <= trailerLargo && largo <= trailerAncho);
        }

        public static Dictionary<string, Group> CloneGroups(Dictionary<string, Group> original)
        {
            var clone = new Dictionary<string, Group>();

            foreach (var kv in original)
            {
                var group = new Group(kv.Value.Largo, kv.Value.Ancho);
                group.UnitHeights.AddRange(kv.Value.UnitHeights);
                group.Codes.AddRange(kv.Value.Codes.Distinct());
                clone[kv.Key] = group;
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

            // 2. Probar todas las combinaciones de rotación por grupo (2^n grupos)
            var groupKeys = groups.Keys.ToList();
            int numGroups = groupKeys.Count;
            int combinations = 1 << numGroups; // 2^n

            for (int mask = 0; mask < combinations; mask++)
            {
                // Construir lista de torres con rotación independiente por grupo
                var towersToUse = new List<Tower>();
                var rotatedGroups = new Dictionary<string, bool>();

                for (int i = 0; i < numGroups; i++)
                {
                    bool rotateThisGroup = (mask & (1 << i)) != 0;
                    rotatedGroups[groupKeys[i]] = rotateThisGroup;
                }

                towersToUse = CreateTowersWithRotation(groups, trailerAlto, rotatedGroups);

                string tempReason;
                bool ok;

                switch (strategy)
                {
                    case PlacementStrategy.GreedyByWidth:
                        ok = TryPlaceWithOrdering(towersToUse.OrderByDescending(t => t.Ancho).ThenByDescending(t => t.Largo).ToList(), trailerLargo, trailerAncho, out tempReason);
                        break;

                    case PlacementStrategy.GreedyByLength:
                        ok = TryPlaceWithOrdering(towersToUse.OrderByDescending(t => t.Largo).ThenByDescending(t => t.Ancho).ToList(), trailerLargo, trailerAncho, out tempReason);
                        break;

                    case PlacementStrategy.GreedyByArea:
                        ok = TryPlaceWithOrdering(towersToUse.OrderByDescending(t => t.Largo * t.Ancho).ToList(), trailerLargo, trailerAncho, out tempReason);
                        break;

                    case PlacementStrategy.BestFit:
                        ok = TryBestFitStrategy(towersToUse, trailerLargo, trailerAncho, out tempReason);
                        break;

                    default:
                        reason = "Estrategia no reconocida.";
                        return false;
                }

                if (ok)
                {
                    // Describir qué grupos se rotaron para info al usuario
                    var rotatedDesc = rotatedGroups
                        .Where(kv => kv.Value)
                        .Select(kv => kv.Key)
                        .ToList();
                    string rotInfo = rotatedDesc.Count > 0
                        ? $" (grupos rotados 90°: {string.Join(", ", rotatedDesc)})"
                        : string.Empty;
                    reason = (tempReason + rotInfo).Trim();
                    return true;
                }

                // Guardar el motivo de fallo de la primera combinación (sin rotaciones) como fallback
                if (mask == 0)
                    reason = tempReason;
            }

            return false;
        }

        #endregion Métodos públicos

        #region Métodos privados

        private static List<Tower> CreateTowers(Dictionary<string, Group> groups, double trailerAlto)
            => CreateTowersWithRotation(groups, trailerAlto, null);

        private static List<Tower> CreateTowersWithRotation(
            Dictionary<string, Group> groups,
            double trailerAlto,
            Dictionary<string, bool>? rotationMask)
        {
            var towers = new List<Tower>();

            foreach (var kv in groups)
            {
                var g = kv.Value;
                bool rotate = rotationMask != null && rotationMask.TryGetValue(kv.Key, out bool r) && r;
                double tLargo = rotate ? g.Ancho : g.Largo;
                double tAncho = rotate ? g.Largo : g.Ancho;

                var sortedHeights = g.UnitHeights.OrderByDescending(h => h).ToList();
                double currentHeight = 0;

                foreach (var h in sortedHeights)
                {
                    if (currentHeight + h > trailerAlto)
                    {
                        towers.Add(new Tower(tLargo, tAncho, currentHeight));
                        currentHeight = h;
                    }
                    else
                    {
                        currentHeight += h;
                    }
                }

                if (currentHeight > 0)
                    towers.Add(new Tower(tLargo, tAncho, currentHeight));
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

        #endregion Métodos privados
    }
}
