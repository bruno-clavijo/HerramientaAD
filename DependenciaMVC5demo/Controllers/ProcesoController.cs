using System;
using System.Linq;
using System.Configuration;
using System.Xml;
using System.IO;
using System.Text;
using System.Web.Mvc;
using DependenciaMVC5demo.com.negocio;
using DependenciaMVC5demo.com.dto;
using DependenciaMVC5demo.com.utilerias;
using DependenciaMVC5demo.Models;
using System.Threading;
using System.Collections.Generic;

namespace DependenciaMVC5demo.Controllers
{
    public class ProcesoController : Controller
    {
        Thread hilo;
        ThreadStart sth;
        // GET: Proceso
        public ActionResult Index()
        {
            ProcesoModel xproc = new ProcesoModel();
            xproc.Xprocesos = ObtenProcesoAvance(0);
            Menu Menu = new Menu();
            ViewBag.XMLMenu = Menu.ComponeMenu(int.Parse(Session["usuid"].ToString()), Session["pfnum"].ToString());
            return View(xproc);
        }

        private void LlamarWS()
        {
            ProcesoDto ProcDto = new ProcesoDto();

            WSParseador objwsp = new WSParseador();
            try
            {
                ProcDto.UsuarioID = int.Parse(ViewData["UsuarioID"].ToString());
                ProcDto.ProcesoID = long.Parse(ViewData["ProcesoID"].ToString());
                ProcDto.AplicacionID = int.Parse(ViewData["AplicacionID"].ToString());
                objwsp.ProcesarAplicacion(ProcDto, ViewData["ruta"].ToString(), ViewData["rutaDestno"].ToString(), ViewData["rutaCSV"].ToString());    
            } catch (Exception)
            {
            }
        }

        public JsonResult ActualizaApps(int areaid)
        {
            Aplicacion appobj = new Aplicacion();
            List<Utilerias.ItemsCombo> aplicacioneslist = new List<Utilerias.ItemsCombo>();

            if (appobj.ObtenAplicacion(1, areaid))
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

        private XmlDocument ObtenProcesoAvance(long ProcesoID)
        {
            Proceso proc = new Proceso();
            XmlDocument xpross= new XmlDocument();
            try
            {
                if (proc.ObtenProcesoAvance(ProcesoID, int.Parse(Session["usuid"].ToString())))
                    xpross = proc.PAvanceXML;
            }
            catch (Exception err)
            {
                proc.EscribeLog("ObtenProcesoAvance ObtenProcesoAvance " + err.Message.ToString());
            }
            finally
            {
                proc = null;
            }
            return xpross;
        }

        [HttpPost]
        public string MultiUpload(string id, string fileName)
        {
            var chunkNumber = id;
            var chunks = Request.InputStream;
            string path = Server.MapPath(ConfigurationManager.AppSettings["cup"].ToString() + "temp/");
            string newpath = Path.Combine(path, fileName + chunkNumber);
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

        public ActionResult Descarga(string proceso)
        {
            try
            {
                string fullName = Server.MapPath(ConfigurationManager.AppSettings["pcsv"].ToString() + proceso + ".csv");
                byte[] fileBytes = GetFile(fullName);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "ProcesoDB" + proceso + ".csv");
            }
            catch (Exception) {
                string mensaje = "Esta en ejecución un parseo para el proceso: " + proceso + ", es necesario esperar los resultados.";
                return File(Encoding.ASCII.GetBytes(mensaje), System.Net.Mime.MediaTypeNames.Application.Octet, "ProcesoDB" + proceso + ".csv"); ;
            }
            
        }

        byte[] GetFile(string s)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new System.IO.IOException(s);
            return data;
        }

        public ActionResult Eliminar(string proceso)
        {
            Proceso proc = new Proceso();
            ProcesoModel xproc = new ProcesoModel();
            proc.EliminaProceso(int.Parse(Session["usuid"].ToString()), long.Parse(proceso));            
            xproc.Xprocesos = ObtenProcesoAvance(0);

            return RedirectToAction("Index");
        }

        public ActionResult ActualizaConsulta() {
            ProcesoModel xproc = new ProcesoModel();
            xproc.Xprocesos = ObtenProcesoAvance(0);
            return PartialView("Consulta",xproc);
        }

        [HttpPost]
        public string UploadComplete(string fileName, string complete, string appid)
        {
            ControlLog ctrl = new ControlLog();
            try
            {
                string tempPath = Server.MapPath(ConfigurationManager.AppSettings["cup"].ToString() + "temp/");
                string videoPath = Server.MapPath(ConfigurationManager.AppSettings["codigoUP"].ToString());
                string newPath = Path.Combine(tempPath, fileName);
                if (complete == "1")
                {
                    string[] filePaths = Directory.GetFiles(tempPath).Where(p => p.Contains(fileName)).OrderBy(p => Int32.Parse(p.Replace(fileName, "$").Split('$')[1])).ToArray();
                    foreach (string filePath in filePaths)
                    {
                        MergeFiles(newPath, filePath);
                    }
                }
                System.IO.File.Move(Path.Combine(tempPath, fileName), Path.Combine(videoPath, fileName));
                actualizaStatus(appid, fileName);
            }catch (Exception exe)
            {
                ctrl.EscribeLog("actualizaStatus.UploadComplete " + exe.Message.ToString());
            }
            return "success";
        }

        private void actualizaStatus(string appid,string filename) {
            Proceso proc = new Proceso();
            ProcesoDto procdto = new ProcesoDto();
            try {
                procdto.UsuarioID = int.Parse(Session["usuid"].ToString());
                procdto.AplicacionID = int.Parse(appid);
                procdto.ProcesoID = 0;
                procdto.ProcesoID = proc.GuardaProceso(procdto);
                if (procdto.ProcesoID > 0)
                {
                    ViewData["ProcesoID"] = procdto.ProcesoID.ToString();
                    ViewData["AplicacionID"] = procdto.AplicacionID.ToString();
                    ViewData["UsuarioID"] = Session["usuid"].ToString();
                    ViewData["ruta"] = System.Web.HttpContext.Current.Request.MapPath(ConfigurationManager.AppSettings["codigoUP"].ToString()) + filename;
                    ViewData["rutaDestno"] = System.Web.HttpContext.Current.Request.MapPath(ConfigurationManager.AppSettings["codigoUN"].ToString() + procdto.ProcesoID.ToString() + "-" + procdto.UsuarioID.ToString() + "-" + procdto.AplicacionID.ToString() + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss"));
                    ViewData["rutaCSV"] = System.Web.HttpContext.Current.Request.MapPath(ConfigurationManager.AppSettings["pcsv"].ToString());
                    sth = new ThreadStart(this.LlamarWS);
                    hilo = new Thread(sth);
                    hilo.Start();
                    TempData["ProcesoID"] = procdto.ProcesoID;
                    ViewData["ProcesoID"] = procdto.ProcesoID;
                    ViewBag["ProcesoID"] = procdto.ProcesoID;
                }                
            }
            catch (Exception exe)
            {
                proc.EscribeLog("actualizaStatus.UploadComplete " + exe.Message.ToString());
            }
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

        // GET: Proceso/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Proceso/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Proceso/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Proceso/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Proceso/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Proceso/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Proceso/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }      

    }
}
