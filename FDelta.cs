using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ProyectoFicheros
{
    internal class FDelta : Fichero
    {
        protected int delta;
        public FDelta() { }
        public FDelta(String path, int delta) : base(path)
        {
            this.delta = delta;
        }

        protected override void GetFileContent()
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
            foreach (KeyValuePair<String, int> entry in this.map)
            {
                if (entry.Key.Contains(w))
                {
                    //Console.WriteLine(w + "; " + entry.Key + "; "+ (StringDiffNumber(w, entry) <= this.delta));
                    if (StringDiffNumber(w, entry) <= this.delta)
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
            }
            catch (Exception e)
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
            }
            catch (Exception e)
            {

            }

            //Console.WriteLine(s + " ; " + entry.Key + " ; " + prefixLength + " ; " + suffixLength);
            int dif = prefixLength + suffixLength;
            return dif;
        }
    }
}
