using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Xml;
using DependenciaMVC5demo.com.dto;


namespace DependenciaMVC5demo.com.negocio
{
    public class RevisaInterfaces
    {
        InterfacesDto IntDto = new InterfacesDto();
        EncuentraMetodo EncMet = new EncuentraMetodo();
        private List<string> ListaClases = new List<string>();
        Proceso Proceso = new Proceso();
        public int DependenciaID = 0;

        public InterfacesDto ObtenerInventario(string LineaCodigo, ProcesoDto pdto)
        {
            try
            {
                InterfacesDto IntDto = new InterfacesDto();
                LineaCodigo = LineaCodigo.Trim();
                string Linea = LineaCodigo.ToLower();

                //Extraer IP
                Regex Regex = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
                Match Match = Regex.Match(Linea);
                if (Match.Success)
                {
                    Regex = new Regex(@"add key=""\w+.\w+");
                    Match = Regex.Match(LineaCodigo);
                    if (Match.Success)
                    {
                        string Nombre = Match.Value.Replace(@"""", string.Empty);
                        IntDto.Nombre = Nombre.Replace("add key=", string.Empty);
                    }

                    Regex = new Regex(@"\bhttp://\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\S+\b");
                    Match = Regex.Match(Linea);
                    if (Match.Success)
                    {
                        string IP = Match.Value;
                        IntDto.IP = IP.Trim();
                    }

                    if (!string.IsNullOrEmpty(IntDto.Nombre) && !string.IsNullOrEmpty(IntDto.IP))
                    {
                        IntDto.Tipo = "Out";
                    }

                    if (!string.IsNullOrEmpty(IntDto.IP))
                    {
                        if (Proceso.ObtenMiddleware(pdto.UsuarioID))
                        {
                            XmlDocument consultaxml = Proceso.PAvanceXML;
                            foreach (XmlNode Fila in consultaxml.DocumentElement.SelectSingleNode("Middleware").SelectNodes("row"))
                            {
                                if (IntDto.IP.IndexOf(Fila.Attributes["IP"].Value.ToString()) > -1)
                                {
                                    IntDto.Middleware = Fila.Attributes["Middleware"].Value.ToString();
                                    break;
                                }
                            }
                        }
                    }
                }

                //Extraer URL
                Regex RegexURL = new Regex(@"(http|ftp|https):\/\/([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?");
                Match MatchURL = RegexURL.Match(Linea);
                if (MatchURL.Success)
                {
                    IntDto.Nombre = MatchURL.Value;
                    IntDto.IP = MatchURL.Value;
                    IntDto.Tipo = "Externo";
                    IntDto.Middleware = "";
                }

                    return IntDto;
            }
            catch (Exception ex)
            {
                throw new Exception("RevisaInterfaces.ObtenerInventario ", ex);
            }
        }

        public RevisaInterfaces ObtenerSalidaWS(string LineaCodigo, string Archivo, HashSet<string> InventarioWS,
            List<string> Resultado, int NoLinea, string Ruta)
        {
            try
            {
                RevisaInterfaces RevisaInterfaces = new RevisaInterfaces();
                string LineaOriginal = LineaCodigo;
                LineaCodigo = LineaCodigo.ToLower();
                LineaCodigo = LineaCodigo.Replace("\t", " ");
                LineaOriginal = LineaOriginal.Replace("\t", " ");

                string LenguajeApp = Regex.Replace(Archivo, @"\w+\.", string.Empty).Trim();
                Archivo = Regex.Replace(Archivo, @"\.\w+", string.Empty).Trim();

                //Para WebServices dentro de la Aplicación
                Regex RegexWS = new Regex(@"\w+\s*:\s*system.web.services.webservice");
                Match MatchWS = RegexWS.Match(LineaCodigo);
                if (MatchWS.Success)
                {
                    ++DependenciaID;
                    string Servicio = MatchWS.Value.Split(':').First().Trim();
                    Resultado.Add(DependenciaID + "|" + NoLinea + "|" + LineaOriginal.Replace("|", string.Empty) + "|" + Ruta + "|" + LenguajeApp + "|" + "Interno" +"|" + "" + "|" + "Archivo" +"|" + Archivo +"|" + "Web Service" + "|" + Servicio);
                }


                //Para los WebServices que estan fuera de la Aplicación 
                if ((LineaCodigo.IndexOf("class ") >= 0) && (LineaCodigo.IndexOf(".class") < 0))
                {
                    bool Excepcion = EsExcepcion(LineaCodigo.ToLower());
                    if (!Excepcion)
                    {
                        ObtenerDatosClase(LineaOriginal);
                    }
                }

                foreach(string Servicio in InventarioWS)
                {
                    string Nombre = Servicio.Split('|').ElementAt(0);
                    string Direccion = Servicio.Split('|').ElementAt(1);
                    string Middleware = Servicio.Split('|').ElementAt(3);
                    RegexWS = new Regex(@"" + Nombre);
                    MatchWS = RegexWS.Match(LineaOriginal);
                    if (MatchWS.Success)
                    {
                        string Clase = "";
                        string Interface = "Interface";
                        if (ListaClases.Count() > 0)
                            Clase = ListaClases.Last();
                        else
                        { 
                            Clase = Archivo;
                            Interface = "Página Externa";
                        }
                        ++DependenciaID;
                        Resultado.Add(DependenciaID + "|" + NoLinea + "|" + LineaOriginal.Replace("|", string.Empty) + "|" + Ruta + "|" + LenguajeApp + "|" + Direccion + "|" + Middleware + "|" + Clase + "|" + Archivo + "|" + Interface + "|" + Nombre);
                    }

                }
                    return RevisaInterfaces;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public bool EsExcepcion(string LineaCodigo)
        {
            bool Exc = false;
            string Linea = string.IsNullOrEmpty(LineaCodigo) ? string.Empty : LineaCodigo.ToLower();
            string[] Excepciones = new string[] { "throw new sqlexception", "throw new exception", "logger.info",
                "logger.error", "logger.debug", "writeerrorlog", "using (", ".append", "else if", "byte[]",
                "static final string" };

            foreach (string Excepcion in Excepciones)
            {
                if (Linea.IndexOf(Excepcion) > -1)
                    Exc = true;
            }

            return Exc;
        }

        //Obtener los datos de la Clase actual
        private void ObtenerDatosClase(string LineaOriginal)
        {
            try
            {
                string Clase = EncMet.ExtraerNombreClase(LineaOriginal);

                if (!string.IsNullOrEmpty(Clase))
                {
                    ListaClases.Add(Clase);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}