using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using DependenciaMVC5demo.com.basedatos;
using DependenciaMVC5demo.com.dto;

namespace DependenciaMVC5demo.com.negocio
{
    public class Aplicaciones : BaseDatos
    {
        private string SPOBTENAPLICACIONES = "Sp_ObtenAplicacion";
        private string SPEDITAAPLICACIONES = "Sp_EditaAplicacion";
        private string SPOBTENLENGUAJES = "ObtenLenguajes";

        private XmlDocument aplicacionesXML;
        private XmlDocument lenguajesXML;

        public XmlDocument AplicacionesXML
        {
            get { return aplicacionesXML; }
        }

        public XmlDocument LenguajesXML
        {
            get { return lenguajesXML; }
        }

        public bool ObtenAplicaciones(int UsuarioID, int AreaID, int AplicacionID)
        {
            bool resp = false;
            try
            {
                PrepareSp(SPOBTENAPLICACIONES);
                CargaCmdParameter("UsuarioID", SqlDbType.Int, 8, ParameterDirection.Input, UsuarioID);
                CargaCmdParameter("AreaID", SqlDbType.Int, 8, ParameterDirection.Input, AreaID);
                CargaCmdParameter("Aplicacion", SqlDbType.Int, 8, ParameterDirection.Input, AplicacionID);
                SqlDataReader objrst = GetResultStoredSQLReader();
                if (objrst.Read())
                {
                    aplicacionesXML = new XmlDocument();
                    aplicacionesXML.LoadXml("<xml>" + objrst[0].ToString() + "</xml>");
                    XmlNode select = aplicacionesXML.DocumentElement.SelectSingleNode("Aplicaciones");
                    resp = select.HasChildNodes;
                }
                else
                    resp = false;
                Close();
                if (resp)
                    EscribeLog("Obten Aplicaciones correcto usuario " + UsuarioID);
                else
                    EscribeLog("Obten Aplicaciones incorrecto usuario  " + UsuarioID);
            }
            catch (Exception exx)
            {
                EscribeLog("Aplicaciones.ObtenAplicaciones " + exx.Message.ToString());
                resp = false;
            }
            return resp;
        }

        public bool EditaAplicaciones(int UsuarioID, string Tipo, int AplicacionID, string Aplicacion, string Descripcion, int LenguajeID, string Clave, int AreaID, int EstatusID)
        {
            bool resp = false;
            try
            {
                PrepareSp(SPEDITAAPLICACIONES);
                CargaCmdParameter("usuarioid", SqlDbType.Int, 8, ParameterDirection.Input, UsuarioID);
                CargaCmdParameter("tipo", SqlDbType.VarChar, 20, ParameterDirection.Input, Tipo);
                CargaCmdParameter("aplicacionid", SqlDbType.Int, 8, ParameterDirection.Input, AplicacionID);
                CargaCmdParameter("aplicacion", SqlDbType.VarChar, 100, ParameterDirection.Input, Aplicacion);
                CargaCmdParameter("descripcion", SqlDbType.VarChar, 250, ParameterDirection.Input, Descripcion);
                CargaCmdParameter("lenguajeid", SqlDbType.Int, 8, ParameterDirection.Input, LenguajeID);
                CargaCmdParameter("claveaplicacion", SqlDbType.VarChar, 30, ParameterDirection.Input, Clave);
                CargaCmdParameter("areaid", SqlDbType.Int, 8, ParameterDirection.Input, AreaID);
                CargaCmdParameter("estatusid", SqlDbType.Int, 8, ParameterDirection.Input, EstatusID);
                ExecStoredProcNoQuery();
                resp = true;
                Close();
                if (resp)
                    EscribeLog("Edita aplicaciones correcto usuario " + UsuarioID + " tipo " + Tipo);
                else
                    EscribeLog("Edita aplicaciones incorrecto usuario " + UsuarioID + " tipo " + Tipo);
            }
            catch (Exception exx)
            {
                EscribeLog("Aplicaciones.EditaAplicaciones " + exx.Message.ToString());
                resp = false;
            }
            return resp;
        }

        public bool ObtenLenguajes(int usuario)
        {
            bool resp = false;
            try
            {
                PrepareSp(SPOBTENLENGUAJES);
                CargaCmdParameter("UsuarioID", SqlDbType.Int, 8, ParameterDirection.Input, usuario);
                SqlDataReader objrst = GetResultStoredSQLReader();
                if (objrst.Read())
                {
                    lenguajesXML = new XmlDocument();
                    lenguajesXML.LoadXml("<xml>" + objrst[0].ToString() + "</xml>");
                    XmlNode select = lenguajesXML.DocumentElement.SelectSingleNode("Lenguajes");
                    resp = select.HasChildNodes;
                }
                else
                    resp = false;
                Close();
                if (resp)
                    EscribeLog("Obten Lenguajes correcto usuario " + usuario);
                else
                    EscribeLog("Obten Lenguajes incorrecto usuario  " + usuario);
            }
            catch (Exception exx)
            {
                EscribeLog("Aplicaciones.ObtenLenguajes " + exx.Message.ToString());
                resp = false;
            }
            return resp;
        }
    }
}