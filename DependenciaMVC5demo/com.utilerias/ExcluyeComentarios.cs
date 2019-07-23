using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DependenciaMVC5demo.com.dto;

namespace DependenciaMVC5demo.com.utilerias
{
    public class ExcluyeComentarios
    {
        public string ComentarioBloque = "";
        public IEnumerable<ComentarioDto> Comentarios = new List<ComentarioDto>();

        string inicioBloque = ""; 
        string finBloque = "";
        string comentarioSimple = "";
        
        public string BuscarComentario(string lineaCodigo,string extension)
        {
            string subStr = "", resultado = "";
            int pos1, pos2, pos3;

            lineaCodigo = lineaCodigo.Trim();
            resultado = lineaCodigo;

            finBloque = (Comentarios.FirstOrDefault(c => c.Extension == extension && c.Tipo == "bloque") == null) ? "" : Comentarios.FirstOrDefault(c => c.Extension == extension && c.Tipo == "bloque").Fin;
            inicioBloque = (Comentarios.FirstOrDefault(c => c.Extension == extension && c.Tipo == "bloque") == null) ? "" : Comentarios.FirstOrDefault(c => c.Extension == extension && c.Tipo == "bloque").Inicio;
            comentarioSimple = (Comentarios.FirstOrDefault(c => c.Extension == extension && c.Tipo == "simple") == null) ? "" : Comentarios.FirstOrDefault(c => c.Extension == extension && c.Tipo == "simple").Inicio;
           
            if (ComentarioBloque == finBloque)
            {
                //pos1 = lineaCodigo.IndexOf("*/");
                pos1 = lineaCodigo.IndexOf(finBloque);
                //si esta el caracter entonces obtener la parte que no forma parte del comentario
                if (pos1 >= 0)
                {
                    ComentarioBloque = "";
                    //resultado = lineaCodigo.Substring(pos1 + 2);
                    resultado = lineaCodigo.Substring(pos1 + finBloque.Length);
                }
                else
                    resultado = "";
            }
            else
            {
                //pos1 = lineaCodigo.IndexOf("//");
                pos1 = (string.IsNullOrEmpty(comentarioSimple)) ?  -1 : lineaCodigo.IndexOf(comentarioSimple);
                //pos2 = lineaCodigo.IndexOf("/*");
                pos2 = (string.IsNullOrEmpty(inicioBloque)) ? -1 : lineaCodigo.IndexOf(inicioBloque);
                //pos3 = lineaCodigo.IndexOf("*/");
                pos3 = (string.IsNullOrEmpty(finBloque)) ? -1 : lineaCodigo.IndexOf(finBloque);
                //pos3 = lineaCodigo.IndexOf(finBloque);

                //las regiones en C# tambien las consideramos como comentarios simples
                if (lineaCodigo.StartsWith("#"))
                    pos1 = 0;

                if (pos1 >= 0 && pos2 < 0)
                    pos2 = pos1 + 2;

                if (pos2 >= 0 && pos1 < 0)
                    pos1 = pos2 + 2;


                if (pos1 >= 0 || pos2 >= 0)
                {
                    //si la linea tiene un comentario de tipo simple entonces
                    if (pos1 < pos2)
                    {
                        ComentarioBloque = "";
                        //si el comentario simple es // verificar que no este dentro de una url
                        if (comentarioSimple == @"//")
                        {
                            //if (lineaCodigo.IndexOf("http") < pos1 || lineaCodigo.IndexOf("ftp") < pos1 || lineaCodigo.IndexOf("ldap") < pos1 || lineaCodigo.IndexOf("mailto") < pos1)
                            //int posUrl = lineaCodigo.IndexOf(":");
                            //if (posUrl >= 0 && posUrl == pos1 -1)
                            if (pos1 > 0)
                            {
                                //resultado = lineaCodigo;
                                string result = lineaCodigo.Substring(pos1 - 1, 1);
                                //if (lineaCodigo.Substring(pos1-1,1) == ":")
                                if(result == ":")
                                    resultado = lineaCodigo;
                                else
                                    resultado = lineaCodigo.Substring(0, pos1);
                            }
                            else
                                resultado = lineaCodigo.Substring(0, pos1);
                        }
                        else
                            resultado = lineaCodigo.Substring(0, pos1);

                    }
                    else
                    {
                        // si */ esta despues que algun /* entonces tenemos cierre de bloque
                        if (pos3 > pos2)
                        {
                            ComentarioBloque = "";
                            //subStr = lineaCodigo.Substring(pos2, pos3 - pos2 + 2);
                            subStr = lineaCodigo.Substring(pos2, pos3 - pos2 + finBloque.Length);
                            resultado = lineaCodigo.Replace(subStr, string.Empty);                            
                        }
                        else
                        {
                            resultado = lineaCodigo.Substring(0, pos2);
                            //resultado:= copy(lineaCodigo, 1, pos2 - 1);
                            //ComentarioBloque = "*/";
                            ComentarioBloque = finBloque;
                        }
                    }
                }

            }
            return resultado;
        }      

    }
}
