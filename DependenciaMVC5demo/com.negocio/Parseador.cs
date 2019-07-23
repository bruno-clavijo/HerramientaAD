using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using DependenciaMVC5demo.com.utilerias;
using DependenciaMVC5demo.com.dto;

namespace DependenciaMVC5demo.com.negocio
{
    public class Parseador
    {
        ControlLog ErrLog = new ControlLog();
        ExcluyeComentarios ExcCom = new ExcluyeComentarios();
        VerificaComentarios VerCom = new VerificaComentarios();
        EncuentraMetodo EncMet = new EncuentraMetodo();

        public List<ObjetoDto> Objetos = new List<ObjetoDto>();
        public List<ConexionBDDto> ConexionesBd { get; set; }
        public string RutaAnalisis;
        private string ClaseMetodo = "";
        public List<CadenaAEncontrarDto> CadenasAEncontrar { get; set; }
        public IEnumerable<CadenaAIgnorarDto> CadenasAIgnorar = new List<CadenaAIgnorarDto>();
        private ArchivoDto ArchivoActual { get; set; }

        private List<ArchivoDto> Archivos { get; set; }
        public IEnumerable<ComentarioDto> Comentarios { get; set; }

        int NumLinea;

        //Este metodo se encarga de comparar linea por linea del archivo con las cadenas
        //a encontrar e ignorar
        public List<ObjetoDto> ProcesarArchivos(List<ArchivoDto> listaArchivos, AplicacionDto AppDto, ProcesoDto pdto)
        {
            double total = listaArchivos.Count();
            double avance = 38 / total;
            ProcesoAvanceDto pdtoA = new ProcesoAvanceDto();
            Proceso proc = new Proceso();

            //if (AppDto.Tecnologia == "NET")
            //    EncontrarWsNet(listaArchivos);
            //else
            //    EncontrarWsJava(listaArchivos);

            int numArchivo = 1;
            Archivos = listaArchivos;
         
            foreach (ArchivoDto archivo in listaArchivos)
            //Parallel.ForEach(listaArchivos, archivo => 
            {
                ErrLog.EscribeLogWS("Procesando archivo: " + numArchivo + " de " + listaArchivos.Count + " " + archivo.Ruta);
                VerCom.ComentarioBloque = "";
                EncMet.Limpiar();

                using (StreamReader sr = new StreamReader(archivo.Ruta))
                {
                    NumLinea = 1;
                    while (sr.Peek() >= 0)
                    {
                        string lineaCodigo = sr.ReadLine().Trim();

                        if (!String.IsNullOrEmpty(lineaCodigo))
                        {
                            //lineaCodigo = ExcCom.BuscarComentario(lineaCodigo, archivo.Extension);
                            lineaCodigo = VerCom.BuscarComentario(lineaCodigo);
                            if (archivo.Extension == "cs" || archivo.Extension == "java")
                                ClaseMetodo = EncMet.EncuentraNombre(lineaCodigo);
                            else
                                ClaseMetodo = "";
                            
                            if (!Excluir(lineaCodigo) && !string.IsNullOrEmpty(lineaCodigo))
                            {
                                ArchivoActual = archivo;

                                if (archivo.Extension == "aspx")
                                {
                                    if (lineaCodigo.ToLower().IndexOf("command") >= 0)
                                        EncontrarCadena(lineaCodigo);
                                }
                                else
                                    EncontrarCadena(lineaCodigo);
                            }
                        }
                        NumLinea++;
                    }
                }
                numArchivo++;
                proc.SeteaAvance("En Proceso", "OK", "OK", Math.Round(2 + (avance * numArchivo),0).ToString(), "--", "", "Leyendo Archivos", pdtoA, pdto);
                proc.ActualizaProcesoAvance(pdtoA, pdto);
            }
            return Objetos;
        }

        public bool Excluir(string lineaCodigo)
        {
            bool resultado = false;
            foreach (CadenaAIgnorarDto stringToFind in CadenasAIgnorar)
            {
                lineaCodigo = lineaCodigo.ToLower();
                string lowerString = stringToFind.Cadena.ToLower();

                switch (stringToFind.TipoBusqueda)
                {
                    case LugarBusqueda.Inicio:
                        if (lineaCodigo.StartsWith(lowerString))
                            resultado = true;
                        break;
                    case LugarBusqueda.Cualquiera:
                        resultado = lineaCodigo.Contains(lowerString);
                        break;
                    case LugarBusqueda.ExpresionRegular:
                        break;
                }
                if (resultado) break;
            }
            return resultado;
        }


