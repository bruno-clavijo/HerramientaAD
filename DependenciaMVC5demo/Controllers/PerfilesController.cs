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
    public class PerfilesController : Controller
    {
        Menu Menu = new Menu();
        Perfiles perfiles = new Perfiles();
        readonly int Todos = 0;

        // GET: Perfiles
        public ActionResult Index()
        {
            ViewBag.XMLMenu = Menu.ComponeMenu(int.Parse(Session["usuid"].ToString()), Session["pfnum"].ToString());
            var perfilesModel = new PerfilesModel(int.Parse(Session["usuid"].ToString()), Todos);
            return View(perfilesModel);
        }

        public ActionResult Perfiles(int PerfilID, string Tipo)
        {
            ViewBag.XMLMenu = Menu.ComponeMenu(int.Parse(Session["usuid"].ToString()), Session["pfnum"].ToString());
            var perfilesModel = new PerfilesModel(int.Parse(Session["usuid"].ToString()), PerfilID);

            if (Tipo == "Elimina")
            {
                Guardar(perfilesModel, Tipo);
                return RedirectToAction("Index", "Perfiles");
            }
            else
            {
                ViewBag.Operacion = Tipo;
                return View(perfilesModel);
            }
        }

        [HttpPost]
        public ActionResult Guardar(PerfilesModel perfilesModel, string Tipo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    perfilesModel.ListaEstatus = perfilesModel.Estatus(int.Parse(Session["usuid"].ToString()));
                    ViewBag.XMLMenu = Menu.ComponeMenu(int.Parse(Session["usuid"].ToString()), Session["pfnum"].ToString());
                    ViewBag.Operacion = Tipo;
                    return View("Perfiles", perfilesModel);
                }

                string Per1 = perfilesModel.OpcionAnalisis == true ? "1" : "0";
                string Per2 = perfilesModel.OpcionDependencias == true ? "1" : "0";
                string Per3 = perfilesModel.OpcionDetalle == true ? "1" : "0";
                string Per4 = perfilesModel.OpcionGraficas == true ? "1" : "0";
                string Per5 = perfilesModel.OpcionUsuarios == true ? "1" : "0";
                string Per6 = perfilesModel.OpcionPerfiles == true ? "1" : "0";
                string Per7 = perfilesModel.OpcionAplicaciones == true ? "1" : "0";
                string Permisos = Per1 + Per2 + Per3 + Per4 + Per5 + Per6 + Per7;

                perfiles.EditaPerfiles(
                    int.Parse(Session["usuid"].ToString()),
                    Tipo,
                    perfilesModel.Identificador,
                    perfilesModel.Perfil,
                    Permisos,
                    perfilesModel.EstatusID);
                return RedirectToAction("Index", "Perfiles");

            }
            catch
            {
                return View();
            }
        }
    }
}
