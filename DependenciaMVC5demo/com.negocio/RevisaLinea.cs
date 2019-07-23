using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace DependenciaMVC5demo.com.negocio
{
    public class RevisaLinea
    {
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string TipoPadre { get; set; }
        public string BibPadre { get; set; }
        public string ObjPadre { get; set; }
        public string TipoHijo { get; set; }
        public string BibHijo { get; set; }
        public string ObjHijo { get; set; }

        public   bool EsValido;
        public   string NombreEspacio = "";
        private  short Nivel = 0;
        public   int DependenciaID = 0;
        private  int UltimoPadre = 0;
        private  string UltimoEspacio = "";
        private  List<string> ListaClases = new List<string>();
        private  HashSet<string> Instancias = new HashSet<string>();
        private  List<string> ListaPadres = new List<string>();
        private List<string> LibreriasArchivo = new List<string>();
        private  string UltimoMetodo = "";
        EncuentraMetodo em = new EncuentraMetodo();

        //Genera el inventario de Clases y Metodos para buscar los metodos que solo se invocan en código
        public RevisaLinea ObtenerInventario(string LineaCodigo, HashSet<string> Librerias)
        {
            try
            {
                
                RevisaLinea DatosInventario = new RevisaLinea();                
                EsValido = false;

                LineaCodigo = LineaCodigo.Trim();
                string Linea = LineaCodigo.ToLower();

                if (Linea.StartsWith("using ") || Linea.StartsWith("import ") ||
                    Linea.StartsWith("imports "))
                {
                    string Libreria = Regex.Replace(Regex.Replace(LineaCodigo, @"\w+\s", string.Empty), ";",
                    string.Empty).Trim();
                    bool Excepcion = EsExcepcion(Linea);
                    if (!Excepcion)
                    {
                        Librerias.Add(Libreria);
                    }
                }
                else if (Linea.IndexOf("namespace ") > -1 || Linea.IndexOf("package ") > -1)
                {
                    NombreEspacio = em.ExtraerNombreEspacio(LineaCodigo);
                }
                else if (((Linea.IndexOf("class ") >= 0) && (Linea.IndexOf(".class") < 0)) || Linea.IndexOf("javascript") > -1)
                {
                    Nombre = em.ExtraerNombreClase(LineaCodigo);
                    Tipo = "Clase";
                    if (Nombre != "")
                        EsValido = true;
                }
                else if (em.EsFuncion(Linea))
                {
                    Nombre = em.ExtraerNombreFuncion(LineaCodigo);
                    Tipo = "Funcion";
                    if (Nombre != "")
                        EsValido = true;
                }
                else if (em.EsMetodo(Linea))
                {
                    
                    Nombre = em.ExtraerNombreMetodo(LineaCodigo);
                    Tipo = Linea.IndexOf("eventargs") > -1 ? "Evento" : "Metodo";
                    if (Nombre != "")
                        EsValido = true;
                }
                

                DatosInventario = this;
                return DatosInventario;
            }
            catch (Exception ex)
            {
                throw new Exception("RevisaLinea.ObtenerInventario ", ex);            
            }
        }

        public  RevisaLinea ObtenerSalida(string LineaCodigo, string Archivo, HashSet<string> InventarioCM,
            List<string> Resultado, int NoLinea, string Ruta, HashSet<string> Librerias)
        {
            try
            {
                RevisaLinea DatosLinea = new RevisaLinea();
                string LineaOriginal = LineaCodigo;
                LineaCodigo = LineaCodigo.ToLower();
                LineaCodigo = LineaCodigo.Replace("\t", " ");
                LineaOriginal = LineaOriginal.Replace("\t", " ");

                string Extension = Regex.Replace(Archivo, @"\w+\.", string.Empty).Trim();
                Archivo = Regex.Replace(Archivo, @"\.\w+", string.Empty).Trim();

                if (LineaCodigo.StartsWith("using ") || LineaCodigo.StartsWith("import ") ||
                    LineaCodigo.StartsWith("imports ") || LineaCodigo.StartsWith("namespace ") ||
                    LineaCodigo.StartsWith("package "))
                {
                    bool Excepcion = EsExcepcion(LineaCodigo.ToLower());
                    if (!Excepcion)
                    {
                        ObtenerDatosEspacioLibreria(DatosLinea, LineaCodigo, LineaOriginal, Archivo, Extension);
                        if (DatosLinea.ObjHijo.Length > 0)
                        {
                            LibreriasArchivo.Add(DatosLinea.ObjHijo.ToString());
                            ++DependenciaID;
                            Resultado.Add(DatosLinea.TipoPadre + "|" + DatosLinea.BibPadre + "|" + DatosLinea.ObjPadre + "|" + DatosLinea.TipoHijo + "|" + DatosLinea.BibHijo + "|" + DatosLinea.ObjHijo + "|" + Ruta + "|" + NoLinea + "|" + Extension + "|" + DependenciaID + "|" + UltimoPadre + "|" + LineaOriginal.Replace("|", string.Empty));
                        }
                    }
                }
                else if (((LineaCodigo.IndexOf("class ") >= 0) && (LineaCodigo.IndexOf(".class") < 0)) || LineaCodigo.IndexOf("javascript") > -1)
                {
                    bool Excepcion = EsExcepcion(LineaCodigo.ToLower());
                    if (!Excepcion)
                    {
                        ObtenerDatosClase(DatosLinea, LineaOriginal, Archivo, Extension);
                        if (DatosLinea.ObjHijo.Length > 0)
                        {
                            ++DependenciaID;
                            Resultado.Add(DatosLinea.TipoPadre + "|" + DatosLinea.BibPadre + "|" + DatosLinea.ObjPadre + "|" + DatosLinea.TipoHijo + "|" + DatosLinea.BibHijo + "|" + DatosLinea.ObjHijo + "|" + Ruta + "|" + NoLinea + "|" + Extension + "|" + DependenciaID + "|" + UltimoPadre + "|" + LineaOriginal.Replace("|", string.Empty));
                        }
                    }
                }
                else if (LineaCodigo.IndexOf("interface ") > -1)
                {
                    bool Excepcion = EsExcepcion(LineaCodigo.ToLower());
                    if (!Excepcion)
                    {
                        ObtenerDatosInterface(DatosLinea, LineaOriginal, Archivo, Extension);
                        if (DatosLinea.ObjHijo.Length > 0)
                        {
                            ++DependenciaID;
                            Resultado.Add(DatosLinea.TipoPadre + "|" + DatosLinea.BibPadre + "|" + DatosLinea.ObjPadre + "|" + DatosLinea.TipoHijo + "|" + DatosLinea.BibHijo + "|" + DatosLinea.ObjHijo + "|" + Ruta + "|" + NoLinea + "|" + Extension + "|" + DependenciaID + "|" + UltimoPadre + "|" + LineaOriginal.Replace("|", string.Empty));
                        }
                    }
                }
                else if (em.EsFuncion(LineaCodigo))
                {
                    ObtenerDatosFuncion(DatosLinea, LineaCodigo, LineaOriginal, Archivo);
                    bool Variable = EsVariable(DatosLinea.ObjHijo, 1);
                    if (DatosLinea.ObjHijo.Length > 0 && !Variable)
                    {
                        ++DependenciaID;
                        Resultado.Add(DatosLinea.TipoPadre + "|" + DatosLinea.BibPadre + "|" + DatosLinea.ObjPadre + "|" + DatosLinea.TipoHijo + "|" + DatosLinea.BibHijo + "|" + DatosLinea.ObjHijo + "|" + Ruta + "|" + NoLinea + "|" + Extension + "|" + DependenciaID + "|" + UltimoPadre + "|" + LineaOriginal.Replace("|", string.Empty));
                    }
                }
                else if (em.EsObjeto(LineaCodigo))
                {
                    ObtenerDatosObjeto(DatosLinea, LineaCodigo, LineaOriginal, Archivo);
                    bool Variable = EsVariable(DatosLinea.ObjHijo, 1);
                    if (DatosLinea.ObjHijo.Length > 0 && !Variable)
                    {
                        ++DependenciaID;
                        Resultado.Add(DatosLinea.TipoPadre + "|" + DatosLinea.BibPadre + "|" + DatosLinea.ObjPadre + "|" + DatosLinea.TipoHijo + "|" + DatosLinea.BibHijo + "|" + DatosLinea.ObjHijo + "|" + Ruta + "|" + NoLinea + "|" + Extension + "|" + DependenciaID + "|" + UltimoPadre + "|" + LineaOriginal.Replace("|", string.Empty));
                    }
                }
                else if (em.EsMetodo(LineaCodigo))
                {
                    bool Excepcion = EsExcepcion(LineaCodigo.ToLower());
                    if (!Excepcion)
                    {
                        ObtenerDatosMetodo(DatosLinea, LineaCodigo, LineaOriginal, Archivo);
                        bool Variable = EsVariable(DatosLinea.ObjHijo, 1);
                        if (ListaClases.Count > 0 && DatosLinea.ObjHijo.Length > 0 && !Variable)
                        {
                            ++DependenciaID;
                            Resultado.Add(DatosLinea.TipoPadre + "|" + DatosLinea.BibPadre + "|" + DatosLinea.ObjPadre + "|" + DatosLinea.TipoHijo + "|" + DatosLinea.BibHijo + "|" + DatosLinea.ObjHijo + "|" + Ruta + "|" + NoLinea + "|" + Extension + "|" + DependenciaID + "|" + UltimoPadre + "|" + LineaOriginal.Replace("|", string.Empty));
                        }
                    }
                }
                else 
                {
                    bool Excepcion = EsExcepcion(LineaCodigo.ToLower());
                    if (!Excepcion && LineaCodigo.Length >= 3)
                    {
                        DependenciaID = ObtenerDatosInvocados(DatosLinea, LineaOriginal, Archivo, InventarioCM, Resultado,
                        NoLinea, Ruta, Librerias, Extension, DependenciaID);
                    }
                }
                AcumularNivelLlave(LineaCodigo);
                return DatosLinea;
            }
            catch (Exception ex)
            {
                AcumularNivelLlave(LineaCodigo);
                Console.WriteLine(ex);
                return null;
            }
        }

        private bool EsVariable(string LineaCodigo, int Metodo)
        {
            bool Var = false;
            string Linea = string.IsNullOrEmpty(LineaCodigo) ? string.Empty : LineaCodigo.ToLower();
            string[] Variables = new string[] {"string", "int", "long", "double", "bool", "decimal", "list", "byte", "integer", "date", "object"};

            if (Metodo == 0)
            {
                foreach (string Variable in Variables)
                {
                    Regex Regex = new Regex(@"private\s*" + Variable);
                    Match Match = Regex.Match(Linea);
                    if (Match.Success)
                        Var = true;
                }

                foreach (string Variable in Variables)
                {
                    Regex Regex = new Regex(@"public\s*" + Variable);
                    Match Match = Regex.Match(Linea);
                    if (Match.Success)
                        Var = true;
                }
            }
            else
            {
                foreach (string Variable in Variables)
                {
                    Regex Regex = new Regex(@"([^/\\\w<>{}-]|^)" + Variable + @"([^/\\\w<>{}-]|$)");
                    Match Match = Regex.Match(Linea);
                    if (Match.Success)
                        Var = true;
                }
            }

            return Var;
        }

        public  bool EsExcepcion(string LineaCodigo)
        {
            bool Exc = false;
            string Linea = string.IsNullOrEmpty(LineaCodigo) ? string.Empty : LineaCodigo.ToLower();
            string[] Excepciones = new string[] { "throw new sqlexception", "throw new exception", "logger.info",
                "logger.error", "logger.debug", "writeerrorlog", "using (", ".append", "else if", "byte[]",
                "static final string" };
            
            foreach (string Excepcion in Excepciones)
            {
                if (Linea.IndexOf(Excepcion) > -1)
                    Exc = true;
            }

            return Exc;
        }

        //Obtener los datos del Espacio de Nombres actual
        private  void ObtenerDatosEspacioLibreria(RevisaLinea DatosLinea, string LineaCodigo,
            string LineaOriginal, string Archivo, string Extension)
        {
            try
            {
                string Espacio = Regex.Replace(Regex.Replace(LineaOriginal, @"\w+\s", string.Empty), ";",
                    string.Empty).Trim();
                DatosLinea.TipoPadre = Extension;
                DatosLinea.BibPadre = Archivo;
                DatosLinea.ObjPadre = Archivo;
                DatosLinea.TipoHijo = "Libreria";

                if (LineaCodigo.StartsWith("namespace") || LineaCodigo.StartsWith("package"))
                    DatosLinea.TipoHijo = "Espacio de Nombres";

                Espacio = Espacio.Replace("= ", string.Empty);
                DatosLinea.BibHijo = Archivo;
                DatosLinea.ObjHijo = Espacio;

                if (LineaCodigo.StartsWith("namespace"))
                    UltimoEspacio = Espacio;

                if (LineaCodigo.StartsWith("package"))
                    UltimoEspacio = Espacio;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        //Obtener los datos de Interface
        private  void ObtenerDatosInterface(RevisaLinea DatosLinea, string LineaOriginal,
            string Archivo, string Extension)
        {
            try
            {
                DatosLinea.TipoPadre = Extension;
                DatosLinea.BibPadre = Archivo;
                DatosLinea.ObjPadre = Archivo;
                DatosLinea.TipoHijo = "Interface";
                DatosLinea.BibHijo = Archivo;
                NombreEspacio= em.ExtraerNombreInterface(LineaOriginal);
                string Interface = NombreEspacio;
                DatosLinea.ObjHijo = Interface;

                if (!string.IsNullOrEmpty(Interface))
                {
                    ListaClases.Add(Interface + "|" + Nivel);
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        //Obtener los datos de la Clase actual
        private  void ObtenerDatosClase(RevisaLinea DatosLinea, string LineaOriginal,
            string Archivo, string Extension)
        {
            try
            {
                if (!string.IsNullOrEmpty(UltimoMetodo) && Nivel > 1)
                {
                    DatosLinea.TipoPadre = "Metodo";
                    DatosLinea.BibPadre = Archivo;
                    DatosLinea.ObjPadre = UltimoMetodo;
                }
                else
                {
                    DatosLinea.TipoPadre = Extension;
                    DatosLinea.BibPadre = Archivo;
                    DatosLinea.ObjPadre = Archivo;
                }
                DatosLinea.TipoHijo = "Clase";

                if (LineaOriginal.IndexOf("javascript") > -1)
                    DatosLinea.TipoHijo = "JavaScript";

                DatosLinea.BibHijo = Archivo;
                string Clase = em.ExtraerNombreClase(LineaOriginal);
                DatosLinea.ObjHijo = Clase;

                //Genera la lista de Clases para controlar Clases dentro de Metodos
                if (!string.IsNullOrEmpty(Clase))
                {
                    ListaClases.Add(Clase + "|" + Nivel);
                }   
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        //Obtener los datos del Método actual
        private  void ObtenerDatosMetodo(RevisaLinea DatosLinea, string LineaCodigo,
            string LineaOriginal, string Archivo)
        {
            try
            {
                if (ListaClases.Count > 0)
                {
                    string[] Clase = ListaClases.ElementAt(ListaClases.Count - 1).Split('|');

                    DatosLinea.TipoPadre = "Clase";
                    DatosLinea.BibPadre = Archivo;
                    DatosLinea.ObjPadre = Clase[0];
                    string Metodo = em.ExtraerNombreMetodo(LineaOriginal);
                    DatosLinea.TipoHijo = "Metodo";

                    if (LineaCodigo.IndexOf("eventargs") > -1)
                        DatosLinea.TipoHijo = "Evento";

                    if (Metodo == Clase[0])
                        DatosLinea.TipoHijo = "Constructor";

                    DatosLinea.BibHijo = Clase[0];
                    DatosLinea.ObjHijo = Metodo;

                    UltimoMetodo = Metodo;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        //Obtener los datos del Objeto actual
        private void ObtenerDatosObjeto(RevisaLinea DatosLinea, string LineaCodigo,
            string LineaOriginal, string Archivo)
        {
            try
            {
                DatosLinea.TipoPadre = "Metodo";
                DatosLinea.BibPadre = Archivo;
                string Metodo = em.ExtraerNombreMetodo(LineaOriginal);
                DatosLinea.ObjPadre = Metodo;

                DatosLinea.TipoHijo = "Objeto";
                DatosLinea.BibHijo = Metodo;
                string Objeto = em.ExtraerNombreObjeto(LineaOriginal);
                DatosLinea.ObjHijo = Objeto;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        //Obtener los datos de la Función actual
        private void ObtenerDatosFuncion(RevisaLinea DatosLinea, string LineaCodigo,
            string LineaOriginal, string Archivo)
        {
            try
            {
                DatosLinea.TipoPadre = "Clase";
                DatosLinea.BibPadre = Archivo;
                DatosLinea.ObjPadre = Archivo;
                string Funcion = em.ExtraerNombreFuncion(LineaOriginal);
                DatosLinea.TipoHijo = "Funcion";
                DatosLinea.BibHijo = Archivo;
                DatosLinea.ObjHijo = Funcion;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        //Cuando se encuentra un metodo o una clase pero solo son invocados
        private int ObtenerDatosInvocados(RevisaLinea DatosLinea, string LineaOriginal, string Archivo,
            HashSet<string> InventarioCM, List<string> Resultado, int NoLinea, string Ruta,
            HashSet<string> Librerias, string Extension, int DependenciaID)
        {
            try
            {
                List<string> Subconjunto;

                if ((LineaOriginal.IndexOf(".class") == -1) && ((LineaOriginal.IndexOf("private") > -1) ||
                    (LineaOriginal.IndexOf("public") > -1) || (LineaOriginal.IndexOf("protected") > -1) ||
                    (LineaOriginal.IndexOf("final") > -1) || 
                    (LineaOriginal.IndexOf("static") > -1) ||
                    (LineaOriginal.IndexOf("= new") > -1)))
                {
                    string Linea = LineaOriginal.Replace(" final", string.Empty);
                    bool Variable = EsVariable(LineaOriginal, 0);
                    Regex Regex = new Regex(@"(private \s\w+\s\w+)|(public \s\w+\s\w+)|(protected \s\w+\s\w+)|(private static\s\w+\s\w+)|(public static\s\w+\s\w+)|(protected static\s\w+\s\w+)|(\w+\s\w+|\w+\<\w+\>\s\w+|\w+\[\]\s\w+)\s*= new");
                    Match Match = Regex.Match(Linea);
                    if ((Match.Success) && (!Variable))
                    {
                        string Instancia;
                        string Clase;
                        if (LineaOriginal.IndexOf("= new") > -1)
                        {
                            Linea = Linea.Substring(0, Linea.IndexOf("= new")).Trim();
                            Instancia = Linea.Split(' ').Last();
                            Clase = Linea.Split(' ').ElementAt(Linea.Split(' ').Length - 2);
                            Clase = Clase.Replace("[]", string.Empty);

                            foreach (string Libreria in LibreriasArchivo)
                            {
                                string LibreriaAux = Libreria.Split('.').Length <= 1 ? Libreria : Libreria.Split('.').Last();
                                if (LibreriaAux.Length <= 1)
                                    LibreriaAux = "No Valida";
                                Regex = new Regex(@"" + LibreriaAux);
                                Match = Regex.Match(LineaOriginal);

                                if (Match.Success)
                                {
                                    string Espacio = Libreria.Split('.').Length == 1 ? Libreria :
                                    Libreria.Substring(0, Libreria.Length - Libreria.Split('.').Last().Length - 1);

                                    if (!string.IsNullOrEmpty(Clase) && !string.IsNullOrEmpty(Instancia))
                                        Instancias.Add(Clase + "|" + Instancia + "|" + Espacio);
                                }

                            }
                        }
                        else
                        {
                            Clase = Match.Value.Split(' ').ElementAt(Match.Value.Split(' ').Count() - 2);
                        }

                        if (string.IsNullOrEmpty(UltimoMetodo))
                        {
                            string[] ClaseLista = ListaClases.ElementAt(ListaClases.Count - 1).Split('|');

                            DatosLinea.TipoPadre = "Clase";
                            DatosLinea.BibPadre = Archivo;
                            DatosLinea.ObjPadre = ClaseLista[0];
                        }
                        else
                        {
                            DatosLinea.TipoPadre = "Metodo";
                            DatosLinea.BibPadre = Archivo;
                            DatosLinea.ObjPadre = UltimoMetodo;
                        }
                        DatosLinea.TipoHijo = "Clase";
                        DatosLinea.BibHijo = Clase;
                        DatosLinea.ObjHijo = Clase;

                        Variable = EsVariable(DatosLinea.ObjHijo, 1);
                        if (!string.IsNullOrEmpty(DatosLinea.ObjHijo) && !Variable)
                        {
                            ++DependenciaID;
                            Resultado.Add(DatosLinea.TipoPadre + "|" + DatosLinea.BibPadre + "|" + DatosLinea.ObjPadre + "|" + DatosLinea.TipoHijo + "|" + DatosLinea.BibHijo + "|" + DatosLinea.ObjHijo + "|" + Ruta + "|" + NoLinea + "|" + Extension + "|" + DependenciaID + "|" + UltimoPadre + "|" + LineaOriginal.Replace("|", string.Empty));
                        }
                    }
                }
                else
                {
                    Regex Regex = new Regex(@"\w+\.\w+");
                    Match Match = Regex.Match(LineaOriginal);
                    
                    if (Match.Success)
                    {
                        //Para C#
                        string ObjetoClase = Match.Value.Split('.').First();
                        string ObjetoMetodo = Match.Value.Split('.').Last();
                        string ClaseOriginal = string.Empty;
                        Subconjunto = (from ICM in InventarioCM
                                       where ICM.Split('|').First() == ObjetoClase
                                       select ICM).ToList();
                        //Para Java
                        if (Subconjunto.Count == 0)
                        {
                            string Espacio = string.Empty;
                            foreach (string Instancia in Instancias)
                            {
                                if (ObjetoClase == Instancia.Split('|').ElementAt(1).ToString())
                                {
                                    Espacio = Instancia.Split('|').Last();
                                    ClaseOriginal = Instancia.Split('|').First();
                                }

                            }
                            if (!string.IsNullOrEmpty(Espacio))
                            {
                                Subconjunto = (from ICM in InventarioCM
                                               where ICM.Split('|').First() == Espacio && ICM.Split('|').ElementAt(2) == "Metodo"
                                               select ICM).ToList();
                            }
                        }
                        bool Excepcion = EsExcepcion(LineaOriginal);
                        foreach (string Item in Subconjunto)
                        {
                            Regex = new Regex(@"([^/\\\w<>{}-]|^)" + Item.Split('|').ElementAt(1) + @"([^/\\\w<>{}-]|$)");
                            Match = Regex.Match(LineaOriginal);

                            if ((Match.Success) && (!Excepcion))
                            {
                                if (ListaClases.Count > 0 && Item.Split('|').ElementAt(1) == ObjetoMetodo)
                                {
                                    string[] Clases = ListaClases.ElementAt(ListaClases.Count - 1).Split('|');

                                    DatosLinea.TipoPadre = "Clase";
                                    DatosLinea.BibPadre = Archivo;
                                    DatosLinea.ObjPadre = Clases[0];
                                    DatosLinea.TipoHijo = Item.Split('|').ElementAt(2);
                                    DatosLinea.BibHijo = string.IsNullOrEmpty(ClaseOriginal) ? Item.Split('|').ElementAt(0) : ClaseOriginal ;
                                    DatosLinea.ObjHijo = Item.Split('|').ElementAt(1);

                                    bool Variable = EsVariable(DatosLinea.ObjHijo, 1);
                                    if (!string.IsNullOrEmpty(DatosLinea.ObjHijo) && !Variable)
                                    {
                                        ++DependenciaID;
                                        Resultado.Add(DatosLinea.TipoPadre + "|" + DatosLinea.BibPadre + "|" + DatosLinea.ObjPadre + "|" + DatosLinea.TipoHijo + "|" + DatosLinea.BibHijo + "|" + DatosLinea.ObjHijo + "|" + Ruta + "|" + NoLinea + "|" + Extension + "|" + DependenciaID + "|" + UltimoPadre + "|" + LineaOriginal.Replace("|", string.Empty));
                                    }
                                }
                            }
                        }

                    }
                    else
                    {
                        if (Librerias.Count > 0)
                        {
                            foreach (string Libreria in Librerias)
                            {
                                string LibreriaAux = Libreria.Split('.').Length <= 1 ? Libreria : Libreria.Split('.').Last();
                                if (LibreriaAux.Length <= 1)
                                    LibreriaAux = "No Valida";
                                Regex = new Regex(@"" + LibreriaAux);
                                Match = Regex.Match(LineaOriginal);
                                
                                if (Match.Success)
                                {
                                    string Espacio = Libreria.Split('.').Length == 1 ? Libreria :
                                    Libreria.Substring(0, Libreria.Length - Libreria.Split('.').Last().Length - 1);
                                    Regex = new Regex(@"" + Match.Value + @"\s*\w+");
                                    Match = Regex.Match(LineaOriginal);
                                    if (Match.Success && Match.Value.Split(' ').Length > 1)
                                    {
                                        string Instancia = Match.Value.Split(' ').Last();
                                        string ClaseInstancia = Match.Value.Split(' ').First();

                                        if (!string.IsNullOrEmpty(ClaseInstancia) && !string.IsNullOrEmpty(Instancia))
                                            Instancias.Add(ClaseInstancia + "|" + Instancia + "|" + Espacio);

                                        if (string.IsNullOrEmpty(UltimoMetodo))
                                        {
                                            string[] ClaseLista = ListaClases.ElementAt(ListaClases.Count - 1).Split('|');

                                            DatosLinea.TipoPadre = "Clase";
                                            DatosLinea.BibPadre = Archivo;
                                            DatosLinea.ObjPadre = ClaseLista[0];
                                        }
                                        else
                                        {
                                            DatosLinea.TipoPadre = "Metodo";
                                            DatosLinea.BibPadre = Archivo;
                                            DatosLinea.ObjPadre = UltimoMetodo;
                                        }
                                        DatosLinea.TipoHijo = "Clase";
                                        DatosLinea.BibHijo = ClaseInstancia;
                                        DatosLinea.ObjHijo = ClaseInstancia;

                                        bool Variable = EsVariable(DatosLinea.ObjHijo, 1);
                                        if (!string.IsNullOrEmpty(DatosLinea.ObjHijo) && !Variable)
                                        {
                                            ++DependenciaID;
                                            Resultado.Add(DatosLinea.TipoPadre + "|" + DatosLinea.BibPadre + "|" + DatosLinea.ObjPadre + "|" + DatosLinea.TipoHijo + "|" + DatosLinea.BibHijo + "|" + DatosLinea.ObjHijo + "|" + Ruta + "|" + NoLinea + "|" + Extension + "|" + DependenciaID + "|" + UltimoPadre + "|" + LineaOriginal.Replace("|", string.Empty));
                                            break;
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return DependenciaID;
        }

        //LLeva la cuenta de los niveles de profundidad de acuerdo a las llaves "{}"
        private  void AcumularNivelLlave(string LineaCodigo)
        {
            try
            {
                if ((LineaCodigo.IndexOf("{") > -1) || (LineaCodigo.IndexOf("}") > -1))
                {
                    string[] Padre = null;
                    if (ListaPadres.Count > 0)
                        Padre = ListaPadres.ElementAt(ListaPadres.Count - 1).Split('|');

                    foreach (char a in LineaCodigo)
                    {
                        if (a == '{')
                        {
                            Nivel++;
                            UltimoPadre = DependenciaID;

                            if (ListaPadres.Count == 0 || UltimoPadre != Convert.ToInt32(Padre[0]))
                            {
                                ListaPadres.Add(UltimoPadre + "|" + Nivel);
                            }
                        }

                        if (a == '}')
                        {

                            if (Convert.ToInt32(Padre[1]) == Nivel)
                            {
                                ListaPadres.RemoveAt(ListaPadres.Count - 1);
                                UltimoPadre = ListaPadres.Count == 0 ? 0 : Convert.ToInt32(ListaPadres.Last().Split('|').ElementAt(0));
                            }
                            Nivel--;
                            VerificarFinClase();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        //Si la ultima llave } termina la clase actual se elimina la clase actual de la lista de clases
        private  void VerificarFinClase()
        {
            try
            {
                if (ListaClases.Count > 0)
                {
                    string[] UltimaClase = ListaClases.ElementAt(ListaClases.Count - 1).Split('|');

                    if (Nivel == Convert.ToInt32(UltimaClase[1]))
                    {
                        ListaClases.RemoveAt(ListaClases.Count - 1);
                    }
                        
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}