using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace DependenciaMVC5demo.com.negocio
{
    public class EncuentraMetodo
    {
        string NombreClase = "";
        string NombreMetodo = "";

        public bool EsMetodo(string Linea)
        {
            try
            {
                if ((Linea.IndexOf("(") < 0) || (Linea.IndexOf("=") >= 0))
                    return false;
                else
                {
                    //Si hay un void entonces sabemos que es un metodo
                    if (Linea.IndexOf("void") >= 0)
                        return true;
                    else
                    {
                        Regex Regex = new Regex(@"^([A-Z]|[a-z])([a-zA-Z]\s*|\[|\s*\])+\s+\w+\s*\(");
                        Match Match = Regex.Match(Linea);

                        if (Linea.StartsWith("private") || Linea.StartsWith("public") || Linea.StartsWith("protected") ||
                            Linea.StartsWith("internal") || Linea.StartsWith("static"))
                            return true;
                        else if ((Match.Success))
                            return true;
                        else //si no tiene private, public o protected entonces lo descartaremos como metodo  	 	  	  		 		  
                            return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public  string ExtraerNombreMetodo(string Linea)
        {
            try
            {
                string Resultado = "";
                Regex Regex = new Regex(@"\w+\s*\(");
                Match Match = Regex.Match(Linea);
                if (Match.Success)
                {
                    Resultado = Match.Value;
                    Resultado = Resultado.Replace("(", "").Trim();
                }
                return Resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return string.Empty;
            }
        }

        public string ExtraerNombreObjeto(string Linea)
        {
            try
            {
                string Resultado = "";
                Regex Regex = new Regex(@"CreateObject\W+\w+\.\w+");
                Match Match = Regex.Match(Linea);
                if (Match.Success)
                {
                    Resultado = Match.Value;
                    Resultado = Resultado.Replace("CreateObject(\"", "").Trim();
                }
                return Resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return string.Empty;
            }
        }

        public bool EsFuncion(string Linea)
        {
            try
            {
                Regex Regex = new Regex(@"function\s*\w*\(");
                Match Match = Regex.Match(Linea);

                if (Match.Success)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public bool EsObjeto(string Linea)
        {
            try
            {
                Regex Regex = new Regex(@"server.createobject");
                Match Match = Regex.Match(Linea);

                if (Match.Success)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public string ExtraerNombreFuncion(string Linea)
        {
            try
            {
                string Resultado = "";
                Regex Regex = new Regex(@"\w+\s*\(");
                Match Match = Regex.Match(Linea);
                if (Match.Success)
                {
                    Resultado = Match.Value;
                    Resultado = Resultado.Replace("(", "").Trim();
                }
                return Resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return string.Empty;
            }
        }

        public  string ExtraerNombreClase(string Linea)
        {
            try
            {
                string resultado = "";
                Regex Regex = new Regex(@"class\s+(\w+)");
                Match Match = Regex.Match(Linea);
                if (Match.Success)
                {
                    resultado = Match.Value;
                    resultado = resultado.Replace("class", "").Trim();
                }

                Regex RegexFuncion = new Regex(@"\w+\.js");
                Match MatchFuncion = RegexFuncion.Match(Linea);
                if (MatchFuncion.Success)
                {
                    resultado = MatchFuncion.Value;
                }

                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return string.Empty;
            }
        }

            public  string ExtraerNombreEspacio(string Linea)
            {
                try
                {
                    string Resultado = "";
                    Regex Regex = new Regex(@"namespace\s+(\w+)");
                    Match Match = Regex.Match(Linea);
                    if (Match.Success)
                    {
                        Resultado = Match.Value;
                        Resultado = Resultado.Replace("namespace", "").Trim();                        
                    }
                    Regex = new Regex(@"package\s+(\w+)");
                    Match = Regex.Match(Linea);
                    if (Match.Success)
                    {
                        Resultado = Linea.Replace("package", "").Trim();
                        Resultado = Resultado.Replace(";", "").Trim();
                    
                    }
                    return Resultado;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return string.Empty;
                }
            }

            public  string ExtraerNombreInterface(string Linea)
            {
                try
                {
                    string Resultado = "";
                    Regex Regex = new Regex(@"interface\s+(\w+)");
                    Match Match = Regex.Match(Linea);
                    if (Match.Success)
                    {
                        Resultado = Match.Value;
                        Resultado = Resultado.Replace("interface", "").Trim();                    
                    }
                    return Resultado;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return string.Empty;
                }
            }

        public string EncuentraNombre(string cadena)
        {
            string linea = "";

            cadena = cadena.Trim();
            linea = cadena.ToLower();

            if (linea.Equals(""))
                return "";
            else if ((linea.IndexOf("class") >= 0) && (linea.IndexOf(".class") < 0))
            {
                Regex regex = new Regex(@"\s+class\s+");
                Match match = regex.Match(linea);
                if (match.Success)
                {
                    NombreClase = ExtraerNombreClase(cadena);
                    NombreMetodo = "";
                    return NombreClase;
                }
                else
                    return "";
            }
            else
            {
                if (EsMetodo(linea))
                {
                    NombreMetodo = ExtraerNombreMetodo(cadena);
                    return NombreClase;
                }
                else
                {
                    if (NombreMetodo != "")
                        return NombreClase + "," + NombreMetodo;
                    else
                        return NombreClase;
                }
            }
        }

        public void Limpiar()
        {
            NombreClase = "";
            NombreMetodo = "";
        }
    }
}