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
    public class Perfiles:BaseDatos
    {
        private string SPOBTENPERFILES = "Sp_ObtenPerfiles";
        private string SPEDITAPERFILES = "Sp_EditaPerfil";

        private XmlDocument perfilesXML;

        public XmlDocument PerfilesXML
        {
            get { return perfilesXML; }
        }

        public bool ObtenPerfiles(int UsuarioID, int Buscado)
        {
            bool resp = false;
            try
            {
                PrepareSp(SPOBTENPERFILES);
                CargaCmdParameter("UsuarioID", SqlDbType.Int, 8, ParameterDirection.Input, UsuarioID);
                CargaCmdParameter("Buscado", SqlDbType.Int, 8, ParameterDirection.Input, Buscado);
                SqlDataReader objrst = GetResultStoredSQLReader();
                if (objrst.Read())
                {
                    perfilesXML = new XmlDocument();
                    perfilesXML.LoadXml("<xml>" + objrst[0].ToString() + "</xml>");
                    XmlNode select = perfilesXML.DocumentElement.SelectSingleNode("Perfiles");
                    resp = select.HasChildNodes;
                }
                else
                    resp = false;
                Close();
                if (resp)
                    EscribeLog("Obten perfiles correcto usuario " + UsuarioID);
                else
                    EscribeLog("Obten perfiles incorrecto usuario  " + UsuarioID);
            }
            catch (Exception exx)
            {
                EscribeLog("Usuario.ObtenPerfiles " + exx.Message.ToString());
                resp = false;
            }
            return resp;
        }

        public bool EditaPerfiles(int UsuarioID, string Tipo, int PerfilID, string Nombre, string Permisos, int Estatus)
        {
            bool resp = false;
            try
            {
                PrepareSp(SPEDITAPERFILES);
                CargaCmdParameter("usuarioid", SqlDbType.Int, 8, ParameterDirection.Input, UsuarioID);
                CargaCmdParameter("tipo", SqlDbType.VarChar, 20, ParameterDirection.Input, Tipo);
                CargaCmdParameter("perfilid", SqlDbType.Int, 8, ParameterDirection.Input, PerfilID);
                CargaCmdParameter("nombre", SqlDbType.VarChar, 100, ParameterDirection.Input, Nombre);
                CargaCmdParameter("permisos", SqlDbType.VarChar, 20, ParameterDirection.Input, Permisos);
                CargaCmdParameter("estatus", SqlDbType.Int, 8, ParameterDirection.Input, Estatus);
                ExecStoredProcNoQuery();
                resp = true;
                Close();
                if (resp)
                    EscribeLog("Edita perfiles correcto usuario " + UsuarioID + " tipo " + Tipo);
                else
                    EscribeLog("Edita perfiles incorrecto usuario " + UsuarioID + " tipo " + Tipo);
            }
            catch (Exception exx)
            {
                EscribeLog("Perfiles.EditaPerfiles " + exx.Message.ToString());
                resp = false;
            }
            return resp;
        }
    }
}