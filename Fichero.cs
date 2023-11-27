using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ProyectoFicheros
{
    internal class Fichero
    {
        protected String path;
        protected Dictionary<String, int> map = new Dictionary<String, int>();
        protected int count;
        protected String[] invalid = { 
            "el", "els", "en", "es", "ets", "'l", "l'", "la", "les", "lo", "los", "n'", "na", "s'", "sa", "ses", "un", "una", "unes", "uns",
            "a", "amb", "arran", "cap", "contra", "d'", "dalt", "damunt", "davall", "de", "deçà", "dellà", "des", "devers", "devora",
            "dintre", "durant", "en", "entre", "envers", "excepte", "fins", "llevat", "malgrat", "mitjançant", "per", "pro", "salvant", "salvat",
            "segons", "sens", "sense", "sobre", "sota", "sots", "tret", "ultra", "via", "al", "als", "del", "dels", "pel", "pels", "as", "des",
            "dets", "pes", "can", "cal", "cals", "cas", "son", "çon", "i", "ni" , "o", "bé", "si", "no", "però", "sinó", "que", "obstant", "això",
            "malgrat", "tanmateix", "ara", "adés", "ja", "un", "altre", "és", "dir", "això", "siga", "doncs", "per", "tant", "conseqüència",
            "encara", "endemés", "més", "encara", "fins", "tot", "malgrat", "encara", "sempre", "perquè", "menys", "aquesta", "ser", "es", "ha",
            "va", "com", "van"
        };
        public Fichero() { }
        public Fichero(String path) 
        {
            this.path = path;
        }

        public virtual void DoInfo()
        {
            try
            {
                this.GetFileContent();
                this.MakeInfoFile();
                this.MakeXML();
                Thread.Sleep(1000);
                Console.WriteLine("The file was summarized successfully");
                Console.ReadLine();
            }
            catch(Exception e)
            {
                Console.WriteLine("There was a problem summarizing!");
                Console.ReadLine();
            }
            
        }

        public String GetPath()
        {
            return this.path;
        }

        public bool SetPath(String path) 
        {
            if(File.Exists(path))
            {
                this.path = path;
                return true;
            }
            else
            {
                return false;
            }
        }

        public Dictionary<string, int> GetMap()
        {
            return this.map;
        }

        protected IOrderedEnumerable<KeyValuePair<string, int>> SortMap()
        {
            return from entry in this.map orderby entry.Value descending select entry;
        }

        protected string GetTheme()
        {
            string theme = "";
            IOrderedEnumerable<KeyValuePair<string, int>> sorted = this.SortMap();
            for(int i = 0; i < 5; i++)
            {
                theme += sorted.ElementAt(i).Key;
                if (i != 4)
                {
                    theme += ", ";
                }
                else
                {
                    theme += ".";
                }
            }
            return theme;
        }

        protected void MakeXML()
        {
            string name = new FileInfo(this.path).Name.Replace(new FileInfo(this.path).Extension, "");
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Indent = true;
            XmlWriter xmlWriter = XmlWriter.Create(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + "PFi" + "\\" + name + "_info.xml",
                xmlWriterSettings);

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("words");

            foreach (var entry in this.map)
            {
                xmlWriter.WriteStartElement("word");
                xmlWriter.WriteAttributeString("number", entry.Value.ToString());
                xmlWriter.WriteString(entry.Key);
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }

        protected virtual void MakeInfoFile()
        {
            string extension = new FileInfo(this.path).Extension;
            string name = new FileInfo(this.path).Name.Replace(extension, "");
            string infoPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + "PFi" + "\\" + name + "_info.txt";
            DateTime modification = File.GetLastWriteTime(this.path);

            if (File.Exists(infoPath))
            {
                File.WriteAllText(infoPath, string.Empty);
            }
            using (StreamWriter sw = File.AppendText(infoPath))
            {
                sw.WriteLine("Nom del fixer: " + name);
                sw.WriteLine("Extensió: " + extension);
                sw.WriteLine("Data: " + modification.ToShortDateString());
                sw.WriteLine("Número de paraules: " + this.count);
                sw.WriteLine("Temàtica: " + this.GetTheme());
                sw.Close();
            }
        }

        protected virtual void GetFileContent()
        {
            if (File.Exists(this.path))
            {
                String content = File.ReadAllText(this.path);
                this.count = 0;
                foreach (String line in content.Split('\n'))
                {
                    foreach (String word in line.Split())
                    {
                        String w2 = word.Replace(",", "").Replace(".", "").Replace("'l", "").Replace("l'", "").Replace("n'", "").Replace("s'", "").Replace("d'", "").ToLower();
                        if (!string.IsNullOrEmpty(w2))
                        {
                            this.count++;
                            if (!this.invalid.Contains(w2))
                            {
                                if (map.ContainsKey(w2))
                                {
                                    int value;
                                    map.TryGetValue(w2, out value);
                                    map[w2] = value + 1;
                                }
                                else
                                {
                                    map.Add(w2, 1);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
