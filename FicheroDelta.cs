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
    internal class FicheroDelta
    {
        private String path;
        private Dictionary<String, int> map = new Dictionary<String, int>();
        private int delta;
        private int count;
        private String[] invalid = { 
            "el", "els", "en", "es", "ets", "'l", "l'", "la", "les", "lo", "los", "n'", "na", "s'", "sa", "ses", "un", "una", "unes", "uns",
            "a", "amb", "arran", "cap", "contra", "d'", "dalt", "damunt", "davall", "de", "deçà", "dellà", "des", "devers", "devora",
            "dintre", "durant", "en", "entre", "envers", "excepte", "fins", "llevat", "malgrat", "mitjançant", "per", "pro", "salvant", "salvat",
            "segons", "sens", "sense", "sobre", "sota", "sots", "tret", "ultra", "via", "al", "als", "del", "dels", "pel", "pels", "as", "des",
            "dets", "pes", "can", "cal", "cals", "cas", "son", "çon", "i", "ni" , "o", "bé", "si", "no", "però", "sinó", "que", "obstant", "això",
            "malgrat", "tanmateix", "ara", "adés", "ja", "un", "altre", "és", "dir", "això", "siga", "doncs", "per", "tant", "conseqüència",
            "encara", "endemés", "més", "encara", "fins", "tot", "malgrat", "encara", "sempre", "perquè", "menys", "aquesta", "ser", "es", "ha",
            "va", "com", "van"
        };
        public FicheroDelta() { }
        public FicheroDelta(String path, int delta) 
        {
            this.path = path;
            this.delta = delta;
        }

        public void DoInfo()
        {
            this.GetFileContent();
            this.MakeInfoFile();
            this.MakeXML();
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

            foreach(var entry in this.map)
            {
                xmlWriter.WriteStartElement("word");
                xmlWriter.WriteAttributeString("number", entry.Value.ToString());
                xmlWriter.WriteString(entry.Key);
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }

        protected void MakeInfoFile()
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

        protected void GetFileContent()
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
                                    this.CheckDelta(w2);
                                    //map.Add(w2, 1);
                                }
                            }
                        }
                    }
                }
                //Console.WriteLine(this.count);
            }
        }
        protected bool CheckDelta(String w)
        {
            foreach(KeyValuePair<String, int> entry in this.map)
            {
                if (entry.Key.Contains(w))
                {
                    //Console.WriteLine(w + "; " + entry.Key + "; "+ (StringDiffNumber(w, entry) <= this.delta));
                    if(StringDiffNumber(w, entry) <= this.delta)
                    {
                        int value;
                        map.TryGetValue(entry.Key, out value);
                        if (entry.Key.Length <= w.Length)
                        {
                            map[entry.Key] = value + 1;
                        }
                        else
                        {
                            map.Remove(entry.Key);
                            map[w] = value + 1;
                        }
                        return true;
                    }
                }
            }
            map.Add(w, 1);
            return false;
        }

        protected int StringDiffNumber(String s, KeyValuePair<String, int> entry)
        {
            int minLength = Math.Min(s.Length, entry.Key.Length);
            int maxLength = Math.Max(s.Length, entry.Key.Length);
            String s1;
            String s2;

            if (entry.Key.Length > minLength)
            {
                s2 = entry.Key;
                s1 = s;
            }
            else
            {
                s2 = s;
                s1 = entry.Key;
            }
            int prefixSame = 0;
            int prefixLength = 0;
            int y = 0;
            try
            {
                for (int i = 0; i < maxLength; i++)
                {
                    if (s1[y] != s2[i])
                    {
                        //Console.WriteLine("Pre s1 = " + s1 + ", s2 = " + s2 + " " + s1[y] + " " + s2[i]);
                        prefixLength++;
                    }
                    else
                    {
                        prefixSame++;
                        y++;
                    }
                }
            }catch (Exception e)
            {

            }
            int suffixSame = 0;
            int suffixLength = 0;
            int j = s1.Length - 1;
            try
            {
                for (int i = 1; i <= maxLength; i++)
                {
                    if (s1[j] != s2[s2.Length - i])
                    {
                        //Console.WriteLine("Suf s1 = " + s1 + ", s2 = " + s2 + " " + s1[j] + " " + s2[s2.Length - i]);
                        suffixLength++;
                    }
                    else
                    {
                        //Console.WriteLine(s1[j] + " " + s2[s2.Length - i]);
                        suffixSame++;
                        j--;
                    }
                }
            } catch (Exception e)
            {

            }
            
            //Console.WriteLine(s + " ; " + entry.Key + " ; " + prefixLength + " ; " + suffixLength);
            int dif = prefixLength + suffixLength;
            return dif;
        }

    }
}
