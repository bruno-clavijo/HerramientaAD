using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using DependenciaMVC5demo.com.utilerias;

namespace DependenciaMVC5demo.com.basedatos
{
    public class BaseDatos : ControlLog
    {

        private SqlConnection objcn;
        private SqlCommand objcmd1;
        private SqlTransaction objtran;

        public BaseDatos() {
            objcn = new SqlConnection(ConfigurationManager.ConnectionStrings["DependenciasBD"].ToString());
        }

        public bool CommitTran()
        {
            bool resp = false;
            try {
                objtran.Commit();
            }
            catch (Exception Err) {
                EscribeLog("CommitTran " + Err.Message.ToString());
            }
            return resp;
        }

        public bool RollBackTran()
        {
            bool resp = false;
            try
            {
                objtran.Rollback();
            }
            catch (Exception Err)
            {
                EscribeLog("RollBackTran " + Err.Message.ToString());
            }
            return resp;
        }

        public bool InicializaTran()
        {
            bool resp = false;
            try
            {
                objtran = objcn.BeginTransaction("resultTran");                
            }
            catch (Exception Err)
            {
                EscribeLog("InicializaTran " + Err.Message.ToString());
            }
            return resp;
        }

        public bool AsignaTran()
        {
            bool resp = false;
            try
            {
                objcmd1.Transaction = objtran;
            }
            catch (Exception Err)
            {
                EscribeLog("AsignaTran " + Err.Message.ToString());
            }
            return resp;
        }

        public BaseDatos(string conexion)
        {
            objcn = new SqlConnection(conexion);
        }

        public void PrepareSp(string SpName)
        {
            try
            {                
                objcn.Open();
                objcmd1 = new SqlCommand();
                objcmd1.Connection = objcn;
                objcmd1.CommandText = SpName;
                objcmd1.CommandType = System.Data.CommandType.StoredProcedure;
            }
            catch (SqlException Err)
            {
                Close();
                EscribeLog("BaseDatos.PrepareSp " + SpName + " " + Err.Message.ToString());
            }
        }

        public void ConnexionPermanentePrepareSp(string SpName)
        {
            try
            {
                if (objcn.State == ConnectionState.Broken) { objcn.Close(); objcn.Open(); }
                if (objcn.State == ConnectionState.Closed) { objcn.Open(); }               
                objcmd1 = new SqlCommand();
                objcmd1.Connection = objcn;
                objcmd1.CommandText = SpName;
                objcmd1.CommandType = System.Data.CommandType.StoredProcedure;
               
            }
            catch (SqlException Err)
            {                
                EscribeLog("BaseDatos.PrepareSp " + SpName + " " + Err.Message.ToString());
            }
        }

        public void ExecStoredProcNoQuery()
        {
            try
            {
                objcmd1.ExecuteNonQuery();
            }
            catch (SqlException Err)
            {
                Close();
                EscribeLog("BaseDatos.ExecStoredProcNoQuery " + Err.Message.ToString() + " " 
                                                                + Err.Number.ToString() + " "
                                                                + Err.Procedure.ToString() + " "
                                                                + Err.LineNumber.ToString() + " "
                                                                + Err.State.ToString());
            }
        }

        public SqlDataReader GetResultStoredSQLReader()
        {
            try
            {
                SqlDataReader sqlr = objcmd1.ExecuteReader();
                return sqlr;
            }
            catch (SqlException Err)
            {                
                Close();
                EscribeLog("BaseDatos.GetResultStoredSQLReader " + Err.Message.ToString() + " "
                                                                + Err.Number.ToString() + " "
                                                                + Err.Procedure.ToString() + " "
                                                                + Err.LineNumber.ToString() + " "
                                                                + Err.State.ToString());
                return null;
            }
        }

        public void Close()
        {
            try
            {
                if (objcn != null)
                {
                    if (objcn.State != ConnectionState.Closed)
                        objcn.Close();
                }
            }
            catch (SqlException Err)
            {
                EscribeLog("BaseDatos.GetResultStoredSQLReader " + Err.Message.ToString() + " "
                                                                + Err.Number.ToString() + " "
                                                                + Err.Procedure.ToString() + " "
                                                                + Err.LineNumber.ToString() + " "
                                                                + Err.State.ToString());
            }
        }

        public void CargaCmdParameter(string name, SqlDbType type, int size,ParameterDirection param ,object value)
        {
            try
            {
                objcmd1.Parameters.Add(new SqlParameter(name, type, size,param,true,0,0,"",DataRowVersion.Current, value));
            }
            catch (SqlException Err)
            {
                Close();
                EscribeLog("BaseDatos.GetResultStoredSQLReader " + Err.Message.ToString() + " "
                                                                + Err.Number.ToString() + " "
                                                                + Err.Procedure.ToString() + " "
                                                                + Err.LineNumber.ToString() + " "
                                                                + Err.State.ToString());
            }
        }

        public object RegresaValorParam(string name) {
            try
            {
                return objcmd1.Parameters[name].Value;
            }
            catch (Exception Err)
            {
                EscribeLog("BaseDatos.RegresaValorParam " + name + " " + Err.Message.ToString());
                return 0;
            }
        }
    }
}