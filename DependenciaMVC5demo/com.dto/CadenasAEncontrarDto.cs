using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DependenciaMVC5demo.com.dto
{
    public enum TipoBusqueda
    {
        PalabraCompleta = 0,
        Generica = 1,
        ExpresionRegular = 2
    }

    public class CadenaAEncontrarDto
    {
        public int TipoObjetoID { get; set; }
        public string TipoObjeto { get; set; }
        public int ObjetoID { get; set; }
        public string NombreObjeto { get; set; }
        public int GrupoID { get; set; }
        public string Grupo { get; set; }
        public TipoBusqueda TipoBusqueda { get; set; }
        public string Tecnologia { get; set; }
    }
}