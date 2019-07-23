using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DependenciaMVC5demo.com.dto;

namespace DependenciaMVC5demo.com.negocio
{
    public class Salida
    {
        public List<string> GenerarSalida(string separador, StringBuilder csvSalida, List<ObjetoDto> listaHallazgos)
        {
            List<string> resultado = new List<string>();
            if (csvSalida != null && listaHallazgos != null)
            {
                int DependenciaID = 0;
                foreach(ObjetoDto linea in listaHallazgos)
                {
                    csvSalida.AppendLine(AgregarComillas(linea.AplicacionID.ToString()) + separador + AgregarComillas(linea.CveAplicacion) + separador + AgregarComillas(linea.NumLinea.ToString()) + separador 
                    + AgregarComillas(linea.Referencia)  +  separador + AgregarComillas(linea.ObjetoID.ToString()) +  separador + AgregarComillas(linea.NombreObjeto) + separador + AgregarComillas(linea.TipoID.ToString()) +separador + AgregarComillas(linea.Tipo) + separador 
                    + AgregarComillas(linea.BaseDatosID.ToString()) + separador + AgregarComillas(linea.NombreBd) + separador +  AgregarComillas(linea.Archivo) + separador + AgregarComillas(linea.Lenguaje) + separador + AgregarComillas(linea.BibPadre) + separador + AgregarComillas(linea.ObjPadre));

                    //(linea.CveAplicacion)(linea.NombreObjeto)(linea.Tipo) (linea.NombreBd)
                    ++DependenciaID;
                    resultado.Add(DependenciaID + "¡" + linea.CveAplicacion + "¡" + linea.BibPadre + "¡" + " " + "¡" + linea.ObjPadre + "¡" + linea.BaseDatosID.ToString() + "¡" + linea.ObjetoID.ToString() + "¡" + linea.TipoID.ToString() + "¡" + linea.NumLinea.ToString() + "¡" + linea.Referencia + "¡" + linea.Archivo + "¡" + linea.Lenguaje);
                }
            }
            return resultado;
        }
        
        public List<ObjetoDto> Depurar(List<ObjetoDto> lista)
        {
            foreach (ObjetoDto linea in lista)
            {

            }
            return null;
        }


        public string AgregarComillas(string cadena)
        {
            return "\"" + cadena + "\"";
        }

    }
}
