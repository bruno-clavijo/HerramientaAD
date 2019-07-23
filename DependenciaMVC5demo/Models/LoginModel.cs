using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml;

namespace DependenciaMVC5demo.Models
{
    public class LoginModel
    {
        private string usuarioID;
        private string nic=string.Empty;
        private string contrasenia = string.Empty;
        private XmlDocument xmenu;


        public string UsuarioID { get => usuarioID; set => usuarioID = value; }
        [Required(ErrorMessage = "Usuario es requerido.")]
        public string Nic { get => nic; set => nic = value; }
        [Required(ErrorMessage = "Contraseña es requerida.")]
        public string Contrasenia { get => contrasenia; set => contrasenia = value; }
        public XmlDocument XMenu { get => xmenu; set => xmenu = value; }
    }
}