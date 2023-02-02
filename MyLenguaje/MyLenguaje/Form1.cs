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
		bool EstadoAnalisis = false;

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
						EstadoAnalisis = true;
					}else
					{
						rchConsola2.Text = textoError;
						EstadoAnalisis = false;
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
			if(EstadoAnalisis)
			{
				EstaForma.Cursor = Cursors.AppStarting;
				rchConsola3.Text = "";
				rchSintactico.Text = "";
				if (!String.IsNullOrWhiteSpace(rchLexico.Text) || !String.IsNullOrEmpty(rchLexico.Text))
				{
					string MensajeError = "";
					try
					{
						bool final = true;
						string[] lexicos = cadenaLexico.Split(careEspacio, '\n', ' ');//rchLexico.Text.Split(' ','\n');
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
								}
								else
								{
									count++;
								}
								if (str == "COMS")
								{
									cadena1 += "";
									count--;
								}
								else
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
							string UltDec = "";
							do
							{
								string[] codigx = codigo;
								string cod1 = "";
								int cont1 = 0;
								string[] arr = new string[0];
								if (compVuelta == 53)
								{

								}
								foreach (string str in codigx)
								{

									if (str != "" && str != "" + careFin)
									{
										if (cont1 == 0)
										{
											cont1++;
										}
										else
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
									MensajeError = "s: " + s + " -CompVuelta: " + compVuelta + " -UltPalabra: " + codigo[s] + " -UltimoMetodo: ";
									
									
									if (codigo[s] == "")//Para cualquier imprevisto
									{
										break;
									}
									//Variable pura
									{
										int posx = (codigo.Length - 1) - s;
										//Zona Ent
										//SPR04
										if (posx >= 4 && codigo[s] == "INIS" && codigo[s + 1] == "DECENT" && codigo[s + 2] == "ASIG" && codigo[s + 3] == "ASIGENT" && codigo[s + 4] == "FIIN")
										{

											sinCambios = 0;
											codigo[s] = "SPR04";
											codigo[s + 1] = "";
											codigo[s + 2] = "";
											codigo[s + 3] = "";
											codigo[s + 4] = "";
											cat += "ENT - Exito\n";
											break;
										}
										//DECENT 
										if (s >= 2 && codigo[s - 2] == "INIS" && codigo[s - 1] == "DECENT" && codigo[s] == "FIIN" )
										{
											sinCambios = 0;
											codigo[s - 2] = "DECENT";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//ASIGENT 
										if (s >= 2 && codigo[s - 2] == "INIS" && codigo[s - 1] == "ASIGENT" && codigo[s] == "FIIN")
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGENT";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//DECENT ASIG ASIGENT --!Arit
										if (s >= 3 && posx > 0 && codigo[s - 3] == "PR04" && codigo[s - 2].Substring(0, 2) == "ID" && codigo[s - 1] == "ASIG" && codigo[s] == "CONE" && !SiOpAritmetico(codigo[s + 1]))
										{
											sinCambios = 0;
											codigo[s - 3] = "DECENT";
											codigo[s - 2] = "ASIG";
											codigo[s - 1] = "ASIGENT";
											codigo[s] = "";
											break;
										}
										//DECENT ASIG ASIGENT
										if (s >= 3 && posx == 0 && codigo[s - 3] == "PR04" && codigo[s - 2].Substring(0, 2) == "ID" && codigo[s - 1] == "ASIG" && codigo[s] == "CONE")
										{
											sinCambios = 0;
											codigo[s - 3] = "DECENT";
											codigo[s - 2] = "ASIG";
											codigo[s - 1] = "ASIGENT";
											codigo[s] = "";
											break;
										}
										//1)ASIGENT 
										if (s >= 2 && (codigo[s - 2] == "ASIGENT" || codigo[s - 2] == "CONE") && codigo[s - 1] == "CEX," && (codigo[s] == "ASIGENT" || codigo[s] == "ASIGID"))
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGENT";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//2)ASIGENT 
										if (s >= 2 && codigo[s - 2] == "ASIGCONE" && codigo[s - 1] == "CEX," && (codigo[s] == "ASIGENT" || codigo[s] == "ASIGID"))
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGENT";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//3)ASIGENT --!ARIT !ASIG -!ID
										if (s >= 2 && posx > 0 && codigo[s - 2] == "ASIGENT" && codigo[s - 1] == "CEX," && codigo[s].Substring(0, 2) == "ID" && !(SiOpAritmetico(codigo[s + 1]) || codigo[s + 1] == "ASIG" ))
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGENT";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										if (s >= 2 && posx == 0 && codigo[s - 2] == "ASIGENT" && codigo[s - 1] == "CEX," && codigo[s].Substring(0, 2) == "ID")
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGENT";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//4)ASIGENT --!ARIT 
										if (s >= 2 && posx > 0 && codigo[s - 2].Substring(0, 2) == "ID" && codigo[s - 1] == "ASIG" && EsIgual(codigo[s], "ASIGCONE", "CONE") && !SiOpAritmetico(codigo[s + 1]))
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGENT";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//4)ASIGENT
										if (s >= 2 && posx == 0 && codigo[s - 2].Substring(0, 2) == "ID" && codigo[s - 1] == "ASIG" && (codigo[s] == "ASIGCONE" || codigo[s] == "CONE"))
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGENT";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//3)DECENT --!ARIT !ASIG
										if (s >= 2 && posx > 0 && codigo[s - 2] == "DECENT" && codigo[s - 1] == "CEX," && codigo[s].Substring(0, 2) == "ID" && !(SiOpAritmetico(codigo[s + 1]) || codigo[s + 1] == "ASIG" ))
										{
											sinCambios = 0;
											codigo[s - 2] = "DECENT";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//if (s >= 2 && posx == 0 && codigo[s - 2] == "DECENT" && codigo[s - 1] == "CEX," && codigo[s].Substring(0, 2) == "ID")
										//{
										//	sinCambios = 0;
										//	codigo[s - 2] = "ASIGENT";
										//	codigo[s - 1] = "";
										//	codigo[s] = "";
										//	break;
										//}
										//DECENT ASIGCONE
										if (s >= 3 && codigo[s - 3] == "DECENT" && codigo[s - 2] == "ASIGCONE" && codigo[s - 1] == "CEX," && codigo[s] == "ASIGENT")
										{
											sinCambios = 0;
											codigo[s - 3] = "DECENT";
											codigo[s - 2] = "ASIGCONE";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//DECENT --!ARIT !ASIG
										if (s >= 2 && codigo[s - 2] == "PR04" && codigo[s - 1].Substring(0, 2) == "ID" && !(SiOpAritmetico(codigo[s]) || codigo[s] == "ASIG" || codigo[s].Substring(0, 2) == "ID"))
										{
											sinCambios = 0;
											codigo[s - 2] = "DECENT";
											codigo[s - 1] = "";
											break;
										}
										//ASIGCONE
										if (s >= 2 && codigo[s - 2] == "CEX(" && codigo[s - 1] == "ASIGCONE" && codigo[s] == "CEX)")
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGCONE";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//ASIGCONE
										if (s >= 2
											&& (EsIgual(codigo[s - 2], "CONE", "ASIGCONE") || codigo[s - 2] == "ASIGID")
											&& codigo[s - 1] == "OPSM"
											&& (EsIgual(codigo[s], "CONE", "ASIGCONE") || codigo[s].Substring(0, 2) == "ID"))
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGCONE";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//Zona Kar
										//SPR07
										if (posx >= 4 && codigo[s] == "INIS" && codigo[s + 1] == "DECKAR" && codigo[s + 2] == "ASIG" && codigo[s + 3] == "ASIGKAR" && codigo[s + 4] == "FIIN")
										{

											sinCambios = 0;
											codigo[s] = "SPR07";
											codigo[s + 1] = "";
											codigo[s + 2] = "";
											codigo[s + 3] = "";
											codigo[s + 4] = "";
											cat += "KAR - Exito\n";
											break;
										}
										//DECKAR 
										if (s >= 2 && codigo[s - 2] == "INIS" && codigo[s - 1] == "DECKAR" && codigo[s] == "FIIN")
										{
											sinCambios = 0;
											codigo[s - 2] = "DECKAR";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//ASIGKAR 
										if (s >= 2 && codigo[s - 2] == "INIS" && codigo[s - 1] == "ASIGKAR" && codigo[s] == "FIIN")
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGKAR";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//DECKAR ASIG ASIGKAR --!Arit
										if (s >= 3 && posx > 0 && codigo[s - 3] == "PR07" && codigo[s - 2].Substring(0, 2) == "ID" && codigo[s - 1] == "ASIG" && (codigo[s] == "CRTR" || codigo[s].Substring(0, 2) == "ID") && !SiOpAritmetico(codigo[s + 1]))
										{
											sinCambios = 0;
											codigo[s - 3] = "DECKAR";
											codigo[s - 2] = "ASIG";
											codigo[s - 1] = "ASIGKAR";
											codigo[s] = "";
											break;
										}
										//DECKAR ASIG ASIGKAR
										if (s >= 3 && posx == 0 && codigo[s - 3] == "PR07" && codigo[s - 2].Substring(0, 2) == "ID" && codigo[s - 1] == "ASIG" && (codigo[s] == "CRTR" || codigo[s].Substring(0, 2) == "ID"))
										{
											sinCambios = 0;
											codigo[s - 3] = "DECKAR";
											codigo[s - 2] = "ASIG";
											codigo[s - 1] = "ASIGKAR";
											codigo[s] = "";
											break;
										}
										//1)ASIGKAR 
										if (s >= 2 && (codigo[s - 2] == "ASIGKAR" || codigo[s - 2] == "CRTR") && codigo[s - 1] == "CEX," && (codigo[s] == "ASIGKAR" || codigo[s] == "ASIGID"))
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGKAR";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//2)ASIGKAR 
										if (s >= 2 && codigo[s - 2] == "ASIGCRTR" && codigo[s - 1] == "CEX," && (codigo[s] == "ASIGKAR" || codigo[s] == "ASIGID"))
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGKAR";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//3)ASIGKAR --!ARIT !ASIG
										if (s >= 2 && posx > 0 && codigo[s - 2] == "ASIGKAR" && codigo[s - 1] == "CEX," && codigo[s].Substring(0, 2) == "ID" && !(SiOpAritmetico(codigo[s + 1]) || codigo[s + 1] == "ASIG"))
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGKAR";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										if (s >= 2 && posx == 0 && codigo[s - 2] == "ASIGKAR" && codigo[s - 1] == "CEX," && codigo[s].Substring(0, 2) == "ID")
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGKAR";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//4)ASIGKAR --!ARIT 
										if (s >= 2 && posx > 0 && codigo[s - 2].Substring(0, 2) == "ID" && codigo[s - 1] == "ASIG" && EsIgual(codigo[s], "ASIGCRTR", "CRTR") && !SiOpAritmetico(codigo[s + 1]))
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGKAR";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//4)ASIGKAR
										if (s >= 2 && posx == 0 && codigo[s - 2].Substring(0, 2) == "ID" && codigo[s - 1] == "ASIG" && (codigo[s] == "ASIGCRTR" || codigo[s] == "CRTR"))
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGKAR";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//3)DECKAR --!ARIT !ASIG
										if (s >= 2 && posx > 0 && codigo[s - 2] == "DECKAR" && codigo[s - 1] == "CEX," && codigo[s].Substring(0, 2) == "ID" && !(SiOpAritmetico(codigo[s + 1]) || codigo[s + 1] == "ASIG" ))
										{
											sinCambios = 0;
											codigo[s - 2] = "DECKAR";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//DECKAR ASIGCRTR
										if (s >= 3 && codigo[s - 3] == "DECKAR" && codigo[s - 2] == "ASIGCRTR" && codigo[s - 1] == "CEX," && codigo[s] == "ASIGKAR")
										{
											sinCambios = 0;
											codigo[s - 3] = "DECKAR";
											codigo[s - 2] = "ASIGCRTR";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//DECKAR --!ARIT !ASIG
										if (s >= 2 && codigo[s - 2] == "PR07" && codigo[s - 1].Substring(0, 2) == "ID" && !(SiOpAritmetico(codigo[s]) || codigo[s] == "ASIG" || codigo[s].Substring(0, 2) == "ID"))
										{
											sinCambios = 0;
											codigo[s - 2] = "DECKAR";
											codigo[s - 1] = "";
											break;
										}
										//ASIGCRTR
										if (s >= 2 && codigo[s - 2] == "CEX(" && (EsIgual(codigo[s - 2], "CRTR", "ASIGCRTR")) && codigo[s] == "CEX)")
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGCRTR";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//Zona Log
										//SPR11
										if (posx >= 4 && codigo[s] == "INIS" && codigo[s + 1] == "DECLOG" && codigo[s + 2] == "ASIG" && codigo[s + 3] == "ASIGLOG" && codigo[s + 4] == "FIIN")
										{

											sinCambios = 0;
											codigo[s] = "SPR11";
											codigo[s + 1] = "";
											codigo[s + 2] = "";
											codigo[s + 3] = "";
											codigo[s + 4] = "";
											cat += "LOG - Exito\n";
											break;
										}
										//DECLOG 
										if (s >= 2 && codigo[s - 2] == "INIS" && codigo[s - 1] == "DECLOG" && codigo[s] == "FIIN")
										{
											sinCambios = 0;
											codigo[s - 2] = "DECLOG";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//ASIGLOG 
										if (s >= 2 && codigo[s - 2] == "INIS" && codigo[s - 1] == "ASIGLOG" && codigo[s] == "FIIN")
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGLOG";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//DECLOG ASIG ASIGLOG --!Arit
										if (s >= 3 && posx > 0 && codigo[s - 3] == "PR11" && codigo[s - 2].Substring(0, 2) == "ID" && codigo[s - 1] == "ASIG" && (codigo[s] == "BLN" || codigo[s].Substring(0, 2) == "ID") && !(SiOpLogico(codigo[s + 1]) || SiOpRelacional(codigo[s + 1]) || codigo[s + 1] == "CEX("))
										{
											sinCambios = 0;
											codigo[s - 3] = "DECLOG";
											codigo[s - 2] = "ASIG";
											codigo[s - 1] = "ASIGLOG";
											codigo[s] = "";
											break;
										}
										//DECLOG ASIG ASIGLOG
										if (s >= 3 && posx == 0 && codigo[s - 3] == "PR11" && codigo[s - 2].Substring(0, 2) == "ID" && codigo[s - 1] == "ASIG" && (codigo[s] == "BLN" || codigo[s].Substring(0, 2) == "ID"))
										{
											sinCambios = 0;
											codigo[s - 3] = "DECLOG";
											codigo[s - 2] = "ASIG";
											codigo[s - 1] = "ASIGLOG";
											codigo[s] = "";
											break;
										}
										//PR11 idn ASIG BLN
										if (s >= 3 && posx == 0 && codigo[s - 3] == "PR11" && codigo[s - 2].Substring(0, 2) == "ID" && codigo[s - 1] == "ASIG" && codigo[s].Substring(0, 2) == "ID")
										{
											sinCambios = 0;
											codigo[s] = "BLN";
											break;
										}
										//1)ASIGLOG 
										if (s >= 2 && (codigo[s - 2] == "ASIGLOG" || codigo[s - 2] == "BLN") && codigo[s - 1] == "CEX," && (codigo[s] == "ASIGLOG" || codigo[s] == "ASIGID"))
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGLOG";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//2)ASIGLOG 
										if (s >= 2 && codigo[s - 2] == "ASIGBLN" && codigo[s - 1] == "CEX," && (codigo[s] == "ASIGLOG" || codigo[s] == "ASIGID"))
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGLOG";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//3)ASIGLOG --!ARIT !ASIG
										if (s >= 2 && posx > 0 && codigo[s - 2] == "ASIGLOG" && codigo[s - 1] == "CEX," && codigo[s].Substring(0, 2) == "ID" && !(SiOpAritmetico(codigo[s + 1]) || codigo[s + 1] == "ASIG"))
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGLOG";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										if (s >= 2 && posx == 0 && codigo[s - 2] == "ASIGLOG" && codigo[s - 1] == "CEX," && codigo[s].Substring(0, 2) == "ID")
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGLOG";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//4)ASIGLOG --!ARIT 
										if (s >= 2 && posx > 0 && codigo[s - 2].Substring(0, 2) == "ID" && codigo[s - 1] == "ASIG" && EsIgual(codigo[s], "ASIGBLN", "BLN") && !SiOpAritmetico(codigo[s + 1]))
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGLOG";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//4)ASIGLOG
										if (s >= 2 && posx == 0 && codigo[s - 2].Substring(0, 2) == "ID" && codigo[s - 1] == "ASIG" && (codigo[s] == "ASIGBLN" || codigo[s] == "BLN"))
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGLOG";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//3)DECLOG --!ARIT !ASIG
										if (s >= 2 && posx > 0 && codigo[s - 2] == "DECLOG" && codigo[s - 1] == "CEX," && codigo[s].Substring(0, 2) == "ID" && !(SiOpAritmetico(codigo[s + 1]) || codigo[s + 1] == "ASIG" ))
										{
											sinCambios = 0;
											codigo[s - 2] = "DECLOG";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}

										//DECLOG ASIGBLN
										if (s >= 3 && codigo[s - 3] == "DECLOG" && codigo[s - 2] == "ASIGBLN" && codigo[s - 1] == "CEX," && codigo[s] == "ASIGLOG")
										{
											sinCambios = 0;
											codigo[s - 3] = "DECLOG";
											codigo[s - 2] = "ASIGBLN";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//DECLOG --!ARIT !ASIG
										if (s >= 2 && codigo[s - 2] == "PR11" && codigo[s - 1].Substring(0, 2) == "ID" && !(SiOpAritmetico(codigo[s]) || codigo[s] == "ASIG" || codigo[s].Substring(0, 2) == "ID"))
										{
											sinCambios = 0;
											codigo[s - 2] = "DECLOG";
											codigo[s - 1] = "";
											break;
										}
										//BLN
										if (s >= 2 && codigo[s - 2] == "CEX(" && codigo[s - 1] == "BLN" && codigo[s] == "CEX)")
										{
											sinCambios = 0;
											codigo[s - 2] = "BLN";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//BLN
										if (s >= 2
											&& (
											((codigo[s - 2] == "ASIGCDNA" || codigo[s - 2] == "CDNA" || codigo[s - 2].Substring(0, 2) == "ID") && (codigo[s] == "ASIGCDNA" || codigo[s] == "CDNA" || codigo[s].Substring(0, 2) == "ID"))
										 || ((codigo[s - 2] == "ASIGCONE" || codigo[s - 2] == "CONE" || codigo[s - 2] == "ASIGCONR" || codigo[s - 2] == "CONR" || codigo[s - 2].Substring(0, 2) == "ID") && (codigo[s - 2] == "ASIGCONE" || codigo[s - 2] == "CONE" || codigo[s - 2] == "ASIGCONR" || codigo[s - 2] == "CONR" || codigo[s - 2].Substring(0, 2) == "ID"))
										 || ((codigo[s - 2] == "ASIGCRTR" || codigo[s - 2] == "CRTR" || codigo[s - 2].Substring(0, 2) == "ID") && (codigo[s - 2] == "ASIGCRTR" || codigo[s - 2] == "CRTR" || codigo[s - 2].Substring(0, 2) == "ID"))
										 || ((codigo[s - 2] == "ASIGBLN" || codigo[s - 2] == "BLN" || codigo[s - 2].Substring(0, 2) == "ID") && (codigo[s - 2] == "ASIGBLN" || codigo[s - 2] == "BLN" || codigo[s - 2].Substring(0, 2) == "ID"))
											)
											&& (SiOpRelacional(codigo[s - 1]))
										)
										{
											sinCambios = 0;
											codigo[s - 2] = "BLN";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//BLN
										if (s >= 2
											&& (EsIgual(codigo[s - 2], "BLN"))
											&& SiOpLogico(codigo[s - 1])
											&& (EsIgual(codigo[s], "BLN") || codigo[s].Substring(0, 2) == "ID"))
										{
											sinCambios = 0;
											codigo[s - 2] = "BLN";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//BLN
										if (codigo[s] == "PR05" || codigo[s] == "PR25")
										{
											sinCambios = 0;
											codigo[s] = "BLN";
											if (s != 0) { s--; }
										}
										//Zona REA
										//SPR18
										if (posx >= 5 && codigo[s] == "INIS" && codigo[s + 1] == "DECREA" && codigo[s + 2] == "ASIG" && (codigo[s + 3] == "ASIGREA" || codigo[s + 3] == "ASIGENT") && codigo[s + 4] == "FIIN")
										{

											sinCambios = 0;
											codigo[s] = "SPR18";
											codigo[s + 1] = "";
											codigo[s + 2] = "";
											codigo[s + 3] = "";
											codigo[s + 4] = "";
											cat += "REA - Exito\n";
											break;
										}
										//DECREA 
										if (s >= 2 && codigo[s - 2] == "INIS" && codigo[s - 1] == "DECREA" && codigo[s] == "FIIN")
										{
											sinCambios = 0;
											codigo[s - 2] = "DECREA";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//ASIGREA
										if (s >= 2 && codigo[s - 2] == "INIS" && codigo[s - 1] == "ASIGREA" && codigo[s] == "FIIN")
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGREA";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//DECREA ASIG ASIGREA --!Arit
										if (s >= 3 && posx > 0 && codigo[s - 3] == "PR18" && codigo[s - 2].Substring(0, 2) == "ID" && codigo[s - 1] == "ASIG" && (codigo[s] == "CONR" || codigo[s] == "CONE") && !SiOpAritmetico(codigo[s + 1]))
										{
											sinCambios = 0;
											codigo[s - 3] = "DECREA";
											codigo[s - 2] = "ASIG";
											codigo[s - 1] = "ASIGREA";
											codigo[s] = "";
											break;
										}
										//DECREA ASIG ASIGREA
										if (s >= 3 && posx == 0 && codigo[s - 3] == "PR18" && codigo[s - 2].Substring(0, 2) == "ID" && codigo[s - 1] == "ASIG" && (codigo[s] == "CONR" || codigo[s] == "CONE"))
										{
											sinCambios = 0;
											codigo[s - 3] = "DECREA";
											codigo[s - 2] = "ASIG";
											codigo[s - 1] = "ASIGREA";
											codigo[s] = "";
											break;
										}
										//1)ASIGREA 
										if (s >= 2 && (codigo[s - 2] == "ASIGREA" || codigo[s - 2] == "CONR") && codigo[s - 1] == "CEX," && (codigo[s] == "ASIGREA" || codigo[s] == "ASIGENT" || codigo[s] == "ASIGID"))
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGREA";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//2)ASIGREA 
										if (s >= 2 && (codigo[s - 2] == "ASIGCONR") && codigo[s - 1] == "CEX," && (codigo[s] == "ASIGREA" || codigo[s] == "ASIGENT" || codigo[s] == "ASIGID"))
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGREA";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//3)ASIGREA --!ARIT !ASIG
										if (s >= 2 && posx > 0 && codigo[s - 2] == "ASIGREA" && codigo[s - 1] == "CEX," && codigo[s].Substring(0, 2) == "ID" && !(SiOpAritmetico(codigo[s + 1]) || codigo[s + 1] == "ASIG"))
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGREA";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										if (s >= 2 && posx == 0 && codigo[s - 2] == "ASIGREA" && codigo[s - 1] == "CEX," && codigo[s].Substring(0, 2) == "ID")
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGREA";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//4)ASIGREA --!ARIT
										if (s >= 2 && posx > 0 && codigo[s - 2].Substring(0, 2) == "ID" && codigo[s - 1] == "ASIG" && EsIgual(codigo[s], "ASIGCONR", "CONR") && !SiOpAritmetico(codigo[s + 1]))
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGREA";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//4)ASIGREA
										if (s >= 2 && posx == 0 && codigo[s - 2].Substring(0, 2) == "ID" && codigo[s - 1] == "ASIG" && (codigo[s] == "ASIGCONR" || codigo[s] == "CONR"))
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGREA";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//DECREA ASIGCONE
										if (s >= 3 && codigo[s - 3] == "DECREA" && codigo[s - 2] == "ASIGCONR" && codigo[s - 1] == "CEX," && codigo[s] == "ASIGREA")
										{
											sinCambios = 0;
											codigo[s - 3] = "DECREA";
											codigo[s - 2] = "ASIGCONR";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//DECREA
										if (s >= 2 && codigo[s - 2] == "PR18" && codigo[s - 1].Substring(0, 2) == "ID" && !(SiOpAritmetico(codigo[s]) || codigo[s] == "ASIG" || codigo[s].Substring(0, 2) == "ID"))
										{
											sinCambios = 0;
											codigo[s - 2] = "DECREA";
											codigo[s - 1] = "";
											break;
										}
										//ASIGCONR
										if (s >= 2 && codigo[s - 2] == "CEX(" && codigo[s - 1] == "ASIGCONR" && codigo[s] == "CEX)")
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGCONR";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//ASIGCONR
										if (s >= 2
											&& (EsIgual(codigo[s - 2], "CONR", "ASIGCONR"))
											&& codigo[s - 1] == "OPSM"
											&& (EsIgual(codigo[s], "CONR", "ASIGCONR") || codigo[s].Substring(0, 2) == "ID"))
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGCONR";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//Zona Sxn
										//SPR23
										if (posx >= 4 && codigo[s] == "INIS" && codigo[s + 1] == "DECSXN" && codigo[s + 2] == "ASIG" && codigo[s + 3] == "ASIGSXN" && codigo[s + 4] == "FIIN")
										{

											sinCambios = 0;
											codigo[s] = "SPR23";
											codigo[s + 1] = "";
											codigo[s + 2] = "";
											codigo[s + 3] = "";
											codigo[s + 4] = "";
											cat += "SXN - Exito\n";
											break;
										}
										//SPR23
										if (posx >= 3 && codigo[s] == "INIS" && codigo[s + 1] == "PR23" && codigo[s + 2] == "ASIGSXN" && codigo[s + 3] == "FIIN")
										{

											sinCambios = 0;
											codigo[s] = "SPR23";
											codigo[s + 1] = "";
											codigo[s + 2] = "";
											codigo[s + 3] = "";
											cat += "SXN - Exito\n";
											break;
										}
										//DECSXN 
										if (s >= 2 && codigo[s - 2] == "INIS" && codigo[s - 1] == "DECSXN" && codigo[s] == "FIIN")
										{
											sinCambios = 0;
											codigo[s - 2] = "DECSXN";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//ASIGESXN
										if (s >= 2 && codigo[s - 2] == "INIS" && codigo[s - 1] == "ASIGSXN" && codigo[s] == "FIIN")
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGSXN";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//DECSXN ASIG ASIGSXN --!Arit
										if (s >= 3 && posx > 0 && codigo[s - 3] == "PR23" && codigo[s - 2].Substring(0, 2) == "ID" && codigo[s - 1] == "ASIG" && codigo[s] == "CDNA" && !SiOpAritmetico(codigo[s + 1]))
										{
											sinCambios = 0;
											codigo[s - 3] = "DECSXN";
											codigo[s - 2] = "ASIG";
											codigo[s - 1] = "ASIGSXN";
											codigo[s] = "";
											break;
										}
										//DECSXN ASIG ASIGSXN
										if (s >= 3 && posx == 0 && codigo[s - 3] == "PR23" && codigo[s - 2].Substring(0, 2) == "ID" && codigo[s - 1] == "ASIG" && codigo[s] == "CDNA")
										{
											sinCambios = 0;
											codigo[s - 3] = "DECSXN";
											codigo[s - 2] = "ASIG";
											codigo[s - 1] = "ASIGSXN";
											codigo[s] = "";
											break;
										}
										//1)ASIGSXN 
										if (s >= 2 && (codigo[s - 2] == "ASIGSXN" || codigo[s - 2] == "CDNA") && codigo[s - 1] == "CEX," && (codigo[s] == "ASIGSXN" || codigo[s] == "ASIGID"))
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGSXN";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//2)ASIGSXN 
										if (s >= 2 && codigo[s - 2] == "ASIGCONE" && codigo[s - 1] == "CEX," && (codigo[s] == "ASIGSXN" || codigo[s] == "ASIGID"))
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGSXN";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//3)ASIGSXN --!ARIT !ASIG
										if (s >= 2 && posx > 0 && codigo[s - 2] == "ASIGSXN" && codigo[s - 1] == "CEX," && codigo[s].Substring(0, 2) == "ID" && !(SiOpAritmetico(codigo[s + 1]) || codigo[s + 1] == "ASIG"))
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGSXN";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										if (s >= 2 && posx == 0 && codigo[s - 2] == "ASIGSXN" && codigo[s - 1] == "CEX," && codigo[s].Substring(0, 2) == "ID")
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGSXN";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//4)ASIGSXN --!ARIT 
										if (s >= 2 && posx > 0 && codigo[s - 2].Substring(0, 2) == "ID" && codigo[s - 1] == "ASIG" && EsIgual(codigo[s], "ASIGCDNA", "CDNA") && !SiOpAritmetico(codigo[s + 1]))
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGSXN";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//4)ASIGSXN
										if (s >= 2 && posx == 0 && codigo[s - 2].Substring(0, 2) == "ID" && codigo[s - 1] == "ASIG" && (codigo[s] == "ASIGCDNA" || codigo[s] == "CDNA"))
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGSXN";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//3)DECSXN --!ARIT !ASIG
										if (s >= 2 && posx > 0 && codigo[s - 2] == "DECSXN" && codigo[s - 1] == "CEX," && codigo[s].Substring(0, 2) == "ID" && !(SiOpAritmetico(codigo[s + 1]) || codigo[s + 1] == "ASIG" ))
										{
											sinCambios = 0;
											codigo[s - 2] = "DECSXN";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//DECSXN ASIGCDNA
										if (s >= 3 && codigo[s - 3] == "DECSXN" && codigo[s - 2] == "ASIGCDNA" && codigo[s - 1] == "CEX," && codigo[s] == "ASIGSXN")
										{
											sinCambios = 0;
											codigo[s - 3] = "DECSXN";
											codigo[s - 2] = "ASIGCDNA";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//DECSXN --!ARIT !ASIG
										if (s >= 2 && codigo[s - 2] == "PR23" && codigo[s - 1].Substring(0, 2) == "ID" && !(SiOpAritmetico(codigo[s]) || codigo[s] == "ASIG" || codigo[s].Substring(0, 2) == "ID"))
										{
											sinCambios = 0;
											codigo[s - 2] = "DECSXN";
											codigo[s - 1] = "";
											break;
										}
										//ASIGCDNA
										if (s >= 2 && codigo[s - 2] == "CEX(" && codigo[s - 1] == "ASIGCDNA" && codigo[s] == "CEX)")
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGCDNA";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//ASIGCDNA
										if (s >= 2
											&& EsIgual(codigo[s - 2], "CDNA", "ASIGCDNA", "CRTR", "CONE", "CONR")
											&& SiOpAritmetico(codigo[s - 1])
											&& EsIgual(codigo[s], "CDNA", "ASIGCDNA")) //VAR1
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGCDNA";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//ASIGCDNA
										if (s >= 2
											&& EsIgual(codigo[s - 2], "CDNA", "ASIGCDNA")
											&& SiOpAritmetico(codigo[s - 1])
											&& EsIgual(codigo[s], "CDNA", "ASIGCDNA", "CRTR", "CONE", "CONR","VARMAT")) //VAR1
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGCDNA";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//ASIGCDNA
										if (s >= 3
											&& EsIgual(codigo[s - 3], "CDNA", "ASIGCDNA")
											&& SiOpAritmetico(codigo[s - 2])
											&& codigo[s - 1].Substring(0,2) == "ID"
											&& codigo[s] != "CEX[") 
										{
											sinCambios = 0;
											codigo[s - 3] = "ASIGCDNA";
											codigo[s - 2] = "";
											codigo[s - 1] = "";
											//codigo[S]
											break;
										}
										//Zona Skr  //PA ABAJO
										//SPR24
										if (posx >= 5 && codigo[s] == "INIS" && codigo[s + 1] == "PR24" && codigo[s + 2] == "CEX(" && (codigo[s + 3] == "CDNA" || codigo[s + 3] == "ASIGCDNA" || codigo[s + 3].Substring(0, 2) == "ID" || codigo[s+3] =="SDCM") && codigo[s + 4] == "CEX)" && codigo[s + 5] == "FIIN")
										{

											sinCambios = 0;
											codigo[s] = "SPR23";
											codigo[s + 1] = "";
											codigo[s + 2] = "";
											codigo[s + 3] = "";
											codigo[s + 4] = "";
											codigo[s + 5] = "";
											cat += "SKR - Exito\n";
											break;
										}
										//Zona Se
										//SPR21
										if (posx >= 6
											&& codigo[s] == "INIS"
											&& codigo[s + 1] == "PR21"
											&& codigo[s + 2] == "BLN"
											&& codigo[s + 3] == "INIS"
											&& (BLOQUEA(codigo[s + 4]) || codigo[s + 4] == "BLOQUE")
											&& codigo[s + 5] == "FIIN"
											&& codigo[s + 6] == "FIIN")
										{
											sinCambios = 0;
											codigo[s] = "SPR21";
											codigo[s + 1] = "";
											codigo[s + 2] = "";
											codigo[s + 3] = "";
											codigo[s + 4] = "";
											codigo[s + 5] = "";
											codigo[s + 6] = "";
											cat += "SE - Exito\n";
											break;
										}
										//SPR21
										if (posx >= 10
											&& codigo[s] == "INIS"
											&& codigo[s + 1] == "PR21"
											&& codigo[s + 2] == "BLN"
											&& codigo[s + 3] == "INIS"
											&& (BLOQUEA(codigo[s + 4]) || codigo[s + 4] == "BLOQUE")
											&& codigo[s + 5] == "FIIN"
											&& codigo[s + 6] == "PR01"
											&& codigo[s + 7] == "INIS"
											&& BLOQUEA(codigo[s + 8])
											&& codigo[s + 9] == "FIIN"
											&& codigo[s + 10] == "FIIN")
										{
											sinCambios = 0;
											codigo[s] = "SPR21";
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
											cat += "SE y ali - Exito\n";
											break;
										}
										//INIS PR21 BLN INIS BLOQUE
										if (s >= 4
											&& codigo[s - 4] == "INIS"
											&& codigo[s - 3] == "PR21"
											&& codigo[s - 2] == "BLN"
											&& codigo[s - 1] == "INIS"
											&& BLOQUEA(codigo[s]))
										{
											sinCambios = 0;
											codigo[s] = "BLOQUE";
											break;
										}
										//PR01 INIS BLOQUE
										if (s >= 2
											&& codigo[s - 2] == "PR01"
											&& codigo[s - 1] == "INIS"
											&& BLOQUEA(codigo[s]))
										{
											sinCambios = 0;
											codigo[s] = "BLOQUE";
											break;
										}
										//BLOQUE
										if (s >= 1
											&& codigo[s - 1] == "BLOQUE"
											&& BLOQUEA(codigo[s]))
										{
											sinCambios = 0;
											codigo[s - 1] = "BLOQUE";
											codigo[s] = "";
											break;
										}
										//DAR
										if (s >= 2
											&& codigo[s - 2] == "INIS"
											&& codigo[s - 1] == "PR02"
											&& codigo[s] == "FIIN")
										{
											sinCambios = 0;
											codigo[s - 2] = "DAR";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//ROM
										if (s >= 2
											&& codigo[s - 2] == "INIS"
											&& codigo[s - 1] == "PR20"
											&& codigo[s] == "FIIN")
										{
											sinCambios = 0;
											codigo[s - 2] = "ROM";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//Zona DUM
										//SPR03
										if (posx >= 4
											&& codigo[s] == "INIS"
											&& codigo[s + 1] == "PR03"
											&& codigo[s + 2] == "BLN"
											&& (BLOQUEA(codigo[s + 3]) || codigo[s + 3] == "BLOQUE")
											&& codigo[s + 4] == "FIIN")
										{
											sinCambios = 0;
											codigo[s] = "SPR03";
											codigo[s + 1] = "";
											codigo[s + 2] = "";
											codigo[s + 3] = "";
											codigo[s + 4] = "";
											cat += "DUM - Exito\n";
											break;
										}
										//PR01 INIS BLOQUE
										if (s >= 3
											&& codigo[s - 3] == "INIS"
											&& codigo[s - 2] == "PR03"
											&& codigo[s - 1] == "BLN"
											&& BLOQUEA(codigo[s]))
										{
											sinCambios = 0;
											codigo[s] = "BLOQUE";
											break;
										}
										//Zona Far
										//SPR06
										if (posx >= 5
											&& codigo[s] == "INIS"
											&& codigo[s + 1] == "PR06"
											&& (BLOQUEA(codigo[s + 2]) || codigo[s + 2] == "BLOQUE")
											&& codigo[s + 3] == "PR03"
											&& codigo[s + 4] == "BLN"
											&& codigo[s + 5] == "FIIN")
										{
											sinCambios = 0;
											codigo[s] = "SPR06";
											codigo[s + 1] = "";
											codigo[s + 2] = "";
											codigo[s + 3] = "";
											codigo[s + 4] = "";
											codigo[s + 5] = "";
											cat += "Far - Exito\n";
											break;
										}
										//INIS PR06 BLOQUE
										if (s >= 2
											&& codigo[s - 2] == "INIS"
											&& codigo[s - 1] == "PR06"
											&& BLOQUEA(codigo[s]))
										{
											sinCambios = 0;
											codigo[s] = "BLOQUE";
											break;
										}
										//Zona Kap
										//SPR09
										if (posx >= 5
											&& codigo[s] == "INIS"
											&& codigo[s + 1] == "PR09"
											&& codigo[s + 2] == "CEX("
											&& codigo[s + 3].Substring(0, 2) == "ID"
											&& codigo[s + 4] == "CEX)"
											&& codigo[s + 5] == "FIIN")
										{
											sinCambios = 0;
											codigo[s] = "SPR09";
											codigo[s + 1] = "";
											codigo[s + 2] = "";
											codigo[s + 3] = "";
											codigo[s + 4] = "";
											codigo[s + 5] = "";
											cat += "KAP - Exito\n";
											break;
										}
										//Zona LEG
										//SPR12
										if (posx >= 5
											&& codigo[s] == "INIS"
											&& codigo[s + 1] == "PR12"
											&& codigo[s + 2] == "CEX("
											&& EsIgual(codigo[s + 3], "ARG21", "CONE", "ASIGCONE", "CONR", "ASIGCONR", "IDCC")
											&& codigo[s + 4] == "CEX)"
											&& codigo[s + 5] == "FIIN")
										{
											sinCambios = 0;
											codigo[s] = "SPR09";
											codigo[s + 1] = "";
											codigo[s + 2] = "";
											codigo[s + 3] = "";
											codigo[s + 4] = "";
											codigo[s + 5] = "";
											cat += "LEG - Exito\n";
											break;
										}
										//ARG21
										if (s >= 2
											&& codigo[s - 2] == "CEX("
											&& EsIgual(codigo[s - 1], "ARG21", "CONE", "ASIGCONE", "CONR", "ASIGCONR", "IDCC")
											&& codigo[s] == "CEX)")
										{
											sinCambios = 0;
											codigo[s] = "ARG21";
											break;
										}
										//Zona Por
										//SPR16
										if (posx >= 8
											&& codigo[s] == "INIS"
											&& codigo[s + 1] == "PR16"
											&& codigo[s + 2] == "CEX("
											&& codigo[s + 3] == "SPR04"
											&& codigo[s + 4] == "P16R2"
											&& (codigo[s + 5] == "P16R3" || codigo[s+5] == "ACUM")
											&& codigo[s + 6] == "CEX)"
											&& (BLOQUEA(codigo[s + 7]) || codigo[s + 7] == "BLOQUE")
											&& codigo[s + 8] == "FIIN")
										{
											sinCambios = 0;
											codigo[s] = "SPR16";
											codigo[s + 1] = "";
											codigo[s + 2] = "";
											codigo[s + 3] = "";
											codigo[s + 4] = "";
											codigo[s + 5] = "";
											codigo[s + 6] = "";
											codigo[s + 7] = "";
											codigo[s + 8] = "";
											cat += "POR - Exito\n";
											break;
										}
										//INIS PR16 CEX( SPR04 P16R2 P16R3 CEX) BLOQUE
										if (s >= 7
											&& codigo[s - 7] == "INIS"
											&& codigo[s - 6] == "PR16"
											&& codigo[s - 5] == "CEX("
											&& codigo[s - 4] == "SPR04"
											&& codigo[s - 3] == "P16R2"
											&& (codigo[s - 2] == "P16R3" || codigo[s - 2] == "ACUM")
											&& codigo[s - 1] == "CEX)"
											&& BLOQUEA(codigo[s]))
										{
											sinCambios = 0;
											codigo[s] = "BLOQUE";
											break;
										}
										//P16R2
										if (s >= 2
											&& codigo[s - 2] == "INIS"
											&& codigo[s - 1] == "BLN"
											&& codigo[s] == "FIIN")
										{
											sinCambios = 0;
											codigo[s - 2] = "P16R2";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//P16R3
										if (s >= 4
											&& codigo[s - 4] == "INIS"
											&& codigo[s - 3].Substring(0, 2) == "ID"
											&& EsIgual(codigo[s - 2], "OPSM", "OPRS")
											&& EsIgual(codigo[s - 1], "CONE", "ASIGCONE", "CONR", "ASIGCONR")
											&& codigo[s] == "FIIN")
										{
											sinCambios = 0;
											codigo[s - 4] = "P16R3";
											codigo[s - 3] = "";
											codigo[s - 2] = "";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										
										//Zona Prx
										//SPR17
										if (posx >= 8
											&& codigo[s] == "PR17"
											&& codigo[s + 1] == "CEX("
											&& EsIgual(codigo[s + 2], "PR04", "PR07", "PR11", "PR18", "PR23")
											&& codigo[s + 3].Substring(0, 2) == "ID"
											&& codigo[s + 4].Substring(0, 2) == "ID"
											&& codigo[s + 5] == "CEX)"
											&& codigo[s + 6] == "INIS"
											&& (BLOQUEA(codigo[s + 7]) || codigo[s + 7] == "BLOQUE")
											&& codigo[s + 8] == "FIIN")
										{
											sinCambios = 0;
											codigo[s] = "SPR17";
											codigo[s + 1] = "";
											codigo[s + 2] = "";
											codigo[s + 3] = "";
											codigo[s + 4] = "";
											codigo[s + 5] = "";
											codigo[s + 6] = "";
											codigo[s + 7] = "";
											codigo[s + 8] = "";
											cat += "PRX - Exito\n";
											break;
										}
										//Zona SXA
										//SPR22
										if (posx >= 6
											&& codigo[s] == "PR22"
											&& codigo[s + 1] == "CEX("
											&& codigo[s + 2].Substring(0,2) == "ID"
											&& codigo[s + 3] == "CEX)"
											&& codigo[s + 4] == "INIS"
											&& codigo[s + 5] == "KAZ"
											&& codigo[s + 6] == "FIIN")
										{
											sinCambios = 0;
											codigo[s] = "SPR22";
											codigo[s + 1] = "";
											codigo[s + 2] = "";
											codigo[s + 3] = "";
											codigo[s + 4] = "";
											codigo[s + 5] = "";
											codigo[s + 6] = "";
											cat += "SXA - Exito\n";
											break;
										}
										//KAZ
										if (s >= 5
											&& codigo[s - 5] == "INIS"
											&& codigo[s - 4] == "PR08"
											&& EsIgual(codigo[s - 3], "ASIGENT", "ASIGKAR", "ASIGLOG", "ASIGMATALL", "ASIGREA", "ASIGSXN", "CONE", "CRTR", "BLN", "CONR", "CDNA")
											&& codigo[s - 2] == "CEX:"
											&& (BLOQUEA(codigo[s - 1]) || codigo[s - 1] == "BLOQUE")
											&& codigo[s] == "FIIN")
										{
											sinCambios = 0;
											codigo[s - 5] = "KAZ";
											codigo[s - 4] = "";
											codigo[s - 3] = "";
											codigo[s - 2] = "";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//KAZ
										if(s>= 1
											&& codigo[s-1] ==  "KAZ"
											&& codigo[s] == "KAZ")
										{
											sinCambios = 0;
											codigo[s - 1] = "KAZ";
											codigo[s] = "";
											break;
										}
										//KAZ
										if (s >= 4
											&& codigo[s - 4] == "KAZ"
											&& codigo[s - 3] == "PR01"
											&& codigo[s - 2] == "INIS"
											&& codigo[s - 1] == "BLOQUE"
											&& codigo[s] == "FIIN")
										{
											sinCambios = 0;
											codigo[s - 4] = "KAZ";
											codigo[s - 3] = "";
											codigo[s - 2] = "";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//INIS PR08 ASIG CEX: BLOQUE
										if (s >= 4
											&& codigo[s - 4] == "INIS"
											&& codigo[s - 3] == "PR08"
											&& EsIgual(codigo[s - 2], "ASIGENT", "ASIGKAR", "ASIGLOG", "ASIGMATALL", "ASIGREA", "ASIGSXN", "CONE", "CRTR", "BLN", "CONR", "CDNA")
											&& codigo[s - 1] == "CEX:"
											&& BLOQUEA(codigo[s]))
										{
											sinCambios = 0;
											codigo[s] = "BLOQUE";
											break;
										}
										//CEX: BLOQUE
										if (s >= 2
											&& codigo[s - 2] == "CEX:"
											&& codigo[s - 1] == "BLOQUE"
											&& BLOQUEA(codigo[s]))
										{
											sinCambios = 0;
											codigo[s - 2] = "CEX:";
											codigo[s - 1] = "BLOQUE";
											codigo[s] = "";
											break;
										}
										//Zona Rev
										//S19CONE
										if (s >= 3
											&& codigo[s - 3] == "INIS"
											&& codigo[s - 2] == "PR19"
											&& EsIgual(codigo[s - 1], "CONE", "ASIGCONE", "NUM")//NUM
											&& codigo[s] == "FIIN")
										{

											sinCambios = 0;
											codigo[s - 3] = "INIS";
											codigo[s - 2] = "S19CONE";
											codigo[s - 1] = "FIIN";
											codigo[s] = "";
											cat += "REV - Exito\n";
											break;
										}
										//S19CONR
										if (s >= 3
											&& codigo[s - 3] == "INIS"
											&& codigo[s - 2] == "PR19"
											&& EsIgual(codigo[s - 1], "CONR", "ASIGCONR", "REAL")//REAL
											&& codigo[s] == "FIIN")
										{

											sinCambios = 0;
											codigo[s - 3] = "INIS";
											codigo[s - 2] = "S19CONR";
											codigo[s - 1] = "FIIN";
											codigo[s] = "";
											cat += "REV - Exito\n";
											break;
										}
										//S19ID
										if (s >= 3
											&& codigo[s - 3] == "INIS"
											&& codigo[s - 2] == "PR19"
											&& (codigo[s - 1].Substring(0, 2) == "ID" || codigo[s - 1] == "ASIGPRIM" || codigo[s - 1] == "PRIM")//REAL
											&& codigo[s] == "FIIN")
										{

											sinCambios = 0;
											codigo[s - 3] = "INIS";
											codigo[s - 2] = "S19ID";
											codigo[s - 1] = "FIIN";
											codigo[s] = "";
											cat += "REV - Exito\n";
											break;
										}
										//NUM
										if (s >= 3 && codigo[s - 3] == "PR19" && codigo[s - 2] == "CEX(" && EsIgual(codigo[s - 1], "CONE", "ASIGCONE", "NUM") && codigo[s] == "CEX)")
										{
											sinCambios = 0;
											codigo[s - 3] = "NUM";
											codigo[s - 2] = "";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//REAL
										if (s >= 3 && codigo[s - 3] == "PR19" && codigo[s - 2] == "CEX(" && EsIgual(codigo[s - 1], "CONR", "ASIGCONR", "REAL") && codigo[s] == "CEX)")
										{
											sinCambios = 0;
											codigo[s - 3] = "REAL";
											codigo[s - 2] = "";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//PRIM
										if (s >= 3 && codigo[s - 3] == "PR19" && codigo[s - 2] == "CEX(" && (codigo[s - 1].Substring(0, 2) == "ID" || codigo[s - 1] == "ASIGPRIM" || codigo[s - 1] == "PRIM") && codigo[s] == "CEX)")
										{
											sinCambios = 0;
											codigo[s - 3] = "PRIM";
											codigo[s - 2] = "";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//Zona MAT

										//SMAT
										if (posx >= 2
											&& codigo[s] == "INIS"
											&& codigo[s + 1] == "CMAT"
											&& codigo[s + 2] == "FIIN")
										{
											sinCambios = 0;
											codigo[s] = "SMAT";
											codigo[s + 1] = "";
											codigo[s + 2] = "";
											cat += "Mat - Exito\n";
											break;
										}
										//DECMATALL 
										if (s >= 2
											&& codigo[s - 2] == "INIS"
											&& EsIgual(codigo[s - 1], "DECMATENT", "DECMATKAR", "DECMATLOG", "DECMATREA", "DECMATSXN")//$DECMATALL DECMATALL
											&& codigo[s] == "FIIN")
										{
											sinCambios = 0;
											codigo[s - 2] = "DECMATALL";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//ASIGMATALL
										if (s >= 2
											&& codigo[s - 2] == "INIS"
											&& EsIgual(codigo[s - 1], "ASIGMATENT", "ASIGMATKAR", "ASIGMATLOG", "ASIGMATREA", "ASIGMATSXN")//$ASIGMATALL ASIGMATALL
											&& codigo[s] == "FIIN")
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGMATALL";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//ARRY
										if (s >= 1 && codigo[s - 1].Substring(0, 2) == "ID" && codigo[s] == "CEX[")
										{
											sinCambios = 0;
											codigo[s - 1] = "ARRY";
											codigo[s] = "";
											break;
										}
										//VARMAT
										if (s >= 2
											&& codigo[s - 2] == "ARRY"
											&& codigo[s - 1] == "CONE"
											&& codigo[s] == "CEX]")
										{
											sinCambios = 0;
											codigo[s - 2] = "VARMAT";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//1)CMAT
										if (s >= 2
											&& codigo[s - 2] == "CMAT"
											&& codigo[s - 1] == "CEX,"
											&& codigo[s] == "CMAT")
										{
											sinCambios = 0;
											codigo[s - 2] = "CMAT";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//2)CMAT --!ASIG
										if (s >= 2 && posx > 0
											&& codigo[s - 2] == "CMAT"
											&& codigo[s - 1] == "CEX,"
											&& EsIgual(codigo[s], "DECMATENT", "DECMATKAR", "DECMATKAR", "DECMATLOG", "DECMATREA", "DECMATSXN")//DECMATALL
											&& codigo[s + 1] != "ASIG")
										{
											sinCambios = 0;
											codigo[s - 2] = "CMAT";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										if (s >= 2 && posx == 0
											&& codigo[s - 2] == "CMAT"
											&& codigo[s - 1] == "CEX,"
											&& EsIgual(codigo[s], "DECMATENT", "DECMATKAR", "DECMATKAR", "DECMATLOG", "DECMATREA", "DECMATSXN"))//DECMATALL
										{
											sinCambios = 0;
											codigo[s - 2] = "CMAT";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//3)CMAT
										if (s >= 2
											&& ((codigo[s - 2] == "DECMATENT" && (codigo[s] == "ASIGMATENT" || codigo[s].Substring(0, 2) == "ID"))
											|| (codigo[s - 2] == "DECMATKAR" && (codigo[s] == "ASIGMATKAR" || codigo[s].Substring(0, 2) == "ID"))
											|| (codigo[s - 2] == "DECMATLOG" && (codigo[s] == "ASIGMATLOG" || codigo[s].Substring(0, 2) == "ID"))
											|| (codigo[s - 2] == "DECMATREA" && (codigo[s] == "ASIGMATREA" || codigo[s].Substring(0, 2) == "ID"))
											|| (codigo[s - 2] == "DECMATSXN" && (codigo[s] == "ASIGMATSXN" || codigo[s].Substring(0, 2) == "ID")))
											&& codigo[s - 1] == "ASIG")
										{
											sinCambios = 0;
											codigo[s - 2] = "CMAT";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//4)CMAT --!ASIG
										if (s >= 2 && posx > 0
											&& codigo[s - 2] == "CMAT"
											&& codigo[s - 1] == "CEX,"
											&& codigo[s].Substring(0, 2) == "ID"
											&& codigo[s + 1] != "ASIG")
										{
											sinCambios = 0;
											codigo[s - 2] = "CMAT";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										if (s >= 2 && posx == 0
											&& codigo[s - 2] == "CMAT"
											&& codigo[s - 1] == "CEX,"
											&& codigo[s].Substring(0, 2) == "ID")
										{
											sinCambios = 0;
											codigo[s - 2] = "CMAT";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//5)CMAT --!ASIG
										if (s >= 2 && posx > 0
											&& EsIgual(codigo[s - 2], "DECMATENT", "DECMATKAR", "DECMATLOG", "DECMATREA", "DECMATSXN", "ASIGMATENT", "ASIGMATKAR", "ASIGMATLOG", "ASIGMATREA", "ASIGMATSXN")
											&& codigo[s - 1] == "CEX,"
											&& EsIgual(codigo[s], "DECMATENT", "DECMATKAR", "DECMATLOG", "DECMATREA", "DECMATSXN")
											&& codigo[s + 1] != "ASIG")
										{
											sinCambios = 0;
											codigo[s - 2] = "CMAT";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										if (s >= 2 && posx == 0
											&& EsIgual(codigo[s - 2], "DECMATENT", "DECMATKAR", "DECMATLOG", "DECMATREA", "DECMATSXN", "ASIGMATENT", "ASIGMATKAR", "ASIGMATLOG", "ASIGMATREA", "ASIGMATSXN")
											&& codigo[s - 1] == "CEX,"
											&& EsIgual(codigo[s], "DECMATENT", "DECMATKAR", "DECMATLOG", "DECMATREA", "DECMATSXN"))
										{
											sinCambios = 0;
											codigo[s - 2] = "CMAT";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//6)CMAT
										if (s >= 2
											&& EsIgual(codigo[s - 2], "DECMATENT", "DECMATKAR", "DECMATLOG", "DECMATREA", "DECMATSXN")
											&& codigo[s - 1] == "ASIG"
											&& EsIgual(codigo[s], "ASIGMATENT", "ASIGMATKAR", "ASIGMATLOG", "ASIGMATREA", "ASIGMATSXN", "ACNE", "ACNR", "ACTR", "ACAD", "ABLN"))
										{
											sinCambios = 0;
											codigo[s - 2] = "CMAT";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//CEX, CMAT
										if (posx >= 3
 											&& codigo[s] == "CEX,"
											&& EsIgual(codigo[s + 1], "PR04", "PR07", "PR11", "PR18", "PR23")
											&& codigo[s + 2] == "CEX["
											&& codigo[s + 3] == "CEX]"
											&& codigo[s + 4].Substring(0, 2) == "ID")
										{
											sinCambios = 0;
											//DECMATENT
											if (codigo[s + 1] == "PR04")
											{
												codigo[s + 1] = "DECMATENT";
											}
											else if (codigo[s + 1] == "PR07")
											{//DECMATKAR
												codigo[s - 4] = "DECMATKAR";
											}
											else if (codigo[s + 1] == "PR11")
											{//DECMATLOG
												codigo[s + 1] = "DECMATLOG";
											}
											else if (codigo[s + 1] == "PR18")
											{//DECMATREA
												codigo[s + 1] = "DECMATREA";
											}
											else
											{//DECMATSXN
												codigo[s + 1] = "DECMATSXN";
											}
											codigo[s] = "CEX,";
											codigo[s + 2] = "";
											codigo[s + 3] = "";
											codigo[s + 4] = "";
											break;
										}
										//DECMAT
										if (s >= 4
											&& codigo[s - 4] == "PR13"
											&& EsIgual(codigo[s - 3], "PR04", "PR07", "PR11", "PR18", "PR23")
											&& codigo[s - 2] == "CEX["
											&& codigo[s - 1] == "CEX]"
											&& codigo[s].Substring(0, 2) == "ID")
										{
											//DECMATENT
											if (codigo[s - 3] == "PR04")
											{
												codigo[s - 4] = "DECMATENT";
											} else if (codigo[s - 3] == "PR07")
											{//DECMATKAR
												codigo[s - 4] = "DECMATKAR";
											}
											else if (codigo[s - 3] == "PR11")
											{//DECMATLOG
												codigo[s - 4] = "DECMATLOG";
											}
											else if (codigo[s - 3] == "PR18")
											{//DECMATREA
												codigo[s - 4] = "DECMATREA";
											}
											else
											{//DECMATSXN
												codigo[s - 4] = "DECMATSXN";
											}
											sinCambios = 0;
											codigo[s - 3] = "";
											codigo[s - 2] = "";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//ASIGMAT
										if (posx >= 4
											&& codigo[s] == "PR14"
											&& EsIgual(codigo[s + 1], "PR04", "PR07", "PR11", "PR18", "PR23")
											&& codigo[s + 2] == "CEX["
											&& codigo[s + 3] == "CONE"
											&& codigo[s + 4] == "CEX]")
										{
											//ASIGMATENT
											if (codigo[s + 1] == "PR04")
											{
												codigo[s] = "ASIGMATENT";
											}
											else if (codigo[s + 1] == "PR07")
											{//ASIGMATKAR
												codigo[s] = "ASIGMATKAR";
											}
											else if (codigo[s + 1] == "PR11")
											{//ASIGMATLOG
												codigo[s] = "ASIGMATLOG";
											}
											else if (codigo[s + 1] == "PR18")
											{//ASIGMATREA
												codigo[s] = "ASIGMATREA";
											}
											else
											{//ASIGMATSXN
												codigo[s] = "ASIGMATSXN";
											}
											sinCambios = 0;
											codigo[s + 1] = "";
											codigo[s + 2] = "";
											codigo[s + 3] = "";
											codigo[s + 4] = "";
											break;
										}
										//ASIGM
										if (posx >= 6 && s > 0
											&& !EsIgual(codigo[s - 1], "PR04", "PR07", "PR11", "PR18", "PR23", "CEX]")
											&& codigo[s].Substring(0, 2) == "ID"
											&& codigo[s + 1] == "ASIG"
											&& codigo[s + 2] == "PR14"
											&& EsIgual(codigo[s + 3], "PR04", "PR07", "PR11", "PR18", "PR23")
											&& codigo[s + 4] == "CEX["
											&& codigo[s + 5] == "CONE"
											&& codigo[s + 6] == "CEX]")
										{
											//ASIGMENT
											if (codigo[s + 3] == "PR04")
											{
												codigo[s] = "ASIGMENT";
											}
											else if (codigo[s + 3] == "PR07")
											{//ASIGMKAR
												codigo[s] = "ASIGMKAR";
											}
											else if (codigo[s + 3] == "PR11")
											{//ASIGMLOG
												codigo[s] = "ASIGMLOG";
											}
											else if (codigo[s + 3] == "PR18")
											{//ASIGMREA
												codigo[s] = "ASIGMREA";
											}
											else
											{//ASIGMSXN
												codigo[s] = "ASIGMSXN";
											}
											sinCambios = 0;
											codigo[s + 1] = "";
											codigo[s + 2] = "";
											codigo[s + 3] = "";
											codigo[s + 4] = "";
											codigo[s + 5] = "";
											codigo[s + 6] = "";
											break;
										}
										//ASIGM
										if (s >= 2
											&& codigo[s - 2].Substring(0, 2) == "ID"
											&& codigo[s - 1] == "ASIG"
											&& EsIgual(codigo[s], "ACNE", "ACNR", "ACTR", "ACAD", "ABLN"))
										{
											if (codigo[s] == "ACNE")
											{
												codigo[s - 2] = "ASIGMENT";
											} else if (codigo[s] == "ACNR")
											{
												codigo[s - 2] = "ASIGMKAR";
											}
											else if (codigo[s] == "ACTR")
											{
												codigo[s - 2] = "ASIGMLOG";
											}
											else if (codigo[s] == "ACAD")
											{
												codigo[s - 2] = "ASIGMREA";
											}
											else
											{
												codigo[s - 2] = "ASIGMSXN";
											}
											sinCambios = 0;
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//ASIGVARMAT
										if(s >= 4
											&& codigo[s - 4] =="INIS"
											&& codigo[s - 3] == "VARMAT"
											&& codigo[s - 2] == "ASIG"
											&& EsIgual(codigo[s- 1], "CONE","CONR","CDNA","CRTR","BLN","ASIGCONE", "ASIGCONR", "ASIGCDNA", "ASIGCRTR")
											&& codigo[s] == "FIIN")
										{
											sinCambios = 0;
											codigo[s-4] = "ASIGVARMAT";
											codigo[s-3] = "";
											codigo[s-2] = "";
											codigo[s-1] = "";
											codigo[s] = "";
											break;
										}
										//ACNE
										if (s >= 2
											&& codigo[s - 2] == "CEX{"
											&& codigo[s - 1] == "NUM2"
											&& codigo[s] == "CEX}")
										{
											sinCambios = 0;
											codigo[s - 2] = "ACNE";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//CEX{ NUM2
										if (s >= 1
											&& codigo[s - 1] == "CEX{"
											&& EsIgual(codigo[s], "CONE", "ASIGCONE"))
										{
											sinCambios = 0;
											codigo[s - 1] = "CEX{";
											codigo[s] = "NUM2";
											break;
										}
										//NUM2
										if (s >= 2
											&& codigo[s - 2] == "NUM2"
											&& codigo[s - 1] == "CEX,"
											&& EsIgual(codigo[s], "CONE", "ASIGCONE"))
										{
											sinCambios = 0;
											codigo[s - 2] = "NUM2";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//ACNR
										if (s >= 2
											&& codigo[s - 2] == "CEX{"
											&& codigo[s - 1] == "REA2"
											&& codigo[s] == "CEX}")
										{
											sinCambios = 0;
											codigo[s - 2] = "ACNR";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//CEX{ REA2
										if (s >= 1
											&& codigo[s - 1] == "CEX{"
											&& EsIgual(codigo[s], "CONR", "ASIGCONR"))
										{
											sinCambios = 0;
											codigo[s - 1] = "CEX{";
											codigo[s] = "REA2";
											break;
										}
										//REA2
										if (s >= 2
											&& codigo[s - 2] == "REA2"
											&& codigo[s - 1] == "CEX,"
											&& EsIgual(codigo[s], "CONR", "ASIGCONR"))
										{
											sinCambios = 0;
											codigo[s - 2] = "REA2";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//ACTR
										if (s >= 2
											&& codigo[s - 2] == "CEX{"
											&& codigo[s - 1] == "KAR2"
											&& codigo[s] == "CEX}")
										{
											sinCambios = 0;
											codigo[s - 2] = "ACTR";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//CEX{ KAR2
										if (s >= 1
											&& codigo[s - 1] == "CEX{"
											&& EsIgual(codigo[s], "CRTR", "ASIGCRTR"))
										{
											sinCambios = 0;
											codigo[s - 1] = "CEX{";
											codigo[s] = "KAR2";
											break;
										}
										//KAR2
										if (s >= 2
											&& codigo[s - 2] == "KAR2"
											&& codigo[s - 1] == "CEX,"
											&& EsIgual(codigo[s], "CTRT", "ASIGCTRT"))
										{
											sinCambios = 0;
											codigo[s - 2] = "KAR2";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//ACAD
										if (s >= 2
											&& codigo[s - 2] == "CEX{"
											&& codigo[s - 1] == "SXN2"
											&& codigo[s] == "CEX}")
										{
											sinCambios = 0;
											codigo[s - 2] = "ACAD";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//CEX{ SXN2
										if (s >= 1
											&& codigo[s - 1] == "CEX{"
											&& EsIgual(codigo[s], "CDNA", "ASIGCDNA"))
										{
											sinCambios = 0;
											codigo[s - 1] = "CEX{";
											codigo[s] = "SXN2";
											break;
										}
										//SXN2
										if (s >= 2
											&& codigo[s - 2] == "SXN2"
											&& codigo[s - 1] == "CEX,"
											&& EsIgual(codigo[s], "CDNA", "ASIGCDNA"))
										{
											sinCambios = 0;
											codigo[s - 2] = "SXN2";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//ABLN
										if (s >= 2
											&& codigo[s - 2] == "CEX{"
											&& codigo[s - 1] == "LOG2"
											&& codigo[s] == "CEX}")
										{
											sinCambios = 0;
											codigo[s - 2] = "ABLN";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//CEX{ LOG2
										if (s >= 1
											&& codigo[s - 1] == "CEX{"
											&& EsIgual(codigo[s], "CDNA", "ASIGCDNA"))
										{
											sinCambios = 0;
											codigo[s - 1] = "CEX{";
											codigo[s] = "LOG2";
											break;
										}
										//LOG2
										if (s >= 2
											&& codigo[s - 2] == "LOG2"
											&& codigo[s - 1] == "CEX,"
											&& EsIgual(codigo[s], "CDNA", "ASIGCDNA"))
										{
											sinCambios = 0;
											codigo[s - 2] = "LOG2";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}

										//Zona OBJ
										//SOBJ 
										if (posx >= 2
										&& codigo[s] == "INIS"
										&& codigo[s + 1] == "COBJ"
										&& codigo[s + 2] == "FIIN")
										{
											sinCambios = 0;
											codigo[s] = "SOBJ";
											codigo[s + 1] = "";
											codigo[s + 2] = "";
											break;
										}
										//SDCM
										if (s >= 1 && codigo[s - 1] == "IDCM" && EsIgual(codigo[s], "CM", "SVMT"))
										{
											sinCambios = 0;
											codigo[s - 1] = "SDCM";
											codigo[s] = "";
											break;
										}
										//COBJ
										if (s >= 2
											&& codigo[s - 2] == "COBJ"
											&& codigo[s - 1] == "CEX,"
											&& codigo[s] == "COBJ")
										{
											sinCambios = 0;
											codigo[s - 2] = "COBJ";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//COBJ --!ASIG | ASIGOBJ
										if (s >= 2 && posx > 0
											&& codigo[s - 2] == "COBJ"
											&& codigo[s - 1] == "CEX,"
											&& codigo[s] == "DECOBJ"
											&& !(codigo[s + 1] == "ASIG" || codigo[s + 1] == "ASIGOBJ"))
										{
											sinCambios = 0;
											codigo[s - 2] = "COBJ";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										if (s >= 2 && posx == 0
											&& codigo[s - 2] == "COBJ"
											&& codigo[s - 1] == "CEX,"
											&& codigo[s] == "DECOBJ")
										{
											sinCambios = 0;
											codigo[s - 2] = "COBJ";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//COBJ
										if (s >= 1 && codigo[s - 1] == "DECOBJ" && codigo[s] == "ASIGOBJ")
										{
											sinCambios = 0;
											codigo[s - 1] = "COBJ";
											codigo[s] = "";
											break;
										}
										//COBJ
										if (s >= 2
											&& codigo[s - 2] == "DECOBJ"
											&& codigo[s - 1] == "ASIG"
											&& codigo[s].Substring(0, 2) == "ID")
										{
											sinCambios = 0;
											codigo[s - 2] = "COBJ";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//DECOBJ
										if (s >= 2 && posx > 0
											&& codigo[s - 2] == "DECOBJ"
											&& codigo[s - 1] == "CEX,"
											&& codigo[s] == "DECOBJ"
											&& codigo[s + 1] != "ASIG")
										{
											sinCambios = 0;
											codigo[s - 2] = "DECOBJ";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										if (s >= 2 && posx == 0
											&& codigo[s - 2] == "DECOBJ"
											&& codigo[s - 1] == "CEX,"
											&& codigo[s] == "DECOBJ")
										{
											sinCambios = 0;
											codigo[s - 2] = "DECOBJ";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//DECOBJ
										if (s >= 2
											&& codigo[s - 2] == "INIS"
											&& codigo[s - 1] == "DECOBJ"
											&& codigo[s] == "FIIN")
										{
											sinCambios = 0;
											codigo[s - 2] = "DECOBJ";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//DECOBJ
										if (s >= 2
											&& codigo[s - 2] == "PR15"
											&& codigo[s - 1].Substring(0, 2) == "ID"
											&& codigo[s].Substring(0, 2) == "ID")
										{
											sinCambios = 0;
											codigo[s - 2] = "DECOBJ";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//ASIGOBJ
										if (s >= 2
											&& codigo[s - 2] == "INIS"
											&& codigo[s - 1] == "ASIGOBJ"
											&& codigo[s] == "FIIN")
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGOBJ";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//ASIG PR14 MOBJ
										if (s >= 3
											&& codigo[s - 3] == "ASIG"
											&& codigo[s - 2] == "PR14"
											&& codigo[s - 1] == "MTDX"
											&& codigo[s] == "CEX)")
										{
											sinCambios = 0;
											codigo[s - 1] = "MOBJ";
											codigo[s] = "";
											break;
										}
										//ASIG PR14 MOBJ
										if (s >= 4
											&& codigo[s - 4] == "ASIG"
											&& codigo[s - 3] == "PR14"
											&& codigo[s - 2] == "MTDX"
											&& ( codigo[s - 1] == "MDEC" || codigo[s-1] == "ASIGPRIM")
											&& codigo[s] == "CEX)")
										{
											sinCambios = 0;
											codigo[s - 1] = "MOBJ";
											codigo[s] = "";
											break;
										}
										//ASIG PR14 MOBJ
										if (s >= 3
											&& codigo[s - 3] == "ASIG"
											&& codigo[s - 2] == "PR14"
											&& codigo[s - 1] == "MTDX"
											&& codigo[s] == "SVMT")
										{
											sinCambios = 0;
											codigo[s - 1] = "MOBJ";
											codigo[s] = "";
											break;
										}
										//ASIGOBJ
										if (s >= 3
											&& codigo[s - 3].Substring(0, 2) == "ID"
											&& codigo[s - 2] == "ASIG"
											&& codigo[s - 1] == "PR14"
											&& codigo[s] == "MOBJ")
										{
											sinCambios = 0;
											codigo[s - 3] = "ASIGOBJ";
											codigo[s - 2] = "";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//ASIGOBJ
										if (s >= 2
											&& codigo[s - 2] == "ASIG"
											&& codigo[s - 1] == "PR14"
											&& codigo[s] == "MOBJ")
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGOBJ";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//ASIGOBJ
										if(s >= 3
											&& codigo[s-3] == "ASIG"
											&& codigo[s-2] == "PR14"
											&& codigo[s-1] == "MTDX"
											&& codigo[s] == "MOBJ")
										{
											sinCambios = 0;
											codigo[s - 3] = "ASIGOBJ";
											codigo[s - 2] = "";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//IDCM
										if (s >= 1 && codigo[s - 1] == "IDCC" && codigo[s] == "CEX(")
										{
											sinCambios = 0;
											codigo[s - 1] = "IDCM";
											codigo[s] = "";
											break;
										}
										//IDCM CM
										if (s >= 2 && codigo[s - 2] == "IDCM" && codigo[s - 1] == "MDEC" && codigo[s] == "CEX(")
										{
											sinCambios = 0;
											codigo[s - 1] = "IDCM";
											codigo[s] = "CM";
											break;
										}
										//IDCM CM
										if (s >= 1 && codigo[s - 1] == "IDCM" && codigo[s] == "CEX)")
										{
											sinCambios = 0;
											codigo[s - 1] = "IDCM";
											codigo[s] = "CM";
											break;
										}
										//Zona Metodo

										//if(EsIgual(codigo[s], "PR04", "PR07", "PR11", "PR12", "PR13", "PR14", "PR17", "PR18", "PR23", "PR27"))
										//{
										//	Bloc = true;
										//}
										//SVMT
										if (s >= 1
										&& codigo[0] == "MDEC"
										&& codigo[1] == "CEX)")
										{

											sinCambios = 0;
											codigo[0] = "SVMT";
											codigo[1] = "";
											break;
										}
										//SIMT
										if (s >= 3
											&& EsIgual(codigo[s - 3], "PR04", "PR07", "PR11", "PR18", "PR23")
											&& codigo[s - 2] == "MDEC"
											&& codigo[s - 1] == "CEX)"
											&& codigo[s] == "BRMT")
										{
											sinCambios = 0;
											codigo[s - 3] = "SIMT";
											codigo[s - 2] = "";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//SIMV
										if (s >= 3
											&& codigo[s - 3] == "PR27"
											&& codigo[s - 2] == "MDEC"
											&& codigo[s - 1] == "CEX)"
											&& codigo[s] == "BMET")
										{
											sinCambios = 0;
											codigo[s - 3] = "SIMV";
											codigo[s - 2] = "";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//SVMT
										if (s >= 2
											&& !EsIgual(codigo[s - 2], "PR04", "PR07", "PR11", "PR18", "PR23")
											&& codigo[s - 1] == "MDEC"
											&& codigo[s] == "CEX)")
										{
											sinCambios = 0;
											codigo[s - 2] = "SVMT";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}

										//MDEC CEX) BMET 
										if (s >= 5 && EsIgual(codigo[s - 4], "MDEC", "DECENT") && codigo[s - 3] == "CEX)" && codigo[s - 2] == "INIS" && codigo[s - 1] == "BMET" && codigo[s] == "FIIN")
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
										if (s >= 4 && EsIgual(codigo[s - 3], "MDEC", "DECENT") && codigo[s - 2] == "CEX)" && codigo[s - 1] == "INIS" && codigo[s] == "FIIN")
										{
											sinCambios = 0;
											codigo[s - 3] = "MDEC";
											codigo[s - 2] = "CEX)";
											codigo[s - 1] = "BMET";
											codigo[s] = "";
											break;
										}
										//MDEC CEX) 
										if (s >= 1 && codigo[s - 1] == "MTDX" && codigo[s] == "CEX)")
										{
											sinCambios = 0;
											codigo[s - 1] = "MDEC";
											codigo[s] = "CEX)";
											break;
										}
										//MDEC CEX) 
										if (s >= 2 && codigo[s - 2].Substring(0, 2) == "ID" && codigo[s - 1] == "CEX(" && codigo[s] == "CEX)")
										{
											sinCambios = 0;
											codigo[s - 2] = "MDEC";
											codigo[s - 1] = "CEX)";
											codigo[s] = "";
											break;
										}
										//MDEC CEX) 
										if (s >= 2 && codigo[s - 2] == "MTDX"
											&& EsIgual(codigo[s - 1], "MDEC", "DECENT")
											&& codigo[s] == "CEX)")
										{
											sinCambios = 0;
											codigo[s - 2] = "MDEC";
											codigo[s - 1] = "CEX)";
											codigo[s] = "";
											break;
										}
										//MDEC CEX) 
										if (s >= 3 && codigo[s - 3].Substring(0, 2) == "ID" &&
											codigo[s - 2] == "CEX("
											&& EsIgual(codigo[s - 4], "MDEC", "DECENT")
											&& codigo[s] == "CEX)")
										{
											sinCambios = 0;
											codigo[s - 3] = "MDEC";
											codigo[s - 2] = "CEX)";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//MDEC
										if (s >= 2
											&& EsIgual(codigo[s - 2], "MDEC", "DECENT", "DECKAR", "DECLOG", "DECMATALL", "DECMATENT", "DECMATKAR", "DECMATKAR", "DECMATLOG", "DECMATREA", "DECMATSXN", "DECREA", "DECSXN")
											&& codigo[s - 1] == "CEX,"
											&& EsIgual(codigo[s], "MDEC", "DECENT", "DECKAR", "DECLOG", "DECMATALL", "DECMATENT", "DECMATKAR", "DECMATKAR", "DECMATLOG", "DECMATREA", "DECMATSXN", "DECREA", "DECSXN")) //$MDEC
										{
											sinCambios = 0;
											codigo[s - 2] = "MDEC";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//BMET 
										if (s >= 1
											&& EsIgual(codigo[s - 1], "SPR01", "SPR21", "SPR02", "SPR24", "SPR03", "SPR06", "SPR08", "SPR09", "SPR22", "SPR20", "SPR16", "SPR04", "SPR07", "SPR11", "SPR12", "SPR13", "SPR14", "SPR17", "SPR18", "SPR23", "BMET", "DECENT", "ASIGID") //BMET
											&& EsIgual(codigo[s], "SPR01", "SPR21", "SPR02", "SPR24", "SPR03", "SPR06", "SPR08", "SPR09", "SPR22", "SPR20", "SPR16", "SPR04", "SPR07", "SPR11", "SPR12", "SPR13", "SPR14", "SPR17", "SPR18", "SPR23", "BMET", "DECENT", "ASIGID", "SIMT", "")) //BMET
										{
											sinCambios = 0;
											codigo[s - 1] = "BMET";
											codigo[s] = "";
											break;
										}
										//BRMT
										if (s >= 2 && codigo[s - 2] == "INIS" && codigo[s - 1] == "BRMT" && codigo[s] == "FIIN")
										{
											sinCambios = 0;
											codigo[s - 2] = "BRMT";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//BRMT --BMET
										if (s >= 1
											&& ((codigo[s - 1] == "BRMT" && codigo[s] == "BMET") || (codigo[s - 1] == "BRMT" && codigo[s] == "BMET")))
										{
											sinCambios = 0;
											codigo[s - 1] = "BRMT";
											codigo[s] = "";
											break;
										}

										//Comprobar valor
										if (s >= 1
											&& EsIgual(codigo[s - 1], "PR04", "PR07", "PR11", "PR12", "PR13", "PR15", "PR18", "PR23") //MARG1 - 27
											&& (EsIgual(codigo[s], "MDEC", "MTDX") || codigo[s].Substring(0, 2) == "ID"))
										{
											UltDec = codigo[s - 1];
										}
										//BRMT
										if (s >= 2
											&& codigo[s - 2] == "INIS"
											&& codigo[s] == "FIIN")
										{
											if (UltDec == "PR04" && (codigo[s - 1] == "S19CONE" || codigo[s - 1] == "S19ID"))
											{//ent
												sinCambios = 0;
												codigo[s - 2] = "BRMT";
												codigo[s - 1] = "";
												codigo[s] = "";
												break;
											}
											else
											if (UltDec == "PR07" && (codigo[s - 1] == "CRTR" || codigo[s - 1] == "SPR07" || codigo[s - 1].Substring(0, 2) == "ID"))
											{//kar
												sinCambios = 0;
												codigo[s - 2] = "BRMT";
												codigo[s - 1] = "";
												codigo[s] = "";
												break;
											}
											else
											if (UltDec == "PR11" && (codigo[s - 1] == "PR05" || codigo[s - 1] == "PR25" || codigo[s - 1] == "SPR11" || codigo[s - 1].Substring(0, 2) == "ID"))
											{//log
												sinCambios = 0;
												codigo[s - 2] = "BRMT";
												codigo[s - 1] = "";
												codigo[s] = "";
												break;
											}
											else
											if (UltDec == "PR13" && (codigo[s - 1] == "SPR15" || codigo[s - 1] == "SPR13" || codigo[s - 1].Substring(0, 2) == "ID"))
											{//mat
												sinCambios = 0;
												codigo[s - 2] = "BRMT";
												codigo[s - 1] = "";
												codigo[s] = "";
												break;
											}
											else
											if (UltDec == "PR15" && (codigo[s - 1] == "SPR15" || codigo[s - 1].Substring(0, 2) == "ID"))
											{//obj
												sinCambios = 0;
												codigo[s - 2] = "BRMT";
												codigo[s - 1] = "";
												codigo[s] = "";
												break;
											}
											else
											if (UltDec == "PR18" && (codigo[s - 1] == "S19CONR" || codigo[s - 1] == "S19ID"))
											{//rea
												sinCambios = 0;
												codigo[s - 2] = "BRMT";
												codigo[s - 1] = "";
												codigo[s] = "";
												break;
											}
											else
											if (UltDec == "PR23" && (codigo[s - 1] == "CDNA" || codigo[s - 1] == "SPR23" || codigo[s - 1].Substring(0, 2) == "ID"))
											{//sxn
												sinCambios = 0;
												codigo[s - 2] = "BRMT";
												codigo[s - 1] = "";
												codigo[s] = "";
												break;
											}
										}
										//Zona Clase KLA
										//ASIGID
										if (s >= 2 && codigo[s - 2] == "INIS" && codigo[s - 1] == "ASIGID" && codigo[s] == "FIIN")
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGID";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//SKLA
										if (posx >= 4 && codigo[s] != "PR26" && codigo[s + 1] == "XKLA" && codigo[s + 2] == "CABZ" && codigo[s + 3] == "BKLA" && codigo[s + 4] == "FIIN")
										{
											sinCambios = 0;
											codigo[s + 1] = "SKLA";
											codigo[s + 2] = "";
											codigo[s + 3] = "";
											codigo[s + 4] = "";
											break;
										}
										if (codigo[0] == "XKLA" && codigo[1] == "CABZ" && codigo[2] == "BKLA" && codigo[3] == "FIIN")
										{
											sinCambios = 0;
											codigo[0] = "SKLA";
											codigo[1] = "";
											codigo[2] = "";
											codigo[3] = "";
											break;
										}
										//SKLA
										if (posx >= 4 && codigo[s] != "PR26" && codigo[s + 1] == "XKLA" && codigo[s + 2] == "BKLA" && codigo[s + 3] == "FIIN")
										{
											sinCambios = 0;
											codigo[s + 1] = "SKLA";
											codigo[s + 2] = "";
											codigo[s + 3] = "";
											break;
										}
										if (codigo[0] == "XKLA" && codigo[1] == "BKLA" && codigo[2] == "FIIN")
										{
											sinCambios = 0;
											codigo[0] = "SKLA";
											codigo[1] = "";
											codigo[2] = "";
											break;
										}
										//SCXF
										if (posx >= 5 && codigo[s] == "PR26" && codigo[s + 1] == "XKLA" && codigo[s + 2] == "CABZ" && codigo[s + 3] == "BODY" && codigo[s + 4] == "FIIN")
										{
											sinCambios = 0;
											codigo[s] = "SCXF";
											codigo[s + 1] = "";
											codigo[s + 2] = "";
											codigo[s + 3] = "";
											codigo[s + 4] = "";
											break;
										}
										//SCXF
										if (posx >= 3 && codigo[s] == "PR26" && codigo[s + 1] == "XKLA" && codigo[s + 2] == "BODY" && codigo[s + 3] == "FIIN")
										{
											sinCambios = 0;
											codigo[s] = "SCXF";
											codigo[s + 1] = "";
											codigo[s + 2] = "";
											codigo[s + 3] = "";
											break;
										}
										//SCXF
										if (posx >= 4 && codigo[s] == "PR26" && codigo[s + 1] == "PR10" && codigo[s + 2].Substring(0, 2) == "ID" && codigo[s + 3] == "INIS" && codigo[s + 4] == "FIIN")
										{
											sinCambios = 0;
											codigo[s] = "SCXF";
											codigo[s + 1] = "";
											codigo[s + 2] = "";
											codigo[s + 3] = "";
											codigo[s + 4] = "";
											break;
										}
										//SKLA
										if (posx >= 3 && codigo[s] == "PR10" && codigo[s + 1].Substring(0, 2) == "ID" && codigo[s + 2] == "INIS" && codigo[s + 3] == "FIIN")
										{
											sinCambios = 0;
											codigo[s] = "SKLA";
											codigo[s + 1] = "";
											codigo[s + 2] = "";
											codigo[s + 3] = "";
											break;
										}
										//ACUM 
										if(s >= 3 
											&& codigo[s - 3] == "INIS"
											&& codigo[s - 2].Substring(0,2) == "ID"
											&& EsIgual(codigo[s - 1],"OPSM","OPRS")
											&& codigo[s] == "FIIN")
										{
											sinCambios = 0;
											codigo[s - 3] = "ACUM";
											codigo[s - 2] = "";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//ACUM 
										if (s >= 4
											&& codigo[s - 4] == "INIS"
											&& codigo[s - 3] == "ID"
											&& EsIgual(codigo[s - 2], "OPSM", "OPRS")
											&& EsIgual(codigo[s - 1], "OPSM", "OPRS")
											&& codigo[s] == "FIIN")
										{
											sinCambios = 0;
											codigo[s - 4] = "ACUM";
											codigo[s - 3] = "";
											codigo[s - 2] = "";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										// ID ASIG ASIGPR 
										if (s >= 5 && !EsIgual(codigo[s - 5], "PR04", "PR07", "PR11", "PR12", "PR13", "PR15", "PR18", "PR23", "PR27") && codigo[s - 4].Substring(0, 2) == "ID" && codigo[s - 3] == "ASIG" && (codigo[s - 2].Substring(0, 2) == "ID" || codigo[s-2] == "ASIGPR") && codigo[s - 1] == "OPSM" && EsIgual(codigo[s],"ASIGCONE","CONE", "ASIGCONR", "CONR", "ASIGCRTR", "CRTR", "ASIGCDNA", "CDNA", "ASIGLOG", "LOG")) //DEC ASIGDEC ID
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGPR";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//ASIGID
										if (s >= 3 && posx > 0 && !EsIgual(codigo[s - 3], "PR04", "PR07", "PR11", "PR12", "PR13", "PR15", "PR18", "PR23", "PR27") && (codigo[s - 2] == "ASIGPRIM" || codigo[s - 2].Substring(0, 2) == "ID") && codigo[s - 1] == "ASIG" && (codigo[s].Substring(0, 2) == "ID" || codigo[s] =="ASIGPR") && !SiOpAritmetico(codigo[s+1]) )
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGID";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										if (s >= 3 && posx == 0 && !EsIgual(codigo[s - 3], "PR04", "PR07", "PR11", "PR12", "PR13", "PR15", "PR18", "PR23", "PR27") && (codigo[s - 2] == "ASIGPRIM" || codigo[s - 2].Substring(0, 2) == "ID") && codigo[s - 1] == "ASIG" && (codigo[s].Substring(0, 2) == "ID" || codigo[s] == "ASIGPR"))
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGID";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//ASIGPRIM --ARIT
										if (s >= 2 && posx > 0
											&& (codigo[s - 2] == "ASIGPRIM" || codigo[s - 2].Substring(0, 2) == "ID")
											&& codigo[s - 1] == "CEX,"
											&& (codigo[s] == "ASIGPRIM" || codigo[s].Substring(0, 2) == "ID")
											&& !SiOpAritmetico(codigo[s + 1]))
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGPRIM";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//ASIGPRIM 
										if (s >= 2
											&& (codigo[s - 2] == "ASIGPRIM" || codigo[s - 2].Substring(0, 2) == "ID")
											&& codigo[s - 1] == "CEX,"
											&& (codigo[s] == "ASIGPRIM" || codigo[s].Substring(0, 2) == "ID"))
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGPRIM";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//ASIGPRIM
										if (s >= 2
											&& (codigo[s - 2] == "ASIGPRIM" || codigo[s - 2].Substring(0, 2) == "ID")
											&& SiOpAritmetico(codigo[s - 1])
											&& (codigo[s] == "ASIGPRIM" || codigo[s].Substring(0, 2) == "ID"))
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGPRIM";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//CABZ
										if (s >= 1
											&& codigo[s - 1] == "SVMT"
											&& codigo[s] == "BRMT")
										{
											sinCambios = 0;
											codigo[s - 1] = "CABZ";
											codigo[s] = "";
											break;
										}
										//CABZ
										if (s >= 3
											&& codigo[s - 3] == "SVMT"
											&& codigo[s - 2] == "INIS"
											&& codigo[s - 1] == "BMET"
											&& codigo[s] == "FIIN")
										{
											sinCambios = 0;
											codigo[s - 3] = "CABZ";
											codigo[s - 2] = "";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										////CABZ
										//if (s >= 3
										//	&& codigo[s - 3] == "MTDX"
										//	&& codigo[s - 2] == "CEX)"
										//	&& codigo[s - 1] == "INIS"
										//	&& codigo[s] == "FIIN")
										//{
										//	sinCambios = 0;
										//	codigo[s - 3] = "CABZ";
										//	codigo[s - 2] = "";
										//	codigo[s - 1] = "";
										//	codigo[s] = "";
										//	break;
										//}
										////CABZ
										//if (s >= 4
										//	&& codigo[s - 4] == "MTDX"
										//	&& codigo[s - 3] == "CEX)"
										//	&& codigo[s - 2] == "INIS"
										//	&& codigo[s - 1] == "BKLA"
										//	&& codigo[s] == "FIIN")
										//{
										//	sinCambios = 0;
										//	codigo[s - 4] = "CABZ";
										//	codigo[s - 3] = "";
										//	codigo[s - 2] = "";
										//	codigo[s - 1] = "";
										//	codigo[s] = "";
										//	break;
										//}
										////CABZ
										//if (s >= 4
										//	&& codigo[s - 4] == "MTDX"
										//	&& codigo[s - 3] == "SVMT"
										//	&& codigo[s - 2] == "INIS"
										//	&& codigo[s - 1] == "BKLA"
										//	&& codigo[s] == "FIIN")
										//{
										//	sinCambios = 0;
										//	codigo[s - 4] = "CABZ";
										//	codigo[s - 3] = "";
										//	codigo[s - 2] = "";
										//	codigo[s - 1] = "";
										//	codigo[s] = "";
										//	break;
										//}
										//CABZ BKLA
										if (s >= 1 && codigo[s - 1] == "BKLA"
											&& codigo[s] == "CABZ")
										{
											sinCambios = 0;
											codigo[s - 1] = "CABZ";
											codigo[s] = "BKLA";
											break;
										}
										//CABZ BODY
										if (s >= 1 && codigo[s - 1] == "BODY"
											&& codigo[s] == "CABZ")
										{
											sinCambios = 0;
											codigo[s - 1] = "CABZ";
											codigo[s] = "BODY";
											break;
										}
										//MTDX SVMT INIS
										if (s >= 3
											&& codigo[s - 3].Substring(0, 2) == "ID"
											&& codigo[s - 2] == "CEX("
											&& codigo[s - 1] == "SVMT"
											&& codigo[s] == "INIS")
										{
											sinCambios = 0;
											codigo[s - 3] = "MTDX";
											codigo[s - 2] = "SVMT";
											codigo[s - 1] = "INIS";
											codigo[s] = "";
											break;
										}
										//MTDX INIS
										if (s >= 2
											&& codigo[s - 2].Substring(0, 2) == "ID"
											&& codigo[s - 1] == "CEX("
											&& codigo[s] == "INIS")
										{
											sinCambios = 0;
											codigo[s - 2] = "MTDX";
											codigo[s - 1] = "INIS";
											codigo[s] = "";
											break;
										}
										//MTDX 
										if (s >= 1
											&& codigo[s - 1].Substring(0,2) == "ID"
											&& codigo[s] == "CEX(")
										{
											sinCambios = 0;
											codigo[s - 1] = "MTDX";
											codigo[s] = "";
											break;
										}
										//PR26 MTDX SVMT INIS BODY
										if (s >= 4
											&& codigo[s - 4] == "PR26"
											&& codigo[s - 3] == "MTDX"
											&& codigo[s - 2] == "SVMT"
											&& codigo[s - 1] == "INIS"
											&& EsIgual(codigo[s], "SPR03", "SPR06", "SPR09", "SPR12", "SPR16", "SPR17", "SPR21", "SPR22", "SPR24", "SPR04", "DECENT", "ASIGENT", "SPR18", "DECREA", "ASIGREA", "SPR11", "SPR13", "SPR15", "SPR23", "SIMT", "SIMV", "ASIGID","BRMT","DECENT","SIMT"))//BODY
										{
											sinCambios = 0;
											codigo[s - 4] = "PR26";
											codigo[s - 3] = "MTDX";
											codigo[s - 2] = "SVMT";
											codigo[s - 1] = "INIS";
											codigo[s] = "BODY";
											break;
										}
										//PR26 XKLA CABZ BODY
										if (s >= 5
											&& codigo[s - 5] == "PR26"
											&& codigo[s - 4] == "PR10"
							  && codigo[s - 3].Substring(0, 2) == "ID"
											&& codigo[s - 2] == "INIS"
											&& codigo[s - 1] == "CABZ"
											&& EsIgual(codigo[s], "SPR03", "SPR06", "SPR09", "SPR12", "SPR16", "SPR17", "SPR21", "SPR22", "SPR24", "SPR04", "DECENT", "ASIGENT", "SPR18", "DECREA", "ASIGREA", "SPR11", "SPR13", "SPR15", "SPR23", "SIMT", "SIMV", "ASIGID"))
										{
											sinCambios = 0;
											codigo[s - 5] = "PR26";
											codigo[s - 4] = "XKLA";
											codigo[s - 3] = "CABZ";
											codigo[s - 2] = "BODY";
											codigo[s - 1] = "PR26";
											codigo[s] = "";
											break;
										}
										//PR26 XKLA BODY
										if (s >= 4
											&& codigo[s - 4] == "PR26"
											&& codigo[s - 3] == "PR10"
							  && codigo[s - 2].Substring(0, 2) == "ID"
											&& codigo[s - 1] == "INIS"
											&& EsIgual(codigo[s], "SPR03", "SPR06", "SPR09", "SPR12", "SPR16", "SPR17", "SPR21", "SPR22", "SPR24", "SPR04", "DECENT", "ASIGENT", "SPR18", "DECREA", "ASIGREA", "SPR11", "SPR13", "SPR15", "SPR23", "SIMT", "SIMV", "ASIGID"))
										{
											sinCambios = 0;
											codigo[s - 4] = "PR26";
											codigo[s - 3] = "XKLA";
											codigo[s - 2] = "BODY";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										if (posx >= 4
											&& codigo[s] == "SVMT"
											&& codigo[s + 1] == "INIS"
											&& codigo[s + 2] == "INIS"
											&& codigo[s + 3] == "ASIGID"
											&& codigo[s + 4] == "FIN")
										{
											sinCambios = 0;
											codigo[s] = "SVMT";
											codigo[s + 1] = "INIS";
											codigo[s + 2] = "BKLA";
											codigo[s + 3] = "";
											codigo[s + 4] = "";
											break;
										}
										//MTDX SVMT INIS BKLA
										if (s >= 3
											&& codigo[s - 3] == "MTDX"
											&& codigo[s - 2] == "SVMT"
											&& codigo[s - 1] == "INIS"
											&& EsIgual(codigo[s], "SPR04", "DECENT", "ASIGENT", "SPR18", "DECREA", "ASIGREA", "SPR11", "SPR13", "SPR15", "SPR23", "SIMT", "SIMV", "ASIGID","ASIGVARMAT","SOBJ"))
										{
											sinCambios = 0;
											codigo[s - 3] = "MTDX";
											codigo[s - 2] = "SVMT";
											codigo[s - 1] = "INIS";
											codigo[s] = "BKLA";
											break;
										}
										//XKLA CABZ BKLA
										if (s >= 4
											&& codigo[s - 4] == "PR10"
							  && codigo[s - 3].Substring(0, 2) == "ID"
											&& codigo[s - 2] == "INIS"
											&& codigo[s - 1] == "CABZ"
											&& EsIgual(codigo[s], "SPR04", "DECENT", "ASIGENT", "SPR18", "DECREA", "ASIGREA", "SPR11", "SPR13", "SPR15", "SPR23", "SIMT", "SIMV", "ASIGID","ASIGVARMAT","SMAT","SOBJ"))
										{
											sinCambios = 0;
											codigo[s - 4] = "XKLA";
											codigo[s - 3] = "BKLA";
											codigo[s - 2] = "";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//XKLA BKLA
										if (s >= 3
											&& codigo[s - 3] == "PR10"
							  && codigo[s - 2].Substring(0, 2) == "ID"
											&& codigo[s - 1] == "INIS"
											&& EsIgual(codigo[s], "SPR04", "DECENT", "ASIGENT", "SPR18", "DECREA", "ASIGREA", "SPR11", "SPR13", "SPR15", "SPR23", "SIMT", "SIMV", "ASIGID","ASIGVARMAT","SMAT","SOBJ"))
										{
											sinCambios = 0;
											codigo[s - 3] = "XKLA";
											codigo[s - 2] = "BKLA";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//BKLA
										if (s >= 1 && codigo[s - 1] == "BKLA"
											&& EsIgual(codigo[s], "SPR04", "DECENT", "ASIGENT", "SPR18", "DECREA", "ASIGREA", "SPR11", "SPR13", "SPR15", "SPR23", "SIMT", "SIMV", "ASIGID","ASIGVARMAT","SMAT","SOBJ")) //$BKLA
										{
											sinCambios = 0;
											codigo[s - 1] = "BKLA";
											codigo[s] = "";
											break;
										}
										//BODY
										if (s >= 1 && codigo[s - 1] == "BODY"
											&& EsIgual(codigo[s], "SPR03", "SPR06", "SPR09", "SPR12", "SPR16", "SPR17", "SPR21", "SPR22", "SPR24", "SPR04", "DECENT", "ASIGENT", "SPR18", "DECREA", "ASIGREA", "SPR11", "SPR13", "SPR15", "SPR23", "SIMT", "SIMV", "ASIGID","ASIGVARMAT","SMAT","BMET","SOBJ")) //$BODY
										{
											sinCambios = 0;
											codigo[s - 1] = "BODY";
											codigo[s] = "";
											break;
										}
									}

									if (s == codigo.Length - 1 && sinCambios == 2)
									{
										Finalizo = true;
									}
								}
								sinCambios++;

							} while (sinCambios < 4 && !Finalizo);
							if (sinCambios > 3)
							{
								Advertir(rchConsola3, "No encontro mas cambios", "Revisa la consola 3");
							}
							else
							{
								cadenaSintactico = "";
								int errores = 0;
								foreach (string str in codigo)
								{
									
									cadenaSintactico += str + "\n";
									if (!(str == "SKLA" || str == "SCXF"))
									{
										errores++;
									}
								}
								if(errores>0)
								{
									rchConsola3.Text = "Se encontraron errores";
								}else
								{
									
									rchConsola3.Text = "Exito";
								}
								rchAnalisisSintactico.Text = cat;
								rchSintactico.Text = cadenaSintactico;
							}

						}
						else if (numI > numF)
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
						Mensaje(ex.Message );
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
			}else
			{
				MessageBox.Show("Debes corregir los errores primero");
			}
		}
		private bool BLOQUEA(string text)
		{
			if(text == "SPR03" || text == "SPR04" || text == "SPR06" || text == "SPR07" || text == "SPR08" || text == "SPR09" || text == "SPR11" || text == "SPR12" || text == "SPR13" || text == "SPR15" || text == "SPR16" || text == "SPR17" || text == "SPR18" || text == "SPR21" || text == "SPR22" || text == "SPR23" || text == "SPR24" || text == "DAR" || text == "ROM" || text == "ASIGID" )
			{
				return true;
			}
			return false;
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
			string repuesto = tx.r3;
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

		private void btnAjustar_Click(object sender, EventArgs e)
		{
			rchTexto.SelectionFont = new Font("Microsoft Sans Serif", 9, FontStyle.Regular);
			rchTexto.SelectionColor = Color.Black;
		}
	}
}
