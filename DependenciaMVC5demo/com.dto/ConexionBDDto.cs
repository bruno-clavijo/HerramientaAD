using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DependenciaMVC5demo.com.dto
{
    public class ConexionBDDto
    {
        public string Nombre { get; set; }
        public string BaseDatos { get; set; }
        public string Servidor { get; set; }
        public string Jndi { get; set; }
    }
}