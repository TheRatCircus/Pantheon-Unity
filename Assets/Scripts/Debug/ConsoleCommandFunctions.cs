// ConsoleCommandFunctions.cs
// Jerome Martina

using System;
using Pantheon.Core;
using Pantheon.Actors;
using Pantheon.World;
using Pantheon.Utils;
using Pantheon.WorldGen;

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

            if (Enum.TryParse(args[0], out WeaponType weaponType))
            {
                Game.GetPlayer().Inventory.Add(ItemFactory.NewWeapon(weaponType));
                Game.GetPlayer().RaiseInventoryChangeEvent();
                return $"Giving {weaponType.ToString()}";
            }
            else if (Enum.TryParse(args[0], out FlaskType flaskType))
            {
                Game.GetPlayer().Inventory.Add(ItemFactory.NewFlask(flaskType));
                Game.GetPlayer().RaiseInventoryChangeEvent();
                return $"Giving {flaskType.ToString()}";
            }
            else if (Enum.TryParse(args[0], out ScrollType scrollType))
            {
                Game.GetPlayer().Inventory.Add(ItemFactory.NewScroll(scrollType));
                Game.GetPlayer().RaiseInventoryChangeEvent();
                return $"Giving {scrollType.ToString()}";
            }
            else if (Enum.TryParse(args[0], out AmmoType ammoType))
            {
                Game.GetPlayer().Inventory.Add(ItemFactory.NewAmmo(ammoType));
                Game.GetPlayer().RaiseInventoryChangeEvent();
                return $"Giving {ammoType.ToString()}";
            }
            else
                return $"Item of type {args[0]} could not be found";
        }

        public static string LearnSpell(string[] args)
        {
            if (args.Length != 1)
                return "Please pass only 1 argument.";

            if (Enum.TryParse(args[0], out SpellType spellType))
            {
                Game.GetPlayer().Spells.Add(Database.GetSpell(spellType));
                return $"You have learned {spellType.ToString()}";
            }
            else
                return $"Spell of type {args[0]} could not be found";
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

            for (int i = Game.instance.activeLevel.NPCs.Count - 1; i >= 0; i--)
            {
                Game.instance.activeLevel.NPCs[i].OnDeath();
                npcsKilled++;
            }

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

        public static string OpenDomain(string[] args)
        {
            if (args.Length != 1)
                return "Please pass only 1 argument.";

            Connection domainPortal = new PortalConnection(
                Game.instance.activeLevel,
                Game.GetPlayer().Cell,
                Database.GetFeature(FeatureType.Portal),
                LevelZones.GenerateDomainAtrium);
            domainPortal.DisplayName = "a portal to IDOL_NAME's Domain";
            domainPortal.OneWay = true;

            Game.GetPlayer().Cell.Connection = domainPortal;
            Game.instance.activeLevel.Connections.Add("IDOL_NAMEDomainPortal",
                domainPortal);

            return "Opened a portal to IDOL_NAME's Domain.";
        }

        public static string ListReligions(string[] args)
        {
            if (args.Length != 0)
                return "This command takes no arguments.";

            string ret = "";

            foreach (System.Collections.Generic.KeyValuePair<string, Faction>
                pair in Game.instance.Religions)
            {
                Faction faction = pair.Value;
                ret += $"{faction.RefName} ({faction.DisplayName}){Environment.NewLine}";
            }

            return ret;
        }

        public static string JoinReligion(string[] args)
        {
            if (args.Length != 1)
                return "Please pass only 1 argument.";

            Game.instance.Religions.TryGetValue(args[0], out Faction religion);
            Game.GetPlayer().Faction = religion;

            return $"You are now a member of {religion.DisplayName}.";
        }
    }
}
