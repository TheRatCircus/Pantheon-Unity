// ConsoleCommandFunctions.cs
// Jerome Martina

using Pantheon.Core;
using Pantheon.ECS;
using Pantheon.ECS.Components;
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

        public static string LoadedAssets(string[] args, GameController ctrl)
        {
            string ret = "";

            foreach (AssetBundle bundle in AssetBundle.GetAllLoadedAssetBundles())
                foreach (string name in bundle.GetAllAssetNames())
                    ret += $"{name}{Environment.NewLine}";

            return ret;
        }

        public static string RevealLevel(string[] args, GameController ctrl)
        {
            foreach (Cell c in ctrl.World.ActiveLevel.Map)
                c.Revealed = true;

            FOV.RefreshFOV(ctrl.World.ActiveLevel,
                ctrl.EntityManager.CellOfPlayer().Position,
                true);

            return "Level revealed.";
        }

        public static string Spawn(string[] args, GameController ctrl)
        {
            EntityTemplate template = ctrl.Loader.LoadTemplate(args[0]);
            Entity entity = ctrl.EntityManager.NewEntity(
                template, ctrl.World.ActiveLevel, ctrl.Cursor.HoveredCell);
            ctrl.AssignGameObject(entity);
            FOV.RefreshFOV(ctrl.World.ActiveLevel,
                ctrl.EntityManager.CellOfPlayer().Position,true);
            return $"Spawned {template.ID} at {ctrl.Cursor.HoveredCell}.";
        }

        public static string TurnOrder(string[] args, GameController ctrl)
        {
            string ret = "";

            foreach (Actor actor in ctrl.Scheduler.Queue)
                ret += $"{actor}{Environment.NewLine}";

            return ret;
        }

        public static string FindEntity(string[] args, GameController ctrl)
        {
            int guid = int.Parse(args[0]);
            Entity entity = ctrl.EntityManager.Entities[guid];
            return entity.ToString();
        }

        public static string ComponentsOf(string[] args, GameController ctrl)
        {
            string ret = "";

            int guid = int.Parse(args[0]);
            guid = Mathf.Clamp(guid, 0, int.MaxValue);

            if (ctrl.EntityManager.Actors[guid] != null)
                ret += ctrl.EntityManager.Actors[guid].ToString() + Environment.NewLine;
            if (ctrl.EntityManager.AI[guid] != null)
                ret += ctrl.EntityManager.AI[guid].ToString() + Environment.NewLine;

            return ret;
        }

        public static string EntitiesHere(string[] args, GameController ctrl)
        {
            return ctrl.Cursor.HoveredCell.Actor?.ToString();
        }
    }
}
