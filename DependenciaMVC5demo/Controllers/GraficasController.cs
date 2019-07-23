using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Web.Helpers;
using System.Web.Mvc;
using System.Xml;
using DependenciaMVC5demo.com.negocio;
using DependenciaMVC5demo.com.dto;
using DependenciaMVC5demo.com.utilerias;
using DependenciaMVC5demo.Models;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
using System.IO;

namespace DependenciaMVC5demo.Controllers
{
    public class GraficasController : Controller
    {
        // GET: AlgoNuevo
        public ActionResult Index()
        {
            Menu Menu = new Menu();
            ViewBag.XMLMenu = Menu.ComponeMenu(int.Parse(Session["usuid"].ToString()), Session["pfnum"].ToString());
            GraficasModel xan = new GraficasModel();
            TempData["appid"] = 0;
            return View(xan);
        }

        public ActionResult GraficaAplicacion(int appid) {
            GraficasModel xan = new GraficasModel();
            TempData["appid"] = appid;
            xan.AplicacionID = appid;
            xan.NombreApp = "Aplicación :" + appid.ToString();
            return PartialView("GraficaAplicacion", xan);            
        }

        public ActionResult GraficaDB(int appid)
        {
            Proceso Proc = new Proceso();
            ProcesoDto ProcDto = new ProcesoDto();

            ProcDto.AplicacionID = appid;

            Bitmap Imagen = new Bitmap(300, 50);
            Graphics Grafica = Graphics.FromImage(Imagen);
            Chart Chart = new Chart();
            int i = 0;
            int tiposp;
            string App;

            Chart.Width = 600;
            Chart.Height = 220;
            Chart.ChartAreas.Add("ObjetosDB").BackColor = System.Drawing.Color.FromArgb(64, System.Drawing.Color.White);
            Chart.ChartAreas["ObjetosDB"].AxisX.MajorGrid.LineWidth = 0;
            Chart.ChartAreas["ObjetosDB"].AxisX.LabelStyle.Enabled = false;
            Chart.Titles.Add("OBJETOS DB");
            Chart.Legends.Add(new Legend("Leyenda"));

            if (ProcDto.AplicacionID != 0)
            {
                tiposp = 5;
            }
            else
            {
                tiposp = 1;
            }

            if (Proc.ObtenGrafica(tiposp, ProcDto.AplicacionID))
            {
                XmlDocument consultaxml = Proc.PAvanceXML;
                foreach (XmlNode Fila in consultaxml.DocumentElement.SelectSingleNode("ObjetosDB").SelectNodes("row"))
                {

                    App = Fila.Attributes["TipoObjeto"].Value.ToString();

                    Chart.Series.Add(App);
                    Chart.Series[App].ChartType = SeriesChartType.Column;
                    Chart.Series[App].Points.AddY(Fila.Attributes["Numero"].Value.ToString());
                    Chart.Series[App].Label = "#VALY";
                    Chart.Series[App].LegendText = Fila.Attributes["TipoObjeto"].Value.ToString();
                    Chart.Series[App]["PieLabelStyle"] = "Outside";
                    Chart.Series[App].Legend = "Leyenda";

                    ++i;
                        }

            }

            Chart.Legends["Leyenda"].Docking = Docking.Bottom;
            Chart.Legends["Leyenda"].Alignment = StringAlignment.Center;
            Chart.BackColor = Color.Transparent;
            MemoryStream imageStream = new MemoryStream();
            Chart.SaveImage(imageStream, ChartImageFormat.Png);
            Chart.TextAntiAliasingQuality = TextAntiAliasingQuality.SystemDefault;
            Response.ContentType = "image/png";
            imageStream.WriteTo(Response.OutputStream);
            Grafica.Dispose();
            Imagen.Dispose();
            return null;

        }

        public ActionResult GraficaCM(int appid)
        {
            Proceso Proc = new Proceso();
            ProcesoDto ProcDto = new ProcesoDto();

            ProcDto.AplicacionID = appid;

            Bitmap Imagen = new Bitmap(300, 50);
            Graphics Grafica = Graphics.FromImage(Imagen);
            Chart Chart = new Chart();
            int i = 0, tiposp;
            string App;

            Chart.Width = 600;
            Chart.Height = 220;
            Chart.ChartAreas.Add("ObjetosCM").BackColor = System.Drawing.Color.FromArgb(64, System.Drawing.Color.White);
            Chart.ChartAreas["ObjetosCM"].AxisX.MajorGrid.LineWidth = 0;
            Chart.ChartAreas["ObjetosCM"].AxisX.LabelStyle.Enabled = false;
            Chart.Titles.Add("OBJETOS CM");
            Chart.Legends.Add(new Legend("Leyenda"));

            if (ProcDto.AplicacionID != 0)
            {
                tiposp = 6;
            }
            else
            {
                tiposp = 2;
            }


            if (Proc.ObtenGrafica(tiposp, ProcDto.AplicacionID))
            {
                XmlDocument consultaxml = Proc.PAvanceXML;
                foreach (XmlNode Fila in consultaxml.DocumentElement.SelectSingleNode("ObjetosCM").SelectNodes("row"))
                {

                    App = Fila.Attributes["TipoHijo"].Value.ToString();

                    Chart.Series.Add(App);
                    Chart.Series[App].ChartType = SeriesChartType.Column;
                    Chart.Series[App].Points.AddY(Fila.Attributes["Numero"].Value.ToString());
                    Chart.Series[App].Label = "#VALY";
                    Chart.Series[App].LegendText = Fila.Attributes["TipoHijo"].Value.ToString();
                    Chart.Series[App]["PieLabelStyle"] = "Outside";
                    Chart.Series[App].Legend = "Leyenda";

                    ++i;
                }

            }

            Chart.Legends["Leyenda"].Docking = Docking.Bottom;
            Chart.Legends["Leyenda"].Alignment = StringAlignment.Center;
            Chart.BackColor = Color.Transparent;
            MemoryStream imageStream = new MemoryStream();
            Chart.SaveImage(imageStream, ChartImageFormat.Png);
            Chart.TextAntiAliasingQuality = TextAntiAliasingQuality.SystemDefault;
            Response.ContentType = "image/png";
            imageStream.WriteTo(Response.OutputStream);
            Grafica.Dispose();
            Imagen.Dispose();
            return null;

        }

