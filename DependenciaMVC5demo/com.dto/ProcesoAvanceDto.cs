using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DependenciaMVC5demo.com.dto
{
    public class ProcesoAvanceDto
    {
        private long procesoID;
        private string carga;        
        private string descomprimir;
        private string parseo;
        private string getinfo;
        private string estatus;
        private string observacion;
        private DateTime fechaHora;

        public long ProcesoID
        {
            get { return procesoID; }
            set { procesoID = value; }
        }

        public string Carga { get => carga; set => carga = value; }
        public string Descomprimir { get => descomprimir; set => descomprimir = value; }
        public string Parseo { get => parseo; set => parseo = value; }
        public string Getinfo { get => getinfo; set => getinfo = value; }
        public string Observacion { get => observacion; set => observacion = value; }
        public DateTime FechaHora { get => fechaHora; set => fechaHora = value; }
        public string Estatus { get => estatus; set => estatus = value; }
    }
}