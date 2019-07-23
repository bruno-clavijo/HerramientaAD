using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Text;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Data;
using DependenciaMVC5demo.com.utilerias;
using DependenciaMVC5demo.com.dto;

namespace DependenciaMVC5demo.com.negocio
{
    public class ParseadorBD:Proceso
    {
        List<ArchivoDto> listaArchivos = new List<ArchivoDto>();
        IEnumerable<ComentarioDto> Comentarios = new List<ComentarioDto>();
        IEnumerable<CadenaAIgnorarDto> _CadenasAIgnorar = new List<CadenaAIgnorarDto>();
        List<CadenaAEncontrarDto> _CadenasAEncontrar = new List<CadenaAEncontrarDto>();
        List<ConexionBDDto> ConexionesBd = new List<ConexionBDDto>();
        List<ObjetoDto> listaHallazgos = new List<ObjetoDto>();
        string separador = ",";
        public string pathsalida = string.Empty;
        //ConfigurationManager.AppSettings["pcsv"].ToString()
        public void GeneraSalida(int CveAplicacion, string Ruta, ProcesoDto pdto)
        {
            ControlLog ErrLog = new ControlLog();
            Entrada Ent = new Entrada();
            AplicacionDto AppDto = new AplicacionDto();
            ExcluyeComentarios ExcCom = new ExcluyeComentarios();
            Procesos Procesos = new Procesos();
            Salida Salida = new Salida();
            DataTable configuracionDt;
            DataRow configuracion;
            string extensiones = "";
            ProcesoAvanceDto pdtoA = new ProcesoAvanceDto();
            Proceso proc = new Proceso();
            string TipoAnalisis = "BD";

            proc.SeteaAvance("En Proceso", "OK", "OK", "1", "--", "", "Iniciando Parseo BD", pdtoA, pdto);
            proc.ActualizaProcesoAvance(pdtoA, pdto);

            //Carga los datos de configuracion la ruta ya se trae, no se utiliza la de configuración
            using (configuracionDt = Consultas(1, CveAplicacion));
            {
                if (configuracionDt == null || configuracionDt.Rows.Count < 1)
                {
                    ErrLog.EscribeLogWS("No se encontraron los datos de configuración de la aplicación");
                    configuracion = null;
                }
                else
                    configuracion = configuracionDt.Rows[0];
            }

            //Determina que extensiones se deben leer deacuerdo al lenguaje
            DataTable LenguajeApp = ConsultaLenguaje(Convert.ToString(CveAplicacion));
            if (LenguajeApp == null || LenguajeApp.Rows.Count < 1)
            {
                ErrLog.EscribeLogWS("No se encontraron los datos de lenguaje de la Aplicación");
                LenguajeApp = null;
            }

                DataRow Lenguaje = LenguajeApp.Rows[0];
                //if (Lenguaje["Lenguaje"].ToString() == "ASP")
                    extensiones = configuracion["Extensiones"].ToString();
                //else
                   // extensiones = configuracion["ExtensionesJava"].ToString();

                String[] extArray = extensiones.ToLower().Split(',');
            

            //Validación
            if (String.IsNullOrEmpty(Convert.ToString(CveAplicacion)))
            {
                ErrLog.EscribeLogWS("No asigno la clave de la aplicación a analizar.");
                return;
            }

            //Obtiene todos los archivos de la ruta que deben leerse se cambio la ruta como parametro
            Ent.ListarArchivos(listaArchivos, Ruta, extArray, Convert.ToString(CveAplicacion));

            //Validación
            if (listaArchivos.Where(l => l.Extension == "java").Count() > 5)
                AppDto.Tecnologia = "JAVA";
            else if (listaArchivos.Where(l => l.Extension == "cs").Count() > 5)
                AppDto.Tecnologia = "NET";
            else if (listaArchivos.Where(l => l.Extension == "asp").Count() > 5)
                AppDto.Tecnologia = "ASP";
            else if (listaArchivos.Where(l => l.Extension == "sql").Count() > 5)
                AppDto.Tecnologia = "Oracle";

            AppDto.CveAplicacion = Convert.ToString(CveAplicacion);

            ////Obtiene lista de marcas de comentarios
            //Comentarios = Ent.CargarComentarios(configuracion["RutaComentarios"].ToString());
            //if (Comentarios == null || !Comentarios.Any())
            //{
            //    ErrLog.EscribeLogWS("El archivo de comentarios esta vacío");
            //    return;
            //}
            //ExcCom.Comentarios = Comentarios;
            //ErrLog.EscribeLogWS("El archivo de comentarios ha sido cargado.");

            //Carga las cadenas que no deben considerarse
            _CadenasAIgnorar = Ent.CargarAIgnorar();
            if (_CadenasAIgnorar == null || !_CadenasAIgnorar.Any())
            {
                ErrLog.EscribeLogWS("No se encontraron cadenas a Ignorar");
                return;
            }
            ErrLog.EscribeLogWS("Cargando cadenas a encontrar...");

            //Obtener las lineas que contienen palabras relacionadas con objetos de la BD
            if (TipoAnalisis == "BD")
            {
                _CadenasAEncontrar = Ent.CargarObjetosBD();
                if (_CadenasAEncontrar == null)
                {
                    ErrLog.EscribeLogWS("No hay cadenas a encontrar, verifique el inventario de objetos");
                    return;
                }
                ErrLog.EscribeLogWS("Las cadenas a encontrar han sido cargadas.");

                if (AppDto.Tecnologia == "NET" || AppDto.Tecnologia == "ASP")
                    ConexionesBd = Ent.EncontrarConexionesNet();

                if (AppDto.Tecnologia == "JAVA")
                    ConexionesBd = Ent.EncontrarConexionesJava();

                if (ConexionesBd == null || ConexionesBd.Count < 1)
                    ErrLog.EscribeLogWS("Dentro de la aplicación no se encontraron conexiones a BD");
                else
                {
                    ErrLog.EscribeLogWS("Se encontraron en la aplicación conexiones a BD");
                    ConexionesBd.ForEach(conexion =>
                    {
                        _CadenasAEncontrar.Add(new CadenaAEncontrarDto
                        {
                            Grupo = conexion.BaseDatos,
                            NombreObjeto = conexion.Nombre,
                            TipoObjeto = "ConexionBD",
                            TipoBusqueda = 0,
                            TipoObjetoID = 7
                        });
                    });
                    _CadenasAEncontrar = Procesos.ReordenarCadenas(_CadenasAEncontrar, ConexionesBd, AppDto);
                }
            }

            Ent.CargarCadenasDesdeBD(_CadenasAEncontrar, TipoAnalisis, AppDto);
            Parseador parser = new Parseador()
            {
                CadenasAIgnorar = _CadenasAIgnorar,
                CadenasAEncontrar = _CadenasAEncontrar,
                RutaAnalisis = Ruta,
                Comentarios = Comentarios,
                ConexionesBd = ConexionesBd
            };
            proc.SeteaAvance("En Proceso", "OK", "OK", "2", "--", "", "Guardando Datos", pdtoA, pdto);
            proc.ActualizaProcesoAvance(pdtoA, pdto);

            try
            {
                listaHallazgos = parser.ProcesarArchivos(listaArchivos, AppDto, pdto);
                
            }
            catch (Exception ex)
            {
                ErrLog.EscribeLogWS("Fallo el procesamiento de archivos: " + ex.Message + " " + ex.StackTrace);
                return;
            }
            StringBuilder csvSalida = new StringBuilder();

            try
            {

                csvSalida.AppendLine("AplicacionID" + separador + "ClaveAplicacion" + separador + "NumLinea" + separador + "Referencia" + separador + "ObjetoBDID" + separador +
                "Objeto" + separador + "TipoObjetoID" + separador + "TipoObjeto" + separador + "BaseDatosID" + separador + "BaseDatos" + separador + "Archivo" + separador
                + "Extension" + separador + "BibPadre" + separador + "ObjPadre");

                ErrLog.EscribeLogWS("Generando salida ...");

                List<string> resultado = new List<string>();
                resultado = Salida.GenerarSalida(separador, csvSalida, listaHallazgos);
                GuardaProcesoBD(pdtoA, pdto, resultado);
            }
            catch (Exception ex)
            {
                ErrLog.EscribeLogWS("Fallo en GenerarSalida " + ex.Message + " " + ex.StackTrace);
                return;
            }
            
                string rutaSalida = pathsalida + pdto.ProcesoID + ".csv";
            try
            {
                //Agregar la carga a la tabla
                File.WriteAllText(rutaSalida + "", csvSalida.ToString());
                ErrLog.EscribeLogWS("Salida CSV '" + rutaSalida + "' generada.");
            }
            catch (Exception ex)
            {
                ErrLog.EscribeLogWS("El archivo de salida CSV no pudo generarse: " + ex.Message);
                return;
            }
            finally
            {
                ErrLog.EscribeLogWS("Proceso terminado");
            }
        }
    }
}