using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DependenciaMVC5demo.com.utilerias
{
    public class Utilerias
    {
        public class ItemsCombo
        {
            private int ivalue;
            public int Ivalue
            {
                get { return ivalue; }
                set { ivalue = value; }
            }

            private string text;
            public string Text
            {
                get { return text; }
                set { text = value; }
            }                        

            public ItemsCombo(int Ivalue, string Text)
            {
                ivalue = Ivalue;
                text = Text;
            }
        }

        public class diagramElem
        {
            public int ide = 0;
            public int top = 0;
            public int left = 0;
            public string name = string.Empty;
            public string info = string.Empty;
            public int ocp = 0;
            public int idepadre = 0;
            public int ispadre = 0;            

            public diagramElem(int ide_p, int top_p, int left_p, string name_p, int ocp_p, int idepadre_p, int ispadre_p,string info_p)
            {
                ide = ide_p;
                top = top_p;
                left = left_p;
                name = name_p;
                ocp = ocp_p;
                idepadre = idepadre_p;
                ispadre = ispadre_p;
                info = info_p;
            }
        }

        public class ItemsCubo {
            private string text;
            public string Text
            {
                get { return text; }
                set { text = value; }
            }

            private string evento;
            public string Evento
            {
                get { return evento; }
                set { evento = value; }
            }

            private string titulo;
            public string Titulo { get => titulo; set => titulo = value; }

            public ItemsCubo(string stexto, string sevento,string titulo1)
            {
                text = stexto;
                evento = sevento;
                titulo = titulo1; 
            }
        }

        public class LineDiagramaDB {
            private int idepadre;
            private int idehijo;
            private string fknombre;
            private string campo_p;
            private string campo_h;

            public LineDiagramaDB(int idepadre, int idehijo, string fknombre, string campo_p, string campo_h)
            {
                this.Idepadre = idepadre;
                this.Idehijo = idehijo;
                this.Fknombre = fknombre;
                this.Campo_p = campo_p;
                this.Campo_h = campo_h;
            }

            public int Idepadre { get => idepadre; set => idepadre = value; }
            public int Idehijo { get => idehijo; set => idehijo = value; }
            public string Fknombre { get => fknombre; set => fknombre = value; }
            public string Campo_p { get => campo_p; set => campo_p = value; }
            public string Campo_h { get => campo_h; set => campo_h = value; }
        }
    }
}