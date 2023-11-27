using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFicheros
{
    internal class FParag : Fichero
    {
        protected ArrayList maps = new ArrayList(); // New
        protected ArrayList counts = new ArrayList(); // New
        public FParag() { }
        public FParag(string path) : base(path) { }

        public override void DoInfo()
        {
            try
            {
                this.GetFileContentParag();
                this.MakeInfoFile();
                this.MakeXML();
                Thread.Sleep(1000);
                Console.WriteLine("The file was summarized successfully");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("There was a problem summarizing!");
                Console.ReadLine();
            }
        }

        protected IOrderedEnumerable<KeyValuePair<string, int>> SortMap(Dictionary<String, int> map)
        {
            return from entry in map orderby entry.Value descending select entry;
        }

        protected string GetTheme(Dictionary<String, int> map)
        {
            string theme = "";
            IOrderedEnumerable<KeyValuePair<string, int>> sorted = this.SortMap(map);
            for (int i = 0; i < 5; i++)
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

        protected override void MakeInfoFile()
        {
            string extension = new FileInfo(this.path).Extension;
            string name = new FileInfo(this.path).Name.Replace(extension, "");
            string infoPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + "PFi" + "\\" + name + "_info";
            DateTime modification = File.GetLastWriteTime(this.path);

            int number_of_parag = 1;

            foreach (Dictionary<String, int> map in this.maps)
            {
                //Console.WriteLine("Here "+ number_of_parag);
                if (File.Exists(infoPath + "P" + number_of_parag + ".txt"))
                {
                    File.WriteAllText(infoPath + "P" + number_of_parag + ".txt", string.Empty);
                }
                using (StreamWriter sw = File.AppendText(infoPath + "P" + number_of_parag + ".txt"))
                {
                    sw.WriteLine("Nom del fixer: " + name);
                    sw.WriteLine("Extensió: " + extension);
                    sw.WriteLine("Data: " + modification.ToShortDateString());
                    sw.WriteLine("Número de paraules: " + this.counts[number_of_parag - 1]);
                    sw.WriteLine("Temàtica: " + this.GetTheme(map));
                    sw.Close();
                }
                number_of_parag++;
            }
        }

        protected void GetFileContentParag()
        {
            if (File.Exists(this.path))
            {
                String content = File.ReadAllText(this.path);
                this.count = 0;
                foreach (String line in content.Split('\n'))
                {
                    Dictionary<String, int> Pmap = new Dictionary<String, int>();
                    int Pcount = 0;
                    foreach (String word in line.Split())
                    {
                        String w2 = word.Replace(",", "").Replace(".", "").Replace("'l", "").Replace("l'", "").Replace("n'", "").Replace("s'", "").Replace("d'", "").ToLower();
                        if (!string.IsNullOrEmpty(w2))
                        {
                            this.count++;
                            Pcount++;
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

                                if (Pmap.ContainsKey(w2))
                                {
                                    int value;
                                    Pmap.TryGetValue(w2, out value);
                                    Pmap[w2] = value + 1;
                                }
                                else
                                {
                                    Pmap.Add(w2, 1);
                                }
                            }
                        }
                    }
                    if (Pmap.Count > 0)
                    {
                        maps.Add(Pmap);
                        counts.Add(Pcount);
                    }

                }
            }
        }
    }
}
