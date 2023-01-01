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
		Form1 EstaForma;
		static string user = DesEncriptar("UwBvAG0AbwBzAFIAbwBiAG8AdABzADcANwA3AA==");
		static string conexionstring = $"server=localhost;user id=root;database=Analizador;port=3310; password = {user}; SslMode = 0;";
		public static MySqlConnection conexion = new MySqlConnection(conexionstring);
		MySqlCommand cmd;
		MySqlDataReader reader;
		const char careEspacio = '˾';
		const char careEnter = 'գ';
		const char careFin = 'Ƒ';
		string cadenaLexico;
		string cadenaSintactico;
		int LexicoErrores = 0;
		public Form1()
		{
			InitializeComponent();
			EstaForma = this;
			btnRellenar.Focus();
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
				EstaForma.Cursor = Cursors.AppStarting;
				if (!String.IsNullOrEmpty(rchTexto.Text))
				{
					rchConsola1.Text = "Exito";
					rchConsola2.Text = "";
					rchConsola3.Text = "";
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
					char gar = char.Parse(cat.Substring(cat.Length - 1, 1));
					if(gar == '\n')
					{
						cadenaLexico = cat + careFin;
					}
					else
					if ( gar != careEspacio)
					{
						cadenaLexico = cat + " " +careFin;
					}
					else
					{
						cadenaLexico = cat;
					}
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
				EstaForma.Cursor = Cursors.Default;
				btnLexicar.Focus();
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
		private string MensajeArray(string[] cc, bool comp)
		{
			string x = "";
			for (int i = 0; i < cc.Length; i++)
			{
				x += cc[i] + " ";
			}
			if(comp)
			{
				MessageBox.Show(x, "Mensaje de arreglo");
			}
			return x;
		}
		private void btnLexicar_Click(object sender, EventArgs e)
		{
			EstaForma.Cursor = Cursors.AppStarting;
			rchConsola3.Text = "";
			if (!String.IsNullOrWhiteSpace(rchLexico.Text) || !String.IsNullOrEmpty(rchLexico.Text))
			{
				try
				{
					bool final = true;
					string[] lexicos = cadenaLexico.Split(careEspacio, '\n');//rchLexico.Text.Split(' ','\n');
					int y = 0;
					int numI = 0;
					int numF = 0;
					int PosicionI = 0;
					int PosicionF = 0;
					do//Analizar si todos los inicios y cierras estan completos
					{
						if (lexicos[y] == "INIS")
						{
							PosicionI = y - 1;
							numI++;
							
						}
						if (lexicos[y] == "FIIN")
						{
							PosicionF = y - 1;
							numF++;
						}
						if (lexicos[y] == "" + careFin || lexicos[y] == " " + careFin)
						{
							final = false;
						}
						else
							y++;
					} while (final);
					if (numI == numF)
					{
						string cadena1 = "";
						string[] codigo;
						int count = 0;
						int numid = 1;
						//Borrar los coms y asignar IDs
						foreach (string str in lexicos)
						{
							if (count != 0)
							{
								cadena1 += " ";
							} else
							{
								count++;
							}
							if (str == "COMS")
							{
								cadena1 += "";
								count--;
							} else
							if (str == "IDEN")
							{
								if (Math.Floor(Math.Log10(numid) + 1) == 1)
								{
									cadena1 += "ID" + "0" + numid;
								}
								else
								{

									cadena1 += "ID" + numid;
								}
								numid++;
							}
							else
							{
								cadena1 += str;
							}

						}
						codigo = cadena1.Split(' ', '\n');
						int sinCambios = 0;
						int cambios = 0;
						string mirarString = "";
						string temp = "";
						string cat = "";
						bool Finalizo = false;
						bool NSe = false;
						do
						{
							string[] codigx = codigo;
							string cod1="";
							int cont1 = 0;
							foreach (string str in codigx)
							{
								
								if(str != "" && str != ""+careFin)
								{
									if(cont1 == 0)
									{
										cont1++;
									}else
									{
										cod1 += " ";
									}
									cod1 += str;
								}
								else
								{
									
								}
							}
							codigo = cod1.Split(' ');
							//Mensaje(cod1);
							
							for (int s = 0; s < codigo.Length; s++)
							{
								const int vuelta = 3;
								int tamaño = codigo.Length;
								int posicion = tamaño - s;
								if (posicion >= 13)
								{
									//MAT 2
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR13"
										&& codigo[s + 2] == "ARG65"
										&& codigo[s + 3] == "CEX["
										&& codigo[s + 4] == "CEX]"
										&& codigo[s + 5] == "ARG63"
										&& codigo[s + 6] == "CEX="
										&& codigo[s + 7] == "PR14"
										&& codigo[s + 8] == "ARG65"
										&& codigo[s + 9] == "CEX["
										&& codigo[s + 10] == "CONE"
										&& codigo[s + 11] == "CEX]"
										&& codigo[s + 12] == "FIIN")
									{
										sinCambios = 0;
									}
								}

								if (posicion >= 11)
								{
									//Se 2 y ali
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR21"
										&& codigo[s + 2] == "ARG10"
										&& codigo[s + 3] == "INIS"
										&& codigo[s + 4] == "BLOQUEA1"
										&& codigo[s + 5] == "FIIN"
										&& codigo[s + 6] == "PR01"
										&& codigo[s + 7] == "INIS"
										&& codigo[s + 8] == "BLOQUEA1"
										&& codigo[s + 9] == "FIIN"
									   && codigo[s + 10] == "FIIN")
									{
										sinCambios = 0;
										codigo[s + 0] = "SPR21";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										codigo[s + 4] = "";
										codigo[s + 5] = "";
										codigo[s + 6] = "";
										codigo[s + 7] = "";
										codigo[s + 8] = "";
										codigo[s + 9] = "";
										codigo[s + 10] = "";
										if(NSe)
										{
											NSe = false;
										}
										cat += "Se y Ali - Exito \n";
										break;
									}
								}
								if (posicion >= 9)
								{
									//MAT 3
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR13"
										&& codigo[s + 2] == "ARG65"
										&& codigo[s + 3] == "CEX["
										&& codigo[s + 4] == "CEX]"
										&& codigo[s + 5] == "ARG63"
										&& codigo[s + 6] == "CEX="
										&& codigo[s + 7] == "ARG67"
										&& codigo[s + 8] == "FIIN")
									{
										sinCambios = 0;
									}
									else
									//POR
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR16"
										&& codigo[s + 2] == "CEX("
										&& codigo[s + 3] == "P16R1"
										&& codigo[s + 4] == "P16R2"
										&& codigo[s + 5] == "P16R3"
										&& codigo[s + 6] == "CEX)"
										&& codigo[s + 7] == "BLOXX5"
										&& codigo[s + 8] == "FIIN")
									{
										sinCambios = 0;
									}
									else
									//PRX
									if (codigo[s + 0] == "PR17"
										&& codigo[s + 1] == "CEX("
										&& codigo[s + 2] == "TIPO"
										&& codigo[s + 3] == "ARG90"
										&& codigo[s + 4] == "ARG90"
										&& codigo[s + 5] == "CEX)"
										&& codigo[s + 6] == "INIS"
										&& codigo[s + 7] == "BLOXX6"
										&& codigo[s + 8] == "FIIN")
									{
										sinCambios = 0;
									}
								}

								if (posicion >= 8)
								{
									//FAR
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR04"
										&& codigo[s + 2] == "BLOXX3"
										&& codigo[s + 3] == "PR03"
										&& codigo[s + 4] == "CEX("
										&& codigo[s + 5] == "ARG36"
										&& codigo[s + 6] == "CEX("
										&& codigo[s + 7] == "FIIN")
									{
										sinCambios = 0;
									}
								}

								if (posicion >= 7)
								{
									//Se 1
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR21"
										&& codigo[s + 2] == "ARG10"
										&& codigo[s + 3] == "INIS"
										&& codigo[s + 4] == "BLOQUEA1"
										&& codigo[s + 5] == "FIIN"
										&& codigo[s + 6] == "FIIN")
									{
										sinCambios = 0;
										codigo[s] = "SPR10";
										codigo[s + 2] = "";
										codigo[s + 1] = "";
										codigo[s + 3] = "";
										codigo[s + 4] = "";
										codigo[s + 5] = "";
										codigo[s + 6] = "";
										cat += "Se - Exito \n";
										break;
									}
									else
									//DUM
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR03"
										&& codigo[s + 2] == "CEX("
										&& codigo[s + 3] == "ARG25"
										&& codigo[s + 4] == "CEX)"
										&& codigo[s + 5] == "BLOXX2"
										&& codigo[s + 6] == "FIIN")
									{
										sinCambios = 0;
									}
									else
									//MAT 1
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR13"
										&& codigo[s + 2] == "ARG65"
										&& codigo[s + 3] == "CEX["
										&& codigo[s + 4] == "CEX]"
										&& codigo[s + 5] == "ARG63"
										&& codigo[s + 6] == "FIIN")
									{
										sinCambios = 0;
									}
									else
									//SXN 2
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR23"
										&& codigo[s + 2] == "CEX("
										&& codigo[s + 3] == "CDNA"
										&& codigo[s + 4] == "AGR20"
										&& codigo[s + 5] == "CEX)"
										&& codigo[s + 6] == "FIIN")
									{
										sinCambios = 0;
									}else
									//SKR 2
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR24"
										&& codigo[s + 2] == "CEX("
										&& codigo[s + 3] == "CDNA"
										&& codigo[s + 4] == "AGR4"
										&& codigo[s + 5] == "CEX)"
										&& codigo[s + 6] == "FIIN")
									{
										sinCambios = 0;
										codigo[s + 0] = "SPR24";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										codigo[s + 4] = "";
										codigo[s + 5] = "";
										codigo[s + 6] = "";
										cat += "SKR - Exito";
										break;
									}
								}

								if (posicion >= 6)
								{
									//KAP
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR09"
										&& codigo[s + 2] == "CEX("
										&& codigo[s + 3] == "ARG58"
										&& codigo[s + 4] == "CEX)"
										&& codigo[s + 5] == "FIIN")
									{
										sinCambios = 0;
									}
									else
									// KLA 2
									if (codigo[s + 0] == "PR10"
										&& codigo[s + 1] == "ARG59"
										&& codigo[s + 2] == "INIS"
										&& codigo[s + 3] == "P10R1"
										&& codigo[s + 4] == "DECFX5"
										&& codigo[s + 5] == "FIIN")
									{
										sinCambios = 0;
									}
									else
									//LEG
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR12"
										&& codigo[s + 2] == "CEX("
										&& codigo[s + 3] == "ARG63"
										&& codigo[s + 4] == "CEX)"
										&& codigo[s + 5] == "FIIN")
									{
										sinCambios = 0;
									}
									else
									//LOG 2
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR11"
										&& codigo[s + 2] == "AGR3"
										&& codigo[s + 3] == "CEX="
										&& codigo[s + 4] == "BLN"
										&& codigo[s + 5] == "FIIN")
									{
										sinCambios = 0;
									}
									else
									//REV
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR19"
										&& codigo[s + 2] == "CEX("
										&& codigo[s + 3] == "ARG100"
										&& codigo[s + 4] == "CEX)"
										&& codigo[s + 5] == "FIIN")
									{
										sinCambios = 0;
									}
									else
									//SKR 1
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR24"
										&& codigo[s + 2] == "CEX("
										&& codigo[s + 3] == "CDNA"
										&& codigo[s + 4] == "CEX)"
										&& codigo[s + 5] == "FIIN")
									{
										sinCambios = 0;
										codigo[s + 0] = "SPR24";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										codigo[s + 4] = "";
										codigo[s + 5] = "";
										cat += "SKR - Exito \n";
										break;
									}
									else
									//SXA SXA
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR22"
										&& codigo[s + 2] == "CEX("
										&& codigo[s + 3] == "ARG55"
										&& codigo[s + 4] == "CEX)"
										&& codigo[s + 5] == "FIIN")
									{
										sinCambios = 0;
									}
									else
									//SXA KAZ
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR08"
										&& codigo[s + 2] == "ARG53"
										&& codigo[s + 3] == "CEX:"
										&& codigo[s + 4] == "BLOXX4"
										&& codigo[s + 5] == "FIIN")
									{
										sinCambios = 0;
									}
									else
									//SXN 1
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR23"
										&& codigo[s + 2] == "CEX("
										&& codigo[s + 3] == "CDNA"
										&& codigo[s + 4] == "CEX)"
										&& codigo[s + 5] == "FIIN")
									{
										sinCambios = 0;
									}
								}

								if (posicion >= 5)
								{
									//KLA 3
									if (codigo[s + 0] == "PR10"
										&& codigo[s + 1] == "ARG59"
										&& codigo[s + 2] == "INIS"
										&& codigo[s + 3] == "DECFX5"
										&& codigo[s + 4] == "FIIN")
									{
										sinCambios = 0;
									}
									else
									//KLA 4
									if (codigo[s + 0] == "PR10"
										&& codigo[s + 1] == "ARG59"
										&& codigo[s + 2] == "INIS"
										&& codigo[s + 3] == "P10R1"
										&& codigo[s + 4] == "FIIN")
									{
										sinCambios = 0;
									}
								}

								if (posicion >= 4)
								{
									//ENT 
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR04"
										&& codigo[s + 2] == "DECP1"
										&& codigo[s + 3] == "FIIN")
									{
										sinCambios = 0;
									}
									else
									//KAR
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR07"
										&& codigo[s + 2] == "DECP2"
										&& codigo[s + 3] == "FIIN")
									{
										sinCambios = 0;
									}
									else
									//KLA 1
									if (codigo[s + 0] == "PR10"
										&& codigo[s + 1] == "ARG59"
										&& codigo[s + 2] == "INIS"
										&& codigo[s + 3] == "FIIN")
									{
										sinCambios = 0;
									}
									else
									//LOG 1
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR11"
										&& codigo[s + 2] == "AGR3"
										&& codigo[s + 3] == "FIIN")
									{
										sinCambios = 0;
									}
									else
									//REA
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR07"
										&& codigo[s + 2] == "DECP3"
										&& codigo[s + 3] == "FIIN")
									{
										sinCambios = 0;
									}
									else
									//SXA ALI/DEFAULT
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR01"
										&& codigo[s + 2] == "BLOXX4"
										&& codigo[s + 3] == "FIIN")
									{
										sinCambios = 0;
									}
								}
								if (posicion >= 3)
								{
									//SXA ROM
									if (codigo[s + 0] == "INIS"
									&& codigo[s + 1] == "PR20"
									&& codigo[s + 2] == "FIIN")
									{
										sinCambios = 0;
									}
								}
								//SE
								if(codigo[s] == "PR21")
								{
									NSe = true;
								}
								if(NSe)
								{

									//ARG10
									if (s >= vuelta && (codigo[s - 2] == "CEX(" && codigo[s - 1] == "ARG10" && codigo[s] == "CEX)"))
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG10";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG10
									if (codigo[s] == "ARG11" || codigo[s] == "ARG12")
									{
										sinCambios = 0;
										codigo[s] = "ARG10";
										s--;
									}
									//ARG12
									if (s >= vuelta && (codigo[s - 2] == "CEX(" && codigo[s - 1] == "ARG18" && codigo[s] == "CEX)"))
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG10";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG12 - OPR
									if (s >= vuelta && codigo[s] == "ARG13"
										&& codigo[s - 2] == "ARG13"
										&& (codigo[s - 1] == "OPR1"
										|| codigo[s - 1] == "OPR2"
										|| codigo[s - 1] == "OPR3"
										|| codigo[s - 1] == "OPR4"
										|| codigo[s - 1] == "OPR5"
										|| codigo[s - 1] == "OPR6"))
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG10";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG11
									if (s >= vuelta && codigo[s - 2] == "CEX(" && codigo[s - 1] == "ARG11" && codigo[s] == "CEX)")
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG11";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG11 OPL
									if (s >= vuelta && codigo[s - 2] == "ARG13"
										&& codigo[s] == "ARG13"
										&& (codigo[s - 1] == "OPLA"
										|| codigo[s - 1] == "OPLO"
										|| codigo[s - 1] == "OPLN"))
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG11";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG13
									if (codigo[s] == "ARG14" || codigo[s] == "CDNA" || codigo[s] == "CRTR" || codigo[s] == "ARG15" || codigo[s] == "ARG11" || codigo[s] == "ARG16")
									{
										sinCambios = 0;
										codigo[s] = "ARG13";
										s--;
									}
									//ARG14
									if (codigo[s] == "CONE" || codigo[s] == "CONR")
									{
										sinCambios = 0;
										codigo[s] = "ARG14";
										s--;
									}
									//ARG16
									if (s >= vuelta && codigo[s - 2] == "CEX(" && codigo[s - 1] == "ARG16" && codigo[s] == "CEX)")
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG16";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG16
									if (s >= vuelta && codigo[s - 2] == "CEX(" && codigo[s - 1] == "ARG16" && codigo[s] == "CEX)")
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG16";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG16
									if (s >= vuelta && codigo[s - 2] == "ARG18" && codigo[s - 1] == "ARG17" && codigo[s] == "ARG18")
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG16";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG17
									if (codigo[s] == "OPSM" || codigo[s] == "OPRS" || codigo[s] == "OPML" || codigo[s] == "OPDV" || codigo[s] == "OPEX")
									{
										sinCambios = 0;
										codigo[s] = "ARG17";
										s--;
									}
									//ARG18
									if (codigo[s] == "ARG14" || codigo[s] == "ARG16" || codigo[s] == "ARG15")
									{
										sinCambios = 0;
										codigo[s] = "ARG18";
										s--;
									}
									//ARG15
									if (codigo[s].Substring(0, 2) == "ID")
									{
										sinCambios = 0;
										codigo[s] = "ARG15";
										s--;
									}
									//BLOQUEA1
									if (s >= vuelta - 1 && codigo[s - 1] == "BLOQUEA1" && codigo[s] == "BLOQUEA1")
									{
										sinCambios = 0;
										codigo[s - 1] = "BLOQUEA1";
										codigo[s] = "";
										break;
									}
									//BLOQUEA1
									if (codigo[s] == "SPR01"
										|| codigo[s] == "SPR21"
										|| codigo[s] == "SPR02"
										|| codigo[s] == "SPR24"
										|| codigo[s] == "SPR03"
										|| codigo[s] == "SPR06"
										|| codigo[s] == "SPR08"
										|| codigo[s] == "SPR09"
										|| codigo[s] == "SPR22"
										|| codigo[s] == "SPR20"
										|| codigo[s] == "SPR16"
										|| codigo[s] == "DEC1")
									{
										sinCambios = 0;
										codigo[s] = "BLOQUEA1";
										s--;
									}
									//DEC1
									if (codigo[s] == "SPR04"
										|| codigo[s] == "SPR07"
										|| codigo[s] == "SPR11"
										|| codigo[s] == "SPR12"
										|| codigo[s] == "SPR13"
										|| codigo[s] == "SPR14"
										|| codigo[s] == "SPR17"
										|| codigo[s] == "SPR18"
										|| codigo[s] == "SPR23")
									{
										sinCambios = 0;
										codigo[s] = "DEC1";
										s--;
									}
								}
								//DUM
								{

								}
								//ENT
								{

								}
								//FAR
								{

								}
								//KAP
								{

								}
								//KAR
								{

								}
								//KLA
								{

								}
								//LEG
								{

								}
								//LOG
								{

								}
								//MAT
								{

								}
								//POR
								{

								}
								//PRX
								{

								}
								//REA
								{

								}
								//REV
								{

								}
								//SKR
								{
									//AGR4
									if (s >= vuelta - 1 && codigo[s - 1] == "CEX," && codigo[s] == "ARG20")
									{
										sinCambios = 0;
										codigo[s - 1] = "AGR4";
										codigo[s] = "";
										break;
									}
									//ARG20
									if (s >= vuelta - 1 && codigo[s - 1] == "OPSM" && codigo[s] == "ARG21")
									{
										sinCambios = 0;
										codigo[s - 1] = "ARG21";
										codigo[s] = "";
										break;
									}
									//ARG21
									if (codigo[s] == "CDNA" || codigo[s] == "ARG22" || codigo[s] == "CRTR" || codigo[s] == "ARG23")
									{
										sinCambios = 0;
										codigo[s] = "ARG21";
										s--;
									}
									//ARG22
									if (codigo[s] == "CONE" || codigo[s] == "CONR")
									{
										sinCambios = 0;
										codigo[s] = "";
									}
									//ARG23
									if (codigo[s].Substring(0, 2) == "ID")
									{
										sinCambios = 0;
										codigo[s] = "ARG23";
										s--;
									}
								}
								//SXA KAZ ROM
								{

								}
								//SXN
								{

								}
							
								mirarString = MensajeArray(codigo, false);
								if(mirarString != temp)
								{
									temp = mirarString;
									cambios++;
									//Mensaje(temp + " -\nNum Cambios: " + cambios +"\n Numero de s vuelta: "+ s);
								}
								if(s == codigo.Length-1 && sinCambios == 2)
								{
									Finalizo = true;
								}
							}
							sinCambios++;
							
						} while (sinCambios < 4 && !Finalizo);
						if(sinCambios > 3)
						{
							Advertir(rchConsola3, "No encontro mas cambios", "Revisa la consola 3");
						}else
						{
							rchConsola3.Text = "Exito";
							foreach (string str in codigo)
							{
								cadenaSintactico += str + "\n"; 
							}
							rchSintactico.Text = cadenaSintactico;
							rchAnalisisSintactico.Text = cat;
						}

					}else if(numI > numF)
					{
						rchConsola3.Text = "Ocurrio un error con un '|' sin cerrar cerca de la palabra numero " + PosicionI;
					}
					else
					{
						rchConsola3.Text = "Ocurrio un error parece tener un '||' extra, compruebelo en la posicion " + PosicionF;
					}
				}
				catch (Exception ex)
				{
					Mensaje(ex.Message);
				}
				finally
				{
					EstaForma.Cursor = Cursors.Default;
				}
				
			}
			else
			{
				Advertir(rchConsola2, "Debes primero introducir el codigo en el tab de codigo y luego pulsar analizar.", "Comprueba la consola 2");
			}
			btnSintactizar.Focus();
		}

		private void btnSintactizar_Click(object sender, EventArgs e)
		{
			
			Mensaje("Funcionalidad proximamente...");
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
			btnAnalizar.Focus();
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

		private void groupBox2_Enter(object sender, EventArgs e)
		{

		}

		private void Form1_Load(object sender, EventArgs e)
		{
			btnRellenar.Focus();
		}

		private void label8_Click(object sender, EventArgs e)
		{

		}
	}
}
