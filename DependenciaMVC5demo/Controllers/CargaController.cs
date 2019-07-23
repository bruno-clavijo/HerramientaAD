using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Configuration;
using DependenciaMVC5demo.Models;
using DependenciaMVC5demo.com.utilerias;
using System.Web.Mvc;

namespace DependenciaMVC5demo.Controllers
{
    public class CargaController : Controller
    {
        // GET: Carga
        public ActionResult Index()
        {
            Menu Menu = new Menu();
            ViewBag.XMLMenu = Menu.ComponeMenu(int.Parse(Session["usuid"].ToString()), Session["pfnum"].ToString());
            return View(new CargaModel());
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

        [HttpPost]
        public string UploadComplete(string fileName, string complete)
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
            return "success";
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

        // GET: Carga/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Carga/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Carga/Create
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

        // GET: Carga/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Carga/Edit/5
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

        // GET: Carga/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Carga/Delete/5
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