        public ActionResult GraficaWS(int appid)
        {
            Proceso Proc = new Proceso();
            ProcesoDto ProcDto = new ProcesoDto();

            ProcDto.AplicacionID =appid;

            Bitmap Imagen = new Bitmap(300, 50);
            Graphics Grafica = Graphics.FromImage(Imagen);
            Chart Chart = new Chart();
            int i = 0, tiposp;
            string App;

            Chart.Width = 600;
            Chart.Height = 220;
            Chart.ChartAreas.Add("ObjetosWS").BackColor = System.Drawing.Color.FromArgb(64, System.Drawing.Color.White);
            Chart.ChartAreas["ObjetosWS"].AxisX.MajorGrid.LineWidth = 0;
            Chart.ChartAreas["ObjetosWS"].AxisX.LabelStyle.Enabled = false;
            Chart.Titles.Add("OBJETOS WS");
            Chart.Legends.Add(new Legend("Leyenda"));

            if (ProcDto.AplicacionID != 0)
            {
                tiposp = 7;
            }
            else
            {
                tiposp = 3;
            }

            if (Proc.ObtenGrafica(tiposp, ProcDto.AplicacionID))
            {
                XmlDocument consultaxml = Proc.PAvanceXML;
                foreach (XmlNode Fila in consultaxml.DocumentElement.SelectSingleNode("ObjetosWS").SelectNodes("row"))
                {

                    App = Fila.Attributes["TipoHijo"].Value.ToString();

                    Chart.Series.Add(App);
                    Chart.Series[App].ChartType = SeriesChartType.Bar;
                    Chart.Series[App].Points.AddY(Fila.Attributes["Numero"].Value.ToString());
                    Chart.Series[App].Label = "#VALY";
                    Chart.Series[App].LegendText = Fila.Attributes["TipoHijo"].Value.ToString();
                    Chart.Series[App]["PieLabelStyle"] = "Outside";
                    Chart.Series[App].Legend = "Leyenda";

                    ++i;
                }

            }

            Chart.Legends["Leyenda"].Docking = Docking.Left;
            Chart.Legends["Leyenda"].Alignment = StringAlignment.Center;
            Chart.BackColor = Color.Transparent;
            MemoryStream imageStream = new MemoryStream();
            Chart.SaveImage(imageStream, ChartImageFormat.Png);
            Chart.TextAntiAliasingQuality = TextAntiAliasingQuality.SystemDefault;
            Response.ContentType = "image/png";
            imageStream.WriteTo(Response.OutputStream);
            Grafica.Dispose();
            Imagen.Dispose();
            return null;

        }

        public ActionResult GraficaDB_Detalle2()
        {
            Proceso Proc = new Proceso();
            ProcesoDto ProcDto = new ProcesoDto();

            Bitmap Imagen = new Bitmap(300, 50);
            Graphics Grafica = Graphics.FromImage(Imagen);
            Chart Chart = new Chart();
            int i = 0, tiposp;
            string App;
            
            Chart.Width = 600;
            Chart.Height = 220;
            Chart.ChartAreas.Add("ObjetosDB_Detalle").BackColor = System.Drawing.Color.FromArgb(64, System.Drawing.Color.White);
            Chart.ChartAreas["ObjetosDB_Detalle"].AxisX.MajorGrid.LineWidth = 0;
            Chart.ChartAreas["ObjetosDB_Detalle"].AxisX.LabelStyle.Enabled = false;
            Chart.Titles.Add("USO OBJETOS");
            Chart.Legends.Add(new Legend("Leyenda"));
            App = "Datos";
            Chart.Series.Add(App);
            Chart.Series[App].ChartType = SeriesChartType.Pie;
            Chart.Series[App].Label = "#PERCENT{P0}";
            Chart.Series[App]["PieLabelStyle"] = "Outside";
            Chart.Series[App].Legend = "Leyenda";

            if (ProcDto.AplicacionID != 0)
            {
                tiposp = 8;
            }
            else
            {
                tiposp = 4;
            }


            if (Proc.ObtenGrafica(tiposp, ProcDto.AplicacionID))
            {
                XmlDocument consultaxml = Proc.PAvanceXML;
                foreach (XmlNode Fila in consultaxml.DocumentElement.SelectSingleNode("ObjetosDB_Detalle").SelectNodes("row"))
                {

                    Chart.Series[App].Points.AddY(Fila.Attributes["Numero"].Value.ToString());
                    Chart.Series[App].Points[i].LegendText = Fila.Attributes["TipoObjeto"].Value.ToString();

                    ++i;
                }

            }

            Chart.Legends["Leyenda"].Docking = Docking.Bottom;
            Chart.Legends["Leyenda"].Alignment = StringAlignment.Center;
            Chart.BackColor = Color.Transparent;
            MemoryStream imageStream = new MemoryStream();
            Chart.SaveImage(imageStream, ChartImageFormat.Png);
            Chart.TextAntiAliasingQuality = TextAntiAliasingQuality.SystemDefault;
            Response.ContentType = "image/png";
            imageStream.WriteTo(Response.OutputStream);
            Grafica.Dispose();
            Imagen.Dispose();
            return null;

        }

    }
}