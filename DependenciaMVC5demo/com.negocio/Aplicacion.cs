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
    public class Aplicacion:BaseDatos
    {
        private string SPOBTENAPP = "Sp_ObtenAplicacion";
        private string SPOBTENPROC = "Sp_ObtenProceso";
        private string SPOBTOBJDB = "Sp_ObtenObjetosDB";
        private string SPOBTOBJDB2 = "Sp_ObtenObjetosDBD";
        private string SPOBTOBJDB3 = "Sp_ObtenObjetosDBD3";
        private string SPOBTOBJDB4 = "Sp_ObtenObjetosDBD4";
        private string SPOBTINDICADORES = "sp_Cubo_Consulta_General";
        private string SPOBTRELTablas = "Sp_ObtenRelacionesDB";
        private string SPOBTRELDEP = "Sp_ObtenRelacionDepDB";
        private string SPOBTRANSVERSAL = "Sp_ObtenTransversal";
        private string SPOBTRANSVERSALD = "Sp_ObtenTransversalDetalle";
        private string SPOBTRANSVERSALDpn = "Sp_ObtenTransversalDependencia";
        private string SPFILTROSDB = "sp_FiltrosDB";
        private string SPFILTROSWS = "sp_FiltrosWS";
        private string SPFILTROSCM = "sp_FiltrosCM";

        private XmlDocument aplicaionXML;
        public long procesoID ;

        public XmlDocument AplicaionXML
        {
            get { return aplicaionXML; }
        }

        public long ObtenerProcesoID(int appid) {
            long procesoid = 0;
            try
            {
                PrepareSp(SPOBTENPROC);
                CargaCmdParameter("AplicacionID", SqlDbType.Int, 8, ParameterDirection.Input, appid);                
                SqlDataReader objrst = GetResultStoredSQLReader();
                if (objrst.Read())
                {
                    procesoid= objrst.GetInt64(0);                  
                }
                else
                    procesoid = 0;
                Close();
                return procesoid;
            }
            catch (Exception exx)
            {
                EscribeLog("Aplicacion.ObtenerProcesoID " + exx.Message.ToString());
                return 0;
            }           
        }

        public bool ObtenFiltros(string Filtro, string Tipo, int AplicacionID, string Filtro1, string Filtro2, string Filtro3) {
            bool resp = false;
            try
            {
                switch (Filtro)
                {
                    case "BD":
                        if (Filtro1 == "") Filtro1 = "0";
                        if (Filtro2 == "") Filtro2 = "0";
                        PrepareSp(SPFILTROSDB);
                        CargaCmdParameter("basedatosid", SqlDbType.Int, 8, ParameterDirection.Input, int.Parse(Filtro1));
                        CargaCmdParameter("objetoid", SqlDbType.Int, 8, ParameterDirection.Input, int.Parse(Filtro2));
                        CargaCmdParameter("archivo", SqlDbType.VarChar, 150, ParameterDirection.Input, Filtro3);
                        break;
                    case "WS":
                        PrepareSp(SPFILTROSWS);
                        break;
                    case "CM":
                        PrepareSp(SPFILTROSCM);
                        break;
                    default:
                        PrepareSp(SPFILTROSDB);
                        break;
                }

                CargaCmdParameter("tipo", SqlDbType.VarChar, 100, ParameterDirection.Input, Tipo);
                CargaCmdParameter("aplicacionid", SqlDbType.Int, 8, ParameterDirection.Input, AplicacionID);
                
                SqlDataReader objrst = GetResultStoredSQLReader();
                if (objrst.Read())
                {
                    aplicaionXML = new XmlDocument();
                    aplicaionXML.LoadXml("<xml>" + objrst[0].ToString() + "</xml>");
                    XmlNode select = aplicaionXML.DocumentElement.SelectSingleNode("Filtros");
                    resp = select.HasChildNodes;
                }
                else
                    resp = false;
                Close();
                if (resp)
                    EscribeLog("Obtención correcta ObtenFiltros Tipo:" + Filtro + " " + Tipo + " Aplicacion " + AplicacionID);
                else
                    EscribeLog("Obtención incorrecta ObtenFiltros Tipo:" + Filtro + " " + Tipo + " Aplicacion " + AplicacionID);
            }
            catch (Exception exx)
            {
                EscribeLog("Aplicacion.ObtenFiltrosBD " + exx.Message.ToString());
                resp = false;
            }
            return resp;
        }

        public bool ObtenAplicacion(int usuarioid,int areaid)
        {
            bool resp = false;
            try
            {
                PrepareSp(SPOBTENAPP);
                CargaCmdParameter("UsuarioID", SqlDbType.Int, 8, ParameterDirection.Input, usuarioid);
                CargaCmdParameter("AreaID", SqlDbType.Int, 8, ParameterDirection.Input, areaid);
                SqlDataReader objrst = GetResultStoredSQLReader();
                if (objrst.Read())
                {
                    aplicaionXML = new XmlDocument();
                    aplicaionXML.LoadXml("<xml>" + objrst[0].ToString() + "</xml>");
                    XmlNode select = aplicaionXML.DocumentElement.SelectSingleNode("Aplicaciones");
                    resp = select.HasChildNodes;
                }
                else
                    resp = false;
                Close();
                if (resp)
                    EscribeLog("Obtención correcta aplicaciones usuario:" + usuarioid + " area " + areaid);
                else
                    EscribeLog("Obtención incorrecta aplicaciones usuario:" + usuarioid + " area " + areaid);
            }
            catch (Exception exx)
            {
                EscribeLog("Aplicacion.ObtenAplicacion " + exx.Message.ToString());
                resp = false;
            }
            return resp;
        }

        public bool ObtenObjetosDB(int usuarioid, int aplicacionid) {
            bool resp = false;
            try
            {
                PrepareSp(SPOBTOBJDB);
                CargaCmdParameter("UsuarioID", SqlDbType.Int, 8, ParameterDirection.Input, usuarioid);
                CargaCmdParameter("AplicacionID", SqlDbType.Int, 8, ParameterDirection.Input, aplicacionid);
                SqlDataReader objrst = GetResultStoredSQLReader();
                if (objrst.Read())
                {
                    aplicaionXML = new XmlDocument();
                    aplicaionXML.LoadXml("<xml>" + objrst[0].ToString() + "</xml>");
                    XmlNode select = aplicaionXML.DocumentElement.SelectSingleNode("Objetos");
                    resp = select.HasChildNodes;
                }
                else
                    resp = false;
                Close();
                if (resp)
                    EscribeLog("Obtención correcta objetosdb usuario:" + usuarioid + " app " + aplicacionid);
                else
                    EscribeLog("Obtención incorrecta aplicaciones usuario:" + usuarioid + " app " + aplicacionid);
            }
            catch (Exception exx)
            {
                EscribeLog("Aplicacion.ObtenObjetosDB " + exx.Message.ToString());
                resp = false;
            }
            return resp;
        }

        public bool ObtenRelTablas(int usuarioid, int aplicacionid) {
            bool resp = false;
            try
            {
                PrepareSp(SPOBTRELTablas);
                CargaCmdParameter("UsuarioID", SqlDbType.Int, 8, ParameterDirection.Input, usuarioid);
                CargaCmdParameter("AplicacionID", SqlDbType.Int, 8, ParameterDirection.Input, aplicacionid);
                SqlDataReader objrst = GetResultStoredSQLReader();
                if (objrst.Read())
                {
                    aplicaionXML = new XmlDocument();
                    aplicaionXML.LoadXml("<xml>" + objrst[0].ToString() + "</xml>");
                    XmlNode select = aplicaionXML.DocumentElement.SelectSingleNode("ObjetosDB");
                    resp = select.HasChildNodes;
                }
                else
                    resp = false;
                Close();
                if (resp)
                    EscribeLog("Obtención correcta RelacionesTablas usuario:" + usuarioid + " app " + aplicacionid);
                else
                    EscribeLog("Obtención incorrecta RelacionesTablas usuario:" + usuarioid + " app " + aplicacionid);
            }
            catch (Exception exx)
            {
                EscribeLog("Aplicacion.ObtenRelTablas " + exx.Message.ToString());
                resp = false;
            }
            return resp;
        }

        public bool ObtenTransversalidad(int usuarioid, int aplicacionid, int tipoid,string indicador, int detalle, string elemento="") {
            bool resp = false;
            try
            {
                switch (detalle) {
                    case 1:
                        PrepareSp(SPOBTRANSVERSAL);
                        break;
                    case 2:
                        PrepareSp(SPOBTRANSVERSALD);
                        break;
                    case 3:
                        PrepareSp(SPOBTRANSVERSALDpn);
                        CargaCmdParameter("ObjNombre2", SqlDbType.VarChar, 100, ParameterDirection.Input, elemento);
                        break;
                }                
                CargaCmdParameter("UsuarioID", SqlDbType.Int, 8, ParameterDirection.Input, usuarioid);
                CargaCmdParameter("AplicacionID", SqlDbType.Int, 8, ParameterDirection.Input, aplicacionid);
                CargaCmdParameter("Tipo", SqlDbType.Int, 8, ParameterDirection.Input, tipoid);
                CargaCmdParameter("ObjNombre", SqlDbType.VarChar, 20, ParameterDirection.Input, indicador);
                SqlDataReader objrst = GetResultStoredSQLReader();
                if (objrst.Read())
                {
                    aplicaionXML = new XmlDocument();
                    aplicaionXML.LoadXml("<xml>" + objrst[0].ToString() + "</xml>");
                    XmlNode select = aplicaionXML.DocumentElement.SelectSingleNode("ObjetosDB");
                    resp = select.HasChildNodes;
                }
                else
                    resp = false;
                Close();
                if (resp)
                    EscribeLog("Obtención correcta ObtenTransversalidad usuario:" + usuarioid + " app " + aplicacionid);
                else
                    EscribeLog("Obtención incorrecta ObtenTransversalidad usuario:" + usuarioid + " app " + aplicacionid);
            }
            catch (Exception exx)
            {
                EscribeLog("Aplicacion.ObtenTransversalidad " + exx.Message.ToString());
                resp = false;
            }
            return resp;
        }

        public bool ObtenObjetosDB2(int usuarioid, int aplicacionid, int tipoid)
        {
            bool resp = false;
            try
            {
                PrepareSp(SPOBTOBJDB2);
                CargaCmdParameter("UsuarioID", SqlDbType.Int, 8, ParameterDirection.Input, usuarioid);
                CargaCmdParameter("AplicacionID", SqlDbType.Int, 8, ParameterDirection.Input, aplicacionid);
                CargaCmdParameter("TipoID", SqlDbType.Int, 8, ParameterDirection.Input, tipoid);                
                SqlDataReader objrst = GetResultStoredSQLReader();
                if (objrst.Read())
                {
                    aplicaionXML = new XmlDocument();
                    aplicaionXML.LoadXml("<xml>" + objrst[0].ToString() + "</xml>");
                    XmlNode select = aplicaionXML.DocumentElement.SelectSingleNode("ObjetosDB");
                    resp = select.HasChildNodes;
                }
                else
                    resp = false;
                Close();
                if (resp)
                    EscribeLog("Obtención correcta objetosdb usuario:" + usuarioid + " app " + aplicacionid);
                else
                    EscribeLog("Obtención incorrecta aplicaciones usuario:" + usuarioid + " app " + aplicacionid);
            }
            catch (Exception exx)
            {
                EscribeLog("Aplicacion.ObtenObjetosDB " + exx.Message.ToString());
                resp = false;
            }
            return resp;
        }

        public bool ObtenObjetosDB3(int usuarioid, int aplicacionid, string nombre)
        {
            bool resp = false;
            try
            {
                PrepareSp(SPOBTOBJDB3);
                CargaCmdParameter("UsuarioID", SqlDbType.Int, 8, ParameterDirection.Input, usuarioid);
                CargaCmdParameter("AplicacionID", SqlDbType.Int, 8, ParameterDirection.Input, aplicacionid);
                CargaCmdParameter("ObjNombre", SqlDbType.VarChar,20,ParameterDirection.Input, nombre);
                SqlDataReader objrst = GetResultStoredSQLReader();
                if (objrst.Read())
                {
                    aplicaionXML = new XmlDocument();
                    aplicaionXML.LoadXml("<xml>" + objrst[0].ToString() + "</xml>");
                    XmlNode select = aplicaionXML.DocumentElement.SelectSingleNode("ObjetosDB");
                    resp = select.HasChildNodes;
                }
                else
                    resp = false;
                Close();
                if (resp)
                    EscribeLog("Obtención correcta objetosdb usuario:" + usuarioid + " app " + aplicacionid);
                else
                    EscribeLog("Obtención incorrecta aplicaciones usuario:" + usuarioid + " app " + aplicacionid);
            }
            catch (Exception exx)
            {
                EscribeLog("Aplicacion.ObtenObjetosDB " + exx.Message.ToString());
                resp = false;
            }
            return resp;
        }

        public bool ObtenObjetosDB4(int usuarioid, int aplicacionid, string nombre,int tipo)
        {
            bool resp = false;
            try
            {
                PrepareSp(SPOBTOBJDB4);
                CargaCmdParameter("UsuarioID", SqlDbType.Int, 8, ParameterDirection.Input, usuarioid);
                CargaCmdParameter("AplicacionID", SqlDbType.Int, 8, ParameterDirection.Input, aplicacionid);
                CargaCmdParameter("ObjNombre", SqlDbType.VarChar, 100, ParameterDirection.Input, nombre);
                CargaCmdParameter("tipo", SqlDbType.Int, 8, ParameterDirection.Input, tipo);
                SqlDataReader objrst = GetResultStoredSQLReader();
                if (objrst.Read())
                {
                    aplicaionXML = new XmlDocument();
                    aplicaionXML.LoadXml("<xml>" + objrst[0].ToString() + "</xml>");
                    XmlNode select = aplicaionXML.DocumentElement.SelectSingleNode("ObjetosDB");
                    resp = select.HasChildNodes;
                }
                else
                    resp = false;
                Close();
                if (resp)
                    EscribeLog("Obtención correcta objetosdb usuario:" + usuarioid + " app " + aplicacionid);
                else
                    EscribeLog("Obtención incorrecta aplicaciones usuario:" + usuarioid + " app " + aplicacionid);
            }
            catch (Exception exx)
            {
                EscribeLog("Aplicacion.ObtenObjetosDB " + exx.Message.ToString());
                resp = false;
            }
            return resp;
        }

        public bool ObtenIndicaores(int usuarioid, int aplicacionid, int procesoid, int idtipo) {
            bool resp = false;
            try
            {
                PrepareSp(SPOBTINDICADORES);
                CargaCmdParameter("UsuarioID", SqlDbType.Int, 8, ParameterDirection.Input, usuarioid);
                CargaCmdParameter("AplicacionID", SqlDbType.Int, 8, ParameterDirection.Input, aplicacionid);
                CargaCmdParameter("ProcesoID", SqlDbType.BigInt, 16, ParameterDirection.InputOutput, procesoid);
                CargaCmdParameter("TipoID", SqlDbType.Int, 8, ParameterDirection.Input, idtipo);
                SqlDataReader objrst = GetResultStoredSQLReader();
                if (objrst.Read())
                {
                    aplicaionXML = new XmlDocument();                    
                    aplicaionXML.LoadXml("<xml>" + objrst[0].ToString() + "</xml>");
                    XmlNode select = aplicaionXML.DocumentElement.SelectSingleNode("Indicadores");
                    resp = select.HasChildNodes;
                    procesoID = 1179;
                }
                else
                    resp = false;
                Close();
                if (resp)
                    EscribeLog("Obtención correcta ObtenIndicaores usuario:" + usuarioid + " app " + aplicacionid);
                else
                    EscribeLog("Obtención incorrecta ObtenIndicaores usuario:" + usuarioid + " app " + aplicacionid);
            }
            catch (Exception exx)
            {
                EscribeLog("Aplicacion.ObtenIndicaores " + exx.Message.ToString());
                resp = false;
            }
            return resp;
        }

        public bool ObtenRelacionDepDB(int usuarioid, int aplicacionid, string nombre)
        {
            bool resp = false;
            try
            {
                PrepareSp(SPOBTRELDEP);
                CargaCmdParameter("UsuarioID", SqlDbType.Int, 8, ParameterDirection.Input, usuarioid);
                CargaCmdParameter("AplicacionID", SqlDbType.Int, 8, ParameterDirection.Input, aplicacionid);
                CargaCmdParameter("ObjNombre", SqlDbType.VarChar, 40, ParameterDirection.Input, nombre);
                SqlDataReader objrst = GetResultStoredSQLReader();
                if (objrst.Read())
                {
                    aplicaionXML = new XmlDocument();
                    aplicaionXML.LoadXml("<xml>" + objrst[0].ToString() + "</xml>");
                    XmlNode select = aplicaionXML.DocumentElement.SelectSingleNode("ObjetosDB");
                    resp = select.HasChildNodes;
                }
                else
                    resp = false;
                Close();
                if (resp)
                    EscribeLog("Obtención correcta relaciondetalle usuario:" + usuarioid + " app " + aplicacionid);
                else
                    EscribeLog("Obtención incorrecta relaciondetalle usuario:" + usuarioid + " app " + aplicacionid);
            }
            catch (Exception exx)
            {
                EscribeLog("Aplicacion.ObtenRelacionDepDB " + exx.Message.ToString());
                resp = false;
            }
            return resp;
        }

    }
}