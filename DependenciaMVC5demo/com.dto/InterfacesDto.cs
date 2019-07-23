using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DependenciaMVC5demo.com.dto
{
    public class InterfacesDto
    {
        public string Nombre { get; set; }
        public string IP { get; set; }
        public string Tipo { get; set; }
        public string Interface { get; set; }

        public int DependenciaID { get; set; }
        public int NumLinea { get; set; }
        public string Referencia { get; set; }
        public string NombreArchivo { get; set; }
        public string LenguajeApp { get; set; }
        public string Direccion { get; set; }
        public string TipoWS { get; set; }
        public string Middleware { get; set; }
        public string TipoPadre { get; set; }
        public string ObjPadre { get; set; }
        public string TipoHijo { get; set; }
        public string ObjHijo { get; set; }

    }
}