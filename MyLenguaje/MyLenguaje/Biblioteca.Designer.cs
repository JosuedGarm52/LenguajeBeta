namespace MyLenguaje
{
	partial class Biblioteca
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.dtgBiblioteca = new System.Windows.Forms.DataGridView();
			this.dtgNumero = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dtgPalabraReservada = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dtgTraduccion = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.bntInsertar = new System.Windows.Forms.Button();
			this.btnRegresar = new System.Windows.Forms.Button();
			this.lblPalabra = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dtgBiblioteca)).BeginInit();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.dtgBiblioteca);
			this.groupBox1.Location = new System.Drawing.Point(13, 13);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(313, 217);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Biblioteca de codigo";
			// 
			// dtgBiblioteca
			// 
			this.dtgBiblioteca.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dtgBiblioteca.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dtgNumero,
            this.dtgPalabraReservada,
            this.dtgTraduccion});
			this.dtgBiblioteca.Location = new System.Drawing.Point(7, 20);
			this.dtgBiblioteca.Name = "dtgBiblioteca";
			this.dtgBiblioteca.Size = new System.Drawing.Size(296, 179);
			this.dtgBiblioteca.TabIndex = 0;
			this.dtgBiblioteca.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DtgBiblioteca_CellClick);
			this.dtgBiblioteca.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dtgBiblioteca_CellContentClick);
			// 
			// dtgNumero
			// 
			this.dtgNumero.HeaderText = "Numero";
			this.dtgNumero.Name = "dtgNumero";
			this.dtgNumero.ReadOnly = true;
			this.dtgNumero.Width = 50;
			// 
			// dtgPalabraReservada
			// 
			this.dtgPalabraReservada.HeaderText = "Palab reserv";
			this.dtgPalabraReservada.Name = "dtgPalabraReservada";
			this.dtgPalabraReservada.ReadOnly = true;
			// 
			// dtgTraduccion
			// 
			this.dtgTraduccion.HeaderText = "Traduccion";
			this.dtgTraduccion.Name = "dtgTraduccion";
			this.dtgTraduccion.ReadOnly = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.btnRegresar);
			this.groupBox2.Controls.Add(this.bntInsertar);
			this.groupBox2.Location = new System.Drawing.Point(8, 236);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(214, 51);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Botones";
			// 
			// bntInsertar
			// 
			this.bntInsertar.Location = new System.Drawing.Point(12, 22);
			this.bntInsertar.Name = "bntInsertar";
			this.bntInsertar.Size = new System.Drawing.Size(75, 23);
			this.bntInsertar.TabIndex = 0;
			this.bntInsertar.Text = "Insertar";
			this.bntInsertar.UseVisualStyleBackColor = true;
			this.bntInsertar.Click += new System.EventHandler(this.bntInsertar_Click);
			// 
			// btnRegresar
			// 
			this.btnRegresar.Location = new System.Drawing.Point(120, 22);
			this.btnRegresar.Name = "btnRegresar";
			this.btnRegresar.Size = new System.Drawing.Size(75, 23);
			this.btnRegresar.TabIndex = 1;
			this.btnRegresar.Text = "Regresar";
			this.btnRegresar.UseVisualStyleBackColor = true;
			this.btnRegresar.Click += new System.EventHandler(this.btnRegresar_Click);
			// 
			// lblPalabra
			// 
			this.lblPalabra.AutoSize = true;
			this.lblPalabra.ForeColor = System.Drawing.SystemColors.HotTrack;
			this.lblPalabra.Location = new System.Drawing.Point(229, 258);
			this.lblPalabra.Name = "lblPalabra";
			this.lblPalabra.Size = new System.Drawing.Size(34, 13);
			this.lblPalabra.TabIndex = 2;
			this.lblPalabra.Text = "CXFA";
			// 
			// Biblioteca
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(333, 299);
			this.Controls.Add(this.lblPalabra);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Name = "Biblioteca";
			this.Text = "Biblioteca de codigo";
			this.Load += new System.EventHandler(this.Biblioteca_Load);
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dtgBiblioteca)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.DataGridView dtgBiblioteca;
		private System.Windows.Forms.DataGridViewTextBoxColumn dtgNumero;
		private System.Windows.Forms.DataGridViewTextBoxColumn dtgPalabraReservada;
		private System.Windows.Forms.DataGridViewTextBoxColumn dtgTraduccion;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button btnRegresar;
		private System.Windows.Forms.Button bntInsertar;
		private System.Windows.Forms.Label lblPalabra;
	}
}