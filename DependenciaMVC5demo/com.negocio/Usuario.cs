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
    public class Usuario:BaseDatos
    {        
        private string SPVALIDAUSUARIO = "Sp_ValidaUsuario";
        private string SPOBTENUSUS = "Sp_ObtenUsuario";
        private string SPEDITAUSUARIO = "Sp_EditaUsuario";

        private XmlDocument usuarioXML;
        private XmlDocument perfilXML;

        public XmlDocument UsuarioXML
        {
            get { return usuarioXML; }            
        }

        public XmlDocument PerfilXML
        {
            get { return perfilXML; }
        }

        public bool ValidaUsuario(string usuario, string contrasenia) {
            bool resp = false;
            try{
                PrepareSp(SPVALIDAUSUARIO);
                CargaCmdParameter("user", SqlDbType.VarChar, 100, ParameterDirection.Input, usuario);
                CargaCmdParameter("password", SqlDbType.VarChar, 100, ParameterDirection.Input, contrasenia);
                SqlDataReader objrst=GetResultStoredSQLReader();
                if (objrst.Read())
                {
                    usuarioXML = new XmlDocument();
                    usuarioXML.LoadXml("<xml>" + objrst[0].ToString() + "</xml>");
                    XmlNode select = usuarioXML.DocumentElement.SelectSingleNode("Usuario");
                    resp = select.HasChildNodes;                    
                }
                else 
                    resp=false;
                Close();
                if(resp)    
                    EscribeLog("Acceso correcto usuario " + usuario);
                else        
                    EscribeLog("Acceso incorrecto usuario " + usuario);                    
            }catch(Exception exx) {
                EscribeLog("Usuario.ValidaUsuario " + exx.Message.ToString());
                resp = false;
            }
            return resp;
        }

        public bool ObtenUsuario(int UsuarioID, int UsuarioBuscado)
        {
            bool resp = false;
            try
            {
                PrepareSp(SPOBTENUSUS);
                CargaCmdParameter("UsuarioID", SqlDbType.Int, 8, ParameterDirection.Input, UsuarioID);
                CargaCmdParameter("UsuarioBuscado", SqlDbType.Int, 8, ParameterDirection.Input, UsuarioBuscado);
                SqlDataReader objrst = GetResultStoredSQLReader();
                if (objrst.Read())
                {
                    usuarioXML = new XmlDocument();
                    usuarioXML.LoadXml("<xml>" + objrst[0].ToString() + "</xml>");
                    XmlNode select = usuarioXML.DocumentElement.SelectSingleNode("Usuarios");
                    resp = select.HasChildNodes;
                }
                else
                    resp = false;
                Close();
                if (resp)
                    EscribeLog("Obten usuarios correcto usuario " + UsuarioID);
                else
                    EscribeLog("Obten usuarios incorrecto usuario  " + UsuarioID);
            }
            catch (Exception exx)
            {
                EscribeLog("Usuario.ObtenUsuario " + exx.Message.ToString());
                resp = false;
            }
            return resp;
        }

        public bool EditaUsuario(int UsuarioID, string Tipo, int IDEditado, string Nombre, string ApellidoP, string ApellidoM, string NIC, string Contrasena, string Correo, int Area, int Perfil, int Estatus)
        {
            bool resp = false;
            try
            {
                PrepareSp(SPEDITAUSUARIO);
                CargaCmdParameter("UsuarioID", SqlDbType.Int, 8, ParameterDirection.Input, UsuarioID);
                CargaCmdParameter("Tipo", SqlDbType.VarChar, 20, ParameterDirection.Input, Tipo);
                CargaCmdParameter("IDEditado", SqlDbType.Int, 8, ParameterDirection.Input, IDEditado);
                CargaCmdParameter("Nombre", SqlDbType.VarChar, 50, ParameterDirection.Input, Nombre);
                CargaCmdParameter("ApellidoP", SqlDbType.VarChar, 50, ParameterDirection.Input, ApellidoP);
                CargaCmdParameter("ApellidoM", SqlDbType.VarChar, 50, ParameterDirection.Input, ApellidoM);
                CargaCmdParameter("NIC", SqlDbType.VarChar, 100, ParameterDirection.Input, NIC);
                CargaCmdParameter("Constrasena", SqlDbType.VarChar, 100, ParameterDirection.Input, Contrasena);
                CargaCmdParameter("Correo", SqlDbType.VarChar, 100, ParameterDirection.Input, Correo);
                CargaCmdParameter("Area", SqlDbType.Int, 8, ParameterDirection.Input, Area);
                CargaCmdParameter("Perfil", SqlDbType.Int, 8, ParameterDirection.Input, Perfil);
                CargaCmdParameter("Estatus", SqlDbType.Int, 8, ParameterDirection.Input, Estatus);
                ExecStoredProcNoQuery();
                resp = true;
                Close();
                if (resp)
                    EscribeLog("Edita Usuarios correcta Usuario " + NIC);
                else
                    EscribeLog("Edita Usuarios incorrecta Usuario " + NIC);
            }
            catch (Exception exx)
            {
                EscribeLog("Usuario.EditaUsuario " + exx.Message.ToString());
                resp = false;
            }
            return resp;
        }
    }
}