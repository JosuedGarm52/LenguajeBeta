using MySql.Data.MySqlClient;
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
	public partial class Form1 : Form
	{
		static string user = DesEncriptar("UwBvAG0AbwBzAFIAbwBiAG8AdABzADcANwA3AA==");
		static string conexionstring = $"server=localhost;user id=root;database=Analizador;port=3310; password = {user}; SslMode = 0;";
		public static MySqlConnection conexion = new MySqlConnection(conexionstring);
		MySqlCommand cmd;
		MySqlDataReader reader;
		const char careEspacio = '˾';
		const char careEnter = 'գ';
		const char careFin = 'Ƒ';
		string cadenaLexico;
		int LexicoErrores = 0;
		public Form1()
		{
			InitializeComponent();
		}
		public static string DesEncriptar(string _cadenaAdesencriptar)
		{
			string result = string.Empty;
			byte[] decryted = Convert.FromBase64String(_cadenaAdesencriptar);
			//result = System.Text.Encoding.Unicode.GetString(decryted, 0, decryted.ToArray().Length);
			result = Encoding.Unicode.GetString(decryted);
			return result;
		}
	

		private void btnSalir_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void label1_Click(object sender, EventArgs e)
		{

		}

		private void btnAnalizar_Click(object sender, EventArgs e)
		{
			try
			{
				if(!String.IsNullOrEmpty(rchTexto.Text))
				{
					rchConsola1.Text = "Exito";
					string codigo2 = "";
					int y = 0;
					
					foreach (char car in rchTexto.Text)
					{
						if(car == ' ')
						{
							codigo2 += ""+careEspacio;
						}else
						if(car == '\n')
						{
							codigo2 += "" + careEnter;
						}else
						{
							codigo2 += "" + car;
						}
					}
					char[] codigo;
					codigo2 += careFin;
					codigo = codigo2.ToCharArray();
					y = 0;
					byte[] ascii = Encoding.ASCII.GetBytes(codigo);//convierte a ascii el caracter
					//Mensaje("Ascii: " + ascii[0] + " Caracter: "+ (char)ascii[0]);
					//string indice = "0";
					LexicoErrores = 0;
					string posicion = "0";
					string cat = "";
					string consola = "";
					bool listo = false;
					int haltura = 1;
					int wancho = 1;
					string textoError = "Errores Lexicos: \n";
					char letraAnt = ' ';
					
					do
					{
						string query = "SELECT * FROM compilador WHERE ESTSIM = " + posicion;
						EstadoConexion(true);
						cmd = new MySqlCommand(query, conexion);
						reader = cmd.ExecuteReader();
						while (reader.Read())
						{
							//indice = reader["ESTSIM"].ToString();
							
							if (codigo[y] == careEspacio || codigo[y] == careFin || codigo[y] == careEnter)
							{
								posicion = "0";
								if(listo)
								{
									cat += reader["CAT"].ToString();
								}
								if (reader["CAT"].ToString() == "FAIL")
								{
									LexicoErrores++;
									textoError += haltura + "," + (wancho - 1) + " Palabra equivocada \n";
								}
								if (codigo[y] == careEspacio)
								{
									cat += careEspacio;
									listo = false;
								}
								if (codigo[y] == careFin)
								{
									cat += careFin;
									listo = false;
								}
								if (codigo[y] == careEnter)
								{
									cat += "\n";
									haltura++;
									wancho = 1;
									listo = false;
								}

								consola += reader["FDC"].ToString();
								consola += "\n";
								letraAnt = codigo[y];

							}
							else
							{
								string ced = "_" + ascii[y];
								posicion = reader[ced].ToString();
								letraAnt = codigo[y];
								listo = true;
							}

						}
						EstadoConexion(false);
						y++;
						wancho++;
					} while (y < codigo.Length);

					//do
					//{
					//	if (codigo[y] == careEnter)
					//	{
					//		cat += "\n";
					//		haltura++;
					//		wancho = 1;
					//		y++;
					//	}
					//	else
					//	{
					//		if (!inicio)
					//		{
					//			string query = "SELECT * FROM compilador WHERE ESTSIM = " + 0;
					//			EstadoConexion(true);
					//			cmd = new MySqlCommand(query, conexion);
					//			reader = cmd.ExecuteReader();
					//			while (reader.Read())
					//			{
					//				//indice = reader["ESTSIM"].ToString();
					//				posicion = reader["_" + ascii[y]].ToString();
					//				inicio = true;

					//			}
					//		}
					//		else
					//		{
					//			string query = "SELECT * FROM compilador WHERE ESTSIM = " + posicion;
					//			EstadoConexion(true);
					//			cmd = new MySqlCommand(query, conexion);
					//			reader = cmd.ExecuteReader();
					//			while (reader.Read())
					//			{
					//				//indice = reader["ESTSIM"].ToString();

					//				if (codigo[y] == careEspacio || codigo[y] == careFin)
					//				{
					//					posicion = "0";
					//					cat += reader["CAT"].ToString();
					//					if (reader["CAT"].ToString() == "FAIL")
					//					{
					//						LexicoErrores++;
					//						textoError += haltura + "," + (wancho - 1) + " Palabra equivocada \n";
					//					}
					//					if (codigo[y] == careEspacio)
					//					{
					//						cat += careEspacio;
					//						inicio = false;
					//					}
					//					else
					//					{
					//						cat += careFin;
					//						inicio = true;
					//					}

					//					consola += reader["FDC"].ToString();
					//					consola += "\n";
					//					letraAnt = codigo[y];

					//				}
					//				else
					//				{
					//					posicion = reader["_" + ascii[y]].ToString();
					//					letraAnt = codigo[y];
					//				}

					//			}

					//			y++;
					//		}

					//		EstadoConexion(false);
					//	}
					//	wancho++;
					//} while (y < codigo.Length);
					
					
					if (LexicoErrores == 0)
					{
						rchConsola2.Text = "Revision Exitosa";
					}else
					{
						rchConsola2.Text = textoError;
					}
					cadenaLexico = cat;
					InicializarCodigo(rchLexico, cadenaLexico, true);
					rhcAnalisisLexico.Text = consola;
					//rchConsola2.Text = consola;
				}else
				{
					rchConsola1.Text = "Error: El codigo esta vacio";
				}
				

			}
			catch (Exception ex)
			{

				Mensaje(ex.Message);
			}
			finally
			{
				EstadoConexion(false);
			}
			
		}
		private void InicializarCodigo(RichTextBox rich, string cadena, bool All)
		{
			if(All)
			{
				rich.Text = cadena;
			}else
			{
				string temp = "";
				foreach (char c in cadena)
				{
					if (c == careEspacio)
					{
						temp += ' ';
					}
					else if (c == careEnter)
					{
						temp += "\n";
					}
					else if (careFin == c)
					{
						temp += "";
					}
					else
					{
						temp += c;
					}
				}
				rich.Text = temp;
			}
			
		}
		private void Mensaje(string men)
		{
			MessageBox.Show(men);
		}

		private void rchTexto_TextChanged(object sender, EventArgs e)
		{

		}

		private void btnComprobar_Click(object sender, EventArgs e)
		{
			try
			{
				string query = "SELECT * FROM compilador ";
				string a = "";
				EstadoConexion(true);
				cmd = new MySqlCommand(query, conexion);
				reader = cmd.ExecuteReader();
				while (reader.Read())
				{
					a = reader["ESTSIM"].ToString();
				}
				reader.Close();
				if (string.IsNullOrWhiteSpace(a))
				{
					MessageBox.Show("Error");
				}
				else
				{
					MessageBox.Show("Si  " + a);
				}
				EstadoConexion(false);
			}
			catch (Exception ex)
			{
				Mensaje(ex.Message);
			}
			finally
			{
				EstadoConexion(false);
			}
		}
		bool estado = false;
		private string EstadoConexion(bool est)
		{
			if (estado == false && est == true)
			{

				conexion.Open(); estado = true;
				return "La conexion se a abierto";
			}
			if (estado == true && est == false)
			{

				conexion.Close(); estado = false;
				return "La conexion se a cerrado";
			}
			return "Al parecer la conexion ya tiene ese estado";

		}
		private void MensajeArray(char[] cc)
		{
			string x = "";
			for (int i = 0; i < cc.Length; i++)
			{
				x += ""+cc[i];
			}
			MessageBox.Show(""+x,"Mensaje de arreglo");
		}

		private void btnLexicar_Click(object sender, EventArgs e)
		{
			Mensaje(EstadoConexion(true));
		}

		private void btnSintactizar_Click(object sender, EventArgs e)
		{
			Mensaje(EstadoConexion(false));
		}
	}
}
