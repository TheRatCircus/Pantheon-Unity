// ConsoleCommandFunctions.cs
// Jerome Martina

using Pantheon.Components.Entity;
using Pantheon.Content;
using Pantheon.Core;
using Pantheon.World;
using System;
using UnityEngine;

namespace Pantheon.Debug
{
    // TODO: Rename to ConsoleFuncs
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
            Level level = ctrl.ActiveLevel;
            foreach (Vector2Int c in level.Map)
                level.Reveal(c.x, c.y);

            level.Draw(level.Map);
            
            return "Level revealed.";
        }

        public static string Spawn(string[] args, GameController ctrl)
        {
            // TODO: Protect player from spawning another PC
            EntityTemplate template = Assets.GetTemplate(args[0]);

            if (template == null)
                return $"No template named \"{args[0]}\" exists.";

            if (Array.Exists(template.Components, ec => { return ec is Actor; }))
            {
                Entity entity = Core.Spawn.SpawnActor(template,
                    ctrl.ActiveLevel, ctrl.Cursor.HoveredCell);
                ctrl.AssignGameObject(entity);
                return $"Spawned {entity} at {ctrl.Cursor.HoveredCell}.";
            }
            else
            {
                Entity entity = new Entity(template);
                entity.Move(ctrl.ActiveLevel, ctrl.Cursor.HoveredCell);
                Locator.Scheduler.RedrawDirtyCells(entity.Level);
                return $"Spawned {entity} at {ctrl.Cursor.HoveredCell}.";
            }
        }

        public static string Give(string[] args, GameController ctrl)
        {
            // TODO: Don't allow giving illegal entities
            Entity receiver = ctrl.ActiveLevel.ActorAt(ctrl.Cursor.HoveredCell);

            if (receiver == null)
                receiver = ctrl.PC;
            
            if (!receiver.TryGetComponent(out Inventory inv))
                return $"{receiver.Name} has no inventory.";
            else
            {
                EntityTemplate template = Assets.GetTemplate(args[0]);

                if (template == null)
                    return $"No item with ID \"{args[0]}\" exists.";

                Entity item = new Entity(template);
                inv.AddItem(item);
                item.Move(receiver.Level, receiver.Cell);
                return $"Gave {item.Name} to {receiver.Name}.";
            }
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
            Entity e = ctrl.ActiveLevel.ActorAt(ctrl.Cursor.HoveredCell);
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
            Entity e = ctrl.ActiveLevel.ActorAt(ctrl.Cursor.HoveredCell);
            if (e == null)
                return $"Nothing under the cursor to destroy.";
            e.Destroy(null);

            return $"Destroyed entity {e.ToString()}";
        }

        public static string ToggleIdolMode(string[] args, GameController ctrl)
        {
            Health health = ctrl.PC.GetComponent<Health>();

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

        public static string Teleport(string[] args, GameController ctrl)
        {
            Level level = ctrl.ActiveLevel;
            Vector2Int cell = ctrl.Cursor.HoveredCell;
            if (level.Walkable(cell))
            {
                ctrl.PC.Move(ctrl.PC.Level, cell);
                FOV.RefreshFOV(ctrl.PC.Level, ctrl.PC.Cell, true);
                return $"Teleported player to {cell}.";
            }
            else
                return "Targeted cell is not walkable.";
        }

        public static string Strategy(string[] args, GameController ctrl)
        {
            return "This command has not been implemented yet.";
        }

        public static string Relic(string[] args, GameController ctrl)
        {
            Entity relic = Gen.Relic.MakeRelic();
            relic.Move(ctrl.ActiveLevel, ctrl.Cursor.HoveredCell);
            FOV.RefreshFOV(ctrl.ActiveLevel, ctrl.PC.Cell, true);
            return $"Spawned {relic} at {ctrl.Cursor.HoveredCell}.";
        }

        public static string MakeIntoRelic(string[] args, GameController ctrl)
        {
            return "This command has not been implemented yet.";
        }

        public static string Enthrall(string[] args, GameController ctrl)
        {
            return $"This command is not implemented yet.";
        }

        public static string Vault(string[] args, GameController ctrl)
        {
            float rotation = 0f;

            if (args.Length == 2)
                float.TryParse(args[1], out rotation);

            if (!Assets.Vaults.ContainsKey(args[0]))
                return $"Vault {args[0]} does not exist.";

            string ret;
            
            if (!Gen.Vault.TryBuild(args[0],
                ctrl.ActiveLevel,
                ctrl.Cursor.HoveredCell, 
                rotation))
                ret = $"Failed to build vault {args[0]} at {ctrl.Cursor.HoveredCell}.";
            else
                ret = $"Successfully built vault {args[0]} at {ctrl.Cursor.HoveredCell}.";

            FOV.RefreshFOV(ctrl.ActiveLevel, ctrl.PC.Cell, true);
            return ret;
        }

        public static string Travel(string[] args, GameController ctrl)
        {
            return "This command has not been implemented yet.";
        }

        public static string KillLevel(string[] args, GameController ctrl)
        {
            // TODO: Iterate through level.Actors instead of cells
            Level lvl = ctrl.ActiveLevel;
            foreach (Vector2Int c in lvl.Map)
            {
                Entity entity = lvl.ActorAt(c);
                if (entity != null && !Actor.PlayerControlled(entity))
                    entity.Destroy(null);
            }
            ctrl.Scheduler.RedrawDirtyCells(lvl);
            return $"Killed all NPCs in {ctrl.ActiveLevel}.";
        }

        public static string Profession(string[] args, GameController ctrl)
        {
            Profession prof = Assets.GetProfession(args[0]);

            if (prof == null)
                return $"No profession named \"{args[0]}\" exists.";

            Entity entity = ctrl.ActiveLevel.ActorAt(ctrl.Cursor.HoveredCell);

            if (entity == null)
                return "Please mouseover an actor first.";

            if (!prof.Apply(entity))
                return "Failed to apply profession.";

            return $"Applied profession {prof.ID} to {entity.Name}.";
        }

        public static string RefreshFOV(string[] args, GameController ctrl)
        {
            FOV.RefreshFOV(ctrl.ActiveLevel, ctrl.PC.Cell, true);
            Locator.Scheduler.RedrawDirtyCells(ctrl.ActiveLevel);
            return "Refreshed FOV.";
        }
    }
}
