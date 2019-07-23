using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data;
using System.Web.Services;
using System.Configuration;
using DependenciaMVC5demo.com;
using DependenciaMVC5demo.com.utilerias;
using DependenciaMVC5demo.com.dto;
using DependenciaMVC5demo.com.negocio;

namespace DependenciaMVC5demo
{
    /// <summary>
    /// Summary description for WSClasesMetodos
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WSParseador : System.Web.Services.WebService
    {
        ControlLog ErrLog = new ControlLog();
        private string ultMensajeError;

        [WebMethod]
        public string ObtenUltMensajeError()
        {
            return ultMensajeError;

        }

        private bool validadatos(int UsuarioID, long ProcesoID, string Ruta)
        {
            bool respuesta = false;
            try
            {
                if (UsuarioID > 0 && ProcesoID > 0 && Ruta.Length > 0)
                {
                    if (Directory.Exists(Ruta))
                    {
                        respuesta = true;
                    }
                    else
                    {
                        ultMensajeError = "El archivo no existe en la ruta especificada.";
                        ErrLog.EscribeLog("WSDependencias.validadatos El archivo no existe en la ruta especificada. " + Ruta);
                    }

                }
                else ultMensajeError = "Los datos de entrada del proceso son incorrectos";
            }
            catch (Exception Err)
            {
                ErrLog.EscribeLogWS("WSDependencias.validadatos " + Err.Message.ToString());
            }
            return respuesta;
        }

        [WebMethod]
        public bool ProcesarDependenciasBD(ProcesoDto ProcDto, string Ruta, string csv)
        {
            bool respuesta = false;
            ParseadorBD PBD = new ParseadorBD();
            ProcesoAvanceDto pdtoA = new ProcesoAvanceDto();
            Proceso proc = new Proceso();

            try
            {
                if (validadatos(ProcDto.UsuarioID, ProcDto.ProcesoID, Ruta))
                {
                    PBD.pathsalida = csv;
                    PBD.GeneraSalida(ProcDto.AplicacionID, Ruta, ProcDto);
                    proc.SeteaAvance("En Proceso", "OK", "OK", "40", "40", "", "Parseo BD Terminado", pdtoA, ProcDto);
                    proc.ActualizaProcesoAvance(pdtoA, ProcDto);
                    respuesta = true;
                }

            }
            catch (Exception Err)
            {
                ErrLog.EscribeLogWS("WSBaseDatosWebService.ProcesaDependenciasBD " + Err.Message.ToString());
            }
            return respuesta;

        }

        [WebMethod]
        public bool ProcesarDependenciasWS(ProcesoDto ProcDto, string Ruta)
        {
            bool respuesta = false;
            ParseadorWS PWS = new ParseadorWS();
            ProcesoAvanceDto pdtoA = new ProcesoAvanceDto();
            Proceso proc = new Proceso();
            try
            {
                if (validadatos(ProcDto.UsuarioID, ProcDto.ProcesoID, Ruta))
                {
                     DataTable LenguajeApp = proc.ConsultaLenguaje(Convert.ToString(ProcDto.AplicacionID));
                    if (LenguajeApp == null || LenguajeApp.Rows.Count < 1)
                    {
                        ErrLog.EscribeLogWS("No se encontraron los datos de lenguaje de la Aplicación");
                        LenguajeApp = null;
                    }
                    else
                    {
                        DataRow Lenguaje = LenguajeApp.Rows[0];
                        string Tipo = Lenguaje["Extension"].ToString();
                        if (Tipo == "cs")
                            Tipo = "*.config";
                        else if (Tipo == "java")
                            Tipo = "*.wsdl";
                        else if (Tipo == "asp")
                            Tipo = "*.asp";

                        string[] Archivos = Directory.GetFiles(Ruta, Tipo, SearchOption.AllDirectories);

                        HashSet<string> InventarioWS = new HashSet<string>();

                        proc.SeteaAvance("En Proceso", "OK", "OK", "40", "40", "", "Iniciando Parseo WS", pdtoA, ProcDto);
                        proc.ActualizaProcesoAvance(pdtoA, ProcDto);

                        for (int i = 0; i <= Archivos.Count() - 1; i++)
                        {
                            InventarioWS = PWS.GenerarInventarioWS(Archivos[i], ProcDto, Tipo);
                        }

                        proc.SeteaAvance("En Proceso", "OK", "OK", "42", "40", "", "Inventario de WS Generado", pdtoA, ProcDto);
                        proc.ActualizaProcesoAvance(pdtoA, ProcDto);

                        Tipo = "*." + Lenguaje["Extension"].ToString();
                        Archivos = Directory.GetFiles(Ruta, Tipo, SearchOption.AllDirectories);

                        PWS.GenerarSalidaWS(InventarioWS, Archivos, ProcDto.AplicacionID.ToString(), ProcDto);
                        proc.SeteaAvance("En Proceso", "OK", "OK", "70", "70", "", "Parseo WS Terminado", pdtoA, ProcDto);
                        proc.ActualizaProcesoAvance(pdtoA, ProcDto);
                        respuesta = true;
                    }
                }
            }
            catch (Exception Err)
            {
                ErrLog.EscribeLogWS("WSInterfacesWebService.ProcesaDependenciasWS " + Err.Message.ToString());
            }
            return respuesta;

        }

        [WebMethod]
        public bool ProcesarDependenciasCM(ProcesoDto ProcDto, string Ruta)
        {
            bool respuesta = false;
            ParseadorCM PCM = new ParseadorCM();
            Proceso proc = new Proceso();
            try
            {
                if (validadatos(ProcDto.UsuarioID, ProcDto.ProcesoID, Ruta))
                {
                    string Extensiones;
                    DataTable ExtensionesApp = proc.Consultas(1, ProcDto.AplicacionID);
                    if (ExtensionesApp == null || ExtensionesApp.Rows.Count < 1)
                    {
                        ErrLog.EscribeLogWS("No se encontraron los datos de lenguaje de la Aplicación");
                        ExtensionesApp = null;
                    }
                    else
                    {
                        DataRow Lenguaje = ExtensionesApp.Rows[0];
                        Extensiones = Lenguaje["Extensiones"].ToString();

                        string[] extArray = Extensiones.ToLower().Split(',');
                        List<string> Archivos = new List<string>();

                        foreach (string Extension in extArray)
                        {
                            string tipo = "*." + Extension;
                            string[] ArchivosArray = Directory.GetFiles(Ruta, tipo, SearchOption.AllDirectories);
                            for (int i = 0; i <= ArchivosArray.Count() - 1; i++)
                            {
                                Archivos.Add(ArchivosArray[i].ToString());
                            }
                        }

                        HashSet<string> InventarioCM = new HashSet<string>();

                        foreach (string Archivo in Archivos)
                        {
                            InventarioCM = PCM.GenerarInventarioCM(Archivo, ProcDto);
                        }
                        ProcesoAvanceDto pdtoA = new ProcesoAvanceDto();
                        proc.SeteaAvance("En Proceso", "OK", "OK", "72", "70", "", "Inventario CM Generado", pdtoA, ProcDto);
                        proc.ActualizaProcesoAvance(pdtoA, ProcDto);

                        PCM.GenerarSalidaCM(InventarioCM, Archivos, ProcDto.AplicacionID.ToString(), ProcDto);
                        proc.SeteaAvance("Terminado", "OK", "OK", "OK", "OK", "", "Parseo Terminado", pdtoA, ProcDto);
                        proc.ActualizaProcesoAvance(pdtoA, ProcDto);
                        respuesta = true;
                    }
                }

            }
            catch (Exception Err)
            {
                ErrLog.EscribeLogWS("WSDependencias.ProcesaDependenciasCM " + Err.Message.ToString());
            }
            return respuesta;

        }

        [WebMethod]
        public bool Descomprimir(ProcesoDto ProcDto,string rutaorigen, string rutadestino)
        {
            bool respuesta = false;
            try
            {
                ZipControl unzip = new ZipControl();
                unzip.Origen = rutaorigen;
                unzip.Destino = rutadestino;
                unzip.DescmprimirArchivos();
                respuesta = true;
            }
            catch (Exception Err)
            {
                ErrLog.EscribeLogWS("WSDependencias.Descomprimir " + Err.Message.ToString());
            }
            return respuesta;
        }

        [WebMethod]
        public bool ProcesarAplicacion(ProcesoDto ProcDto,string origen, string destino, string csv ) {
            bool respuesta = false;
            try
            {             
                EliminarParseo(ProcDto);
                if (Descomprimir(ProcDto, origen, destino))
                {                    
                    ProcesarDependenciasBD(ProcDto, destino, csv);                    
                    ProcesarDependenciasWS(ProcDto, destino);
                    ProcesarDependenciasCM(ProcDto, destino);
                }
            }
            catch (Exception Err)
            {
                Proceso proceso = new Proceso();
                ProcesoAvanceDto pdtoA = new ProcesoAvanceDto();
                proceso.SeteaAvance("En Proceso", "OK", "OK", "X", "X", "", "Parseo Fallido", pdtoA, ProcDto);
                proceso.ActualizaProcesoAvance(pdtoA, ProcDto);
                ErrLog.EscribeLogWS("WSDependencias.ProcesarAplicacion " + Err.Message.ToString());
            }
            return respuesta;
        }

        [WebMethod]
        public bool EliminarParseo(ProcesoDto ProcDto)
        {
            bool respuesta = false;
            try
            {
                Proceso Proc = new Proceso();
                if (Proc.EliminarParseo(ProcDto, 1))
                    respuesta = true;
            }
            catch (Exception Err)
            {
                ErrLog.EscribeLogWS("WSDependencias.EliminarParseo " + Err.Message.ToString());
            }
            return respuesta;
        }
    }
}
