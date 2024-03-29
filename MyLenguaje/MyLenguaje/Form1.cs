﻿using MySql.Data.MySqlClient;
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
using System.Text.RegularExpressions;

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
		bool EstadoSintactico = false;
		bool EstadoSemantico = false;
		string[][] matrizLexico;
		string[][] matrizCodigo;
		List<MetaDatos> _listaMeta = new List<MetaDatos>();
		List<string[]> listaDeStringCodIntermed = new List<string[]>();

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
		bool EstadoIntermedio = false;
		private void btnAnalizar_Click(object sender, EventArgs e)
		{
			try
			{
				EstaForma.Cursor = Cursors.AppStarting;
				dtgSimbolo.Rows.Clear();
				_listaMeta.Clear();
				EstadoSintactico = false;
				EstadoSemantico = false;
				listaDeStringCodIntermed.Clear();
				IsCorrectInPosPrefijo = false;
				rchPosPrefijo.Text = "";
				EstadoIntermedio = false;
				if (!String.IsNullOrEmpty(rchTexto.Text))
				{
					rchConsola1.Text = "Exito";
					rchConsola2.Text = "";
					rchConsola3.Text = "";
					rchConsola4.Text = "";
					rchLexico.Text = "";
					rchSintactico.Text = "";
					rchSemantico.Text = "";
					rchConsola5.Text = "Exito";
					string codigo2 = "";
					int y = 0;

					foreach (char car in rchTexto.Text)
					{
						if (car == ' ')
						{
							codigo2 += "" + careEspacio;
						}
						else
						if (car == '\n')
						{
							codigo2 += "" + careEnter;
						}
						else
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
					bool modoLetra = false;
					string palabras = "x ";
					string letras = "";
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
							if ((codigo[y] > 125 && codigo[y] != careEnter && codigo[y] != careEspacio && codigo[y] != careFin) || CaracterProhibido(posicion))
							{
								rchConsola1.Text = "Error: Letra no permitida en el lenguaje " + codigo[y];
								throw new Exception("Se a encontrado un caracter fuera del rango valido: Caracter: " + codigo[y]);
							}
							if (codigo[y] == careEspacio || codigo[y] == careFin || codigo[y] == careEnter)
							{
								if (espacio == "92")
								{
									if (modoLetra)
									{
										palabras += letras + ' ';
										letras = "";
										modoLetra = false;
									}
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
									//if (codigo[y] == careEnter)
									//{
									//	cat += "\n";
									//	haltura++;
									//	wancho = 1;
									//	listo = false;
									//}

									consola += reader["FDC"].ToString();
									consola += "\n";
								}
								if (codigo[y] == careEnter)
								{
									cat += "\n";
									haltura++;
									wancho = 1;
									listo = false;
								}

							}
							else
							{
								if (codigo[y] == '_')
								{
									modoLetra = true;
								}
								if (modoLetra)
								{
									letras += codigo[y];
								}
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
						rchConsola2.Text = "Exito";//Revision exitosa --cambio
						EstadoAnalisis = true;
					}
					else
					{
						rchConsola2.Text = textoError;
						EstadoAnalisis = false;
					}
					char gar = char.Parse(cat.Substring(cat.Length - 1, 1));
					if (gar == '\n')
					{
						cadenaLexico = cat + careFin;
					}
					else
					if (gar == careFin)
					{
						cadenaLexico = cat;
					}
					else
					if (gar == careEspacio)
					{
						cadenaLexico = cat + " " + careFin;
					}
					else
					{
						cadenaLexico = cat;
					}
					matrizLexico = ConvertirTextoEnMatriz(cadenaLexico);
					matrizCodigo = ConvertirCodigoEnMatriz(rchTexto.Text);
					int count = 0;
					int NumID = 0;
					int NumENT = 0, NumREA = 0, NumSXN = 0, NumKAR = 0;
					for (int i = 0; i < matrizLexico.Length; i++)
					{
						// Obtener el número de columnas de la fila actual
						int numColumnas = matrizLexico[i].Length;

						for (int j = 0; j < numColumnas; j++)
						{
							if (i == 106 && j == 0) //ignorar
							{

							}
							if (matrizLexico[i][j] == "IDEN") //Buscador --ignorar
							{
								if (matrizCodigo[i][j] == "_Operaciones(")
								{

								}
							}
							int faltantes = numColumnas - j;
							MetaDatos meta = new MetaDatos();
							//Declaracion de variable
							//Guardar ENT PR04 KAR PR07 LOG PR11 REA PR18 SXN PR23
							if (j >= 4 && matrizLexico[i][j - 1] == "ASIG"
									 && matrizLexico[i][j - 2] == "IDEN"
									 && EsIgual(matrizLexico[i][j - 3], "PR04", "PR07", "PR11", "PR18", "PR23")
									 && matrizLexico[i][j - 4] != "PR13")
							{
								//matrizLexico[i][j - 0] == "CONE" 
								meta = new MetaDatos();
								count++;
								meta.ID = count;
								meta.Variable = matrizCodigo[i][j - 2];//_x
								meta.TipoDato = matrizCodigo[i][j - 3].ToUpper();//Tipo de dato Mayuscula = ENT
								meta.Token = matrizLexico[i][j - 2];
								meta.Valor = matrizCodigo[i][j];
								NumID++;
								meta.TokenUnico = MarcarToken("ID", NumID);
								meta.Fila = i + 1;
								//Declaracion de id
								_listaMeta.Add(meta);
							}
							else
							//Guardar declaraciones sin asignacion
							if (j >= 3 && faltantes > 0
									 && matrizLexico[i][j - 1] == "IDEN"
									 && EsIgual(matrizLexico[i][j - 2], "PR04", "PR07", "PR11", "PR18", "PR23")
									 && matrizLexico[i][j - 3] != "PR13"
									 && matrizLexico[i][j] != "ASIG")//	3:!MAT 2:ENT 1:_X 0:!ASIG
							{
								//matrizLexico[i][j - 0] == "FIIN" 
								meta = new MetaDatos();
								count++;
								meta.ID = count;
								meta.Variable = matrizCodigo[i][j - 1];//_x
								meta.TipoDato = matrizCodigo[i][j - 2].ToUpper();//Tipo de dato Mayuscula = ENT
								meta.Token = matrizLexico[i][j - 1];
								meta.Valor = "NULL";
								NumID++;
								meta.TokenUnico = MarcarToken("ID", NumID);
								meta.Fila = i + 1;
								//Declaracion de id
								_listaMeta.Add(meta);
							}
							else
							//Guardar declaracion y asigancion de OBJ PR15
							if (j >= 5 && matrizLexico[i][j] == "IDEN"
									  && matrizLexico[i][j - 1] == "PR14"
									  && matrizLexico[i][j - 2] == "ASIG"
									  && matrizLexico[i][j - 3] == "IDEN"
									  && matrizLexico[i][j - 4] == "IDEN"
									  && matrizLexico[i][j - 5] == "PR15")
							// INIS 5:PR15 4:IDEN 3:IDEN 2:ASIG 1:PR14 0:IDEN CEX( CEX) FIIN
							{
								meta = new MetaDatos();
								count++;
								meta.ID = count;
								meta.Variable = matrizCodigo[i][j - 3];//Nombre var: _x
								meta.TipoDato = matrizCodigo[i][j - 5].ToUpper() + "." + matrizCodigo[i][j - 4];
								//Tipo de dato Mayuscula = OBJ._y
								meta.Token = matrizLexico[i][j - 3];
								meta.Valor = matrizCodigo[i][j - 4] + "()";
								NumID++;
								meta.TokenUnico = MarcarToken("ID", NumID);
								meta.Fila = i + 1;
								//Declaracion de id
								_listaMeta.Add(meta);
							}
							else
							//Guardar declaracion y asignacion de OBJ PR15
							if (j >= 3 && matrizLexico[i][j - 0] != "ASIG"
									  && matrizLexico[i][j - 1] == "IDEN"
									  && matrizLexico[i][j - 2] == "IDEN"
									  && matrizLexico[i][j - 3] == "PR15")// INIS 3:PR15 2:IDEN 1:IDEN 0:!ASIG 
							{
								meta = new MetaDatos();
								count++;
								meta.ID = count;
								meta.Variable = matrizCodigo[i][j - 1];//Nombre var: _x
								meta.TipoDato = matrizCodigo[i][j - 3].ToUpper() + "." + matrizCodigo[i][j - 2];//Tipo de dato Mayuscula = ENT
								meta.Token = matrizLexico[i][j - 3];
								meta.Valor = "NULL";
								NumID++;
								meta.TokenUnico = MarcarToken("ID", NumID);
								meta.Fila = i + 1;
								//Declaracion de id
								_listaMeta.Add(meta);
							}
							else
							//Declarar arreglo Mat e inicializado
							if (j >= 9 && matrizLexico[i][j - 0] == "CONE"
									  && matrizLexico[i][j - 1] == "CEX["
									  && (matrizLexico[i][j - 2] == matrizLexico[i][j - 8])//Los dos sean igual sxn = sxn
									  && matrizLexico[i][j - 3] == "PR14"
									  && matrizLexico[i][j - 4] == "ASIG"
									  && matrizLexico[i][j - 5] == "IDEN"
									  && matrizLexico[i][j - 6] == "CEX]"
									  && matrizLexico[i][j - 7] == "CEX["
									  && matrizLexico[i][j - 9] == "PR13")
							//INIS 9:PR13 8:PR23 7:CEX[ 6:CEX] 5:IDEN       4:ASIG 3:PR14 2:PR23 1:CEX[ 0:CONE CEX] FIIN
							//  |  MAT       sxn  [        ]   _sxnArray       =     NOV    sxn   [        0    ] ||
							{
								meta = new MetaDatos();
								count++;
								meta.ID = count;
								meta.Variable = matrizCodigo[i][j - 5];//Nombre var: _x
								meta.TipoDato = "MAT " + matrizCodigo[i][j - 2].ToUpper();
								meta.Token = matrizLexico[i][j - 5];
								int n = int.Parse(matrizCodigo[i][j]);
								meta.Valor = "{ " + string.Join(", ", Enumerable.Repeat("null", n)) + " }";
								NumID++;
								meta.TokenUnico = MarcarToken("ID", NumID);
								meta.Fila = i + 1;
								//Declaracion de id
								_listaMeta.Add(meta);
							}
							else
							//Declarar arreglo Mat y meterle valores
							if (j >= 6 && matrizLexico[i][j - 0] == "CEX{"
									   && matrizLexico[i][j - 1] == "ASIG"
									   && matrizLexico[i][j - 2] == "IDEN"
									   && matrizLexico[i][j - 3] == "CEX]"
									   && matrizLexico[i][j - 4] == "CEX["
									   && EsIgual(matrizLexico[i][j - 5], "PR04", "PR07", "PR11", "PR18", "PR23")
									   && matrizLexico[i][j - 6] == "PR13")
							//INIS   6:PR13 5:PR04 4:CEX[ 3:CEX] 2:IDEN       1:ASIG 0:CEX{ CONE CEX, CONE CEX, CONE CEX} FIIN
							// |     MAT ENT    [     ]  _entArray1 =     {     1 , 3 , 4 } ||
							{
								meta = new MetaDatos();
								count++;
								meta.ID = count;
								meta.Variable = matrizCodigo[i][j - 2];//Nombre var: _x
								meta.TipoDato = "MAT " + matrizCodigo[i][j - 5].ToUpper();
								meta.Token = matrizLexico[i][j - 2];
								//int n = int.Parse(matrizCodigo[i][j]);
								// Encuentra la posición del primer '{'
								int startIndex = Array.IndexOf(matrizCodigo[i], "{");

								// Encuentra la posición del último '}'
								int endIndex = Array.LastIndexOf(matrizCodigo[i], "}");
								// Crea un nuevo arreglo que contenga solo los elementos entre el primer '{' y el último '}'
								string[] newArr = new string[endIndex - startIndex - 1];
								Array.Copy(matrizCodigo[i], startIndex + 1, newArr, 0, endIndex - startIndex - 1);

								// Convierte el nuevo arreglo en una cadena usando la función string.Join()
								string result = "{" + string.Join("", newArr) + "}";
								meta.Valor = result;
								NumID++;
								meta.TokenUnico = MarcarToken("ID", NumID);
								meta.Fila = i + 1;
								//Declaracion de id
								_listaMeta.Add(meta);
							}
							else
							//Declarar arreglo Mat Sin inicializar
							if (j >= 4 && faltantes > 1
										&& matrizLexico[i][j + 1] != "ASIG"
									  && matrizLexico[i][j - 0] == "IDEN"
									  && EsIgual(matrizLexico[i][j - 3], "PR04", "PR07", "PR11", "PR18", "PR23")
									  && matrizLexico[i][j - 4] == "PR13")
							//INIS  PR13    3:PR04 2:CEX[ 1:CEX] 0:IDEN FIIN !ASIG
							//|     MAT     ENT   [     ]  _entArray ||
							{
								meta = new MetaDatos();
								count++;
								meta.ID = count;
								meta.Variable = matrizCodigo[i][j - 0];//Nombre var: _x
								meta.TipoDato = "MAT " + matrizCodigo[i][j - 3].ToUpper();
								meta.Token = "IDEN";
								meta.Valor = "NULL";
								NumID++;
								meta.TokenUnico = MarcarToken("ID", NumID);
								meta.Fila = i + 1;
								//Declaracion de id
								_listaMeta.Add(meta);
							}
							else
							//Declaracion de clase main
							if (j >= 2 && matrizLexico[i][j - 0] == "IDEN"
									  && matrizLexico[i][j - 1] == "PR10"
									  && matrizLexico[i][j - 2] == "PR26")// PR26 PR10 IDEN-CXFA KLA _X
							{
								meta = new MetaDatos();
								count++;
								meta.ID = count;
								meta.Variable = matrizCodigo[i][j - 0];//Nombre var: _x
								meta.TipoDato = "CXFA KLA";
								meta.Token = matrizLexico[i][j - 0];
								meta.Valor = "KLA";
								NumID++;
								meta.TokenUnico = MarcarToken("ID", NumID);
								meta.Fila = i + 1;
								//Declaracion de id
								_listaMeta.Add(meta);
							}
							else
							//Declaracion de clase
							if (j >= 2 && matrizLexico[i][j - 0] == "IDEN"
									  && matrizLexico[i][j - 1] == "PR10"
									  && matrizLexico[i][j - 2] != "PR26")// !PR26 PR10 IDEN- KLA _X
							{
								meta = new MetaDatos();
								count++;
								meta.ID = count;
								meta.Variable = matrizCodigo[i][j - 0];//Nombre var: _x
								meta.TipoDato = "KLA";
								meta.Token = matrizLexico[i][j - 0];
								meta.Valor = "KLA";
								NumID++;
								meta.TokenUnico = MarcarToken("ID", NumID);
								meta.Fila = i + 1;
								//Declaracion de id
								_listaMeta.Add(meta);
							}
							else
							//Declaracion de clase
							if (j >= 1 && matrizLexico[i][j - 0] == "IDEN"
									  && matrizLexico[i][j - 1] == "PR10")// PR10 IDEN-KLA _X
							{
								meta = new MetaDatos();
								count++;
								meta.ID = count;
								meta.Variable = matrizCodigo[i][j - 0];//Nombre var: _x
								meta.TipoDato = "KLA";
								meta.Token = matrizLexico[i][j - 0];
								meta.Valor = "KLA";
								NumID++;
								meta.TokenUnico = MarcarToken("ID", NumID);
								meta.Fila = i + 1;
								//Declaracion de id
								_listaMeta.Add(meta);
							}
							else
							//Declaracion de metodos con parametros
							if (j >= 4 && matrizLexico[i][j - 0] == "IDEN"
									  && matrizLexico[i][j - 2] == "CEX("
									  && matrizLexico[i][j - 3] == "IDEN"
									  && EsIgual(matrizLexico[i][j - 4], "PR04", "PR07", "PR11", "PR18", "PR23", "PR27"))
							//4:PR27 3:IDEN 2:CEX( 1:PR04 0:IDEN CEX)
							{
								meta = new MetaDatos();
								count++;
								meta.ID = count;
								meta.Variable = matrizCodigo[i][j - 3];//Nombre var: _x
								meta.TipoDato = "MTDX " + matrizCodigo[i][j - 4].ToUpper();//MLP
								meta.Token = matrizLexico[i][j - 3];
								meta.Valor = matrizCodigo[i][j - 0];
								int n = numColumnas - 3;

								string[] ultimosL = matrizLexico[i].Reverse().Take(n).ToArray();
								string[] ultimosC = matrizCodigo[i].Reverse().Take(n).ToArray();
								Array.Reverse(ultimosL); Array.Reverse(ultimosC);
								while (ultimosL.Length > 1)
								{
									if (n >= 2 && ultimosL.Length >= 3
										  && ultimosL[2] == "CEX,"
										  //&& ultimosL[1] == "PR04"
										  && ultimosL[0] == "IDEN")
									{
										meta.Valor += ", " + ultimosC[0];
									}
									// Eliminar el primer elemento de los arreglos
									Array.Resize(ref ultimosL, ultimosL.Length - 1);
									Array.Resize(ref ultimosC, ultimosC.Length - 1);

									// Si queda solo un elemento en los arreglos, salir del ciclo
									if (ultimosL.Length == 1) break;
								}

								NumID++;
								meta.TokenUnico = MarcarToken("ID", NumID);
								meta.Fila = i + 1;
								//Declaracion de id
								_listaMeta.Add(meta);
							}
							else
							if (j >= 2 && faltantes > 1
									  && !EsIgual(matrizLexico[i][j + 1], "PR04", "PR07", "PR11", "PR18", "PR23")
									  && matrizLexico[i][j - 0] == "CEX("
									  && matrizLexico[i][j - 1] == "IDEN"
									  && EsIgual(matrizLexico[i][j - 2], "PR04", "PR07", "PR11", "PR18", "PR23", "PR27"))
							//2:PR27 1:IDEN 0:CEX( +1:Tipo
							{
								meta = new MetaDatos();
								count++;
								meta.ID = count;
								meta.Variable = matrizCodigo[i][j - 1];//Nombre var: _x
								meta.TipoDato = "MTDX " + matrizCodigo[i][j - 2].ToUpper();
								meta.Token = matrizLexico[i][j - 1];
								meta.Valor = "NULL";
								NumID++;
								meta.TokenUnico = MarcarToken("ID", NumID);
								meta.Fila = i + 1;
								//Declaracion de id
								_listaMeta.Add(meta);
							}
							else
							//Asignacion de variables
							if (j >= 2 && faltantes > 1
									   && EsIgual(matrizLexico[i][j - 2], "INIS", "CEX,")
									   && matrizLexico[i][j - 1] == "IDEN"
									   && matrizLexico[i][j - 0] == "ASIG")
							//&& matrizLexico[i][j + 1] != "ASIG")
							//2:INIS  1:IDEN   0:ASIG +1:CONE FIIN
							//|     _num   =    2    ||
							//| _entArray2 = { 1 , 3 , 4 } ||
							//INIS IDEN    ASIG PR14 IDEN CEX( CEX) FIIN
							//   |  _prueba =   nov _Prueba (  ) ||
							{
								meta = new MetaDatos();
								meta.Variable = matrizCodigo[i][j - 1];//Nombre var: _x
								meta.TipoDato = "VAR";
								meta.Token = matrizLexico[i][j - 1];
								if (matrizLexico[i][j + 1] == "CEX{")
								{
									// Encuentra la posición del primer '{'
									int startIndex = Array.IndexOf(matrizCodigo[i], "{");

									// Encuentra la posición del último '}'
									int endIndex = Array.LastIndexOf(matrizCodigo[i], "}");
									// Crea un nuevo arreglo que contenga solo los elementos entre el primer '{' y el último '}'
									string[] newArr = new string[endIndex - startIndex - 1];
									Array.Copy(matrizCodigo[i], startIndex + 1, newArr, 0, endIndex - startIndex - 1);

									// Convierte el nuevo arreglo en una cadena usando la función string.Join()
									string result = "{" + string.Join("", newArr) + "}";
									meta.Valor = result;
								}
								else
								if (faltantes > 2 && matrizLexico[i][j + 1] == "PR14")
								{
									meta.Valor = matrizCodigo[i][j + 2] + "()";
								}
								else
								if (matrizLexico[i][j + 1] != "IDEN")
								{
									meta.Valor = matrizCodigo[i][j + 1];
								}
								NumID++;
								meta.TokenUnico = MarcarToken("ID", NumID);
								meta.Fila = i + 1;
								//Declaracion de id
								if (!_listaMeta.Any(m => m.Variable == meta.Variable))
								{
									//Si no existe lo agrega pero sin TiPoDato especifico: VAR
									count++;
									meta.ID = count;
									_listaMeta.Add(meta);
									//listaFilasAceptadas.Add(meta.Fila);
								}
								else
								{
									foreach (MetaDatos objeto in _listaMeta)
									{
										if (objeto.Variable == meta.Variable)//Busca que ambas tengan el mismo nombre _x
										{
											if (matrizLexico[i][j + 1] != "IDEN" && meta.Valor != "")//Si no es un idenficador y su valor esta vacio
											{
												objeto.Valor = meta.Valor;
												break;
											}
										}
									}
								}
							}
							if (matrizLexico[i][j] == "CONE")
							{
								meta = new MetaDatos();
								meta.Variable = "NULL";
								meta.TipoDato = "ENT";
								meta.Token = matrizLexico[i][j];
								meta.Valor = matrizCodigo[i][j];
								meta.Fila = 0;
								// Buscamos si existe un objeto con el mismo valor
								if (!_listaMeta.Any(m => m.Valor == meta.Valor && m.Variable == "NULL"))
								{
									count++;
									meta.ID = count;
									NumENT++;
									meta.TokenUnico = MarcarToken("CNE", NumENT);//regresa CNE01
																				 // Si no existe, lo agregamos a la lista
									_listaMeta.Add(meta);
								}
							}
							else
							if (matrizLexico[i][j] == "CONR")
							{
								meta = new MetaDatos();
								meta.Variable = "NULL";
								meta.TipoDato = "REA";
								meta.Token = matrizLexico[i][j];
								meta.Valor = matrizCodigo[i][j];
								meta.Fila = 0;
								// Buscamos si existe un objeto con el mismo valor
								if (!_listaMeta.Any(m => m.Valor == meta.Valor && m.Variable == "NULL"))
								{
									count++;
									meta.ID = count;
									NumREA++;
									meta.TokenUnico = MarcarToken("CNR", NumREA);//regresa CNE01
																				 // Si no existe, lo agregamos a la lista
									_listaMeta.Add(meta);
								}
							}
							else
							if (matrizLexico[i][j] == "CDNA")
							{
								meta = new MetaDatos();
								meta.Variable = "NULL";
								meta.TipoDato = "SXN";
								meta.Token = matrizLexico[i][j];
								meta.Valor = matrizCodigo[i][j];
								meta.Fila = 0;
								// Buscamos si existe un objeto con el mismo valor
								if (!_listaMeta.Any(m => m.Valor == meta.Valor && m.Variable == "NULL"))
								{
									count++;
									meta.ID = count;
									NumSXN++;
									meta.TokenUnico = MarcarToken("CDN", NumSXN);//regresa CNE01
																				 // Si no existe, lo agregamos a la lista
									_listaMeta.Add(meta);
								}
							}
							else
							if (matrizLexico[i][j] == "CRTR")
							{
								meta = new MetaDatos();
								meta.Variable = "NULL";
								meta.TipoDato = "KAR";
								meta.Token = matrizLexico[i][j];
								meta.Valor = matrizCodigo[i][j];
								meta.Fila = 0;
								// Buscamos si existe un objeto con el mismo valor
								if (!_listaMeta.Any(m => m.Valor == meta.Valor && m.Variable == "NULL"))
								{
									count++;
									meta.ID = count;
									NumKAR++;
									meta.TokenUnico = MarcarToken("CTR", NumKAR);//regresa CNE01
																				 // Si no existe, lo agregamos a la lista
									_listaMeta.Add(meta);
								}
							}
							else
							{
								//VERA y FALSA ya tiene su identicador unico y son solo dos valores
							}
						}

					}
					LlenarTablaSimbolos();
					InicializarCodigo(rchLexico, cadenaLexico, false);//true enseña todos los caracteres ocultos
					rhcAnalisisLexico.Text = consola;

					//rchConsola2.Text = consola;
				}
				else
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
		public static string MarcarToken(string str, int num)
		{
			string numStr = num.ToString();
			if (num < 10)
			{
				numStr = "0" + numStr;
			}
			return str + numStr;
		}
		public string[][] ConvertirTextoEnMatriz(string texto)
		{
			string[] lineas = texto.Split('\n'); // Dividimos el texto en líneas
			int numFilas = lineas.Length;

			// Encontrar la longitud máxima de cualquier renglón
			int longitudMaxima = 0;
			foreach (string linea in lineas)
			{
				int longitud = linea.Split(careEspacio).Length;
				if (longitud > longitudMaxima)
				{
					longitudMaxima = longitud;
				}
			}

			// Inicializar la matriz con la longitud máxima encontrada
			string[][] matriz = new string[numFilas][];

			for (int i = 0; i < numFilas; i++)
			{
				string[] palabras = lineas[i].Split(careEspacio); // Dividimos cada línea en palabras
				int longitudReal = palabras.Length;
				matriz[i] = new string[longitudReal];
				for (int j = 0; j < longitudReal; j++)
				{
					matriz[i][j] = palabras[j]; // Asignamos cada palabra a la posición correspondiente de la matriz
				}
			}

			return matriz;
		}

		public static string[][] ConvertirCodigoEnMatriz(string texto)
		{
			string[] lineas = texto.Split('\n'); // Dividimos el texto en líneas
			int numFilas = lineas.Length;
			string[][] matriz = new string[numFilas][];

			for (int i = 0; i < numFilas; i++)
			{
				string[] palabras = lineas[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); // Dividimos cada línea en palabras
				List<string> fila = new List<string>();

				for (int j = 0; j < palabras.Length; j++)
				{
					string palabraActual = palabras[j];
					if (palabraActual.StartsWith("\""))
					{
						// Si la palabra actual empieza con una comilla doble, concatenamos las palabras siguientes hasta encontrar una que termine con una comilla doble
						while (!palabraActual.EndsWith("\"") && j < palabras.Length - 1)
						{
							j++;
							palabraActual += " " + palabras[j];
						}
					}
					fila.Add(palabraActual); // Agregamos la palabra a la fila actual
				}

				matriz[i] = fila.ToArray(); // Convertimos la fila actual en un arreglo y la agregamos a la matriz
			}

			return matriz;
		}

		private void LlenarTablaSimbolos()
		{
			dtgSimbolo.Rows.Clear();
			if (_listaMeta.Count > 0)
			{
				foreach (MetaDatos meta in _listaMeta)
				{
					string a1 = meta.ID + "";
					string a2 = meta.Variable;
					string a3 = meta.TipoDato;
					string a4 = meta.Token;
					string a5 = meta.Valor;
					string a6 = meta.TokenUnico;
					string a7 = meta.Fila + "";
					dtgSimbolo.Rows.Add(a1, a2, a3, a4, a5, a6, a7);
				}
			}
		}
		private void Advertir(RichTextBox rich, string textoRich, string textoMensaje)
		{
			rich.Text = textoRich;
			Mensaje(textoMensaje);
		}
		private void InicializarCodigo(RichTextBox rich, string cadena, bool All)
		{
			if (All)
			{
				rich.Text = cadena;
			}
			else
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
				x += "" + cc[i];
			}
			MessageBox.Show("" + x, "Mensaje de arreglo");
		}
		private string MensajeArray(string[] cc, bool comp)
		{
			string x = "";
			for (int i = 0; i < cc.Length; i++)
			{
				x += cc[i] + " ";
			}
			if (comp)
			{
				MessageBox.Show(x, "Mensaje de arreglo");
			}
			return x;
		}
		//CODIGO TEMP
		private void btnLexicar_Click(object sender, EventArgs e)
		{
			if (EstadoAnalisis)
			{
				EstaForma.Cursor = Cursors.AppStarting;
				rchConsola3.Text = "";
				rchSintactico.Text = "";
				if (!String.IsNullOrWhiteSpace(rchLexico.Text) || !String.IsNullOrEmpty(rchLexico.Text))
				{
					//string MensajeError = "";
					try
					{
						//bool final = true;
						string[] lexicos = cadenaLexico.Split(careEspacio, '\n', ' ');//rchLexico.Text.Split(' ','\n');
						string[] entreEnter = cadenaLexico.Split('\n');

						int MayorFila = 0;
						int[] MetaArray;
						string CadenaNumeros = "";
						bool primero = true;
						for (int i = 0; i < entreEnter.Length; i++)
						{
							string[] lex = entreEnter[i].Split(careEspacio);
							string cod1 = "";
							int cont1 = 0;
							foreach (string str in lex)
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
							lex = cod1.Split(' ');
							int x = lex.Length;
							//if((lex.Length-1) > 0)
							//{
							//	x = lex.Length - 1;
							//}
							if (primero)
							{
								CadenaNumeros = "" + x;
								primero = false;
							}
							else
							{
								CadenaNumeros += " " + x;
							}
							if (x > MayorFila)
							{
								MayorFila = x;
							}
						}
						string cadena1 = "";
						MetaArray = Array.ConvertAll<string, int>(CadenaNumeros.Split(' '), int.Parse);
						string[,] arrayCodigo = new string[entreEnter.Length, MayorFila];

						//llenar arreglo
						for (int i = 0; i < entreEnter.Length; i++)
						{
							string[] fila = entreEnter[i].Split(careEspacio);
							string cod1 = "";
							int cont1 = 0;
							int numid = 0;
							foreach (string str in fila)
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
							fila = cod1.Split(' ');
							for (int j = 0; j < fila.Length; j++)
							{
								if (fila[j] == "IDEN")
								{
									numid++;
									if (Math.Floor(Math.Log10(numid) + 1) == 1)
									{
										arrayCodigo[i, j] = "ID" + "0" + numid;
									}
									else
									{

										cadena1 += "ID" + numid;
									}
								}
								else
									arrayCodigo[i, j] = fila[j];
							}
						}

						string[,] respaldo = arrayCodigo;//por si acaso 
						bool finalizo = false;
						int sinCambios = 0;
						int compVuelta = 0;
						int cambios = 0;

						for (int i = 0; i < arrayCodigo.GetLength(0); i++)
						{
							if (MetaArray[i] > 1)
							{

								string[] arregloFila = new string[MetaArray[i]];
								for (int j = 0; j < arregloFila.Length; j++)
								{
									arregloFila[j] = arrayCodigo[i, j];
								}
								do
								{
									SintacticoSinClase(ref arregloFila, ref compVuelta, ref cambios, ref sinCambios, ref finalizo);
									int cont2 = 0;
									string cod2 = "";
									foreach (string str in arregloFila)
									{

										if (str != "" && str != "" + careFin)
										{
											if (cont2 == 0)
											{
												cont2++;
											}
											else
											{
												cod2 += " ";
											}
											cod2 += str;
										}
										else
										{

										}
									}
									arregloFila = cod2.Split(' ');
								} while (sinCambios < 4);
								for (int j = 0; j < arrayCodigo.GetLength(1); j++)
								{
									arrayCodigo[i, j] = null;
								}
								for (int j = 0; j < arregloFila.Length; j++)
								{

									arrayCodigo[i, j] = arregloFila[j];
								}
							}
						}
						string prueba = "";
						//imprimir la matriz
						for (int i = 0; i < arrayCodigo.GetLength(0); i++)
						{
							for (int j = 0; j < MetaArray[i]; j++)
							{
								if (j != 0)
								{
									prueba += " ";
								}
								prueba += arrayCodigo[i, j];
							}
							prueba += "\n";
						}
						cadenaSintactico = prueba;
						rchSintactico.Text = cadenaSintactico + "";
						int numeroErrores = 0;
						string consolaError = "";
						string catEstado = "";
						bool verificado = false;
						//Busqueda de errores
						for (int i = 0; i < arrayCodigo.GetLength(0); i++)
						{
							int errores = 0;
							int longitud = 0;
							int fila = i + 1;
							bool estaticoReglaCXFA = false;
							for (int j = 0; j < arrayCodigo.GetLength(1); j++)
							{
								if (arrayCodigo[i, j] != null)
								{
									longitud++;
								}
							}

							for (int j = 0; j < longitud; j++)
							{
								if (arrayCodigo[i, j] != null)
								{
									//Error si le falta un = de asignacion -- | ent _x 1 ||
									if (j >= 1 && EsIgual(arrayCodigo[i, j - 1], "DECENT", "DECREA", "DECKAR", "DECSXN", "DECLOG")
											&& EsIgual(arrayCodigo[i, j - 0], "CONE", "CONR", "CRTR", "CDNA", "BLN"))
									{
										estaticoReglaCXFA = true;
										numeroErrores++;
										errores++;
										consolaError += "Error" + $" {numeroErrores}:" + $"({fila},{8}) " + "Se espera el caracter = para asignar la variable.\n";
									}
									if (longitud > 1)
									{
										int expRestantes = longitud - j;
										//Reglas
										if (!estaticoReglaCXFA && longitud > 2 && arrayCodigo[i, 0] == "PR26")
										{
											if (arrayCodigo[i, 1] == "PR10"
												&& arrayCodigo[i, 2].Substring(0, 2) == "ID")
											{
												estaticoReglaCXFA = true;
											}
											else if (arrayCodigo[i, 1] != "PR10"
											   && arrayCodigo[i, 2].Substring(0, 2) == "ID")
											{
												estaticoReglaCXFA = true;
												numeroErrores++;
												errores++;
												consolaError += "Error" + $" {numeroErrores}:" + $"({fila},{4 + 1}) " + "Debe seguir la palabra 'KLA' despues de la palabra 'CXFA'\n";
											}
											else if (arrayCodigo[i, 1] == "PR10"
												&& arrayCodigo[i, 2].Substring(0, 2) != "ID")
											{
												estaticoReglaCXFA = true;
												numeroErrores++;
												errores++;
												consolaError += "Error" + $" {numeroErrores}:" + $"({fila},{4 + 1 + 4 + 1}) "
													+ "Despues de 'KLA' se espera un identificador\n";
											}
											else
											{
												estaticoReglaCXFA = true;
												numeroErrores++;
												errores++;
												consolaError += "Error" + $" {numeroErrores}:" + $"({fila},{4 + 1}) "
													+ "Despues del 'CXFA' se espera la palabra 'KLA' y un identificador\n";
											}
											verificado = true;
										}
										else if (!estaticoReglaCXFA && longitud > 1 && arrayCodigo[i, 0] == "PR26")
										{
											if (arrayCodigo[i, 1].Substring(0, 2) == "ID")
											{
												estaticoReglaCXFA = true;
												numeroErrores++;
												errores++;
												consolaError += "Error" + $" {numeroErrores}:" + $"({fila},{4 + 1}) " + "Debe seguir la palabra 'KLA' despues de la palabra 'CXFA'\n";
											}
											else if (arrayCodigo[i, 1] != "PR10"
												&& arrayCodigo[i, 2].Substring(0, 2) == "ID")
											{
												estaticoReglaCXFA = true;
												numeroErrores++;
												errores++;
												consolaError += "Error" + $" {numeroErrores}:" + $"({i},{4 + 1 + 4 + 1}) "
													+ "Despues de 'KLA' se espera un identificador\n";
											}
											else
											{
												estaticoReglaCXFA = true;
												numeroErrores++;
												errores++;
												consolaError += "Error" + $" {numeroErrores}:" + $"({i},{4 + 1}) "
													+ "Despues del 'CXFA' se espera la palabra 'KLA' y un identificador\n";
											}
										}
										//CXFA KLA _ID
										if (!verificado && expRestantes >= 3 && arrayCodigo[i, j] == "PR26")
										{
											int caracFila = CountCharactersBeforeWord(arrayCodigo, i, "PR26");
											if (arrayCodigo[i, j + 1] == "PR10"
												&& arrayCodigo[i, j + 2].Substring(0, 2) == "ID")
											{

											}
											else if (arrayCodigo[i, j + 1] != "PR10"
											   && arrayCodigo[i, j + 2].Substring(0, 2) == "ID")
											{

												numeroErrores++;
												errores++;
												consolaError += "Error" + $" {numeroErrores}:" + $"({fila},{caracFila + 5}) " + "Debe seguir la palabra 'KLA' despues de la palabra 'CXFA'\n";
											}
											else if (arrayCodigo[i, j + 1] == "PR10"
												&& arrayCodigo[i, j + 2].Substring(0, 2) != "ID")
											{

												numeroErrores++;
												errores++;
												consolaError += "Error" + $" {numeroErrores}:" + $"({fila},{caracFila + 9}) "
													+ "Despues de 'KLA' se espera un identificador\n";
											}
											else
											{

												numeroErrores++;
												errores++;
												consolaError += "Error" + $" {numeroErrores}:" + $"({fila},{caracFila + 5}) "
													+ "Despues del 'CXFA' se espera la palabra 'KLA' y un identificador\n";
											}
											verificado = true;
										}
										else
										if (!verificado && expRestantes >= 2 && arrayCodigo[i, j] == "PR26")
										{
											int caracFila = CountCharactersBeforeWord(arrayCodigo, i, "PR26");
											if (arrayCodigo[i, j + 1] != "PR10"
											   && arrayCodigo[i, j + 2].Substring(0, 2) == "ID")
											{
												numeroErrores++;
												errores++;
												consolaError += "Error" + $" {numeroErrores}:" + $"({fila},{caracFila + 5}) " + "Debe seguir la palabra 'KLA' despues de la palabra 'CXFA'\n";
											}
										}
										//x  pr21 cex( BLN  ó pr21 BLN cex)
										if (expRestantes >= 2 && arrayCodigo[i, j] == "PR21")
										{
											if (arrayCodigo[i, j + 1] == "CEX(" &&
												arrayCodigo[i, j + 2] == "BLN")
											{
												int caracFila = CountCharactersBeforeWord(arrayCodigo, i, "PR21");
												numeroErrores++;
												errores++;
												consolaError += "Error"
													+ $" {numeroErrores}:"
													+ $"({fila},{caracFila + 10}) "
													+ "Se espera que cierres los parentesis con )"
													+ "\n";
											}
											else if (expRestantes >= 3 && arrayCodigo[i, j + 1] == "CEX(" &&
											   arrayCodigo[i, j + 2] == "BLN" &&
											   arrayCodigo[i, j + 3] != "CEX)")
											{
												int caracFila = CountCharactersBeforeWord(arrayCodigo, i, "PR21");
												numeroErrores++;
												errores++;
												consolaError += "Error"
													+ $" {numeroErrores}:"
													+ $"({fila},{caracFila + 10}) "
													+ "Se espera que cierres los parentesis con )"
													+ "\n";
											}
											else if (arrayCodigo[i, j + 1] == "BLN"
												&& arrayCodigo[i, j + 2] == "CEX)")
											{
												int caracFila = CountCharactersBeforeWord(arrayCodigo, i, "PR21");
												numeroErrores++;
												errores++;
												consolaError += "Error"
													+ $" {numeroErrores}:"
													+ $"({fila},{caracFila + 5}) "
													+ "Se espera que cierres los parentesis con ("
													+ "\n";
											}
										}
										//| ent _x = 1 . || 
										//x INIS CEX. DECENT ASIG ASIGENT FIIN
										if (expRestantes >= 5
											&& arrayCodigo[i, j] == "INIS"
											&& arrayCodigo[i, j + 1] != "DECENT"
											&& arrayCodigo[i, j + 2] == "DECENT"
											&& arrayCodigo[i, j + 3] == "ASIG"
											&& arrayCodigo[i, j + 4] == "ASIGENT"
											&& arrayCodigo[i, j + 5] == "FIIN")
										{
											int caracFila = CountCharactersBeforeWord(arrayCodigo, i, "INIS");
											numeroErrores++;
											errores++;
											consolaError += "Error"
												+ $" {numeroErrores}:"
												+ $"({fila},{caracFila + 1}) "
												+ "Se esperaba una palabra reservada"
												+ "\n";
										}
										//| ent _x . = 1 || 
										//x INIS DECENT CEX. ASIG CONE FIIN 
										if (expRestantes >= 5
											&& arrayCodigo[i, j] == "INIS"
											&& arrayCodigo[i, j + 1] == "DECENT"
											&& arrayCodigo[i, j + 2] != "ASIG"
											&& arrayCodigo[i, j + 3] == "ASIG"
											&& (arrayCodigo[i, j + 4] == "ASIGENT" || arrayCodigo[i, j + 4] == "CONE")
											&& arrayCodigo[i, j + 5] == "FIIN")
										{
											int caracFila = CountCharactersBeforeWord(arrayCodigo, i, "DECENT");
											numeroErrores++;
											errores++;
											consolaError += "Error"
												+ $" {numeroErrores}:"
												+ $"({fila},{caracFila + 1}) "
												+ "Se esperaba un identificador"
												+ "\n";
										}
										//| ent _x = . 1 ||
										//x   INIS PR04 ID01 ASIG CEX. CONE FIIN 
										if (expRestantes >= 6
											&& arrayCodigo[i, j] == "INIS"
											&& arrayCodigo[i, j + 1] == "PR04"
											&& arrayCodigo[i, j + 2].Substring(0, 2) == "ID"
											&& arrayCodigo[i, j + 3] == "ASIG"
											&& !(arrayCodigo[i, j + 4] == "ASIGENT" || arrayCodigo[i, j + 4] == "CONE")
											&& (arrayCodigo[i, j + 5] == "ASIGENT" || arrayCodigo[i, j + 5] == "CONE")
											&& arrayCodigo[i, j + 6] == "FIIN")
										{
											int caracFila = CountCharactersBeforeWord(arrayCodigo, i, "ASIG");
											numeroErrores++;
											errores++;
											consolaError += "Error"
												+ $" {numeroErrores}:"
												+ $"({fila},{caracFila + 1}) "
												+ "Ese caracter no es valido en este contexto"
												+ "\n";
										}
										//| ent _x = 1 . || 
										//x INIS DECENT ASIG ASIGENT CEX. FIIN
										if (expRestantes >= 4
											&& arrayCodigo[i, j] == "INIS"
											&& arrayCodigo[i, j + 1] == "DECENT"
											&& arrayCodigo[i, j + 2] == "ASIG"
											&& arrayCodigo[i, j + 3] == "ASIGENT"
											&& arrayCodigo[i, j + 4] != "FIIN")
										{
											int caracFila = CountCharactersBeforeWord(arrayCodigo, i, "ASIGENT");
											numeroErrores++;
											errores++;
											consolaError += "Error"
												+ $" {numeroErrores}:"
												+ $"({fila},{caracFila + 1}) "
												+ "Se espera un identificador o un valor"
												+ "\n";
										}
										//| ent _x _y ||
										//x INIS PR04 ID01 ID02 FIIN
										if (expRestantes >= 4
											&& arrayCodigo[i, j] == "INIS"
											&& arrayCodigo[i, j + 1] == "PR04"
											&& arrayCodigo[i, j + 2].Substring(0, 2) == "ID"
											&& arrayCodigo[i, j + 3].Substring(0, 2) == "ID"
											&& arrayCodigo[i, j + 4] == "FIIN")
										{
											int caracFila = CountCharactersBeforeWord(arrayCodigo, i, "ID");
											numeroErrores++;
											errores++;
											consolaError += "Error"
												+ $" {numeroErrores}:"
												+ $"({fila},{caracFila + 2 + 1}) "
												+ "Se esperaba un '||' o un ',' "
												+ "\n";
										}
										//| = 1 ||
										//x INIS ASIG CONE FIIN
										if (expRestantes >= 3
											&& arrayCodigo[i, j] == "INIS"
											&& arrayCodigo[i, j + 1] == "ASIG"
									&& EsIgual(arrayCodigo[i, j + 2], "ASIGENT", "CONE", "ASIGCONE")
											&& arrayCodigo[i, j + 3] == "FIIN")
										{
											int caracFila = CountCharactersBeforeWord(arrayCodigo, i, "INIS");
											numeroErrores++;
											errores++;
											consolaError += "Error"
												+ $" {numeroErrores}:"
												+ $"({fila},{caracFila + 1}) "
												+ "El caracter '=' no es valido en este contexto "
												+ "\n";
										}
										//( _x rsq rsq _y )
										//x	CEX( ID01 OPR1 OPR1 ID02 CEX)  
										if (expRestantes >= 5
											&& arrayCodigo[i, j] == "CEX("
											&& arrayCodigo[i, j + 1].Substring(0, 2) == "ID"
									&& EsIgual(arrayCodigo[i, j + 2], "OPR1", "OPR2", "OPR3", "OPR4", "OPR5", "OPR6", "OPSM", "OPRS", "OPML", "OPDV", "OPEX", "OPLA", "OPLO", "OPLN") //ES Operador
									&& EsIgual(arrayCodigo[i, j + 3], "OPR1", "OPR2", "OPR3", "OPR4", "OPR5", "OPR6", "OPSM", "OPRS", "OPML", "OPDV", "OPEX", "OPLA", "OPLO", "OPLN") //ES Operador
											&& arrayCodigo[i, j + 4].Substring(0, 2) == "ID"
											&& arrayCodigo[i, j + 5] == "CEX)")
										{
											int caracFila = CountCharactersBeforeWord(arrayCodigo, i, "ID");
											numeroErrores++;
											errores++;
											consolaError += "Error"
												+ $" {numeroErrores}:"
												+ $"({fila},{caracFila + 2 + 1}) "
												+ "Esa expresion no es valida"
												+ "\n";
										}
										//| ent _x = 1 | 
										//x INIS DECENT ASIG ASIGENT INIS
										if (expRestantes >= 4
											&& arrayCodigo[i, j] == "INIS"
											&& arrayCodigo[i, j + 1] == "DECENT"
											&& arrayCodigo[i, j + 2] == "ASIG"
											&& arrayCodigo[i, j + 3] == "ASIGENT"
											&& arrayCodigo[i, j + 4] == "INIS")
										{
											int caracFila = CountCharactersBeforeWord(arrayCodigo, i, "ASIGENT");
											numeroErrores++;
											errores++;
											consolaError += "Error"
												+ $" {numeroErrores}:"
												+ $"({fila},{caracFila + 1}) "
												+ "Se esperaba un '||'"
												+ "\n";
										}
										//| ent _x |
										//x INIS DECENT INIS
										if (expRestantes >= 2
											&& arrayCodigo[i, j] == "INIS"
											&& arrayCodigo[i, j + 1] == "DECENT"
											&& arrayCodigo[i, j + 2] == "INIS")
										{
											int caracFila = CountCharactersBeforeWord(arrayCodigo, i, "DECENT");
											numeroErrores++;
											errores++;
											consolaError += "Error"
												+ $" {numeroErrores}:"
												+ $"({fila},{caracFila + 1}) "
												+ "Se esperaba un '||'"
												+ "\n";
										}
										//| _x = 1 |
										//x INIS ASIGENT INIS
										if (expRestantes >= 2
											&& arrayCodigo[i, j] == "INIS"
											&& arrayCodigo[i, j + 1] == "ASIGENT"
											&& arrayCodigo[i, j + 2] == "INIS")
										{
											int caracFila = CountCharactersBeforeWord(arrayCodigo, i, "ASIGENT");
											numeroErrores++;
											errores++;
											consolaError += "Error"
												+ $" {numeroErrores}:"
												+ $"({fila},{caracFila + 1}) "
												+ "Se esperaba un '||'"
												+ "\n";
										}
									}
								}
							}
							if (errores == 0)
							{
								catEstado += "Fila " + fila + ": CORRECTA\n";
							}
							else
							{
								catEstado += "Fila " + fila + ": Con error/es\n";
							}
						}
						rchAnalisisSintactico.Text = catEstado;
						if (numeroErrores == 0)
						{
							rchConsola3.Text = "Exito";
							EstadoSintactico = true;
						}
						else
						{
							rchConsola3.Text = consolaError;
							EstadoSintactico = false;
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
			else
			{
				MessageBox.Show("Debes corregir los errores primero");
			}
		}
		public static int CountCharactersBeforeWord(string[,] matrix, int rowIndex, string word)
		{
			string[] rowArray = new string[matrix.GetLength(1)];
			for (int j = 0; j < matrix.GetLength(1); j++)
			{
				rowArray[j] = matrix[rowIndex, j];
			}
			string rowString = string.Join(" ", rowArray);
			int index = rowString.IndexOf(word);
			if (index == -1)
			{
				return rowString.Length;
			}
			else
			{
				return index;
			}
		}
		public static int[] ConvertirArregloStringAInt(string[] arregloString)
		{
			int[] arregloInt = new int[arregloString.Length];
			for (int i = 0; i < arregloString.Length; i++)
			{
				int.TryParse(arregloString[i], out arregloInt[i]);
			}
			return arregloInt;
		}
		public static int CountUntilWord(string inputString, string word)
		{
			int count = 0;
			int index = inputString.IndexOf(word);
			if (index != -1)
			{
				count = index;
			}
			else
			{
				count = inputString.Length;
			}
			return count;
		}

		public string[] ObtenerFilaDeMatriz(string[,] matriz, int fila)
		{
			int columnas = matriz.GetLength(1);
			string[] filaArreglo = new string[columnas];
			for (int i = 0; i < columnas; i++)
			{
				filaArreglo[i] = matriz[fila, i];
			}
			return filaArreglo;
		}

		private bool BLOQUEA(string text)
		{
			if (text == "SPR03" || text == "SPR04" || text == "SPR06" || text == "SPR07" || text == "SPR08" || text == "SPR09" || text == "SPR11" || text == "SPR12" || text == "SPR13" || text == "SPR15" || text == "SPR16" || text == "SPR17" || text == "SPR18" || text == "SPR21" || text == "SPR22" || text == "SPR23" || text == "SPR24" || text == "DAR" || text == "ROM" || text == "ASIGID")
			{
				return true;
			}
			return false;
		}
		private bool EsIgual(string cad1, params string[] conjunto)
		{
			foreach (string cad in conjunto)
			{
				if (cad == cad1)
					return true;
			}
			return false;
		}
		private void Sintactico(ref string[] codigo, ref bool Finalizo, ref int sinCambios, ref int compVuelta, ref int cambios)
		{

			//int cambios = 0;
			string MensajeError = "";
			string mirarString = "";
			string temp = "";
			string cat = "";//recordar colocar
							//int compVuelta = 0;
			string UltDec = "";//Se utiliza despues
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
					if (s >= 2 && codigo[s - 2] == "INIS" && codigo[s - 1] == "DECENT" && codigo[s] == "FIIN")
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
					if (s >= 2 && posx > 0 && codigo[s - 2] == "ASIGENT" && codigo[s - 1] == "CEX," && codigo[s].Substring(0, 2) == "ID" && !(SiOpAritmetico(codigo[s + 1]) || codigo[s + 1] == "ASIG"))
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
					if (s >= 2 && posx > 0 && codigo[s - 2] == "DECENT" && codigo[s - 1] == "CEX," && codigo[s].Substring(0, 2) == "ID" && !(SiOpAritmetico(codigo[s + 1]) || codigo[s + 1] == "ASIG"))
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
					if (s >= 2 && posx > 0 && codigo[s - 2] == "DECKAR" && codigo[s - 1] == "CEX," && codigo[s].Substring(0, 2) == "ID" && !(SiOpAritmetico(codigo[s + 1]) || codigo[s + 1] == "ASIG"))
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
					if (s >= 2 && posx > 0 && codigo[s - 2] == "DECLOG" && codigo[s - 1] == "CEX," && codigo[s].Substring(0, 2) == "ID" && !(SiOpAritmetico(codigo[s + 1]) || codigo[s + 1] == "ASIG"))
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
					if (s >= 2 && posx > 0 && codigo[s - 2] == "DECSXN" && codigo[s - 1] == "CEX," && codigo[s].Substring(0, 2) == "ID" && !(SiOpAritmetico(codigo[s + 1]) || codigo[s + 1] == "ASIG"))
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
						&& EsIgual(codigo[s], "CDNA", "ASIGCDNA", "CRTR", "CONE", "CONR", "VARMAT")) //VAR1
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
						&& codigo[s - 1].Substring(0, 2) == "ID"
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
					if (posx >= 5 && codigo[s] == "INIS" && codigo[s + 1] == "PR24" && codigo[s + 2] == "CEX(" && (codigo[s + 3] == "CDNA" || codigo[s + 3] == "ASIGCDNA" || codigo[s + 3].Substring(0, 2) == "ID" || codigo[s + 3] == "SDCM") && codigo[s + 4] == "CEX)" && codigo[s + 5] == "FIIN")
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
						&& (codigo[s + 5] == "P16R3" || codigo[s + 5] == "ACUM")
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
						&& codigo[s + 2].Substring(0, 2) == "ID"
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
					if (s >= 1
						&& codigo[s - 1] == "KAZ"
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
						}
						else if (codigo[s - 3] == "PR07")
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
						}
						else if (codigo[s] == "ACNR")
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
					if (s >= 4
						&& codigo[s - 4] == "INIS"
						&& codigo[s - 3] == "VARMAT"
						&& codigo[s - 2] == "ASIG"
						&& EsIgual(codigo[s - 1], "CONE", "CONR", "CDNA", "CRTR", "BLN", "ASIGCONE", "ASIGCONR", "ASIGCDNA", "ASIGCRTR")
						&& codigo[s] == "FIIN")
					{
						sinCambios = 0;
						codigo[s - 4] = "ASIGVARMAT";
						codigo[s - 3] = "";
						codigo[s - 2] = "";
						codigo[s - 1] = "";
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
						&& (codigo[s - 1] == "MDEC" || codigo[s - 1] == "ASIGPRIM")
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
					if (s >= 3
						&& codigo[s - 3] == "ASIG"
						&& codigo[s - 2] == "PR14"
						&& codigo[s - 1] == "MTDX"
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
					if (s >= 3
						&& codigo[s - 3] == "INIS"
						&& codigo[s - 2].Substring(0, 2) == "ID"
						&& EsIgual(codigo[s - 1], "OPSM", "OPRS")
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
					if (s >= 5 && !EsIgual(codigo[s - 5], "PR04", "PR07", "PR11", "PR12", "PR13", "PR15", "PR18", "PR23", "PR27") && codigo[s - 4].Substring(0, 2) == "ID" && codigo[s - 3] == "ASIG" && (codigo[s - 2].Substring(0, 2) == "ID" || codigo[s - 2] == "ASIGPR") && codigo[s - 1] == "OPSM" && EsIgual(codigo[s], "ASIGCONE", "CONE", "ASIGCONR", "CONR", "ASIGCRTR", "CRTR", "ASIGCDNA", "CDNA", "ASIGLOG", "LOG")) //DEC ASIGDEC ID
					{
						sinCambios = 0;
						codigo[s - 2] = "ASIGPR";
						codigo[s - 1] = "";
						codigo[s] = "";
						break;
					}
					//ASIGID
					if (s >= 3 && posx > 0 && !EsIgual(codigo[s - 3], "PR04", "PR07", "PR11", "PR12", "PR13", "PR15", "PR18", "PR23", "PR27") && (codigo[s - 2] == "ASIGPRIM" || codigo[s - 2].Substring(0, 2) == "ID") && codigo[s - 1] == "ASIG" && (codigo[s].Substring(0, 2) == "ID" || codigo[s] == "ASIGPR") && !SiOpAritmetico(codigo[s + 1]))
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
						&& codigo[s - 1].Substring(0, 2) == "ID"
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
						&& EsIgual(codigo[s], "SPR03", "SPR06", "SPR09", "SPR12", "SPR16", "SPR17", "SPR21", "SPR22", "SPR24", "SPR04", "DECENT", "ASIGENT", "SPR18", "DECREA", "ASIGREA", "SPR11", "SPR13", "SPR15", "SPR23", "SIMT", "SIMV", "ASIGID", "BRMT", "DECENT", "SIMT"))//BODY
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
						&& EsIgual(codigo[s], "SPR04", "DECENT", "ASIGENT", "SPR18", "DECREA", "ASIGREA", "SPR11", "SPR13", "SPR15", "SPR23", "SIMT", "SIMV", "ASIGID", "ASIGVARMAT", "SOBJ"))
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
						&& EsIgual(codigo[s], "SPR04", "DECENT", "ASIGENT", "SPR18", "DECREA", "ASIGREA", "SPR11", "SPR13", "SPR15", "SPR23", "SIMT", "SIMV", "ASIGID", "ASIGVARMAT", "SMAT", "SOBJ"))
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
						&& EsIgual(codigo[s], "SPR04", "DECENT", "ASIGENT", "SPR18", "DECREA", "ASIGREA", "SPR11", "SPR13", "SPR15", "SPR23", "SIMT", "SIMV", "ASIGID", "ASIGVARMAT", "SMAT", "SOBJ"))
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
						&& EsIgual(codigo[s], "SPR04", "DECENT", "ASIGENT", "SPR18", "DECREA", "ASIGREA", "SPR11", "SPR13", "SPR15", "SPR23", "SIMT", "SIMV", "ASIGID", "ASIGVARMAT", "SMAT", "SOBJ")) //$BKLA
					{
						sinCambios = 0;
						codigo[s - 1] = "BKLA";
						codigo[s] = "";
						break;
					}
					//BODY
					if (s >= 1 && codigo[s - 1] == "BODY"
						&& EsIgual(codigo[s], "SPR03", "SPR06", "SPR09", "SPR12", "SPR16", "SPR17", "SPR21", "SPR22", "SPR24", "SPR04", "DECENT", "ASIGENT", "SPR18", "DECREA", "ASIGREA", "SPR11", "SPR13", "SPR15", "SPR23", "SIMT", "SIMV", "ASIGID", "ASIGVARMAT", "SMAT", "BMET", "SOBJ")) //$BODY
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
				sinCambios++;
			}
		}
		private void SintacticoSinClase(ref string[] codigo, ref int compVuelta, ref int cambios, ref int sinCambios, ref bool Finalizo)
		{

			//int cambios = 0;
			string MensajeError = "";
			string mirarString = "";
			string temp = "";
			string cat = "";
			//compVuelta = 0;
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

				//Comprobador de codigo *ignorar*
				if (compVuelta == 0 || cambios == 0)
				{

				}
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
					if (s >= 2 && codigo[s - 2] == "INIS" && codigo[s - 1] == "DECENT" && codigo[s] == "FIIN")
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
					if (s >= 2 && posx > 0 && codigo[s - 2] == "ASIGENT" && codigo[s - 1] == "CEX," && codigo[s].Substring(0, 2) == "ID" && !(SiOpAritmetico(codigo[s + 1]) || codigo[s + 1] == "ASIG"))
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
					if (s >= 2 && posx > 0 && codigo[s - 2] == "DECENT" && codigo[s - 1] == "CEX," && codigo[s].Substring(0, 2) == "ID" && !(SiOpAritmetico(codigo[s + 1]) || codigo[s + 1] == "ASIG"))
					{
						sinCambios = 0;
						codigo[s - 2] = "DECENT";
						codigo[s - 1] = "";
						codigo[s] = "";
						break;
					}

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
					if (s >= 2 && posx > 0 && codigo[s - 2] == "DECKAR" && codigo[s - 1] == "CEX," && codigo[s].Substring(0, 2) == "ID" && !(SiOpAritmetico(codigo[s + 1]) || codigo[s + 1] == "ASIG"))
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
					if (s >= 2 && posx > 0 && codigo[s - 2] == "DECLOG" && codigo[s - 1] == "CEX," && codigo[s].Substring(0, 2) == "ID" && !(SiOpAritmetico(codigo[s + 1]) || codigo[s + 1] == "ASIG"))
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
					 || ((codigo[s - 2] == "ASIGCONE" || codigo[s - 2] == "CONE" || codigo[s - 2] == "ASIGCONR" || codigo[s - 2] == "CONR" || codigo[s - 2].Substring(0, 2) == "ID") && (codigo[s] == "ASIGCONE" || codigo[s] == "CONE" || codigo[s] == "ASIGCONR" || codigo[s] == "CONR" || codigo[s].Substring(0, 2) == "ID"))
					 || ((codigo[s - 2] == "ASIGCRTR" || codigo[s - 2] == "CRTR" || codigo[s - 2].Substring(0, 2) == "ID") && (codigo[s] == "ASIGCRTR" || codigo[s] == "CRTR" || codigo[s].Substring(0, 2) == "ID"))
					 || ((codigo[s - 2] == "ASIGBLN" || codigo[s - 2] == "BLN" || codigo[s - 2].Substring(0, 2) == "ID") && (codigo[s] == "ASIGBLN" || codigo[s] == "BLN" || codigo[s].Substring(0, 2) == "ID"))
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
					if (s >= 2 && posx > 0 && codigo[s - 2] == "DECSXN" && codigo[s - 1] == "CEX," && codigo[s].Substring(0, 2) == "ID" && !(SiOpAritmetico(codigo[s + 1]) || codigo[s + 1] == "ASIG"))
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
						&& EsIgual(codigo[s], "CDNA", "ASIGCDNA", "CRTR", "CONE", "CONR", "VARMAT")) //VAR1
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
						&& codigo[s - 1].Substring(0, 2) == "ID"
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
					if (posx >= 5 && codigo[s] == "INIS" && codigo[s + 1] == "PR24" && codigo[s + 2] == "CEX(" && (codigo[s + 3] == "CDNA" || codigo[s + 3] == "ASIGCDNA" || codigo[s + 3].Substring(0, 2) == "ID" || codigo[s + 3] == "SDCM") && codigo[s + 4] == "CEX)" && codigo[s + 5] == "FIIN")
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
						&& (codigo[s + 5] == "P16R3" || codigo[s + 5] == "ACUM")
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
						&& codigo[s + 2].Substring(0, 2) == "ID"
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
					if (s >= 1
						&& codigo[s - 1] == "KAZ"
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
						}
						else if (codigo[s - 3] == "PR07")
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
						}
						else if (codigo[s] == "ACNR")
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
					if (s >= 4
						&& codigo[s - 4] == "INIS"
						&& codigo[s - 3] == "VARMAT"
						&& codigo[s - 2] == "ASIG"
						&& EsIgual(codigo[s - 1], "CONE", "CONR", "CDNA", "CRTR", "BLN", "ASIGCONE", "ASIGCONR", "ASIGCDNA", "ASIGCRTR")
						&& codigo[s] == "FIIN")
					{
						sinCambios = 0;
						codigo[s - 4] = "ASIGVARMAT";
						codigo[s - 3] = "";
						codigo[s - 2] = "";
						codigo[s - 1] = "";
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
						&& (codigo[s - 1] == "MDEC" || codigo[s - 1] == "ASIGPRIM")
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
					if (s >= 3
						&& codigo[s - 3] == "ASIG"
						&& codigo[s - 2] == "PR14"
						&& codigo[s - 1] == "MTDX"
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
					if (s >= 3
						&& codigo[s - 3] == "INIS"
						&& codigo[s - 2].Substring(0, 2) == "ID"
						&& EsIgual(codigo[s - 1], "OPSM", "OPRS")
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
					if (s >= 5 && !EsIgual(codigo[s - 5], "PR04", "PR07", "PR11", "PR12", "PR13", "PR15", "PR18", "PR23", "PR27") && codigo[s - 4].Substring(0, 2) == "ID" && codigo[s - 3] == "ASIG" && (codigo[s - 2].Substring(0, 2) == "ID" || codigo[s - 2] == "ASIGPR") && codigo[s - 1] == "OPSM" && EsIgual(codigo[s], "ASIGCONE", "CONE", "ASIGCONR", "CONR", "ASIGCRTR", "CRTR", "ASIGCDNA", "CDNA", "ASIGLOG", "LOG")) //DEC ASIGDEC ID
					{
						sinCambios = 0;
						codigo[s - 2] = "ASIGPR";
						codigo[s - 1] = "";
						codigo[s] = "";
						break;
					}
					//ASIGID
					if (s >= 3 && posx > 0 && !EsIgual(codigo[s - 3], "PR04", "PR07", "PR11", "PR12", "PR13", "PR15", "PR18", "PR23", "PR27") && (codigo[s - 2] == "ASIGPRIM" || codigo[s - 2].Substring(0, 2) == "ID") && codigo[s - 1] == "ASIG" && (codigo[s].Substring(0, 2) == "ID" || codigo[s] == "ASIGPR") && !SiOpAritmetico(codigo[s + 1]))
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
						&& codigo[s - 1].Substring(0, 2) == "ID"
						&& codigo[s] == "CEX(")
					{
						sinCambios = 0;
						codigo[s - 1] = "MTDX";
						codigo[s] = "";
						break;
					}

				}

				if (s == codigo.Length - 1 && sinCambios == 2)
				{
					Finalizo = true;
				}
				sinCambios++;
			}
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
			//Semantico
			if (EstadoSintactico)
			{
				EstaForma.Cursor = Cursors.AppStarting;
				rchConsola4.Text = "";
				rchSemantico.Text = "";
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
						do//Analizar si todos los inicios y cierras estan completos
						{
							if (lexicos[y] == "INIS")
							{
								numI++;

							}
							if (lexicos[y] == "FIIN")
							{
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
										if (s >= 2 && codigo[s - 2] == "INIS" && codigo[s - 1] == "DECENT" && codigo[s] == "FIIN")
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
										if (s >= 2 && posx > 0 && codigo[s - 2] == "ASIGENT" && codigo[s - 1] == "CEX," && codigo[s].Substring(0, 2) == "ID" && !(SiOpAritmetico(codigo[s + 1]) || codigo[s + 1] == "ASIG"))
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
										if (s >= 2 && posx > 0 && codigo[s - 2] == "DECENT" && codigo[s - 1] == "CEX," && codigo[s].Substring(0, 2) == "ID" && !(SiOpAritmetico(codigo[s + 1]) || codigo[s + 1] == "ASIG"))
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
										if (s >= 2 && posx > 0 && codigo[s - 2] == "DECKAR" && codigo[s - 1] == "CEX," && codigo[s].Substring(0, 2) == "ID" && !(SiOpAritmetico(codigo[s + 1]) || codigo[s + 1] == "ASIG"))
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
										if (s >= 2 && posx > 0 && codigo[s - 2] == "DECLOG" && codigo[s - 1] == "CEX," && codigo[s].Substring(0, 2) == "ID" && !(SiOpAritmetico(codigo[s + 1]) || codigo[s + 1] == "ASIG"))
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
										if (s >= 2 && posx > 0 && codigo[s - 2] == "DECSXN" && codigo[s - 1] == "CEX," && codigo[s].Substring(0, 2) == "ID" && !(SiOpAritmetico(codigo[s + 1]) || codigo[s + 1] == "ASIG"))
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
											&& EsIgual(codigo[s], "CDNA", "ASIGCDNA", "CRTR", "CONE", "CONR", "VARMAT")) //VAR1
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
											&& codigo[s - 1].Substring(0, 2) == "ID"
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
										if (posx >= 5 && codigo[s] == "INIS" && codigo[s + 1] == "PR24" && codigo[s + 2] == "CEX(" && (codigo[s + 3] == "CDNA" || codigo[s + 3] == "ASIGCDNA" || codigo[s + 3].Substring(0, 2) == "ID" || codigo[s + 3] == "SDCM") && codigo[s + 4] == "CEX)" && codigo[s + 5] == "FIIN")
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
											&& (codigo[s + 5] == "P16R3" || codigo[s + 5] == "ACUM")
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
											&& codigo[s + 2].Substring(0, 2) == "ID"
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
										if (s >= 1
											&& codigo[s - 1] == "KAZ"
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
											}
											else if (codigo[s - 3] == "PR07")
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
											}
											else if (codigo[s] == "ACNR")
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
										if (s >= 4
											&& codigo[s - 4] == "INIS"
											&& codigo[s - 3] == "VARMAT"
											&& codigo[s - 2] == "ASIG"
											&& EsIgual(codigo[s - 1], "CONE", "CONR", "CDNA", "CRTR", "BLN", "ASIGCONE", "ASIGCONR", "ASIGCDNA", "ASIGCRTR")
											&& codigo[s] == "FIIN")
										{
											sinCambios = 0;
											codigo[s - 4] = "ASIGVARMAT";
											codigo[s - 3] = "";
											codigo[s - 2] = "";
											codigo[s - 1] = "";
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
											&& (codigo[s - 1] == "MDEC" || codigo[s - 1] == "ASIGPRIM")
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
										if (s >= 3
											&& codigo[s - 3] == "ASIG"
											&& codigo[s - 2] == "PR14"
											&& codigo[s - 1] == "MTDX"
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
										if (s >= 3
											&& codigo[s - 3] == "INIS"
											&& codigo[s - 2].Substring(0, 2) == "ID"
											&& EsIgual(codigo[s - 1], "OPSM", "OPRS")
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
										if (s >= 5 && !EsIgual(codigo[s - 5], "PR04", "PR07", "PR11", "PR12", "PR13", "PR15", "PR18", "PR23", "PR27") && codigo[s - 4].Substring(0, 2) == "ID" && codigo[s - 3] == "ASIG" && (codigo[s - 2].Substring(0, 2) == "ID" || codigo[s - 2] == "ASIGPR") && codigo[s - 1] == "OPSM" && EsIgual(codigo[s], "ASIGCONE", "CONE", "ASIGCONR", "CONR", "ASIGCRTR", "CRTR", "ASIGCDNA", "CDNA", "ASIGLOG", "LOG")) //DEC ASIGDEC ID
										{
											sinCambios = 0;
											codigo[s - 2] = "ASIGPR";
											codigo[s - 1] = "";
											codigo[s] = "";
											break;
										}
										//ASIGID
										if (s >= 3 && posx > 0 && !EsIgual(codigo[s - 3], "PR04", "PR07", "PR11", "PR12", "PR13", "PR15", "PR18", "PR23", "PR27") && (codigo[s - 2] == "ASIGPRIM" || codigo[s - 2].Substring(0, 2) == "ID") && codigo[s - 1] == "ASIG" && (codigo[s].Substring(0, 2) == "ID" || codigo[s] == "ASIGPR") && !SiOpAritmetico(codigo[s + 1]))
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
											&& codigo[s - 1].Substring(0, 2) == "ID"
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
											&& EsIgual(codigo[s], "SPR03", "SPR06", "SPR09", "SPR12", "SPR16", "SPR17", "SPR21", "SPR22", "SPR24", "SPR04", "DECENT", "ASIGENT", "SPR18", "DECREA", "ASIGREA", "SPR11", "SPR13", "SPR15", "SPR23", "SIMT", "SIMV", "ASIGID", "BRMT", "DECENT", "SIMT"))//BODY
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
											&& EsIgual(codigo[s], "SPR04", "DECENT", "ASIGENT", "SPR18", "DECREA", "ASIGREA", "SPR11", "SPR13", "SPR15", "SPR23", "SIMT", "SIMV", "ASIGID", "ASIGVARMAT", "SOBJ"))
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
											&& EsIgual(codigo[s], "SPR04", "DECENT", "ASIGENT", "SPR18", "DECREA", "ASIGREA", "SPR11", "SPR13", "SPR15", "SPR23", "SIMT", "SIMV", "ASIGID", "ASIGVARMAT", "SMAT", "SOBJ"))
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
											&& EsIgual(codigo[s], "SPR04", "DECENT", "ASIGENT", "SPR18", "DECREA", "ASIGREA", "SPR11", "SPR13", "SPR15", "SPR23", "SIMT", "SIMV", "ASIGID", "ASIGVARMAT", "SMAT", "SOBJ"))
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
											&& EsIgual(codigo[s], "SPR04", "DECENT", "ASIGENT", "SPR18", "DECREA", "ASIGREA", "SPR11", "SPR13", "SPR15", "SPR23", "SIMT", "SIMV", "ASIGID", "ASIGVARMAT", "SMAT", "SOBJ")) //$BKLA
										{
											sinCambios = 0;
											codigo[s - 1] = "BKLA";
											codigo[s] = "";
											break;
										}
										//BODY
										if (s >= 1 && codigo[s - 1] == "BODY"
											&& EsIgual(codigo[s], "SPR03", "SPR06", "SPR09", "SPR12", "SPR16", "SPR17", "SPR21", "SPR22", "SPR24", "SPR04", "DECENT", "ASIGENT", "SPR18", "DECREA", "ASIGREA", "SPR11", "SPR13", "SPR15", "SPR23", "SIMT", "SIMV", "ASIGID", "ASIGVARMAT", "SMAT", "BMET", "SOBJ")) //$BODY
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
								errores = 0;
								string v1 = "";
								foreach (MetaDatos dato in _listaMeta)
								{
									v1 += dato.ToString() + "\n";
								}
								string v2 = v1;
								//Verificar que no haya variables repetidas con el mismo nombre
								string errorReport = "";
								var duplicados = _listaMeta
												.Where(m => m.Token == "IDEN")
												.GroupBy(m => m.Variable)
												.Where(g => g.Count() > 1)
												.Select(g => g.OrderBy(m => m.ID).First());
								if (duplicados.Any())
								{
									foreach (var item in duplicados)
									{
										errorReport += $"Error: Ya existe un identificador con el nombre de {item.Variable} en la linea: ({item.Fila}).\n";
										errores++;
									}
								}
								//Vericar que los identificadores tengan un valor y tipoDato igual
								foreach (var metaDato in _listaMeta)
								{
									if (metaDato.Variable != "NULL" && metaDato.Valor != "NULL")
									{
										if (metaDato.TipoDato == "ENT")
										{
											//if (!int.TryParse(metaDato.Valor, out int valorEntero) )
											//{
											//	errorReport += ("Error: El valor de la variable " + metaDato.Variable + " que esta en la fila (" + metaDato.Fila + ") no es un número entero.\n");
											//	errores++;
											//}
											//else

										}
										if (metaDato.TipoDato == "REA")
										{
											//if ((!double.TryParse(metaDato.Valor, out double valor) && !int.TryParse(metaDato.Valor, out int intValor)) || metaDato.Valor[0] != '_')
											//{
											//	errorReport += ("Error: El valor de la variable " + metaDato.Variable + " que esta en la fila (" + metaDato.Fila + ") no es un número válido.\n");
											//	errores++;
											//}
											//else
										}
										//if (metaDato.TipoDato == "KAR")
										//{
										//	if (metaDato.Valor.Length != 3 || metaDato.Valor[0] != '\'' || metaDato.Valor[2] != '\'')
										//	{
										//		errorReport += ("Error: El valor de la variable " + metaDato.Variable + " que esta en la fila (" + metaDato.Fila + ") no es un caracter.\n");
										//		errores++;
										//	}
										//}
										//if (metaDato.TipoDato == "SXN")
										//{
										//	if (!metaDato.Valor.StartsWith("\"") || !metaDato.Valor.EndsWith("\""))
										//	{
										//		errorReport += ("Error: El valor de la variable " + metaDato.Variable + " que esta en la fila (" + metaDato.Fila + ") debe ser una cadena.\n");
										//		errores++;
										//	}
										//}
										//if (metaDato.TipoDato == "LOG")
										//{
										//	string valorBooleano = metaDato.Valor.ToUpper();
										//	// Verificar que el valor sea true o false
										//	if (valorBooleano != "VERA" && valorBooleano != "FALSA")
										//	{
										//		errorReport += ("Error: El valor de la variable " + metaDato.Variable + " que esta en la fila (" + metaDato.Fila + ") debe ser VERA o FALSA.\n");
										//		errores++;
										//	}
										//}
									}
								}


								//Verificar que los datos estan llenos
								//Me canse por hoy 15/05 continuo otro dia 
								for (int i = 0; i < matrizCodigo.Length; i++)
								{
									int index = Array.IndexOf(matrizCodigo[i], "=");

									if (index >= 0) // Si el signo "=" se encontró en la fila actual
									{
										int indiceEntero = Array.FindIndex(matrizCodigo[i], s => string.Equals(s, "ENT", StringComparison.OrdinalIgnoreCase));
										int indiceReal = Array.FindIndex(matrizCodigo[i], s => string.Equals(s, "REA", StringComparison.OrdinalIgnoreCase));
										int indiceChar = Array.FindIndex(matrizCodigo[i], s => string.Equals(s, "KAR", StringComparison.OrdinalIgnoreCase));
										int indiceCadena = Array.FindIndex(matrizCodigo[i], s => string.Equals(s, "SXN", StringComparison.OrdinalIgnoreCase));

										if (indiceEntero >= 0 || indiceReal >= 0 || indiceChar >= 0 || indiceCadena >= 0) // Verifica que se haya encontrado un token válido
										{
											// Obtener el nombre de la variable a asignar
											string variable = matrizCodigo[i][index - 1];

											// Subarreglo de la parte después del "="
											string[] parte2 = new string[matrizCodigo[i].Length - index - 1];
											Array.Copy(matrizCodigo[i], index + 1, parte2, 0, parte2.Length);

											// Agregar los valores de la parte después del "=" a una lista
											List<object> valores = new List<object>();
											foreach (string valor in parte2)
											{
												if (int.TryParse(valor, out int entero))
												{
													valores.Add(entero);
												}
												else if (double.TryParse(valor, out double real))
												{
													valores.Add(real);
												}
												else if (valor.Length == 1)
												{
													valores.Add(valor[0]);
												}
												else
												{
													valores.Add(valor);
												}
											}

											// Realizar la suma de los valores enteros en la lista
											int sumaEnteros = 0;
											foreach (object valor in valores)
											{
												if (valor is int)
												{
													sumaEnteros += (int)valor;
												}
											}

										}
									}
								}

								if (errores > 0)
								{
									rchConsola4.Text = "Se encontraron errores.\n" + errorReport;
									EstadoSemantico = false;
								}
								else
								{
									EstadoSemantico = true;
									rchConsola4.Text = "Exito";
								}
								rchAnalisisSemantica.Text = cat;
								rchSemantico.Text = cadenaSintactico;
							}

						}
						else if (numI > numF)
						{
							rchConsola4.Text = "Ocurrio un error con un '|' sin cerrar.";
							EstadoSemantico = false;

						}
						else
						{
							rchConsola4.Text = "Ocurrio un error parece tener un '||' extra. Comprueba que no te falte un '|'";
							EstadoSemantico = false;
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
			else
			{
				MessageBox.Show("Debes corregir los errores primero");
			}
		}
		//CODIGO TEMP
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
				r += line + "\n";
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
			string newLine2 = newLine.Replace(bla.Substring(0, 1), "");
			if (cero == 0)
			{
				string relleno = rchTexto.Text;
				rchTexto.Text = newLine2;
				cero++;
			}
			else
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
			rchSemantico.Text = "";
			rchPosPrefijo.Text = "";
			rchConsola1.Text = "";
			rchConsola2.Text = "";
			rchConsola3.Text = "";
			rchConsola4.Text = "";
			rhcAnalisisLexico.Text = "";
			rchAnalisisSintactico.Text = "";
			rchAnalisisSemantica.Text = "";
			dtgSimbolo.Rows.Clear();//vaciar datagridview
			_listaMeta.Clear();//vaciar lista
			listaDeStringCodIntermed.Clear();
			rchConsola5.Text = "";
			rchPosPrefijo.Text = "";
			dtgCuadruplo.Rows.Clear();
			rchTexto.Focus();
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
			if (chbMensaje.Checked)
			{
				MenSi = true;
			}
			else
			{
				MenSi = false;
			}
		}

		private void btnAjustar_Click(object sender, EventArgs e)
		{
			rchTexto.SelectionFont = new Font("Microsoft Sans Serif", 9, FontStyle.Regular);
			rchTexto.SelectionColor = Color.Black;
		}

		private void btnBiblioteca_Click(object sender, EventArgs e)
		{
			using (var formularioSecundario = new Biblioteca())
			{
				if (formularioSecundario.ShowDialog() == DialogResult.Yes)
				{
					string texto = formularioSecundario.codigo;
					int pos = rchTexto.SelectionStart;
					rchTexto.Text = rchTexto.Text.Insert(pos, texto);
					rchTexto.SelectionStart = pos + texto.Length;

				}
			}
		}

		private void btnPPrueba_Click(object sender, EventArgs e)
		{
			//boton para pruebas
			//int pos = rchTexto.SelectionStart;
			//rchTexto.Text = rchTexto.Text.Insert(pos, "nueva");
			//rchTexto.SelectionStart = pos + "nueva".Length;
			DelimitarPosfijo(new string[] { });
		}
		string[][] matrizTokens;
		private void btnInPrePosFija_Click(object sender, EventArgs e)
		{
			if (EstadoSemantico)
			{
				 matrizTokens = new string[0][];
				for (int i = 0; i < matrizLexico.Length; i++)
				{
					for (int j = 0; j < matrizLexico[i].Length; j++)
					{
						foreach (MetaDatos datos in _listaMeta)
						{
							if(j < matrizLexico[i].Length && j < matrizCodigo[i].Length)
							{
								if (datos.Token == "IDEN" && datos.Variable == matrizCodigo[i][j])//Si su token es iden es un id y si su nombre es igual a uno guardado _x = _x
								{
									matrizLexico[i][j] = datos.TokenUnico;
								}
								if (datos.Variable == "NULL" && datos.Valor == matrizCodigo[i][j])//Si es un numero y tienen el mismo valor se reemplaza su token
								{
									matrizLexico[i][j] = datos.TokenUnico;
								}
							}
						}
					}
					if(matrizLexico[i].Length >= 4
						&& matrizLexico[i][0] == "INIS" 
						&& matrizLexico[i][1].Substring(0,2) == "ID"
						&& matrizLexico[i][2] == "OPSM"
						&& matrizLexico[i][3] == "FIIN"
						&& matrizLexico[i][4] == "CEX)")
					{
						matrizLexico[i] = new string[] { "INIS", "INC", "FIIN", "CEX)","" };
					}
				}

				string codigo = ConvertirArregloACadena1(matrizLexico);
				string[][] temp11;
				temp11 = Reconocimiento(codigo);//Acondicionamiento
				matrizTokens = temp11;
				temp11 = FormatoPosfijo(matrizTokens);//Conversion Posfija
				matrizTokens = temp11;
				temp11 = LimpiarArreglo(matrizTokens);
				matrizTokens = temp11;

				rchPosPrefijo.Text = ConvertirArregloACadena(matrizTokens);
				IsCorrectInPosPrefijo = true;
			}
		}
		static string[][] ReemplazarCombinacion(string[][] matriz)
		{
			string[][] nuevaMatriz = new string[matriz.Length][];
			for (int i = 0; i < matriz.Length; i++)
			{
				nuevaMatriz[i] = new string[matriz[i].Length];
				for (int j = 0; j < matriz[i].Length; j++)
				{
					if (j + 1 < matriz[i].Length && matriz[i][j].Substring(0, 2) == "ID" && matriz[i][j + 1] == "OPSM")
					{
						nuevaMatriz[i][j] = "DEC";
						nuevaMatriz[i][j + 1] = "";
					}
					else
					{
						nuevaMatriz[i][j] = matriz[i][j];
					}
				}
			}
			return nuevaMatriz;
		}


		private bool EsIgualSigno(params string[] coleccion)
		{
			foreach (string signo in coleccion)
			{
				if (signo == '^' + "" || signo == '/' + "" || signo == '*' + "" || signo == '-' + "" || signo == '+' + "" || signo == '=' + "")
				{
					return true;
				}
			}
			return false;
		}
		private bool EsIgual(char cad1, params char[] conjunto)
		{
			foreach (char cad in conjunto)
			{
				if (cad == cad1)
					return true;
			}
			return false;
		}
		private bool EsIgualSigno(params char[] coleccion)
		{
			foreach (char signo in coleccion)
			{
				if (signo == '^' || signo == '/' || signo == '*' || signo == '-' || signo == '+' || signo == '=')
				{
					return true;
				}
			}
			return false;
		}
		
		
		public static string[] LimpiarArreglo(string[] arreglo)
		{
			return arreglo.Select(s => s.TrimEnd()).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
		}

		private void label4_Click(object sender, EventArgs e)
		{

		}
		bool IsCorrectInPosPrefijo = false;
		private void btnCuadruploTriplo_Click(object sender, EventArgs e)
		{
			dtgCuadruplo.Rows.Clear();
			if (IsCorrectInPosPrefijo )
			{
				try
				{
					dtgCuadruplo.Focus();
					//Conversion a cuadruplos
					Cuadruplos(matrizTokens, dtgCuadruplo);
					//Adaptar Datos
					foreach (Cuadruplo cuadro in cuadruplos)
					{
						if (cuadro.Operador.Contains(","))
						{
							string[] div = cuadro.Operador.Split(',');
							cuadro.Operador = div[0];
							cuadro.Destino = int.Parse(div[1]);
						}
					}
				}
				catch (Exception ex)
				{
					rchConsola5.Text = ex.Message;
				}
			}
			else
			{
				rchConsola5.Text = "No se encontro ningun elemento para el codigo intermedio";
			}
		}
		private string[][] Reconocimiento(string texto)
		{
			// Separar las líneas usando saltos de línea
			string[] lines = texto.Split('\n');

			// Crear el arreglo bidimensional
			string[][] result = new string[lines.Length][];

			for (int i = 0; i < lines.Length; i++)
			{
				// Separar los valores de cada línea usando espacios
				string[] values = lines[i].Split(' ');

				// Asignar los valores al arreglo bidimensional
				result[i] = values;
			}
			result = LimpiarArreglo(result);
			int inis = 0;
			int fiin = 0;
			bool ModoSE = false;
			for (int i = 0; i < result.GetLength(0); i++)
			{
				if (result[i][0] != null)
				{
					for (int j = 0; j < result[i].Length; j++)
					{
						if (result[i][j] == "PR21")//SEE
						{
							Array.Resize(ref result[i - 1], result[i - 1].Length + 1);
							// Agregar un nuevo arreglo de strings en la última posición
							result[i - 1][result[i - 1].Length - 1] = "SEE";
							ModoSE = true;
						}
						if (result[i][j] == "INIS" && ModoSE)
						{
							inis++;
						}
						if (result[i][j] == "FIIN" && ModoSE)
						{
							fiin++;
							if (inis >= fiin)
							{
								inis--;
								fiin--;
							}
							if (inis == 0 && fiin == 0 && !Contiene(result[i + 1], "PR01"))//SEF
							{
								ModoSE = false;
								if (result[i][result[i].Length - 1] == "")
								{
									result[i][result[i].Length - 1] = "SEF";
								}
								else
								// Verificar si el tamaño de la matriz necesita ser redimensionado
								if (result[i] == null)
								{
									result[i] = new string[1]; // Crear un nuevo arreglo de tamaño 1
									result[i][0] = "SEF"; // Asignar el valor "SEF" al primer elemento del arreglo
								}
								else
								{
									Array.Resize(ref result[i], result[i].Length + 1); // Redimensionar el arreglo para agregar un nuevo elemento
									result[i][result[i].Length - 1] = "SEF"; // Asignar el valor "SEF" al último elemento del arreglo
								}

							}
						}
					}
				}
			}
			//Asignarle los POSTE Y POSTF para el formato posfijo
			for (int i = 0; i < result.GetLength(0); i++)
			{
				if (Contiene(result[i], "OPLA", "OPLN", "OPLO", "OPR1", "OPR2", "OPR3", "OPR4", "OPR5", "OPR6"))//LA LO LN > >= != == <= <else
				{
					int tamaño = result[i].Length;
					string[] nuevo = new string[tamaño + 2];
					int num = 0;
					for (int j = 0; j < tamaño; j++)
					{
						if (j == 0)
						{
							nuevo[num++] = result[i][j];
							nuevo[num++] = "POSTE";
						}
						else
						if (j == tamaño - 1)
						{
							nuevo[num++] = "POSTF";
							nuevo[num++] = result[i][j];
						}
						else
						{
							nuevo[num++] = result[i][j];
						}
					}
					result[i] = nuevo;
				}
				else
				if (result[i][0] != null)
				{
					for (int j = 0; j < result[i].Length; j++)
					{
						string palabra = result[i][j];
						//IsEnt
						if (result[i].Length >= 2 && palabra == "PR04")
						{
							result[i] = ReemplazarElementos(result[i]);
							string textoprueba1 = ConvertirArregloACadena(result[i]);
							result[i] = DelimitarPosfijo(result[i],true);
							string textoprueba2 = ConvertirArregloACadena(result[i]);
						}
						//IsREA
						if (result[i].Length >= 2 && palabra == "PR18")
						{
							result[i] = ReemplazarElementos(result[i], "REAE", "REAF");
							result[i] = DelimitarPosfijo(result[i]);
						}
						if (j > 2 && result[i][0] == "INIS"
								&& VerificarID(result[i][1])
								&& result[i][2] == "ASIG")
						{
							//result[i] = ReemplazarElementos(result[i], "ASIGE", "ASIGF");
							result[i] = ReemplazarPalabras(result[i], "INIS", "FIIN", "ASIGE", "ASIGF");
							result[i] = AgregarPoste(result[i]);
						}
					}
				}
			}

			return result;
		}
		private string[][] FormatoPosfijo(string[][] matriz)
		{
			for (int i = 0; i < matriz.Length; i++)
			{
				if(matriz[i].Length>1)
				{
					if (Contiene(matriz[i], "POSTE", "POSTF"))
					{
						string[] temporal = ObtenerElementosEntre(matriz[i], "POSTE", "POSTF");
						string temporal1 = ConversionPosfija(temporal);
						try
						{
							// Reemplazar los elementos reacomodados de vuelta en la matriz original
							string[] elementos = temporal1.Split(' ');
							elementos = LimpiarArreglo(elementos);

							//Esta manera borra los POSTE Y POSTF
							//// Buscar el índice de "POSTE" y "POSTF" en la matriz
							int indicePoste = Array.IndexOf(matriz[i], "POSTE");
							int indicePostf = Array.IndexOf(matriz[i], "POSTF");

							//string[] nuevaMatriz = new string[matriz[i].Length + elementos.Length - (indicePostf - indicePoste + 1)];

							//Array.Copy(matriz[i], 0, nuevaMatriz, 0, indicePoste);  // Copiar los elementos antes de "POSTE"
							//Array.Copy(elementos, 0, nuevaMatriz, indicePoste, elementos.Length);  // Copiar los elementos reacomodados
							//Array.Copy(matriz[i], indicePostf + 1, nuevaMatriz, indicePoste + elementos.Length, matriz[i].Length - indicePostf - 1);  // Copiar los elementos después de "POSTF"
							string[] nuevaMatriz = new string[matriz[i].Length + elementos.Length];

							Array.Copy(matriz[i], 0, nuevaMatriz, 0, indicePoste + 1);  // Copiar los elementos antes de "POSTE"
							Array.Copy(elementos, 0, nuevaMatriz, indicePoste + 1, elementos.Length);  // Copiar los elementos reacomodados
							Array.Copy(matriz[i], indicePostf, nuevaMatriz, indicePoste + elementos.Length + 1, matriz[i].Length - indicePostf);  // Copiar los elementos después de "POSTF"

							matriz[i] = nuevaMatriz;
						}
						catch (Exception ex)
						{
							rchPosPrefijo.Text = "ERROR: " + ex.Message;
						}
						
					}
				}
			}
			return matriz;
		}
		public static string[] ReemplazarPalabras(string[] arreglo, string palabra1, string palabra2, string nuevo1, string nuevo2)
		{
			string[] nuevoArreglo = new string[arreglo.Length];

			for (int i = 0; i < arreglo.Length; i++)
			{
				if (arreglo[i] == palabra1)
				{
					nuevoArreglo[i] = nuevo1;
				}
				else if (arreglo[i] == palabra2)
				{
					nuevoArreglo[i] = nuevo2;
				}
				else
				{
					nuevoArreglo[i] = arreglo[i];
				}
			}

			return nuevoArreglo;
		}

		private static string ConvertirArregloACadena(string[][] arreglo)
		{
			List<string> elementos = new List<string>();

			foreach (string[] subarreglo in arreglo)
			{
				string subcadena = string.Join(" ", subarreglo);
				elementos.Add(subcadena);
			}

			string resultado = string.Join(Environment.NewLine, elementos);

			return resultado;
		}
		private static string ConvertirArregloACadena(string[] arreglo)
		{
			string resultado = string.Join(" ", arreglo);
			return resultado;
		}

		List<Cuadruplo> cuadruplos;
		private void Cuadruplos(string[][] matriz, DataGridView dtgTabla)
		{
			cuadruplos = new List<Cuadruplo>();
			string[][] temporal1 = matriz
				.Select(innerArray => innerArray.Where(elemento => !string.IsNullOrEmpty(elemento)).ToArray())
				.Where(innerArray => innerArray.Length > 0)
				.ToArray();
			matriz = temporal1;
			Stack<string> aux1 = new Stack<string>();
			Stack<string> aux2 = new Stack<string>();
			Stack<string> _ModoRecorrido = new Stack<string>();
			int ID = 1;
			int Indice = 1;
			int inis = 0;
			int fiin = 0;
			string Pedido = "NADA";
			bool salir = false;
			int trueFila = 1;
			int al=0, bl=0;
			string PruebaDebug = "ID03";
			bool ApagarPor = false;
			for (int i = 0; i < matriz.Length; i++)
			{
				for (int j = 0; j < matriz[i].Length; j++)
				{
					string y, x;
					string var = matriz[i][j];
					if (matriz[i][j] == PruebaDebug)
					{

					}
					if (al == i && bl == j)
					{

					}
					if (_ModoRecorrido.Count > 0)
					{
						string modo = _ModoRecorrido.Peek();
						if (EsIgual(modo, "SEE") && !EsIgual(var, "PR21","PR24","ENTE","POSTE","POSTF", "INIS","FIIN"))
						{
							if(EsIgual(var,"OPLA","OPLO","OPLN"))
							{
								if(var == "OPLA")
								{
									
									for (int a = 0; a < cuadruplos.Count; a++)
									{
										Cuadruplo c1 = cuadruplos[a];

										if (c1.DatoFuente2 == "TRUE" && c1.Operador == "XXX")
										{
											c1.Operador = "" + (c1.Indice + 2);

										}
									}
									Pedido = "LA";
								}
								if(var == "OPLO")
								{
									for (int a = 0; a < cuadruplos.Count; a++)
									{
										Cuadruplo c1 = cuadruplos[a];

										if (c1.DatoFuente2 == "FALSE" && c1.Operador == "XXX")
										{
											c1.Operador = (c1.Indice+1).ToString();
											salir = true;
											break;
										}

										if (salir)
										{
											salir = false;
											break;
										}
									}

									Pedido = "LO";
								}

							}
							else
							if(EsIgual(var,"OPR1", "OPR2", "OPR3", "OPR4", "OPR5", "OPR6"))
							{
								string x1 = aux1.Pop();
								string y1 = aux1.Pop();
								aux2.Push(x1);
								y = aux2.Pop();
								aux2.Push(y1);
								x = aux2.Pop();
								string temporal;
								Cuadruplo cuadruplo;
								
								//guardar la temporal de relacion
								temporal = AsignarNumero(ID++);
								cuadruplo = new Cuadruplo
								{
									Indice = Indice++,
									DatoObj = temporal,
									DatoFuente1 = x,
									DatoFuente2 = y,
									Operador = var
								};
								cuadruplos.Add(cuadruplo);

								//Si es verdadero
								cuadruplo = new Cuadruplo
								{
									Indice = Indice++,
									DatoObj = temporal,
									DatoFuente1 = "",
									DatoFuente2 = "TRUE",
									Operador = "XXX"
								};
								cuadruplos.Add(cuadruplo);

								//Si es falso
								cuadruplo = new Cuadruplo
								{
									Indice = Indice++,
									DatoObj = temporal,
									DatoFuente1 = "",
									DatoFuente2 = "FALSE",
									Operador = "XXX"
								};
								cuadruplos.Add(cuadruplo);

								ActualizarCuadruplo();
							}
							else
							{
								aux1.Push(var);
							}
						}
						

						if (EsIgual(modo, "POR") && !EsIgual(var, "PR21", "POSTE", "POSTF", "INIS", "FIIN","CEX(","CEX)","ENT", "PR24", "ENTE"))
						{
							if(var=="INC")
							{
								string x1 = aux1.Pop();
								string y1 = aux1.Pop();
								aux2.Push(x1);
								y = aux2.Pop();
								aux2.Push(y1);
								x = aux2.Pop();
								Cuadruplo cuadruplo;
								string ultimoToken = "";
								foreach (MetaDatos item in _listaMeta)
								{
									if (item.Valor == "1" && item.Token == "CONE") 
									{
										ultimoToken = item.TokenUnico;
									}
								}
								if(ultimoToken=="")
								{
									ultimoToken = AsignarNumero(ID++);

								}
								//guardar la temporal de relacion
								//string temporal = AsignarNumero(ID++);
								string temp2 = ObtenerVarTempCuadruplo("PORENT");
								cuadruplo = new Cuadruplo //Fast
								{
									Indice = Indice++,
									DatoObj = temp2,
									DatoFuente1 = temp2,
									DatoFuente2 = ultimoToken,
									Operador = "OPSM" + ", " +
									ObtenerIndiceCuadruplo("OPRPOR"),
									TokenUnico = "INC"
								};
								cuadruplos.Add(cuadruplo);

								ActualizarCuadruplo();
								ApagarPor = true;
							}
							else
							if (var == "DEC")
							{

							}
							else
							if (EsIgual(var, "OPLA", "OPLO", "OPLN"))
							{
								if (var == "OPLA")
								{
									
								}
								if (var == "OPLO")
								{
									
								}

							}
							else
							if (EsIgual(var, "OPR1", "OPR2", "OPR3", "OPR4", "OPR5", "OPR6"))
							{
								string x1 = aux1.Pop();
								string y1 = aux1.Pop();
								aux2.Push(x1);
								y = aux2.Pop();
								aux2.Push(y1);
								x = aux2.Pop();
								string temporal;
								Cuadruplo cuadruplo;
								//guardar la temporal de relacion
								temporal = AsignarNumero(ID++);
								cuadruplo = new Cuadruplo
								{
									Indice = Indice++,
									DatoObj = temporal,
									DatoFuente1 = x,
									DatoFuente2 = y,
									Operador = var,
									TokenUnico = "OPRPOR"
								};
								cuadruplos.Add(cuadruplo);

								//Si es verdadero
								cuadruplo = new Cuadruplo
								{
									Indice = Indice++,
									DatoObj = temporal,
									DatoFuente1 = "",
									DatoFuente2 = "TRUE",
									Operador = "PORT"
								};
								cuadruplos.Add(cuadruplo);

								//Si es falso
								cuadruplo = new Cuadruplo
								{
									Indice = Indice++,
									DatoObj = temporal,
									DatoFuente1 = "",
									DatoFuente2 = "FALSE",
									Operador = "PORF"
								};
								cuadruplos.Add(cuadruplo);

								ActualizarCuadruplo();

							}
							else
							{
								aux1.Push(var);
							}
						}

						if (modo == "POR" && EsIgual(var, "INIS", "FIIN"))
						{
							if (var == "INIS")
							{
								inis++;
							}
							if (var == "FIIN")
							{
								fiin++;
								if (inis >= fiin && ApagarPor)
								{
									inis--;
									fiin--;
								}
								if (inis == 0 && fiin == 0)
								{
									_ModoRecorrido.Pop();
								}
							}
						}

						if (EsIgual(modo,"ALI"))
						{
							if(EsIgual(var,"INIS","FIIN"))
							{
								if(var == "INIS")
								{
									inis++;
								}
								if (var == "FIIN")
								{
									fiin++;
									if (inis >= fiin)
									{
										inis--;
										fiin--;
									}
									if (inis == 0 && fiin == 0 )
									{
										_ModoRecorrido.Pop();
									}
								}
							}
						}else if (EsIgual(modo, "SEE"))
						{
							if (EsIgual(var, "INIS", "FIIN"))
							{
								if (var == "INIS")
								{
									inis++;
								}
								if (var == "FIIN")
								{
									fiin++;
									if (inis >= fiin)
									{
										inis--;
										fiin--;
									}
									if (inis == 0 && fiin == 0 )
									{
										_ModoRecorrido.Pop();
									}
								}
							}
						}

						if (i == 9)
						{ }
						if (EsIgual(modo, "ENTE", "REAE", "ASIG") && !EsIgual(var, "PR04", "PR18", "ENTE", "ENTF", "REAE", "REAF","ASIGE","ASIGF", "POSTE", "POSTF", "PR24", "ENTE"))
						{
							if (EsIgual(var, "OPSM", "OPRS", "OPML", "OPDV", "OPEX", "ASIG"))
							{
								if (aux1.Count >= 3)
								{
									string x1 = aux1.Pop();
									string y1 = aux1.Pop();
									aux2.Push(x1);
									y = aux2.Pop();
									aux2.Push(y1);
									x = aux2.Pop();
								}
								else
								{
									y = aux1.Pop();
									x = aux1.Pop();
								}
								Cuadruplo cuadruplo;

								string lt = "NO";
								if (_ModoRecorrido.Count > 1)
								{
									string aux = _ModoRecorrido.Pop();
									
									if (var == "ASIG" && _ModoRecorrido.Peek() == "POR") // POR ( | ENT _Y = 1 | 
									{
										lt = aux;
										cuadruplo = new Cuadruplo
										{
											Indice = Indice++,
											DatoObj = x,
											DatoFuente1 = y,
											DatoFuente2 = "",
											Operador = "ASIG",
											TokenUnico = "PORENT"
										};
										cuadruplos.Add(cuadruplo);
										ActualizarCuadruplo();
										aux1.Push(x);
									}
									if (Pedido == "LA" && _ModoRecorrido.Peek() == "ALI")
									{
										lt = aux;
										foreach (Cuadruplo c in cuadruplos)
										{
											if (c.DatoFuente2 == "FALSE" && c.Operador == "XXX")
											{
												c.Operador = "SEFIN" + "";
											}
										}
										ActualizarCuadruplo();
									}
									if (Pedido == "LO" && _ModoRecorrido.Peek() == "SEE")
									{
										lt = aux;
										foreach (Cuadruplo c in cuadruplos)
										{
											if (c.DatoFuente2 == "TRUE" && c.Operador == "XXX")
											{
												c.Operador = (c.Indice + 2) + "";
											}
										}
										ActualizarCuadruplo();
									}
									else if (Pedido == "LO" && _ModoRecorrido.Peek() == "ALI")
									{
										lt = aux;
										foreach (Cuadruplo c in cuadruplos)
										{
											if (c.DatoFuente2 == "FALSE" && c.Operador == "XXX")
											{
												c.Operador = "SEFIN" + "";
											}
										}
										ActualizarCuadruplo();
									}
									_ModoRecorrido.Push(aux);
								}
								if (lt == "NO")
								{
									if (var == "ASIG")
									{
										if (y != "") { trueFila = Indice; }
										cuadruplo = new Cuadruplo();
										cuadruplo.Indice = Indice++;
										cuadruplo.DatoObj = x;
										cuadruplo.DatoFuente1 = y;
										cuadruplo.DatoFuente2 = "";
										cuadruplo.Operador = var;
										cuadruplos.Add(cuadruplo);
										ActualizarCuadruplo();
										aux1.Push(x);

									}
									else
									{
										//if(ContarElementosEntre(matriz[i],"POSTE","POSTF") >0){ }
										if(x.Substring(0,2) == "ID")
										{
											//string temporal = AsignarNumero(ID++);
											cuadruplo = new Cuadruplo();
											cuadruplo.Indice = Indice++;
											cuadruplo.DatoObj = x;
											cuadruplo.DatoFuente1 = x;
											cuadruplo.DatoFuente2 = y;
											cuadruplo.Operador = var;
											cuadruplos.Add(cuadruplo);
											ActualizarCuadruplo();
											aux1.Push(x);
										}else
										{
											string temporal = AsignarNumero(ID++);
											cuadruplo = new Cuadruplo();
											cuadruplo.Indice = Indice++;
											cuadruplo.DatoObj = temporal;
											cuadruplo.DatoFuente1 = x;
											cuadruplo.DatoFuente2 = y;
											cuadruplo.Operador = var;
											cuadruplos.Add(cuadruplo);
											ActualizarCuadruplo();
											aux1.Push(temporal);
										}
									}
								}
							}
							else
							{
								aux1.Push(var);
							}
						}
						
					}
					if(EsIgual(var, "PR21", "PR01", "PR24", "PR16", "ENTE","ENTF", "REAE", "REAF", "ASIGE", "ASIGF"))
					{
						//SE 
						if (var == "PR21")
						{
							_ModoRecorrido.Push("SEE");
						}else
						//
						if (var == "PR01")//ALI
						{
							_ModoRecorrido.Push("ALI");
						}else
						//SKR
						if (var == "PR24")
						{
							_ModoRecorrido.Push("SKR");
						}else
						//POR
						if (var == "PR16")
						{
							_ModoRecorrido.Push("POR");
							ApagarPor = false;
						}else
						//ENT
						if (var == "ENTE")
						{
							_ModoRecorrido.Push("ENTE");
						}else
						if (var == "ENTF")
						{
							_ModoRecorrido.Pop();
						}else
						//REA
						if (var == "REAE")
						{
							_ModoRecorrido.Push("REAE");
						}else
						if (var == "REAF")
						{
							_ModoRecorrido.Pop();
						}else
						//ASIG
						if (var == "ASIGE")
						{
							_ModoRecorrido.Push("ASIG");
						}else
						if (var == "ASIGF")
						{
							_ModoRecorrido.Pop();
						}
					}
					
				}
				if ( matriz[i].Length >= 5
					&& matriz[i][0] == "INIS" 
					&& matriz[i][1]=="PR24" 
					&& matriz[i][2] == "CEX("
					&& matriz[i][3].Substring(0,2) == "ID"
					&& matriz[i][4] == "CEX)"
					&& matriz[i][5] == "FIIN")
				{
					
					Cuadruplo cuadruplo = new Cuadruplo
					{
						Indice = Indice++,
						DatoObj = ObtenerVarTempCuadruplo("INC"),
						DatoFuente1 ="",
						DatoFuente2 = "",
						Operador = "PR24"+", " + ObtenerIndiceCuadruplo("INC"),
						TokenUnico = "SKR"
					};
					cuadruplos.Add(cuadruplo);
					ActualizarCuadruplo();
				}
			}
			//Direccionar PORT y PORF
			foreach (var cuadro in cuadruplos)
			{
				if (cuadro.Operador == "PORT")
				{
					cuadro.Operador = $"{Indice}";
				}
				if (cuadro.Operador == "PORF")
				{
					cuadro.Operador = $"{ObtenerIndiceCuadruplo("INC") + 1}";
				}
				if (cuadro.Operador == "SEFIN")
				{
					cuadro.Operador = $"{Indice}";
				}
			}
			//Cuadruplo elementoMasReciente = cuadruplos[trueFila-1];
			//elementoMasReciente.Operador += ", " + Indice;
			const int numerx = 2;
			if (cuadruplos.Count >= numerx)
			{
				int currentIndex = 0;

				foreach (Cuadruplo elemento in cuadruplos)
				{
					if (currentIndex == (cuadruplos.Count) - numerx)
					{
						cuadruplos[currentIndex].Operador += ", " + Indice; ;
						break;
					}

					currentIndex++;
				}
			}
				
			Cuadruplo cnew = new Cuadruplo
			{
				Indice = Indice++,
				DatoObj = "FIN",
				DatoFuente1 = "",
				DatoFuente2 = "",
				Operador = ""
			};
			cuadruplos.Add(cnew);
			ActualizarCuadruplo();
			EstadoIntermedio = true;
			string AsignarNumero(int numero)
			{
				if (numero.ToString().Length == 1)
				{
					return "TE0" + numero;
				}
				else
				{
					return "TE" + numero;
				}
			}
		}
		private int ObtenerIndiceCuadruplo(string Token)
		{
			int x = -1;
			foreach (Cuadruplo cuadro in cuadruplos)
			{
				if(cuadro.TokenUnico == Token)
				{
					x = cuadro.Indice;
				}
			}
			return x;
		}
		private string ObtenerVarTempCuadruplo(string Token)
		{
			string x = "";
			foreach (Cuadruplo cuadro in cuadruplos)
			{
				if (cuadro.TokenUnico == Token)
				{
					x = cuadro.DatoObj;
				}
			}
			return x;
		}
		private string ObtenerDatoObjXTokenUnico(string Token)
		{
			string x = "";
			foreach (Cuadruplo cuadro in cuadruplos)
			{
				if (cuadro.TokenUnico == Token)
				{
					x = cuadro.DatoObj;
				}
			}
			return x;
		}
		private void ActualizarCuadruplo()
		{
			if(cuadruplos.Count>0)
			{
				dtgCuadruplo.Rows.Clear();
				foreach (Cuadruplo c in cuadruplos)
				{
					dtgCuadruplo.Rows.Add(c.Indice.ToString(), c.DatoObj,c.DatoFuente1,c.DatoFuente2,c.Operador);
				}
			}
		}
		private bool Contiene(string[] arreglo, params string[] valores)
		{
			return arreglo.Intersect(valores).Any();
		}
		
		static string[] ReemplazarElementos(string[] arreglo)
		{
			int indicePR04 = Array.IndexOf(arreglo, "PR04");

			if (indicePR04 == -1)
			{
				throw new ArgumentException("No se encontró la cadena 'PR04' en el arreglo.");
			}

			arreglo[indicePR04 - 1] = "ENTE";
			arreglo[indicePR04] = "PR04";

			for (int i = arreglo.Length - 1; i > indicePR04; i--)
			{
				if (arreglo[i] == "FIIN")
				{
					arreglo[i] = "ENTF";
					break;
				}
			}

			return arreglo;
		}
		static string[] ReemplazarElementos(string[] arreglo, string primerString, string segundoString)
		{
			if (arreglo.Length < 2)
			{
				throw new ArgumentException("El arreglo debe tener al menos dos elementos.");
			}

			arreglo[0] = primerString;
			int indice = Array.IndexOf(arreglo, "FIIN");
			if (indice != -1)
			{
				arreglo[indice] = segundoString;
			}

			return arreglo;
		}
		private string[] DelimitarPosfijo(string[] arreglo)
		{
			string[] nuevoArreglo = new string[arreglo.Length+2];
			if (!(AntesDe(arreglo, "PR04", "POSTE") || AntesDe(arreglo, "POSTF", "ENTF")))
			{
				//ENTE PR04 ID02 ASIG CNE01 ENTF
				int numero = 0;
				for (int i = 0; i < arreglo.Length; i++)
				{
					if(arreglo[i] == "PR04" || arreglo[i] == "PR18")//POSTE
					{
						nuevoArreglo[numero++] = arreglo[i];
						nuevoArreglo[numero++] = "POSTE";
					}else if (arreglo[i] == "ENTF" || arreglo[i] == "REAF")//POSTF
					{
						nuevoArreglo[numero++] = "POSTF";
						nuevoArreglo[numero++] = arreglo[i];
					}
					else
					{
						nuevoArreglo[numero++] = arreglo[i];
					}
				}

				return nuevoArreglo;
			}
			else
			{
				return arreglo;
			}
		}
		private string[] DelimitarPosfijo(string[] arreglo,bool IsEnt)
		{
			if(IsEnt)
			{
				if (!(AntesDe(arreglo, "PR04", "POSTE") || AntesDe(arreglo, "POSTF", "ENTF")))
				{
					List<string> nuevoArreglo = new List<string>();
					for (int i = 0; i < arreglo.Length; i++)
					{
						if (arreglo[i] == "PR04" || arreglo[i] == "PR18")
						{
							nuevoArreglo.Add(arreglo[i]);
							nuevoArreglo.Add("POSTE");
						}
						else if (arreglo[i] == "ENTF" || arreglo[i] == "REAF")
						{
							nuevoArreglo.Add("POSTF");
							nuevoArreglo.Add(arreglo[i]);
						}
						else
						{
							nuevoArreglo.Add(arreglo[i]);
						}
					}

					return nuevoArreglo.ToArray();
				}
				else
				{
					return arreglo;
				}
			}else
			{
				if (!(AntesDe(arreglo, "PR04", "POSTE") || AntesDe(arreglo, "POSTF", "ENTF")))
				{
					List<string> nuevoArreglo = new List<string>();
					for (int i = 0; i < arreglo.Length; i++)
					{
						if (arreglo[i] == "PR04" || arreglo[i] == "PR18")
						{
							nuevoArreglo.Add(arreglo[i]);
							nuevoArreglo.Add("POSTE");
						}
						else if (arreglo[i] == "ENTF" || arreglo[i] == "REAF")
						{
							nuevoArreglo.Add("POSTF");
							nuevoArreglo.Add(arreglo[i]);
						}
						else
						{
							nuevoArreglo.Add(arreglo[i]);
						}
					}

					return nuevoArreglo.ToArray();
				}
				else
				{
					return arreglo;
				}
			}
			
		}
		public static string[] AgregarPoste(string[] arreglo)
		{
			List<string> nuevoArreglo = new List<string>();

			for (int i = 0; i < arreglo.Length; i++)
			{
				if (arreglo[i] == "ASIGE")
				{
					nuevoArreglo.Add(arreglo[i]);
					nuevoArreglo.Add("POSTE");
				}
				else if (arreglo[i] == "ASIGF")
				{
					nuevoArreglo.Add("POSTF");
					nuevoArreglo.Add(arreglo[i]);
				}
				else
				{
					nuevoArreglo.Add(arreglo[i]);
				}
			}

			return nuevoArreglo.ToArray();
		}
		private static bool AntesDe(string[] arreglo, string primeraPalabra, string segundaPalabra)
		{
			if(arreglo.Length > 1)
			{
				for (int i = 1; i < arreglo.Length; i++)
				{
					if (arreglo[i - 1] == primeraPalabra && arreglo[i] == segundaPalabra)
					{
						return true;
					}
				}

				return false;
			}
			return false;
		}

		private static string[] ObtenerElementosEntre(string[] arreglo, string izq, string der)
		{
			string inicio = izq;
			string fin = der;

			int indiceInicio = Array.IndexOf(arreglo, inicio);
			int indiceFin = Array.IndexOf(arreglo, fin);

			// Verificar si se encontraron ambos marcadores
			if (indiceInicio >= 0 && indiceFin >= 0 && indiceFin > indiceInicio)
			{
				// Extraer los elementos entre "POSTE" y "POSTF"
				int elementosCount = indiceFin - indiceInicio - 1;
				string[] elementos = new string[elementosCount];
				Array.Copy(arreglo, indiceInicio + 1, elementos, 0, elementosCount);

				return elementos;
			}

			// Si no se encontraron los marcadores, retornar un arreglo vacío o null, según convenga en tu caso
			return arreglo;
		}
		private string ConversionPosfija(string[] token)
		{
			string[] lista = token;
			Stack<string> pila = new Stack<string>();
			string salida = "";
			int inicios = 0;
			int cierres = 0;
			for (int i = 0; i <= lista.Length - 1; i++)
			{
				if (lista[i] == " ")
				{

				}
				else
				if (EsIgual(lista[i], "OPEX"))
				{
					pila.Push(lista[i]);


				}
				else
				if (EsIgual(lista[i], "OPEX", "OPML", "OPDV", "OPSM", "OPRS", "ASIG", "OPLA", "OPLN", "OPLO", "OPR1", "OPR2", "OPR3", "OPR4", "OPR5", "OPR6") || lista[i] == "CEX(" || lista[i] == "CEX)")
				{
					//Si es parentesis P1
					if (EsIgual(lista[i], "CEX(", "CEX)"))
					{
						if (lista[i] == "CEX(")
						{
							inicios++;
						}
						else
						{
							cierres++;
						}
						pila.Push(lista[i]);
						if (inicios > 0 && cierres > 0)
						{
							string p;
							while (pila.Peek() != "CEX(")
							{
								if (pila.Peek() == "CEX)")
								{
									p = pila.Pop();//Saca el )
								}
								else
								{
									p = pila.Pop(); // saca el elemento
									salida += p + " ";
								}

							}
							pila.Pop();//Saca el (

							inicios--; cierres--;
						}
					}
					else
					// si es mutiplicacion o division P3
					if (EsIgual(lista[i], "OPML", "OPDV"))
					{
						if (pila.Count > 0 && EsIgual(pila.Peek(), "OPEX", "OPML", "OPDV"))
						{
							salida += pila.Pop() + " ";
							pila.Push(lista[i]);
						}
						else
						{
							pila.Push(lista[i]);
						}
					}
					else
					//si es suma o resta P4
					if (EsIgual(lista[i], "OPSM", "OPRS"))
					{
						if (pila.Count > 0 && EsIgual(pila.Peek(), "OPEX", "OPML", "OPDV", "OPSM", "OPRS"))
						{
							salida += pila.Pop() + " ";
							pila.Push(lista[i]);
						}
						else
						{
							pila.Push(lista[i]);
						}
					}
					else
					//Si es op relacional 
					if (EsIgual(lista[i], "OPR1", "OPR2", "OPR3", "OPR4", "OPR5", "OPR6"))
					{
						if (pila.Count > 0 && EsIgual(pila.Peek(), "OPEX", "OPML", "OPDV", "OPSM", "OPRS", "OPR1", "OPR2", "OPR3", "OPR4", "OPR5", "OPR6"))
						{
							salida += pila.Pop() + " ";
							pila.Push(lista[i]);
						}
						else
						{
							pila.Push(lista[i]);
						}
					}
					else
					//si es op logica
					if (EsIgual(lista[i], "OPLA", "OPLN", "OPLO"))
					{
						if (pila.Count > 0 && EsIgual(pila.Peek(), "OPEX", "OPML", "OPDV", "OPSM", "OPRS", "OPR1", "OPR2", "OPR3", "OPR4", "OPR5", "OPR6", "OPLA", "OPLN", "OPLO"))
						{
							salida += pila.Pop() + " ";
							pila.Push(lista[i]);
						}
						else
						{
							pila.Push(lista[i]);
						}
					}
					else
					//si es suma o resta P5
					if (lista[i] == "ASIG")
					{
						if (pila.Count > 0 && EsIgual(pila.Peek(), "OPEX", "OPML", "OPDV", "OPSM", "OPRS"))
						{
							salida += pila.Pop() + " ";
							pila.Push(lista[i]);
						}
						else
						{
							pila.Push(lista[i]);
						}
					}
					else
					{
						//algo fuera de lo comun, no hacer nada
					}
				}
				else
				{
					string sinEspacios = "";
					foreach (char item in lista[i])
					{
						if (item != ' ')
						{
							sinEspacios += item;
						}
					}
					salida += sinEspacios + " ";
				}
				if (i == lista.Length - 1)
				{
					while (pila.Count > 0)
					{
						string x = pila.Pop();
						if (x == "CEX(" || x == "CEX)")
						{

						}
						else
						{
							salida += x + " ";
						}
					}

				}
			}
			return salida;
		}
		static bool VerificarID(string texto)
		{
			if (texto.Length >= 2)
			{
				string primerosDosCaracteres = texto.Substring(0, 2);
				return primerosDosCaracteres == "ID";
			}

			return false;
		}
		private static string ConvertirArregloACadena1(string[][] arreglo)
		{
			List<string> elementos = new List<string>();

			foreach (string[] subarreglo in arreglo)
			{
				string subcadena = string.Join(" ", subarreglo);
				elementos.Add(subcadena);
			}

			string resultado = string.Join(Environment.NewLine, elementos);

			return resultado;
		}
		private static string[][] LimpiarArreglo(string[][] matriz)
		{
			List<string[]> nuevaMatriz = new List<string[]>();

			foreach (string[] fila in matriz)
			{
				// Filtrar los elementos no vacíos en la fila
				string[] filaFiltrada = fila.Where(elemento => !string.IsNullOrWhiteSpace(elemento)).ToArray();

				// Agregar la fila filtrada a la nueva matriz
				if (filaFiltrada.Length > 0)
				{
					nuevaMatriz.Add(filaFiltrada);
				}
			}

			return nuevaMatriz.ToArray();
		}

		private void panelCodigo_Paint(object sender, PaintEventArgs e)
		{

		}

		private void CargarCuadruploARich()
		{
			StringBuilder contenido = new StringBuilder();

			// Recorrer las filas del DataGridView
			foreach (DataGridViewRow fila in dtgCuadruplo.Rows)
			{
				// Recorrer las celdas de la fila actual
				foreach (DataGridViewCell celda in fila.Cells)
				{
					// Verificar si la celda tiene un valor
					if (celda.Value != null)
					{
						// Obtener el valor de la celda y agregarlo al contenido
						contenido.Append(celda.Value.ToString());
					}
					contenido.Append(" | "); // Separador entre celdas
				}

				contenido.AppendLine(); // Nueva línea después de cada fila
			}

			// Asignar el contenido al TextBox
			rchCodigoObjeto.Text = contenido.ToString();

		}
		public void ReemplazarValor(List<Cuadruplo> cuadruplos, string valorActual, string valorNuevo)
		{
			foreach (Cuadruplo cuadro in cuadruplos)
			{
				if (cuadro.DatoFuente1 == valorActual)
				{
					cuadro.DatoFuente1 = valorNuevo;
				}

				if (cuadro.DatoFuente2 == valorActual)
				{
					cuadro.DatoFuente2 = valorNuevo;
				}

				if (cuadro.Operador == valorActual)
				{
					cuadro.Operador = valorNuevo;
				}
			}
		}

		public int ContarRepeticiones(List<Cuadruplo> cuadruplos, string parametro)
		{
			int contador = 0;

			foreach (Cuadruplo cuadro in cuadruplos)
			{
				if (cuadro.DatoObj == parametro)
				{
					contador++;
				}

				if (cuadro.DatoFuente1 == parametro)
				{
					contador++;
				}

				if (cuadro.DatoFuente2 == parametro)
				{
					contador++;
				}
			}

			return contador;
		}
		List<Cuadruplo> cuadruploOptimo;
		private void btnOptimo_Click(object sender, EventArgs e)
		{
			if(EstadoIntermedio)
			{
				//CargarCuadruploARich();
				cuadruploOptimo = cuadruplos;

				//Adaptar Datos
				foreach (Cuadruplo cuadro in cuadruploOptimo)
				{
					if(cuadro.Operador.Contains(","))
					{
						string[] div = cuadro.Operador.Split(',');
						cuadro.Operador = div[0];
						cuadro.Destino = int.Parse(div[1]);
					}
				}
				//Quitar 0
				MetaDatos TokenCero = _listaMeta.FirstOrDefault(x => x.Valor == "0");
				List<Cuadruplo> listaTemporalCuadriplo = new List<Cuadruplo>();
				int filasmenos = 0;

				if (TokenCero != null)
				{
					foreach (Cuadruplo cuadro in cuadruploOptimo)
					{
						if (cuadro.DatoFuente1 == TokenCero.TokenUnico && cuadro.DatoFuente2 == TokenCero.TokenUnico)
						{
							listaTemporalCuadriplo.Add(cuadro);
							filasmenos++;
						}
						else if (cuadro.DatoFuente1 == TokenCero.TokenUnico)
						{
							cuadro.DatoFuente1 = cuadro.DatoFuente2;
							cuadro.DatoFuente2 = "";
							cuadro.Operador = "ASIG";
						}
						else if (cuadro.DatoFuente2 == TokenCero.TokenUnico)
						{
							cuadro.DatoFuente2 = "";
							cuadro.Operador = "ASIG";
						}
						else
						{
							if (int.TryParse(cuadro.Operador, out int resultado))
							{
								cuadro.Operador = "" + (resultado - filasmenos);
							}
							cuadro.Indice = cuadro.Indice - filasmenos;
							cuadro.Destino = cuadro.Destino - filasmenos;
						}
					}

					// Elimina los cuádruplos de la lista original
					foreach (Cuadruplo cuadro in listaTemporalCuadriplo)
					{
						cuadruploOptimo.Remove(cuadro);
					}
				}
				//Buscar valores que se declararon pero no usaron
				listaTemporalCuadriplo = new List<Cuadruplo>();
				filasmenos = 0;

				foreach (Cuadruplo cuadro in cuadruploOptimo)
				{
					if (cuadro.Operador == "ASIG" && ContarRepeticiones(cuadruploOptimo, cuadro.DatoObj) <= 1)
					{
						listaTemporalCuadriplo.Add(cuadro);
						filasmenos++;
					}
					else
					{
						if(int.TryParse(cuadro.Operador, out int resultado))
						{
							cuadro.Operador = "" + (resultado - filasmenos);
						}
						cuadro.Indice = cuadro.Indice - filasmenos;
						cuadro.Destino = cuadro.Destino - filasmenos;
					}
				}
				string token1 = "CNE02";
				string token2 = "ID02";
				string token3 = "TE02";
				string token4 = "OPSM";
				string token5 = "ASIG";
				// Elimina los cuádruplos de la lista original
				foreach (Cuadruplo cuadro in listaTemporalCuadriplo)
				{
					cuadruploOptimo.Remove(cuadro);
				}
				//Eliminar declaraciones o asignacion iguales
				int counta = 0;
				string antiguoa = "";
				int countb = 0;
				string antiguob = "";
				listaTemporalCuadriplo = new List<Cuadruplo>();
				filasmenos = 0;

				foreach (Cuadruplo cuadro in cuadruploOptimo)
				{
					if (cuadro.DatoFuente1 == token1 && cuadro.DatoFuente2 == token2 && token4 == cuadro.Operador)
					{
						if (counta > 0)
						{
							listaTemporalCuadriplo.Add(cuadro);
							filasmenos++;
							ReemplazarValor(cuadruploOptimo, cuadro.DatoFuente1, antiguoa);
						}
						else
						{
							counta++;
							antiguoa = cuadro.DatoObj;
						}
					}else
					if (cuadro.DatoFuente1 == token3 && cuadro.DatoFuente2 == "" && token5 == cuadro.Operador)
					{
						if(countb>0)
						{
							listaTemporalCuadriplo.Add(cuadro);
							filasmenos++;
							ReemplazarValor(cuadruploOptimo, cuadro.DatoFuente1, antiguob);
						}
						else
						{
							countb++;
							antiguob = cuadro.DatoObj;
						}
					}
					else
					{
						if (int.TryParse(cuadro.Operador, out int resultado))
						{
							cuadro.Operador = "" + (resultado - filasmenos);
						}
						cuadro.Indice = cuadro.Indice - filasmenos;
						cuadro.Destino = cuadro.Destino - filasmenos;
					}
				}

				// Elimina los cuádruplos de la lista original
				foreach (Cuadruplo cuadro in listaTemporalCuadriplo)
				{
					cuadruploOptimo.Remove(cuadro);
				}
				ActualizardtgOptimo();
			}
			else
			{
				MessageBox.Show("Debes crear primero el codigo intermedio");
			}
		}
		public static bool IsNumeric(string input)
		{
			return double.TryParse(input, out _);
		}
		static bool VerificarExistencia(List<string> lista, string palabra)
		{
			return lista.Contains(palabra);
		}
		private void ActualizardtgOptimo()
		{
			dtgViewOptimizada.Rows.Clear();
			if(cuadruploOptimo != null)
			{
				foreach (Cuadruplo c in cuadruploOptimo)
				{
					string x = "";
					if(c.Destino >0)
					{
						x = c.Operador + ", " + c.Destino;
					}else
					{
						x = c.Operador;
					}
					dtgViewOptimizada.Rows.Add(c.Indice, c.DatoObj, c.DatoFuente1, c.DatoFuente2, x);
				}
			}
		}
		List<Cuadruplo> cuadruploObjeto;
		private void btnObjeto_Click(object sender, EventArgs e)
		{
			if(EstadoIntermedio)
			{
				rchCodigoObjeto.Focus();
				cuadruploObjeto = cuadruplos;
				string CABECERA = "";
				string CUERPO = "";
				List<string> listaFiltrada = new List<string>();
				int al = 0;
				List<int> numerosUnicos = new List<int>();
				foreach (Cuadruplo numero in cuadruploObjeto)
				{
					if (!numerosUnicos.Contains(numero.Destino))
					{
						numerosUnicos.Add(numero.Destino);
					}
					int operador;
					if (int.TryParse(numero.Operador, out operador))
					{
						if (!numerosUnicos.Contains(operador))
						{
							numerosUnicos.Add(operador);
						}
					}
				}
				int c19 = 0;
				string ultimacompar = "no";
				int varn = 12;
				bool existe;
				foreach (Cuadruplo c in cuadruploObjeto)
				{
					int.TryParse(c.Operador, out int destiny);
					bool isNotNumber = !IsNumeric(c.Operador);
					
					if (c.Indice == varn)
					{

					}
					if (c.Indice == 9)
					{
						CUERPO += "V9 \n";
						c19++;
					}
					//Comparacion es mayor que >
					string ray;
					if(c.Operador != "22")
					{
						ray = $"F{c.Operador}";
					}else
					{
						ray = "FIN";
					}
					if (c.DatoFuente2 == "TRUE")
					{
						if (ultimacompar == "OPR1")
						{
							CUERPO += $"JG {ray}     ; Saltar a la etiqueta MAYOR si la comparación es mayor que{Environment.NewLine}";
							
						}if(ultimacompar == "OPR6")
						{
							CUERPO += $"JL {ray}     ; Saltar a la etiqueta MENOR si la comparación es menor que{Environment.NewLine}";
									 
						}
					}
					else
					if (c.DatoFuente2 == "FALSE") // si es menor que
					{
						if (ultimacompar == "OPR1")
						{
							CUERPO += $"JMP {ray}     ; Saltar a la etiqueta FIN si la comparación no es mayor que{Environment.NewLine}";
							ultimacompar = "nada";
						}else if (ultimacompar == "OPR6")
						{
							CUERPO += $"JL {ray}    ; Saltar a la etiqueta MENOR si la comparación es menor que{Environment.NewLine}";
							ultimacompar = "nada";
						}
					}
					if (isNotNumber)
					{
						
						if ( c.Indice == 9 || c.Indice == 12 || c.Indice == 22 || c.Indice == 13 || c.Indice == 17 || c.Indice == 20)
						{
							
							//if(c.Indice == 9)
							//{
							//	CUERPO += $"X 9: \n";
							//}
							if (c.Indice == 12)
							{
								CUERPO += $"X 12: \n";
							}
							if (c.Indice == 22)
							{
								CUERPO += $"X 22: \n";
							}
							if (c.Indice == 13)
							{
								CUERPO += $"X 13: \n";
							}
							if (c.Indice == 17)
							{
								CUERPO += $"X 17: \n";
							}
							if (c.Indice == 20)
							{
								CUERPO += $"X 20: \n";
							}
						}
						//Datos objeto
						MetaDatos metadatoObjeto = _listaMeta.Find(objeto => objeto.TokenUnico == c.DatoObj);
						MetaDatos metadatoFuente1 = _listaMeta.Find(objeto => objeto.TokenUnico == c.DatoFuente1);
						MetaDatos metadatoFuente2 = _listaMeta.Find(objeto => objeto.TokenUnico == c.DatoFuente2);
						existe = VerificarExistencia(listaFiltrada, c.DatoObj);

						if (metadatoObjeto != null && metadatoFuente1 != null && metadatoFuente2 != null)
						{
							//incrementar variable
							if(metadatoObjeto.Token == "IDEN" && metadatoFuente1.Token =="IDEN" && metadatoFuente2.Token=="CONE")
							{
								existe = VerificarExistencia(listaFiltrada, metadatoObjeto.Variable);
								if (!existe)
								{
									CABECERA += $"{metadatoObjeto.Variable} DW ? \n";
									listaFiltrada.Add(metadatoObjeto.Variable);
								}
								CUERPO += $"MOV AX, [{metadatoFuente1.Variable}]     ; Cargar el valor de la variable x en el registro AX{Environment.NewLine}" +
											$"ADD AX, {metadatoFuente2.Valor}     ; Incrementar el valor en 1{Environment.NewLine}" +
											 $"MOV [{metadatoObjeto.Variable}], AX     ; Guardar el resultado de vuelta en la variable x{Environment.NewLine}";
								
							}

						}
						else
						if (metadatoObjeto == null && metadatoFuente1 != null && metadatoFuente2 != null)// objeto es TE01
						{
							//Suma
							if (metadatoFuente1.Token == "CONE" && metadatoFuente2.Token == "IDEN"
								&& c.Operador == "OPSM")//temporal con numero y identificador
							{ 
								existe = VerificarExistencia(listaFiltrada, c.DatoObj);
								if (!existe &&  c.DatoObj != "TE02")
								{
									CABECERA += $"{c.DatoObj} DW ? \n";
									listaFiltrada.Add(c.DatoObj);
								}
								if(metadatoFuente1.Valor == "3" && metadatoFuente2.Variable == "_a")
								{
									if(al == 0)
									{
										al++;
										CUERPO += $"MOV AX, 3 \n" +
										  $"ADD AX, [_a]\n" +
										  $"MOV [{c.DatoObj}], AX\n\n";
									}
								}else
								{
									CUERPO += $"MOV AX, {metadatoFuente1.Valor}\n" +
										  $"ADD AX, [{metadatoFuente2.Variable}]\n" +
										  $"MOV [{c.DatoObj}], AX\n\n";
								}
							}
							//Suma si los dos son numeros
							if (metadatoFuente1.Token == "CONE" && metadatoFuente2.Token == "CONE" && c.Operador == "OPSM") // fuente 1 y 2 son numeros
							{
								existe = VerificarExistencia(listaFiltrada, c.DatoObj);
								if (!existe)
								{
									CABECERA += $"{c.DatoObj} DW ? \n";
									listaFiltrada.Add(c.DatoObj);
								}
								CUERPO += $"MOV AX, {metadatoFuente1.Valor}\n" +
										  $"ADD AX, [{metadatoFuente2.Valor}]\n" +
										  $"MOV [{c.DatoObj}], AX\n\n";
							}
						}
						if (metadatoObjeto != null && metadatoFuente1 == null && metadatoFuente2 == null)
						{
							//el objeto tiene registro pero las fuentes no
							if (c.DatoFuente2 == "" && c.Operador == "ASIG")
							{
								existe = VerificarExistencia(listaFiltrada, metadatoObjeto.Variable);
								if (!existe)
								{
									CABECERA += $"{metadatoObjeto.Variable} DW ? \n";
									listaFiltrada.Add(metadatoObjeto.Variable);
								}
								CUERPO += $"MOV AX, [{c.DatoFuente1}]\n" +
								$"MOV [{metadatoObjeto.Variable}], AX\n\n";
							}
							
							
						}
						else if(metadatoObjeto != null && metadatoFuente1 != null  && metadatoFuente2 == null)
						{
							//ASIGNACION SIMPLE 1
							if(c.DatoFuente2 == "" && metadatoFuente1.Token == "CONE" && c.Operador == "ASIG")
							{
								existe = VerificarExistencia(listaFiltrada, metadatoObjeto.Variable);
								if (!existe)
								{
									CABECERA += $"{metadatoObjeto.Variable} DW {metadatoFuente1.Valor} ;ASIG1\n";
									listaFiltrada.Add(metadatoObjeto.Variable);
								}else
								{
									CUERPO += $"MOV AX, {metadatoFuente1.Valor}\n" +
											 $"MOV [{metadatoObjeto.Variable}], AX\n\n";
								}
								
							}
							//Si fuente 1 es una id
							if(c.DatoFuente2 == "" && metadatoFuente1.Token == "IDEN" && c.Operador == "ASIG")
							{
								CUERPO += $"MOV AX, {metadatoFuente1.Variable}\n" +
											 $"MOV [{metadatoObjeto.Variable}], AX\n\n";

							}
						}else if(metadatoObjeto == null && metadatoFuente1 != null && metadatoFuente2 != null)
						{
							//TE0x para operaciones relacionales
							if (metadatoFuente1.Token == "IDEN" && metadatoFuente2.Token == "CONE" && c.Operador == "OPR1")
							{
								if (!existe)
								{//Declara la variabele como byte 
									CABECERA += $"{c.DatoObj} db ?     ; Declaración de la variable como un byte\n";
									listaFiltrada.Add(c.DatoObj);
								}
								CUERPO += CUERPO += $"CMP [{metadatoFuente1.Variable}], {metadatoFuente2.Valor}     ; Comparar el valor de la variable ID{Environment.NewLine}" +
														$"MOV AX, 0     ; Limpiar el registro AX{Environment.NewLine}"+
														 $"MOV [{c.DatoObj}], AX     ; Guardar el resultado en la variable{Environment.NewLine}"; ;

								ultimacompar = "OPR1";


							}else
							if(metadatoFuente1.Token == "IDEN" && metadatoFuente2.Token == "CONE" && c.Operador == "OPR6")
							{
								
								if (!existe)
								{//Declara la variabele como byte 
									CABECERA += $"{c.DatoObj} db ?     ; Declaración de la variable como un byte\n";
									listaFiltrada.Add(c.DatoObj);
								}
								CUERPO += $"CMP [{metadatoFuente1.Variable}], {metadatoFuente2.Valor}     ; Comparar el valor de la variable con {metadatoFuente2.Valor}{Environment.NewLine}" +
										  $"MOV AX, 0     ; Limpiar el registro AX{Environment.NewLine}" +
										  $"MOV [{c.DatoObj}], AX     ; Guardar el resultado en la variable{Environment.NewLine}";
								ultimacompar = "OPR6";
							}
						}
						else
						{

						}
						if (c.Operador == "PR24" && metadatoObjeto!= null)
						{
							
							CUERPO += $"MOV AX, {metadatoObjeto.Variable}     ; Cargar el valor del identificador en el registro AX{Environment.NewLine}" +
								$"MOV AH, 2     ; Establecer la función de impresión de carácter del servicio de interrupción 21H{Environment.NewLine}" +
							$"INT 21H     ; Llamar a la interrupción del sistema para imprimir el valor{Environment.NewLine}";

						}
					}
					if (c.DatoObj == "FIN")
					{
						CUERPO += "FIN:\n" +
						   "MOV AH, 4CH\n" +
						   "INT 21H\n";
					}
					if (c.Destino > 0 && c.Destino < cuadruploObjeto.Count )
					{
						if(c.Destino == 22)
						{
							CUERPO += $"JMP FIN     ; Saltar a la etiqueta\n\n\n";
						}
						else 
						CUERPO += $"JMP F{c.Destino}     ; Saltar a la etiqueta\n\n\n";
					}
				}
				//foreach (Cuadruplo elemento in cuadruplos)
				//{
				//	if (!listaFiltrada.Contains(elemento.DatoObj))
				//	{
				//		MetaDatos metadatoObjeto = _listaMeta.Find(objeto => objeto.TokenUnico == elemento.DatoObj);
				//		listaFiltrada.Add(elemento.DatoObj);
				//		CABECERA += $"{metadatoObjeto.Variable} DW ? ;AUTO ASIG\n";
				//	}
				//}
				string codigoEnsamblador = "DATA SEGMENT\n"+
										$"{CABECERA}\n"+
									"DATA ENDS\n"+

									"CODE SEGMENT\n"+
									"START:\n"+
										$"{CUERPO}\n"+
									"END START\n"+
									"CODE ENDS\n"+

									"END\n";
				rchCodigoObjeto.Text = codigoEnsamblador;
			}
			else
			{
				MessageBox.Show("Debes generar un cuadruplo.");
			}
		}

		private void rchSintactico_TextChanged(object sender, EventArgs e)
		{

		}
	}
}
