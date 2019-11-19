// ConsoleCommandFunctions.cs
// Jerome Martina

using Pantheon.Core;
using Pantheon.ECS;
using Pantheon.ECS.Components;
using Pantheon.ECS.Templates;
using Pantheon.World;
using System;
using UnityEngine;

namespace Pantheon.Debug
{
    public static class ConsoleCommandFunctions
    {
        public static string ListLayers(string[] args, GameController ctrl)
        {
            string ret = "";
            foreach (Layer layer in ctrl.World.Layers.Values)
                ret += $"layer {layer.ZLevel} {Environment.NewLine}";
            return ret;
        }

        public static string ListLevels(string[] args, GameController ctrl)
        {
            string ret = "";
            foreach (Layer layer in ctrl.World.Layers.Values)
                foreach (Level level in layer.Levels.Values)
                    ret += $"{level.ID}";
            return ret;
        }

        public static string WhereAmI(string[] args, GameController ctrl)
        {
            Position playerPos = ctrl.Manager.Player.GetComponent<Position>();
            return playerPos.ToString();
        }

        public static string DescribeCell(string[] args, GameController ctrl)
        {
            Cell c = ctrl.Cursor.HoveredCell;
            string ret = "";
            foreach (Entity e in c.Entities)
                ret += $"{e}{Environment.NewLine}";
            ret += $"Tile: {c.Terrain}";
            return ret;
        }

        public static string FindEntities(string[] args, GameController ctrl)
        {
            if (args.Length < 1)
                return "Please supply a search key.";

            string key = string.Join(" ", args);
            string ret = "";
            foreach (Entity e in ctrl.Manager.Entities.Values)
                if (e.Name.Contains(key))
                    ret += $"{e.Name} ({e.GUID}){Environment.NewLine}";

            if (ret == "")
                return $"Nothing found.";

            return ret;
        }

        public static string DescribeEntity(string[] args, GameController ctrl)
        {
            if (args.Length != 1)
                return "Please supply exactly 1 argument.";

            int guid = int.Parse(args[0]);
            if (ctrl.Manager.Entities.TryGetValue(guid, out Entity e))
            {
                string ret = $"{e.Name} ({e.GUID}){Environment.NewLine}";
                foreach (BaseComponent bc in e.Components.Values)
                    ret += bc.ToString() + Environment.NewLine;
                return ret;
            }
            else
                return "GUID not found.";
        }

        public static string LoadedAssets(string[] args, GameController ctrl)
        {
            string ret = "";

            foreach (AssetBundle bundle in AssetBundle.GetAllLoadedAssetBundles())
                foreach (string name in bundle.GetAllAssetNames())
                    ret += $"{name}{Environment.NewLine}";

            return ret;
        }

        public static string Spawn(string[] args, GameController ctrl)
        {
            Template template = ctrl.Loader.Load<Template>(args[0]);
            if (template == null)
                return $"{args[0]} not found.";

            ctrl.EntityFactory.NewEntityAt(template, ctrl.World.ActiveLevel, 
                ctrl.Cursor.HoveredCell);

            return $"Spawned {template.name}";
        }

        public static string RevealLevel(string[] args, GameController ctrl)
        {
            foreach (Cell c in ctrl.World.ActiveLevel.Map.Values)
            {
                c.Revealed = true;
                ctrl.World.ActiveLevel.VisualizeTile(c);
            }

            return "Level revealed.";
        }
    }
}
