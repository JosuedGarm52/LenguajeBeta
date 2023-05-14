using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyLenguaje
{
	public partial class Biblioteca : Form
	{
		public Biblioteca()
		{
			InitializeComponent();
		}

		private void Biblioteca_Load(object sender, EventArgs e)
		{
			llenarDTG(1, "CXFA", "MAIN");
			llenarDTG(2, "KLA", "CLASS");
			llenarDTG(3, "ENT", "INT");
			llenarDTG(4, "KAR", "CHAR");
			llenarDTG(5, "LOG", "BOOL");
			llenarDTG(6, "MAT", "ARRAY");
			llenarDTG(7, "REA", "DOUBLE");
			llenarDTG(8, "SXN", "STRING");
			llenarDTG(9, "OBJ", "OBJECT");
			llenarDTG(10, "SE", "IF");
			llenarDTG(11, "DAR", "CONTINUE");
			llenarDTG(12, "SKR", "WRITE");
			llenarDTG(13, "DUM", "WHILE");
			llenarDTG(14, "FAR", "DO");
			llenarDTG(15, "KAZ SXA ROM", "CASE SWITCH BREAK");
			llenarDTG(16, "KAP", "CAPTURE");
			llenarDTG(17, "LEG", "READ");
			llenarDTG(18, "POR", "FOR");
			llenarDTG(19, "PRX", "FOREACH");
			llenarDTG(20, "MTDX", "METODO");
			llenarDTG(21, "LA", "AND");
			llenarDTG(22, "LN", "NOT");
			llenarDTG(23, "LO", "OR");
			llenarDTG(24, "RSQ", ">");
			llenarDTG(25, "RSI", ">=");
			llenarDTG(26, "RD", "!=");
			llenarDTG(27, "RI", "==");
			llenarDTG(28, "RII", "<=");
			llenarDTG(29, "RIQ", "<");
		}
		private void llenarDTG(int numFila, string palabra, string traduccion)
		{
			dtgBiblioteca.Rows.Add(numFila, palabra, traduccion);
		}

		private void DtgBiblioteca_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if(e.ColumnIndex >=0 && e.RowIndex>=0)
			{
				// Obtener valor de la primera columna de la fila seleccionada
				valor = int.Parse(dtgBiblioteca.Rows[e.RowIndex].Cells[0].Value.ToString());
				lblPalabra.Text = dtgBiblioteca.Rows[e.RowIndex].Cells[1].Value.ToString();
			}
		}

		private void dtgBiblioteca_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
			{
				// Obtener valor de la primera columna de la fila seleccionada
				valor = int.Parse(dtgBiblioteca.Rows[e.RowIndex].Cells[0].Value.ToString());
				lblPalabra.Text = dtgBiblioteca.Rows[e.RowIndex].Cells[1].Value.ToString();
			}
		}
		public string ValorTextBox { get; set; }

		int valor = 1;
		public string codigo { get; set; }
		private void bntInsertar_Click(object sender, EventArgs e)
		{
			switch (valor)
			{
				case 1:
					codigo = "CXFA KLA <IDEN>\n"+
						"|\n"+
						"   :0Aqui va el codigo0:\n" +
						"||";
					break;
				case 2:
					codigo = "kla <IDEN>\n|\n   :0Aqui va el codigo0:\n||";
					break;
				case 3:
					codigo = "| ENT <IDEN> = <CONE> ||";
					break;
				case 4:
					codigo = "| KAR <IDEN> = <CRTR> ||";
					break;
				case 5:
					codigo = "| LOG <IDEN> = <VERA> ||\n| LOG < IDEN > = < FALSA > ||";
					break;
				case 6:
					codigo = "| MAT <TIPO> [ ] <IDEN> ||\n| < IDEN > = NOV <TIPO> [ <CONE> ] ||";
					break;
				case 7:
					codigo = "| REA <IDEN> = <CONR> ||";
					break;
				case 8:
					codigo = "| SXN <IDEN> = <CDNA> ||";
					break;
				case 9:
					codigo = "| OBJ <ID01> <ID02> = nov <ID01> ( <PARAM> ) ||";
					break;
				case 10:
					codigo = "| SE ( <LOG> )\n" +
							"|\n"+
							"   :0Aqui va el codigo0:\n"+ 
							"||\n"+
							"ALI\n"+
							"|\n"+
							"   :0Aqui va el codigo0:\n"+ 
							"||\n"+
						"||";
					break;
				case 11:
					codigo = "| DAR ||";
					break;
				case 12:
					codigo = "| SKR ( <CDNA> ) ||";
					break;
				case 13:
					codigo = "| DUM (<LOG> )\n"+
							"  :0Aqui va el codigo0:\n" +
							"||";
					break;
				case 14:
					codigo = "| FAR\n"+
							"   :0Aqui va el codigo0:\n"+ 
								"DUM( < LOG > ) ||";
					break;
				case 15:
					codigo = "SXA ( <VAL> )\n"+
							"|\n"+
								"| KAZ<VAL> :\n"+ 
								"   :0Aqui va el codigo0:\n"+ 
								"   | ROM ||\n"+
								"||\n"+
								"| KAZ<VAL> :\n"+ 
								"   :0Aqui va el codigo0:\n"+ 
								"   | ROM ||\n"+
								"||\n"+
								"ALI\n"+
								"|\n"+
								"   :0Aqui va el codigo0:\n" +
								"   | ROM ||\n"+
								"||\n"+
							"||";
					break;
				case 16:
					codigo = "| KAP ( <IDEN> ) ||";
					break;
				case 17:
					codigo = "| LEG ( <CONE> ) ||";
					break;
				case 18:
					codigo = "| POR ( | ent <IDEN = <CONE> ||\n"+
							"                | < LOG > ||\n"+
							"                | < IDEN > ++ || )\n"+
							"   :0Aqui va el codigo0:\n"+ 
							"||";
					break;
				case 19:
					codigo = "PRX ( <tipo> <id01> <id02> )\n"+
							"|\n"+
							"   :0Aqui va el codigo0:\n"+ 
							"||";
					break;
				case 20:
					codigo = "<TIPO> <IDEN>( <param> )\n"+ 
							"|\n"+
							"   :0Aqui va el codigo0:\n"+
							"||";
					break;
				case 21:
					codigo = "<LOG> LA <LOG>";
					break;
				case 22:
					codigo = "<LOG> LN <LOG>";
					break;
				case 23:
					codigo = "<LOG> LO <LOG>";
					break;
				case 24:
					codigo = "<VAL> RSQ <VAL>";
					break;
				case 25:
					codigo = "<VAL> RSI <VAL> ";
					break;
				case 26:
					codigo = "<VAL> RD <VAL>";
					break;
				case 27:
					codigo = "<VAL> RI <VAL>";
					break;
				case 28:
					codigo = "<VAL> RII <VAL>";
					break;
				case 29:
					codigo = "<VAL> RIQ <VAL>";
					break;
				default:
					codigo = "null";
					break;
			}
			this.DialogResult = DialogResult.Yes;
			this.Close();

		}

		private void btnRegresar_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.No;
			this.Close();
		}
	}
}
