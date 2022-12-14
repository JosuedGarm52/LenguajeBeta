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
		static string conexionstring = "Server = localhost; Database = lenguajesyautomatas; Uid = root; Pwd = ;";
		public static MySqlConnection conexion = new MySqlConnection(conexionstring);
		MySqlCommand cmd;
		MySqlDataReader reader;
		public Form1()
		{
			InitializeComponent();
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
				string query = "SELECT * FROM validador ";
				string a = "";
				cmd = new MySqlCommand(query, conexion);
				reader = cmd.ExecuteReader();
				while (reader.Read())
				{
					a = reader["letra"].ToString();
				}
				reader.Close();
				if (string.IsNullOrWhiteSpace(a))
				{
					MessageBox.Show("Error");
				}
				else
				{
					MessageBox.Show("Si" + a);
				}
			}
			catch (Exception ex)
			{
				Mensaje(ex.Message);
			}
			
		}
		private void Mensaje(string men)
		{
			MessageBox.Show(men);
		}
	}
}
