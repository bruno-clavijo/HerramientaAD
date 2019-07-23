using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Configuration;

namespace DependenciaMVC5demo.com.utilerias
{
    public class ControlLog
    {
        private string pathfile;

        private string ulterror;

        public string Ulterror
        {
          get { return ulterror; }          
        }

        public string Pathfile
        {           
            set { pathfile = value; }
        }

        private void InitPath()
        {
            try
            {

                pathfile = HttpContext.Current.Request.MapPath(ConfigurationManager.AppSettings["pathlog"].ToString() + DateTime.Now.ToString("yyyyMMdd") + ".log");
            }
            catch (Exception)
            {

            }
        }

        private void InitPathWS()
        {
            try
            {

                pathfile = HttpContext.Current.Request.MapPath(ConfigurationManager.AppSettings["pathlog"].ToString() + "WSDep" + DateTime.Now.ToString("yyyyMMdd") + ".log");
            }
            catch (Exception)
            {

            }
        }

        public ControlLog() {
           
        }

        public ControlLog(string name)
        {
         
        }

        public bool EscribeLogWS(string line)
        {
            bool exito = false;
            try
            {
                InitPathWS();
                StreamWriter sw = new StreamWriter(pathfile, true);
                sw.WriteLine(DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + " " + line);
                sw.Flush();
                sw.Close();
                exito = true;
            }
            catch (Exception ex)
            {
                ulterror = ex.Message.ToString();
                exito = false;
            }
            return exito;
        }

        public bool EscribeLog(string line){
            bool exito = false;            
            try {
                InitPath();
                StreamWriter sw = new StreamWriter(pathfile, true);
                sw.WriteLine(DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + " " + line);
                sw.Flush();
                sw.Close();                                   
                exito = true;                           
            }catch(Exception ex){
                ulterror = ex.Message.ToString();
                exito = false;
            }
            return exito;
        }
    }
}