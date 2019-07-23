using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DependenciaMVC5demo.com.utilerias;
using DependenciaMVC5demo.com.dto;
using System.Threading.Tasks;

namespace DependenciaMVC5demo.com.negocio
{
    public class Entrada:Proceso
    {
        ControlLog ErrLog = new ControlLog();
        
        List<ArchivoDto> ArchivosConfiguracionNET = new List<ArchivoDto>();
        List<ArchivoDto> ArchivosConfiguracionJava = new List<ArchivoDto>();
        VerificaComentarios VerCom = new VerificaComentarios();

        public List<CadenaAEncontrarDto> CargarObjetosBD()
        {
            List<CadenaAEncontrarDto> listado = new List<CadenaAEncontrarDto>();

            try
            {
                using (DataTable tabla = Consultas(3, 0))
                {
                    if (tabla == null || tabla.Rows.Count < 1)
                    {
                        ErrLog.EscribeLogWS("No se encontraron Objetos en el inventario... ");
                    }
                    else
                    {
                        foreach (DataRow fila in tabla.Rows)
                        {
                            listado.Add(new CadenaAEncontrarDto()
                            {
                                GrupoID = int.Parse(fila["BaseDatosID"].ToString()),
                                Grupo = fila["BaseDatos"].ToString(),
                                TipoObjetoID = int.Parse(fila["TipoObjetoID"].ToString()),
                                TipoObjeto = fila["TipoObjeto"].ToString(),
                                ObjetoID = int.Parse(fila["ObjetoBDID"].ToString()),
                                NombreObjeto = fila["Objeto"].ToString(),
                                TipoBusqueda = 0
                            });
                        }
                    }
                }

                ErrLog.EscribeLogWS("Cargando Inventario de objetos ...");
                return listado;
            }
            catch (Exception ex)
            {
                ErrLog.EscribeLogWS("No se pudieron cargar los Objetos BD del inventario " + ex.Message);
                return null;
            }
        }

        public void CargarCadenasDesdeBD(List<CadenaAEncontrarDto> listadoObjetos, string tipoAnalisis, AplicacionDto AppDto)
        {
            List<CadenaAEncontrarDto> listado = new List<CadenaAEncontrarDto>();
            try
            {
                //SqlParameter[] Params = new SqlParameter[2];
                //Params[0] = new SqlParameter("@Tecnologia", SqlDbType.VarChar);
                string Tecnologia = AppDto.Tecnologia;
                //Params[1] = new SqlParameter("@Grupos", SqlDbType.VarChar);
                string Grupos;
                if (tipoAnalisis == "WS")
                    Grupos = "webservice";
                else
                    Grupos = "conexionbd";

                using (DataTable tabla = ConsultaPalabras(Tecnologia, Grupos))
                {
                    if (tabla == null || tabla.Rows.Count < 1)
                        ErrLog.EscribeLogWS("No hay mas cadenas a encontrar en la BD...");
                    else
                    {
                        foreach (DataRow fila in tabla.Rows)
                        {
                            if (!listado.Exists(x => x.TipoObjeto == fila["TipoPalabra"].ToString() && x.NombreObjeto == fila["PalabraClave"].ToString()))
                            {
                                int temp = 0;
                                listado.Add(new CadenaAEncontrarDto()
                                {
                                    Grupo = fila["GRUPO"].ToString(),
                                    TipoObjeto = fila["TipoPalabra"].ToString(),
                                    NombreObjeto = fila["PalabraClave"].ToString(),
                                    TipoBusqueda = int.TryParse(fila["TipoBusqueda"].ToString(), out temp) ? (TipoBusqueda)Convert.ToInt32(temp) : 0,
                                    Tecnologia = fila["TECNOLOGIA"].ToString()
                                });
                            }
                        }
                    }
                }
                ErrLog.EscribeLogWS("Cargando palabras a buscar desde la BD.");
                listadoObjetos.AddRange(listado);

            }
            catch (Exception ex)
            {
                ErrLog.EscribeLogWS("No se pudieron cargar los palabras buscar desde la BD" + ex.Message);
            }

        }