        public void EncontrarCadena(string lineaCodigo)
        {
            //foreach (CadenaAEncontrar stringToFind in CadenasAEncontrar)
            List<CadenaAEncontrarDto> cadenas = new List<CadenaAEncontrarDto>();
            cadenas.AddRange(CadenasAEncontrar);
            if (ArchivoActual.Extension == "config")
            {
                cadenas.Clear();
                cadenas.AddRange(CadenasAEncontrar.Where(c => c.TipoObjeto == "ConexionBD"));
            }

            if (ArchivoActual.Nombre == "ConsultaEstadosSTN.asmx")
                 ErrLog.EscribeLogWS("buscando cadenas");

            //Parallel.ForEach(cadenas, stringToFind =>
            foreach (CadenaAEncontrarDto stringToFind in CadenasAEncontrar)
            {
                string lowerLinea = lineaCodigo.ToLower();
                string lowerString = stringToFind.NombreObjeto.ToLower();

                if (stringToFind.TipoBusqueda == TipoBusqueda.ExpresionRegular)
                {
                    Match m = Regex.Match(lowerLinea, lowerString);
                    if (m.Success)
                        LlenarObjetos(lineaCodigo, stringToFind);
                }
                else
                {
                    //Este if es indispensable para aumentar la velocidad del analisis
                    if (lowerLinea.IndexOf(lowerString) >= 0)
                    {
                        Regex regex = new Regex(@"([^/\\\w<>{}-]|^)" + lowerString + @"([^/\\\w<>{}-]|$)");
                        switch (stringToFind.TipoBusqueda)
                        {
                            case TipoBusqueda.PalabraCompleta:
                                Match match = regex.Match(lowerLinea);
                                if (match.Success)
                                {
                                    if (stringToFind.TipoObjeto == "ConexionBD")
                                    {
                                        //verificamos que no sea un from porque hay nombres de tablas iguales alos de la base de datos
                                        if (lineaCodigo.IndexOf("from") < 0)
                                        {
                                            ArchivoActual.ConexionBd = new ConexionBDDto
                                            {
                                                BaseDatos = stringToFind.Grupo,
                                                Nombre = stringToFind.NombreObjeto
                                            };
                                        }
                                    }
                                    LlenarObjetos(lineaCodigo, stringToFind);
                                }
                                break;
                            case TipoBusqueda.Generica:
                                if (lowerLinea.Contains(lowerString))
                                    LlenarObjetos(lineaCodigo, stringToFind);
                                break;
                        }
                    }
                }
            }
        }


        public void LlenarObjetos(string lineaCodigo, CadenaAEncontrarDto stringToFind)
        {
            string[] secciones;
            string Archivo = "";
            string regexString = stringToFind.NombreObjeto.ToLower() + @"\.get|" + stringToFind.NombreObjeto.ToLower() + @"\.set";
            Regex regex = new Regex(regexString);
            Match match = regex.Match(lineaCodigo.ToLower());

            if (!match.Success)
            {
                // if(string.IsNullOrEmpty(ClaseMetodo))
                secciones = ClaseMetodo.Split(',');
                if (string.IsNullOrEmpty(secciones[0]))
                {
                    Archivo = Path.GetFileName(ArchivoActual.Ruta);
                    Archivo = Regex.Replace(Archivo, @"\.\w+", string.Empty).Trim();
                }

                Objetos.Add(new ObjetoDto()
                {
                    BaseDatosID = stringToFind.GrupoID,
                    NombreBd = stringToFind.Grupo,
                    ObjetoID = stringToFind.ObjetoID,
                    NombreObjeto = stringToFind.NombreObjeto,
                    TipoID = stringToFind.TipoObjetoID,
                    Tipo = stringToFind.TipoObjeto,
                    NumLinea = NumLinea,
                    Referencia = lineaCodigo.Replace("\"", string.Empty).Trim(),
                    Archivo = ArchivoActual.Ruta,
                    Lenguaje = ArchivoActual.Lenguaje,
                    CveAplicacion = ArchivoActual.CveAplicacion,
                    BibPadre = (secciones.Length > 1) ? secciones[0] : Archivo,
                    ObjPadre = (secciones.Length > 1) ? secciones[1] : Archivo,
                    //NombreBd = (ArchivoActual.ConexionBd != null) ? ArchivoActual.ConexionBd.BaseDatos : "" 
                });
            }
        }

