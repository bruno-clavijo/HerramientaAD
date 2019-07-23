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
    public class AnalisisModel
    {        
        public AnalisisModel()
        {
            Area areaobj = new Area();

            if (areaobj.ObtenArea(1))
            {
                XmlNode select = areaobj.AreaXML.DocumentElement.SelectSingleNode("Areas");
                areasList.Add(new Utilerias.ItemsCombo(0, "--select--"));
                aplicacionesList.Add(new Utilerias.ItemsCombo(0, "--select--"));
                filtro1List.Add(new Utilerias.ItemsCombo(0, "--select--"));
                filtro2List.Add(new Utilerias.ItemsCombo(0, "--select--"));
                filtro3List.Add(new Utilerias.ItemsCombo(0, "--select--"));
                filtro4List.Add(new Utilerias.ItemsCombo(0, "--select--"));
                foreach (XmlNode area in select.SelectNodes("row"))
                {
                    areasList.Add(new Utilerias.ItemsCombo(int.Parse(area.Attributes["AreaID"].Value.ToString()), area.Attributes["Nombre"].Value.ToString()));
                }
            }


            ConsultaDep consultaobj = new ConsultaDep();
            if (consultaobj.ConsultaDependencia())
            {
                xdetalle = consultaobj.ConsultaXML;
            }
        }

        private List<Utilerias.ItemsCombo> filtro4List = new List<Utilerias.ItemsCombo>();
        public List<Utilerias.ItemsCombo> Filtro4List { get => filtro4List; set => filtro4List = value; }

        private List<Utilerias.ItemsCombo> filtro3List = new List<Utilerias.ItemsCombo>();
        public List<Utilerias.ItemsCombo> Filtro3List { get => filtro3List; set => filtro3List = value; }

        private List<Utilerias.ItemsCombo> filtro2List = new List<Utilerias.ItemsCombo>();
        public List<Utilerias.ItemsCombo> Filtro2List { get => filtro2List; set => filtro2List = value; }

        private List<Utilerias.ItemsCombo> filtro1List = new List<Utilerias.ItemsCombo>();
        public List<Utilerias.ItemsCombo> Filtro1List { get => filtro1List; set => filtro1List = value; }

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

        private string filtro1;
        public string Filtro1
        {
            get { return filtro1; }
            set { filtro1 = value; }
        }

        private int filtro2;
        public int Filtro2
        {
            get { return filtro2; }
            set { filtro2 = value; }
        }

        private int filtro3;
        public int Filtro3
        {
            get { return filtro3; }
            set { filtro3 = value; }
        }

        private int filtro4;
        public int Filtro4
        {
            get { return filtro4; }
            set { filtro4 = value; }
        }

        private XmlDocument xdetalle;
        public XmlDocument Xdetalle { get => xdetalle; set => xdetalle = value; }
        
    }
}