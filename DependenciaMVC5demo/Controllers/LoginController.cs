using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Web.Mvc;
using System.Net;
using System.Configuration;
using DependenciaMVC5demo.com.negocio;
using DependenciaMVC5demo.com.utilerias;
using DependenciaMVC5demo.Models;

namespace DependenciaMVC5demo.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Login(LoginModel model)
        {
            Usuario usobj = new Usuario();
            LoginModel loginModel = new LoginModel();
            string Autenticacion = string.Empty;

            bool valido = false;
            if (ModelState.IsValid)
            {

                //ServicePointManager.Expect100Continue = true;
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;
                //ServicePointManager.ServerCertificateValidationCallback = (snder, cert, chain, error) => true;

                //Autenticacion.SI_autenticaUsuario_SOService AutenticaService = new Autenticacion.SI_autenticaUsuario_SOService();
                //AutenticaService.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["UsuarioServicio"].ToString(), ConfigurationManager.AppSettings["Autenticacion"].ToString());
                //AutenticaService.PreAuthenticate = true;
                //Autenticacion = AutenticaService.SI_autenticaUsuario_SO(model.Nic.ToUpper(), model.Contrasenia);

                //if (Autenticacion.IndexOf("Password verificado:") > -1)
                //{
                if (usobj.ValidaUsuario(model.Nic, model.Contrasenia))
                {
                    //usobj.EditaUsuario(0, "Inserta", 0, model.Nic, "", "", model.Nic,"","",1,1,1);
                    //usobj.ValidaUsuario(model.Nic, model.Contrasenia);


                    XmlNode select = usobj.UsuarioXML.DocumentElement.SelectSingleNode("Usuario");
                    XmlNode row = select.ChildNodes[0];
                    Session["usuid"] = row.Attributes["UsuarioID"].Value.ToString();
                    @TempData["usuname"] = row.Attributes["Nombre"].Value.ToString() + " " + row.Attributes["Apellido_Paterno"].Value.ToString() + " " + row.Attributes["Apellido_Materno"].Value.ToString() + " - " + row.Attributes["pNombre"].Value.ToString();
                    Session["pfnum"] = row.Attributes["Permisos"].Value.ToString();
                    valido = true;
                }
                //}
            }
            if (valido)
                return RedirectToAction("Index", "Proceso");
            else
            {
                return (HttpNotFound());
                //ModelState.AddModelError(string.Empty, Autenticacion);
                //return View("Index", loginModel);
            }
        }
    }
}