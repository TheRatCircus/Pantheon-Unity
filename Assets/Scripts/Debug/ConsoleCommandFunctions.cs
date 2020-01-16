// ConsoleCommandFunctions.cs
// Jerome Martina

using Pantheon.Components;
using Pantheon.Content;
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
            {
                c.Revealed = true;
                ctrl.World.ActiveLevel.Draw(new[] { c });
            }
            return "Level revealed.";
        }

        public static string Spawn(string[] args, GameController ctrl)
        {
            if (!ctrl.Loader.AssetExists(args[0]))
                return $"No template named \"{args[0]}\" exists.";

            EntityTemplate template = ctrl.Loader.LoadTemplate(args[0]);
            if (Array.Exists(template.Components, ec => { return ec is Actor; }))
            {
                Entity entity = Core.Spawn.SpawnActor(template,
                    ctrl.World.ActiveLevel, ctrl.Cursor.HoveredCell);
                ctrl.AssignGameObject(entity);
                return $"Spawned {entity} at {ctrl.Cursor.HoveredCell}.";
            }
            else
            {
                Entity entity = new Entity(template);
                entity.Move(ctrl.World.ActiveLevel, ctrl.Cursor.HoveredCell);
                FOV.RefreshFOV(ctrl.World.ActiveLevel, ctrl.PC.Cell, true);
                return $"Spawned {entity} at {ctrl.Cursor.HoveredCell}.";
            }
        }

        public static string Give(string[] args, GameController ctrl)
        {
            Entity receiver = ctrl.Cursor.HoveredCell.Actor;
            
            if (!receiver.TryGetComponent(out Inventory inv))
                return $"{receiver.Name} has no inventory.";
            else
            {
                EntityTemplate template = ctrl.Loader.LoadTemplate(args[0]);
                Entity item = new Entity(template);
                item.Move(receiver.Level, receiver.Cell);
                inv.AddItem(item);
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
            Cell cell = ctrl.Cursor.HoveredCell;
            if (Cell.Walkable(cell))
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
            if (ctrl.Cursor.HoveredCell.Actor == null)
                return "No NPC in selected cell.";

            if (!ctrl.Cursor.HoveredCell.Actor.TryGetComponent(out AI ai))
                return "Entity in selected cell has no AI.";

            switch (args[0].ToLower())
            {
                case "default":
                    ai.Strategy = new DefaultStrategy();
                    return $"Changed strategy of {ai.Entity} to Default.";
                case "wander":
                    ai.Strategy = new WanderStrategy();
                    return $"Changed strategy of {ai.Entity} to Wander.";
                case "sleep":
                    ai.Strategy = new SleepStrategy();
                    return $"Changed strategy of {ai.Entity} to Sleep.";
                case "thrallfollow":
                    ai.Strategy = new ThrallFollowStrategy(ctrl.PC.GetComponent<Actor>());
                    return $"Changed strategy of {ai.Entity} to Thrall Follow.";
                default:
                    return $"Strategy {args[0]} does not exist.";
            }
        }

        public static string Relic(string[] args, GameController ctrl)
        {
            Entity relic = Gen.Relic.MakeRelic();
            relic.Move(ctrl.World.ActiveLevel, ctrl.Cursor.HoveredCell);
            FOV.RefreshFOV(ctrl.World.ActiveLevel, ctrl.PC.Cell, true);
            return $"Spawned {relic} at {ctrl.Cursor.HoveredCell}.";
        }

        public static string MakeIntoRelic(string[] args, GameController ctrl)
        {
            throw new NotImplementedException();

            //Inventory inv = ctrl.Player.GetComponent<Inventory>();
            //if (inv.Items.Count < 1)
            //    return "No items in inventory to make into relics.";

            //Entity item = inv.Items[0];
            
            //return $"Made relic: {item}.";
        }

        public static string Enthrall(string[] args, GameController ctrl)
        {
            if (ctrl.Cursor.HoveredCell.Actor == null)
                return "No NPC in selected cell.";

            if (!ctrl.Cursor.HoveredCell.Actor.TryGetComponent(out AI ai))
                return "Entity in selected cell has no AI.";

            ai.Strategy = new ThrallFollowStrategy(ctrl.PC.GetComponent<Actor>());
            return $"Changed strategy of {ai.Entity} to Thrall Follow.";
        }

        public static string Vault(string[] args, GameController ctrl)
        {
            if (!Locator.Loader.AssetExists(args[0]))
                return $"Vault {args[0]} does not exist.";

            string ret;
            
            if (!Gen.Vault.Build(args[0], ctrl.World.ActiveLevel,
                ctrl.Cursor.HoveredCell.Position))
                ret = $"Failed to build vault {args[0]} at {ctrl.Cursor.HoveredCell.Position}.";
            else
                ret = $"Successfully built vault {args[0]} at {ctrl.Cursor.HoveredCell.Position}.";

            FOV.RefreshFOV(ctrl.World.ActiveLevel, ctrl.PC.Cell, true);
            return ret;
        }

        public static string Travel(string[] args, GameController ctrl)
        {
            Level destination;
            switch (args[0].ToLower())
            {
                case "subterrane":
                    if (!ctrl.World.Levels.TryGetValue(
                        "sub_0_0_-2", out destination))
                    {
                        destination = ctrl.World.RequestLevel(new Vector3Int(0, 0, -2));
                    }
                    break;
                case "reformatory":
                    if (!ctrl.World.Levels.TryGetValue(
                        "reform_0_0_-1", out destination))
                    {
                        destination = ctrl.World.RequestLevel(new Vector3Int(0, 0, -1));
                    }
                    break;
                case "floodplain":
                    if (!ctrl.World.Levels.TryGetValue(
                        "floodplain_0_0_0", out destination))
                    {
                        destination = ctrl.World.RequestLevel(new Vector3Int(0, 0, 0));
                    }
                    break;
                default:
                    return $"Level of ID {args[0]} does not exist.";
            }

            if (destination == ctrl.PC.Level)
                return $"You're already at {destination}.";

            Level prev = ctrl.World.ActiveLevel;
            ctrl.PC.Move(destination, destination.RandomCell(true));
            ctrl.LoadLevel(destination, true);
            ctrl.UnloadLevel(prev);
            return $"Moved to {destination}.";
        }

        public static string KillLevel(string[] args, GameController ctrl)
        {
            foreach (Cell c in ctrl.World.ActiveLevel.Map)
            {
                if (c.Actor != null && !c.Actor.PlayerControlled)
                    c.Actor.Destroy(null);
            }
            FOV.RefreshFOV(ctrl.World.ActiveLevel, ctrl.PC.Cell, true);
            return $"Killed all NPCs in {ctrl.World.ActiveLevel}.";
        }
    }
}
