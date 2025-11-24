using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CalculoRacksTrailerDesktop.V2
{
    public partial class Form1 : Form
    {
        private double trailerLargo, trailerAncho, trailerAlto;
        private Dictionary<string, Group> groups = new Dictionary<string, Group>();
        private PlacementStrategy currentStrategy = PlacementStrategy.GreedyByWidth;

        public Form1()
        {
            InitializeComponent();
            InitializeStrategyComboBox();
        }

        private void InitializeStrategyComboBox()
        {
            cmbStrategy.Items.Clear();
            cmbStrategy.Items.Add("Greedy - Por Ancho (Rápido)");
            cmbStrategy.Items.Add("Greedy - Por Largo (Rápido)");
            cmbStrategy.Items.Add("Greedy - Por Área (Rápido)");
            cmbStrategy.Items.Add("Best Fit - Mejor Ajuste (Más lento)");
            cmbStrategy.SelectedIndex = 0;
        }

        private void cmbStrategy_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentStrategy = (PlacementStrategy)cmbStrategy.SelectedIndex;

            string description = GetStrategyDescription(currentStrategy);
            lblStrategyInfo.Text = description;
        }

        private string GetStrategyDescription(PlacementStrategy strategy)
        {
            switch (strategy)
            {
                case PlacementStrategy.GreedyByWidth:
                    return "Coloca primero las torres más anchas.\nÓptimo para aprovechar el ancho del tráiler.";

                case PlacementStrategy.GreedyByLength:
                    return "Coloca primero las torres más largas.\nÓptimo cuando el largo es limitante.";

                case PlacementStrategy.GreedyByArea:
                    return "Coloca primero las torres más grandes (área).\nBalance entre largo y ancho.";

                case PlacementStrategy.BestFit:
                    return "Prueba todas las estrategias y elige la mejor.\nMás lento pero garantiza mejor resultado.";

                default:
                    return "";
            }
        }

        private void btnSetTrailer_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(txtTrailerLargo.Text, out trailerLargo) ||
                !double.TryParse(txtTrailerAncho.Text, out trailerAncho) ||
                !double.TryParse(txtTrailerAlto.Text, out trailerAlto))
            {
                MessageBox.Show("Valores inválidos.");
                return;
            }

            rtbResultado.AppendText(
                $"Tráiler configurado: {trailerLargo}×{trailerAncho}×{trailerAlto}\n");
        }

        private void btnAgregarRack_Click(object sender, EventArgs e)
        {
            string codigo = txtCodigoRack.Text.Trim();

            if (!double.TryParse(txtRackLargo.Text, out double largo) ||
                !double.TryParse(txtRackAncho.Text, out double ancho) ||
                !double.TryParse(txtRackAlto.Text, out double alto) ||
                !int.TryParse(txtRackUnidades.Text, out int unidades))
            {
                MessageBox.Show("Valores inválidos.");
                return;
            }

            if (!TrailerCalculator.UnitFitsSingle(largo, ancho, alto,
                trailerLargo, trailerAncho, trailerAlto))
            {
                rtbResultado.AppendText($"ERROR → El rack {codigo} no cabe.\n");
                return;
            }

            var temp = TrailerCalculator.CloneGroups(groups);

            string key = $"{largo}x{ancho}";

            if (!temp.ContainsKey(key))
                temp[key] = new Group(largo, ancho);

            for (int i = 0; i < unidades; i++)
                temp[key].UnitHeights.Add(alto);

            if (!temp[key].Codes.Contains(codigo))
                temp[key].Codes.Add(codigo);

            // Usar la estrategia seleccionada
            bool ok = TrailerCalculator.TryPlaceAllGroupsOptimized(
                temp, trailerLargo, trailerAncho, trailerAlto, out string reason, currentStrategy);

            if (!ok)
            {
                rtbResultado.AppendText($"NO CABE → {reason}\n");
                return;
            }

            groups = temp;

            rtbResultado.AppendText($"OK → Se añadieron {unidades} unidades de '{codigo}: {largo}x{ancho}x{alto}'. {reason}\n");
        }

        private void btnMostrarResumen_Click(object sender, EventArgs e)
        {
            MostrarResumen();
        }

        private void MostrarResumen()
        {
            rtbResultado.AppendText("\n--- Resumen ---\n");
            rtbResultado.AppendText($"Estrategia: {GetStrategyName(currentStrategy)}\n\n");

            int i = 1;

            foreach (var kv in groups)
            {
                var g = kv.Value;
                rtbResultado.AppendText(
                    $"{i}. {g.Largo}×{g.Ancho} → {g.UnitHeights.Count} unidades, códigos: {string.Join(",", g.Codes)}\n");
                i++;
            }

            rtbResultado.AppendText("----------------\n");
        }

        private string GetStrategyName(PlacementStrategy strategy)
        {
            switch (strategy)
            {
                case PlacementStrategy.GreedyByWidth: return "Greedy - Por Ancho";
                case PlacementStrategy.GreedyByLength: return "Greedy - Por Largo";
                case PlacementStrategy.GreedyByArea: return "Greedy - Por Área";
                case PlacementStrategy.BestFit: return "Best Fit";
                default: return "Desconocida";
            }
        }

        private void btnMostrarDiagrama_Click(object sender, EventArgs e)
        {
            DibujarDiagrama();
        }

        private void DibujarDiagrama()
        {
            rtbResultado.AppendText("\n╔════════════════════════════════════════════════════════════╗\n");
            rtbResultado.AppendText("║            DIAGRAMA - VISTA SUPERIOR DEL TRÁILER           ║\n");
            rtbResultado.AppendText("╚════════════════════════════════════════════════════════════╝\n\n");

            // 1. Crear torres
            var towers = new List<(string code, double largo, double ancho, double alto)>();

            foreach (var g in groups.Values)
            {
                var sorted = g.UnitHeights.OrderByDescending(x => x).ToList();
                double current = 0;

                foreach (var h in sorted)
                {
                    if (current + h > trailerAlto)
                    {
                        towers.Add((string.Join(",", g.Codes), g.Largo, g.Ancho, current));
                        current = h;
                    }
                    else
                    {
                        current += h;
                    }
                }

                if (current > 0)
                    towers.Add((string.Join(",", g.Codes), g.Largo, g.Ancho, current));
            }

            if (towers.Count == 0)
            {
                rtbResultado.AppendText("No hay racks para mostrar.\n");
                return;
            }

            // 2. Ordenar según estrategia actual
            towers = towers.OrderByDescending(t => t.ancho).ThenByDescending(t => t.largo).ToList();

            // 3. Organizar en filas (de izquierda a derecha usando el ANCHO del tráiler)
            var rows = new List<List<(string code, double largo, double ancho, double alto)>>();
            var rowMaxLargos = new List<double>();

            foreach (var t in towers)
            {
                bool placed = false;

                for (int i = 0; i < rows.Count; i++)
                {
                    double currentRowAncho = rows[i].Sum(tower => tower.ancho);

                    if (currentRowAncho + t.ancho <= trailerAncho)
                    {
                        rows[i].Add(t);
                        rowMaxLargos[i] = Math.Max(rowMaxLargos[i], t.largo);
                        placed = true;
                        break;
                    }
                }

                if (!placed)
                {
                    rows.Add(new List<(string code, double largo, double ancho, double alto)>() { t });
                    rowMaxLargos.Add(t.largo);
                }
            }

            // 4. Calcular espacio usado
            double totalLargoUsado = rowMaxLargos.Sum();

            // 5. Dibujar información del tráiler
            rtbResultado.AppendText($"Dimensiones del tráiler:\n");
            rtbResultado.AppendText($"  • Largo (profundidad): {trailerLargo:F0}mm\n");
            rtbResultado.AppendText($"  • Ancho (lado a lado): {trailerAncho:F0}mm\n");
            rtbResultado.AppendText($"  • Alto: {trailerAlto:F0}mm\n\n");
            rtbResultado.AppendText($"Uso del espacio:\n");
            rtbResultado.AppendText($"  • Largo usado: {totalLargoUsado:F0}mm de {trailerLargo:F0}mm ({(totalLargoUsado / trailerLargo * 100):F1}%)\n");
            rtbResultado.AppendText($"  • Torres colocadas: {towers.Count}\n");
            rtbResultado.AppendText($"  • Filas creadas: {rows.Count}\n\n");

            // 6. Dibujar vista superior (mirando desde arriba)
            int diagramWidth = 68;

            rtbResultado.AppendText("           ┌" + new string('─', diagramWidth) + "┐\n");
            rtbResultado.AppendText("           │" + " FRENTE DEL TRÁILER ".PadLeft((diagramWidth + 20) / 2).PadRight(diagramWidth) + "│\n");
            rtbResultado.AppendText("        ↑  ├" + new string('─', diagramWidth) + "┤\n");

            int rowNumber = 1;

            foreach (var row in rows)
            {
                int rowIndex = rowNumber - 1;
                double rowLargo = rowMaxLargos[rowIndex];
                double rowAnchoTotal = row.Sum(t => t.ancho);

                rtbResultado.AppendText($"        │  │ Profundidad: {rowLargo:F0}mm | Ancho usado: {rowAnchoTotal:F0}/{trailerAncho:F0}mm (FILA {rowNumber})\n");
                rtbResultado.AppendText($" LARGO  │  ├" + new string('─', diagramWidth) + "┤\n");

                // Dibujar las torres de esta fila
                int numLines = 3;

                for (int line = 0; line < numLines; line++)
                {
                    rtbResultado.AppendText("        │  │");

                    foreach (var tower in row)
                    {
                        // El ancho de cada torre en caracteres (proporcional al ancho real)
                        int towerWidth = Math.Max(8, (int)Math.Round((tower.ancho / trailerAncho) * diagramWidth));

                        string content;
                        if (line == 0 || line == numLines - 1)
                        {
                            content = "+" + new string('─', towerWidth - 2) + "+";
                        }
                        else
                        {
                            // Mostrar: Ancho x Largo (Altura)
                            string info = $"{tower.ancho:F0}x{tower.largo:F0}({tower.alto:F0})";
                            if (info.Length > towerWidth - 2)
                            {
                                info = tower.code.Split(',')[0];
                                if (info.Length > towerWidth - 2)
                                    info = info.Substring(0, towerWidth - 2);
                            }
                            content = "│" + info.PadLeft((towerWidth + info.Length - 2) / 2).PadRight(towerWidth - 2) + "│";
                        }

                        rtbResultado.AppendText(content);
                    }

                    // Espacio vacío en el ancho
                    int usedWidth = row.Sum(t => Math.Max(8, (int)Math.Round((t.ancho / trailerAncho) * diagramWidth)));
                    int remainingWidth = diagramWidth - usedWidth;
                    if (remainingWidth > 0)
                        rtbResultado.AppendText(new string('·', remainingWidth));

                    rtbResultado.AppendText("│\n");
                }

                rowNumber++;
            }

            // Mostrar espacio vacío
            double espacioVacio = trailerLargo - totalLargoUsado;
            if (espacioVacio > 10)
            {
                rtbResultado.AppendText("        │  ├" + new string('─', diagramWidth) + "┤\n");
                rtbResultado.AppendText($" {trailerLargo:F0}mm │  │ [ ESPACIO VACÍO: {espacioVacio:F0}mm de profundidad restante ]");
                rtbResultado.AppendText(new string(' ', Math.Max(0, diagramWidth - 52)) + "│\n");
            }

            rtbResultado.AppendText("        ↓  ├" + new string('─', diagramWidth) + "┤\n");
            rtbResultado.AppendText("           │" + " PARTE TRASERA ".PadLeft((diagramWidth + 15) / 2).PadRight(diagramWidth) + "│\n");
            rtbResultado.AppendText("           └" + new string('─', diagramWidth) + "┘\n");
            rtbResultado.AppendText("            ←" + new string('─', diagramWidth - 2) + "→\n");
            rtbResultado.AppendText($"             ANCHO DEL TRÁILER ({trailerAncho:F0}mm)\n\n");

            // 7. Leyenda detallada
            rtbResultado.AppendText("═══════════════════════════════════════════════════════════\n");
            rtbResultado.AppendText("LEYENDA:\n");
            rtbResultado.AppendText("  • Vista desde arriba del tráiler\n");
            rtbResultado.AppendText("  • Cada caja representa una torre de racks apilados\n");
            rtbResultado.AppendText("  • Formato dentro: ANCHO×LARGO(ALTURA) - todas en mm\n");
            rtbResultado.AppendText("  • Las torres se colocan DE IZQUIERDA A DERECHA\n");
            rtbResultado.AppendText("  • Profundidad de fila = LARGO máximo de sus torres\n");
            rtbResultado.AppendText("  • Nueva fila cuando el ancho acumulado excede " + trailerAncho + "mm\n");
            rtbResultado.AppendText("  • Los puntos (···) indican ancho sin utilizar\n");
            rtbResultado.AppendText("═══════════════════════════════════════════════════════════\n");
        }

        private void btnLimpiarRack_Click(object sender, EventArgs e)
        {
            txtCodigoRack.Clear();
            txtRackLargo.Clear();
            txtRackAncho.Clear();
            txtRackAlto.Clear();
            txtRackUnidades.Clear();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            txtTrailerLargo.Clear();
            txtTrailerAncho.Clear();
            txtTrailerAlto.Clear();

            txtCodigoRack.Clear();
            txtRackLargo.Clear();
            txtRackAncho.Clear();
            txtRackAlto.Clear();
            txtRackUnidades.Clear();

            groups = new Dictionary<string, Group>();
            trailerLargo = 0;
            trailerAncho = 0;
            trailerAlto = 0;

            rtbResultado.Clear();

            rtbResultado.AppendText("Nuevo proyecto iniciado.\n");
        }
    }
}