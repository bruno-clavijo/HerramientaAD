using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DependenciaMVC5demo.com.dto
{
    public class ArchivoDto
    {
        private string extension;

        public string Nombre { get; set; }
        public string Ruta { get; set; }
        public string Lenguaje { get; set; }
        public string CveAplicacion { get; set; }
        public string Extension
        {
            get { return extension; }
            set
            {
                switch (value)
                {
                    case ".cs":
                        Lenguaje = "NET";
                        break;
                    /*case ".java":
                        lenguaje = "JAVA";
                        break;
                    case ".xml":
                        lenguaje = "XML";
                        break;*/
                    default:
                        Lenguaje = value.Replace(".", string.Empty).ToUpper();
                        break;
                }
                extension = value;
            }
        }
        public ConexionBDDto ConexionBd { get; set; }
    }
}