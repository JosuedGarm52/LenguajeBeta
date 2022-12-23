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
using System.IO;

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
		private bool CaracterProhibido(string caracter)
		{
			if (caracter == "_63")
			{
				return true;
			}
			return false;
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
					string indice = "0";
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
							indice = reader["ESTSIM"].ToString();
							string espacio = reader["_32"].ToString();
							if ((codigo[y] > 125 && codigo[y] != careEnter && codigo[y] != careEspacio && codigo[y] != careFin) || CaracterProhibido(posicion) )
							{
								rchConsola1.Text = "Error: Letra no permitida en el lenguaje " + codigo[y];
								throw new Exception("Se a encontrado un caracter fuera del rango valido: Caracter: "+ codigo[y]);
							}
							if (codigo[y] == careEspacio || codigo[y] == careFin || codigo[y] == careEnter )
							{
								if(espacio == "92")
								{
									posicion = "0";

									if (listo)
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

					if (LexicoErrores == 0)
					{
						rchConsola2.Text = "Revision Exitosa";
					}else
					{
						rchConsola2.Text = textoError;
					}
					cadenaLexico = cat;
					InicializarCodigo(rchLexico, cadenaLexico, false);//true enseña todos los caracteres ocultos
					rhcAnalisisLexico.Text = consola;
					//rchConsola2.Text = consola;
				}else
				{
					Advertir(rchConsola1, "Error: El codigo esta vacio", "Error: Compruebe la consola 1");
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
		private void Advertir(RichTextBox rich , string textoRich, string textoMensaje)
		{
			rich.Text = textoRich;
			Mensaje(textoMensaje); 
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
		int cero = 0;
		private void btnRellenar_Click(object sender, EventArgs e)
		{
			const string ruta = @".\codigos.txt";
			const string repuesto = ":0 Se 0:\n| SE ( _x RSQ 2 )\n| SKR ( \"X es mayor que 2\" ) ||\nALI\n| SKR ( \"X es menor que 2\" ) ||\n||\n\n:0 Dar 0:\nSE ( _x RSQ 2 )\n| SKR ( \"X es mayor que 2\" ) ||\n| DAR ||\n||\n\n:0 SKR 0:\n| SKR ( \"Resultado: \" + 2 + _letra ) ||\n\n:0 DUM 0:\n| DUM ( _x RSQ 5 ) \n| SKR ( _x ) ||\n| _x = _x + 1 ||\n||\n\n:0 Far 0:\n| FAR\n| SKR ( _x ) ||\n| _x = _x + 1 ||\nDUM ( _x RSQ 5 ) ||\n\n:0 ENT 0:\n| ENT _entero1 = 1 ||\n\n:0 FALSA 0:\nFALSA \n\n:0 KAR 0:\n| KAR _caracter = 'c' || \n\n:0 KAZ SXA ROM 0:\n\nSXA ( _entero1 )\n| KAZ 1 :\n| SKR ( \"El valor es 1\" ) ||\n| ROM ||\n||\n| KAZ 2 :\n| SKR ( \"El valor es 2\" ) ||\n| ROM ||\n||\n| ALI\n| SKR ( \"Esta vacio\" ) ||\n| ROM ||\n||\n||\n\n:0 KAP 0:\nKAP ( _Variable1 )\n\n:0 KLA 0:\nKLA _claseprueba \n|\n_claseprueba ( ) \n|\n| _x = 0 ||\n| _valor = _x + _y ||\n||\n| ent _x ||\n| ent _y = 12 ||\n| REA _valor ||\n||\n\n:0 LOG 0:\n| LOG _boolean = VERA || \n\n:0 LEG 0:\n| LEG ( 1000 ) ||\n\n:0 MAT 0:\n| MAT ent [ ] _arreglo = NOV ent [ 2 ] ||\n| _arreglo [ 1 ] = 323 ||\n| SKR ( \"Res \" + _arreglo [ 1 ] ) ||\n\n:0 POR 0:\n| POR ( | ent _i = 0 ||\n| _i RSQ 5 ||\n| _i ++ || )\n| SKR ( \"Res\" + _i ) ||\n||\n\n:0 PRX 0:\nPRX ( ent _val _arreglo1 )\n|\n| skr ( \"tc: \" + _var ) ||\n||\n\n:0 REA 0:\n| REA _real = 12.53 ||\n\n:0 SXN 0:\n| SXN _cadena = \"Hola\" ||";
			string r = "";
			try
			{
				//Pass the file path and file name to the StreamReader constructor
				StreamReader sr = new StreamReader(ruta);
				//Read the first line of text
				string line = sr.ReadLine();
				r += line+"\n";
				while (line != null)
				{
					//write the line to console window
					Console.WriteLine(line);
					//Read the next line
					line = sr.ReadLine();
					r += line;
					r += "\n";
				}
				//close the file
				sr.Close();
			}
			catch (Exception ex)
			{
				Mensaje("Exception: " + ex.Message);
				r = repuesto;
			}
			//string r = ":0 Se 0:\n| SE ( _x RSQ 2 )\n| SKR ( \"X es mayor que 2\" ) ||\nALI\n| SKR( \"X es menor que 2\" ) ||\n||\n\n:0 Dar 0:\n SE ( _x RSQ 2 )\n| SKR( \"X es mayor que 2\" ) ||\n| DAR ||\n||\n\n:0 SKR 0:\n| SKR ( \"Resultado: \" + 2 + _letra ) ||\n\n:0 DUM 0:\n| DUM( _x RSQ 5 ) \n| SKR ( _x ) ||\n| _x = _x +1 ||\n||\n\n:0 Far 0:\n| FAR\n| SKR (_x) ||\n| _x = _x +1 ||\nDUM( _x RSQ 5 )||\n\n:0 ENT 0:\n| ENT _entero1 = 1 ||\n\n:0 FALSA 0:\nFALSA \n\n:0 KAR 0:\n| KAR _caracter = 'c' || \n\n:0 KAZ SXA ROM 0:\n\nSXA (<ID>)\n|\n\nSXA ( _entero1 )\n| KAZ 1:\n| SKR ( \"El valor es 1\" ) ||\n| ROM ||\n||\n| KAZ 2:\n| SKR ( \"El valor es 2\" ) ||\n| ROM ||\n||\n| ALI\n| SKR( \"Esta vacio\" ) ||\n| ROM ||\n||\n||\n\n:0 KAP 0:\nKAP ( _Variable1 )\n\n:0 KLA 0:\nKLA _claseprueba \n|\n_claseprueba () \n|\n| _x = 0 ||\n| _valor = _x + _y ||\n||\n| ent _x ||\n| ent _y = 12 ||\n| REA _valor ||\n||\n\n:0 LOG 0:\n| LOG _boolean = VERA || \n\n:0 LEG 0:\n| LEG ( 1000 ) ||\n\n:0 MAT 0:\n| MAT ent[] _arreglo = NOV ent[2] ||\n| _arreglo[1] = 323 ||\n| SKR( \"Res \" + _arreglo[1] ) ||\n\n0: POR 0:\n| POR ( | ent _i = 0 ||\n| _i RSQ 5 ||\n| _i++ || )\n| SKR ( \"Res\" + _i ) ||\n||\n\n:0 PRX 0:\nPRX ( ent _val _arreglo1 )\n|\n| skr( \"tc: \" + _var ) ||\n||\n\n:0 REA 0:\n| REA _real = 12.53 ||\n\n:0 SXN 0:\n| SXN _cadena = \"Hola\" ||";
			// RESERVA = ":0 Se 0:\n| SE ( _x RSQ 2 )\n| SKR ( \"X es mayor que 2\" ) ||\nALI\n| SKR ( \"X es menor que 2\" ) ||\n||\n\n:0 Dar 0:\n SE ( _x RSQ 2 )\n| SKR ( \"X es mayor que 2\" ) ||\n| DAR ||\n||\n\n:0 SKR 0:\n| SKR ( \"Resultado: \" + 2 + _letra ) ||\n\n:0 DUM 0:\n| DUM ( _x RSQ 5 ) \n| SKR ( _x ) ||\n| _x = _x +1 ||\n||\n\n:0 Far 0:\n| FAR\n| SKR ( _x) ||\n| _x = _x +1 ||\nDUM( _x RSQ 5 )||\n\n:0 ENT 0:\n| ENT _entero1 = 1 ||\n\n:0 FALSA 0:\nFALSA \n\n:0 KAR 0:\n| KAR _caracter = 'c' || \n\n:0 KAZ SXA ROM 0:\n\nSXA ( _entero1 )\n| KAZ 1:\n| SKR ( \"El valor es 1\" ) ||\n| ROM ||\n||\n| KAZ 2:\n| SKR ( \"El valor es 2\" ) ||\n| ROM ||\n||\n| ALI\n| SKR( \"Esta vacio\" ) ||\n| ROM ||\n||\n||\n\n:0 KAP 0:\nKAP ( _Variable1 )\n\n:0 KLA 0:\nKLA _claseprueba \n|\n_claseprueba () \n|\n| _x = 0 ||\n| _valor = _x + _y ||\n||\n| ent _x ||\n| ent _y = 12 ||\n| REA _valor ||\n||\n\n:0 LOG 0:\n| LOG _boolean = VERA || \n\n:0 LEG 0:\n| LEG ( 1000 ) ||\n\n:0 MAT 0:\n| MAT ent[] _arreglo = NOV ent[2] ||\n| _arreglo[1] = 323 ||\n| SKR( \"Res \" + _arreglo[1] ) ||\n\n0: POR 0:\n| POR ( | ent _i = 0 ||\n| _i RSQ 5 ||\n| _i++ || )\n| SKR ( \"Res\" + _i ) ||\n||\n\n:0 PRX 0:\nPRX ( ent _val _arreglo1 )\n|\n| skr( \"tc: \" + _var ) ||\n||\n\n:0 REA 0:\n| REA _real = 12.53 ||\n\n:0 SXN 0:\n| SXN _cadena = \"Hola\" ||"
			string newLine = r.Replace("\\n", "\n");
			string bla = "\\";
			string newLine2 = newLine.Replace(bla.Substring(0,1), "");
			if (cero == 0)
			{
				string relleno = rchTexto.Text;
				rchTexto.Text = newLine2;
				cero++;
			}else
			{

				DialogResult result = MessageBox.Show("Deseas sobreescribir el texto?: Pulsa YES para sobreescribir, NO para cargar el anterior y CANCEL para cargar un predeterminado.", "Cambio de texto", MessageBoxButtons.YesNoCancel);
				if (result == DialogResult.Yes)
				{
					if (!String.IsNullOrEmpty(rchTexto.Text))
					{
						try
						{
							//Pass the filepath and filename to the StreamWriter Constructor
							StreamWriter sw = new StreamWriter(ruta);
							//Write a line of text
							string rich = rchTexto.Text;
							sw.WriteLine(rich);
							//Close the file
							sw.Close();
						}
						catch (Exception ex)
						{
							Console.WriteLine("Exception: " + ex.Message);
						}
					}
					else
					{
						Advertir(rchConsola1, "Esta vacio, no puedes sobreescribir un texto vacio", "Comprueba la consola 1");
					}
					
				}
				else if (result == DialogResult.No)
				{
					rchTexto.Text = newLine2;
				}
				else
				{
					rchTexto.Text = repuesto;
				}
			}
			
		}

		private void btnLimpiar_Click(object sender, EventArgs e)
		{
			rchTexto.Text = "";
			rchSintactico.Text = "";
			rchLexico.Text = "";
			rchConsola1.Text = "";
			rchConsola2.Text = "";
			rchConsola3.Text = "";
			rhcAnalisisLexico.Text = "";
		}

		private void label2_Click(object sender, EventArgs e)
		{

		}
	}
}
