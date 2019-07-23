using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using DependenciaMVC5demo.Models;
using DependenciaMVC5demo.com.negocio;
using DependenciaMVC5demo.com.utilerias;

namespace DependenciaMVC5demo.Controllers
{
    public class UsuarioController : Controller
    {
        Menu Menu = new Menu();
        Usuario Usuario = new Usuario();
        readonly int Todos = 0;

        // GET: Usuario
        public ActionResult Index()
        {
            ViewBag.XMLMenu = Menu.ComponeMenu(int.Parse(Session["usuid"].ToString()), Session["pfnum"].ToString());
            var usuariomodel = new UsuarioModel(int.Parse(Session["usuid"].ToString()), Todos);
            return View(usuariomodel);
        }

        public ActionResult Usuarios(int UsuarioID, string Tipo)
        {
            ViewBag.XMLMenu = Menu.ComponeMenu(int.Parse(Session["usuid"].ToString()), Session["pfnum"].ToString());
            var usuarioModel = new UsuarioModel(int.Parse(Session["usuid"].ToString()), UsuarioID);

            if (Tipo == "Elimina")
            {
                Guardar(usuarioModel, Tipo);
                return RedirectToAction("Index", "Usuario");
            }
            else
            {
                ViewBag.Operacion = Tipo;
                return View(usuarioModel);
            }
        }

        [HttpPost]
        public ActionResult Guardar(UsuarioModel usuarioModel, string Tipo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    usuarioModel.ListaAreas = usuarioModel.Areas(int.Parse(Session["usuid"].ToString()));
                    usuarioModel.ListaPerfiles = usuarioModel.Perfiles(int.Parse(Session["usuid"].ToString()));
                    usuarioModel.ListaEstatus = usuarioModel.Estatus(int.Parse(Session["usuid"].ToString()));
                    ViewBag.XMLMenu = Menu.ComponeMenu(int.Parse(Session["usuid"].ToString()), Session["pfnum"].ToString());
                    ViewBag.Operacion = Tipo;
                    return View("Usuarios", usuarioModel);
                }

                Usuario.EditaUsuario(
                    int.Parse(Session["usuid"].ToString()),
                    Tipo,
                    usuarioModel.Identificador,
                    usuarioModel.Nombre,
                    string.IsNullOrEmpty(usuarioModel.Apaterno) ? "" : usuarioModel.Apaterno,
                    string.IsNullOrEmpty(usuarioModel.Amaterno) ? "" : usuarioModel.Amaterno,
                    usuarioModel.Nic,
                    "",
                    string.IsNullOrEmpty(usuarioModel.Correo) ? "" : usuarioModel.Correo,
                    usuarioModel.AreaID,
                    usuarioModel.PerfilID,
                    usuarioModel.EstatusID);
                return RedirectToAction("Index", "Usuario");
            }
            catch
            {
                return View();
            }
        }
    }
}
