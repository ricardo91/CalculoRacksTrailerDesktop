using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CalculoRacksTrailerDesktop.V2.Models;
using CalculoRacksTrailerDesktop.V2.Services;
using CalculoRacksTrailerDesktop.V2.Utils;

namespace CalculoRacksTrailerDesktop.V2.Views
{
    public partial class Form1 : Form
    {
        #region Campos privados

        private double trailerLargo, trailerAncho, trailerAlto;
        private readonly RackService rackService = new RackService();

        private Dictionary<string, Group> groups = new Dictionary<string, Group>();
        private PlacementStrategy currentStrategy = PlacementStrategy.GreedyByWidth;

        // Catálogo de racks cargados desde CSV
        private Dictionary<string, Rack> rackCatalog = new Dictionary<string, Rack>();
        private const string CSV_FILENAME = "racks_catalog.csv";
        private static readonly char[] CSV_SEPARATORS = { ',', ';' };

        #endregion Campos privados

        #region Constructor e Inicialización

        public Form1()
        {
            InitializeComponent();
            InitializeStrategyComboBox();
            CargarDatosIniciales();
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

        private void CargarDatosIniciales()
        {
            // Configurar dimensiones fijas del tráiler
            ConfigurarTrailer(largo: 13600, ancho: 2500, alto: 2900);

            // Bloquear edición de campos
            BloquearEdicionCamposTrailer(); // Las dimensiones del tráiler son fijas
            BloquearEdicionCamposRack(); // Las dimensiones del rack se autocompletan desde el catálogo

            rtbResultado.AppendText($"Tráiler configurado automáticamente: {trailerLargo}×{trailerAncho}×{trailerAlto}mm{Environment.NewLine}{Environment.NewLine}");

            // Cargar catálogo de racks desde CSV
            CargarCatalogoRacks();
        }

        #endregion Constructor e Inicialización

        private void ConfigurarTrailer(double largo, double ancho, double alto)
        {
            trailerLargo = largo;
            trailerAncho = ancho;
            trailerAlto = alto;

            txtTrailerLargo.Text = trailerLargo.ToString();
            txtTrailerAncho.Text = trailerAncho.ToString();
            txtTrailerAlto.Text = trailerAlto.ToString();
        }

        private void BloquearEdicionCamposTrailer()
        {
            txtTrailerLargo.ReadOnly = true;
            txtTrailerAncho.ReadOnly = true;
            txtTrailerAlto.ReadOnly = true;
        }

        private void BloquearEdicionCamposRack()
        {
            txtRackLargo.ReadOnly = true;
            txtRackAncho.ReadOnly = true;
            txtRackAlto.ReadOnly = true;
        }

        private void LimpiarCamposDimensionesRack()
        {
            txtRackLargo.Clear();
            txtRackAncho.Clear();
            txtRackAlto.Clear();
        }

        private void SetBackColorCamposRack(System.Drawing.Color color)
        {
            txtRackLargo.BackColor = color;
            txtRackAncho.BackColor = color;
            txtRackAlto.BackColor = color;
        }

        private void CargarCatalogoRacks()
        {
            string? exeDir = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            if (string.IsNullOrEmpty(exeDir))
            {
                MessageBox.Show("No se pudo determinar la carpeta del ejecutable.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string csvPath = System.IO.Path.Combine(exeDir, CSV_FILENAME);

            if (!System.IO.File.Exists(csvPath))
            {
                rtbResultado.AppendText($"⚠ Archivo 'racks_catalog.csv' no encontrado.{Environment.NewLine}");
                rtbResultado.AppendText($"Se creará un archivo de ejemplo en la carpeta del programa.{Environment.NewLine}{Environment.NewLine}");
                CrearArchivoEjemplo(csvPath);
                return;
            }

            try
            {
                var lines = System.IO.File.ReadAllLines(csvPath);
                if (lines.Length == 0)
                {
                    rtbResultado.AppendText($"⚠ Archivo vacío: {csvPath}{Environment.NewLine}{Environment.NewLine}");
                    return;
                }

                int loaded = 0;
                int errors = 0;

                // Saltar la primera línea si es encabezado
                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var parts = line.Split(CSV_SEPARATORS);
                    if (parts.Length >= 4)
                    {
                        string codigo = parts[0].Trim();

                        if (double.TryParse(parts[1].Trim(), out double largo) &&
                            double.TryParse(parts[2].Trim(), out double ancho) &&
                            double.TryParse(parts[3].Trim(), out double alto))
                        {
                            string descripcion = parts.Length > 4 ? parts[4].Trim() : string.Empty;

                            rackCatalog[codigo.ToUpper()] = new Rack
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
                {
                    rtbResultado.AppendText($" ({errors} líneas con errores ignoradas)");
                }
                rtbResultado.AppendText($"{Environment.NewLine}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el catálogo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CrearArchivoEjemplo(string path)
        {
            try
            {
                var ejemplo = new System.Text.StringBuilder();
                ejemplo.AppendLine("Codigo;Largo;Ancho;Alto;Descripcion");
                ejemplo.AppendLine("00518;1950;1200;1610;");
                ejemplo.AppendLine("04968;1670;1200;1100;");
                ejemplo.AppendLine("04971;1670;1200;1300;");
                ejemplo.AppendLine("04990;2400;1480;1980;");
                ejemplo.AppendLine("04993;2400;1480;1380;");
                ejemplo.AppendLine("10077;1670;1200;900;");
                ejemplo.AppendLine("10078;1670;1200;920;");
                ejemplo.AppendLine("10147;1670;1200;920;");
                ejemplo.AppendLine("40065;2400;1910;1350;");
                ejemplo.AppendLine("40066;2400;1910;650;");
                ejemplo.AppendLine("40068;3350;2400;1900;");
                ejemplo.AppendLine("40070;3700;2400;1900;");
                ejemplo.AppendLine("40071;3700;2400;1900;");
                ejemplo.AppendLine("40074;1670;1200;1680;");
                ejemplo.AppendLine("40075;1670;1200;1680;");
                ejemplo.AppendLine("40076;1670;1200;1400;");
                ejemplo.AppendLine("40077;1670;1200;1400;");
                ejemplo.AppendLine("40078;1670;1200;1350;");
                ejemplo.AppendLine("40079;1670;1200;1350;");
                ejemplo.AppendLine("40082;1670;1200;1450;");
                ejemplo.AppendLine("40083;1670;1200;1450;");
                ejemplo.AppendLine("40094;3500;1670;1400;");
                ejemplo.AppendLine("40103;1670;1200;1400;");
                ejemplo.AppendLine("40104;1480;1200;1350;");
                ejemplo.AppendLine("40207;2400;1910;1650;");
                ejemplo.AppendLine("41380;1500;1200;700;");
                ejemplo.AppendLine("41384;2400;1200;1450;");
                ejemplo.AppendLine("41385;1340;1200;1440;");
                ejemplo.AppendLine("41386;2400;1670;925;");
                ejemplo.AppendLine("41387;2400;1670;925;");
                ejemplo.AppendLine("41500;1200;1480;700;");

                System.IO.File.WriteAllText(path, ejemplo.ToString());

                rtbResultado.AppendText($"✓ Archivo de ejemplo creado: {path}{Environment.NewLine}");
                rtbResultado.AppendText($"Edita el archivo y reinicia la aplicación para cargar tus racks.{Environment.NewLine}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear archivo de ejemplo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtCodigoRack_TextChanged(object sender, EventArgs e)
        {
            string codigo = txtCodigoRack.Text.Trim().ToUpper();

            if (string.IsNullOrEmpty(codigo))
            {
                // Limpiar campos si el código está vacío
                LimpiarCamposDimensionesRack();
                SetBackColorCamposRack(System.Drawing.SystemColors.Window);
                return;
            }

            // Buscar en el catálogo
            if (rackCatalog.TryGetValue(codigo, out Rack? rack))
            {
                // Autocompletar los campos
                txtRackLargo.Text = rack.Largo.ToString();
                txtRackAncho.Text = rack.Ancho.ToString();
                txtRackAlto.Text = rack.Alto.ToString();

                // Fondo verde claro para indicar que se encontró
                SetBackColorCamposRack(System.Drawing.Color.LightGreen);               

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
                LimpiarCamposDimensionesRack();
                // Fondo amarillo claro para indicar que no se encontró. Solo mostrar si ya escribió algo significativo
                SetBackColorCamposRack(codigo.Length > 2 ? System.Drawing.Color.LightYellow : System.Drawing.SystemColors.Window);
            }
        }

        private void cmbStrategy_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentStrategy = (PlacementStrategy)cmbStrategy.SelectedIndex;
            string description = GetStrategyDescription(currentStrategy);
            lblStrategyInfo.Text = description;
        }

        #region Helpers de Estrategia

        private string GetStrategyName(PlacementStrategy strategy)
        {
            return strategy switch
            {
                PlacementStrategy.GreedyByWidth => "Greedy - Por Ancho",
                PlacementStrategy.GreedyByLength => "Greedy - Por Largo",
                PlacementStrategy.GreedyByArea => "Greedy - Por Área",
                PlacementStrategy.BestFit => "Best Fit",
                _ => "Desconocida"
            };
        }

        private string GetStrategyDescription(PlacementStrategy strategy)
        {
            return strategy switch
            {
                PlacementStrategy.GreedyByWidth => $"Coloca primero las torres más anchas.{Environment.NewLine}Óptimo para aprovechar el ancho del tráiler.",
                PlacementStrategy.GreedyByLength => $"Coloca primero las torres más largas.{Environment.NewLine}Óptimo cuando el largo es limitante.",
                PlacementStrategy.GreedyByArea => $"Coloca primero las torres más grandes (área).{Environment.NewLine}Balance entre largo y ancho.",
                PlacementStrategy.BestFit => $"Prueba todas las estrategias y elige la mejor.{Environment.NewLine}Más lento pero garantiza mejor resultado.",
                _ => string.Empty
            };
        }

        #endregion Helpers de Estrategia

        #region Botones

        private void btnSetTrailer_Click(object sender, EventArgs e)
        {
            // Las dimensiones del tráiler son fijas, solo mostramos confirmación
            MessageBox.Show(
                $"Dimensiones del tráiler (fijas):{Environment.NewLine}{Environment.NewLine}" +
                $"Largo: {trailerLargo}mm{Environment.NewLine}" +
                $"Ancho: {trailerAncho}mm{Environment.NewLine}" +
                $"Alto: {trailerAlto}mm",
                "Configuración del Tráiler",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void btnAgregarRack_Click(object sender, EventArgs e)
        {
            AddRackResult result = rackService.AddRack(new AddRackRequest
            {
                Codigo = txtCodigoRack.Text,
                UnidadesStr = txtRackUnidades.Text,
                TrailerLargo = trailerLargo,
                TrailerAncho = trailerAncho,
                TrailerAlto = trailerAlto,
                Groups = groups,
                RackCatalog = rackCatalog,
                Strategy = currentStrategy
            });

            if (result.IsSuccess)
            {
                if (result.UpdatedGroups is not null)
                {
                    groups = result.UpdatedGroups;
                }

                if (!string.IsNullOrEmpty(result.Message))
                {
                    rtbResultado.AppendText(result.Message);
                }

                // Limpiar para el próximo
                txtCodigoRack.Clear();
                txtRackUnidades.Clear();
                txtCodigoRack.Focus();
            }
            else
            {
                switch (result.ErrorType)
                {
                    case ErrorType.CodeEmpty:
                        MessageBox.Show(result.Message, result.ShortMessage, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtCodigoRack.Focus();
                        break;
                    case ErrorType.CodeNotFound:
                        MessageBox.Show(result.Message, result.ShortMessage, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtCodigoRack.Focus();
                        txtCodigoRack.SelectAll();
                        break;
                    case ErrorType.InvalidUnits:
                        MessageBox.Show(result.Message, result.ShortMessage, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtRackUnidades.Focus();
                        txtRackUnidades.SelectAll();
                        break;
                    case ErrorType.DoesNotFit:
                    case ErrorType.PlacementFailed:
                        rtbResultado.AppendText(result.Message);
                        break;
                    default:
                        break;
                }
            }

            #region Commented Old Code
            /*
            string codigo = txtCodigoRack.Text.Trim().ToUpper();

            if (string.IsNullOrEmpty(codigo))
            {
                MessageBox.Show("Por favor, introduce un código de rack.", "Código requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCodigoRack.Focus();
                return;
            }

            // Buscar en el catálogo
            if (!rackCatalog.TryGetValue(codigo, out Rack? rack))
            {
                MessageBox.Show(
                    $"El código '{codigo}' no se encuentra en el catálogo.{Environment.NewLine}{Environment.NewLine}" +
                    $"Racks disponibles: {rackCatalog.Count}{Environment.NewLine}" +
                    $"Usa el botón '🔍 Ver Catálogo' para ver la lista completa.",
                    "Código no encontrado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtCodigoRack.Focus();
                txtCodigoRack.SelectAll();
                return;
            }

            // Rack existente
            // Obtener las unidades
            if (!int.TryParse(txtRackUnidades.Text, out int unidades) || unidades <= 0)
            {
                MessageBox.Show("Introduce un número válido de unidades (mayor a 0).", "Unidades inválidas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRackUnidades.Focus();
                txtRackUnidades.SelectAll();
                return;
            }

            double largo = rack.Largo;
            double ancho = rack.Ancho;
            double alto = rack.Alto;

            if (!TrailerCalculator.UnitFitsSingle(largo, ancho, alto, trailerLargo, trailerAncho, trailerAlto))
            {
                rtbResultado.AppendText($"❌ ERROR → El rack {codigo} ({largo}×{ancho}×{alto}) no cabe en el tráiler.{Environment.NewLine}");
                return;
            }

            var temp = TrailerCalculator.CloneGroups(groups);

            string key = $"{largo}x{ancho}";

            if (!temp.ContainsKey(key))
            {
                temp[key] = new Group(largo, ancho);
            }                

            for (int i = 0; i < unidades; i++)
            {
                temp[key].UnitHeights.Add(alto);
            }                

            if (!temp[key].Codes.Contains(codigo))
            {
                temp[key].Codes.Add(codigo);
            }                

            // Usar la estrategia seleccionada
            bool ok = TrailerCalculator.TryPlaceAllGroupsOptimized(temp, trailerLargo, trailerAncho, trailerAlto, out string reason, currentStrategy);

            if (!ok)
            {
                rtbResultado.AppendText($"❌ NO CABE → {codigo}: {reason}{Environment.NewLine}");
                return;
            }

            groups = temp;

            string desc = !string.IsNullOrEmpty(rack.Descripcion) ? $" ({rack.Descripcion})" : string.Empty;
            rtbResultado.AppendText($"✓ {unidades}x {codigo}{desc} - {largo}×{ancho}×{alto}mm{Environment.NewLine}");

            if (!string.IsNullOrEmpty(reason))
            {
                rtbResultado.AppendText($"  {reason}{Environment.NewLine}");
            }               

            // Limpiar para el próximo
            txtCodigoRack.Clear();
            txtRackUnidades.Clear();
            txtCodigoRack.Focus();
            */
            #endregion Commented Old Code
        }

        private void btnMostrarResumen_Click(object sender, EventArgs e)
        {
            MostrarResumen();
        }

        private void MostrarResumen()
        {
            rtbResultado.AppendText(Environment.NewLine);
            rtbResultado.AppendText("--- Resumen ---");
            rtbResultado.AppendText(Environment.NewLine);
            rtbResultado.AppendText($"Estrategia: {GetStrategyName(currentStrategy)}{Environment.NewLine}{Environment.NewLine}");

            int i = 1;

            foreach (var kv in groups)
            {
                var g = kv.Value;
                rtbResultado.AppendText($"{i}. {g.Largo}×{g.Ancho} → {g.UnitHeights.Count} unidades, códigos: {string.Join(",", g.Codes)}{Environment.NewLine}");
                i++;
            }

            rtbResultado.AppendText("----------------");
            rtbResultado.AppendText(Environment.NewLine);
        }

        private void btnMostrarDiagrama_Click(object sender, EventArgs e)
        {
            DibujarDiagrama();
        }

        private void DibujarDiagrama()
        {
            rtbResultado.AppendText(Environment.NewLine);
            rtbResultado.AppendText($"╔════════════════════════════════════════════════════════════╗{Environment.NewLine}");
            rtbResultado.AppendText($"║            DIAGRAMA - VISTA SUPERIOR DEL TRÁILER           ║{Environment.NewLine}");
            rtbResultado.AppendText($"╚════════════════════════════════════════════════════════════╝{Environment.NewLine}{Environment.NewLine}");

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
                {
                    towers.Add((string.Join(",", g.Codes), g.Largo, g.Ancho, current));
                }                    
            }

            if (towers.Count == 0)
            {
                rtbResultado.AppendText($"No hay racks para mostrar.{Environment.NewLine}");
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
            rtbResultado.AppendText($"Dimensiones del tráiler:{Environment.NewLine}");
            rtbResultado.AppendText($"  • Largo (profundidad): {trailerLargo:F0}mm{Environment.NewLine}");
            rtbResultado.AppendText($"  • Ancho (lado a lado): {trailerAncho:F0}mm{Environment.NewLine}");
            rtbResultado.AppendText($"  • Alto: {trailerAlto:F0}mm{Environment.NewLine}{Environment.NewLine}");
            rtbResultado.AppendText($"Uso del espacio:{Environment.NewLine}");
            rtbResultado.AppendText($"  • Largo usado: {totalLargoUsado:F0}mm de {trailerLargo:F0}mm ({(totalLargoUsado / trailerLargo * 100):F1}%){Environment.NewLine}");
            rtbResultado.AppendText($"  • Torres colocadas: {towers.Count}{Environment.NewLine}");
            rtbResultado.AppendText($"  • Filas creadas: {rows.Count}{Environment.NewLine}{Environment.NewLine}");

            // 6. Dibujar vista superior (mirando desde arriba)
            int diagramWidth = 68;

            rtbResultado.AppendText("           ┌" + new string('─', diagramWidth) + "┐");
            rtbResultado.AppendText(Environment.NewLine);
            rtbResultado.AppendText("           │" + " FRENTE DEL TRÁILER ".PadLeft((diagramWidth + 20) / 2).PadRight(diagramWidth) + "│");
            rtbResultado.AppendText(Environment.NewLine);
            rtbResultado.AppendText("        ↑  ├" + new string('─', diagramWidth) + "┤");
            rtbResultado.AppendText(Environment.NewLine);

            int rowNumber = 1;

            foreach (var row in rows)
            {
                int rowIndex = rowNumber - 1;
                double rowLargo = rowMaxLargos[rowIndex];
                double rowAnchoTotal = row.Sum(t => t.ancho);

                rtbResultado.AppendText($"        │  │ Profundidad: {rowLargo:F0}mm | Ancho usado: {rowAnchoTotal:F0}/{trailerAncho:F0}mm (FILA {rowNumber}){Environment.NewLine}");
                rtbResultado.AppendText($" LARGO  │  ├" + new string('─', diagramWidth) + "┤");
                rtbResultado.AppendText(Environment.NewLine);

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

                    rtbResultado.AppendText("│");
                    rtbResultado.AppendText(Environment.NewLine);
                }

                rowNumber++;
            }

            // Mostrar espacio vacío
            double espacioVacio = trailerLargo - totalLargoUsado;
            if (espacioVacio > 10)
            {
                rtbResultado.AppendText("        │  ├" + new string('─', diagramWidth) + "┤");
                rtbResultado.AppendText(Environment.NewLine);
                rtbResultado.AppendText($" {trailerLargo:F0}mm │  │ [ ESPACIO VACÍO: {espacioVacio:F0}mm de profundidad restante ]");
                rtbResultado.AppendText(new string(' ', Math.Max(0, diagramWidth - 52)) + "│");
                rtbResultado.AppendText(Environment.NewLine);
            }

            rtbResultado.AppendText("        ↓  ├" + new string('─', diagramWidth) + "┤");
            rtbResultado.AppendText(Environment.NewLine);
            rtbResultado.AppendText("           │" + " PARTE TRASERA ".PadLeft((diagramWidth + 15) / 2).PadRight(diagramWidth) + "│");
            rtbResultado.AppendText(Environment.NewLine);
            rtbResultado.AppendText("           └" + new string('─', diagramWidth) + "┘");
            rtbResultado.AppendText(Environment.NewLine);
            rtbResultado.AppendText("            ←" + new string('─', diagramWidth - 2) + "→");
            rtbResultado.AppendText(Environment.NewLine);
            rtbResultado.AppendText($"             ANCHO DEL TRÁILER ({trailerAncho:F0}mm){Environment.NewLine}{Environment.NewLine}");

            // 7. Leyenda detallada
            rtbResultado.AppendText($"═══════════════════════════════════════════════════════════{Environment.NewLine}");
            rtbResultado.AppendText($"LEYENDA:{Environment.NewLine}");
            rtbResultado.AppendText($"  • Vista desde arriba del tráiler{Environment.NewLine}");
            rtbResultado.AppendText($"  • Cada caja representa una torre de racks apilados{Environment.NewLine}");
            rtbResultado.AppendText($"  • Formato dentro: ANCHO×LARGO(ALTURA) - todas en mm{Environment.NewLine}");
            rtbResultado.AppendText($"  • Las torres se colocan DE IZQUIERDA A DERECHA{Environment.NewLine}");
            rtbResultado.AppendText($"  • Profundidad de fila = LARGO máximo de sus torres{Environment.NewLine}");
            rtbResultado.AppendText($"  • Nueva fila cuando el ancho acumulado excede {trailerAncho}mm{Environment.NewLine}");
            rtbResultado.AppendText($"  • Los puntos (···) indican ancho sin utilizar{Environment.NewLine}");
            rtbResultado.AppendText($"═══════════════════════════════════════════════════════════{Environment.NewLine}");
        }

        private void btnLimpiarRack_Click(object sender, EventArgs e)
        {
            LimpiarRack();
        }

        private void LimpiarRack()
        {
            LimpiarCamposDimensionesRack();
            txtCodigoRack.Clear();
            txtRackUnidades.Clear();

            // Restaurar colores normales
            SetBackColorCamposRack(System.Drawing.SystemColors.Window);

            txtCodigoRack.Focus();
        }

        private void btnBuscarCatalogo_Click(object sender, EventArgs e)
        {
            BuscarCatalogo();
        }

        private void BuscarCatalogo()
        {
            if (rackCatalog.Count == 0)
            {
                MessageBox.Show(
                    $"No hay racks en el catálogo.{Environment.NewLine}{Environment.NewLine}" +
                    $"Asegúrate de que el archivo 'racks_catalog.csv' existe " +
                    $"en la carpeta del programa y contiene datos válidos.",
                    $"Catálogo vacío",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            rtbResultado.AppendText(Environment.NewLine);
            rtbResultado.AppendText($"╔════════════════════════════════════════════════════════════╗{Environment.NewLine}");
            rtbResultado.AppendText($"║                   CATÁLOGO DE RACKS                        ║{Environment.NewLine}");
            rtbResultado.AppendText($"╚════════════════════════════════════════════════════════════╝{Environment.NewLine}{Environment.NewLine}");

            rtbResultado.AppendText($"Total de racks disponibles: {rackCatalog.Count}{Environment.NewLine}{Environment.NewLine}");
            rtbResultado.AppendText(string.Format("{0,-10} {1,-10} {2,-10} {3,-10} {4}{5}",
                "Código", "Largo", "Ancho", "Alto", "Descripción", Environment.NewLine));
            rtbResultado.AppendText(new string('─', 70) + Environment.NewLine);

            foreach (Rack rack in rackCatalog.Values.OrderBy(r => r.Codigo))
            {
                rtbResultado.AppendText(string.Format("{0,-10} {1,-10:F0} {2,-10:F0} {3,-10:F0} {4}{5}",
                    rack.Codigo,
                    rack.Largo,
                    rack.Ancho,
                    rack.Alto,
                    rack.Descripcion,
                    Environment.NewLine));
            }

            rtbResultado.AppendText(Environment.NewLine + new string('═', 70) + Environment.NewLine);
        }

        private void btnLimpiarResultado_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                $"¿Deseas limpiar el panel de resultado?{Environment.NewLine}{Environment.NewLine}" +
                $"Esto solo limpiará el texto mostrado, no eliminará los racks agregados al tráiler.",
                $"Limpiar Panel",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                rtbResultado.Clear();
                rtbResultado.AppendText($"Panel limpiado.{Environment.NewLine}");
                rtbResultado.AppendText($"Tráiler: {trailerLargo}×{trailerAncho}×{trailerAlto}mm | ");
                rtbResultado.AppendText($"Catálogo: {rackCatalog.Count} racks | ");
                rtbResultado.AppendText($"Racks agregados: {groups.Sum(g => g.Value.UnitHeights.Count)}{Environment.NewLine}{Environment.NewLine}");
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            IniciarNuevoProyecto();
        }

        private void IniciarNuevoProyecto()
        {
            LimpiarCamposDimensionesRack();
            txtCodigoRack.Clear();
            txtRackUnidades.Clear();

            groups = new Dictionary<string, Group>();

            rtbResultado.Clear();
            rtbResultado.AppendText($"Tráiler configurado: {trailerLargo}×{trailerAncho}×{trailerAlto}mm{Environment.NewLine}");

            if ((rackCatalog?.Count ?? 0) == 0)
            {
                CargarCatalogoRacks();
            }

            rtbResultado.AppendText($"Catálogo: {rackCatalog?.Count ?? 0} racks disponibles{Environment.NewLine}{Environment.NewLine}");
            rtbResultado.AppendText($"Nuevo proyecto iniciado.{Environment.NewLine}");            
        }

        #endregion Botones
    }
}