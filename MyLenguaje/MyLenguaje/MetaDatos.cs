using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLenguaje
{
	public class MetaDatos
	{
		public MetaDatos()
		{

		}
		public int ID { get; set; } //1
		public string Variable { get; set; } // _x
		public string TipoDato { get; set; } //Ent
		public string Token { get; set; } // IDEN
		public string Valor { get; set; } // "1"
		public string TokenUnico { get; set; }// ID01
		public int Fila { get; set; }//1
		public override string ToString()
		{
			return $"[ID: {ID}, Variable: {Variable}, TipoDato: {TipoDato}, Token: {Token}, Valor: {Valor}, TokenUnico: {TokenUnico}, Fila: {Fila}]";
		}
	}
}