        public void EncontrarWsNet(List<ArchivoDto> listaArchivos)
        {
            ErrLog.EscribeLogWS("Buscando WebServices...");
            foreach (ArchivoDto archivo in listaArchivos.Where(a => a.Extension == "wsdl" || a.Extension == "cs"))
            {
                
                using (StreamReader sr = new StreamReader(archivo.Ruta))
                {
                    ArchivoActual = archivo;

                    while (sr.Peek() >= 0)
                    {
                        string lineaCodigo = sr.ReadLine().Trim();
                        //lineaCodigo = ExcCom.BuscarComentario(lineaCodigo, archivo.Extension);
                        lineaCodigo = VerCom.BuscarComentario(lineaCodigo);

                        if (!String.IsNullOrEmpty(lineaCodigo))
                        {
                            EncontrarCadena(lineaCodigo);
                        }
                    }
                }
            }

            if (Objetos != null)
            {
                foreach(ObjetoDto obj in Objetos)
                {
                    string nombre = "";
                    
                    if (obj.Tipo == "IN")
                    {
                        Match match = Regex.Match(obj.Referencia, @"class\s+\w+");
                        if (match.Success)
                        {
                            nombre = match.Value.Replace("class", string.Empty).Trim();
                            CadenasAEncontrar.Add(new CadenaAEncontrarDto
                            {
                                Grupo = "WebService",
                                NombreObjeto = nombre,
                                TipoObjeto = "IN",
                                TipoBusqueda = TipoBusqueda.PalabraCompleta
                            });
                        }
                    }
                    else if (obj.Tipo == "OUT")
                    {
                        Match match = Regex.Match(obj.Referencia, @"=\s*\w*s*");
                        if (match.Success)
                        {
                            nombre = match.Value.Replace("=", string.Empty).Trim();
                            CadenasAEncontrar.Add(new CadenaAEncontrarDto
                            {
                                Grupo = "WebService",
                                NombreObjeto = nombre,
                                TipoObjeto = "OUT",
                                TipoBusqueda = TipoBusqueda.PalabraCompleta
                            });
                        }
                    }
                }
            }
            Objetos.Clear();            
        }


        public void EncontrarWsJava(List<ArchivoDto> listaArchivos)
        {
            ErrLog.EscribeLogWS("Buscando WebServices...");
            foreach (ArchivoDto archivo in listaArchivos.Where(a => a.Extension == "java"))
            {                
                using (StreamReader sr = new StreamReader(archivo.Ruta))
                {
                    ArchivoActual = archivo;

                    while (sr.Peek() >= 0)
                    {
                        string lineaCodigo = sr.ReadLine().Trim();
                        //lineaCodigo = ExcCom.BuscarComentario(lineaCodigo, archivo.Extension);
                        lineaCodigo = VerCom.BuscarComentario(lineaCodigo);

                        if (!String.IsNullOrEmpty(lineaCodigo))
                            EncontrarCadena(lineaCodigo);

                    }
                }
            }

            if (Objetos != null)
            {
                foreach (ObjetoDto obj in Objetos)
                {
                    string nombre = "";

                    if (obj.Tipo == "OUT" || obj.Tipo == "IN")
                    {
                        Match match = Regex.Match(obj.Referencia, @"name\s*=\s*\w+");
                        if (match.Success)
                        {
                            nombre = match.Value.Replace("name", string.Empty).Replace("=", string.Empty).Replace("\"", string.Empty).Trim();
                            CadenasAEncontrar.Add(new CadenaAEncontrarDto
                            {
                                Grupo = "WebService",
                                NombreObjeto = nombre,
                                TipoObjeto = "OUT",
                                TipoBusqueda = TipoBusqueda.PalabraCompleta
                            });
                        }
                    }
                }
            }
            Objetos.Clear();
        }



    }
}