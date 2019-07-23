using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Data;
using System.Data.SqlClient;
using DependenciaMVC5demo.com.negocio;
    
namespace DependenciaMVC5demo.com.utilerias
{
    public class Menu : Proceso
    {
        private XmlDocument resultadoXML;
        public XmlDocument ResultadoXML { get { return resultadoXML; } }
        private string SPMENU = "Sp_ObtenMenu";

        public bool ObtenMenu(int UsuarioID)
        {
            bool exito = false;
            try
            {
                PrepareSp(SPMENU);
                CargaCmdParameter("UsuarioID", SqlDbType.Int, 8, ParameterDirection.Input, UsuarioID);
                SqlDataReader objrst = GetResultStoredSQLReader();
                if (objrst.Read())
                {
                    resultadoXML = new XmlDocument();
                    resultadoXML.LoadXml("<xml>" + objrst[0].ToString() + "</xml>");
                    XmlNode select = resultadoXML.DocumentElement.SelectSingleNode("Menu");
                    exito = select.HasChildNodes;
                }
                else
                    exito = false;
                Close();
            }
            catch (Exception Err)
            {
                EscribeLog("Proceso.ObtenProcesoJerarquia " + Err.Message.ToString());
            }
            return exito;

        }


        public XmlDocument ComponeMenu(int UsuarioID, string Permisos)
        {
            Proceso procesoobj = new Proceso();
            XmlDocument consultaxml = procesoobj.PAvanceXML;
            try
            {
                if (ObtenMenu(UsuarioID))
                {
                    consultaxml = resultadoXML;
                }
            }
            catch (Exception err)
            {
                procesoobj.EscribeLog("Menu ComponeMenu " + err.Message.ToString());
            }
            finally
            {
                procesoobj = null;
            }
            return consultaxml;
        }


    }
}
