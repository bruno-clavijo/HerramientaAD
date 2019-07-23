using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DependenciaMVC5demo.com.dto
{
    public class ProcesoDto
    {
        private long procesoID;
        private int usuarioID;
        private int aplicacionID;
        private string ultMensaje;
        private string estatus;
        private DateTime fechaHora;

        public int UsuarioID
        {
            get { return usuarioID; }
            set { usuarioID = value; }
        }
        
        public int AplicacionID
        {
            get { return aplicacionID; }
            set { aplicacionID = value; }
        }
        
        public string UltMensaje
        {
            get { return ultMensaje; }
            set { ultMensaje = value; }
        }
       
        public string Estatus
        {
            get { return estatus; }
            set { estatus = value; }
        }
        
        public DateTime FechaHora
        {
            get { return fechaHora; }
            set { fechaHora = value; }
        }

        public long ProcesoID
        {
            get { return procesoID; }
            set { procesoID = value; }
        }

    }
}