using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Data;
using System.Data.SqlClient;
using DependenciaMVC5demo.com.basedatos;

namespace DependenciaMVC5demo.com.negocio
{
    public class Estatus:BaseDatos
    {
        private string SPOBTENESTATUS = "Sp_ObtenEstatus";

        private XmlDocument estatusXML;

        public XmlDocument EstatusXML
        {
            get { return estatusXML; }
        }

        public bool ObtenEstatus(int usuarioid)
        {
            bool resp = false;
            try
            {
                PrepareSp(SPOBTENESTATUS);
                CargaCmdParameter("UsuarioID", SqlDbType.Int, 8, ParameterDirection.Input, usuarioid);
                SqlDataReader objrst = GetResultStoredSQLReader();
                if (objrst.Read())
                {
                    estatusXML = new XmlDocument();
                    estatusXML.LoadXml("<xml>" + objrst[0].ToString() + "</xml>");
                    XmlNode select = estatusXML.DocumentElement.SelectSingleNode("Estatus");
                    resp = select.HasChildNodes;
                }
                else
                    resp = false;
                Close();
                if (resp)
                    EscribeLog("Obtención correcta estatus usuario:" + usuarioid);
                else
                    EscribeLog("Obtención incorrecta estatus usuario:" + usuarioid);
            }
            catch (Exception exx)
            {
                EscribeLog("Estatus.ObtenEstatus " + exx.Message.ToString());
                resp = false;
            }
            return resp;
        }
    }
}