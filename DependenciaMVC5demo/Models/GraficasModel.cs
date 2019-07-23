using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using DependenciaMVC5demo.com.negocio;
using DependenciaMVC5demo.com.utilerias;
using System.Web.Mvc;

namespace DependenciaMVC5demo.Models
{
    public class GraficasModel
    {

        public GraficasModel()
        {
            Area areaobj = new Area();
            if (areaobj.ObtenArea(1))
            {
                XmlNode select = areaobj.AreaXML.DocumentElement.SelectSingleNode("Areas");
                areasList.Add(new Utilerias.ItemsCombo(0, "--select--"));
                aplicacionesList.Add(new Utilerias.ItemsCombo(0, "--select--"));
                foreach (XmlNode area in select.SelectNodes("row"))
                {
                    areasList.Add(new Utilerias.ItemsCombo(int.Parse(area.Attributes["AreaID"].Value.ToString()), area.Attributes["Nombre"].Value.ToString()));
                }
            }


            XobjetosDB = new XmlDocument();
            XobjetosDB.LoadXml("<xml><ObjetosDB><row tipo='' uso='' ObjetoID='' Objeto=''/></ObjetosDB></xml>");

            NombreApp = "Todas las aplicaciones.";
            aplicacionID = 0;
        }

        private string nombreApp=string.Empty;
        public string NombreApp { get => nombreApp; set => nombreApp = value; }

        private List<Utilerias.ItemsCombo> areasList = new List<Utilerias.ItemsCombo>();
        public List<Utilerias.ItemsCombo> AreasList
        {
            get { return areasList; }
            set { areasList = value; }
        }

        private List<Utilerias.ItemsCombo> aplicacionesList = new List<Utilerias.ItemsCombo>();
        public List<Utilerias.ItemsCombo> AplicacionesList
        {
            get { return aplicacionesList; }
            set { aplicacionesList = value; }
        }

        private int areaID;
        public int AreaID
        {
            get { return areaID; }
            set { areaID = value; }
        }

        private int aplicacionID;
        public int AplicacionID
        {
            get { return aplicacionID; }
            set { aplicacionID = value; }
        }

        private List<Utilerias.ItemsCubo> lcub = new List<Utilerias.ItemsCubo>();
        public List<Utilerias.ItemsCubo> Lcub { get => lcub; set => lcub = value; }

        private XmlDocument xobjetosDB;
        public XmlDocument XobjetosDB { get => xobjetosDB; set => xobjetosDB = value; }
        
    }
}