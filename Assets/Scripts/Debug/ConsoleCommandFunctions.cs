// ConsoleCommandFunctions.cs
// Jerome Martina

using System;
using Pantheon.Core;
using Pantheon.Actors;
using Pantheon.World;
using Pantheon.Utils;

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

            if (Traits.traitsLookup.TryGetValue(args[0], out Trait trait))
            {
                Game.GetPlayer().AddTrait(trait);
                return $"Added trait {trait}";
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
    }
}
