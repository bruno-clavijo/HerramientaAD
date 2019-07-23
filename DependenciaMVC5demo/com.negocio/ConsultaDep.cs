using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Xml;
using System.Data.SqlClient;
using DependenciaMVC5demo.com.basedatos;
using DependenciaMVC5demo.com.dto;

namespace DependenciaMVC5demo.com.negocio
{
    public class ConsultaDep:BaseDatos
    {
        private string SPCONSULTABD = "Sp_ConsultaDependenciaBD";
        private string SPCONSULTAWS = "Sp_ConsultaDependenciaWS";
        private string SPCONSULTACM = "Sp_ConsultaDependenciaCM";

        private XmlDocument consultaXML;

        public XmlDocument ConsultaXML
        {
            get { return consultaXML; }
        }

        public bool VerificaDatosConsulta()
        {
            bool resp = true;
            return resp;
        }

        public bool ConsultaDependencia()
        {
            bool resp = false;
            try
            {
                PrepareSp(SPCONSULTABD);
                CargaCmdParameter("aplicacionid", SqlDbType.Int, 8, ParameterDirection.Input, 0);
                CargaCmdParameter("basedatos", SqlDbType.Int, 8, ParameterDirection.Input, 0);
                CargaCmdParameter("objetodb", SqlDbType.Int, 8, ParameterDirection.Input, 0);
                CargaCmdParameter("archivo", SqlDbType.VarChar, 150, ParameterDirection.Input, "");
                CargaCmdParameter("numerolinea", SqlDbType.Int, 8, ParameterDirection.Input, 0);
                SqlDataReader objrst = GetResultStoredSQLReader();
                if (objrst.Read())
                {
                    consultaXML = new XmlDocument();
                    consultaXML.LoadXml("<xml>" + objrst[0].ToString() + "</xml>");
                    XmlNode select = consultaXML.DocumentElement.SelectSingleNode("Consulta");
                    resp = select.HasChildNodes;
                }
                else
                    resp = false;
                Close();
                if (resp)
                    EscribeLog("Consulta correcta aplicacion " + 0);
                else
                    EscribeLog("Consulta incorrecta aplicacion " + 0);
            }
            catch (Exception exx)
            {
                EscribeLog("ConsultaDep.ConsultaDependencia " + exx.Message.ToString());
                resp = false;
            }
            return resp;
        }


        public bool ConsultaDependenciaFiltro(string Tipo, int AplicacionID, string Filtro1, string Filtro2, string Filtro3, string Filtro4)
        {
            bool resp = false;
            try
            {
                switch (Tipo)
                {
                    case "BD":
                        if (string.IsNullOrEmpty(Filtro1) || Filtro1 == "--select--") Filtro1 = "0";
                        if (string.IsNullOrEmpty(Filtro2) || Filtro2 == "--select--") Filtro2 = "0";
                        if (string.IsNullOrEmpty(Filtro3) || Filtro3 == "--select--") Filtro3 = "";
                        if (string.IsNullOrEmpty(Filtro4) || Filtro4 == "--select--") Filtro4 = "0";

                        PrepareSp(SPCONSULTABD);
                        CargaCmdParameter("basedatos", SqlDbType.Int, 8, ParameterDirection.Input, int.Parse(Filtro1));
                        CargaCmdParameter("objetodb", SqlDbType.Int, 8, ParameterDirection.Input, int.Parse(Filtro2));
                        CargaCmdParameter("archivo", SqlDbType.VarChar, 150, ParameterDirection.Input, Filtro3);
                        CargaCmdParameter("numerolinea", SqlDbType.Int, 8, ParameterDirection.Input, int.Parse(Filtro4));
                        break;

                    case "WS":
                        if (string.IsNullOrEmpty(Filtro1) || Filtro1 == "--select--") Filtro1 = "";
                        if (string.IsNullOrEmpty(Filtro2) || Filtro2 == "--select--") Filtro2 = "";
                        if (string.IsNullOrEmpty(Filtro3) || Filtro3 == "--select--") Filtro3 = "";
                        if (string.IsNullOrEmpty(Filtro4) || Filtro4 == "--select--") Filtro4 = "";

                        PrepareSp(SPCONSULTAWS);
                        CargaCmdParameter("tipohijo", SqlDbType.VarChar, 50, ParameterDirection.Input, Filtro1);
                        CargaCmdParameter("middleware", SqlDbType.VarChar, 50, ParameterDirection.Input, Filtro2);
                        CargaCmdParameter("url", SqlDbType.VarChar, 200, ParameterDirection.Input, Filtro3);
                        CargaCmdParameter("archivo", SqlDbType.VarChar, 150, ParameterDirection.Input, Filtro4);
                        break;

                    case "CM":
                        if (string.IsNullOrEmpty(Filtro1) || Filtro1 == "--select--") Filtro1 = "";
                        if (string.IsNullOrEmpty(Filtro2) || Filtro2 == "--select--") Filtro2 = "";
                        if (string.IsNullOrEmpty(Filtro3) || Filtro3 == "--select--") Filtro3 = "";
                        if (string.IsNullOrEmpty(Filtro4) || Filtro4 == "--select--") Filtro4 = "0";

                        PrepareSp(SPCONSULTACM);
                        CargaCmdParameter("tipohijo", SqlDbType.VarChar, 50, ParameterDirection.Input, Filtro1);
                        CargaCmdParameter("archivo", SqlDbType.VarChar, 150, ParameterDirection.Input, Filtro2);
                        CargaCmdParameter("lenguajeapp", SqlDbType.VarChar, 50, ParameterDirection.Input, Filtro3);
                        CargaCmdParameter("numlinea", SqlDbType.Int, 8, ParameterDirection.Input, int.Parse(Filtro4));
                        break;
                }

                CargaCmdParameter("aplicacionid", SqlDbType.Int, 8, ParameterDirection.Input, AplicacionID);
                SqlDataReader objrst = GetResultStoredSQLReader();
                if (objrst.Read())
                {
                    consultaXML = new XmlDocument();
                    consultaXML.LoadXml("<xml>" + objrst[0].ToString() + "</xml>");
                    XmlNode select = consultaXML.DocumentElement.SelectSingleNode("Consulta");
                    resp = select.HasChildNodes;
                }
                else
                    resp = false;
                Close();
                if (resp)
                    EscribeLog("Consulta correcta apliación: " + Tipo + " " + AplicacionID);
                else
                    EscribeLog("Consulta incorrecta aplicación: " + Tipo + " " + AplicacionID);
            }
            catch (Exception exx)
            {
                EscribeLog("ConsultaDep.ConsultaDependencia " + exx.Message.ToString());
                resp = false;
            }
            return resp;
        }
    }
}