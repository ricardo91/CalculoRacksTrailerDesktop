using System;
using System.Collections.Generic;
using System.Linq;

namespace CalculoRacksTrailerDesktop.V1
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

                // Los códigos solo deben guardarse una vez
                g.Codes.AddRange(kv.Value.Codes.Distinct());

                clone[kv.Key] = g;
            }

            return clone;
        }

        public static bool TryPlaceAllGroupsOptimized(Dictionary<string, Group> groups, double trailerLargo, double trailerAncho, double trailerAlto, out string reason)
        {
            reason = string.Empty;

            // 1. Convertir grupos en torres (apilando unidades hasta trailerAlto)
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

            // 2. Ordenar torres (grandes primero para mejor encaje)
            towers = towers
                .OrderByDescending(t => t.Ancho)
                .ThenByDescending(t => t.Largo)
                .ToList();

            // 3. Colocación por filas (de izquierda a derecha, usando el ancho del tráiler)
            double usedLength = 0;       // Largo total consumido (filas completas)
            double rowAccumAncho = 0;    // Ancho acumulado en la fila actual
            double rowMaxLargo = 0;      // Largo máximo de la fila actual

            foreach (var t in towers)
            {
                // ¿Cabe la torre en la fila actual (a lo ancho)?
                if (rowAccumAncho + t.Ancho <= trailerAncho)
                {
                    // Sí cabe: agregar a la fila actual
                    rowAccumAncho += t.Ancho;
                    rowMaxLargo = Math.Max(rowMaxLargo, t.Largo);
                }
                else
                {
                    // No cabe: cerrar la fila actual y empezar una nueva
                    usedLength += rowMaxLargo;

                    if (usedLength > trailerLargo)
                    {
                        reason = $"Supera el largo del tráiler ({usedLength:F0} > {trailerLargo:F0} mm).";
                        return false;
                    }

                    // Iniciar nueva fila
                    rowAccumAncho = t.Ancho;
                    rowMaxLargo = t.Largo;
                }
            }

            // 4. Añadir la última fila
            usedLength += rowMaxLargo;

            if (usedLength > trailerLargo)
            {
                reason = $"Supera el largo del tráiler ({usedLength:F0} > {trailerLargo:F0} mm).";
                return false;
            }

            return true;
        }
    }
}