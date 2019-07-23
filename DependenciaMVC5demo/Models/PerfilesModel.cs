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
    public class PerfilesModel
    {
        Estatus estatus = new Estatus();
        Perfiles perfiles = new Perfiles();

        private int identificador;
        public int Identificador { get => identificador; set => identificador = value; }
        
        private string perfil;
        [Required(ErrorMessage = "Nombre es requerido.")]
        [StringLength(50, ErrorMessage = "Longitud máxima de 50.")]
        public string Perfil { get => perfil; set => perfil = value; }
        
        private bool opcionanalisis;
        public bool OpcionAnalisis { get => opcionanalisis; set => opcionanalisis = value; }
        
        private bool opciondependencias;
        public bool OpcionDependencias { get => opciondependencias; set => opciondependencias = value; }

        private bool opciondetalle;
        public bool OpcionDetalle { get => opciondetalle; set => opciondetalle = value; }

        private bool opciongraficas;
        public bool OpcionGraficas { get => opciongraficas; set => opciongraficas = value; }

        private bool opcionusuarios;
        public bool OpcionUsuarios { get => opcionusuarios; set => opcionusuarios = value; }

        private bool opcionperfiles;
        public bool OpcionPerfiles { get => opcionperfiles; set => opcionperfiles = value; }

        private bool opcionaplicaciones;
        public bool OpcionAplicaciones { get => opcionaplicaciones; set => opcionaplicaciones = value; }

        private int estatusid;
        [Required(ErrorMessage = "Estatus es requerido.")]
        public int EstatusID { get => estatusid; set => estatusid = value; }

        private List<Utilerias.ItemsCombo> listaestatus = new List<Utilerias.ItemsCombo>();
        public List<Utilerias.ItemsCombo> ListaEstatus
        {
            get { return listaestatus; }
            set { listaestatus = value; }
        }

        private XmlDocument perfilesxml = new XmlDocument();
        public XmlDocument PerfilesXML { get => perfilesxml; set => perfilesxml = value; }

        public PerfilesModel()
        {

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

        public PerfilesModel(int UsuarioID, int PerfilBuscado)
        {         
            if (perfiles.ObtenPerfiles(UsuarioID, PerfilBuscado))
            {
                perfilesxml = perfiles.PerfilesXML;
            }

            Estatus(UsuarioID);

            if (PerfilBuscado != 0)
            {
                foreach (XmlNode xmlNode in perfiles.PerfilesXML.DocumentElement.SelectSingleNode("Perfiles").SelectNodes("row"))
                {
                    identificador = int.Parse(xmlNode.Attributes["PerfilID"].Value.ToString());
                    perfil = xmlNode.Attributes["Nombre"].Value.ToString();
                    opcionanalisis = xmlNode.Attributes["Analisis"].Value.ToString() == "SI" ? true : false;
                    opciondependencias = xmlNode.Attributes["Dependencias"].Value.ToString() == "SI" ? true : false;
                    opciondetalle = xmlNode.Attributes["Detalle"].Value.ToString() == "SI" ? true : false;
                    opciongraficas = xmlNode.Attributes["Graficas"].Value.ToString() == "SI" ? true : false;
                    opcionusuarios = xmlNode.Attributes["ABMUsuarios"].Value.ToString() == "SI" ? true : false;
                    opcionperfiles = xmlNode.Attributes["ABMPerfiles"].Value.ToString() == "SI" ? true : false;
                    opcionaplicaciones = xmlNode.Attributes["ABMAplicaciones"].Value.ToString() == "SI" ? true : false;
                    estatusid = int.Parse(xmlNode.Attributes["EstatusID"].Value.ToString());
                }
            }
        }
    }
}