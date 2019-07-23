using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.IO.Compression;

using System.Text;
using System.Configuration;

namespace DependenciaMVC5demo.com.utilerias
{
    public class ZipControl:ControlLog
    {
        private string origen;
        private string destino;

        public string Origen { get => origen; set => origen = value; }
        public string Destino { get => destino; set => destino = value; }

        public bool DescmprimirArchivos() {
            bool resp = false;
            string descomadicional = string.Empty;
            try {
                ZipFile.ExtractToDirectory(origen, destino);
                string[] Archivos = Directory.GetFiles(Destino, "*.ear", SearchOption.AllDirectories);                
                for (int i = 0; i <= Archivos.Count() - 1; i++)
                {
                    origen = Archivos[i];
                    descomadicional = Destino +  ("\\" + "ear" + i.ToString());
                    ZipFile.ExtractToDirectory(origen, descomadicional);
                }

                Archivos = Directory.GetFiles(Destino, "*.war", SearchOption.AllDirectories);
                for (int i = 0; i <= Archivos.Count() - 1; i++)
                {
                    origen = Archivos[i];
                    descomadicional = Destino + ("\\" + "war" + i.ToString());
                    ZipFile.ExtractToDirectory(origen, descomadicional);
                }

                Archivos = Directory.GetFiles(Destino, "*.zip", SearchOption.AllDirectories);
                for (int i = 0; i <= Archivos.Count() - 1; i++)
                {
                    origen = Archivos[i];
                    descomadicional = Destino + ("\\" + "zip" + i.ToString());
                    ZipFile.ExtractToDirectory(origen, descomadicional);
                }
            }
            catch (Exception Err) {
                EscribeLogWS("ZipControl.DescmprimirArchivos " + Err.Message.ToString());
            }            
            return resp;
        }

    }
}