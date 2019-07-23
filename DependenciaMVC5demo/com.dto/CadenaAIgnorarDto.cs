using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DependenciaMVC5demo.com.dto
{
    public enum LugarBusqueda
    {
        Inicio = 0,
        Cualquiera = 1,
        ExpresionRegular = 2
    }

    public class CadenaAIgnorarDto
    {
        public string Cadena { get; set; }
        public LugarBusqueda TipoBusqueda { get; set; }
    }
}