        public IEnumerable<CadenaAIgnorarDto> CargarAIgnorar()
        {
            List<CadenaAIgnorarDto> listado = new List<CadenaAIgnorarDto>();
            int tipoBusqueda = 0;
            try
            {
                using (DataTable tabla = Consultas(2, 0))
                {
                    if (tabla == null || tabla.Rows.Count < 1)
                    {
                        ErrLog.EscribeLogWS("No se encontraron Cadenas a Ignorar...");
                    }
                    else
                    {
                        foreach (DataRow fila in tabla.Rows)
                        {
                            //elimina duplicados
                            if (!listado.Exists(x => x.Cadena == fila["Exclusion"].ToString()))
                            {
                                listado.Add(new CadenaAIgnorarDto()
                                {
                                    Cadena = fila["Exclusion"].ToString(),
                                    //TipoBusqueda = (secciones.Length == 1)? LugarBusqueda.Cualquiera : (LugarBusqueda)Convert.ToInt32(secciones[1])
                                    TipoBusqueda = int.TryParse(fila["TipoBusqueda"].ToString(), out tipoBusqueda) ? (LugarBusqueda)Convert.ToInt32(tipoBusqueda) : 0
                                    //TipoBusqueda = (secciones.Length == 1) ? LugarBusqueda.Inicio : (LugarBusqueda)tipoBusqueda
                                });
                            }
                        }
                        ErrLog.EscribeLogWS("Cargando Cadenas a Ignorar...");
                    }
                }
                return listado.AsEnumerable();
            }
            catch (Exception ex)
            {
                ErrLog.EscribeLogWS("Ocurrió un problema al cargar las cadenas a ignorar: " + ex.Message + " " + ex.StackTrace.ToString());
                return null;
            }
        }

        public void ListarArchivos(List<ArchivoDto> lista, string ruta, string[] extensiones, string cveApp)
        {
            ErrLog.EscribeLogWS("Exploración en '" + ruta);
            foreach (string filepath in Directory.GetFiles(ruta))
            {
                FileInfo fileInfo = new FileInfo(filepath);

                if (fileInfo.Extension.ToLower() == ".config" || fileInfo.Extension.ToLower() == ".asp")
                {
                    ArchivosConfiguracionNET.Add(new ArchivoDto()
                    {
                        Nombre = fileInfo.Name,
                        Ruta = filepath,
                        Extension = fileInfo.Extension.ToLower().TrimStart('.'),
                        CveAplicacion = cveApp
                    });
                }

                if (fileInfo.Extension.ToLower() == ".xml" || fileInfo.Extension.ToLower() == ".java")
                {
                    ArchivosConfiguracionJava.Add(new ArchivoDto()
                    {
                        Nombre = fileInfo.Name,
                        Ruta = filepath,
                        Extension = fileInfo.Extension.ToLower().TrimStart('.'),
                        CveAplicacion = cveApp
                    });
                }
                //verificamos si el archivo esta dentro de las extensiones a analizar antes
                //pero antes le borramos el caracter punto que es parte de la extension
                if (extensiones.Contains(fileInfo.Extension.ToLower().TrimStart('.')))
                {
                    lista.Add(new ArchivoDto()
                    {
                        Nombre = fileInfo.Name,
                        Ruta = filepath,
                        Extension = fileInfo.Extension.ToLower().TrimStart('.'),
                        CveAplicacion = cveApp
                    });
                    ErrLog.EscribeLogWS("Archivo a analizar: " + fileInfo.FullName);

                }
            }

                foreach (string dir in Directory.GetDirectories(ruta))
                {
                    ListarArchivos(lista, dir, extensiones, cveApp);
                }
        }


