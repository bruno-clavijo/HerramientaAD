using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.ComponentModel.DataAnnotations;
using DependenciaMVC5demo.com.utilerias;
using DependenciaMVC5demo.com.negocio;

namespace DependenciaMVC5demo.Models
{
    public class UsuarioModel
    {
        Aplicaciones aplicaciones = new Aplicaciones();
        Area area = new Area();
        Usuario usuario = new Usuario();
        Estatus estatus = new Estatus();
        Perfiles perfiles = new Perfiles();

        private int identificador;
        public int Identificador { get => identificador; set => identificador = value; }

        private string nombre;
        [Required(ErrorMessage = "Nombre es requerido.")]
        [StringLength(50, ErrorMessage = "Longitud máxima de 50.")]
        public string Nombre { get => nombre; set => nombre = value; }

        private string apaterno;
        [Required(ErrorMessage = "Apellido Paterno es requerido.")]
        [StringLength(50, ErrorMessage = "Longitud máxima de 50.")]
        public string Apaterno { get => apaterno; set => apaterno = value; }

        private string amaterno;
        [StringLength(50, ErrorMessage = "Longitud máxima de 50.")]
        public string Amaterno { get => amaterno; set => amaterno = value; }

        private string nic;
        [Required(ErrorMessage = "Nic es requerido.")]
        [StringLength(100, ErrorMessage = "Longitud máxima de 100.")]
        public string Nic { get => nic; set => nic = value; }

        private string contrasenia;
        public string Contrasenia { get => contrasenia; set => contrasenia = value; }

        private string correo;
        [StringLength(100, ErrorMessage = "Longitud máxima de 100.")]
        public string Correo { get => correo; set => correo = value; }

        private int areaid;
        [Required(ErrorMessage = "Área es requerido.")]
        public int AreaID { get => areaid; set => areaid = value; }

        private int perfilid;
        [Required(ErrorMessage = "Perfil es requerido.")]
        public int PerfilID { get => perfilid; set => perfilid = value; }

        private int estatusid;
        [Required(ErrorMessage = "Estatus es requerido.")]
        public int EstatusID { get => estatusid; set => estatusid = value; }

        private List<Utilerias.ItemsCombo> listaareas = new List<Utilerias.ItemsCombo>();
        public List<Utilerias.ItemsCombo> ListaAreas
        {
            get { return listaareas; }
            set { listaareas = value; }
        }

        private List<Utilerias.ItemsCombo> listaperfiles = new List<Utilerias.ItemsCombo>();
        public List<Utilerias.ItemsCombo> ListaPerfiles
        {
            get { return listaperfiles; }
            set { listaperfiles = value; }
        }

        private List<Utilerias.ItemsCombo> listaestatus = new List<Utilerias.ItemsCombo>();
        public List<Utilerias.ItemsCombo> ListaEstatus
        {
            get { return listaestatus; }
            set { listaestatus = value; }
        }

        private XmlDocument usuariosXML;
        public XmlDocument UsuariosXML { get => usuariosXML; set => usuariosXML = value; }

        public UsuarioModel()
        {

        }

        public List<Utilerias.ItemsCombo> Areas(int UsuarioID)
        {
            if (area.ObtenArea(UsuarioID))
            {
                foreach (XmlNode areanode in area.AreaXML.DocumentElement.SelectSingleNode("Areas").SelectNodes("row"))
                {
                    listaareas.Add(new Utilerias.ItemsCombo(int.Parse(areanode.Attributes["AreaID"].Value.ToString()), areanode.Attributes["Nombre"].Value.ToString()));
                }
            }
            return listaareas;
        }

        public List<Utilerias.ItemsCombo> Perfiles(int UsuarioID)
        {
            if (perfiles.ObtenPerfiles(UsuarioID, 0))
            {
                foreach (XmlNode usuarionode in perfiles.PerfilesXML.DocumentElement.SelectSingleNode("Perfiles").SelectNodes("row"))
                {
                    listaperfiles.Add(new Utilerias.ItemsCombo(int.Parse(usuarionode.Attributes["PerfilID"].Value.ToString()), usuarionode.Attributes["Nombre"].Value.ToString()));
                }
            }
            return listaperfiles;
        }

        public List<Utilerias.ItemsCombo> Estatus(int UsuarioID)
        {
            if (estatus.ObtenEstatus(UsuarioID))
            {
                foreach (XmlNode estatusnode in estatus.EstatusXML.DocumentElement.SelectSingleNode("Estatus").SelectNodes("row"))
                {
                    listaestatus.Add(new Utilerias.ItemsCombo(int.Parse(estatusnode.Attributes["EstatusID"].Value.ToString()), estatusnode.Attributes["Estatus"].Value.ToString()));
                }
            }
            return listaestatus;
        }

        public UsuarioModel(int UsuarioID, int UsuarioBuscado)
        {
            if (usuario.ObtenUsuario(UsuarioID, UsuarioBuscado))
            {
                usuariosXML = usuario.UsuarioXML;                
            }

            Areas(UsuarioID);
            Perfiles(UsuarioID);
            Estatus(UsuarioID);

            if (UsuarioBuscado != 0)
            {
                foreach (XmlNode usuarionode in usuario.UsuarioXML.DocumentElement.SelectSingleNode("Usuarios").SelectNodes("row"))
                {
                    identificador = int.Parse(usuarionode.Attributes["UsuarioID"].Value.ToString());
                    nombre = usuarionode.Attributes["Nombre"].Value.ToString();
                    apaterno = usuarionode.Attributes["Apellido_Paterno"].Value.ToString();
                    amaterno = usuarionode.Attributes["Apellido_Materno"].Value.ToString();
                    nic = usuarionode.Attributes["Nic"].Value.ToString();
                    correo = usuarionode.Attributes["Correo"].Value.ToString();
                    areaid = int.Parse(usuarionode.Attributes["AreaID"].Value.ToString());
                    perfilid = int.Parse(usuarionode.Attributes["PerfilID"].Value.ToString());
                    estatusid = int.Parse(usuarionode.Attributes["EstatusID"].Value.ToString());
                }
            }   
        }
    }
}