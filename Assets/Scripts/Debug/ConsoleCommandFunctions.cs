// ConsoleCommandFunctions.cs
// Jerome Martina

using Pantheon.Actors;
using Pantheon.Core;
using Pantheon.Utils;
using Pantheon.World;
using System;
using System.Collections.Generic;

namespace Pantheon.Debug
{
    public static class ConsoleCommandFunctions
    {
        public static string RevealLevel(string[] args)
        {
            foreach (Cell cell in Game.instance.activeLevel.Map)
                cell.Reveal();

            Game.instance.activeLevel.RefreshFOV();
            CellDrawer.DrawLevel(Game.instance.activeLevel);

            return "Revealing level...";
        }

        public static string ApplyStatus(string[] args)
        {
            if (args.Length != 1)
                return "Please only pass 1 argument.";

            StatusType statusType
                = (StatusType)Enum.Parse(typeof(StatusType), args[0]);

            StatusEffect status = StatusFactory.GetStatus(statusType);

            Game.GetPlayer().ApplyStatus(status);

            return $"Status effect {status.DisplayName} applied to player.";
        }

        public static string GiveItem(string[] args)
        {
            if (args.Length != 1)
                return "Please pass only 1 argument.";

            if (Database.Contains(args[0]))
            {
                ItemDef item = Database.Get<ItemDef>(args[0]);
                Game.GetPlayer().AddItem(new Item(item));
                return $"Giving {item.DisplayName}...";
            }
            else
                return $"{args[0]} not found.";
        }

        public static string LearnSpell(string[] args)
        {
            if (args.Length != 1)
                return "Please pass only 1 argument.";

            if (Database.Contains(args[0]))
            {
                Spell spell = Database.Get<Spell>(args[0]);
                Game.GetPlayer().Spells.Add(spell);
                return $"You have learned {spell.DisplayName}.";
            }
            else
                return $"{args[0]} not found.";
        }

        public static string AddTrait(string[] args)
        {
            if (args.Length != 1)
                return "Please pass only 1 argument.";

            if (Enum.TryParse(args[0], out TraitRef traitRef))
            {
                if (Traits._traits.TryGetValue(traitRef, out Trait trait))
                {
                    Game.GetPlayer().AddTrait(trait);
                    return $"Added trait {trait}";
                }
                else return $"ERROR: Trait {traitRef.ToString()} is null.";
            }
            else
                return $"Could not find trait {args[0]}";
        }

        public static string KillLevel(string[] args)
        {
            if (args.Length != 0)
                return "This command takes no arguments.";

            int npcsKilled = 0;

            for (int i = Game.instance.activeLevel.NPCs.Count - 1;
                i >= 0;
                i--)
            {
                Game.instance.activeLevel.NPCs[i].OnDeath(null);
                npcsKilled++;
            }

            Game.instance.activeLevel.RefreshFOV();
            return $"Killed {npcsKilled} NPCs.";
        }

        public static string IdolMode(string[] args)
        {
            if (args.Length != 0)
                return "This command takes no arguments.";

            Game.instance.IdolMode = !Game.instance.IdolMode;

            if (Game.instance.IdolMode)
                return "Idolmode enabled.";
            else
                return "Idolmode disabled.";
        }

        public static string OpenSanctum(string[] args)
        {
            if (args.Length != 1)
                return "Please pass only 1 argument.";

            string idolRef = args[0].ToLower();

            if (!Game.Pantheon.Idols.TryGetValue(idolRef,
                out Idol idol))
                return "That idol was not found.";

            Connection sanctumPortal = new Connection(
                Game.instance.activeLevel,
                Game.GetPlayer().Cell,
                ID.Feature._portal,
                $"sanctum_{idolRef}_0");
            sanctumPortal.OneWay = true;
            sanctumPortal.DisplayName = $"a portal to {idol.DisplayName}'s" +
                $" sanctum";

            Game.GetPlayer().Cell.Connection = sanctumPortal;

            return $"Opened a portal to {idol.DisplayName}'s sanctum.";
        }

        public static string ListIdols(string[] args)
        {
            if (args.Length != 0)
                return "This command takes no arguments.";

            string ret = "";

            foreach (Idol idol in Game.Pantheon.Idols.Values)
                ret += $"{idol} {Environment.NewLine}";

            return ret;
        }

        public static string ListReligions(string[] args)
        {
            if (args.Length != 0)
                return "This command takes no arguments.";

            string ret = "";

            foreach (KeyValuePair<Idol, Faction>
                pair in Game.instance.Religions)
            {
                Faction faction = pair.Value;
                ret += $"{faction.RefName} ({faction.DisplayName})" +
                    $"{Environment.NewLine}";
            }

            return ret;
        }

        public static string JoinReligion(string[] args)
        {
            if (args.Length != 1)
                return "Please pass only 1 argument.";

            if (Game.Pantheon.Idols.TryGetValue(args[0],
                out Idol idol))
                return $"Idol {args[0]} does not exist.";

            Game.instance.Religions.TryGetValue(idol, out Faction religion);
            Game.GetPlayer().Faction = religion;

            return $"You are now a member of {religion.DisplayName}.";
        }

        public static string ListLevels(string[] args)
        {
            if (args.Length != 0)
                return "This command takes no arguments.";

            string ret = "";

            foreach (Level level in Game.instance.Levels.Values)
                ret += $"{level.RefName} ({level.DisplayName})" +
                    $"{Environment.NewLine}";

            return ret;
        }

        public static string WhereAmI(string[] args)
        {
            if (args.Length != 0)
                return "This command takes no arguments.";

            return $"{Game.instance.activeLevel.RefName} " +
                $"({Game.instance.activeLevel.LayerPos})";
        }

        public static string AddTraitPoints(string[] args)
        {
            if (args.Length < 1)
                return "Please enter a number of trait points to add.";

            if (args.Length > 1)
                return "Please pass only 1 argument.";

            int toAdd = Math.Min(int.Parse(args[0]), 64);
            Game.GetPlayer().ChangeTraitPoints(toAdd);
            return $"Added {toAdd} trait points.";
        }

        public static string SeeAll(string[] args)
        {
            if (args.Length != 0)
                return "This command takes no arguments.";

            foreach (Cell c in Game.instance.activeLevel.Map)
            {
                c.SetVisibility(true, -1);
                CellDrawer.DrawCell(Game.instance.activeLevel, c);
                if (c.Actor != null && c.Actor is NPC npc)
                    npc.UpdateVisibility();
            }

            return $"All cells in level made visible.";
        }

        public static string LevelUp(string[] args)
        {
            if (args.Length != 0)
                return "This command takes no arguments.";

            Player player = Game.GetPlayer();
            player.LevelUp();
            player.XP = player.XPToLevel(player.ExpLevel);

            return $"Levelled up the player to {player.ExpLevel}.";
        }

        public static string Relic(string[] args)
        {
            if (args.Length != 1)
                return "Please supply an item base type.";

            ItemDef item;
            if (Database.Contains(args[0]))
                item = Database.Get<ItemDef>(args[0]);
            else
                return $"{args[0]} not found.";

            Item relic = new Item(item);
            Enchant.EnchantItem(relic, true);
            
            Game.GetPlayer().Cell.Items.Add(relic);
            Game.instance.activeLevel.RefreshFOV();

            return $"Relic {relic.DisplayName} created.";
        }
    }
}
