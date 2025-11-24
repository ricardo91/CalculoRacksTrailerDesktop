namespace CalculoRacksTrailerDesktop.V1
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
            splitContainer1.Panel1.Controls.Add(lblTrailerLargo);
            splitContainer1.Panel1.Controls.Add(txtTrailerLargo);
            splitContainer1.Panel1.Controls.Add(lblTrailerAncho);
            splitContainer1.Panel1.Controls.Add(txtTrailerAncho);
            splitContainer1.Panel1.Controls.Add(lblTrailerAlto);
            splitContainer1.Panel1.Controls.Add(txtTrailerAlto);
            splitContainer1.Panel1.Controls.Add(btnSetTrailer);
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
            // lblCodigoRack
            // 
            lblCodigoRack.Location = new Point(20, 173);
            lblCodigoRack.Name = "lblCodigoRack";
            lblCodigoRack.Size = new Size(136, 23);
            lblCodigoRack.TabIndex = 7;
            lblCodigoRack.Text = "Código Rack";
            // 
            // txtCodigoRack
            // 
            txtCodigoRack.Location = new Point(180, 173);
            txtCodigoRack.Name = "txtCodigoRack";
            txtCodigoRack.Size = new Size(150, 27);
            txtCodigoRack.TabIndex = 8;
            // 
            // lblRackLargo
            // 
            lblRackLargo.Location = new Point(20, 203);
            lblRackLargo.Name = "lblRackLargo";
            lblRackLargo.Size = new Size(150, 23);
            lblRackLargo.TabIndex = 9;
            lblRackLargo.Text = "Largo Rack (mm)";
            // 
            // txtRackLargo
            // 
            txtRackLargo.Location = new Point(180, 203);
            txtRackLargo.Name = "txtRackLargo";
            txtRackLargo.Size = new Size(150, 27);
            txtRackLargo.TabIndex = 10;
            // 
            // lblRackAncho
            // 
            lblRackAncho.Location = new Point(20, 233);
            lblRackAncho.Name = "lblRackAncho";
            lblRackAncho.Size = new Size(136, 23);
            lblRackAncho.TabIndex = 11;
            lblRackAncho.Text = "Ancho Rack (mm)";
            // 
            // txtRackAncho
            // 
            txtRackAncho.Location = new Point(180, 233);
            txtRackAncho.Name = "txtRackAncho";
            txtRackAncho.Size = new Size(150, 27);
            txtRackAncho.TabIndex = 12;
            // 
            // lblRackAlto
            // 
            lblRackAlto.Location = new Point(20, 263);
            lblRackAlto.Name = "lblRackAlto";
            lblRackAlto.Size = new Size(136, 23);
            lblRackAlto.TabIndex = 13;
            lblRackAlto.Text = "Alto Rack (mm)";
            // 
            // txtRackAlto
            // 
            txtRackAlto.Location = new Point(180, 263);
            txtRackAlto.Name = "txtRackAlto";
            txtRackAlto.Size = new Size(150, 27);
            txtRackAlto.TabIndex = 14;
            // 
            // lblRackUnidades
            // 
            lblRackUnidades.Location = new Point(20, 293);
            lblRackUnidades.Name = "lblRackUnidades";
            lblRackUnidades.Size = new Size(136, 23);
            lblRackUnidades.TabIndex = 15;
            lblRackUnidades.Text = "Unidades";
            // 
            // txtRackUnidades
            // 
            txtRackUnidades.Location = new Point(180, 293);
            txtRackUnidades.Name = "txtRackUnidades";
            txtRackUnidades.Size = new Size(150, 27);
            txtRackUnidades.TabIndex = 16;
            // 
            // btnAgregarRack
            // 
            btnAgregarRack.Location = new Point(20, 323);
            btnAgregarRack.Name = "btnAgregarRack";
            btnAgregarRack.Size = new Size(150, 30);
            btnAgregarRack.TabIndex = 17;
            btnAgregarRack.Text = "Agregar Rack";
            btnAgregarRack.Click += btnAgregarRack_Click;
            // 
            // btnMostrarResumen
            // 
            btnMostrarResumen.Location = new Point(20, 410);
            btnMostrarResumen.Name = "btnMostrarResumen";
            btnMostrarResumen.Size = new Size(150, 30);
            btnMostrarResumen.TabIndex = 18;
            btnMostrarResumen.Text = "Mostrar Resumen";
            btnMostrarResumen.Click += btnMostrarResumen_Click;
            // 
            // btnMostrarDiagrama
            // 
            btnMostrarDiagrama.Location = new Point(20, 445);
            btnMostrarDiagrama.Name = "btnMostrarDiagrama";
            btnMostrarDiagrama.Size = new Size(150, 30);
            btnMostrarDiagrama.TabIndex = 19;
            btnMostrarDiagrama.Text = "Mostrar Diagrama";
            btnMostrarDiagrama.Click += btnMostrarDiagrama_Click;
            // 
            // btnLimpiarRack
            // 
            btnLimpiarRack.Location = new Point(20, 359);
            btnLimpiarRack.Name = "btnLimpiarRack";
            btnLimpiarRack.Size = new Size(150, 30);
            btnLimpiarRack.TabIndex = 20;
            btnLimpiarRack.Text = "Limpiar Datos Rack";
            btnLimpiarRack.Click += btnLimpiarRack_Click;
            // 
            // btnNuevo
            // 
            btnNuevo.Location = new Point(20, 8);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(150, 30);
            btnNuevo.TabIndex = 21;
            btnNuevo.Text = "Nuevo";
            btnNuevo.Click += btnNuevo_Click;
            // 
            // rtbResultado
            // 
            rtbResultado.Dock = DockStyle.Fill;
            rtbResultado.Font = new Font("Consolas", 10F);
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
            Text = "Cálculo de Racks en Tráiler (mm)";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
