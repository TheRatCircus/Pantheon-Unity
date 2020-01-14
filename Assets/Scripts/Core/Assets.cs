using Pantheon.Content;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using UnityEngine;

namespace Pantheon.Core
{
    public static class Assets
    {
        [RuntimeInitializeOnLoadMethod]
        private static void OnStartup()
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(Path.Combine(
                Application.streamingAssetsPath, "pantheon"));
            System.Diagnostics.Debug.Assert(bundle != null);
            Object[] objs = bundle.LoadAllAssets();

            int t = 1; // 0 represents no terrain
            foreach (Object obj in objs)
            {
                if (obj is TerrainDefinition td)
                {
                    TerrainIDs[t] = td.name;
                    Terrains[t++] = td;
                }
            }
        }

        public static string[] TerrainIDs { get; private set; }
            = new string[8];
        public static TerrainDefinition[] Terrains { get; private set; }
            = new TerrainDefinition[8];

        public static TerrainDefinition GetTerrain(string id)
        {
            int i = 0;
            foreach (string s in TerrainIDs)
            {
                if (TerrainIDs[i] == id)
                    break;

                i++;
            }
            return Terrains[i];
        }

        public static TerrainDefinition GetTerrain(byte index)
        {
            return Terrains[index];
        }
    }
}
