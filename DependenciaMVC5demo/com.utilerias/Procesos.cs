using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DependenciaMVC5demo.com.dto;
using DependenciaMVC5demo.com.negocio;

namespace DependenciaMVC5demo.com.utilerias
{
    public class Procesos:Proceso
    {
        List<CadenaAEncontrarDto> CadenasApp = new List<CadenaAEncontrarDto>();
        List<CadenaAEncontrarDto> OtrasCadenas = new List<CadenaAEncontrarDto>();
  
        public List<CadenaAEncontrarDto> ReordenarCadenas(List<CadenaAEncontrarDto> cadenas, List<ConexionBDDto> conexionesBd, AplicacionDto AppDto)
        {
            if(AppDto.Tecnologia == "NET")
            {
                string conPrincipal = conexionesBd.FirstOrDefault().BaseDatos.ToLower();

                foreach (CadenaAEncontrarDto cadena in cadenas)
                    if (cadena.Grupo.ToLower() == conPrincipal)
                        CadenasApp.Add(cadena);

                foreach (CadenaAEncontrarDto cadena in cadenas)
                {
                    var con = conexionesBd.FirstOrDefault(c => c.BaseDatos.ToLower() == cadena.Grupo.ToLower() && c.BaseDatos.ToLower() != conPrincipal);
                    if (con != null)
                        CadenasApp.Add(cadena);
                    else
                    {
                        if (cadena.Grupo.ToLower() != conPrincipal)
                            OtrasCadenas.Add(cadena);
                    }
                }
                //Quitamos las cadenas de bases de datos diferentes a las de la app
                //if (CadenasApp != null && OtrasCadenas != null)
                //    OtrasCadenas.ForEach(x => { CadenasApp.Add(x); });
            }
            else
            {
                string jndi = "", baseDatos = "";
                foreach (var con in conexionesBd)
                {
                    if (!string.IsNullOrEmpty(con.BaseDatos))
                        baseDatos = baseDatos + "," + "'" + con.BaseDatos + "'";
                    else
                        jndi = jndi + "," + "'" +con.Jndi + "'";
                }
                baseDatos = baseDatos.TrimStart(',');
                jndi = jndi.TrimStart(',');
                if (string.IsNullOrEmpty(jndi)) jndi = "''";
                if (string.IsNullOrEmpty(baseDatos)) baseDatos = "''";
                //DataTable tabla = Entrada.EjecutaConsulta("SELECT obj.nombre_bd, obj.nombre_objeto, obj.Tipo_Objeto, jndi.jndi from BD_JNDI Jndi " +
                //                                           "inner join OBJETOS_BD obj on " +
                //                                           "lower(obj.nombre_bd) = lower(jndi.basedatos) " +
                //                                           "where lower(Jndi.jndi) in ("+ jndi.ToLower() +") or lower(Jndi.BaseDatos) in ("+ baseDatos.ToLower() + ");");

                //SqlParameter[] Params = new SqlParameter[2];
                //Params[0] = new SqlParameter("@ListaJndi", SqlDbType.VarChar);
                string ListaJNDI = jndi.ToLower();
                //Params[1] = new SqlParameter("@ListaBd", SqlDbType.VarChar);
                string ListaBD = baseDatos.ToLower();
     
                using (DataTable tabla = SeleccionaObjetosBD(ListaBD, ListaJNDI))
                {
                    if (tabla != null && tabla.Rows.Count > 0)
                    {   
                        foreach (DataRow fila in tabla.Rows)
                        {
                            CadenasApp.Add(new CadenaAEncontrarDto
                            {
                                Grupo = fila["NOMBRE_BD"].ToString(),
                                TipoObjeto = fila["TIPO_OBJETO"].ToString(),
                                NombreObjeto = fila["NOMBRE_OBJETO"].ToString(),
                                //TipoBusqueda = secciones[4] == "" ? TipoBusqueda.PalabraCompleta : (TipoBusqueda)Convert.ToInt32(secciones[4])
                                TipoBusqueda = 0
                            });

                        }
                    }
                    else
                    {
                        //cadenas.ForEach(x => { CadenasApp.Add(x); });
                        CadenasApp.AddRange(cadenas);
                    }
                }
            }            
            return CadenasApp;
        }
    }
}
