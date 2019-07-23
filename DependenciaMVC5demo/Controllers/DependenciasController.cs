using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Text;
using System.Xml;
using System.IO;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
using System.Web.Mvc;
using System.Web;
using System.Windows.Forms;
using System.Drawing.Imaging;
using DependenciaMVC5demo.Models;
using DependenciaMVC5demo.com.negocio;
using DependenciaMVC5demo.com.dto;
using DependenciaMVC5demo.com.utilerias;
using Menu = DependenciaMVC5demo.com.utilerias.Menu;

namespace MVCDependencias5.Controllers
{

    public class DependenciasController : Controller
    {        
        // GET: /Dependencias/
        List<Utilerias.diagramElem> de = new List<Utilerias.diagramElem>();
        List<Utilerias.LineDiagramaDB> ldbd = new List<Utilerias.LineDiagramaDB>();

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

        public string GeneraDiagramaDependenciaDB(int appid, string nomapp)
        {
            int maxe = 100;
            Aplicacion appobj = new Aplicacion();
            string nomapli = TempData["nomapp"].ToString();
            TempData.Keep("nomapp");
            string diagramahtml = string.Empty;
            int xini = 0, xfin = 0, yini = 0, yfin = 0, xinc = 0, yinc = 0;
            string dim = string.Empty;
            DiagramaDimension(maxe, ref xini, ref xfin, ref yini, ref yfin, ref xinc, ref yinc, ref dim);
            ldbd.Clear();

            int ide_padre = 49;
            if (dim.Equals("2"))
                ide_padre = 21;
            if (appobj.ObtenRelacionDepDB(int.Parse(Session["usuid"].ToString()), appid, nomapp))
            {
                int contador = 0;
                for (int x = xini; x < xfin; x += xinc)
                {
                    for (int y = yini; y < yfin; y += yinc)
                    {
                        de.Add(new Utilerias.diagramElem(contador, x, y, string.Empty, 0, 0, 0, ""));
                        contador++;
                    }
                }
                XmlNode select = appobj.AplicaionXML.DocumentElement.SelectSingleNode("ObjetosDB");

                de[ide_padre].name = nomapli; de[ide_padre].ocp = 1; de[ide_padre].idepadre = -1; de[ide_padre].ispadre = 1;
                ldbd.Add(new Utilerias.LineDiagramaDB(-1, ide_padre, "", "", ""));
                ide_padre++;
                de[ide_padre].name = nomapp; de[ide_padre].ocp = 1; de[ide_padre].idepadre = (ide_padre - 1); de[ide_padre].ispadre = 1;
                ldbd.Add(new Utilerias.LineDiagramaDB((ide_padre - 1), ide_padre, "", "", ""));
                int ide_hijo = 0;
                foreach (XmlNode padre in select.SelectNodes("row"))
                {
                    ide_hijo = EncuentraHijoAmigo(ide_padre, de, xini);
                    de[ide_hijo].name = padre.Attributes["Objeto"].Value.ToString(); de[ide_hijo].ocp = 1; de[ide_hijo].idepadre = ide_padre;
                    de[ide_hijo].info = padre.Attributes["codigo"].Value.ToString();
                    ldbd.Add(new Utilerias.LineDiagramaDB(ide_hijo, ide_padre, padre.Attributes["PK_Nombre"].Value.ToString(), padre.Attributes["ColumnaPadre"].Value.ToString(), padre.Attributes["ColumnaHijo"].Value.ToString()));                    
                }
                diagramahtml = ArmandoDIagramaBD(de, ldbd);
            }            
            TempData["appid"] = appid;
            TempData["ControlDiv"] = de;
            TempData["DiagBD"] = 1;
            TempData["xmlActual"] = appobj.AplicaionXML;
            TempData["LineasDB"] = ldbd;

            return diagramahtml;
        }