        public List<ComentarioDto> CargarComentarios(string rutaArchivo)
        {
            List<ComentarioDto> comentarios = new List<ComentarioDto>();
            try
            {
                using (StreamReader sr = new StreamReader(rutaArchivo))
                {
                    while (sr.Peek() >= 0)
                    {
                        string linea = sr.ReadLine().Trim().ToLower();
                        if (string.IsNullOrEmpty(linea)) continue;

                        string[] secciones = linea.Split(',');
                        if (secciones.Length == 4)
                        {
                            comentarios.Add(new ComentarioDto()
                            {
                                Tipo = secciones[0].Trim(),
                                Inicio = secciones[1].Trim(),
                                Fin = secciones[2].Trim(),
                                Extension = secciones[3].Trim()
                            });
                        }
                    }
                }
                ErrLog.EscribeLogWS("Cargando Comentarios...");
                return comentarios;
            }
            catch (Exception ex)
            {
                ErrLog.EscribeLogWS("El archivo '" + rutaArchivo + "' no pudo ser leido: " + ex.Message);
                return null;
            }
        }


        public List<ConexionBDDto> EncontrarConexionesNet()
        {
            ExcluyeComentarios ExcCom = new ExcluyeComentarios();
            int NumLinea;
            List<ConexionBDDto> ListadoConexiones = new List<ConexionBDDto>();
            foreach (ArchivoDto archivo in ArchivosConfiguracionNET)
            {
                //si el nombre del archivo tiene numeros entonces exluimos el archivo porque es un duplicado
                Match m = Regex.Match(archivo.Nombre, @"\d+");
                if (m.Success)
                    continue;

                if (archivo.Nombre.IndexOf("CambiaPwd") > -1)
                    NumLinea = 0;

                NumLinea = 1;
                VerCom.ComentarioBloque = "";

                using (StreamReader sr = new StreamReader(archivo.Ruta))
                {
                    while (sr.Peek() >= 0)
                    {
                        string linea = sr.ReadLine().Trim();
                        string lineaCodigo = linea.ToLower();
                        if (!String.IsNullOrEmpty(lineaCodigo))
                        {
                            lineaCodigo = VerCom.BuscarComentario(lineaCodigo);

                            if (!string.IsNullOrEmpty(lineaCodigo))
                            {
                                string nombreBd = "", nombreConexion = "", nombreServidor = "";
                                if (lineaCodigo.IndexOf("connectionstring") >= 0)
                                {

                                    string[] patrones = { @"database\s*=\s*(\w+)", @"initial\s*catalog\s*=\s*(\w+)\s*(\w*)", "name\\s*=\\s*\"(\\w+)\"", @"server\s*=\s*\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}", @"data source=\w+", @"provider=\w+" };
                                    int index = 0;
                                    foreach (var patron in patrones)
                                    {
                                        Match match = Regex.Match(lineaCodigo, patron);
                                        if (match.Success)
                                        {
                                            switch (index)
                                            {
                                                case 0:
                                                    nombreBd = match.Value;
                                                    nombreBd = nombreBd.Replace("database", string.Empty).Replace("=", string.Empty).Trim();
                                                    break;
                                                case 1:
                                                    nombreBd = match.Value;
                                                    nombreBd = nombreBd.Replace("initial", string.Empty).Replace("catalog", string.Empty).Replace("=", string.Empty).Trim();
                                                    break;
                                                case 2:
                                                    nombreConexion = match.Value;
                                                    nombreConexion = nombreConexion.Replace("name", string.Empty).Replace("=", string.Empty).Replace("\"", string.Empty).Trim();
                                                    break;
                                                case 3:
                                                    nombreServidor = match.Value;
                                                    nombreServidor = nombreServidor.Replace("server", string.Empty).Replace("=", string.Empty).Trim();
                                                    break;
                                                case 4:
                                                    nombreBd = match.Value;
                                                    nombreBd = nombreBd.Replace("data source", string.Empty).Replace("=", string.Empty).Trim();
                                                    break;
                                                case 5:
                                                    nombreConexion = match.Value;
                                                    nombreConexion = nombreConexion.Replace("provider", string.Empty).Replace("=", string.Empty).Replace("\"", string.Empty).Trim();
                                                    break;
                                            }

                                        }
                                        index++;
                                    }
                                    if (!string.IsNullOrEmpty(nombreBd) && !string.IsNullOrEmpty(nombreConexion))
                                    {
                                        if (!ListadoConexiones.Exists(x => x.BaseDatos == nombreBd && x.Nombre == nombreConexion))
                                        {
                                            ListadoConexiones.Add(new ConexionBDDto
                                            {
                                                BaseDatos = nombreBd,
                                                Nombre = nombreConexion,
                                                Servidor = nombreServidor
                                            });
                                        }
                                    }
                                }
                            }
                        }
                        NumLinea++;
                    }
                }
            }
            return ListadoConexiones;
        }

