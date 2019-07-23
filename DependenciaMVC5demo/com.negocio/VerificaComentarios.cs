using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependenciaMVC5demo.com.negocio
{
    public class VerificaComentarios
    {
        public string ComentarioBloque = "";

        public string BuscarComentario(string lineaCodigo)
        {
            string subStr = string.Empty, resultado = string.Empty;
            try
            {

                ////int pos1, pos2, pos3;

                lineaCodigo = lineaCodigo.Trim();
                resultado = lineaCodigo;

                if (lineaCodigo.StartsWith("//") || lineaCodigo.StartsWith("/*") || lineaCodigo.StartsWith("*/") ||
                    lineaCodigo.StartsWith("*") || lineaCodigo.StartsWith("<!--") || lineaCodigo.StartsWith("'") ||
                    lineaCodigo.StartsWith("#"))
                {
                    resultado = "";
                }

                //if (comentarioBloque == "*/")
                //{
                //    pos1 = lineaCodigo.IndexOf("*/");
                //    //si esta el caracter entonces obtener la parte que no forma parte del comentario
                //    if (pos1 >= 0)
                //    {
                //        comentarioBloque = "";
                //        resultado = lineaCodigo.Substring(pos1 + 2);
                //        //copy(lineaCodigo, pos1 + 2, length(lineaCodigo));
                //    }
                //    else
                //        resultado = "";
                //}
                //else
                //{
                //    pos1 = lineaCodigo.IndexOf("//");
                //    pos2 = lineaCodigo.IndexOf("/*");
                //    pos3 = lineaCodigo.IndexOf("*/");

                    //    //las regiones en C# tambien las consideramos como comentarios simples
                    //    if (lineaCodigo.StartsWith("#"))
                    //        pos1 = 0;

                    //    if (pos1 >= 0 && pos2 < 0)
                    //        pos2 = pos1 + 2;

                    //    if (pos2 >= 0 && pos1 < 0)
                    //        pos1 = pos2 + 2;


                    //    if (pos1 >= 0 || pos2 >= 0)
                    //    {
                    //        if (pos1 < pos2)
                    //        {
                    //            comentarioBloque = "";
                    //            resultado = lineaCodigo.Substring(0, pos1);
                    //            //subStr = copy(lineaCodigo, pos1, length(lineacodigo));
                    //            //resultado:= AnsiReplaceStr(lineaCodigo, subStr, ' ');
                    //        }
                    //        else
                    //        {
                    //            // si */ esta despues que algun /* entonces tenemos cierre de bloque
                    //            if (pos3 > pos2)
                    //            {
                    //                comentarioBloque = "";
                    //                subStr = lineaCodigo.Substring(pos2, pos3 - pos2 + 2);
                    //                resultado = lineaCodigo.Replace(subStr, string.Empty);
                    //                //copy(lineaCodigo, pos2, pos3 - pos2 + 2);
                    //                //resultado:= AnsiReplaceStr(lineaCodigo, subStr, ' ');
                    //            }
                    //            else
                    //            {
                    //                resultado = lineaCodigo.Substring(0, pos2);
                    //                //resultado:= copy(lineaCodigo, 1, pos2 - 1);
                    //                comentarioBloque = "*/";
                    //            }
                    //        }
                    //    }

                    //}
            }
            catch (Exception Err)
            {
                throw new Exception("VerificaComentarios.BuscarComentario ", Err);
            }
            return resultado;
        }
    }
}