        public JsonResult ActualizaDiagrama(string xn, string yn, string id, int maxe)
        {
            string diagramahtml = string.Empty;
            int dbd = (int)TempData["DiagBD"];
            TempData.Keep("DiagBD");
            if (id.Contains("xplod"))
            {
                string[] dto = id.Split('-');
                if (dbd == 0)
                    de = GeneraDiagramaDependencia(dto[0], dto[1], maxe);
                else
                {
                    diagramahtml = GeneraDiagramaDependenciaDB(int.Parse(dto[0]), dto[1]);
                    return Json(diagramahtml, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                de = (List<Utilerias.diagramElem>)TempData["ControlDiv"];
                try
                {
                    int idchange = int.Parse(id.Remove(0, 8));
                    de[idchange].top = (int)double.Parse(yn);
                    de[idchange].left = (int)double.Parse(xn);
                }
                catch (Exception Err)
                {
                    if (id.Contains("label"))
                    {
                        int idchange = int.Parse(id.Remove(0, 5));
                        de[idchange].top = (int)double.Parse(yn);
                        de[idchange].left = (int)double.Parse(xn);
                    }
                    else
                    {
                        ControlLog clog = new ControlLog();
                        clog.EscribeLog("Dependencias.ActualizaDiagrama" + Err.Message.ToString());
                    }
                }
            }
           
            if (dbd == 0)
                diagramahtml = ArmandoDIagrama(de, maxe);
            else
            {
                ldbd = (List<Utilerias.LineDiagramaDB>)TempData["LineasDB"];
                diagramahtml = ArmandoDIagramaBD(de,ldbd);
                TempData.Keep("LineasDB");                              
            }
            TempData["ControlDiv"] = de;
            return Json(diagramahtml, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GeneraArbol(int idpadre, int idapp) {
            string arbol = string.Empty;
            ConsultaDep consultaobj = new ConsultaDep();                        
            try
            {
                if (consultaobj.VerificaDatosConsulta())
                {                   
                    Proceso procesoobj = new Proceso();
                    ProcesoDto prodto = new ProcesoDto();
                    prodto.AplicacionID = idapp;
                    prodto.ProcesoID = 50;
                    prodto.UsuarioID = 1;
                    if (procesoobj.ObtenProcesoJerarquia(prodto))
                    {
                        arbol=CargaTree(procesoobj);
                    }
                 }                                                     
            }
            catch (Exception err)
            {
                consultaobj.EscribeLogWS("DependenciasController.GeneraArbol " + err.Message.ToString());                
            }
            finally
            {
                consultaobj = null;
            }
            return Json(arbol, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GeneraDiagramaDetalle(int appid,string nombre,int maxe)
        {
            string nomapp = TempData["nomapp"].ToString();
            TempData.Keep("nomapp");            
            Aplicacion appobj = new Aplicacion();
            string diagramahtml = string.Empty;
            int xini = 0, xfin = 0, yini = 0, yfin = 0, xinc = 0, yinc = 0;
            string dim = string.Empty;
            DiagramaDimension(maxe, ref xini, ref xfin, ref yini, ref yfin, ref xinc, ref yinc, ref dim);
            int ide_padre = 49;
            if (dim.Equals("2"))
                ide_padre = 21;

            if (appobj.ObtenObjetosDB3(1, appid, nombre))
            {
                int contador = 0;
                for (int x = xini; x < xfin; x += xinc)
                {
                    for (int y = yini; y < yfin; y += yinc)
                    {
                        de.Add(new Utilerias.diagramElem(contador, x, y, string.Empty, 0, 0, 0, ""));
                        contador++;
                    }
                }
                ValidaXMLDiagrama(appobj.AplicaionXML, "ObjetosDB",1, maxe, false);
                XmlNode select = appobj.AplicaionXML.DocumentElement.SelectSingleNode("ObjetosDB");                
                de[ide_padre].name = nomapp; de[ide_padre].ocp = 1; de[ide_padre].idepadre = -1; de[ide_padre].ispadre = 1;
                ide_padre++;
                de[ide_padre].name = ""; de[ide_padre].ocp = 1; de[ide_padre].idepadre = (ide_padre-1); de[ide_padre].ispadre = 1;
                int ide_hijo = 0;
                foreach (XmlNode area in select.SelectNodes("row[@visible='true']"))
                {
                    ide_hijo = EncuentraHijoAmigo(ide_padre, de, xini);
                    de[ide_hijo].name = area.Attributes["Objeto"].Value.ToString(); de[ide_hijo].ocp = 1; de[ide_hijo].idepadre = ide_padre;
                }
                diagramahtml = ArmandoDIagrama(de,maxe);
            }

            TempData["ControlDiv"] = de;
            TempData["xmlActual"] = appobj.AplicaionXML;
            return Json(diagramahtml, JsonRequestBehavior.AllowGet);
        }

        private List<Utilerias.diagramElem> GeneraDiagramaDependencia(string ide, string name, int maxe) {
            Aplicacion appobj = new Aplicacion();
            string nomapp = TempData["nomapp"].ToString();
            int appid=(int)TempData["appid"];
            int tipoid=(int)TempData["tipoid"];
            int transversal = (int)TempData["transversal"];
            TempData.Keep("transversal");TempData.Keep("DiagBD"); TempData.Keep("nomapp");TempData.Keep("tipoid"); TempData.Keep("appid");
            string diagramahtml = string.Empty;
            int xini = 0, xfin = 0, yini = 0, yfin = 0, xinc = 0, yinc = 0;
            string dim = string.Empty;
            bool result = false;
            DiagramaDimension(maxe, ref xini, ref xfin, ref yini, ref yfin, ref xinc, ref yinc, ref dim);
            int ide_padre = 49;
            if (dim.Equals("2"))
                ide_padre = 21;

            if (transversal == 1)
            {
                string indicador= TempData["indicador"].ToString();
                TempData.Keep("indicador");
                result = appobj.ObtenTransversalidad(int.Parse(Session["usuid"].ToString()), appid, tipoid,indicador,3, name);
            }
            else
                result = appobj.ObtenObjetosDB4(int.Parse(Session["usuid"].ToString()), appid, name, tipoid);
            if (result)
            {
                int contador = 0;
                for (int x = xini; x < xfin; x += xinc)
                {
                    for (int y = yini; y < yfin; y += yinc)
                    {
                        de.Add(new Utilerias.diagramElem(contador, x, y, string.Empty, 0, 0, 0,""));
                        contador++;
                    }
                }
                XmlNode select = appobj.AplicaionXML.DocumentElement.SelectSingleNode("ObjetosDB");                
                de[ide_padre].name = nomapp; de[ide_padre].ocp = 1; de[ide_padre].idepadre = -1; de[ide_padre].ispadre = 1;
                ide_padre++;
                de[ide_padre].name = name; de[ide_padre].ocp = 1; de[ide_padre].idepadre = (ide_padre-1); de[ide_padre].ispadre = 1;
                int ide_hijo = 0;
                foreach (XmlNode area in select.SelectNodes("row"))
                {
                    ide_hijo = EncuentraHijoAmigo(ide_padre, de, xini);
                    de[ide_hijo].name = area.Attributes["Objeto"].Value.ToString(); de[ide_hijo].ocp = 1; de[ide_hijo].idepadre = ide_padre;
                    de[ide_hijo].info= area.Attributes["codigo"].Value.ToString();
                }                
            }           
            return de;
        }

        private void ValidaXMLDiagrama(XmlDocument xd,string ObjetosDB,int ini, int maxe, bool actualiza) {
            try {
                XmlNode select = xd.DocumentElement.SelectSingleNode("ObjetosDB");
                int contador = 1;
                XmlAttribute attr;
                foreach (XmlNode area in select.SelectNodes("row"))
                {
                    if (!actualiza)
                    {
                        attr = xd.CreateAttribute("visible");
                        if (contador <= maxe && contador >= ini)
                        {
                            attr.Value = "true";
                            area.Attributes.Append(attr);
                        }
                        else
                        {
                            attr.Value = "false";
                            area.Attributes.Append(attr);
                        }
                    }
                    else {
                        if (contador <= maxe && contador >= ini)
                            area.Attributes["visible"].Value = "true";
                        else
                            area.Attributes["visible"].Value = "false";
                    }
                    contador++;
                }

            } catch (Exception)
            {

            }
        }

        public JsonResult ObtenTotalElementos() {
            try
            {
                XmlDocument xmlactual = (XmlDocument)TempData["xmlActual"];
                TempData.Keep("xmlActual");
                return Json(xmlactual.DocumentElement.SelectSingleNode("ObjetosDB").ChildNodes.Count.ToString(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception) {
                return Json("0", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult PaginaDiagrama(int actual, int maxe, int para) {
            XmlDocument xmlactual = (XmlDocument)TempData["xmlActual"];
            string nomapp = TempData["nomapp"].ToString();
            TempData.Keep("nomapp");
            TempData.Keep("xmlActual");
            if (para>actual)
                ValidaXMLDiagrama(xmlactual, "ObjetosDB", actual * maxe, para * maxe,true);
            else
                ValidaXMLDiagrama(xmlactual, "ObjetosDB", (para * maxe)- maxe, (actual * maxe)- maxe, true);
            string diagramahtml = string.Empty;
            int xini = 0, xfin = 0, yini = 0, yfin = 0, xinc = 0, yinc = 0;
            string dim = string.Empty;
            DiagramaDimension(maxe, ref xini, ref xfin, ref yini, ref yfin, ref xinc, ref yinc, ref dim);
            int ide_padre = 49;
            if (dim.Equals("2"))
                ide_padre = 21;

            int contador = 0;
            for (int x = xini; x < xfin; x += xinc)
            {
                for (int y = yini; y < yfin; y += yinc)
                {
                    de.Add(new Utilerias.diagramElem(contador, x, y, string.Empty, 0, 0, 0, ""));
                    contador++;
                }
            }
            XmlNode select = xmlactual.DocumentElement.SelectSingleNode("ObjetosDB");            
            de[ide_padre].name = nomapp; de[ide_padre].ocp = 1; de[ide_padre].idepadre = -1; de[ide_padre].ispadre = 1;
            ide_padre++;
            de[ide_padre].name = "Tablas"; de[ide_padre].ocp = 1; de[ide_padre].idepadre = (ide_padre-1); de[ide_padre].ispadre = 1;
            int ide_hijo = 0;
            foreach (XmlNode area in select.SelectNodes("row[@visible='true']"))
            {
                ide_hijo = EncuentraHijoAmigo(ide_padre, de, xini);
                de[ide_hijo].name = area.Attributes["Objeto"].Value.ToString(); de[ide_hijo].ocp = 1; de[ide_hijo].idepadre = ide_padre;
            }
            diagramahtml = ArmandoDIagrama(de,maxe);
            TempData["ControlDiv"] = de;
            TempData["xmlActual"] = xmlactual;
            return Json(diagramahtml, JsonRequestBehavior.AllowGet);
        }

        public void DiagramaDimension(int maxe, ref int xini, ref int xfin, ref int yini, ref int yfin, ref int xinc, ref int yinc,ref string dim)
        {
            dim = string.Empty;
            switch (maxe)
            {
                case 25:
                case 50:
                    dim = "2";
                    break;          
                default:
                    dim = string.Empty;
                    break;
            }
            xini = int.Parse(ConfigurationManager.AppSettings["xini"].ToString());
            xfin = int.Parse(ConfigurationManager.AppSettings["xfin"].ToString());
            yini = int.Parse(ConfigurationManager.AppSettings["yini"].ToString());
            yfin = int.Parse(ConfigurationManager.AppSettings["yfin"].ToString());
            xinc = int.Parse(ConfigurationManager.AppSettings["xinc" + dim].ToString());
            yinc = int.Parse(ConfigurationManager.AppSettings["yinc" + dim].ToString());
        }

        public JsonResult GeneraDiagramaTransversal(int appid , int maxe, int tipoid, string indicador) {
            string diagramahtml = string.Empty;            
            Aplicacion appobj = new Aplicacion();            
            int xini = 0, xfin = 0, yini = 0, yfin = 0, xinc = 0, yinc = 0;
            string dim = string.Empty;
            string nomapp = TempData["nomapp"].ToString();           
            TempData.Keep("nomapp");
            DiagramaDimension(maxe, ref xini, ref xfin, ref yini, ref yfin, ref xinc, ref yinc, ref dim);            
            int existe = 0;
            int ide_hijo = 0;
            int ide_padre = 49;
            if (dim.Equals("2"))
                ide_padre = 21;

            if (appobj.ObtenTransversalidad(int.Parse(Session["usuid"].ToString()), appid, tipoid, indicador,1))
            {
                int contador = 0;
                for (int x = xini; x < xfin; x += xinc)
                {
                    for (int y = yini; y < yfin; y += yinc)
                    {
                        de.Add(new Utilerias.diagramElem(contador, x, y, string.Empty, 0, 0, 0, ""));
                        contador++;
                    }
                }
                ValidaXMLDiagrama(appobj.AplicaionXML, "ObjetosDB", 1, maxe, false);
                XmlNode select = appobj.AplicaionXML.DocumentElement.SelectSingleNode("ObjetosDB");

                de[ide_padre].name = nomapp; de[ide_padre].ocp = 1; de[ide_padre].idepadre = -1; de[ide_padre].ispadre = 1;
                ide_padre++;
                de[ide_padre].name = indicador; de[ide_padre].ocp = 1; de[ide_padre].idepadre = (ide_padre - 1); de[ide_padre].ispadre = 1;
                foreach (XmlNode area in select.SelectNodes("row[@visible='true']"))
                {
                    ide_hijo = EncuentraHijoAmigo(ide_padre, de, xini);
                    de[ide_hijo].name = area.Attributes["Objeto"].Value.ToString(); de[ide_hijo].ocp = 1; de[ide_hijo].idepadre = ide_padre;
                }
                diagramahtml = ArmandoDIagrama(de, maxe);
            }
            TempData["tipoid"] = tipoid;            
            TempData["appid"] = appid;
            TempData["transversal"] = 1;
            TempData["indicador"] = indicador;
            TempData["ControlDiv"] = de;
            TempData["DiagBD"] = 0;
            TempData["xmlActual"] = appobj.AplicaionXML;
            return Json(diagramahtml, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GeneraDiagramaBD(int appid, string nomapp)
        {
            int maxe = 100;
            Aplicacion appobj = new Aplicacion();
            string diagramahtml = string.Empty;
            int xini = 0, xfin = 0, yini = 0, yfin = 0, xinc = 0, yinc = 0;
            string dim = string.Empty;
            DiagramaDimension(maxe, ref xini, ref xfin, ref yini, ref yfin, ref xinc, ref yinc, ref dim);
            ldbd.Clear();
            int existe = 0;

            int ide_padre = 49;
            if (dim.Equals("2"))
                ide_padre = 21;
            if (appobj.ObtenRelTablas(int.Parse(Session["usuid"].ToString()), appid))
            {
                int contador = 0;
                for (int x = xini; x < xfin; x += xinc)
                {
                    for (int y = yini; y < yfin; y += yinc)
                    {
                        de.Add(new Utilerias.diagramElem(contador, x, y, string.Empty, 0, 0, 0, ""));
                        contador++;
                    }
                }
                XmlNode select = appobj.AplicaionXML.DocumentElement.SelectSingleNode("ObjetosDB");

                de[ide_padre].name = nomapp; de[ide_padre].ocp = 1; de[ide_padre].idepadre = -1; de[ide_padre].ispadre = 1;
                ldbd.Add(new Utilerias.LineDiagramaDB(-1, ide_padre, "", "", ""));
                ide_padre++;
                de[ide_padre].name = "DiagramaBD"; de[ide_padre].ocp = 1; de[ide_padre].idepadre = (ide_padre - 1); de[ide_padre].ispadre = 1;
                ldbd.Add(new Utilerias.LineDiagramaDB((ide_padre - 1), ide_padre, "", "", ""));
                int ide_hijo = 0;
                foreach (XmlNode padre in select.SelectNodes("padre"))
                {
                    ide_hijo = EncuentraHijoAmigo(ide_padre, de, xini);
                    de[ide_hijo].name = padre.Attributes["TblPadre"].Value.ToString(); de[ide_hijo].ocp = 1; de[ide_hijo].idepadre = ide_padre;
                    ldbd.Add(new Utilerias.LineDiagramaDB(ide_hijo, ide_padre, padre.Attributes["PK_Nombre"].Value.ToString(), padre.Attributes["ColumnaPadre"].Value.ToString(), ""));
                    foreach (XmlNode hijo in padre.SelectNodes("hijo"))
                    {
                        existe=existeTabla(de, hijo.Attributes["TblHijo"].Value.ToString());
                        if (existe == 0) { 
                            existe = EncuentraHijoAmigo(ide_hijo, de, xini);
                            de[existe].name = hijo.Attributes["TblHijo"].Value.ToString(); de[existe].ocp = 1; de[existe].idepadre = ide_hijo;
                        }
                        ldbd.Add(new Utilerias.LineDiagramaDB(ide_hijo, existe, hijo.Attributes["FK_Nombre"].Value.ToString(), hijo.Attributes["ColumnaHijo"].Value.ToString(), padre.Attributes["ColumnaPadre"].Value.ToString()));                        
                    }
                }
                diagramahtml = ArmandoDIagramaBD(de, ldbd);
            }
            TempData["nomapp"] = nomapp;
            TempData["appid"] = appid;
            TempData["ControlDiv"] = de;
            TempData["DiagBD"] = 1;
            TempData["xmlActual"] = appobj.AplicaionXML;
            TempData["LineasDB"] = ldbd;

            return Json(diagramahtml, JsonRequestBehavior.AllowGet);
        }

        private string ArmandoDIagramaBD(List<Utilerias.diagramElem> de, List<Utilerias.LineDiagramaDB> ldbd)
        {
            string diag = string.Empty;
            string line = "<svg id=\"svg1\">";
            int maxe = 100;
            int xini = 0, xfin = 0, yini = 0, yfin = 0, xinc = 0, yinc = 0;
            string dim = string.Empty;
            DiagramaDimension(maxe, ref xini, ref xfin, ref yini, ref yfin, ref xinc, ref yinc, ref dim);
            int xinca = 45;

            int centerx = (xinc - 30) / 2;
            int centery = (yinc - 30) / 2;
            string styles = string.Empty;

            string csscuadrado = "cuadrado" + dim;
            string csspadre = "padre" + dim;
            string cssabuelo = "abuelo" + dim;
            string csslabel = string.Empty;


            //crea entidades
            foreach (Utilerias.diagramElem dee in de)
            {
                if (dee.ocp == 1)
                {
                    csslabel = "labelcom";
                    if (dee.ispadre == 1) styles = csspadre; else styles = csscuadrado;
                    if (dee.idepadre == -1) { styles = cssabuelo; csslabel = "labelcom2"; }

                    diag += "<div onmouseout=\"EscondeRelacion()\" onmouseover=\"MuestraCodigo(event,'" + dee.info + "')\" ondblclick=\"OtroDiagrama('" + dee.name + "')\" id=\"" + "cuadrado" + dee.ide + "\" draggable=\"true\" class=\"" + styles + "\" style=\"top:" + dee.top + "px;left:" + dee.left + "px;\">" +
                                "<center><label id=\"" + "label" + dee.ide + "\" class=\"" + csslabel + "\">" + dee.name + "</label></center></div>";                                  
                }
            }
            int conta = 1;
            foreach (Utilerias.LineDiagramaDB ln in ldbd)
            {
                if (ln.Idepadre > -1)
                {
                    if (ln.Fknombre.Length == 0)
                        line += "<line onmouseout=\"EscondeRelacion()\" onmouseover=\"MuestraRelacion(event,'" + ln.Campo_p + "-" + ln.Campo_h + "-"+ ln.Fknombre + "')\" id=\"" + "line" + conta + "\" class=\"lineDB\" x1=\"" + (de[ln.Idepadre].left + centery) + "\" y1=\"" + ((de[ln.Idepadre].top - xini) + centerx + xinca) + "\" x2=\"" + (de[ln.Idehijo].left + centery) + "\" y2=\"" + ((de[ln.Idehijo].top - xini) + centerx + xinca) + "\"></line>";
                    else
                        line += "<line onmouseout=\"EscondeRelacion()\" onmouseover=\"MuestraRelacion(event,'" + ln.Campo_p + "-" + ln.Campo_h + "-" + ln.Fknombre + "')\" id=\"" + "line" + conta + "\" class=\"lineDB\" x1=\"" + (de[ln.Idepadre].left + centery) + "\" y1=\"" + ((de[ln.Idepadre].top - xini) + centerx + xinca) + "\" x2=\"" + (de[ln.Idehijo].left + centery) + "\" y2=\"" + ((de[ln.Idehijo].top - xini) + centerx + xinca) + "\"></line>";
                }//ln.Campo_p + "-" + ln.Campo_h 
                conta++;
            }
            line += "</svg>";
            return (diag + line);
        }

        public int existeTabla(List<Utilerias.diagramElem> de, string tabla) {
            int existe = 0;
            foreach (Utilerias.diagramElem ent in de)
            {
                if (ent.name.Equals(tabla))
                    existe = ent.ide;
            }
            return existe;
        }

        public JsonResult GeneraDiagrama(int appid, int maxe, int tipoid,string nomapp)
        {
            Aplicacion appobj = new Aplicacion();            
            string diagramahtml = string.Empty;
            int xini = 0, xfin = 0, yini = 0, yfin = 0, xinc = 0, yinc = 0;
            string dim = string.Empty;
            DiagramaDimension(maxe, ref xini, ref xfin, ref yini, ref yfin, ref xinc, ref yinc,ref dim);
            
            int ide_padre = 49;         
            if (dim.Equals("2"))
                ide_padre = 21;

            if (appobj.ObtenObjetosDB2(int.Parse(Session["usuid"].ToString()), appid, tipoid))
            {
                int contador = 0;
                for (int x = xini; x < xfin; x += xinc)
                {
                    for (int y = yini; y < yfin; y += yinc)
                    {
                        de.Add(new Utilerias.diagramElem(contador, x, y, string.Empty, 0, 0, 0, ""));
                        contador++;
                    }
                }
                ValidaXMLDiagrama(appobj.AplicaionXML, "ObjetosDB",1, maxe,false);
                XmlNode select = appobj.AplicaionXML.DocumentElement.SelectSingleNode("ObjetosDB");
                
                de[ide_padre].name = nomapp ; de[ide_padre].ocp = 1; de[ide_padre].idepadre = -1; de[ide_padre].ispadre = 1;
                ide_padre ++;
                de[ide_padre].name = ""; de[ide_padre].ocp = 1; de[ide_padre].idepadre = (ide_padre-1); de[ide_padre].ispadre = 1;
                int ide_hijo = 0;
                foreach (XmlNode area in select.SelectNodes("row[@visible='true']"))
                {
                    ide_hijo = EncuentraHijoAmigo(ide_padre, de, xini);
                    de[ide_hijo].name = area.Attributes["Objeto"].Value.ToString(); de[ide_hijo].ocp = 1; de[ide_hijo].idepadre = ide_padre;
                }
                diagramahtml = ArmandoDIagrama(de, maxe);
            }
            TempData["tipoid"] = tipoid;
            TempData["nomapp"] = nomapp;
            TempData["appid"] = appid;
            TempData["transversal"] = 0;
            TempData["indicador"] = "";
            TempData["ControlDiv"] = de;
            TempData["DiagBD"] = 0;
            TempData["xmlActual"] = appobj.AplicaionXML;
            return Json(diagramahtml, JsonRequestBehavior.AllowGet);
        }

        private string subhijos(XmlNode archivo, string padre, string itemdata) {
            string pruebaaux2 = string.Empty;
            int contadorn = 0;
            string newitemdata = string.Empty;
            
            foreach (XmlNode clases in archivo.SelectNodes("Hijo[@IDPadre='" + padre + "']")) {
                if (contadorn == 0)
                    pruebaaux2 += "<ul>";
                newitemdata = itemdata + "-" + clases.Attributes["DependenciaCMID"].Value.ToString();
                pruebaaux2 += "<li> <input type = \"checkbox\" id=\"" + newitemdata + "\" /><label for=\"" + newitemdata + "\">" + clases.Attributes["ObjHijo"].Value.ToString() + "</label>";
                pruebaaux2 += subhijos(archivo, clases.Attributes["DependenciaCMID"].Value.ToString(), newitemdata);
                contadorn++;
                pruebaaux2 += "</li>";               
            }
            if (contadorn > 0)
                pruebaaux2 += "</ul>";
            return pruebaaux2;
        }

        private string CargaNivel1(XmlDocument consultaxml)
        {
            string padreprin = "<ul><li> <input type = \"checkbox\" id=\"item-0\" /><label for=\"item-0\">Dependencias</label>";            
            try
            {
                string archivoanterior = string.Empty;
                string padreaux = string.Empty;
                string nwitem = string.Empty;
                int contador =0;
                int contador2 = 0;
                foreach (XmlNode archivo in consultaxml.SelectNodes("xml/Padre[@File = 'cs']"))
                {
                    contador2 = 0;
                    if (contador == 0)
                        padreaux += "<ul>";
                    padreaux += "<li> <input type = \"checkbox\" id=\"item-0-" + contador.ToString() + "\" /><label for=\"item-0-" + contador.ToString() + "\">" + archivo.Attributes["BibPadre"].Value.ToString() + "</label>";
                    foreach (XmlNode clases in archivo.SelectNodes("Hijo"))
                    {                        
                        if (clases.Attributes["IDPadre"].Value.ToString().Equals("0"))
                        {
                            if (contador2 == 0)
                                padreaux += "<ul>";
                            padreaux += "<li> <input type = \"checkbox\" id=\"item-0-" + contador.ToString() + "-" + clases.Attributes["DependenciaCMID"].Value.ToString() + "\" /><label for=\"item-0-" + contador.ToString() + "-" + clases.Attributes["DependenciaCMID"].Value.ToString() +  "\">" + clases.Attributes["ObjHijo"].Value.ToString() + "</label>";
                            nwitem = "item-0-" + contador.ToString() + "-" + clases.Attributes["DependenciaCMID"].Value.ToString();
                            if(clases.Attributes["TIpoHijo"].Value.ToString().Equals("Metodo") || clases.Attributes["TIpoHijo"].Value.ToString().Equals("Evento") || clases.Attributes["TIpoHijo"].Value.ToString().Equals("Constructor"))
                                 padreaux += subhijos(archivo, clases.Attributes["DependenciaCMID"].Value.ToString(), nwitem);
                            contador2++;
                            padreaux += "</li>";
                        }                        
                    }
                    if (contador2 > 0)
                        padreaux += "</ul>";
                    contador++;
                    padreaux += "</li>";
                }
                if (contador > 0)
                    padreaux += "</ul>";
                padreprin += padreaux;
                padreprin += "</ul></li>";
            }
            catch (Exception err)
            {
                ControlLog controlobj = new ControlLog();
                controlobj.EscribeLog("Consulta.aspx.cs.CargaNivel1 " + err.Message.ToString());
            }           
            return padreprin;
        }

        private string CargaTree(Proceso procesoobj)
        {
            string arbol = string.Empty;
            try
            {                
                XmlDocument consultaxml = procesoobj.PAvanceXML;
                arbol = CargaNivel1(consultaxml);
                //primernivel = CargaNiveli(consultaxml, primernivel, int.Parse(ConfigurationManager.AppSettings["maxNivelJerarq"].ToString()));                
            }
            catch (Exception Err)
            {
                procesoobj.EscribeLog("Consulta.aspx.cs.CargaTree " + Err.Message.ToString());
            }
            return arbol;
        }

        private int EncuentraHijoAmigo(int ide_padre, List<Utilerias.diagramElem> diag, int xini)
        {
            int ide_hijo = 0;
            double distmenora = 2000;
            double calcaux = 0;
            foreach (Utilerias.diagramElem dee in diag)
            {
                if (dee.ocp == 0)
                {
                    calcaux = distancia(diag[ide_padre].left, (diag[ide_padre].top - xini), (dee.left), (dee.top - xini));
                    if (distmenora > calcaux)
                    {
                        ide_hijo = (dee.ide);
                        distmenora = calcaux;
                    }
                }
            }
            return ide_hijo;
        }

        //teorema de pitagoras
        private double distancia(int x1, int y1, int x2, int y2)
        {
            double distancia = 0;
            int x = x2 - x1;
            int y = y2 - y1;
            double pt = (x * x) + (y * y);
            distancia = Math.Sqrt(pt);
            return distancia;
        }

        private string ArmandoDIagrama(List<Utilerias.diagramElem> de, int maxe)
        {
            string diag = string.Empty;
            string line = "<svg id=\"svg1\">";
            
            int xini = 0, xfin = 0, yini = 0, yfin = 0, xinc = 0, yinc = 0;
            string dim = string.Empty;
            DiagramaDimension(maxe, ref xini, ref xfin, ref yini, ref yfin, ref xinc, ref yinc, ref dim);            
            int xinca = 45;

            int centerx = (xinc-30)/2;
            int centery = (yinc - 30) / 2;
            string styles = string.Empty;

            string csscuadrado = "cuadrado" + dim;
            string csspadre = "padre" + dim;
            string cssabuelo = "abuelo" + dim;
            string csslabel = string.Empty;


            //crea entidades
            foreach (Utilerias.diagramElem dee in de)
            {
                if (dee.ocp == 1)
                {
                    csslabel = "labelcom";
                    if (dee.ispadre == 1) styles = csspadre; else styles = csscuadrado;
                    if (dee.idepadre == -1) { styles = cssabuelo; csslabel = "labelcom2"; }

                    diag += "<div onmouseout=\"EscondeRelacion()\" onmouseover=\"MuestraCodigo(event,'" + dee.info + "')\" ondblclick=\"OtroDiagrama('" + dee.name + "')\" id=\"" + "cuadrado" + dee.ide + "\" draggable=\"true\" class=\"" + styles + "\" style=\"top:" + dee.top + "px;left:" + dee.left + "px;\">" +
                                "<center><label id=\"" + "label" + dee.ide + "\" class=\"" + csslabel + "\">" + dee.name + "</label></center></div>";
                    if (dee.idepadre > -1)
                    {
                        if (de[dee.idepadre].idepadre == -1)
                            line += "<line id=\"" + "line" + dee.ide + "\" class=\"linepadre\" x1=\"" + (dee.left + centery) + "\" y1=\"" + ((dee.top - xini) + centerx + xinca) + "\" x2=\"" + (de[dee.idepadre].left + centery) + "\" y2=\"" + ((de[dee.idepadre].top - xini) + centerx + xinca) + "\"></line>";
                        else
                            line += "<line id=\"" + "line" + dee.ide + "\" class=\"line\" x1=\"" + (dee.left + centery) + "\" y1=\"" + ((dee.top - xini) + centerx + xinca) + "\" x2=\"" + (de[dee.idepadre].left + centery) + "\" y2=\"" + ((de[dee.idepadre].top - xini) + centerx + xinca) + "\"></line>";
                    }
                    else {

                    }
                }
            }
            line += "</svg>";
            return (diag + line);
        }

        public ActionResult Index()
        {
            Menu Menu = new Menu();
            ViewBag.XMLMenu = Menu.ComponeMenu(int.Parse(Session["usuid"].ToString()), Session["pfnum"].ToString());
            TempData["appid"] = 0;            
            return View(new Dependencias());
        }

        // GET: /Dependencias/Details/5
        public ActionResult Detalle(int id)
        {
            Dependencias objdep = new Dependencias();
            return PartialView("Detalle", objdep);
        }

        public ActionResult ActualizaCubo(int idtipo,int appid)
        {
            Dependencias objdep = new Dependencias(idtipo, appid, int.Parse(Session["usuid"].ToString()),0);
            TempData["ProcesoID"] = objdep.procesaid;
            return PartialView("Cubo", objdep);
        }

        public ActionResult GraficaDBTotal(int appid)
        {
            Proceso Proc = new Proceso();
            ProcesoDto ProcDto = new ProcesoDto();

            Bitmap Imagen = new Bitmap(300, 50);
            Graphics Grafica = Graphics.FromImage(Imagen);
            Chart Chart = new Chart();
            int i = 0;

            ProcDto.AplicacionID = appid;
            Chart.Width = 600;
            Chart.Height = 400;
            Chart.ChartAreas.Add("ObjetosDB").BackColor = System.Drawing.Color.FromArgb(64, System.Drawing.Color.White);
            Chart.ChartAreas["ObjetosDB"].AxisX.MajorGrid.LineWidth = 0;
            Chart.ChartAreas["ObjetosDB"].AxisX.LabelStyle.Enabled = false;
            Chart.ChartAreas["ObjetosDB"].AxisX.ScaleBreakStyle.Spacing = 2;
            Chart.Titles.Add("OBJETOSDB");
            Chart.Legends.Add(new Legend("Leyenda"));

            if (Proc.ObtenGrafica(9, ProcDto.AplicacionID))
            {
                XmlDocument consultaxml = Proc.PAvanceXML;
                foreach (XmlNode Fila in consultaxml.DocumentElement.SelectSingleNode("ObjetosDB").SelectNodes("row"))
                {

                    Chart.Series.Add("ObjetosDB" + i);
                    Chart.Series["ObjetosDB" + i].ChartType = SeriesChartType.Column;
                    Chart.Series["ObjetosDB" + i].Points.AddY(Fila.Attributes["Numero"].Value.ToString());
                    Chart.Series["ObjetosDB" + i].Label = "#VALY";
                    Chart.Series["ObjetosDB" + i].LegendText = Fila.Attributes["TipoObjeto"].Value.ToString();
                    Chart.Series["ObjetosDB" + i].Font = new Font("Segoe UI", 10.0f, FontStyle.Regular);
                    Chart.Series["ObjetosDB" + i]["PieLabelStyle"] = "Outside";
                    Chart.Series["ObjetosDB" + i].Legend = "Leyenda";

                    ++i;
                }

            }

            Chart.Legends["Leyenda"].Docking = Docking.Bottom;
            Chart.Legends["Leyenda"].Alignment = StringAlignment.Center;
            Chart.BackColor = Color.Transparent;
            MemoryStream imageStream = new MemoryStream();
            Chart.SaveImage(imageStream, ChartImageFormat.Png);
            Chart.TextAntiAliasingQuality = TextAntiAliasingQuality.SystemDefault;
            imageStream.Position = 0;
            Grafica.Dispose();
            Imagen.Dispose();
            return new FileStreamResult(imageStream, "image/png");

        }

        public ActionResult Descarga(string app)
        {
            try
            {
                int idapp =(int)TempData["appid"];
                TempData.Keep("appid");
                Aplicacion objapp = new Aplicacion();
                long proceso = objapp.ObtenerProcesoID(idapp);
                string fullName = Server.MapPath(ConfigurationManager.AppSettings["pcsv"].ToString() + proceso + ".csv");
                byte[] fileBytes = GetFile(fullName);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "ProcesoDB" + proceso + ".csv");
            }
            catch (Exception)
            {
                string mensaje = "Esta en ejecusión un parseo para el proceso: " + "incorrecto" + ", es necesario esperar los resultados.";
                return File(Encoding.ASCII.GetBytes(mensaje), System.Net.Mime.MediaTypeNames.Application.Octet, "ProcesoDBIncorrecto.csv"); ;
            }

        }

        public ActionResult ImprimirPantalla(string app)
        {
            var date = "";
            try
            {
                Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.WorkingArea.Width,
                                    Screen.PrimaryScreen.WorkingArea.Height);
                Graphics graphics = Graphics.FromImage(bitmap as Image);
                graphics.CopyFromScreen(Screen.PrimaryScreen.WorkingArea.X, 
                    Screen.PrimaryScreen.WorkingArea.Y, 0, 0, Screen.PrimaryScreen.WorkingArea.Size);
                date = DateTime.Now.ToString("MMddyyHmmss");
                string fullName = Server.MapPath(ConfigurationManager.AppSettings["Imprimir"].ToString() + "Img" + date + ".jpg");
                bitmap.Save(fullName, ImageFormat.Jpeg);

                byte[] fileBytes = System.IO.File.ReadAllBytes(fullName);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Img" + date + ".jpg");
            }
            catch (Exception)
            {
                string mensaje = "No se puede obtener la imagen";
                return File(Encoding.ASCII.GetBytes(mensaje), System.Net.Mime.MediaTypeNames.Application.Octet, "Img" + date + ".jpg"); ;
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

        public ActionResult ActualizaDetalleTransversal(int appid, int idtipo, string indicador) {
            Dependencias objdep = new Dependencias(idtipo, appid, int.Parse(Session["usuid"].ToString()), indicador);
            return PartialView("Detalle", objdep);
        }

        public ActionResult ActualizaDetalle(int appid, string nombre)
        {
            Dependencias objdep = new Dependencias(appid, int.Parse(Session["usuid"].ToString()), nombre);
            return PartialView("Detalle", objdep);
        }

        //// GET: /Dependencias/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: /Dependencias/Create
        //[HttpPost]
        //public ActionResult Create(FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: /Dependencias/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: /Dependencias/Edit/5
        //[HttpPost]
        //public ActionResult Edit(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: /Dependencias/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: /Dependencias/Delete/
        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
