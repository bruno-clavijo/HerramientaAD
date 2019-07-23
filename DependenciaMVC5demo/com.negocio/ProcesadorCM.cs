using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using DependenciaMVC5demo.com.dto;

namespace DependenciaMVC5demo.com.negocio
{
    public class ParseadorCM:Proceso
    {
        HashSet<string> InventarioCM = new HashSet<string>();
        public  HashSet<string> Librerias = new HashSet<string>();
        List<string> Resultado = new List<string>();
        VerificaComentarios VerCom = new VerificaComentarios();

        public HashSet<string> GenerarInventarioCM(string Ruta, ProcesoDto pdto)
        {
            RevisaLinea DatosInventario = new RevisaLinea();
            try
            {
                string Archivo = Path.GetFileName(Ruta);
                Archivo = Regex.Replace(Archivo, @"\.\w+", string.Empty).Trim();
                using (StreamReader sr = new StreamReader(Ruta))
                {
                    while (sr.Peek() >= 0)
                    {
                        string lineaCodigo = sr.ReadLine().Trim();

                        if (!String.IsNullOrEmpty(lineaCodigo))
                        {

                            //if (lineaCodigo.IndexOf("oWorkgroup") > -1)
                            //    lineaCodigo = lineaCodigo;

                            lineaCodigo = VerCom.BuscarComentario(lineaCodigo);
                            if (!String.IsNullOrEmpty(lineaCodigo))
                            {
                                DatosInventario = DatosInventario.ObtenerInventario(lineaCodigo, Librerias);
                                if (DatosInventario.EsValido)
                                {
                                    InventarioCM.Add((string.IsNullOrEmpty(DatosInventario.NombreEspacio) ?
                                        Archivo : DatosInventario.NombreEspacio) + "|" + DatosInventario.Nombre +
                                        "|" + DatosInventario.Tipo);
                                }
                            }
                        }
                    }
                }
            }catch (Exception Err) {
                EscribeLogWS("ParseadorCM.GenerarInventarioCM " + Err.Message.ToString());                
                ProcesoAvanceDto pdtoA = new ProcesoAvanceDto();
                Proceso proc = new Proceso();
                proc.SeteaAvance("Error","OK", "X", "--", "--", Err.Message.ToString(), "Error al realizar la descompresión del archivo", pdtoA, pdto);                              
                proc.ActualizaProcesoAvance(pdtoA, pdto);                
            }
            return InventarioCM;
        }

        public void GenerarSalidaCM(HashSet<string> InventarioCM, List<string> Archivos, string App, ProcesoDto pdt)
        {
            ProcesoAvanceDto pdtoA = new ProcesoAvanceDto();
            Proceso proc = new Proceso();
            double total = Archivos.Count();
            double avance = 27 / total;
            
            for (int i = 0; i <= Archivos.Count() - 1; i++)
            {

                //Aquí van a ir los parametros para iniciar
                string Ruta = Archivos[i];
                string Archivo = Path.GetFileName(Ruta);
                int NoLinea = 0;
                RevisaLinea datosLinea = new RevisaLinea();

                //Empezar a leer el archivo
                using (StreamReader sr = new StreamReader(Ruta))
                {
                    while (sr.Peek() >= 0)
                    {
                        string lineaCodigo = sr.ReadLine().Trim();

                        //Contar No. Linea
                        ++NoLinea;

                        //if (NoLinea == 32)
                        //    NoLinea = NoLinea;

                        if (!String.IsNullOrEmpty(lineaCodigo))
                        {
                            //Actualizar el nuevo VerificaComentarios
                            lineaCodigo = VerCom.BuscarComentario(lineaCodigo);
                            if (!String.IsNullOrEmpty(lineaCodigo))
                            {
                                datosLinea.ObtenerSalida(lineaCodigo, Archivo, InventarioCM, Resultado, NoLinea, Ruta, Librerias);
                            }
                        }
                    }
                }
                proc.SeteaAvance("En Proceso", "OK", "OK", Math.Round((72 + avance * i),0).ToString(), "70", "", "Recuperando Datos", pdtoA, pdt);
                proc.ActualizaProcesoAvance(pdtoA, pdt);
            }

            proc.SeteaAvance("En Proceso", "OK", "OK", "OK", "70", "", "Recuperando Datos", pdtoA, pdt);
            proc.ActualizaProcesoAvance(pdtoA, pdt);

            GuardaProcesoCM(pdtoA, pdt, Resultado);
            //System.IO.File.WriteAllLines(@"C:\INFONAVIT\ClasesMetodos.txt", Resultado);
        }
    }
}