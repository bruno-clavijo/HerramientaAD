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
    public class AplicacionesModel
    {
        Aplicaciones aplicaciones = new Aplicaciones();
        Area area = new Area();
        Estatus estatus = new Estatus();

        private int identificador;
        public int Identificador { get => identificador; set => identificador = value; }


        private string aplicacion;
        [Required(ErrorMessage = "Nombre Corto es requerido.")]
        [StringLength(100, ErrorMessage = "Longitud máxima de 100.")]
        public string Aplicacion { get => aplicacion; set => aplicacion = value; }


        private string descripcion;
        [StringLength(300, ErrorMessage = "Longitud máxima de 300.")]
        public string Descripcion { get => descripcion; set => descripcion = value; }

        private int lenguajeid;
        [Required(ErrorMessage = "Lenguaje es requerido.")]
        public int LenguajeID { get => lenguajeid; set => lenguajeid = value; }

        private string claveaplicacion;
        [StringLength(20, ErrorMessage = "Longitud máxima de 20.")]
        public string ClaveAplicacion { get => claveaplicacion; set => claveaplicacion = value; }

        private int estatusid;
        [Required(ErrorMessage = "Estatus es requerido.")]
        public int EstatusID { get => estatusid; set => estatusid = value; }

        private int areaid;
        [Required(ErrorMessage = "Área es requerido.")]
        public int AreaID { get => areaid; set => areaid = value; }

        private List<Utilerias.ItemsCombo> listaareas = new List<Utilerias.ItemsCombo>();
        public List<Utilerias.ItemsCombo> ListaAreas
        {
            get { return listaareas; }
            set { listaareas = value; }
        }

        private List<Utilerias.ItemsCombo> listalenguajes = new List<Utilerias.ItemsCombo>();
        public List<Utilerias.ItemsCombo> ListaLenguajes
        {
            get { return listalenguajes; }
            set { listalenguajes = value; }
        }

        private List<Utilerias.ItemsCombo> listaestatus = new List<Utilerias.ItemsCombo>();
        public List<Utilerias.ItemsCombo> ListaEstatus
        {
            get { return listaestatus; }
            set { listaestatus = value; }
        }

        private XmlDocument aplicacionesXML;
        public XmlDocument AplicacionesXML { get => aplicacionesXML; set => aplicacionesXML = value; }

        public AplicacionesModel()
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

        public List<Utilerias.ItemsCombo> Lenguajes(int UsuarioID)
        {
            if (aplicaciones.ObtenLenguajes(UsuarioID))
            {
                foreach (XmlNode lenguajenode in aplicaciones.LenguajesXML.DocumentElement.SelectSingleNode("Lenguajes").SelectNodes("row"))
                {
                    listalenguajes.Add(new Utilerias.ItemsCombo(int.Parse(lenguajenode.Attributes["LenguajeID"].Value.ToString()), lenguajenode.Attributes["Lenguaje"].Value.ToString()));
                }
            }
            return listalenguajes;
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

        public AplicacionesModel(int UsuarioID, int AreaID, int AplicacionID)
        {
            if (aplicaciones.ObtenAplicaciones(UsuarioID, AreaID, AplicacionID))
            {
                aplicacionesXML = aplicaciones.AplicacionesXML;
            }

            Areas(UsuarioID);
            Lenguajes(UsuarioID);
            Estatus(UsuarioID);

            if (AplicacionID != 0)
            {
                foreach (XmlNode xmlNode in aplicaciones.AplicacionesXML.DocumentElement.SelectSingleNode("Aplicaciones").SelectNodes("row"))
                {
                    identificador = int.Parse(xmlNode.Attributes["AplicacionID"].Value.ToString());
                    aplicacion = xmlNode.Attributes["Aplicacion"].Value.ToString();
                    descripcion = xmlNode.Attributes["Descripcion"].Value.ToString();
                    lenguajeid = int.Parse(xmlNode.Attributes["LenguajeID"].Value.ToString());
                    claveaplicacion = xmlNode.Attributes["ClaveAplicacion"].Value.ToString();
                    estatusid = int.Parse(xmlNode.Attributes["EstatusID"].Value.ToString());
                    areaid = int.Parse(xmlNode.Attributes["AreaID"].Value.ToString());
                }
            }
        }
    }
}