using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using DependenciaMVC5demo.com.negocio;
using DependenciaMVC5demo.com.utilerias;

namespace DependenciaMVC5demo.Models
{
    public class Dependencias
    {
        public long procesaid;

        public Dependencias()
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
            lcub.Add(new Utilerias.ItemsCubo("", "CuboCara(1)",""));
            lcub.Add(new Utilerias.ItemsCubo("", "CuboCara(2)", ""));
            lcub.Add(new Utilerias.ItemsCubo("", "CuboCara(3)", ""));
            lcub.Add(new Utilerias.ItemsCubo("", "CuboCara(4)", ""));
            lcub.Add(new Utilerias.ItemsCubo("", "CuboCara(5)", ""));
            lcub.Add(new Utilerias.ItemsCubo("", "CuboCara(6)", ""));

            XobjetosDB = new XmlDocument();
            XobjetosDB.LoadXml("<xml><ObjetosDB><row tipo='' uso='' ObjetoID='' Objeto=''/></ObjetosDB></xml>");                       
         }

        public Dependencias(int idtipo, int appid, int usuarioid, int procesoid) {
            ObtenCubo(idtipo, appid, usuarioid, procesoid);
        }

        public Dependencias(int idtipo, int appid, int usuarioid, string  indicador)
        {
            Aplicacion objapp = new Aplicacion();
            objapp.ObtenTransversalidad(usuarioid, appid, idtipo, indicador,2);
            XobjetosDB = objapp.AplicaionXML;
        }

        public Dependencias(int appid, int usuarioid,string nombre) {
            Aplicacion objapp = new Aplicacion();
            objapp.ObtenObjetosDB3(usuarioid, appid, nombre);            
                XobjetosDB = objapp.AplicaionXML;            
        }

        public bool ObtenCubo(int idtipo, int appid, int usuarioid, int procesoid) {
            bool exito = false;
            try {
                Aplicacion appobj = new Aplicacion();
                if (appobj.ObtenIndicaores(usuarioid,appid,procesoid,idtipo)) {
                    procesaid = appobj.procesoID;
                    lcub.Clear();
                    XmlNode select = appobj.AplicaionXML.DocumentElement.SelectSingleNode("Indicadores");                    
                    foreach (XmlNode area in select.SelectNodes("row"))
                    {
                        if (lcub.Count < 6 ) 
                            lcub.Add(new Utilerias.ItemsCubo(area.Attributes["Registros"].Value.ToString(), "CuboCara('" + area.Attributes["TipoObjeto"].Value.ToString() + "'," + idtipo + "," + appid + ")", area.Attributes["TipoObjeto"].Value.ToString()));
                    }

                    for (int i = lcub.Count; i < 6; i++)
                    {
                        lcub.Add(new Utilerias.ItemsCubo("", "", ""));
                    }
                }
            }
            catch(Exception) {

            }
            return exito;        
        }

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