using System.Windows.Forms;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace CalculoRacksTrailerDesktop.V2
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.SplitContainer splitContainer1;

        private System.Windows.Forms.Label lblTrailerLargo;
        private System.Windows.Forms.Label lblTrailerAncho;
        private System.Windows.Forms.Label lblTrailerAlto;
        private System.Windows.Forms.TextBox txtTrailerLargo;
        private System.Windows.Forms.TextBox txtTrailerAncho;
        private System.Windows.Forms.TextBox txtTrailerAlto;
        private System.Windows.Forms.Button btnSetTrailer;

        private System.Windows.Forms.Label lblCodigoRack;
        private System.Windows.Forms.Label lblRackLargo;
        private System.Windows.Forms.Label lblRackAncho;
        private System.Windows.Forms.Label lblRackAlto;
        private System.Windows.Forms.Label lblRackUnidades;

        private System.Windows.Forms.TextBox txtCodigoRack;
        private System.Windows.Forms.TextBox txtRackLargo;
        private System.Windows.Forms.TextBox txtRackAncho;
        private System.Windows.Forms.TextBox txtRackAlto;
        private System.Windows.Forms.TextBox txtRackUnidades;

        private System.Windows.Forms.Button btnAgregarRack;
        private System.Windows.Forms.Button btnMostrarResumen;
        private System.Windows.Forms.Button btnMostrarDiagrama;
        private System.Windows.Forms.Button btnNuevo;
        private System.Windows.Forms.Button btnLimpiarRack;

        // Nuevos controles para estrategia
        private System.Windows.Forms.Label lblStrategy;
        private System.Windows.Forms.ComboBox cmbStrategy;
        private System.Windows.Forms.Label lblStrategyInfo;

        private System.Windows.Forms.RichTextBox rtbResultado;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            splitContainer1 = new SplitContainer();
            lblTrailerLargo = new Label();
            txtTrailerLargo = new TextBox();
            lblTrailerAncho = new Label();
            txtTrailerAncho = new TextBox();
            lblTrailerAlto = new Label();
            txtTrailerAlto = new TextBox();
            btnSetTrailer = new Button();
            lblStrategy = new Label();
            cmbStrategy = new ComboBox();
            lblStrategyInfo = new Label();
            lblCodigoRack = new Label();
            txtCodigoRack = new TextBox();
            lblRackLargo = new Label();
            txtRackLargo = new TextBox();
            lblRackAncho = new Label();
            txtRackAncho = new TextBox();
            lblRackAlto = new Label();
            txtRackAlto = new TextBox();
            lblRackUnidades = new Label();
            txtRackUnidades = new TextBox();
            btnAgregarRack = new Button();
            btnMostrarResumen = new Button();
            btnMostrarDiagrama = new Button();
            btnLimpiarRack = new Button();
            btnNuevo = new Button();
            rtbResultado = new RichTextBox();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.AutoScroll = true;
            splitContainer1.Panel1.Controls.Add(lblTrailerLargo);
            splitContainer1.Panel1.Controls.Add(txtTrailerLargo);
            splitContainer1.Panel1.Controls.Add(lblTrailerAncho);
            splitContainer1.Panel1.Controls.Add(txtTrailerAncho);
            splitContainer1.Panel1.Controls.Add(lblTrailerAlto);
            splitContainer1.Panel1.Controls.Add(txtTrailerAlto);
            splitContainer1.Panel1.Controls.Add(btnSetTrailer);
            splitContainer1.Panel1.Controls.Add(lblStrategy);
            splitContainer1.Panel1.Controls.Add(cmbStrategy);
            splitContainer1.Panel1.Controls.Add(lblStrategyInfo);
            splitContainer1.Panel1.Controls.Add(lblCodigoRack);
            splitContainer1.Panel1.Controls.Add(txtCodigoRack);
            splitContainer1.Panel1.Controls.Add(lblRackLargo);
            splitContainer1.Panel1.Controls.Add(txtRackLargo);
            splitContainer1.Panel1.Controls.Add(lblRackAncho);
            splitContainer1.Panel1.Controls.Add(txtRackAncho);
            splitContainer1.Panel1.Controls.Add(lblRackAlto);
            splitContainer1.Panel1.Controls.Add(txtRackAlto);
            splitContainer1.Panel1.Controls.Add(lblRackUnidades);
            splitContainer1.Panel1.Controls.Add(txtRackUnidades);
            splitContainer1.Panel1.Controls.Add(btnAgregarRack);
            splitContainer1.Panel1.Controls.Add(btnMostrarResumen);
            splitContainer1.Panel1.Controls.Add(btnMostrarDiagrama);
            splitContainer1.Panel1.Controls.Add(btnLimpiarRack);
            splitContainer1.Panel1.Controls.Add(btnNuevo);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(rtbResultado);
            splitContainer1.Size = new Size(1200, 700);
            splitContainer1.SplitterDistance = 349;
            splitContainer1.TabIndex = 0;
            // 
            // lblTrailerLargo
            // 
            lblTrailerLargo.Location = new Point(20, 43);
            lblTrailerLargo.Name = "lblTrailerLargo";
            lblTrailerLargo.Size = new Size(136, 23);
            lblTrailerLargo.TabIndex = 0;
            lblTrailerLargo.Text = "Largo Trailer (mm)";
            // 
            // txtTrailerLargo
            // 
            txtTrailerLargo.Location = new Point(180, 43);
            txtTrailerLargo.Name = "txtTrailerLargo";
            txtTrailerLargo.Size = new Size(150, 27);
            txtTrailerLargo.TabIndex = 1;
            // 
            // lblTrailerAncho
            // 
            lblTrailerAncho.Location = new Point(20, 73);
            lblTrailerAncho.Name = "lblTrailerAncho";
            lblTrailerAncho.Size = new Size(136, 23);
            lblTrailerAncho.TabIndex = 2;
            lblTrailerAncho.Text = "Ancho Trailer (mm)";
            // 
            // txtTrailerAncho
            // 
            txtTrailerAncho.Location = new Point(180, 73);
            txtTrailerAncho.Name = "txtTrailerAncho";
            txtTrailerAncho.Size = new Size(150, 27);
            txtTrailerAncho.TabIndex = 3;
            // 
            // lblTrailerAlto
            // 
            lblTrailerAlto.Location = new Point(20, 103);
            lblTrailerAlto.Name = "lblTrailerAlto";
            lblTrailerAlto.Size = new Size(136, 23);
            lblTrailerAlto.TabIndex = 4;
            lblTrailerAlto.Text = "Alto Trailer (mm)";
            // 
            // txtTrailerAlto
            // 
            txtTrailerAlto.Location = new Point(180, 103);
            txtTrailerAlto.Name = "txtTrailerAlto";
            txtTrailerAlto.Size = new Size(150, 27);
            txtTrailerAlto.TabIndex = 5;
            // 
            // btnSetTrailer
            // 
            btnSetTrailer.Location = new Point(20, 133);
            btnSetTrailer.Name = "btnSetTrailer";
            btnSetTrailer.Size = new Size(150, 30);
            btnSetTrailer.TabIndex = 6;
            btnSetTrailer.Text = "Configurar Tráiler";
            btnSetTrailer.Click += btnSetTrailer_Click;
            // 
            // lblStrategy
            // 
            lblStrategy.Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold);
            lblStrategy.Location = new Point(20, 173);
            lblStrategy.Name = "lblStrategy";
            lblStrategy.Size = new Size(150, 23);
            lblStrategy.TabIndex = 7;
            lblStrategy.Text = "Estrategia de Colocación";
            // 
            // cmbStrategy
            // 
            cmbStrategy.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStrategy.Location = new Point(20, 196);
            cmbStrategy.Name = "cmbStrategy";
            cmbStrategy.Size = new Size(310, 28);
            cmbStrategy.TabIndex = 8;
            cmbStrategy.SelectedIndexChanged += cmbStrategy_SelectedIndexChanged;
            // 
            // lblStrategyInfo
            // 
            lblStrategyInfo.Font = new System.Drawing.Font("Segoe UI", 8F, FontStyle.Italic);
            lblStrategyInfo.ForeColor = Color.DarkBlue;
            lblStrategyInfo.Location = new Point(20, 230);
            lblStrategyInfo.Name = "lblStrategyInfo";
            lblStrategyInfo.Size = new Size(310, 50);
            lblStrategyInfo.TabIndex = 9;
            lblStrategyInfo.Text = "Coloca primero las torres más anchas.\nÓptimo para aprovechar el ancho del tráiler.";
            // 
            // lblCodigoRack
            // 
            lblCodigoRack.Location = new Point(20, 290);
            lblCodigoRack.Name = "lblCodigoRack";
            lblCodigoRack.Size = new Size(136, 23);
            lblCodigoRack.TabIndex = 10;
            lblCodigoRack.Text = "Código Rack";
            // 
            // txtCodigoRack
            // 
            txtCodigoRack.Location = new Point(180, 290);
            txtCodigoRack.Name = "txtCodigoRack";
            txtCodigoRack.Size = new Size(150, 27);
            txtCodigoRack.TabIndex = 11;
            // 
            // lblRackLargo
            // 
            lblRackLargo.Location = new Point(20, 320);
            lblRackLargo.Name = "lblRackLargo";
            lblRackLargo.Size = new Size(150, 23);
            lblRackLargo.TabIndex = 12;
            lblRackLargo.Text = "Largo Rack (mm)";
            // 
            // txtRackLargo
            // 
            txtRackLargo.Location = new Point(180, 320);
            txtRackLargo.Name = "txtRackLargo";
            txtRackLargo.Size = new Size(150, 27);
            txtRackLargo.TabIndex = 13;
            // 
            // lblRackAncho
            // 
            lblRackAncho.Location = new Point(20, 350);
            lblRackAncho.Name = "lblRackAncho";
            lblRackAncho.Size = new Size(136, 23);
            lblRackAncho.TabIndex = 14;
            lblRackAncho.Text = "Ancho Rack (mm)";
            // 
            // txtRackAncho
            // 
            txtRackAncho.Location = new Point(180, 350);
            txtRackAncho.Name = "txtRackAncho";
            txtRackAncho.Size = new Size(150, 27);
            txtRackAncho.TabIndex = 15;
            // 
            // lblRackAlto
            // 
            lblRackAlto.Location = new Point(20, 380);
            lblRackAlto.Name = "lblRackAlto";
            lblRackAlto.Size = new Size(136, 23);
            lblRackAlto.TabIndex = 16;
            lblRackAlto.Text = "Alto Rack (mm)";
            // 
            // txtRackAlto
            // 
            txtRackAlto.Location = new Point(180, 380);
            txtRackAlto.Name = "txtRackAlto";
            txtRackAlto.Size = new Size(150, 27);
            txtRackAlto.TabIndex = 17;
            // 
            // lblRackUnidades
            // 
            lblRackUnidades.Location = new Point(20, 410);
            lblRackUnidades.Name = "lblRackUnidades";
            lblRackUnidades.Size = new Size(136, 23);
            lblRackUnidades.TabIndex = 18;
            lblRackUnidades.Text = "Unidades";
            // 
            // txtRackUnidades
            // 
            txtRackUnidades.Location = new Point(180, 410);
            txtRackUnidades.Name = "txtRackUnidades";
            txtRackUnidades.Size = new Size(150, 27);
            txtRackUnidades.TabIndex = 19;
            // 
            // btnAgregarRack
            // 
            btnAgregarRack.Location = new Point(20, 440);
            btnAgregarRack.Name = "btnAgregarRack";
            btnAgregarRack.Size = new Size(150, 30);
            btnAgregarRack.TabIndex = 20;
            btnAgregarRack.Text = "Agregar Rack";
            btnAgregarRack.Click += btnAgregarRack_Click;
            // 
            // btnMostrarResumen
            // 
            btnMostrarResumen.Location = new Point(20, 527);
            btnMostrarResumen.Name = "btnMostrarResumen";
            btnMostrarResumen.Size = new Size(150, 30);
            btnMostrarResumen.TabIndex = 21;
            btnMostrarResumen.Text = "Mostrar Resumen";
            btnMostrarResumen.Click += btnMostrarResumen_Click;
            // 
            // btnMostrarDiagrama
            // 
            btnMostrarDiagrama.Location = new Point(20, 562);
            btnMostrarDiagrama.Name = "btnMostrarDiagrama";
            btnMostrarDiagrama.Size = new Size(150, 30);
            btnMostrarDiagrama.TabIndex = 22;
            btnMostrarDiagrama.Text = "Mostrar Diagrama";
            btnMostrarDiagrama.Click += btnMostrarDiagrama_Click;
            // 
            // btnLimpiarRack
            // 
            btnLimpiarRack.Location = new Point(20, 476);
            btnLimpiarRack.Name = "btnLimpiarRack";
            btnLimpiarRack.Size = new Size(150, 30);
            btnLimpiarRack.TabIndex = 23;
            btnLimpiarRack.Text = "Limpiar Datos Rack";
            btnLimpiarRack.Click += btnLimpiarRack_Click;
            // 
            // btnNuevo
            // 
            btnNuevo.Location = new Point(20, 8);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(150, 30);
            btnNuevo.TabIndex = 24;
            btnNuevo.Text = "Nuevo";
            btnNuevo.Click += btnNuevo_Click;
            // 
            // rtbResultado
            // 
            rtbResultado.Dock = DockStyle.Fill;
            rtbResultado.Font = new System.Drawing.Font("Consolas", 10F);
            rtbResultado.Location = new Point(0, 0);
            rtbResultado.Name = "rtbResultado";
            rtbResultado.ReadOnly = true;
            rtbResultado.Size = new Size(847, 700);
            rtbResultado.TabIndex = 0;
            rtbResultado.Text = "";
            // 
            // Form1
            // 
            ClientSize = new Size(1200, 700);
            Controls.Add(splitContainer1);
            Name = "Form1";
            Text = "Cálculo de Racks en Tráiler (mm) - Con Estrategias";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}