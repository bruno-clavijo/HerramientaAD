using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DependenciaMVC5demo.Models;
using DependenciaMVC5demo.com.negocio;
using DependenciaMVC5demo.com.utilerias;

namespace DependenciaMVC5demo.Controllers
{
    public class AplicacionesController : Controller
    {
        Menu Menu = new Menu();
        Aplicaciones aplicaciones = new Aplicaciones();
        readonly int Todos = 0;

        // GET: Aplicaciones
        public ActionResult Index()
        {
            ViewBag.XMLMenu = Menu.ComponeMenu(int.Parse(Session["usuid"].ToString()), Session["pfnum"].ToString());
            var aplicacionesModel = new AplicacionesModel(int.Parse(Session["usuid"].ToString()), Todos, Todos);
            return View(aplicacionesModel);
        }

        public ActionResult Aplicaciones(int AplicacionID, string Tipo)
        {
            ViewBag.XMLMenu = Menu.ComponeMenu(int.Parse(Session["usuid"].ToString()), Session["pfnum"].ToString());
            var aplicacionesModel = new AplicacionesModel(int.Parse(Session["usuid"].ToString()), Todos, AplicacionID);

            if (Tipo == "Elimina")
            {
                Guardar(aplicacionesModel, Tipo);
                return RedirectToAction("Index", "Aplicaciones");
            }
            else
            {
                ViewBag.Operacion = Tipo;
                return View(aplicacionesModel);
            }
        }

        [HttpPost]
        public ActionResult Guardar(AplicacionesModel aplicacionesModel, string Tipo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    aplicacionesModel.ListaAreas = aplicacionesModel.Areas(int.Parse(Session["usuid"].ToString()));
                    aplicacionesModel.ListaLenguajes = aplicacionesModel.Lenguajes(int.Parse(Session["usuid"].ToString()));
                    aplicacionesModel.ListaEstatus = aplicacionesModel.Estatus(int.Parse(Session["usuid"].ToString()));
                    ViewBag.XMLMenu = Menu.ComponeMenu(int.Parse(Session["usuid"].ToString()), Session["pfnum"].ToString());
                    ViewBag.Operacion = Tipo;
                    return View("Aplicaciones", aplicacionesModel);
                }

                aplicaciones.EditaAplicaciones(
                    int.Parse(Session["usuid"].ToString()),
                    Tipo,
                    aplicacionesModel.Identificador,
                    aplicacionesModel.Aplicacion,
                    string.IsNullOrEmpty(aplicacionesModel.Descripcion) ? "" : aplicacionesModel.Descripcion,
                    aplicacionesModel.LenguajeID,
                    string.IsNullOrEmpty(aplicacionesModel.ClaveAplicacion) ? "" : aplicacionesModel.ClaveAplicacion,
                    aplicacionesModel.AreaID,
                    aplicacionesModel.EstatusID);
                return RedirectToAction("Index", "Aplicaciones");
            }
            catch
            {
                return View();
            }
        }
    }
}