using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Data;
using System.Data.SqlClient;
using DependenciaMVC5demo.com.basedatos;
using DependenciaMVC5demo.com.dto;

namespace DependenciaMVC5demo.com.negocio
{
    public class Area:BaseDatos
    {
        private string SPOBTENAREA = "Sp_ObtenArea";

        private XmlDocument areaXML;

        public XmlDocument AreaXML
        {
            get { return areaXML; }
        }

        public bool ObtenArea(int usuarioid)
        {
            bool resp = false;
            try
            {
                PrepareSp(SPOBTENAREA);
                CargaCmdParameter("UsuarioID", SqlDbType.Int, 8, ParameterDirection.Input, usuarioid);
                SqlDataReader objrst = GetResultStoredSQLReader();
                if (objrst.Read())
                {
                    areaXML = new XmlDocument();
                    areaXML.LoadXml("<xml>" + objrst[0].ToString() + "</xml>");
                    XmlNode select = areaXML.DocumentElement.SelectSingleNode("Areas");
                    resp = select.HasChildNodes;
                }
                else
                    resp = false;
                Close();
                if (resp)
                    EscribeLog("Obtención correcta areas usuario:" + usuarioid);
                else
                    EscribeLog("Obtención incorrecta areas usuario:" + usuarioid);
            }
            catch (Exception exx)
            {
                EscribeLog("Area.ObtenArea " + exx.Message.ToString());
                resp = false;
            }
            return resp;
        }
    }
}