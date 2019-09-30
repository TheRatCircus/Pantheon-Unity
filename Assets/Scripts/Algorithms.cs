// Algorithms.cs
// Jerome Martina

using Pantheon.World;
using Pantheon.WorldGen;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pantheon.Utils
{
    public static class Algorithms
    {
        /// <summary>
        /// Bresenham's circle algorithm.
        /// </summary>
        /// <param name="xc">X coordinate of circle centre.</param>
        /// <param name="yc">Y coordinate of circle centre.</param>
        /// <param name="r">Radius.</param>
        /// <param name="action"></param>
        public static void DrawCircle(int xc, int yc, int r,
            Action<int, int> action)
        {
            // Credit to Shivam Pradhan
            int x = 0, y = r;
            int d = 3 - 2 * r;
            Subsequence();
            while (y >= x)
            {
                x++;

                if (d > 0)
                {
                    y--;
                    d = d + 4 * (x - y) + 10;
                }
                else
                {
                    d = d + 4 * x + 6;
                }
                Subsequence();
            }

            void Subsequence()
            {
                action.Invoke(xc + x, yc + y);
                action.Invoke(xc - x, yc + y);
                action.Invoke(xc + x, yc - y);
                action.Invoke(xc - x, yc - y);
                action.Invoke(xc + y, yc + x);
                action.Invoke(xc - y, yc + x);
                action.Invoke(xc + y, yc - x);
                action.Invoke(xc - y, yc - x);
            }
        }

        public static HashSet<Cell> FloodFill(Level level, Cell start)
        {
            HashSet<Cell> filled = new HashSet<Cell>();
            List<Cell> open = new List<Cell>();
            HashSet<Cell> closed = new HashSet<Cell>();

            filled.Add(start);
            open.Add(start);

            while (open.Count > 0)
            {
                // Keep a temporary list so open can be emptied and then
                // refreshed from scratch
                List<Cell> temp = new List<Cell>();
                for (int i = 0; i < open.Count; i++)
                {
                    closed.Add(open[i]);
                    for (int x = -1; x <= 1; x++)
                        for (int y = -1; y <= 1; y++)
                        {
                            Vector2Int frontier = open[i].Position;
                            frontier += new Vector2Int(x, y);
                            Cell frontierCell;

                            if (level.Contains(frontier))
                                frontierCell = level.GetCell(frontier);
                            else
                                continue;

                            if (closed.Contains(frontierCell))
                                continue;

                            if (frontierCell.Blocked)
                            {
                                closed.Add(frontierCell);
                                continue;
                            }

                            if (filled.Contains(frontierCell))
                            {
                                closed.Add(frontierCell);
                                continue;
                            }

                            filled.Add(frontierCell);
                            temp.Add(frontierCell);
                        }
                }
                open.Clear();
                open.AddRange(temp);
                temp.Clear();
            }
            return filled;
        }

        public static HashSet<Cell> FloodFill(Level level, LevelRect rect,
            Cell start)
        {
            HashSet<Cell> filled = new HashSet<Cell>();
            List<Cell> open = new List<Cell>();
            HashSet<Cell> closed = new HashSet<Cell>();

            filled.Add(start);
            open.Add(start);

            while (open.Count > 0)
            {
                for (int i = 0; i < open.Count; i++)
                {
                    closed.Add(open[i]);
                    for (int x = -1; x <= 1; x++)
                        for (int y = -1; y <= 1; y++)
                        {
                            Vector2Int frontier = open[i].Position;
                            frontier += new Vector2Int(x, y);
                            Cell frontierCell;

                            if (level.Contains(frontier) &&
                                rect.Contains(frontier))
                                frontierCell = level.GetCell(frontier);
                            else
                                continue;

                            if (closed.Contains(frontierCell))
                                continue;

                            if (frontierCell.Blocked)
                            {
                                closed.Add(frontierCell);
                                continue;
                            }

                            if (filled.Contains(frontierCell))
                            {
                                closed.Add(frontierCell);
                                continue;
                            }

                            filled.Add(frontierCell);
                            open.Add(frontierCell);
                        }
                    open.RemoveAt(i);
                }
            }
            return filled;
        }

        public static HashSet<Cell> FloodFill(Level level, Cell start,
            Action<Cell> onFillDelegate)
        {
            HashSet<Cell> filled = new HashSet<Cell>();
            List<Cell> open = new List<Cell>();
            HashSet<Cell> closed = new HashSet<Cell>();

            filled.Add(start);
            open.Add(start);

            while (open.Count > 0)
            {
                // Keep a temporary list so open can be emptied and then
                // refreshed from scratch
                List<Cell> temp = new List<Cell>();
                for (int i = 0; i < open.Count; i++)
                {
                    closed.Add(open[i]);
                    for (int x = -1; x <= 1; x++)
                        for (int y = -1; y <= 1; y++)
                        {
                            Vector2Int frontier = open[i].Position;
                            frontier += new Vector2Int(x, y);
                            Cell frontierCell;

                            if (level.Contains(frontier))
                                frontierCell = level.GetCell(frontier);
                            else
                                continue;

                            if (closed.Contains(frontierCell))
                                continue;

                            if (frontierCell.Blocked)
                            {
                                closed.Add(frontierCell);
                                continue;
                            }

                            if (filled.Contains(frontierCell))
                            {
                                closed.Add(frontierCell);
                                continue;
                            }

                            onFillDelegate.Invoke(frontierCell);
                            filled.Add(frontierCell);
                            temp.Add(frontierCell);
                        }
                }
                open.Clear();
                open.AddRange(temp);
                temp.Clear();
            }
            return filled;
        }

        public static HashSet<Cell> FloodFill(Level level, Cell start,
            Predicate<Cell> onFillPredicate)
        {
            HashSet<Cell> filled = new HashSet<Cell>();
            List<Cell> open = new List<Cell>();
            HashSet<Cell> closed = new HashSet<Cell>();

            filled.Add(start);
            open.Add(start);

            while (open.Count > 0)
            {
                // Keep a temporary list so open can be emptied and then
                // refreshed from scratch
                List<Cell> temp = new List<Cell>();
                for (int i = 0; i < open.Count; i++)
                {
                    closed.Add(open[i]);
                    for (int x = -1; x <= 1; x++)
                        for (int y = -1; y <= 1; y++)
                        {
                            Vector2Int frontier = open[i].Position;
                            frontier += new Vector2Int(x, y);
                            Cell frontierCell;

                            if (level.Contains(frontier))
                                frontierCell = level.GetCell(frontier);
                            else
                                continue;

                            if (closed.Contains(frontierCell))
                                continue;

                            if (frontierCell.Blocked || frontierCell.Actor != null)
                            {
                                closed.Add(frontierCell);
                                continue;
                            }

                            if (filled.Contains(frontierCell))
                            {
                                closed.Add(frontierCell);
                                continue;
                            }

                            filled.Add(frontierCell);

                            if (onFillPredicate.Invoke(frontierCell))
                            {
                                return filled;
                            }

                            temp.Add(frontierCell);
                        }
                }
                open.Clear();
                open.AddRange(temp);
                temp.Clear();
            }
            return filled;
        }

        /// <summary>
        /// Flood filler which runs a fallible callback i times.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="start"></param>
        /// <param name="iterateFillDelegate"></param>
        /// <param name="iterations"></param>
        /// <returns></returns>
        public static HashSet<Cell> FloodFill(Level level, Cell start,
            Func<Cell, bool> iterateFillDelegate, int iterations)
        {
            HashSet<Cell> filled = new HashSet<Cell>();
            List<Cell> open = new List<Cell>();
            HashSet<Cell> closed = new HashSet<Cell>();

            filled.Add(start);
            open.Add(start);

            int callbacksComplete = 0;

            while (open.Count > 0)
            {
                // Keep a temporary list so open can be emptied and then
                // refreshed from scratch
                List<Cell> temp = new List<Cell>();
                for (int i = 0; i < open.Count; i++)
                {
                    closed.Add(open[i]);
                    for (int x = -1; x <= 1; x++)
                        for (int y = -1; y <= 1; y++)
                        {
                            if (callbacksComplete == iterations)
                                return filled;

                            Vector2Int frontier = open[i].Position;
                            frontier += new Vector2Int(x, y);
                            Cell frontierCell;

                            if (level.Contains(frontier))
                                frontierCell = level.GetCell(frontier);
                            else
                                continue;

                            if (closed.Contains(frontierCell))
                                continue;

                            if (frontierCell.Blocked)
                            {
                                closed.Add(frontierCell);
                                continue;
                            }

                            if (filled.Contains(frontierCell))
                            {
                                closed.Add(frontierCell);
                                continue;
                            }

                            if (iterateFillDelegate.Invoke(frontierCell))
                                callbacksComplete++;

                            filled.Add(frontierCell);
                            temp.Add(frontierCell);
                        }
                }
                open.Clear();
                open.AddRange(temp);
                temp.Clear();
            }
            return filled;
        }

        
    }

    public sealed class Markov
    {
        // Credit to Peter Corbett
        public static readonly string[] baseNames = { "Almy", "Arkwright",
"Aronow",
"Astrand",
"Bornite",
"Brouwer",
"Brume",
"Chandalar",
"Chason",
"Cutler",
"Darcy",
"Delahunty",
"Devender",
"Dorn",
"Engber",
"Fenlason",
"Gambell",
"Grayling",
"Hafting",
"Haggerty",
"Hairston",
"Hamel",
"Henzell",
"Ignacy",
"Iwaarden",
"Kneller",
"Kompel",
"Larn",
"Laskin",
"Laufer",
"LeRoy",
"Lee",
"Lennan",
"Linhart",
"Linley",
"Lorber",
"Luick",
"Margulies",
"Martel",
"McGrath",
"Meluch",
"Menke",
"Modrall",
"Moria",
"Nystrom",
"Olson",
"Opstus",
"Oren",
"Palmer",
"Proudfoot",
"Rankin",
"Rupley",
"Rupley",
"Samon",
"Sapir",
"Scarmer",
"Seibert",
"Spackman",
"Stiker",
"Swanson",
"Thome",
"Threepoint",
"Wainwright",
"Walz",
"Waratah",
"Warwick",
"Willow",
"Yuval" };

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
            return l.Random(true);
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
