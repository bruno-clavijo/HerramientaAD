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
    public class Proceso:BaseDatos
    {
        private string SPGUARDAPROCESO = "Sp_RegistraProceso";
        private string SPOBTENPROCESOAVANCE="Sp_ObtenProcesoAvance";
        private string SPACTUALIZAPROCESO = "Sp_ActualizaProceso";
        private string SPGGUARDARESULTADOCM = "Sp_GuardaResultadoProcesoCM";
        private string SPOBTENJERARQUIA = "Sp_ObtenProcesoJerarquia";
        private string SPELIMINAPROCESO = "Sp_EliminaProceso";
        private string SPCONSULTALENJUAGE = "SP_ConsultaLenguaje";
        private string SPGUARDARESULTADOWS = "Sp_GuardaResultadoProcesoWS";
        private string SPGUARDARESULTADOBD = "Sp_GuardaResultadoProcesoBD";
        private string SPCONSULTAS = "sp_Consultas";
        private string SPSELECCIONAOBJETOSBD = "SP_SeleccionaObjetosBD";
        private string SPCONSULTAPALABRAS = "SP_ConsultaPalabras";
        private string SPELIMINAPARSEO = "Sp_EliminarParseo";
        private string SPOBTENMIDDLEWARE = "Sp_ObtenMiddleware";
        private string SPGRAFICAS = "Sp_Graficas";
        private string SPOBTENPROCESO = "Sp_ObtenProceso";

        private string msjerr;
        private string msjerravance;
        private XmlDocument pAvanceXML;

        public XmlDocument PAvanceXML
        {
          get { return pAvanceXML; }          
        }

        public string Msjerr
        {
            get { return msjerr; }
            set { msjerr = value; }
        }

        private bool ValidaAvanceDto(ProcesoAvanceDto pdto)
        {
            bool valida = true;
            msjerr = "";
            try
            {
                if (pdto.ProcesoID <= 0)
                {
                    msjerravance = "UsuarioID incorrecto ";
                    valida = false;
                }
                if (!(pdto.Carga.Equals("ok") || pdto.Carga.Equals("--") || pdto.Carga.Equals("x") || pdto.Carga.Equals("OK") || pdto.Carga.Equals("--") || pdto.Carga.Equals("X")))
                {
                    msjerravance += "Carga valor incorrecto debe ser ('ok', '--' ,'x') ";
                    valida = false;
                }
                if (!(pdto.Descomprimir.Equals("ok") || pdto.Descomprimir.Equals("--") || pdto.Descomprimir.Equals("x") || pdto.Descomprimir.Equals("OK") || pdto.Descomprimir.Equals("--") || pdto.Descomprimir.Equals("X")))
                {
                    msjerravance += "Descomprimir valor incorrecto debe ser ('ok', '--' ,'x') ";
                    valida = false;
                }
                if (!(pdto.Parseo.Equals("ok") || pdto.Parseo.Equals("--") || pdto.Parseo.Equals("x") || pdto.Parseo.Equals("OK") || pdto.Parseo.Equals("--") || pdto.Parseo.Equals("X")))
                {
                    if (!(int.Parse(pdto.Parseo) > 0 && int.Parse(pdto.Parseo) <= 100))
                    {
                        msjerravance += "Parseo valor incorrecto debe ser ('ok', '--' ,'x') ";
                        valida = false;
                    }
                }
                if (!(pdto.Getinfo.Equals("ok") || pdto.Getinfo.Equals("--") || pdto.Getinfo.Equals("x") || pdto.Getinfo.Equals("OK") || pdto.Getinfo.Equals("--") || pdto.Getinfo.Equals("X")))
                {
                    if (!(int.Parse(pdto.Getinfo) >= 0 && int.Parse(pdto.Getinfo) <= 100))
                    {
                        msjerravance += "Getinfo valor incorrecto debe ser ('ok', '--' ,'x') ";
                        valida = false;
                    }
                }

            }
            catch (Exception Err)
            {
                EscribeLog("Proceso.validaavancedto " + Err.Message.ToString());
                valida = false;
            }
            return valida;
        }

        private bool ValidaDto(ProcesoDto pdto)
        {
            bool valida = true;
            msjerr = "";
            try {
                if (pdto.UsuarioID <= 0)
                {
                    msjerr = "UsuarioID incorrecto ";
                    valida = false;
                }
                if (pdto.AplicacionID <= 0)
                {
                    msjerr += "AplicacionID incorrecto ";
                    valida = false;
                }

            }catch(Exception Err){
                EscribeLog("Proceso.validadto " + Err.Message.ToString());
                valida = false;
            }
            return valida;        
        }

        public bool EliminaProceso(int usuario,long proceso) {
            bool elimino = false;
            try
            {
                PrepareSp(SPELIMINAPROCESO);
                CargaCmdParameter("UsuarioID", SqlDbType.Int, 8, ParameterDirection.Input, usuario);                
                CargaCmdParameter("ProcesoID", SqlDbType.BigInt, 16, ParameterDirection.Input, proceso);
                ExecStoredProcNoQuery();                
                Close();
            }
            catch (Exception Err) {
                EscribeLog("Proceso.EliminaProceso " + Err.Message.ToString());
                elimino = false;
            }
            return elimino;
        }
      
        public int GuardaProceso(ProcesoDto pdto) { 
            int idnewProc=0;
            try{
                if (ValidaDto(pdto))
                {
                    PrepareSp(SPGUARDAPROCESO);
                    CargaCmdParameter("UsuarioID", SqlDbType.Int, 8, ParameterDirection.Input, pdto.UsuarioID);
                    CargaCmdParameter("AplicacionID", SqlDbType.Int, 8, ParameterDirection.Input, pdto.AplicacionID);
                    CargaCmdParameter("ProcesoID", SqlDbType.Int, 8, ParameterDirection.InputOutput, 0);
                    ExecStoredProcNoQuery();
                    idnewProc = (int)RegresaValorParam("ProcesoID");                    
                    Close();                    
                }
                else {                    
                    EscribeLog("Proceso.GuardaProceso " + msjerr);
                    return idnewProc;
                }
            }
            catch (Exception Err) {
                EscribeLog("Proceso.GuardaProceso " + Err.Message.ToString());                
            }
            return idnewProc;
        }

        public bool ObtenProcesoAvance(long procesoid, int usuarioid){
            bool exito = false;            
            try {
                PrepareSp(SPOBTENPROCESOAVANCE);
                CargaCmdParameter("UsuarioID", SqlDbType.Int, 8, ParameterDirection.Input, usuarioid);                
                CargaCmdParameter("ProcesoID", SqlDbType.BigInt, 8, ParameterDirection.Input, procesoid);
                SqlDataReader objrst=GetResultStoredSQLReader();
                if (objrst.Read())
                {
                    pAvanceXML = new XmlDocument();
                    pAvanceXML.LoadXml("<xml>" + objrst[0].ToString() + "</xml>");
                    XmlNode select = pAvanceXML.DocumentElement.SelectSingleNode("Proceso");
                    exito = select.HasChildNodes;                    
                }
                else
                    exito = false;
                Close();
                if (exito)
                    EscribeLog("Avance correcto proceso " + procesoid);
                else
                    EscribeLog("Avance incorrecto proceso " + procesoid); 
            }catch(Exception Err){
                EscribeLog("Proceso.ObtenProcesoAvance " + Err.Message.ToString());
                exito = false;
            }
            return exito;                  
        }

        public bool ActualizaProcesoAvance(ProcesoAvanceDto pdtoA, ProcesoDto pdto) {
            bool exito = false;
            try
            {
                if (ValidaAvanceDto(pdtoA) && ValidaDto(pdto))
                {
                    PrepareSp(SPACTUALIZAPROCESO);
                    CargaCmdParameter("UsuarioID", SqlDbType.Int, 8, ParameterDirection.Input, pdto.UsuarioID);                    
                    CargaCmdParameter("ProcesoID", SqlDbType.BigInt, 8, ParameterDirection.Input, pdto.ProcesoID);
                    CargaCmdParameter("Carga", SqlDbType.VarChar, 2, ParameterDirection.Input, pdtoA.Carga);
                    CargaCmdParameter("Descomprimir", SqlDbType.VarChar, 2, ParameterDirection.Input, pdtoA.Descomprimir);
                    CargaCmdParameter("Parseo", SqlDbType.VarChar, 2, ParameterDirection.Input, pdtoA.Parseo);
                    CargaCmdParameter("GenInfo", SqlDbType.VarChar, 2, ParameterDirection.Input, pdtoA.Getinfo);
                    CargaCmdParameter("Estatus", SqlDbType.VarChar, 10, ParameterDirection.Input, pdtoA.Estatus);
                    CargaCmdParameter("UltMensaje", SqlDbType.VarChar, 1000, ParameterDirection.Input, pdto.UltMensaje);
                    CargaCmdParameter("Observacion", SqlDbType.VarChar, 100, ParameterDirection.Input, pdtoA.Observacion);                 
                    ExecStoredProcNoQuery();                    
                    Close();
                }
                else
                {
                    EscribeLog("Proceso.ActualizaProcesoAvance " + msjerr + msjerravance);
                    return false;
                }
            }
            catch (Exception Err) {
                EscribeLog("Proceso.GuardaProceso " + Err.Message.ToString());
            }
            return exito;
        }

        public void SeteaAvance(string estatus,string carga,string descomprimir,string parseo,string getinfo,string umsj,string obs, ProcesoAvanceDto pdtoA, ProcesoDto pdto) {
            pdtoA.ProcesoID = pdto.ProcesoID;
            pdtoA.Carga = carga;
            pdtoA.Descomprimir = descomprimir;
            pdtoA.Parseo = parseo;
            pdtoA.Getinfo = getinfo;
            pdto.UltMensaje = umsj;
            pdtoA.Observacion = obs;
            pdtoA.Estatus = estatus;
        }

        public bool GuardaProcesoCM(ProcesoAvanceDto pdtoA, ProcesoDto pdto, List<string> resultado)
        {
            bool resp = false;
            double total = resultado.Count();
            double avance = 30 / total;
            try
            {
                int conta = 0;
                foreach (string res in resultado)
                {
                    string[] conceptos = res.Replace('\"', ' ').Split('|');
                    ConnexionPermanentePrepareSp(SPGGUARDARESULTADOCM);
                    if (conta == 0)
                        InicializaTran();
                    AsignaTran();
                    CargaCmdParameter("UsuarioID", SqlDbType.Int, 8, ParameterDirection.Input, pdto.UsuarioID);
                    CargaCmdParameter("ProcesoID", SqlDbType.Int, 8, ParameterDirection.Input, pdto.ProcesoID);
                    CargaCmdParameter("AplicacionID", SqlDbType.Int, 8, ParameterDirection.Input, pdto.AplicacionID);
                    CargaCmdParameter("NumLinea", SqlDbType.Int, 8, ParameterDirection.Input, int.Parse(conceptos[7]));
                    CargaCmdParameter("Referencia", SqlDbType.VarChar, 3000, ParameterDirection.Input, conceptos[11]);
                    CargaCmdParameter("NombreArchivo", SqlDbType.VarChar, 400, ParameterDirection.Input, conceptos[6]);
                    CargaCmdParameter("LenguajeApp", SqlDbType.VarChar, 10, ParameterDirection.Input, conceptos[8]);
                    CargaCmdParameter("BibPadre", SqlDbType.VarChar, 100, ParameterDirection.Input, conceptos[1]);
                    CargaCmdParameter("TipoPadre", SqlDbType.VarChar, 100, ParameterDirection.Input, conceptos[0]);
                    CargaCmdParameter("ObjPadre", SqlDbType.VarChar, 100, ParameterDirection.Input, conceptos[2]);
                    CargaCmdParameter("BibHijo", SqlDbType.VarChar, 100, ParameterDirection.Input, conceptos[4]);
                    CargaCmdParameter("TipoHijo", SqlDbType.VarChar, 100, ParameterDirection.Input, conceptos[3]);
                    CargaCmdParameter("ObjHijo", SqlDbType.VarChar, 100, ParameterDirection.Input, conceptos[5]);
                    CargaCmdParameter("DependenciaCMID", SqlDbType.Int, 8, ParameterDirection.Input, int.Parse(conceptos[9]));
                    CargaCmdParameter("IDPadre", SqlDbType.Int, 8, ParameterDirection.Input, int.Parse(conceptos[10]));
                    ++conta;
                    ExecStoredProcNoQuery();
                    SeteaAvance("En Proceso", "OK", "OK", "OK", Math.Round((70 + avance * conta), 0).ToString(), "", "Leyendo Archivos", pdtoA, pdto);
                   
                }
                CommitTran();
                Close();
                ActualizaProcesoAvance(pdtoA, pdto);

            }
            catch (Exception Err)
            {
                EscribeLogWS("Guarda Resultado" + Err.Message.ToString());
                RollBackTran();
                Close();
                SeteaAvance("Error", "OK", "OK", "OK", "X", "Proceso terminado incorrectamente", Err.Message.ToString(), pdtoA, pdto);
                ActualizaProcesoAvance(pdtoA, pdto);
            }
            return resp;
        }  

        public bool ObtenProcesoJerarquia(ProcesoDto pdto) {
            bool exito = false;
            try
            {
                if (ValidaDto(pdto))
                {
                    PrepareSp(SPOBTENJERARQUIA);
                    CargaCmdParameter("UsuarioID", SqlDbType.Int, 8, ParameterDirection.Input, pdto.UsuarioID);
                    CargaCmdParameter("ProcesoID", SqlDbType.Int, 8, ParameterDirection.Input, pdto.ProcesoID);
                    CargaCmdParameter("AplicacionID", SqlDbType.Int, 8, ParameterDirection.Input, pdto.AplicacionID);
                    SqlDataReader objrst = GetResultStoredSQLReader();
                    if (objrst.Read())
                    {
                        pAvanceXML = new XmlDocument();
                        pAvanceXML.LoadXml(objrst[0].ToString());
                        XmlNode select = pAvanceXML.DocumentElement.SelectSingleNode("Padre");
                        exito = select.HasChildNodes;
                    }
                    else
                        exito = false;
                    Close();
                }
                else
                {
                    EscribeLog("Proceso.ObtenProcesoJerarquia " + msjerr);
                    return exito;
                }
            }
            catch (Exception Err)
            {
                EscribeLog("Proceso.ObtenProcesoJerarquia " + Err.Message.ToString());
            }
            return exito;

        }

        public DataTable Consultas(int Tipo, int CveAplicacion)
        {
            DataTable dt = new DataTable();
            try
            {
                PrepareSp(SPCONSULTAS);
                CargaCmdParameter("tipo", SqlDbType.Int, 8, ParameterDirection.Input, Tipo);
                CargaCmdParameter("cveAplicacion", SqlDbType.Int, 8, ParameterDirection.Input, CveAplicacion);
                SqlDataReader objrst = GetResultStoredSQLReader();
                dt.Load(objrst);
                Close();

                if (dt.Rows.Count >= 1)
                    switch (Tipo)
                    {
                        case 1:
                            EscribeLog("Consulta de Configuración correcta");
                            break;
                        case 2:
                            EscribeLog("Consulta de Exclusiones correcta");
                            break;
                        case 3:
                            EscribeLog("Consulta de Objetos BD correcta");
                            break;
                    }
                else
                    switch (Tipo)
                    {
                        case 1:
                            EscribeLog("Consulta de Configuración incorrecta");
                            break;
                        case 2:
                            EscribeLog("Consulta de Exclusiones incorrecta");
                            break;
                        case 3:
                            EscribeLog("Consulta de Objetos BD incorrecta");
                            break;
                    }
            }
            catch (Exception Err)
            {
                EscribeLog("Proceso.Consultas " + Err.Message.ToString());
            }
            return dt;
        }

        public DataTable ConsultaLenguaje(string CveAplicacion)
        {
            DataTable dt = new DataTable();
            try
            {
                PrepareSp(SPCONSULTALENJUAGE);
                CargaCmdParameter("AplicacionID", SqlDbType.Int, 8, ParameterDirection.Input, CveAplicacion);
                SqlDataReader objrst = GetResultStoredSQLReader();
                dt.Load(objrst);
                Close();

                if (dt.Rows.Count >= 1)
                    EscribeLog("Consulta de Lenguaje correcta");
                else
                    EscribeLog("Consulta de Lenguaje incorrecta");
            }
            catch (Exception Err)
            {
                EscribeLog("Proceso.ConsultaLenguaje " + Err.Message.ToString());
            }
            return dt;
        }

        public DataTable SeleccionaObjetosBD(string ListaBD, string ListaJNDI)
        {
            DataTable dt = new DataTable();
            try
            {
                PrepareSp(SPSELECCIONAOBJETOSBD);
                CargaCmdParameter("ListaBd", SqlDbType.VarChar, 100, ParameterDirection.Input, ListaBD);
                CargaCmdParameter("ListaJndi", SqlDbType.NVarChar, 200, ParameterDirection.Input, ListaJNDI);
                SqlDataReader objrst = GetResultStoredSQLReader();
                dt.Load(objrst);
                Close();

                if (dt.Rows.Count >= 1)
                    EscribeLog("Seleccion de Objetos BD correcta");
                else
                    EscribeLog("Seleccion de Objetos BD incorrecta");
            }
            catch (Exception Err)
            {
                EscribeLog("Proceso.SeleccionaObjetosBD " + Err.Message.ToString());
            }
            return dt;
        }

        public DataTable ConsultaPalabras(string Tecnologia, string Grupos)
        {
            DataTable dt = new DataTable();
            try
            {
                PrepareSp(SPCONSULTAPALABRAS);
                CargaCmdParameter("Tecnologia", SqlDbType.VarChar, 10, ParameterDirection.Input, Tecnologia);
                CargaCmdParameter("Grupos", SqlDbType.VarChar, 100, ParameterDirection.Input, Grupos);
                SqlDataReader objrst = GetResultStoredSQLReader();
                dt.Load(objrst);
                Close();

                if (dt.Rows.Count >= 1)
                    EscribeLog("Consulta de Palabras correcta");
                else
                    EscribeLog("Consulta de Palabras BD incorrecta");
            }
            catch (Exception Err)
            {
                EscribeLog("Proceso.ConsultaPalabras " + Err.Message.ToString());
            }
            return dt;
        }

        public bool GuardaProcesoWS(ProcesoAvanceDto pdtoA, ProcesoDto pdto, List<string> resultado)
        {
            bool resp = false;
            double total = resultado.Count();
            double avance = 28 / total;
            try
            {
                int conta = 0;
                foreach (string res in resultado)
                {
                    string[] conceptos = res.Replace('\"', ' ').Split('|');
                    ConnexionPermanentePrepareSp(SPGUARDARESULTADOWS);
                    if (conta == 0)
                        InicializaTran();
                    AsignaTran();
                    CargaCmdParameter("UsuarioID", SqlDbType.Int, 8, ParameterDirection.Input, pdto.UsuarioID);
                    CargaCmdParameter("DependenciaWSID", SqlDbType.Int, 8, ParameterDirection.Input, int.Parse(conceptos[0]));
                    CargaCmdParameter("AplicacionID", SqlDbType.Int, 8, ParameterDirection.Input, pdto.AplicacionID);
                    CargaCmdParameter("ProcesoID", SqlDbType.Int, 8, ParameterDirection.Input, pdto.ProcesoID);
                    CargaCmdParameter("NumLinea", SqlDbType.Int, 8, ParameterDirection.Input, int.Parse(conceptos[1]));
                    CargaCmdParameter("Referencia", SqlDbType.VarChar, 3000, ParameterDirection.Input, conceptos[2]);
                    CargaCmdParameter("NombreArchivo", SqlDbType.VarChar, 400, ParameterDirection.Input, conceptos[3]);
                    CargaCmdParameter("LenguajeApp", SqlDbType.VarChar, 10, ParameterDirection.Input, conceptos[4]);
                    CargaCmdParameter("Direccion", SqlDbType.VarChar, 200, ParameterDirection.Input, conceptos[5]);
                    CargaCmdParameter("Middleware", SqlDbType.VarChar, 50, ParameterDirection.Input, conceptos[6]);
                    CargaCmdParameter("TipoPadre", SqlDbType.VarChar, 100, ParameterDirection.Input, conceptos[7]);
                    CargaCmdParameter("ObjPadre", SqlDbType.VarChar, 100, ParameterDirection.Input, conceptos[8]);
                    CargaCmdParameter("TipoHijo", SqlDbType.VarChar, 100, ParameterDirection.Input, conceptos[9]);
                    CargaCmdParameter("ObjHijo", SqlDbType.VarChar, 100, ParameterDirection.Input, conceptos[10]);
                    ++conta;
                    ExecStoredProcNoQuery();
                    SeteaAvance("En Proceso", "OK", "OK", "70", Math.Round(40 + (avance * conta), 0).ToString(), "", "Leyendo Archivos", pdtoA, pdto);
                    ActualizaProcesoAvance(pdtoA, pdto);
                }
                CommitTran();
                Close();

            }
            catch (Exception Err)
            {
                EscribeLogWS("Guarda Resultado" + Err.Message.ToString());
                RollBackTran();
                Close();
                SeteaAvance("Error", "OK", "OK", "OK", "X", "Proceso terminado incorrectamente", Err.Message.ToString(), pdtoA, pdto);
                ActualizaProcesoAvance(pdtoA, pdto);
            }
            return resp;
        }

        public bool GuardaProcesoBD(ProcesoAvanceDto pdtoA, ProcesoDto pdto, List<string> resultado)
        {
            bool resp = false;
            double total = resultado.Count();
            double avance = 40 / total;
            try
            {
                int conta = 0;
                foreach (string res in resultado)
                {
                    string[] conceptos = res.Split('¡');
                    ConnexionPermanentePrepareSp(SPGUARDARESULTADOBD);
                    if (conta == 0)
                        InicializaTran();
                    AsignaTran();
                    CargaCmdParameter("UsuarioID", SqlDbType.Int, 8, ParameterDirection.Input, pdto.UsuarioID);
                    CargaCmdParameter("DependenciaBDID", SqlDbType.Int, 8, ParameterDirection.Input, int.Parse(conceptos[0]));
                    CargaCmdParameter("AplicacionID", SqlDbType.Int, 8, ParameterDirection.Input, pdto.AplicacionID);
                    CargaCmdParameter("ProcesoID", SqlDbType.Int, 8, ParameterDirection.Input, pdto.ProcesoID);
                    CargaCmdParameter("BibPadre", SqlDbType.VarChar, 100, ParameterDirection.Input, conceptos[2]);
                    CargaCmdParameter("TipoPadre", SqlDbType.VarChar, 100, ParameterDirection.Input, conceptos[3]);
                    CargaCmdParameter("ObjPadre", SqlDbType.VarChar, 100, ParameterDirection.Input, conceptos[4]);
                    CargaCmdParameter("BaseDatosID", SqlDbType.Int, 8, ParameterDirection.Input, int.Parse(conceptos[5]));
                    CargaCmdParameter("ObjetoBDID", SqlDbType.Int, 8, ParameterDirection.Input, int.Parse(conceptos[6]));
                    CargaCmdParameter("TipoObjetoID", SqlDbType.Int, 8, ParameterDirection.Input, int.Parse(conceptos[7]));
                    CargaCmdParameter("NumLinea", SqlDbType.Int, 8, ParameterDirection.Input, int.Parse(conceptos[8]));
                    CargaCmdParameter("Referencia", SqlDbType.VarChar, 3000, ParameterDirection.Input, conceptos[9]);
                    CargaCmdParameter("NombreArchivo", SqlDbType.VarChar, 400, ParameterDirection.Input, conceptos[10]);
                    CargaCmdParameter("LenguajeApp", SqlDbType.VarChar, 10, ParameterDirection.Input, conceptos[11]);
                    ++conta;
                    ExecStoredProcNoQuery();
                    SeteaAvance("En Proceso", "OK", "OK", "40", Math.Round((avance * conta), 0).ToString(), "", "Leyendo Archivos", pdtoA, pdto);
                    
                }
                CommitTran();
                Close();
                ActualizaProcesoAvance(pdtoA, pdto);
            }
            catch (Exception Err)
            {
                EscribeLogWS("Guarda Resultado" + Err.Message.ToString());
                RollBackTran();
                Close();
                SeteaAvance("Error", "OK", "OK", "OK", "X", "Proceso terminado incorrectamente", Err.Message.ToString(), pdtoA, pdto);
                ActualizaProcesoAvance(pdtoA, pdto);
            }
            return resp;
        }

        public bool EliminarParseo(ProcesoDto ProcDto, int Tipo)
        {
            bool respuesta = false;
            try
            {
                PrepareSp(SPELIMINAPARSEO);
                CargaCmdParameter("UsuarioID", SqlDbType.Int, 8, ParameterDirection.Input, ProcDto.UsuarioID);
                CargaCmdParameter("AplicacionID", SqlDbType.Int, 8, ParameterDirection.Input, ProcDto.AplicacionID);
                CargaCmdParameter("Tipo", SqlDbType.Int, 8, ParameterDirection.Input, Tipo);
                ExecStoredProcNoQuery();
                respuesta = true;
                Close();
            }
            catch (Exception Err)
            {
                EscribeLog("Proceso.ConsultaPalabras " + Err.Message.ToString());
            }
            return respuesta;
        }

        public bool ObtenGrafica(int Tipo, int AplicacionID)
        {
            bool exito = false;
            try
            {
                PrepareSp(SPGRAFICAS);
                CargaCmdParameter("Tipo", SqlDbType.Int, 8, ParameterDirection.Input, Tipo);
                CargaCmdParameter("AplicacionID", SqlDbType.Int, 8, ParameterDirection.Input, AplicacionID);
                SqlDataReader objrst = GetResultStoredSQLReader();
                if (objrst.Read())
                {
                    string Solicitud;
                    switch (Tipo)
                    {
                        case 1:
                            Solicitud = "ObjetosDB";
                            break;
                        case 2:
                            Solicitud = "ObjetosCM";
                            break;
                        case 3:
                            Solicitud = "ObjetosWS";
                            break;
                        case 4:
                            Solicitud = "ObjetosDB_Detalle";
                            break;
                        case 5:
                            Solicitud = "ObjetosDB";
                            break;
                        case 6:
                            Solicitud = "ObjetosCM";
                            break;
                        case 7:
                            Solicitud = "ObjetosWS";
                            break;
                        case 8:
                            Solicitud = "ObjetosDB_Detalle";
                            break;
                        case 9:
                            Solicitud = "ObjetosDB";
                            break;
                        default:
                            Solicitud = "";
                            break;
                    }

                    pAvanceXML = new XmlDocument();
                    pAvanceXML.LoadXml("<xml>" + objrst[0].ToString() + "</xml>");
                    XmlNode select = pAvanceXML.DocumentElement.SelectSingleNode(Solicitud);
                    exito = select.HasChildNodes;
                }
                else
                    exito = false;
                Close();
            }
            catch (Exception Err)
            {
                EscribeLog("Proceso.ObtenGrafica " + Err.Message.ToString());
            }
            return exito;

        }

        public bool ObtenMiddleware(int UsuarioID)
        {
            bool exito = false;
            try
            {
                PrepareSp(SPOBTENMIDDLEWARE);
                CargaCmdParameter("UsuarioID", SqlDbType.Int, 8, ParameterDirection.Input, UsuarioID);
                SqlDataReader objrst = GetResultStoredSQLReader();
                if (objrst.Read())
                {
                    pAvanceXML = new XmlDocument();
                    pAvanceXML.LoadXml("<xml>" + objrst[0].ToString() + "</xml>");
                    XmlNode select = pAvanceXML.DocumentElement.SelectSingleNode("Middleware");
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

        public DataTable ConsultaProceso(int CveAplicacion)
        {
            DataTable dt = new DataTable();
            try
            {
                PrepareSp(SPOBTENPROCESO);
                CargaCmdParameter("AplicacionID", SqlDbType.Int, 8, ParameterDirection.Input, CveAplicacion);
                SqlDataReader objrst = GetResultStoredSQLReader();
                dt.Load(objrst);
                Close();

                if (dt.Rows.Count >= 1)
                    EscribeLog("Consulta de Proceso correcta");
                else
                    EscribeLog("Consulta de Proceso incorrecta");
            }
            catch (Exception Err)
            {
                EscribeLog("Proceso.ConsultaProceso " + Err.Message.ToString());
            }
            return dt;
        }
    }
}