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
            rtbResultado.AppendText("\n--- DIAGRAMA ---\n");

            int consoleWidth = 60;
            int maxHeightLines = 12;

            var towers = new List<(string code, double length, double width, double height)>();

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

            towers = towers.OrderByDescending(t => t.width).ThenByDescending(t => t.length).ToList();

            var rows = new List<List<(string code, double length, double width, double height)>>();
            var rowWidths = new List<double>();

            foreach (var t in towers)
            {
                bool placed = false;

                for (int i = 0; i < rows.Count; i++)
                {
                    if (rowWidths[i] + t.width <= trailerAncho)
                    {
                        rows[i].Add(t);
                        rowWidths[i] += t.width;
                        placed = true;
                        break;
                    }
                }

                if (!placed)
                {
                    rows.Add(new List<(string code, double length, double width, double height)>() { t });
                    rowWidths.Add(t.width);
                }
            }

            foreach (var row in rows)
            {
                rtbResultado.AppendText("\nFila:\n");

                for (int level = maxHeightLines - 1; level >= 0; level--)
                {
                    foreach (var tower in row)
                    {
                        int towerChars = (int)Math.Round((tower.length / trailerLargo) * consoleWidth);
                        if (towerChars < 2) towerChars = 2;

                        int towerHeightLines =
                            (int)Math.Round((tower.height / trailerAlto) * maxHeightLines);
                        if (towerHeightLines < 1) towerHeightLines = 1;

                        string block =
                            (level < towerHeightLines)
                                ? new string('█', towerChars)
                                : new string(' ', towerChars);

                        rtbResultado.AppendText(block);
                    }

                    rtbResultado.AppendText("\n");
                }

                foreach (var tower in row)
                {
                    int towerChars = (int)Math.Round((tower.length / trailerLargo) * consoleWidth);
                    string label = $"{tower.code}({tower.height:0}mm)";
                    if (label.Length > towerChars) label = label.Substring(0, towerChars);
                    label = label.PadRight(towerChars);
                    rtbResultado.AppendText(label);
                }

                rtbResultado.AppendText("\n");
            }

            rtbResultado.AppendText("----------------\n");
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