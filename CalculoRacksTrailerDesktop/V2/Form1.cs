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

        // Catálogo de racks cargados desde CSV
        private Dictionary<string, RackData> rackCatalog = new Dictionary<string, RackData>();

        public Form1()
        {
            InitializeComponent();
            InitializeStrategyComboBox();
            CargarDatosIniciales();
        }

        private void CargarDatosIniciales()
        {
            // Cargar dimensiones fijas del tráiler
            trailerLargo = 13600;
            trailerAncho = 2500;
            trailerAlto = 2900;

            txtTrailerLargo.Text = trailerLargo.ToString();
            txtTrailerAncho.Text = trailerAncho.ToString();
            txtTrailerAlto.Text = trailerAlto.ToString();

            // Deshabilitar edición de dimensiones del tráiler (son fijas)
            txtTrailerLargo.ReadOnly = true;
            txtTrailerAncho.ReadOnly = true;
            txtTrailerAlto.ReadOnly = true;

            // También deshabilitar edición manual de las dimensiones del rack
            txtRackLargo.ReadOnly = true;
            txtRackAncho.ReadOnly = true;
            txtRackAlto.ReadOnly = true;

            rtbResultado.AppendText($"Tráiler configurado automáticamente: {trailerLargo}×{trailerAncho}×{trailerAlto}mm\n\n");

            // Cargar catálogo de racks desde CSV
            CargarCatalogoRacks();
        }

        private void CargarCatalogoRacks()
        {
            string csvPath = System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(Application.ExecutablePath),
                "racks_catalog.csv");

            if (!System.IO.File.Exists(csvPath))
            {
                rtbResultado.AppendText("⚠ Archivo 'racks_catalog.csv' no encontrado.\n");
                rtbResultado.AppendText("Se creará un archivo de ejemplo en la carpeta del programa.\n\n");
                CrearArchivoEjemplo(csvPath);
                return;
            }

            try
            {
                var lines = System.IO.File.ReadAllLines(csvPath);
                int loaded = 0;
                int errors = 0;

                // Saltar la primera línea si es encabezado
                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var parts = line.Split(',', ';');
                    if (parts.Length >= 4)
                    {
                        string codigo = parts[0].Trim();

                        if (double.TryParse(parts[1].Trim(), out double largo) &&
                            double.TryParse(parts[2].Trim(), out double ancho) &&
                            double.TryParse(parts[3].Trim(), out double alto))
                        {
                            string descripcion = parts.Length > 4 ? parts[4].Trim() : "";

                            rackCatalog[codigo.ToUpper()] = new RackData
                            {
                                Codigo = codigo,
                                Largo = largo,
                                Ancho = ancho,
                                Alto = alto,
                                Descripcion = descripcion
                            };
                            loaded++;
                        }
                        else
                        {
                            errors++;
                        }
                    }
                }

                rtbResultado.AppendText($"✓ Catálogo cargado: {loaded} racks disponibles");
                if (errors > 0)
                    rtbResultado.AppendText($" ({errors} líneas con errores ignoradas)");
                rtbResultado.AppendText("\n\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el catálogo: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CrearArchivoEjemplo(string path)
        {
            try
            {
                var ejemplo = new System.Text.StringBuilder();
                ejemplo.AppendLine("Codigo,Largo,Ancho,Alto,Descripcion");
                ejemplo.AppendLine("00518,1670,1200,900,Rack estándar A");
                ejemplo.AppendLine("04968,3700,2400,1200,Rack grande B");
                ejemplo.AppendLine("04971,1650,1200,920,Rack estándar C");
                ejemplo.AppendLine("04993,1650,1200,920,Rack estándar D");
                ejemplo.AppendLine("10077,1650,1200,920,Rack estándar E");

                System.IO.File.WriteAllText(path, ejemplo.ToString());

                rtbResultado.AppendText($"✓ Archivo de ejemplo creado: {path}\n");
                rtbResultado.AppendText("Edita el archivo y reinicia la aplicación para cargar tus racks.\n\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear archivo de ejemplo: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private void txtCodigoRack_TextChanged(object sender, EventArgs e)
        {
            string codigo = txtCodigoRack.Text.Trim().ToUpper();

            if (string.IsNullOrEmpty(codigo))
            {
                // Limpiar campos si el código está vacío
                txtRackLargo.Clear();
                txtRackAncho.Clear();
                txtRackAlto.Clear();
                txtRackLargo.BackColor = System.Drawing.SystemColors.Window;
                txtRackAncho.BackColor = System.Drawing.SystemColors.Window;
                txtRackAlto.BackColor = System.Drawing.SystemColors.Window;
                return;
            }

            // Buscar en el catálogo
            if (rackCatalog.ContainsKey(codigo))
            {
                var rackData = rackCatalog[codigo];

                // Autocompletar los campos
                txtRackLargo.Text = rackData.Largo.ToString();
                txtRackAncho.Text = rackData.Ancho.ToString();
                txtRackAlto.Text = rackData.Alto.ToString();

                // Fondo verde claro para indicar que se encontró
                txtRackLargo.BackColor = System.Drawing.Color.LightGreen;
                txtRackAncho.BackColor = System.Drawing.Color.LightGreen;
                txtRackAlto.BackColor = System.Drawing.Color.LightGreen;

                // Enfocar el campo de unidades para que el usuario pueda continuar
                if (!string.IsNullOrEmpty(txtCodigoRack.Text) && txtRackUnidades.Text.Length == 0)
                {
                    // Solo mover el foco si el campo de unidades está vacío
                    // (evita interrumpir si el usuario ya está escribiendo ahí)
                }
            }
            else
            {
                // Limpiar campos si no se encuentra
                txtRackLargo.Clear();
                txtRackAncho.Clear();
                txtRackAlto.Clear();

                // Fondo amarillo claro para indicar que no se encontró
                if (codigo.Length > 2) // Solo mostrar si ya escribió algo significativo
                {
                    txtRackLargo.BackColor = System.Drawing.Color.LightYellow;
                    txtRackAncho.BackColor = System.Drawing.Color.LightYellow;
                    txtRackAlto.BackColor = System.Drawing.Color.LightYellow;
                }
                else
                {
                    txtRackLargo.BackColor = System.Drawing.SystemColors.Window;
                    txtRackAncho.BackColor = System.Drawing.SystemColors.Window;
                    txtRackAlto.BackColor = System.Drawing.SystemColors.Window;
                }
            }
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
            // Las dimensiones del tráiler son fijas, solo mostramos confirmación
            MessageBox.Show(
                $"Dimensiones del tráiler (fijas):\n\n" +
                $"Largo: {trailerLargo}mm\n" +
                $"Ancho: {trailerAncho}mm\n" +
                $"Alto: {trailerAlto}mm",
                "Configuración del Tráiler",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void btnAgregarRack_Click(object sender, EventArgs e)
        {
            string codigo = txtCodigoRack.Text.Trim().ToUpper();

            if (string.IsNullOrEmpty(codigo))
            {
                MessageBox.Show("Por favor, introduce un código de rack.", "Código requerido",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCodigoRack.Focus();
                return;
            }

            // Buscar en el catálogo
            if (!rackCatalog.ContainsKey(codigo))
            {
                MessageBox.Show(
                    $"El código '{codigo}' no se encuentra en el catálogo.\n\n" +
                    $"Racks disponibles: {rackCatalog.Count}\n" +
                    $"Usa el botón '🔍 Ver Catálogo' para ver la lista completa.",
                    "Código no encontrado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtCodigoRack.Focus();
                txtCodigoRack.SelectAll();
                return;
            }

            var rackData = rackCatalog[codigo];

            // Obtener las unidades
            if (!int.TryParse(txtRackUnidades.Text, out int unidades) || unidades <= 0)
            {
                MessageBox.Show("Introduce un número válido de unidades (mayor a 0).", "Unidades inválidas",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRackUnidades.Focus();
                txtRackUnidades.SelectAll();
                return;
            }

            double largo = rackData.Largo;
            double ancho = rackData.Ancho;
            double alto = rackData.Alto;

            if (!TrailerCalculator.UnitFitsSingle(largo, ancho, alto,
                trailerLargo, trailerAncho, trailerAlto))
            {
                rtbResultado.AppendText($"❌ ERROR → El rack {codigo} ({largo}×{ancho}×{alto}) no cabe en el tráiler.\n");
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
                rtbResultado.AppendText($"❌ NO CABE → {codigo}: {reason}\n");
                return;
            }

            groups = temp;

            string desc = !string.IsNullOrEmpty(rackData.Descripcion) ? $" ({rackData.Descripcion})" : "";
            rtbResultado.AppendText($"✓ {unidades}x {codigo}{desc} - {largo}×{ancho}×{alto}mm\n");

            if (!string.IsNullOrEmpty(reason))
                rtbResultado.AppendText($"  {reason}\n");

            // Limpiar para el próximo
            txtCodigoRack.Clear();
            txtRackUnidades.Clear();
            txtCodigoRack.Focus();
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

            // Restaurar colores normales
            txtRackLargo.BackColor = System.Drawing.SystemColors.Window;
            txtRackAncho.BackColor = System.Drawing.SystemColors.Window;
            txtRackAlto.BackColor = System.Drawing.SystemColors.Window;

            txtCodigoRack.Focus();
        }

        private void btnBuscarCatalogo_Click(object sender, EventArgs e)
        {
            if (rackCatalog.Count == 0)
            {
                MessageBox.Show(
                    "No hay racks en el catálogo.\n\n" +
                    "Asegúrate de que el archivo 'racks_catalog.csv' existe " +
                    "en la carpeta del programa y contiene datos válidos.",
                    "Catálogo vacío",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            rtbResultado.AppendText("\n╔════════════════════════════════════════════════════════════╗\n");
            rtbResultado.AppendText("║                   CATÁLOGO DE RACKS                        ║\n");
            rtbResultado.AppendText("╚════════════════════════════════════════════════════════════╝\n\n");

            rtbResultado.AppendText($"Total de racks disponibles: {rackCatalog.Count}\n\n");
            rtbResultado.AppendText(string.Format("{0,-10} {1,-10} {2,-10} {3,-10} {4}\n",
                "Código", "Largo", "Ancho", "Alto", "Descripción"));
            rtbResultado.AppendText(new string('─', 70) + "\n");

            foreach (var rack in rackCatalog.Values.OrderBy(r => r.Codigo))
            {
                rtbResultado.AppendText(string.Format("{0,-10} {1,-10:F0} {2,-10:F0} {3,-10:F0} {4}\n",
                    rack.Codigo,
                    rack.Largo,
                    rack.Ancho,
                    rack.Alto,
                    rack.Descripcion));
            }

            rtbResultado.AppendText("\n" + new string('═', 70) + "\n");
        }

        private void btnLimpiarResultado_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "¿Deseas limpiar el panel de resultado?\n\n" +
                "Esto solo limpiará el texto mostrado, no eliminará los racks agregados al tráiler.",
                "Limpiar Panel",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                rtbResultado.Clear();
                rtbResultado.AppendText($"Panel limpiado.\n");
                rtbResultado.AppendText($"Tráiler: {trailerLargo}×{trailerAncho}×{trailerAlto}mm | ");
                rtbResultado.AppendText($"Catálogo: {rackCatalog.Count} racks | ");
                rtbResultado.AppendText($"Racks agregados: {groups.Sum(g => g.Value.UnitHeights.Count)}\n\n");
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            txtCodigoRack.Clear();
            txtRackLargo.Clear();
            txtRackAncho.Clear();
            txtRackAlto.Clear();
            txtRackUnidades.Clear();

            groups = new Dictionary<string, Group>();

            rtbResultado.Clear();
            rtbResultado.AppendText($"Tráiler configurado: {trailerLargo}×{trailerAncho}×{trailerAlto}mm\n");
            rtbResultado.AppendText($"Catálogo: {rackCatalog.Count} racks disponibles\n\n");
            rtbResultado.AppendText("Nuevo proyecto iniciado.\n");
        }
    }

    // Clase para almacenar datos de los racks del catálogo
    public class RackData
    {
        public string Codigo { get; set; }
        public double Largo { get; set; }
        public double Ancho { get; set; }
        public double Alto { get; set; }
        public string Descripcion { get; set; }
    }
}