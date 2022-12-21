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
			this.btnAnalizar = new System.Windows.Forms.Button();
			this.btnLexicar = new System.Windows.Forms.Button();
			this.btnSintactizar = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.btnComprobar = new System.Windows.Forms.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.rchConsola3 = new System.Windows.Forms.RichTextBox();
			this.rchConsola2 = new System.Windows.Forms.RichTextBox();
			this.rchConsola1 = new System.Windows.Forms.RichTextBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.rhcAnalisisLexico = new System.Windows.Forms.RichTextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.btnRellenar = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnSalir
			// 
			this.btnSalir.Location = new System.Drawing.Point(76, 48);
			this.btnSalir.Name = "btnSalir";
			this.btnSalir.Size = new System.Drawing.Size(75, 23);
			this.btnSalir.TabIndex = 0;
			this.btnSalir.Text = "Salir";
			this.btnSalir.UseVisualStyleBackColor = true;
			this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
			// 
			// rchTexto
			// 
			this.rchTexto.BackColor = System.Drawing.SystemColors.InactiveCaption;
			this.rchTexto.Location = new System.Drawing.Point(6, 37);
			this.rchTexto.Name = "rchTexto";
			this.rchTexto.Size = new System.Drawing.Size(306, 211);
			this.rchTexto.TabIndex = 1;
			this.rchTexto.Text = "";
			this.rchTexto.TextChanged += new System.EventHandler(this.rchTexto_TextChanged);
			// 
			// rchLexico
			// 
			this.rchLexico.BackColor = System.Drawing.SystemColors.MenuHighlight;
			this.rchLexico.Location = new System.Drawing.Point(327, 37);
			this.rchLexico.Name = "rchLexico";
			this.rchLexico.ReadOnly = true;
			this.rchLexico.Size = new System.Drawing.Size(247, 211);
			this.rchLexico.TabIndex = 2;
			this.rchLexico.Text = "";
			// 
			// rchSintactico
			// 
			this.rchSintactico.BackColor = System.Drawing.SystemColors.MenuHighlight;
			this.rchSintactico.Location = new System.Drawing.Point(588, 37);
			this.rchSintactico.Name = "rchSintactico";
			this.rchSintactico.ReadOnly = true;
			this.rchSintactico.Size = new System.Drawing.Size(269, 211);
			this.rchSintactico.TabIndex = 3;
			this.rchSintactico.Text = "";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(388, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(114, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Transformacion Lexico";
			this.label1.Click += new System.EventHandler(this.label1_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(585, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(130, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "Transformacion Sintactico";
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
			this.groupBox1.Controls.Add(this.rchTexto);
			this.groupBox1.Controls.Add(this.rchSintactico);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.rchLexico);
			this.groupBox1.Location = new System.Drawing.Point(18, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(870, 260);
			this.groupBox1.TabIndex = 7;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Tab de codificacion y transformacion";
			// 
			// btnAnalizar
			// 
			this.btnAnalizar.Location = new System.Drawing.Point(76, 16);
			this.btnAnalizar.Name = "btnAnalizar";
			this.btnAnalizar.Size = new System.Drawing.Size(75, 23);
			this.btnAnalizar.TabIndex = 8;
			this.btnAnalizar.Text = "Analizar";
			this.btnAnalizar.UseVisualStyleBackColor = true;
			this.btnAnalizar.Click += new System.EventHandler(this.btnAnalizar_Click);
			// 
			// btnLexicar
			// 
			this.btnLexicar.Location = new System.Drawing.Point(375, 19);
			this.btnLexicar.Name = "btnLexicar";
			this.btnLexicar.Size = new System.Drawing.Size(75, 23);
			this.btnLexicar.TabIndex = 9;
			this.btnLexicar.Text = "Lexicar";
			this.btnLexicar.UseVisualStyleBackColor = true;
			this.btnLexicar.Click += new System.EventHandler(this.btnLexicar_Click);
			// 
			// btnSintactizar
			// 
			this.btnSintactizar.Location = new System.Drawing.Point(655, 19);
			this.btnSintactizar.Name = "btnSintactizar";
			this.btnSintactizar.Size = new System.Drawing.Size(75, 23);
			this.btnSintactizar.TabIndex = 10;
			this.btnSintactizar.Text = "Sintactizar";
			this.btnSintactizar.UseVisualStyleBackColor = true;
			this.btnSintactizar.Click += new System.EventHandler(this.btnSintactizar_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.btnRellenar);
			this.groupBox2.Controls.Add(this.btnComprobar);
			this.groupBox2.Controls.Add(this.btnAnalizar);
			this.groupBox2.Controls.Add(this.btnSintactizar);
			this.groupBox2.Controls.Add(this.btnSalir);
			this.groupBox2.Controls.Add(this.btnLexicar);
			this.groupBox2.Location = new System.Drawing.Point(18, 511);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(823, 89);
			this.groupBox2.TabIndex = 11;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Botones";
			// 
			// btnComprobar
			// 
			this.btnComprobar.Location = new System.Drawing.Point(375, 47);
			this.btnComprobar.Name = "btnComprobar";
			this.btnComprobar.Size = new System.Drawing.Size(75, 23);
			this.btnComprobar.TabIndex = 11;
			this.btnComprobar.Text = "Comprobar";
			this.btnComprobar.UseVisualStyleBackColor = true;
			this.btnComprobar.Click += new System.EventHandler(this.btnComprobar_Click);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.label6);
			this.groupBox3.Controls.Add(this.label5);
			this.groupBox3.Controls.Add(this.label4);
			this.groupBox3.Controls.Add(this.rchConsola3);
			this.groupBox3.Controls.Add(this.rchConsola2);
			this.groupBox3.Controls.Add(this.rchConsola1);
			this.groupBox3.Location = new System.Drawing.Point(18, 279);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(870, 226);
			this.groupBox3.TabIndex = 12;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Tab Consola";
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
			this.groupBox4.Controls.Add(this.rhcAnalisisLexico);
			this.groupBox4.Controls.Add(this.label7);
			this.groupBox4.Location = new System.Drawing.Point(895, 13);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(200, 269);
			this.groupBox4.TabIndex = 13;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Caja de analisis";
			// 
			// rhcAnalisisLexico
			// 
			this.rhcAnalisisLexico.BackColor = System.Drawing.Color.Orange;
			this.rhcAnalisisLexico.Location = new System.Drawing.Point(10, 49);
			this.rhcAnalisisLexico.Name = "rhcAnalisisLexico";
			this.rhcAnalisisLexico.Size = new System.Drawing.Size(184, 128);
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
			// btnRellenar
			// 
			this.btnRellenar.Location = new System.Drawing.Point(588, 48);
			this.btnRellenar.Name = "btnRellenar";
			this.btnRellenar.Size = new System.Drawing.Size(203, 23);
			this.btnRellenar.TabIndex = 12;
			this.btnRellenar.Text = "Rellenar w/palabras";
			this.btnRellenar.UseVisualStyleBackColor = true;
			this.btnRellenar.Click += new System.EventHandler(this.btnRellenar_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(1307, 608);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Name = "Form1";
			this.Text = "Analisis de lenguaje";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
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
	}
}