        public List<ConexionBDDto> EncontrarConexionesJava()
        {
            ExcluyeComentarios ExcCom = new ExcluyeComentarios();
            ErrLog.EscribeLogWS("Buscando conexiones a BD...");
            int NumLinea;
            List<ConexionBDDto> ListadoConexiones = new List<ConexionBDDto>();
            foreach (ArchivoDto archivo in ArchivosConfiguracionJava)
            {
                //si el nombre del archivo tiene numeros entonces exluimos el archivo porque es un duplicado
                Match m = Regex.Match(archivo.Nombre, @"\d+");
                if (m.Success || archivo.Nombre.ToLower().IndexOf("copy") >= 0)
                    continue;

                NumLinea = 1;
                VerCom.ComentarioBloque = "";

                using (StreamReader sr = new StreamReader(archivo.Ruta))
                {
                    while (sr.Peek() >= 0)
                    {
                        string linea = sr.ReadLine().Trim();
                        string lineaCodigo = linea.ToLower();

                        if (!String.IsNullOrEmpty(lineaCodigo))
                        {
                            //lineaCodigo = ExcCom.BuscarComentario(lineaCodigo, archivo.Extension);
                            lineaCodigo = VerCom.BuscarComentario(lineaCodigo);

                            if (!string.IsNullOrEmpty(lineaCodigo) && lineaCodigo.IndexOf("jdbc") >= 0)
                            {
                                string nombreBd = "", nombreConexion = "";
                                Match match = Regex.Match(lineaCodigo, @"jdbc:\w+:|jdbc\/\w+");
                                if (match.Success)
                                {
                                    string[] patrones = { @"database=\w+\s*\w+", @"databasename=\w+", @"jdbc\/\w+", @"[^jdbc]\/\w+[^\.{0}]" };
                                    int index = 0;
                                    foreach (var patron in patrones)
                                    {
                                        Match mat = Regex.Match(lineaCodigo, patron);
                                        if (mat.Success)
                                        {
                                            switch (index)
                                            {
                                                case 0:
                                                    nombreBd = mat.Value;
                                                    nombreBd = nombreBd.Replace("database", string.Empty).Replace("=", string.Empty).Trim();
                                                    break;
                                                case 1:
                                                    nombreBd = mat.Value;
                                                    nombreBd = nombreBd.Replace("databasename", string.Empty).Replace("=", string.Empty).Trim();
                                                    break;
                                                case 2:
                                                    nombreConexion = mat.Value;
                                                    break;
                                                case 3:
                                                    nombreBd = mat.Value;
                                                    nombreBd = nombreBd.Replace("/", string.Empty).Replace("\"", string.Empty).Trim();
                                                    break;
                                            }
                                            //en el primer match true salimos
                                            break;
                                        }
                                        index++;
                                    }

                                    if (!string.IsNullOrEmpty(nombreBd) || !string.IsNullOrEmpty(nombreConexion))
                                    {
                                        if (!ListadoConexiones.Exists(x => x.BaseDatos == nombreBd && x.Nombre == nombreConexion))
                                        {
                                            ListadoConexiones.Add(new ConexionBDDto
                                            {
                                                BaseDatos = nombreBd,
                                                Jndi = nombreConexion,
                                                Nombre = nombreConexion
                                            });
                                        }
                                    }
                                }
                            }
                        }
                        NumLinea++;
                    }
                }
            }
            return ListadoConexiones;
        }
    }
}
