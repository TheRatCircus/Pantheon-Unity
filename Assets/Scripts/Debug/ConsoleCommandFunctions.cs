// ConsoleCommandFunctions.cs
// Jerome Martina

using Pantheon.Commands;
using Pantheon.Components;
using Pantheon.Core;
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

            return "Level revealed.";
        }

        public static string Spawn(string[] args, GameController ctrl)
        {
            EntityTemplate template = ctrl.Loader.LoadTemplate(args[0]);
            if (Array.Exists(template.Components, ec => { return ec is Actor; }))
            {
                Entity e = Core.Spawn.SpawnActor(template,
                    ctrl.World.ActiveLevel, ctrl.Cursor.HoveredCell);
                Core.Spawn.AssignGameObject(e);
            }                
            else
                throw new NotImplementedException();

            return $"Spawned {template.ID} at {ctrl.Cursor.HoveredCell}.";
        }

        public static string TurnOrder(string[] args, GameController ctrl)
        {
            string ret = "";

            foreach (Actor actor in ctrl.Scheduler.Queue)
                ret += $"{actor}{Environment.NewLine}";

            return ret;
        }

        public static string DescribeComponent(string[] args, GameController ctrl)
        {
            Entity e = ctrl.Cursor.HoveredCell.Actor;
            switch (args[0].ToLower())
            {
                case "actor":
                    return e.GetComponent<Actor>().ToString();
                case "ai":
                    return e.GetComponent<AI>().ToString();
                case "health":
                    return e.GetComponent<Health>().ToString();
                case "species":
                    return e.GetComponent<Species>().ToString();
                default:
                    return $"Component of type \"{args[0]}\" not found.";
            }
        }

        public static string Destroy(string[] args, GameController ctrl)
        {
            Entity e = ctrl.Cursor.HoveredCell.Actor;
            if (e == null)
                return $"Nothing under the cursor to destroy.";
            e.Destroy(null);

            return $"Destroyed entity {e.ToString()}";
        }

        public static string Explode(string[] args, GameController ctrl)
        {
            Entity e = ctrl.Cursor.HoveredCell.Actor;
            GameObject explPrefab = ctrl.Loader.Load<GameObject>("FX_HandGrenade");
            ExplodeCommand cmd = new ExplodeCommand(e, explPrefab, ExplosionPattern.Point);
            cmd.Execute();
            return $"Blew up {e.Name}.";
        }

        public static string ToggleIdolMode(string[] args, GameController ctrl)
        {
            Health health = ctrl.Player.GetComponent<Health>();

            if (!health.Invincible)
            {
                health.Invincible = true;
                return $"Idol mode enabled; you are indestructible.";
            }
            else
            {
                health.Invincible = false;
                return $"Idol mode disabled; you are mortal again.";
            }
        }

        public static string FragWand(string[] args, GameController ctrl)
        {
            GameObject fxPrefab = ctrl.Loader.Load<GameObject>("FX_HandGrenade");
            ExplodeCommand expl = new ExplodeCommand(ctrl.Player, fxPrefab, ExplosionPattern.Point);
            PointEffectCommand pec = new PointEffectCommand(ctrl.Player, expl);
            OnUse onUse = new OnUse(TurnScheduler.TurnTime, pec);
            Entity entity = new Entity(onUse);
            entity.Name = "Wand of Fragmentation";
            entity.Move(ctrl.World.ActiveLevel, ctrl.Player.Cell);
            return "Spawned the Wand of Fragmentation.";
        }
    }
}
