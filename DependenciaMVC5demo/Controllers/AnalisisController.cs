using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Web.Mvc;
using System.Data;
using System.Text.RegularExpressions;
using System.IO;
using System.Configuration;
using Microsoft.VisualBasic.FileIO;
using DependenciaMVC5demo.com.utilerias;
using DependenciaMVC5demo.Models;
using DependenciaMVC5demo.com.negocio;
using DependenciaMVC5demo.com.dto;

namespace DependenciaMVC5demo.Controllers
{
    public class AnalisisController : Controller
    {
        // GET: Analisis
        public ActionResult Index()
        {
            Menu Menu = new Menu();
            ViewBag.XMLMenu = Menu.ComponeMenu(int.Parse(Session["usuid"].ToString()), Session["pfnum"].ToString());
            AnalisisModel xan = new AnalisisModel();            
            return View(xan);
        }

        public JsonResult Limpiar()
        {
            return Json("");
        }

        public JsonResult ActualizaFiltros(string Filtro, string Tipo, int AplicacionID, string Filtro1, string Filtro2, string Filtro3)
        {
            Aplicacion appobj = new Aplicacion();
            List<Utilerias.ItemsCombo> ObjdbList = new List<Utilerias.ItemsCombo>();

            //int.Parse(Session["usuid"].ToString())

            if (appobj.ObtenFiltros(Filtro, Tipo, AplicacionID, Filtro1, Filtro2, Filtro3))
            {
                XmlNode select = appobj.AplicaionXML.DocumentElement.SelectSingleNode("Filtros");
                ObjdbList.Add(new Utilerias.ItemsCombo(0, "--select--"));
                foreach (XmlNode area in select.SelectNodes("row"))
                {
                    ObjdbList.Add(new Utilerias.ItemsCombo(int.Parse(area.Attributes["Numero"].Value.ToString()), area.Attributes["Nombre"].Value.ToString()));
                }
                
            }
            return Json(new SelectList(ObjdbList, "Ivalue", "Text"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ActualizaApps(int areaid)
        {
            Aplicacion appobj = new Aplicacion();
            List<Utilerias.ItemsCombo> aplicacioneslist = new List<Utilerias.ItemsCombo>();

            if (appobj.ObtenAplicacion(int.Parse(Session["usuid"].ToString()), areaid))
            {
                XmlNode select = appobj.AplicaionXML.DocumentElement.SelectSingleNode("Aplicaciones");
                aplicacioneslist.Add(new Utilerias.ItemsCombo(0, "--select--"));
                foreach (XmlNode area in select.SelectNodes("row"))
                {
                    aplicacioneslist.Add(new Utilerias.ItemsCombo(int.Parse(area.Attributes["AplicacionID"].Value.ToString()), area.Attributes["Aplicacion"].Value.ToString()));
                }

            }

            return Json(new SelectList(aplicacioneslist, "Ivalue", "Text"), JsonRequestBehavior.AllowGet);
        }            

        public ActionResult Descarga(string proceso)
        {
            return View();
        }

        public ActionResult ActualizaConsulta(string Tipo, int AplicacionID, string Filtro1, string Filtro2, string Filtro3, string Filtro4)
        {
            
            AnalisisModel xan = new AnalisisModel();
            xan.Xdetalle = ObtenDetalle(Tipo, AplicacionID, Filtro1, Filtro2, Filtro3, Filtro4);
            return PartialView("ConsultaDetalle", xan);
        }

        public XmlDocument ObtenDetalle(string Tipo, int AplicacionID, string Filtro1, string Filtro2, string Filtro3, string Filtro4)
        {
            XmlDocument xdet = new XmlDocument();
            ConsultaDep consultaobj = new ConsultaDep();
            if (consultaobj.ConsultaDependenciaFiltro(Tipo, AplicacionID, Filtro1, Filtro2, Filtro3, Filtro4))
            {
                xdet = consultaobj.ConsultaXML;
            }
            return xdet;
        }

        private static void MergeFiles(string file1, string file2)
        {
            FileStream fs1 = null;
            FileStream fs2 = null;
            try
            {
                fs1 = System.IO.File.Open(file1, FileMode.Append);
                fs2 = System.IO.File.Open(file2, FileMode.Open);
                byte[] fs2Content = new byte[fs2.Length];
                fs2.Read(fs2Content, 0, (int)fs2.Length);
                fs1.Write(fs2Content, 0, (int)fs2.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " : " + ex.StackTrace);
            }
            finally
            {
                if (fs1 != null) fs1.Close();
                if (fs2 != null) fs2.Close();
                System.IO.File.Delete(file2);
            }
        }

        [HttpPost]
        public string MultiUpload(string id, string fileName)
        {
            var chunkNumber = id;
            var chunks = Request.InputStream;
            string path = Server.MapPath(ConfigurationManager.AppSettings["cup"].ToString() + "temp/");
            string newpath = Path.Combine(path, fileName);
            using (FileStream fs = System.IO.File.Create(newpath))
            {
                byte[] bytes = new byte[1048576];
                int bytesRead;
                while ((bytesRead = Request.InputStream.Read(bytes, 0, bytes.Length)) > 0)
                {
                    fs.Write(bytes, 0, bytesRead);
                }
            }
            return "done";
        }

        [HttpPost]
        public string UploadComplete(string fileName, string complete, string appid)
        {
            ControlLog ctrl = new ControlLog();
            try
            {
                string tempPath = Server.MapPath(ConfigurationManager.AppSettings["cup"].ToString() + "temp/");
                string newPath = Path.Combine(tempPath, fileName);
                IniciaCargaCSV(newPath, 1);
                System.IO.File.Delete(newPath);
            }
            catch (Exception exe)
            {
                ctrl.EscribeLog("actualizaStatus.UploadComplete " + exe.Message.ToString());
            }
            return "success";
        }

        public JsonResult IniciaCargaCSV(string Ruta, int UsuarioID)
        {
            ProcesoDto ProDto = new ProcesoDto();
            ControlLog Log = new ControlLog();
            ProcesoAvanceDto ProcADto = new ProcesoAvanceDto();
            Proceso Proc = new Proceso();
            ObjetoDto ObjDto = new ObjetoDto();
            bool respuesta = false;
            bool ValidaLayOut = true;
            int DependenciaID = 0;
            DataTable DatosParseo;
            DataRow DatoProceso;
            List<string> Resultado = new List<string>();

            try
            {
                using (TextFieldParser Lector = new TextFieldParser(Ruta))
                {
                    Lector.SetDelimiters(new string[] { "," });
                    Lector.HasFieldsEnclosedInQuotes = true;

                    //Lector.ReadLine();

                    string[] LayOut = new string[] { "AplicacionID", "ClaveAplicacion", "NumLinea", "Referencia", "ObjetoBDID", "Objeto", "TipoObjetoID", "TipoObjeto", "BaseDatosID", "BaseDatos", "Archivo", "Extension", "BibPadre", "ObjPadre" };

                    while (!Lector.EndOfData)
                    {
                        string[] LineaActual = Lector.ReadFields();
                        if (Lector.LineNumber == 2)
                        {
                            for (int i = 0; i < LineaActual.Length; i++)
                            {
                                if (LineaActual[i].ToString() != LayOut[i].ToString())
                                    ValidaLayOut = false;
                            }
                        }
                        else
                        {
                            if (ValidaLayOut)
                            {
                                ObjDto.CveAplicacion = LineaActual[1].ToString();
                                ObjDto.BibPadre = LineaActual[12].ToString();
                                ObjDto.ObjPadre = LineaActual[13].ToString();
                                ObjDto.BaseDatosID = int.Parse(LineaActual[8]);
                                ObjDto.ObjetoID = int.Parse(LineaActual[4]);
                                ObjDto.TipoID = int.Parse(LineaActual[6]);
                                ObjDto.NumLinea = int.Parse(LineaActual[2]);
                                ObjDto.Referencia = LineaActual[3].ToString();
                                ObjDto.Archivo = LineaActual[10].ToString();
                                ObjDto.Lenguaje = LineaActual[11].ToString();

                                string Archivo = Path.GetFileName(ObjDto.Archivo);
                                Archivo = Regex.Replace(Archivo, @"\.\w+", string.Empty).Trim();
                                ProDto.AplicacionID = int.Parse(ObjDto.CveAplicacion);
                                ++DependenciaID;
                                Resultado.Add(DependenciaID + "¡" + ObjDto.CveAplicacion + "¡" + (string.IsNullOrEmpty(ObjDto.BibPadre) ? Archivo : ObjDto.BibPadre) + "¡" + " " + "¡" + (string.IsNullOrEmpty(ObjDto.ObjPadre) ? Archivo : ObjDto.ObjPadre) + "¡" + ObjDto.BaseDatosID.ToString() + "¡" + ObjDto.ObjetoID.ToString() + "¡" + ObjDto.TipoID.ToString() + "¡" + ObjDto.NumLinea.ToString() + "¡" + ObjDto.Referencia + "¡" + ObjDto.Archivo + "¡" + ObjDto.Lenguaje);
                            }
                            else
                            {
                                Log.EscribeLog("LayOut incorrecto del archivo .csv.");
                                return null;
                            }
                        }
                    }

                    if (Resultado.Count >= 1 && ProDto.AplicacionID > 0)
                    {
                        DatosParseo = Proc.ConsultaProceso(ProDto.AplicacionID);
                        if (DatosParseo.Rows.Count >= 1)
                        {
                            DatoProceso = DatosParseo.Rows[0];
                            ProDto.UsuarioID = UsuarioID;
                            ProDto.ProcesoID = long.Parse(DatoProceso["ProcesoID"].ToString());

                            Proc.EliminarParseo(ProDto, 0);
                            Proc.GuardaProcesoBD(ProcADto, ProDto, Resultado);
                        }
                        else
                        {
                            Log.EscribeLog("No se realizo la carga completa del archivo .csv");
                            DatosParseo = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.EscribeLog("No existe un ProcesoID cargado. No es posible cargar el archivo " + Path.GetFileName(Ruta) );
                return null;
            }

            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }
    }
}