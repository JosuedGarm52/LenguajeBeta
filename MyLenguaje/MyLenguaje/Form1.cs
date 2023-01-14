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
					rchLexico.Text = "";
					rchSintactico.Text = "";
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
										cat += " ";
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
					if (gar == careFin)
					{
						cadenaLexico = cat;
					}else
					if ( gar == careEspacio)
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
			rchSintactico.Text = "";
			if (!String.IsNullOrWhiteSpace(rchLexico.Text) || !String.IsNullOrEmpty(rchLexico.Text))
			{
				string MensajeError = "";
				string MensajeError2 = "";
				try
				{
					bool final = true;
					string[] lexicos = cadenaLexico.Split(careEspacio, '\n',' ');//rchLexico.Text.Split(' ','\n');
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
						string UltVar = "PR01";
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
						int compVuelta = 0;
						bool modoBloque = false;
						string UltDec = "";
						bool Bloc = false;
						bool NSe = false, NDum = false, NEnt = false ,NFar = false,NKap = false,NKar = false, NKla = false, NLeg = false, NLog = false, NMat = false, NPor = false, NPrx = false, NRea = false, NRev = false,NSkr = false, NSxa = false, NSxn = false,NObj = false ;
						do
						{
							string[] codigx = codigo;
							string cod1="";
							int cont1 = 0;
							if(compVuelta == 53)
							{

							}
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
								compVuelta++;
								const int vuelta = 3;
								int tamaño = codigo.Length;
								int posicion = tamaño - s;
								mirarString = MensajeArray(codigo, false);
								if (mirarString != temp)
								{
									temp = mirarString;
									cambios++;
									if (MenSi)
									{
										DialogResult dr = MessageBox.Show(temp + " -\nNum Cambios: " + cambios + "\nNumero de s vuelta: " + s + "\nCompVuelta: " + compVuelta, "Mensaje", MessageBoxButtons.OKCancel);
										switch (dr)
										{
											case DialogResult.OK:
												break;
											case DialogResult.Cancel:
												MenSi = false;
												chbMensaje.Checked = false;
												break;
											default:
												MenSi = false;
												chbMensaje.Checked = false;
												break;
										}
									}
								}
								MensajeError = "s: " + s + " -CompVuelta: " + compVuelta + " -UltPalabra: " + codigo[s] + " -UltimoMetodo: " ;
								if (posicion >= 13)
								{
									//MAT 2
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR13"
										&& codigo[s + 2] == "ARG65"
										&& codigo[s + 3] == "CEX["
										&& codigo[s + 4] == "CEX]"
						&& codigo[s + 5].Substring(0,2) == "ID"
										&& codigo[s + 6] == "ASIG"
										&& codigo[s + 7] == "PR14"
										&& codigo[s + 8] == "ARG65"
										&& codigo[s + 9] == "CEX["
										&& codigo[s + 10] == "CONE"
										&& codigo[s + 11] == "CEX]"
										&& codigo[s + 12] == "FIIN")
									{
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SPR13";
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
										codigo[s + 11] = "";
										codigo[s + 12] = "";
										if(NMat)
										{
											NMat = false;
										}
										cat += "Mat - Exito \n";
										break;
									}
								}
								if (posicion >= 12)
								{
									//OBJ 
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR15"
						&& codigo[s + 2].Substring(0,2) == "ID"
										&& codigo[s + 3] == "CEX{"
						&& codigo[s + 4].Substring(0, 2) == "ID"
										&& codigo[s + 5] == "ASIG"
										&& codigo[s + 6] == "PR14"
						&& codigo[s + 7].Substring(0, 2) == "ID"
										&& codigo[s + 8] == "CEX("
										&& codigo[s + 9] == "CEX)"
										&& codigo[s + 10] == "CEX}"
										&& codigo[s + 11] == "FIIN")
									{
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SPR15";
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
										codigo[s + 11] = "";
										if (NObj)
										{
											NObj = false;
										}
										cat += "OBJ - Exito \n";
										break;
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
										UltVar = "";
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
									else
									//POR
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR16"
										&& codigo[s + 2] == "CEX("
										&& codigo[s + 3] == "P16R1"
										&& codigo[s + 4] == "P16R2"
										&& codigo[s + 5] == "P16R3"
										&& codigo[s + 6] == "CEX)"
										&& codigo[s + 7] == "INIS"
										&& codigo[s + 8] == "BLOXX7"
										&& codigo[s + 9] == "FIIN"
										&& codigo[s + 10] == "FIIN")
									{
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SPR16";
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
										
										cat += "Por - Exito \n";
										break;
									}
								}
								if(posicion >= 10)
								{
									//FAR 2
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR06"
										&& codigo[s + 2] == "INIS"
										&& codigo[s + 3] == "BLOXX2"
										&& codigo[s + 4] == "FIIN"
										&& codigo[s + 5] == "PR03"
										&& codigo[s + 6] == "CEX("
										&& codigo[s + 7] == "ARG36" || codigo[s + 7] == "ARG25"
										&& codigo[s + 8] == "CEX)"
										&& codigo[s + 8] == "FIIN")
									{
										
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SPR06";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										codigo[s + 4] = "";
										codigo[s + 5] = "";
										codigo[s + 6] = "";
										codigo[s + 7] = "";
										codigo[s + 8] = "";
										
										cat += "Far - Exito \n";
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
										&& codigo[s + 6] == "ASIG"
										&& codigo[s + 7] == "ARG67"
										&& codigo[s + 8] == "FIIN")
									{
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SPR13";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										codigo[s + 4] = "";
										codigo[s + 5] = "";
										codigo[s + 6] = "";
										codigo[s + 7] = "";
										codigo[s + 8] = "";
										if (NMat)
										{
											NMat = false;
										}
										cat += "Mat - Exito \n";
										break;
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
										&& codigo[s + 7] == "BLOXX7"
										&& codigo[s + 8] == "FIIN")
									{
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SPR16";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										codigo[s + 4] = "";
										codigo[s + 5] = "";
										codigo[s + 6] = "";
										codigo[s + 7] = "";
										codigo[s + 8] = "";
										if (NPor)
										{
											NPor = false;
										}
										cat += "Por - Exito \n";
										break;

									}
									else
									//PRX
									if (codigo[s + 0] == "PR17"
										&& codigo[s + 1] == "CEX("
										&& codigo[s + 2] == "ARG91"
										&& codigo[s + 3].Substring(0,2) == "ID"
										&& codigo[s + 4].Substring(0,2) == "ID"
										&& codigo[s + 5] == "CEX)"
										&& codigo[s + 6] == "INIS"
										&& codigo[s + 7] == "BLOXX6"
										&& codigo[s + 8] == "FIIN")
									{
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SPR17";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										codigo[s + 4] = "";
										codigo[s + 5] = "";
										codigo[s + 6] = "";
										codigo[s + 7] = "";
										codigo[s + 8] = "";
										if (NPrx)
										{
											NPrx = false;
										}
										cat += "Prx - Exito \n";
										break;
									}
									else
									//DUM 2
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR03"
										&& codigo[s + 2] == "CEX("
										&& codigo[s + 3] == "ARG25"
										&& codigo[s + 4] == "CEX)"
										&& codigo[s + 5] == "INIS"
										&& codigo[s + 6] == "BLOXX2"
										&& codigo[s + 7] == "FIIN"
										&& codigo[s + 8] == "FIIN")
									{
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SPR03";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										codigo[s + 4] = "";
										codigo[s + 5] = "";
										codigo[s + 6] = "";
										codigo[s + 7] = "";
										codigo[s + 8] = "";
										
										cat += "DUM - Exito \n";
										break;
									}
								}

								if (posicion >= 8)
								{
									//FAR
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR04"
										&& codigo[s + 2] == "BLOXX2"
										&& codigo[s + 3] == "PR03"
										&& codigo[s + 4] == "CEX("
										&& codigo[s + 5] == "ARG36" || codigo[s + 5] == "ARG25"
										&& codigo[s + 6] == "CEX("
										&& codigo[s + 7] == "FIIN")
									{
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SPR04";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										codigo[s + 4] = "";
										codigo[s + 5] = "";
										codigo[s + 6] = "";
										codigo[s + 7] = "";
										if (NFar)
										{
											NFar = false;
										}
										cat += "Far - Exito \n";
										break;
									}
									else
									//Sxa Kaz 2
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR08"
										&& codigo[s + 2] == "ARG53"
										&& codigo[s + 3] == "CEX:"
										&& codigo[s + 4] == "INIS"
										&& codigo[s + 5] == "BLOXX4"
										&& codigo[s + 6] == "FIIN"
										&& codigo[s + 7] == "FIIN")
									{
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SPR08";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										codigo[s + 4] = "";
										codigo[s + 5] = "";
										codigo[s + 6] = "";
										codigo[s + 7] = "";

										cat += "SXA/KAZ - Exito \n";
										break;
									}
									else
									//FAR 4
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR06"
										&& codigo[s + 2] == "INIS"
										&& codigo[s + 3] == "BLOXX2"
										&& codigo[s + 4] == "FIIN"
										&& codigo[s + 5] == "PR03"
										&& codigo[s + 6] == "ARG36" || codigo[s + 6] == "ARG25"
										&& codigo[s + 7] == "FIIN")
									{
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SPR06";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										codigo[s + 4] = "";
										codigo[s + 5] = "";
										codigo[s + 6] = "";
										codigo[s + 7] = "";

										cat += "FAR - Exito \n";
										break;
									}
									else
									//SXA KAZ 2
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR08"
										&& codigo[s + 2] == "ARG53"
										&& codigo[s + 3] == "CEX:"
										&& codigo[s + 4] =="INIS"
										&& codigo[s + 5] == "BLOXX4"
										&& codigo[s + 6] == "FIIN"
										&& codigo[s + 7] == "FIIN")
									{
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "CASOS";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										codigo[s + 4] = "";
										codigo[s + 5] = "";
										codigo[s + 6] = "";
										codigo[s + 7] = "";
										if (NSxa)
										{
											NSxa = false;
										}
										cat += "Sxa/Kaz - Exito\n";
										break;
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
										UltVar = "";
										sinCambios = 0;
										codigo[s] = "SPR21";
										codigo[s + 2] = "";
										codigo[s + 1] = "";
										codigo[s + 3] = "";
										codigo[s + 4] = "";
										codigo[s + 5] = "";
										codigo[s + 6] = "";
										if (NSe)
										{
											NSe = false;
										}
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
										UltVar = "";
										sinCambios = 0;
										codigo[s] = "SPR03";
										codigo[s + 2] = "";
										codigo[s + 1] = "";
										codigo[s + 3] = "";
										codigo[s + 4] = "";
										codigo[s + 5] = "";
										codigo[s + 6] = "";
										if (NDum)
										{
											NDum = false;
										}
										cat += "Dum - Exito \n";
										break;
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
										UltVar = "";
										sinCambios = 0;
										codigo[s] = "SPR13";
										codigo[s + 2] = "";
										codigo[s + 1] = "";
										codigo[s + 3] = "";
										codigo[s + 4] = "";
										codigo[s + 5] = "";
										codigo[s + 6] = "";
										if (NMat)
										{
											NMat = false;
										}
										cat += "Mat - Exito \n";
										break;
									}
									else
									//DUM 4
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR03"
										&& codigo[s + 2] == "ARG25"
										&& codigo[s + 3] == "INIS"
										&& codigo[s + 4] == "BLOXX2"
										&& codigo[s + 5] == "FIIN"
										&& codigo[s + 6] == "FIIN")
									{
										UltVar = "";
										sinCambios = 0;
										codigo[s] = "SPR03";
										codigo[s + 2] = "";
										codigo[s + 1] = "";
										codigo[s + 3] = "";
										codigo[s + 4] = "";
										codigo[s + 5] = "";
										codigo[s + 6] = "";
										cat += "Dum - Exito \n";
										break;
									}else
									//SXA SXA
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR22"
										&& codigo[s + 2] == "CEX("
										&& codigo[s + 3] == "ARG53"
										&& codigo[s + 4] == "CEX)"
										&& codigo[s + 5] == "CASOS"
										&& codigo[s + 6] == "FIIN")
									{
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SPR22";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										codigo[s + 4] = "";
										codigo[s + 5] = "";
										codigo[s + 6] = "";
										cat += "Sxa/Sxa - Exito\n";
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
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SPR09";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										codigo[s + 4] = "";
										codigo[s + 5] = "";
										if (NKap)
										{
											NKap = false;
										}
										cat += "Kap - Exito\n";
										break;
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
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SPR10";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										codigo[s + 4] = "";
										codigo[s + 5] = "";
										if (NKla)
										{
											NKla = false;
										}
										cat += "Kla - Exito\n";
										break;
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
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SPR12";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										codigo[s + 4] = "";
										codigo[s + 5] = "";
										if (NLeg)
										{
											NLeg = false;
										}
										cat += "Leg - Exito\n";
										break;
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
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SPR19";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										codigo[s + 4] = "";
										codigo[s + 5] = "";
										if (NRev)
										{
											NRev = false;
										}
										cat += "Rev - Exito\n";
										break;
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
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SPR24";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										codigo[s + 4] = "";
										codigo[s + 5] = "";
										if (NSkr)
										{
											NSkr = false;
										}
										cat += "Skr - Exito \n";
										break;
									}
									else
									//SXA SXA
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR22"
										&& codigo[s + 2] == "CEX("
										&& codigo[s + 3] == "ARG53"
										&& codigo[s + 4] == "CEX)"
										&& codigo[s + 5] == "FIIN")
									{
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SPR22";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										codigo[s + 4] = "";
										codigo[s + 5] = "";
										if (NSxa)
										{
											NSxa = false;
										}
										cat += "Sxa/Sxa - Exito\n";
										break;
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
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "CASOS";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										codigo[s + 4] = "";
										codigo[s + 5] = "";
										if (NSxa)
										{
											NSxa = false;
										}
										cat += "Sxa/Kaz - Exito\n";
										break;
									}
									else
									//Sxa Ali 2
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR01"
										&& codigo[s + 2] == "INIS"
										&& codigo[s + 3] == "BLOXX4"
										&& codigo[s + 4] == "FIIN"
										&& codigo[s + 5] == "FIIN")
									{
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SPR23";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										codigo[s + 4] = "";
										codigo[s + 5] = "";
										if (NSxn)
										{
											NSxn = false;
										}
										cat += "Sxn - Exito\n";
										break;
									}
									else
									//SKR 2
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR24"
										&& codigo[s + 2] == "CEX("
										&& codigo[s + 3] == "AGR4"
										&& codigo[s + 4] == "CEX)"
										&& codigo[s + 5] == "FIIN")
									{
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SPR24";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										codigo[s + 4] = "";
										codigo[s + 5] = "";
										if (NSkr)
										{
											NSkr = false;
										}
										cat += "SKR - Exito\n";
										break;
									}
									else
									//FAR 3
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR06"
										&& codigo[s + 2] == "BLOXX2"
										&& codigo[s + 3] == "PR03"
										&& codigo[s + 4] == "ARG36" || codigo[s + 4] == "ARG25"
										&& codigo[s + 5] == "FIIN")
									{
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SPR06";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										codigo[s + 4] = "";
										codigo[s + 5] = "";
										
										cat += "FAR - Exito\n";
										break;
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
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SPR10";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										codigo[s + 4] = "";
										if (NKla)
										{
											NKla = false;
										}
										cat += "Kla - Exito\n";
										break;
									}
									else
									//KLA 4
									if (codigo[s + 0] == "PR10"
										&& codigo[s + 1] == "ARG59"
										&& codigo[s + 2] == "INIS"
										&& codigo[s + 3] == "P10R1"
										&& codigo[s + 4] == "FIIN")
									{
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SPR10";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										codigo[s + 4] = "";
										if (NKla)
										{
											NKla = false;
										}
										cat += "Kla - Exito\n";
										break;
									}else
									//DUM 3
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR03"
										&& codigo[s + 2] == "ARG25"
										&& codigo[s + 3] == "BLOXX2"
										&& codigo[s + 4] == "FIIN")
									{
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SPR03";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										codigo[s + 4] = "";
										
										cat += "Dum - Exito\n";
										break;
									}
								}

								if (posicion >= 4)
								{
									//ENT 1
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR04"
										&& codigo[s + 2] == "DECP1"
										&& codigo[s + 3] == "FIIN")
									{
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SPR04";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										if (NEnt)
										{
											NEnt = false;
										}
										cat += "Ent - Exito\n";
										break;
									}
									else
									//ENT 2
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR04"
										&& codigo[s + 2] == "ARG31"
										&& codigo[s + 3] == "FIIN")
									{
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SPR04";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										if (NEnt)
										{
											NEnt = false;
										}
										cat += "Ent - Exito\n";
										break;
									}
									else
									//KAR
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR07"
										&& codigo[s + 2] == "DECP2"
										&& codigo[s + 3] == "FIIN")
									{
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SPR07";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										if (NKar)
										{
											NKar = false;
										}
										cat += "Kar - Exito\n";
										break;
									}
									else
									//KLA 1
									if (codigo[s + 0] == "PR10"
										&& codigo[s + 1] == "ARG59"
										&& codigo[s + 2] == "INIS"
										&& codigo[s + 3] == "FIIN")
									{
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SPR10";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										if (NKla)
										{
											NKla = false;
										}
										cat += "Kla - Exito\n";
										break;
									}
									else
									//LOG 
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR11"
										&& codigo[s + 2] == "DECP3"
										&& codigo[s + 3] == "FIIN")
									{
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SPR11";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										if (NLog)
										{
											NLog = false;
										}
										cat += "Log - Exito\n";
										break;
									}
									else
									//REA
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR18"
										&& codigo[s + 2] == "DECP4"
										&& codigo[s + 3] == "FIIN")
									{
										UltVar = "";
										sinCambios = 0;

										codigo[s + 0] = "SPR18";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										if (NRea)
										{
											NRea = false;
										}
										cat += "Rea - Exito\n";
										break;
									}
									else
									//SXA ALI/DEFAULT
									if (codigo[s + 0] == "PR01"
										&& codigo[s + 1] == "INIS"
										&& codigo[s + 2] == "BLOXX4"
										&& codigo[s + 3] == "FIIN")
									{
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "ALI";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										if (NSxa)
										{
											NSxa = false;
										}
										cat += "Sxa/Ali - Exito\n";
										break;
									}else
									//KAP 2
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR09"
										&& codigo[s + 2] == "ARG58"
										&& codigo[s + 3] == "FIIN")
									{
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SPR09";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										cat += "KAP - Exito\n";
										break;
									}
									else
									//LEG 2
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR12"
										&& codigo[s + 2] == "ARG63"
										&& codigo[s + 3] == "FIIN")
									{
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SPR12";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										cat += "LEG - Exito\n";
										break;
									}else
									//SXN
									if (codigo[s + 0] == "INIS"
										&& codigo[s + 1] == "PR23"
										&& codigo[s + 2] == "DECP5"
										&& codigo[s + 3] == "FIIN")
									{
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SPR23";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										cat += "LEG - Exito\n";
										break;
									}
									else
									//Metodo 1 SIMT
									if (codigo[s + 0] == "MARG1"
										&& codigo[s + 1] == "MDEC"
										&& codigo[s + 2] == "CEX)"
										&& codigo[s + 3] == "BRMT")
									{
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SIMT";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										cat += "METODO - Exito\n";
										break;
									}
									else
									//Metodo 4 SIMV
									if (codigo[s + 0] == "PR27"
										&& codigo[s + 1] == "MDEC"
										&& codigo[s + 2] == "CEX)"
										&& codigo[s + 3] == "BMET")
									{
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SIMV";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										codigo[s + 3] = "";
										cat += "METODO - Exito\n";
										break;
									}
								}
								if (posicion >= 3)
								{
									//SXA ROM
									if (codigo[s + 0] == "INIS"
									&& codigo[s + 1] == "PR20"
									&& codigo[s + 2] == "FIIN")
									{
										UltVar = "";
										sinCambios = 0;
										codigo[s + 0] = "SPR20";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										if (NSxa)
										{
											NSxa = false;
										}
										cat += "Sxa/Rom - Exito\n";
										break;
									}
									
								}
								if (posicion >= 2)
								{
									////METODO 5 SDMT
									//if (codigo[s + 0] == "MDEC"
									//&& codigo[s + 1] == "CEX)")
									//{
									//	UltVar = "";
									//	sinCambios = 0;
									//	codigo[s + 0] = "SDMT";
									//	codigo[s + 1] = "";
									//	//cat += "Sxa/Rom - Exito\n";
									//	break;
									//}
								}
								if (codigo[s] == "")//Para cualquier imprevisto
								{
									break;
								}
								if(codigo[s] == "FIIN")
								{
									modoBloque = true;
								}
								//PRX
								if (codigo[s] == "PR17")
								{
									NPrx = true;
									UltVar = "PR17";
								}
								if (UltVar == "PR17")
								{
									MensajeError2 = "PRX";
									//ARG91
									if (EsIgual(codigo[s], "PR04", "PR07", "PR11", "PR14", "PR18", "PR23"))
									{
										sinCambios = 0;
										codigo[s] = "ARG91";
										if (s != 0) { s--; }
									}
									//BLOXX6
									if (EsIgual(codigo[s], "SPR01", "SPR02", "SPR21", "SPR24", "SPR03", "SPR06", "SPR08", "SPR09", "SPR22", "SPR20", "SPR16", "DECX6"))
									{
										sinCambios = 0;
										codigo[s] = "BLOXX6";
										if (s != 0) { s--; }
									}
									//DECX6
									//if (EsIgual(codigo[s], "SPR04","SPR07","SPR11","SPR12","SPR13","SPR14","SPR17","SPR18","SPR23"))
									//{
									//	sinCambios = 0;
									//	codigo[s] = "DECX6";
									//	if(s != 0){s--;}
									//}


								}
								//SE
								if (codigo[s] == "PR21")
								{
									NSe = true;
									UltVar = "PR21";
								}
								if(UltVar == "PR21" )
								{
									MensajeError2 = "Se";
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
										if(s != 0){s--;}
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
									if (codigo[s] == "CONE" || codigo[s] == "CONR" || codigo[s] == "CDNA" || codigo[s] == "CRTR" || codigo[s].Substring(0, 2) == "ID" || codigo[s] == "ARG11" || codigo[s] == "ARG16")
									{
										sinCambios = 0;
										codigo[s] = "ARG13";
										if(s != 0){s--;}
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
									if (s >= vuelta && codigo[s - 2] == "ARG18" && codigo[s] == "ARG18" 
										&& (codigo[s - 1] == "OPSM" 
										||  codigo[s - 1] == "OPRS"
										||  codigo[s - 1] == "OPML"
										||  codigo[s - 1] == "OPDV"
										||  codigo[s - 1] == "OPEX"))
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG16";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG18
									if (codigo[s] == "ARG14" || codigo[s] == "ARG16" || codigo[s].Substring(0, 2) == "ID")
									{
										sinCambios = 0;
										codigo[s] = "ARG18";
										if(s != 0){s--;}
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
										|| codigo[s] == "DAR"
										|| codigo[s] == "DEC1")
									{
										sinCambios = 0;
										codigo[s] = "BLOQUEA1";
										if(s != 0){s--;}
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
										if(s != 0){s--;}
									}
									//DAR
									if (s >= vuelta && codigo[s - 2] == "INIS" && codigo[s - 1] == "PR02" && codigo[s] == "FIIN")
									{
										sinCambios = 0;
										codigo[s - 2] = "DAR";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
								}
								
								
								//FAR
								if (codigo[s] == "PR06")
								{
									NDum = false;
									UltVar = "PR06";
									modoBloque = true;
								}
								if (UltVar == "PR06")
								{
									MensajeError2 = "Far";
									//ARG36
									if (s >= vuelta && codigo[s - 2] == "CEX(" && codigo[s - 1] == "ARG36" && codigo[s] == "CEX)")
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG36";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG36
									if (EsIgual(codigo[s], "ARG37", "ARG39"))
									{
										sinCambios = 0;
										codigo[s] = "ARG36";
										if(s != 0){s--;}
									}
									//ARG39
									if (s >= vuelta && codigo[s - 2] == "CEX(" && codigo[s - 1] == "ARG39" && codigo[s] == "CEX)")
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG39";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG39
									if (s >= vuelta && (EsIgual(codigo[s-2], "CONE", "CONR", "CDNA", "CRTR", "ARG37", "ARG43") || codigo[s-2].Substring(0, 2) == "ID") && (EsIgual(codigo[s], "CONE", "CONR", "CDNA", "CRTR", "ARG37", "ARG43") || codigo[s].Substring(0, 2) == "ID")
										&& SiOpRelacional(codigo[s - 1]))
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG39";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG37
									if (s >= vuelta && codigo[s - 2] == "CEX(" && codigo[s - 1] == "ARG37" && codigo[s] == "CEX)")
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG37";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG37
									if (s >= vuelta 
										&& (EsIgual(codigo[s-2], "CONE", "CONR", "CDNA", "CRTR", "ARG37", "ARG43") || codigo[s-2].Substring(0, 2) == "ID") 
										&& (EsIgual(codigo[s], "CONE", "CONR", "CDNA", "CRTR", "ARG37", "ARG43") || codigo[s].Substring(0, 2) == "ID")
										&& SiOpLogico(codigo[s - 1]))
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG37";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									
									//ARG43
									if (s >= vuelta && codigo[s - 2] == "CEX(" && codigo[s - 1] == "ARG43" && codigo[s] == "CEX)")
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG43";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG43
									if (s >= vuelta && (codigo[s - 2] == "CONE" || codigo[s - 2] == "CONR" || codigo[s - 2] == "ARG43" || codigo[s-2].Substring(0, 2) == "ID") && (codigo[s] == "CONE" || codigo[s] == "CONR" || codigo[s] == "ARG43" || codigo[s].Substring(0, 2) == "ID")
										&& SiOpAritmetico(codigo[s - 1]))
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG43";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//BLOXX2
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
										|| codigo[s] == "DECX2")
									{
										sinCambios = 0;
										codigo[s] = "BLOXX2";
										if(s != 0){s--;}
									}
									//DECX2
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
										codigo[s] = "DECX2";
										if(s != 0){s--;}
									}
								}
								//DUM

								if (codigo[s] == "PR03")
								{
									modoBloque = true;
									UltVar = "PR03";
								}
								if (UltVar == "PR03" )
								{
									MensajeError2 = "DUM";
									//ARG25
									if (s >= vuelta && codigo[s - 2] == "CEX(" && codigo[s - 1] == "ARG25" && codigo[s] == "CEX)")
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG25";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG25
									if (codigo[s] == "ARG28" || codigo[s] == "ARG26")
									{
										sinCambios = 0;
										codigo[s] = "ARG25";
										if (s != 0) { s--; }
									}
									//ARG26
									if (s >= vuelta && codigo[s - 2] == "CEX(" && codigo[s - 1] == "ARG26" && codigo[s] == "CEX)")
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG26";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG26 Relx
									if (s >= vuelta && (EsIgual(codigo[s - 2], "CONE", "CONR", "CDNA", "CRTR", "ARG28", "ARG30") || codigo[s - 2].Substring(0, 2) == "ID") && (EsIgual(codigo[s], "CONE", "CONR", "CDNA", "CRTR", "ARG28", "ARG30") || codigo[s].Substring(0, 2) == "ID")
										&& SiOpRelacional(codigo[s - 1]))
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG26";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG28
									if (s >= vuelta && codigo[s - 2] == "CEX(" && codigo[s - 1] == "ARG28" && codigo[s] == "CEX)")
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG28";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG28 Logx
									if (s >= vuelta && (EsIgual(codigo[s - 2], "CONE", "CONR", "CDNA", "CRTR", "ARG28", "ARG30") || codigo[s - 2].Substring(0, 2) == "ID") && (EsIgual(codigo[s], "CONE", "CONR", "CDNA", "CRTR", "ARG28", "ARG30") || codigo[s].Substring(0, 2) == "ID")
										&& SiOpLogico(codigo[s - 1]))
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG28";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}

									//ARG30
									if (s >= vuelta && codigo[s - 2] == "CEX(" && codigo[s - 1] == "ARG30" && codigo[s] == "CEX)")
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG30";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG30 Aritx
									if (s >= vuelta && (EsIgual(codigo[s - 2], "CONE", "CONR", "ARG30") || codigo[s - 2].Substring(0, 2) == "ID") && (EsIgual(codigo[s], "CONE", "CONR", "ARG30") || codigo[s].Substring(0, 2) == "ID")
										&& SiOpAritmetico(codigo[s - 1]))
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG30";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									if (s >= vuelta - 1 && codigo[s - 1] == "BLOXX2" && codigo[s] == "BLOXX2")
									{
										sinCambios = 0;
										codigo[s] = "BLOXX2";
										codigo[s] = "";
										if (s != 0) { s--; }
									}
									//BLOXX2
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
										|| codigo[s] == "SPR16" || codigo[s] == "ASG1"
										|| codigo[s] == "DECX2")
									{
										sinCambios = 0;
										codigo[s] = "BLOXX2";
										break;
									}
									//DECX2
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
										codigo[s] = "DECX2";
										if (s != 0) { s--; }
									}
									//ARG35
									if (s >= vuelta && codigo[s - 2].Substring(0, 2) == "ID" && codigo[s - 1] == "ASIG" && EsIgual(codigo[s], "ARG30", "CONE"))
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG35";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ASIG1
									if (s >= vuelta && codigo[s - 2] == "INIS" && codigo[s - 1] == "ARG35" && codigo[s] == "FIIN")
									{
										sinCambios = 0;
										codigo[s - 2] = "ASG1";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
								}

								//KAP
								if (codigo[s] == "PR09")
								{
									NKap = true;
									UltVar = "PR09";
								}
								if ( UltVar == "PR09")
								{
									MensajeError2 = "KAP";
									//ARG58
									if (s >= vuelta && codigo[s - 2] == "CEX(" && codigo[s - 1].Substring(0, 2) == "ID" && codigo[s] == "CEX)")
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG58";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG58
									if(codigo[s].Substring(0, 2) == "ID")
									{
										sinCambios = 0;
										codigo[s] = "ARG58";
										if(s != 0){s--;}
									}
									
								}
								//KAR
								if (codigo[s] == "PR07")
								{
									NKar = true;
									UltVar = "PR07";
								}
								if (UltVar == "PR07" && !modoBloque)
								{
									MensajeError2 = "KAR";
									//DECP2
									if (s >= vuelta && codigo[s - 2] == "DECP2" && codigo[s - 1] == "CEX," && codigo[s] == "DECP2")
									{
										sinCambios = 0;
										codigo[s - 2] = "DECP2";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//DECP2
									if (s >= vuelta + 1 && codigo[s - 2].Substring(0, 2) == "ID" && codigo[s - 1] == "ASIG" && (codigo[s] == "CRTR"))
									{
										sinCambios = 0;
										codigo[s - 2] = "DECP2";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//DECP2
									if (s >= vuelta + 1 && (codigo[s - 3] == "DECP2" || codigo[s - 3].Substring(0, 2) == "ID") && codigo[s - 2] == "CEX," && (codigo[s - 1] == "DECP2" || codigo[s - 1].Substring(0, 2) == "ID") && codigo[s] != "ASIG")
									{
										sinCambios = 0;
										codigo[s - 3] = "DECP2";
										codigo[s - 2] = "";
										codigo[s - 1] = "";
										break;
									}
								}
								//KLA
								if (codigo[s] == "PR10")
								{
									NKla = true;
									UltVar = "PR10";
								}
								if (UltVar == "PR10")
								{
									MensajeError2 = "KLA";
									//P10R1
									if (s >= 7
										&& codigo[s - 6] == "INIS"
										&& codigo[s - 5].Substring(0, 2) == "ID"
										&& codigo[s - 4] == "CEX("
										&& codigo[s - 3] == "DECX5"
										&& codigo[s - 2] == "CEX)"
										&& codigo[s - 1] == "P10R2"
										&& codigo[s] == "FIIN")
									{
										sinCambios = 0;
										codigo[s - 6] = "P10R1";
										codigo[s - 5] = "";
										codigo[s - 4] = "";
										codigo[s - 3] = "";
										codigo[s - 2] = "";
										codigo[s - 1] = "";
										codigo[s] = "";
										if(s != 0){s--;}
									}
									//P10R1
									if (s >= 9
										&& codigo[s - 8] == "INIS"
										&& codigo[s - 7].Substring(0, 2) == "ID"
										&& codigo[s - 6] == "CEX("
										&& codigo[s - 5] == "DECX5"
										&& codigo[s - 4] == "CEX,"
										&& codigo[s - 3] == "DECX5"
										&& codigo[s - 2] == "CEX)"
										&& codigo[s - 1] == "P10R2"
										&& codigo[s] == "FIIN")
									{
										sinCambios = 0;
										codigo[s - 8] = "P10R1";
										codigo[s - 7] = "";
										codigo[s - 6] = "";
										codigo[s - 5] = "";
										codigo[s - 4] = "";
										codigo[s - 3] = "";
										codigo[s - 2] = "";
										codigo[s - 1] = "";
										codigo[s] = "";
										if(s != 0){s--;}
									}
									//P10R2
									if (s >= vuelta && codigo[s - 2] == "CEX(" && codigo[s - 1] == "ASIGNP2" && codigo[s] == "CEX)")
									{
										sinCambios = 0;
										codigo[s - 2] = "P10R2";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//BLOXX5
									if (EsIgual(codigo[s], "SPR01", "SPR21", "SPR02", "SPR24", "SPR03", "SPR06", "SPR08", "SPR09", "SPR22", "SPR20", "SPR16", "DECX5"))
									{
										sinCambios = 0;
										codigo[s] = "BLOXX5";
										if(s != 0){s--;}
									}
									//DECFX5
									if(s >= vuelta-1 && codigo[s-1] =="DECFX5" && codigo[s] == "DECFX5")
									{
										sinCambios = 0;
										codigo[s - 1] = "DECFX5";
										codigo[s] = "";
										break;
									}
									//DECFX5
									if (s >= vuelta && codigo[s - 2] == "INIS" && codigo[s - 1] == "DECXF5" && codigo[s] == "FIIN")
									{
										sinCambios = 0;
										codigo[s - 2] = "DECXF5";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//DECX5
									if (EsIgual(codigo[s], "SPR04", "SPR07", "SPR11", "SPR12", "SPR13", "SPR14", "SPR17", "SPR18", "SPR23") )
									{
										sinCambios = 0;
										codigo[s] = "DECX5";
										if(s != 0){s--;}
									}
								}
								//LEG
								if (codigo[s] == "PR12")
								{
									NLeg = true;
									UltVar = "PR12";
								}
								if (UltVar == "PR12")
								{
									MensajeError2 = "LEG";
									//ARG63
									if (EsIgual(codigo[s], "CONE", "CONR", "SPR15"))
									{
										sinCambios = 0;
										codigo[s] = "ARG63";
										if(s != 0){s--;}
									}
									//ARG63
									if (s >= vuelta && codigo[s - 2] == "CEX(" && codigo[s - 1] == "ARG63" && codigo[s] == "CEX)")
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG63";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
								}
								//LOG
								if (codigo[s] == "PR11")
								{
									NLog = true;
									UltVar = "PR11";
								}
								if (UltVar == "PR11" && !modoBloque)
								{
									MensajeError2 = "LOG";
									//BLN
									//if (EsIgual(codigo[s], "PR05", "PR25"))
									//{
									//	sinCambios = 0;
									//	codigo[s] = "BLN";
									//	if(s != 0){s--;}
									//}
									//DECP3
									if(s >= vuelta && (codigo[s - 2] == "ARG38" || codigo[s - 2] == "DECP3") && codigo[s - 1] == "CEX," && codigo[s] == "DECP3")
									{
										sinCambios = 0;
										codigo[s - 2] = "DECP3";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//DECP3
									if (s >= vuelta+1 && codigo[s - 3] == "DECP3"  && codigo[s - 2] == "CEX," && codigo[s - 1] == "ARG38" && codigo[s] != "ASIG")
									{
										sinCambios = 0;
										codigo[s - 3] = "DECP3";
										codigo[s - 2] = "";
										codigo[s - 1] = "";
										break;
									}
									//DECP3
									if (s >= vuelta && codigo[s - 2] == "ARG38"  && codigo[s - 1] == "ASIG" && EsIgual(codigo[s], "PR05", "PR25"))
									{
										sinCambios = 0;
										codigo[s - 2] = "DECP3";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG38
									if(s >= vuelta- 1 && codigo[s-1].Substring(0,2) == "ID" && codigo[s] != "ASIG")
									{
										sinCambios = 0;
										codigo[s-1] = "ARG38";
										if (s != 0) { s--; }
									}
								}
								//MAT
								if (codigo[s] == "PR13")
								{
									NMat = true;
									UltVar = "PR13";
									modoBloque = true;
								}
								if (UltVar == "PR13")
								{
									MensajeError2 = "MAT";
									//ARG65
									if (EsIgual(codigo[s], "PR04", "PR07", "PR11", "PR18", "PR23"))
									{
										sinCambios = 0;
										codigo[s] = "ARG65";
										if(s != 0){s--;}
									}
									

									////ARG69
									//if (s >= vuelta && codigo[s - 2] == "CEX(" && codigo[s - 1] == "ARG69" && codigo[s] == "CEX)")
									//{
									//	sinCambios = 0;
									//	codigo[s - 2] = "ARG69";
									//	codigo[s - 1] = "";
									//	codigo[s] = "";
									//	break;
									//}
									////ARG69
									//if (s >= vuelta && (EsIgual(codigo[s-2], "CONE", "CONR", "CDNA", "CRTR", "ARG69", "ARG71") || codigo[s-2].Substring(0, 2) == "ID") && (EsIgual(codigo[s], "CONE", "CONR", "CDNA", "CRTR", "ARG69", "ARG71") || codigo[s].Substring(0, 2) == "ID")
									//	&& SiOpLogico(codigo[s - 1]))
									//{
									//	sinCambios = 0;
									//	codigo[s - 2] = "ARG69";
									//	codigo[s - 1] = "";
									//	codigo[s] = "";
									//	break;
									//}
									////ARG71
									//if (s >= vuelta && codigo[s - 2] == "CEX(" && codigo[s - 1] == "ARG71" && codigo[s] == "CEX)")
									//{
									//	sinCambios = 0;
									//	codigo[s - 2] = "ARG71";
									//	codigo[s - 1] = "";
									//	codigo[s] = "";
									//	break;
									//}
								
									////ARG71
									//if (s >= vuelta && (EsIgual(codigo[s-2], "CONE", "CONR", "ARG71") || codigo[s-2].Substring(0, 2) == "ID") && (EsIgual(codigo[s], "CONE", "CONR", "ARG71") || codigo[s].Substring(0, 2) == "ID")
									//	&& SiOpAritmetico(codigo[s - 1]))
									//{
									//	sinCambios = 0;
									//	codigo[s - 2] = "ARG71";
									//	codigo[s - 1] = "";
									//	codigo[s] = "";
									//	break;
									//}
								}
								//POR
								if (codigo[s] == "PR16")
								{
									NPor = true;
									UltVar = "PR16";
									modoBloque = true;
								}
								if ( UltVar == "PR16")
								{
			
									MensajeError2 = "POR";
									//P16R1
									if (codigo[s] == "ARG89")
									{
										sinCambios = 0;
										codigo[s] = "P16R1";
										if (s != 0) { s--; }
									}
									//P16R2
									if (s >= 3
										&& codigo[s - 2] == "INIS"
										&& codigo[s - 1] == "ARG75"
										&& codigo[s] == "FIIN")
									{
										sinCambios = 0;
										codigo[s - 2] = "P16R2";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//P16R3
									if (s >= 3
										&& codigo[s - 2] == "INIS"
										&& codigo[s - 1] == "ARG85"
										&& codigo[s] == "FIIN")
									{
										sinCambios = 0;
										codigo[s - 2] = "P16R3";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG75
									if (s >= vuelta && codigo[s - 2] == "CEX(" && codigo[s - 1] == "ARG75" && codigo[s] == "CEX)")
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG75";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG75
									if (s >= 3
										&& (EsIgual(codigo[s - 2], "CONE", "CONR", "CDNA", "CRTR", "ARG77", "ARG81") || codigo[s - 2].Substring(0, 2) == "ID")
										&& (EsIgual(codigo[s], "CONE", "CONR", "CDNA", "CRTR", "ARG77", "ARG81") || codigo[s].Substring(0, 2) == "ID")
										&& SiOpRelacional(codigo[s - 1]))
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG75";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG77
									if (s >= vuelta && codigo[s - 2] == "CEX(" && codigo[s - 1] == "ARG77" && codigo[s] == "CEX)")
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG77";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG77
									if (s >= 3
										&& (EsIgual(codigo[s - 2], "CONE", "CONR", "CDNA", "CRTR", "ARG77", "ARG81") || codigo[s - 2].Substring(0, 2) == "ID")
										&& (EsIgual(codigo[s], "CONE", "CONR", "CDNA", "CRTR", "ARG77", "ARG81") || codigo[s].Substring(0, 2) == "ID")
										&& SiOpLogico(codigo[s - 1]))
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG77";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG81
									if (s >= vuelta && codigo[s - 2] == "CEX(" && codigo[s - 1] == "ARG81" && codigo[s] == "CEX)")
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG81";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG81
									if (s >= 3
										&& (EsIgual(codigo[s - 2], "CONE", "CONR", "ARG81") || codigo[s - 2].Substring(0, 2) == "ID")
										&& (EsIgual(codigo[s], "CONE", "CONR", "ARG81") || codigo[s].Substring(0, 2) == "ID")
										&& SiOpLogico(codigo[s - 1]))
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG81";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//BLOXX7
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
										|| codigo[s] == "DECX7")
									{
										sinCambios = 0;
										codigo[s] = "BLOXX7";
										if(s != 0){s--;}
									}
									//DECX7
									//if (codigo[s] == "SPR04"
									//	|| codigo[s] == "SPR07"
									//	|| codigo[s] == "SPR11"
									//	|| codigo[s] == "SPR12"
									//	|| codigo[s] == "SPR13"
									//	|| codigo[s] == "SPR14"
									//	|| codigo[s] == "SPR17"
									//	|| codigo[s] == "SPR18"
									//	|| codigo[s] == "SPR23")
									//{
									//	sinCambios = 0;
									//	codigo[s] = "DECX7";
									//	if(s != 0){s--;}
									//}
									//ARG85
									if(s >= 3 ) //por alguna razon 0 es mayor que 3??XD
									{
										if(codigo[s - 2].Substring(0, 2) == "ID" && ((codigo[s - 1] == "OPSM" && codigo[s] == "CONE") || (codigo[s - 1] == "OPRS" && codigo[s] == "CONE")))
										{
											sinCambios = 0;
											codigo[s - 2] = "ARG85";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										
									}
									//ARG85
									if (s >= 2 && codigo[s - 1].Substring(0, 2) == "ID" && (codigo[s] == "OPSM" || codigo[s] == "OPRS") )
									{
										sinCambios = 0;
										codigo[s - 1] = "ARG85";
										codigo[s] = "";
										break;
									}
									//ARG89
									if (EsIgual(codigo[s],"SPR04","SPR07","SPR11","SPR14","SPR18","SPR23"))
									{
										sinCambios = 0;
										codigo[s] = "ARG89";
										if (s != 0) { s--; }
									}

								}
								
								//REA
								if (codigo[s] == "PR18")
								{
									NRea = true;
									UltVar = "PR18";
								}
								if (UltVar == "PR18" && !modoBloque)
								{
									MensajeError2 = "REA";
									//ARG74
									if (s >= vuelta - 1 && codigo[s - 1].Substring(0, 2) == "ID" && codigo[s] != "ASIG")
									{
										sinCambios = 0;
										codigo[s - 1] = "ARG74";
										if (s != 0) { s--; }
									}
									//DECP4
									if (s >= vuelta + 1 && codigo[s - 3] == "DECP4" && codigo[s - 2] == "CEX," && (codigo[s - 1] == "ARG74") && codigo[s] != "ASIG")
									{
										sinCambios = 0;
										codigo[s - 3] = "DECP4";
										codigo[s - 2] = "";
										codigo[s - 1] = "";
										break;
									}
									//DECP4
									if (s >= vuelta && (codigo[s - 2] == "DECP4" || codigo[s - 2] == "ARG74" || codigo[s-2].Substring(0,2) == "ID") && codigo[s - 1] == "ASIG" && (codigo[s] == "ASIGNP4" || codigo[s] == "CONE" || codigo[s] == "CONR") )
									{
										sinCambios = 0;
										codigo[s - 2] = "DECP4";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ASIGNP4
									if (s >= vuelta && codigo[s - 2] == "CEX(" && codigo[s - 1] == "ASIGNP4" && codigo[s] == "CEX)")
									{
										sinCambios = 0;
										codigo[s - 2] = "ASIGNP4";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ASIGNP4
									if (s >= vuelta && (codigo[s - 2] == "ASIGNP4" || codigo[s - 2] == "CONE" || codigo[s - 2] == "CONR" || codigo[s - 2] == "ARG74")
										&& (codigo[s] == "ASIGNP4" || codigo[s] == "CONE" || codigo[s] == "CONE" || codigo[s]== "ARG74")
										&& SiOpAritmetico(codigo[s - 1]))
									{
										sinCambios = 0;
										codigo[s - 2] = "ASIGNP4";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//DECP4
									if (s >= vuelta && (codigo[s - 2] == "ARG74" || codigo[s - 2] == "DECP4") && codigo[s - 1] == "CEX," && codigo[s] == "DECP4")
									{
										sinCambios = 0;
										codigo[s - 2] = "DECP4";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									
	
								}
								//REV
								if (codigo[s] == "PR19")
								{
									NRev = true;
									UltVar = "PR19";
								}
								if (UltVar == "PR19")
								{
									MensajeError2 = "rev";
									//ARG100
									if (EsIgual(codigo[s],"CONE","CONR","CDNA","CRTR","ARG95","ARG96") || codigo[s].Substring(0, 2) == "ID")
									{
										sinCambios = 0;
										codigo[s] = "ARG100";
										if(s != 0){s--;}
									}
									//ARG95
									if (s >= vuelta && codigo[s - 2] == "CEX(" && codigo[s - 1] == "ARG95" && codigo[s] == "CEX)")
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG95";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG95
									if (s >= vuelta 
										&& (EsIgual(codigo[s], "CONE", "CONR", "CDNA", "CRTR", "ARG95", "ARG96") || codigo[s].Substring(0, 2) == "ID")
										&& SiOpLogico(codigo[s-1]) 
										&& (EsIgual(codigo[s], "CONE", "CONR", "CDNA", "CRTR", "ARG95", "ARG96") || codigo[s].Substring(0, 2) == "ID"))
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG95";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									
									//ARG96
									if (s >= vuelta && codigo[s - 2] == "CEX(" && codigo[s - 1] == "ARG96" && codigo[s] == "CEX)")
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG96";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG96
									if (s >= vuelta && codigo[s - 2].Substring(0, 2) == "ID" && SiOpAritmetico(codigo[s - 1]) && codigo[s].Substring(0, 2) == "ID")
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG96";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
								}
								//SKR
								if (codigo[s] == "PR24")
								{
									NSkr = true;
									UltVar = "PR24";
								}
								if (UltVar == "PR24")
								{
									MensajeError2 = "SKR";
									//AGR4
									if (s >= vuelta - 1 && (codigo[s - 1] == "AGR4" || (EsIgual(codigo[s-1], "CDNA", "CRTR", "CONE", "CONR","AGR4","SDMT") || codigo[s-1].Substring(0, 2) == "ID")) && codigo[s] == "AGR4")
									{
										sinCambios = 0;
										codigo[s - 1] = "AGR4";
										codigo[s] = "";
										break;
									}
									//AGR4
									if (EsIgual(codigo[s], "CDNA", "CRTR", "CONE", "CONR","SDMT") || codigo[s].Substring(0, 2) == "ID")
									{
										sinCambios = 0;
										codigo[s] = "AGR4";
										break;//Por si acaso
									}
									//AGR4
									if (s >= vuelta - 1 && codigo[s - 1] == "OPSM" && (EsIgual(codigo[s], "CDNA","CRTR","CONE","CONR","AGR4","SDMT") || codigo[s].Substring(0, 2) == "ID"))
									{
										sinCambios = 0;
										codigo[s - 1] = "AGR4";
										codigo[s] = "";
										break;
									}
									//AGR4
									if (s >= vuelta - 1 && codigo[s - 1] == "AGR4" && (EsIgual(codigo[s-1], "CDNA", "CRTR", "CONE", "CONR","AGR4","SDMT") || codigo[s-1].Substring(0, 2) == "ID"))
									{
										sinCambios = 0;
										codigo[s - 1] = "AGR4";
										codigo[s] = "";
										break;
									}

								}
								//SXA KAZ ROM
								if (codigo[s] == "PR22")
								{
									NSxa = true;
									UltVar = "PR22";
								}
								if (UltVar == "PR22")
								{
									MensajeError2 = "SXA";
									//ARG53
									if (EsIgual(codigo[s], "CDNA","CONE","CONR","CRTR")||codigo[s].Substring(0,2) =="ID")
									{
										sinCambios = 0;
										codigo[s] = "ARG53";
										if(s != 0){s--;}
									}
									//CASOS
									if(s >= vuelta-1 && codigo[s-1] == "CASOS" && (codigo[s]=="CASOS"|| codigo[s]=="ALI"))
									{
										sinCambios = 0;
										codigo[s - 1] = "CASOS";
										codigo[s] = "";
										break;
									}
									//BLOXX4
									if (s >= vuelta && codigo[s - 2] == "INIS" && codigo[s - 1] == "BLOXX4" && codigo[s] == "FIIN")
									{
										sinCambios = 0;
										codigo[s - 2] = "BLOXX4";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//BLOXX4
									if (s >= vuelta - 1 && codigo[s - 1] == "BLOXX4" && codigo[s] == "BLOXX4" )
									{
										sinCambios = 0;
										codigo[s - 1] = "BLOXX4";
										codigo[s] = "";
										break;
									}
									//BLOXX4
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
										|| codigo[s] == "SPR16" || codigo[s] == "SPR20"
										|| codigo[s] == "DECX4")
									{
										sinCambios = 0;
										codigo[s] = "BLOXX4";
										if(s != 0){s--;}
									}
									//DECX4
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
										codigo[s] = "DECX4";
										if(s != 0){s--;}
									}
								}
								//SXN
								if (codigo[s] == "PR23")
								{
									NSxn = true;
									UltVar = "PR23";
								}
								if (UltVar == "PR23" && !modoBloque)
								{
									MensajeError2 = "SXN";
									//DECP5
									if (s >= vuelta && codigo[s - 2] == "DECP5" && codigo[s - 1] == "CEX," && codigo[s] == "DECP2")
									{
										sinCambios = 0;
										codigo[s - 2] = "DECP5";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									
									//DECP5
									if (s >= vuelta + 1 && (codigo[s - 3] == "DECP5" || codigo[s - 3].Substring(0, 2) == "ID") && codigo[s - 2] == "CEX," && (codigo[s - 1] == "DECP5" || codigo[s - 1].Substring(0, 2) == "ID" || codigo[s-1] == "ARG56") && codigo[s] != "ASIG")
									{
										sinCambios = 0;
										codigo[s - 3] = "DECP5";
										codigo[s - 2] = "";
										codigo[s - 1] = "";
										break;
									}
									//AGR5
									if (s >= vuelta && codigo[s - 2] == "CEX" && codigo[s - 1] == "AGR5" && codigo[s] == "CEX)")
									{
										sinCambios = 0;
										codigo[s - 2] = "AGR5";
										codigo[s - 1] = "";
										codigo[s ] = "";
										break;
									}
									//AGR5
									if( s >= 2 && (codigo[s-1] == "AGR5" || codigo[s-1] == "ARG56") && codigo[s] == "AGR5")
									{
										sinCambios = 0;
										codigo[s - 1] = "AGR5";
										codigo[s] = "";
										break;

									}
									//DECP5
									if (s >= vuelta && codigo[s - 2] == "DECP5" && codigo[s - 1] == "OPSM" && (codigo[s] == "ARG56" || codigo[s].Substring(0, 2) == "ID"))
									{
										sinCambios = 0;
										codigo[s - 2] = "DECP5";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//AGR5
									if (s >= vuelta && (codigo[s-2] == "ARG56" || codigo[s-2].Substring(0, 2) == "ID") && codigo[s - 1] == "OPSM"  && (codigo[s] == "ARG56" || codigo[s].Substring(0,2) == "ID"))
									{
										sinCambios = 0;
										codigo[s - 2] = "AGR5";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG56
									if (s >= vuelta && codigo[s - 2]== "CEX(" && codigo[s - 1] == "ARG56" && codigo[s] == "CEX)")
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG56";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//DECP5
									if (s >= vuelta && codigo[s - 2].Substring(0, 2) == "ID" && codigo[s - 1] == "ASIG" && (codigo[s] == "AGR5" || codigo[s] == "ARG56"))
									{
										sinCambios = 0;
										codigo[s - 2] = "DECP5";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG56
									if (EsIgual(codigo[s],"CDNA","CONE","CONR","CRTR","AGR5"))
									{
										sinCambios = 0;
										codigo[s] = "ARG56";
										if (s != 0) { s--; }
									}
								}
								//ENT
								if (codigo[s] == "PR04")
								{
									NEnt = true;
									UltVar = "PR04";
								}
								if (UltVar == "PR04" && !modoBloque)
								{
									MensajeError2 = "ent";
									//DECP1
									if (s >= vuelta && codigo[s - 2] == "DECP1" && codigo[s - 1] == "CEX," && codigo[s] == "DECP1")
									{
										sinCambios = 0;
										codigo[s - 2] = "DECP1";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									if (sinCambios >= 1 && s >= vuelta && (codigo[s - 2] == "DECP1" || codigo[s - 2] == "ARG31") && codigo[s - 1] == "CEX," && (codigo[s] == "DECP1" || codigo[s] == "ARG31"))
									{
										sinCambios = 0;
										codigo[s - 2] = "DECP1";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//DECP1
									if (s >= vuelta && codigo[s - 2] == "ARG31" && codigo[s - 1] == "ASIG" && (codigo[s] == "ASIGNP1" || codigo[s] == "CONE"))
									{
										sinCambios = 0;
										codigo[s - 2] = "DECP1";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}

									//ASIGNP1
									if (s >= vuelta && codigo[s - 2] == "CEX(" && codigo[s - 1] == "ASIGNP1" && codigo[s] == "CEX)")
									{
										sinCambios = 0;
										codigo[s - 2] = "ASIGNP1";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ASIGNP1
									if (s >= vuelta && (codigo[s - 2] == "ASIGNP1" || codigo[s - 2] == "CONE" || codigo[s - 2] == "ARG31") && (codigo[s] == "ASIGNP1" || codigo[s] == "CONE" || codigo[s] == "ARG31")
										&& codigo[s - 1] == "ASIGNP1")
									{
										sinCambios = 0;
										codigo[s - 2] = "ASIGNP1";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG31
									if (codigo[s].Substring(0, 2) == "ID")
									{
										sinCambios = 0;
										codigo[s] = "ARG31";
										if (s != 0) { s--; }
									}
									
								}
								//OBJ
								if (codigo[s] == "PR15")
								{
									NObj = true;
									UltVar = "PR15";
								}
								if (UltVar == "PR15")
								{
									MensajeError2 = "OBJ";
								}
								//Variable pura
								{
									//Zona MAT
									//ARG69
									if (s >= vuelta && codigo[s - 2] == "CEX(" && codigo[s - 1] == "ARG69" && codigo[s] == "CEX)")
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG69";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG69
									if (s >= vuelta && (EsIgual(codigo[s - 2], "CONE", "CONR", "CDNA", "CRTR", "ARG69", "ARG71") || codigo[s - 2].Substring(0, 2) == "ID") && (EsIgual(codigo[s], "CONE", "CONR", "CDNA", "CRTR", "ARG69", "ARG71") || codigo[s].Substring(0, 2) == "ID")
										&& SiOpLogico(codigo[s - 1]))
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG69";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARG71
									if (s >= vuelta && codigo[s - 2] == "CEX(" && codigo[s - 1] == "ARG71" && codigo[s] == "CEX)")
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG71";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}

									//ARG71
									if (s >= vuelta && (EsIgual(codigo[s - 2], "CONE", "CONR", "ARG71") || codigo[s - 2].Substring(0, 2) == "ID") && (EsIgual(codigo[s], "CONE", "CONR", "ARG71") || codigo[s].Substring(0, 2) == "ID")
										&& SiOpAritmetico(codigo[s - 1]))
									{
										sinCambios = 0;
										codigo[s - 2] = "ARG71";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//SPR13
									if (codigo.Length >= 3 && codigo[s] == "INIS" && codigo[s + 1] == "AASG" && codigo[s + 2] == "FIIN")
									{
										sinCambios = 0;
										codigo[s] = "SPR13";
										codigo[s + 1] = "";
										codigo[s + 2] = "";
										cat += "Mat - Exito";
										break;
									}
									//AASG
									if (s >= vuelta && codigo[s - 2] == "AASG" && codigo[s - 1] == "CEX," && codigo[s] == "AASG")
									{
										sinCambios = 0;
										codigo[s - 2] = "AASG";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//AASG
									if (s >= vuelta && codigo[s - 2] == "ARRAY" && codigo[s - 1] == "ASIG" && (EsIgual(codigo[s], "CONE", "CONR", "CDNA", "CRTR", "ARG69", "ARG71") || codigo[s].Substring(0, 2) == "ID"))
									{
										sinCambios = 0;
										codigo[s - 2] = "AASG";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//AASG
									if(s >= vuelta-1 && codigo[s-1] == "ARRAY" && codigo[s] != "ASIG" )
									{
										sinCambios = 0;
										codigo[s-1] = "AASG";
										if (s != 0) { s--; }
									}
									//ARRAY
									if (s >= vuelta && codigo[s - 2] == "ARRY" && codigo[s - 1] == "CONE" && codigo[s] == "CEX]")
									{
										sinCambios = 0;
										codigo[s - 2] = "ARRAY";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//ARRAY
									if (s >= 4 && codigo[s - 3].Substring(0,2) == "ID" && codigo[s - 2] == "CEX[" && codigo[s - 1] == "CONE" && codigo[s] == "CEX]")
									{
										sinCambios = 0;
										codigo[s - 3] = "ARRAY";
										codigo[s - 2] = "";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//Zona Metodo
									
									if(EsIgual(codigo[s], "PR04", "PR07", "PR11", "PR12", "PR13", "PR14", "PR17", "PR18", "PR23", "PR27"))
									{
										Bloc = true;
									}
									//METODO 5 SDMT
									if (s >= 1 && !Bloc
									&& codigo[s - 1] == "MDEC"
									&& codigo[s] == "CEX)")
									{
										UltVar = "";
										sinCambios = 0;
										codigo[s - 1] = "SDMT";
										codigo[s] = "";
										Bloc = false;
										break;
									}
									//MDEC CEX) BMET 
									if (s >= 5 && codigo[s-4] == "MDEC" && codigo[s - 3] == "CEX)" && codigo[s - 2] == "INIS" && codigo[s - 1] == "BMET" && codigo[s] == "FIIN")
									{
										sinCambios = 0;
										codigo[s - 4] = "MDEC";
										codigo[s - 3] = "CEX)";
										codigo[s - 2] = "BMET";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//MDEC CEX) BMET 
									if (s >= 4 && codigo[s - 3] == "MDEC" && codigo[s - 2] == "CEX)" && codigo[s - 1] == "INIS" && codigo[s] == "FIIN")
									{
										sinCambios = 0;
										codigo[s - 3] = "MDEC";
										codigo[s - 2] = "CEX)";
										codigo[s - 1] = "BMET";
										codigo[s] = "";
										break;
									}
									//MDEC CEX) 
									if(s >= 1 && codigo[s-1] == "MTDX" && codigo[s] == "CEX)")
									{
										sinCambios = 0;
										codigo[s - 1] = "MDEC";
										codigo[s] = "CEX)";
										break;
									}
									//MDEC CEX) 
									if (s >= 3 && codigo[s-2].Substring(0,2) == "ID" && codigo[s - 1] == "CEX(" && codigo[s] == "CEX)")
									{
										sinCambios = 0;
										codigo[s - 2] = "MDEC";
										codigo[s - 1] = "CEX)";
										codigo[s] = "";
										break;
									}
									//MDEC CEX) 
									if (s >= 3 && codigo[s - 2] == "MTDX" 
										&& EsIgual(codigo[s - 1], "SPR04", "SPR07", "SPR11", "SPR12", "SPR13", "SPR14", "SPR17", "SPR18", "SPR23") 
										&& codigo[s] == "CEX)")
									{
										sinCambios = 0;
										codigo[s - 2] = "MDEC";
										codigo[s - 1] = "CEX)";
										codigo[s] = "";
										break;
									}
									//MDEC CEX) 
									if (s >= 4 && codigo[s-3].Substring(0,2) == "ID" && 
										codigo[s - 2] == "CEX(" 
										&& EsIgual(codigo[s - 1], "SPR04", "SPR07", "SPR11", "SPR12", "SPR13", "SPR14", "SPR17", "SPR18", "SPR23")
										&& codigo[s] == "CEX)")
									{
										sinCambios = 0;
										codigo[s - 3] = "MDEC";
										codigo[s - 2] = "CEX)";
										codigo[s - 1] = "";
										codigo[s] = "";
										break;
									}
									//BMET 
									if(s >= 2 
										&& EsIgual(codigo[s - 1], "SPR01", "SPR21", "SPR02", "SPR24", "SPR03", "SPR06", "SPR08", "SPR09", "SPR22", "SPR20", "SPR16", "SPR04", "SPR07", "SPR11", "SPR12", "SPR13", "SPR14", "SPR17", "SPR18", "SPR23", "BMET")
										&& EsIgual(codigo[s], "SPR01", "SPR21", "SPR02", "SPR24", "SPR03", "SPR06", "SPR08", "SPR09", "SPR22", "SPR20", "SPR16", "SPR04", "SPR07", "SPR11", "SPR12", "SPR13", "SPR14", "SPR17", "SPR18", "SPR23", "BMET"))
									{
										sinCambios = 0;
										codigo[s - 1] = "BMET";
										codigo[s] = "";
										break;
									}
									//BRMT --BMET
									if (s >= 2 
										&& (codigo[s - 1] == "BRMT" || codigo[s-1] == "BMET")
										&& (codigo[s] == "BRMT" || codigo[s] == "BMET"))
									{
										sinCambios = 0;
										codigo[s - 1] = "BRMT";
										codigo[s] = "";
										break;
									}
									//Comprobar valor
									if(s >= 2 
										&& EsIgual(codigo[s - 1], "PR04", "PR07", "PR11", "PR12", "PR13", "PR14", "PR17", "PR18", "PR23")
										&& (EsIgual(codigo[s], "MDEC","MTDX") || codigo[s].Substring(0,2) == "ID"))
									{
										UltDec = codigo[s-1];
									}
									//BRMT
									if(s >= 4 
										&& codigo[s-3] == "INIS"
										&& codigo[s-2] == "PR19"
										&& codigo[s] == "FIIN")
									{
										if(UltDec == "PR04" && (codigo[s-1] == "CONE" || codigo[s-1] == "SPR04" || codigo[s-1].Substring(0,2) == "ID"))
										{
											sinCambios = 0;
											codigo[s - 3] = "BRMT";
											codigo[s - 2] = "";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}else
										if (UltDec == "PR07" && (codigo[s - 1] == "CRTR" || codigo[s - 1] == "SPR07" || codigo[s - 1].Substring(0, 2) == "ID"))
										{
											sinCambios = 0;
											codigo[s - 3] = "BRMT";
											codigo[s - 2] = "";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										else
										if (UltDec == "PR11" && (codigo[s - 1] == "PR05" || codigo[s - 1] == "PR25" || codigo[s - 1] == "SPR11" || codigo[s - 1].Substring(0, 2) == "ID"))
										{
											sinCambios = 0;
											codigo[s - 3] = "BRMT";
											codigo[s - 2] = "";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										else
										if (UltDec == "PR13" && (codigo[s - 1] == "SPR15" || codigo[s - 1] == "SPR13" || codigo[s - 1].Substring(0, 2) == "ID"))
										{
											sinCambios = 0;
											codigo[s - 3] = "BRMT";
											codigo[s - 2] = "";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										else
										if (UltDec == "PR15" && ( codigo[s - 1] == "SPR15" || codigo[s - 1].Substring(0, 2) == "ID"))
										{
											sinCambios = 0;
											codigo[s - 3] = "BRMT";
											codigo[s - 2] = "";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										else
										if (UltDec == "PR18" && (codigo[s - 1] == "CONE" || codigo[s - 1] == "CONR" || codigo[s - 1] == "SPR18" || codigo[s - 1].Substring(0, 2) == "ID"))
										{
											sinCambios = 0;
											codigo[s - 3] = "BRMT";
											codigo[s - 2] = "";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										else
										if (UltDec == "PR23" && (codigo[s - 1] == "CDNA" || codigo[s - 1] == "SPR23" || codigo[s - 1].Substring(0, 2) == "ID"))
										{
											sinCambios = 0;
											codigo[s - 3] = "BRMT";
											codigo[s - 2] = "";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
									}
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
							cadenaSintactico = "";
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
					Mensaje(ex.Message +"\n"+MensajeError+"\n"+MensajeError2+";");
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
		private bool EsIgual(string cad1 , params string[] conjunto)
		{
			foreach (string cad in conjunto)
			{
				if (cad == cad1)
					return true;
			}
			return false;
		}
		private bool SiOpRelacional(string cadena)
		{
			if (cadena == "OPR1"
			|| cadena == "OPR2"
			|| cadena == "OPR3"
			|| cadena == "OPR4"
			|| cadena == "OPR5"
			|| cadena == "OPR6")
			{
				return true;
			}
			return false;
		}
		private bool SiOpLogico(string cadena)
		{
			if (cadena == "OPLA"
			|| cadena == "OPLO"
			|| cadena == "OPLN")
			{
				return true;
			}
			return false;
		}
		private bool SiOpAritmetico(string cadena)
		{
			if (cadena == "OPSM"
			|| cadena == "OPRS"
			|| cadena == "OPML"
			|| cadena == "OPDV"
			|| cadena == "OPEX")
			{
				return true;
			}
			return false;
		}
		private void btnSintactizar_Click(object sender, EventArgs e)
		{
			
			Mensaje("Funcionalidad proximamente...");
		}
		int cero = 0;
		private void btnRellenar_Click(object sender, EventArgs e)
		{
			Textosxp tx = new Textosxp();
			const string ruta = @".\codigos.txt";
			string repuesto = tx.repuesto;
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
			rchAnalisisSintactico.Text = "";
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
		bool MenSi = false;
		private void chbMensaje_CheckedChanged(object sender, EventArgs e)
		{
			if(chbMensaje.Checked)
			{
				MenSi = true;
			}else
			{
				MenSi = false;
			}
		}
	}
}
