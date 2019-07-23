using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;
using DependenciaMVC5demo.com.dto;

namespace DependenciaMVC5demo.com.negocio
{
    public class ParseadorWS : Proceso
    {
        HashSet<string> InventarioWS = new HashSet<string>();
        List<string> Resultado = new List<string>();
        RevisaInterfaces RI = new RevisaInterfaces();
        InterfacesDto IntDto = new InterfacesDto();
        VerificaComentarios VerCom = new VerificaComentarios();

        public HashSet<string> GenerarInventarioWS(string Ruta, ProcesoDto pdto, string Tipo)
        {
            try
            {
                using (StreamReader sr = new StreamReader(Ruta))
                {
                    IntDto.Nombre = string.Empty;
                    IntDto.IP = string.Empty;
                    IntDto.Middleware = string.Empty;
                    while (sr.Peek() >= 0)
                    {
                        string LineaCodigo = sr.ReadLine().Trim();

                        //if (LineaCodigo.IndexOf("http://www.presidencia.gob.mx") > -1)
                        //    LineaCodigo = LineaCodigo;

                        if (!String.IsNullOrEmpty(LineaCodigo))
                        {
                            // Los datos para los servicios de C# se encuentran dentro de la misma línea
                            if (Tipo.IndexOf("config") > -1)
                            {
                                IntDto = RI.ObtenerInventario(LineaCodigo, pdto);
                                if (!string.IsNullOrEmpty(IntDto.Nombre) && !string.IsNullOrEmpty(IntDto.IP))
                                {
                                    InventarioWS.Add(IntDto.Nombre + "|" + IntDto.IP + "|" + IntDto.Tipo + "|" + IntDto.Middleware);
                                }
                            }

                            // Los datos para los servicios de C# se encuentran dentro de la misma línea
                            if (Tipo.IndexOf("asp") > -1)
                            {
                                IntDto = RI.ObtenerInventario(LineaCodigo, pdto);
                                if (!string.IsNullOrEmpty(IntDto.Nombre) && !string.IsNullOrEmpty(IntDto.IP))
                                {
                                    InventarioWS.Add(IntDto.Nombre + "|" + IntDto.IP + "|" + IntDto.Tipo + "|" + IntDto.Middleware);
                                }
                            }

                            //Los datos para los servicios Java se encuentran en varias líneas
                            if (Tipo.IndexOf("wsdl") > -1)
                            {
                                //string Linea = LineaCodigo.ToLower();
                                Regex Regex = new Regex(@"service name=""\w+");
                                Match Match = Regex.Match(LineaCodigo);
                                if (Match.Success)
                                {
                                    string Nombre = Regex.Replace(Match.Value, @"service name=", string.Empty);
                                    Nombre = Nombre.Replace(@"""", string.Empty).Replace("Service", string.Empty);
                                    IntDto.Nombre = Nombre.Trim();
                                }

                                Regex = new Regex(@"\bhttp://\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\S+\b");
                                Match = Regex.Match(LineaCodigo);
                                if (Match.Success)
                                {
                                    string IP = Match.Value;
                                    IntDto.IP = IP.Trim();
                                }

                                if (!string.IsNullOrEmpty(IntDto.IP))
                                {
                                    if (ObtenMiddleware(pdto.UsuarioID))
                                    {
                                        XmlDocument consultaxml = PAvanceXML;
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
                        }
                    }

                    if (!string.IsNullOrEmpty(IntDto.Nombre) && !string.IsNullOrEmpty(IntDto.IP))
                    {
                        InventarioWS.Add(IntDto.Nombre + "|" + IntDto.IP + "|" + IntDto.Tipo + "|" + IntDto.Middleware);
                    }
                }
            }
            catch (Exception Err)
            {
                EscribeLogWS("ParseadorWS.GenerarInventarioWS " + Err.Message.ToString());
                ProcesoAvanceDto pdtoA = new ProcesoAvanceDto();
                Proceso proc = new Proceso();
                proc.SeteaAvance("Error", "OK", "X", "--", "--", Err.Message.ToString(), "Error al realizar la descompresión del archivo", pdtoA, pdto);
                proc.ActualizaProcesoAvance(pdtoA, pdto);
            }
            return InventarioWS;
        }

        public void GenerarSalidaWS(HashSet<string> InventarioWS, string[] Archivos, string App, ProcesoDto pdt)
        {
            ProcesoAvanceDto pdtoA = new ProcesoAvanceDto();
            Proceso proc = new Proceso();
            double total = Archivos.Count();
            double avance = 28 / total;

            for (int i = 0; i <= Archivos.Count() - 1; i++)
            {

                //Aquí van a ir los parametros para iniciar
                string Ruta = Archivos[i];
                string Archivo = Path.GetFileName(Ruta);
                int NoLinea = 0;
                RevisaInterfaces RevInt = new RevisaInterfaces();

                //Empezar a leer el archivo
                using (StreamReader sr = new StreamReader(Ruta))
                {
                    while (sr.Peek() >= 0)
                    {
                        string lineaCodigo = sr.ReadLine().Trim();

                        //Contar No. Linea
                        ++NoLinea;

                        //if (NoLinea == 106)
                        //    NoLinea = NoLinea;

                        if (!String.IsNullOrEmpty(lineaCodigo))
                        {
                            //Actualizar el nuevo VerificaComentarios
                            lineaCodigo = VerCom.BuscarComentario(lineaCodigo);
                            if (!String.IsNullOrEmpty(lineaCodigo))
                            {
                                RevInt.ObtenerSalidaWS(lineaCodigo, Archivo, InventarioWS, Resultado, NoLinea, Ruta);
                            }
                        }
                    }
                }
                proc.SeteaAvance("En Proceso", "OK", "OK", Math.Round((42 + avance * i),0).ToString(), "40", "", "Leyendo Archivos", pdtoA, pdt);
                proc.ActualizaProcesoAvance(pdtoA, pdt);
            }

            GuardaProcesoWS(pdtoA, pdt, Resultado);
            //System.IO.File.WriteAllLines(@"C:\INFONAVIT\ClasesMetodos.txt", Resultado);
        }
    }
}