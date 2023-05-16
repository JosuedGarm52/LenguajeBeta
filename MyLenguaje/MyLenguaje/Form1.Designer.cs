namespace MyLenguaje
{
	partial class Form1
	{
		/// <summary>
		/// Variable del diseñador necesaria.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Limpiar los recursos que se estén usando.
		/// </summary>
		/// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Código generado por el Diseñador de Windows Forms

		/// <summary>
		/// Método necesario para admitir el Diseñador. No se puede modificar
		/// el contenido de este método con el editor de código.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnSalir = new System.Windows.Forms.Button();
			this.rchTexto = new System.Windows.Forms.RichTextBox();
			this.rchLexico = new System.Windows.Forms.RichTextBox();
			this.rchSintactico = new System.Windows.Forms.RichTextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label9 = new System.Windows.Forms.Label();
			this.rchSemantico = new System.Windows.Forms.RichTextBox();
			this.btnAnalizar = new System.Windows.Forms.Button();
			this.btnLexicar = new System.Windows.Forms.Button();
			this.btnSintactizar = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.btnAjustar = new System.Windows.Forms.Button();
			this.chbMensaje = new System.Windows.Forms.CheckBox();
			this.btnLimpiar = new System.Windows.Forms.Button();
			this.btnRellenar = new System.Windows.Forms.Button();
			this.btnComprobar = new System.Windows.Forms.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label10 = new System.Windows.Forms.Label();
			this.rchConsola4 = new System.Windows.Forms.RichTextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.rchConsola3 = new System.Windows.Forms.RichTextBox();
			this.rchConsola2 = new System.Windows.Forms.RichTextBox();
			this.rchConsola1 = new System.Windows.Forms.RichTextBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.rchAnalisisSintactico = new System.Windows.Forms.RichTextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.rhcAnalisisLexico = new System.Windows.Forms.RichTextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.dtgSimbolo = new System.Windows.Forms.DataGridView();
			this.panelCodigo = new System.Windows.Forms.Panel();
			this.panelCaja = new System.Windows.Forms.Panel();
			this.panelBotones = new System.Windows.Forms.Panel();
			this.btnBiblioteca = new System.Windows.Forms.Button();
			this.btnPPrueba = new System.Windows.Forms.Button();
			this.dtgColSimboloID = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dtgColSimboloVariable = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dtgColSimboloTipoDato = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dtgColToken = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dtgColValor = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dtgColTokenUnico = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dtgColFila = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.rchAnalisisSemantica = new System.Windows.Forms.RichTextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox5.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dtgSimbolo)).BeginInit();
			this.panelCodigo.SuspendLayout();
			this.panelCaja.SuspendLayout();
			this.panelBotones.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnSalir
			// 
			this.btnSalir.Location = new System.Drawing.Point(35, 45);
			this.btnSalir.Name = "btnSalir";
			this.btnSalir.Size = new System.Drawing.Size(75, 23);
			this.btnSalir.TabIndex = 0;
			this.btnSalir.Text = "Salir";
			this.btnSalir.UseVisualStyleBackColor = true;
			this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
			// 
			// rchTexto
			// 
			this.rchTexto.AcceptsTab = true;
			this.rchTexto.BackColor = System.Drawing.SystemColors.InactiveCaption;
			this.rchTexto.Location = new System.Drawing.Point(6, 37);
			this.rchTexto.Name = "rchTexto";
			this.rchTexto.Size = new System.Drawing.Size(306, 211);
			this.rchTexto.TabIndex = 1;
			this.rchTexto.Text = "";
			this.rchTexto.WordWrap = false;
			this.rchTexto.TextChanged += new System.EventHandler(this.rchTexto_TextChanged);
			// 
			// rchLexico
			// 
			this.rchLexico.BackColor = System.Drawing.SystemColors.MenuHighlight;
			this.rchLexico.Location = new System.Drawing.Point(330, 37);
			this.rchLexico.Name = "rchLexico";
			this.rchLexico.ReadOnly = true;
			this.rchLexico.Size = new System.Drawing.Size(265, 211);
			this.rchLexico.TabIndex = 2;
			this.rchLexico.Text = "";
			this.rchLexico.WordWrap = false;
			// 
			// rchSintactico
			// 
			this.rchSintactico.BackColor = System.Drawing.SystemColors.MenuHighlight;
			this.rchSintactico.Location = new System.Drawing.Point(601, 37);
			this.rchSintactico.Name = "rchSintactico";
			this.rchSintactico.ReadOnly = true;
			this.rchSintactico.Size = new System.Drawing.Size(265, 211);
			this.rchSintactico.TabIndex = 3;
			this.rchSintactico.Text = "";
			this.rchSintactico.WordWrap = false;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(387, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(114, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Transformacion Lexico";
			this.label1.Click += new System.EventHandler(this.label1_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(670, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(130, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "Transformacion Sintactico";
			this.label2.Click += new System.EventHandler(this.label2_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(87, 16);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(76, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "Tab de codigo";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label9);
			this.groupBox1.Controls.Add(this.rchSemantico);
			this.groupBox1.Controls.Add(this.rchTexto);
			this.groupBox1.Controls.Add(this.rchSintactico);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.rchLexico);
			this.groupBox1.Location = new System.Drawing.Point(5, 10);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(1152, 258);
			this.groupBox1.TabIndex = 7;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Tab de codificacion y transformacion";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(926, 16);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(133, 13);
			this.label9.TabIndex = 8;
			this.label9.Text = "Transformacion Semantico";
			// 
			// rchSemantico
			// 
			this.rchSemantico.BackColor = System.Drawing.SystemColors.MenuHighlight;
			this.rchSemantico.Location = new System.Drawing.Point(872, 36);
			this.rchSemantico.Name = "rchSemantico";
			this.rchSemantico.ReadOnly = true;
			this.rchSemantico.Size = new System.Drawing.Size(265, 211);
			this.rchSemantico.TabIndex = 7;
			this.rchSemantico.Text = "";
			this.rchSemantico.WordWrap = false;
			// 
			// btnAnalizar
			// 
			this.btnAnalizar.Location = new System.Drawing.Point(25, 16);
			this.btnAnalizar.Name = "btnAnalizar";
			this.btnAnalizar.Size = new System.Drawing.Size(95, 23);
			this.btnAnalizar.TabIndex = 8;
			this.btnAnalizar.Text = "Analizar->Lexico";
			this.btnAnalizar.UseVisualStyleBackColor = true;
			this.btnAnalizar.Click += new System.EventHandler(this.btnAnalizar_Click);
			// 
			// btnLexicar
			// 
			this.btnLexicar.Location = new System.Drawing.Point(201, 16);
			this.btnLexicar.Name = "btnLexicar";
			this.btnLexicar.Size = new System.Drawing.Size(110, 23);
			this.btnLexicar.TabIndex = 9;
			this.btnLexicar.Text = "Lexico -> sintactico";
			this.btnLexicar.UseVisualStyleBackColor = true;
			this.btnLexicar.Click += new System.EventHandler(this.btnLexicar_Click);
			// 
			// btnSintactizar
			// 
			this.btnSintactizar.Location = new System.Drawing.Point(353, 19);
			this.btnSintactizar.Name = "btnSintactizar";
			this.btnSintactizar.Size = new System.Drawing.Size(130, 23);
			this.btnSintactizar.TabIndex = 10;
			this.btnSintactizar.Text = "Sintactico->Semantico";
			this.btnSintactizar.UseVisualStyleBackColor = true;
			this.btnSintactizar.Click += new System.EventHandler(this.btnSintactizar_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.btnPPrueba);
			this.groupBox2.Controls.Add(this.btnBiblioteca);
			this.groupBox2.Controls.Add(this.btnAjustar);
			this.groupBox2.Controls.Add(this.chbMensaje);
			this.groupBox2.Controls.Add(this.btnLimpiar);
			this.groupBox2.Controls.Add(this.btnRellenar);
			this.groupBox2.Controls.Add(this.btnComprobar);
			this.groupBox2.Controls.Add(this.btnAnalizar);
			this.groupBox2.Controls.Add(this.btnSintactizar);
			this.groupBox2.Controls.Add(this.btnSalir);
			this.groupBox2.Controls.Add(this.btnLexicar);
			this.groupBox2.Location = new System.Drawing.Point(15, 3);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(823, 108);
			this.groupBox2.TabIndex = 11;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Botones";
			this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
			// 
			// btnAjustar
			// 
			this.btnAjustar.Location = new System.Drawing.Point(353, 72);
			this.btnAjustar.Name = "btnAjustar";
			this.btnAjustar.Size = new System.Drawing.Size(130, 23);
			this.btnAjustar.TabIndex = 15;
			this.btnAjustar.Text = "Ajustar Tab Control";
			this.btnAjustar.UseVisualStyleBackColor = true;
			this.btnAjustar.Click += new System.EventHandler(this.btnAjustar_Click);
			// 
			// chbMensaje
			// 
			this.chbMensaje.AutoSize = true;
			this.chbMensaje.Location = new System.Drawing.Point(201, 49);
			this.chbMensaje.Name = "chbMensaje";
			this.chbMensaje.Size = new System.Drawing.Size(110, 17);
			this.chbMensaje.TabIndex = 14;
			this.chbMensaje.Text = "Proceso de lexico";
			this.chbMensaje.UseVisualStyleBackColor = true;
			this.chbMensaje.CheckedChanged += new System.EventHandler(this.chbMensaje_CheckedChanged);
			// 
			// btnLimpiar
			// 
			this.btnLimpiar.Location = new System.Drawing.Point(35, 77);
			this.btnLimpiar.Name = "btnLimpiar";
			this.btnLimpiar.Size = new System.Drawing.Size(75, 23);
			this.btnLimpiar.TabIndex = 13;
			this.btnLimpiar.Text = "Limpiar todo";
			this.btnLimpiar.UseVisualStyleBackColor = true;
			this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
			// 
			// btnRellenar
			// 
			this.btnRellenar.Location = new System.Drawing.Point(353, 45);
			this.btnRellenar.Name = "btnRellenar";
			this.btnRellenar.Size = new System.Drawing.Size(130, 23);
			this.btnRellenar.TabIndex = 12;
			this.btnRellenar.Text = "Rellenar w/palabras";
			this.btnRellenar.UseVisualStyleBackColor = true;
			this.btnRellenar.Click += new System.EventHandler(this.btnRellenar_Click);
			// 
			// btnComprobar
			// 
			this.btnComprobar.Location = new System.Drawing.Point(215, 72);
			this.btnComprobar.Name = "btnComprobar";
			this.btnComprobar.Size = new System.Drawing.Size(75, 23);
			this.btnComprobar.TabIndex = 11;
			this.btnComprobar.Text = "Comprobar";
			this.btnComprobar.UseVisualStyleBackColor = true;
			this.btnComprobar.Click += new System.EventHandler(this.btnComprobar_Click);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.label10);
			this.groupBox3.Controls.Add(this.rchConsola4);
			this.groupBox3.Controls.Add(this.label6);
			this.groupBox3.Controls.Add(this.label5);
			this.groupBox3.Controls.Add(this.label4);
			this.groupBox3.Controls.Add(this.rchConsola3);
			this.groupBox3.Controls.Add(this.rchConsola2);
			this.groupBox3.Controls.Add(this.rchConsola1);
			this.groupBox3.Location = new System.Drawing.Point(11, 267);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(1108, 226);
			this.groupBox3.TabIndex = 12;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Tab Consola";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(930, 31);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(51, 13);
			this.label10.TabIndex = 7;
			this.label10.Text = "Analisis 4";
			// 
			// rchConsola4
			// 
			this.rchConsola4.BackColor = System.Drawing.SystemColors.HotTrack;
			this.rchConsola4.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.rchConsola4.Location = new System.Drawing.Point(833, 51);
			this.rchConsola4.Name = "rchConsola4";
			this.rchConsola4.ReadOnly = true;
			this.rchConsola4.Size = new System.Drawing.Size(268, 169);
			this.rchConsola4.TabIndex = 6;
			this.rchConsola4.Text = "";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(652, 31);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(51, 13);
			this.label6.TabIndex = 5;
			this.label6.Text = "Analisis 3";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(372, 32);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(51, 13);
			this.label5.TabIndex = 4;
			this.label5.Text = "Analisis 2";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(100, 32);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(51, 13);
			this.label4.TabIndex = 3;
			this.label4.Text = "Analisis 1";
			// 
			// rchConsola3
			// 
			this.rchConsola3.BackColor = System.Drawing.SystemColors.HotTrack;
			this.rchConsola3.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.rchConsola3.Location = new System.Drawing.Point(555, 51);
			this.rchConsola3.Name = "rchConsola3";
			this.rchConsola3.ReadOnly = true;
			this.rchConsola3.Size = new System.Drawing.Size(268, 169);
			this.rchConsola3.TabIndex = 2;
			this.rchConsola3.Text = "";
			// 
			// rchConsola2
			// 
			this.rchConsola2.BackColor = System.Drawing.SystemColors.HotTrack;
			this.rchConsola2.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.rchConsola2.Location = new System.Drawing.Point(281, 51);
			this.rchConsola2.Name = "rchConsola2";
			this.rchConsola2.ReadOnly = true;
			this.rchConsola2.Size = new System.Drawing.Size(268, 169);
			this.rchConsola2.TabIndex = 1;
			this.rchConsola2.Text = "";
			// 
			// rchConsola1
			// 
			this.rchConsola1.BackColor = System.Drawing.SystemColors.HotTrack;
			this.rchConsola1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.rchConsola1.Location = new System.Drawing.Point(6, 51);
			this.rchConsola1.Name = "rchConsola1";
			this.rchConsola1.ReadOnly = true;
			this.rchConsola1.Size = new System.Drawing.Size(268, 169);
			this.rchConsola1.TabIndex = 0;
			this.rchConsola1.Text = "";
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.label11);
			this.groupBox4.Controls.Add(this.rchAnalisisSemantica);
			this.groupBox4.Controls.Add(this.rchAnalisisSintactico);
			this.groupBox4.Controls.Add(this.label8);
			this.groupBox4.Controls.Add(this.rhcAnalisisLexico);
			this.groupBox4.Controls.Add(this.label7);
			this.groupBox4.Location = new System.Drawing.Point(3, 10);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(585, 266);
			this.groupBox4.TabIndex = 13;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Caja de analisis";
			// 
			// rchAnalisisSintactico
			// 
			this.rchAnalisisSintactico.BackColor = System.Drawing.Color.Orange;
			this.rchAnalisisSintactico.Location = new System.Drawing.Point(201, 49);
			this.rchAnalisisSintactico.Name = "rchAnalisisSintactico";
			this.rchAnalisisSintactico.ReadOnly = true;
			this.rchAnalisisSintactico.Size = new System.Drawing.Size(184, 202);
			this.rchAnalisisSintactico.TabIndex = 3;
			this.rchAnalisisSintactico.Text = "";
			this.rchAnalisisSintactico.WordWrap = false;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(198, 20);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(54, 13);
			this.label8.TabIndex = 2;
			this.label8.Text = "Sintactico";
			this.label8.Click += new System.EventHandler(this.label8_Click);
			// 
			// rhcAnalisisLexico
			// 
			this.rhcAnalisisLexico.BackColor = System.Drawing.Color.Orange;
			this.rhcAnalisisLexico.Location = new System.Drawing.Point(10, 49);
			this.rhcAnalisisLexico.Name = "rhcAnalisisLexico";
			this.rhcAnalisisLexico.Size = new System.Drawing.Size(184, 199);
			this.rhcAnalisisLexico.TabIndex = 1;
			this.rhcAnalisisLexico.Text = "";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(7, 20);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(38, 13);
			this.label7.TabIndex = 0;
			this.label7.Text = "Lexico";
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.dtgSimbolo);
			this.groupBox5.Location = new System.Drawing.Point(13, 277);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(763, 335);
			this.groupBox5.TabIndex = 14;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Tabla de simbolos";
			// 
			// dtgSimbolo
			// 
			this.dtgSimbolo.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dtgSimbolo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dtgSimbolo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dtgColSimboloID,
            this.dtgColSimboloVariable,
            this.dtgColSimboloTipoDato,
            this.dtgColToken,
            this.dtgColValor,
            this.dtgColTokenUnico,
            this.dtgColFila});
			this.dtgSimbolo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dtgSimbolo.Location = new System.Drawing.Point(3, 16);
			this.dtgSimbolo.Name = "dtgSimbolo";
			this.dtgSimbolo.Size = new System.Drawing.Size(757, 316);
			this.dtgSimbolo.TabIndex = 0;
			// 
			// panelCodigo
			// 
			this.panelCodigo.AutoScroll = true;
			this.panelCodigo.Controls.Add(this.groupBox1);
			this.panelCodigo.Controls.Add(this.groupBox3);
			this.panelCodigo.Location = new System.Drawing.Point(12, 2);
			this.panelCodigo.Name = "panelCodigo";
			this.panelCodigo.Size = new System.Drawing.Size(768, 503);
			this.panelCodigo.TabIndex = 6;
			// 
			// panelCaja
			// 
			this.panelCaja.AutoScroll = true;
			this.panelCaja.Controls.Add(this.groupBox4);
			this.panelCaja.Controls.Add(this.groupBox5);
			this.panelCaja.Location = new System.Drawing.Point(786, 2);
			this.panelCaja.Name = "panelCaja";
			this.panelCaja.Size = new System.Drawing.Size(421, 638);
			this.panelCaja.TabIndex = 15;
			// 
			// panelBotones
			// 
			this.panelBotones.AutoScroll = true;
			this.panelBotones.Controls.Add(this.groupBox2);
			this.panelBotones.Location = new System.Drawing.Point(3, 511);
			this.panelBotones.Name = "panelBotones";
			this.panelBotones.Size = new System.Drawing.Size(777, 129);
			this.panelBotones.TabIndex = 16;
			// 
			// btnBiblioteca
			// 
			this.btnBiblioteca.Location = new System.Drawing.Point(515, 72);
			this.btnBiblioteca.Name = "btnBiblioteca";
			this.btnBiblioteca.Size = new System.Drawing.Size(130, 23);
			this.btnBiblioteca.TabIndex = 16;
			this.btnBiblioteca.Text = "Biblioteca de codigo";
			this.btnBiblioteca.UseVisualStyleBackColor = true;
			this.btnBiblioteca.Click += new System.EventHandler(this.btnBiblioteca_Click);
			// 
			// btnPPrueba
			// 
			this.btnPPrueba.Location = new System.Drawing.Point(515, 43);
			this.btnPPrueba.Name = "btnPPrueba";
			this.btnPPrueba.Size = new System.Drawing.Size(130, 23);
			this.btnPPrueba.TabIndex = 17;
			this.btnPPrueba.Text = "Prueba";
			this.btnPPrueba.UseVisualStyleBackColor = true;
			this.btnPPrueba.Click += new System.EventHandler(this.btnPPrueba_Click);
			// 
			// dtgColSimboloID
			// 
			this.dtgColSimboloID.FillWeight = 50F;
			this.dtgColSimboloID.HeaderText = "ID";
			this.dtgColSimboloID.MaxInputLength = 6;
			this.dtgColSimboloID.MinimumWidth = 30;
			this.dtgColSimboloID.Name = "dtgColSimboloID";
			this.dtgColSimboloID.ReadOnly = true;
			// 
			// dtgColSimboloVariable
			// 
			this.dtgColSimboloVariable.FillWeight = 87.05585F;
			this.dtgColSimboloVariable.HeaderText = "Variable";
			this.dtgColSimboloVariable.Name = "dtgColSimboloVariable";
			this.dtgColSimboloVariable.ReadOnly = true;
			// 
			// dtgColSimboloTipoDato
			// 
			this.dtgColSimboloTipoDato.FillWeight = 87.05585F;
			this.dtgColSimboloTipoDato.HeaderText = "TipoDato";
			this.dtgColSimboloTipoDato.Name = "dtgColSimboloTipoDato";
			this.dtgColSimboloTipoDato.ReadOnly = true;
			// 
			// dtgColToken
			// 
			this.dtgColToken.FillWeight = 87.05585F;
			this.dtgColToken.HeaderText = "Token";
			this.dtgColToken.Name = "dtgColToken";
			// 
			// dtgColValor
			// 
			this.dtgColValor.FillWeight = 87.05585F;
			this.dtgColValor.HeaderText = "Valor";
			this.dtgColValor.Name = "dtgColValor";
			// 
			// dtgColTokenUnico
			// 
			this.dtgColTokenUnico.FillWeight = 87.05585F;
			this.dtgColTokenUnico.HeaderText = "TokenUnico";
			this.dtgColTokenUnico.Name = "dtgColTokenUnico";
			// 
			// dtgColFila
			// 
			this.dtgColFila.FillWeight = 87.05585F;
			this.dtgColFila.HeaderText = "Fila";
			this.dtgColFila.Name = "dtgColFila";
			this.dtgColFila.ReadOnly = true;
			// 
			// rchAnalisisSemantica
			// 
			this.rchAnalisisSemantica.BackColor = System.Drawing.Color.Orange;
			this.rchAnalisisSemantica.Location = new System.Drawing.Point(391, 49);
			this.rchAnalisisSemantica.Name = "rchAnalisisSemantica";
			this.rchAnalisisSemantica.ReadOnly = true;
			this.rchAnalisisSemantica.Size = new System.Drawing.Size(184, 202);
			this.rchAnalisisSemantica.TabIndex = 4;
			this.rchAnalisisSemantica.Text = "";
			this.rchAnalisisSemantica.WordWrap = false;
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(399, 20);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(54, 13);
			this.label11.TabIndex = 5;
			this.label11.Text = "Sintactico";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(1229, 648);
			this.Controls.Add(this.panelBotones);
			this.Controls.Add(this.panelCaja);
			this.Controls.Add(this.panelCodigo);
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Analisis de lenguaje";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.groupBox5.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dtgSimbolo)).EndInit();
			this.panelCodigo.ResumeLayout(false);
			this.panelCaja.ResumeLayout(false);
			this.panelBotones.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnSalir;
		private System.Windows.Forms.RichTextBox rchTexto;
		private System.Windows.Forms.RichTextBox rchLexico;
		private System.Windows.Forms.RichTextBox rchSintactico;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button btnAnalizar;
		private System.Windows.Forms.Button btnLexicar;
		private System.Windows.Forms.Button btnSintactizar;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.RichTextBox rchConsola3;
		private System.Windows.Forms.RichTextBox rchConsola2;
		private System.Windows.Forms.RichTextBox rchConsola1;
		private System.Windows.Forms.Button btnComprobar;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.RichTextBox rhcAnalisisLexico;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Button btnRellenar;
		private System.Windows.Forms.Button btnLimpiar;
		private System.Windows.Forms.RichTextBox rchAnalisisSintactico;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.CheckBox chbMensaje;
		private System.Windows.Forms.Button btnAjustar;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.DataGridView dtgSimbolo;
		private System.Windows.Forms.Panel panelCodigo;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.RichTextBox rchSemantico;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.RichTextBox rchConsola4;
		private System.Windows.Forms.Panel panelCaja;
		private System.Windows.Forms.Panel panelBotones;
		private System.Windows.Forms.Button btnBiblioteca;
		private System.Windows.Forms.Button btnPPrueba;
		private System.Windows.Forms.DataGridViewTextBoxColumn dtgColSimboloID;
		private System.Windows.Forms.DataGridViewTextBoxColumn dtgColSimboloVariable;
		private System.Windows.Forms.DataGridViewTextBoxColumn dtgColSimboloTipoDato;
		private System.Windows.Forms.DataGridViewTextBoxColumn dtgColToken;
		private System.Windows.Forms.DataGridViewTextBoxColumn dtgColValor;
		private System.Windows.Forms.DataGridViewTextBoxColumn dtgColTokenUnico;
		private System.Windows.Forms.DataGridViewTextBoxColumn dtgColFila;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.RichTextBox rchAnalisisSemantica;
	}
}

