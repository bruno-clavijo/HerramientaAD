using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DependenciaMVC5demo.com.dto
{
    public class ObjetoDto
    {
        public bool EnProgreso { get; set; }
        public bool Procesado { get; set; }

        public string CveAplicacion { get; set; }
        public int AplicacionID { get; set; }
        public int NumLinea { get; set; }
        public string Referencia { get; set; }
        public int ObjetoID { get; set; }
        public string NombreObjeto { get; set; }
        public int TipoID { get; set; }
        public string Tipo { get; set; }
        public int BaseDatosID { get; set; }
        public string Grupo { get; set; }
        public string Archivo { get; set; }
        public string Lenguaje { get; set; }
        public string NombreBd { get; set; }

        public string TipoPadre { get; set; } //cs,java,con,met
        public string BibPadre { get; set; }
        public string ObjPadre { get; set; }
        public string TipoHijo { get; set; } //owner,cs,java,con,met
        public string BibHijo { get; set; }
        public string ObjHijo { get; set; }
    }
}