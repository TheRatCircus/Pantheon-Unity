// Markov.cs
// Jerome Martina

using System;
using System.Linq;
using System.Collections.Generic;

namespace Pantheon.Utils
{
    public sealed class Markov
    {
        // Credit to Peter Corbett
        public static readonly string[] baseNames = {"Almy", "Arkwright",
            "Aronow", "Astrand", "Bornite", "Brouwer", "Brume", "Chandalar",
            "Chason", "Cutler", "Darcy", "Delahunty", "Devender", "Dorn",
            "Engber", "Fenlason", "Gambell", "Grayling", "Hafting", "Haggerty",
            "Hairston", "Hamel", "Henzell", "Ignacy", "Iwaarden", "Kneller",
            "Kompel","Larn", "Laskin", "Laufer", "LeRoy", "Lee", "Lennan",
            "Linhart", "Linley", "Lorber", "Luick", "Margulies", "Martel",
            "McGrath", "Meluch", "Menke", "Modrall", "Moria", "Nystrom",
            "Olson", "Opstus", "Oren", "Palmer", "Rankin", "Rupley", "Rupley",
            "Samon", "Sapir", "Scarmer", "Seibert", "Stiker", "Swanson",
            "Thome", "Walz", "Waratah", "Warwick", "Willow", "Yuval" };

        Dictionary<string, List<string>> dict;
        List<string> oldNames;
        int chainLength;

        public Markov(int chainLength = 2)
        {
            if (chainLength < 1 || chainLength > 10)
            {
                throw new ArgumentException
                    ("Chain length must be between 1 and 10, inclusive.");
            }

            dict = new Dictionary<string, List<string>>();
            oldNames = new List<string>();
            this.chainLength = chainLength;

            foreach (string c in baseNames)
            {
                string trimmed = c.Trim();
                oldNames.Add(trimmed);

                string s = "";
                for (int i = 0; i < chainLength; i++)
                {
                    s += " ";
                }
                s += trimmed;
                for (int i = 0; i < trimmed.Length; i++)
                {
                    Add(s.Substring(i, chainLength), s[i + chainLength]
                        .ToString());
                }
                Add(s.Substring(trimmed.Length, chainLength),
                    $"\n");
            }
        }

        private void Add(string prefix, string suffix)
        {
            if (dict.TryGetValue(prefix, out List<string> s))
            {
                s.Add(suffix);
            }
            else
            {
                dict.Add(prefix, new List<string> { suffix });
            }
        }

        private string GetSuffix(string prefix)
        {
            dict.TryGetValue(prefix, out List<string> l);
            return l.Random();
        }

        public string GetName()
        {
            string prefix = "";
            for (int i = 0; i < chainLength; i++)
            {
                prefix += " ";
            }
            string name = "";
            string suffix = "";
            while (true)
            {
                suffix = GetSuffix(prefix);
                if (suffix == $"\n" || name.Length > 9)
                {
                    break;
                }
                else
                {
                    name += suffix;
                    prefix = prefix.Substring(1) + suffix;
                }
            }
            return name.First().ToString().ToUpper() + name.Substring(1);
        }
    }
}
