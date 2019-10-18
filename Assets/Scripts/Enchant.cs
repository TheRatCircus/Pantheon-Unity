// Enchant.cs
// Jerome Martina

using Pantheon.Actors;
using Pantheon.Core;
using System.Collections.Generic;

namespace Pantheon
{
    public abstract class Enchant
    {
        public static readonly List<System.Func<Enchant>> _enchantCallbacks =
            new List<System.Func<Enchant>>()
        {
            MakeEnchant<HealthEnchant>,
            MakeEnchant<ArmourEnchant>,
            MakeEnchant<EvasionEnchant>,
            MakeEnchant<SpellEnchant>
        };

        public static T MakeEnchant<T>() where T : new()
        {
            return new T();
        }
    }

    /// <summary>
    /// For enchants which take effect when the item is equipped.
    /// </summary>
    public interface IOnEquipEnchant
    {
        void EquipEffect(Actor actor);
        void UnequipEffect(Actor actor);
    }

    public sealed class HealthEnchant : Enchant, IOnEquipEnchant
    {
        public int Health { get; private set; }

        public HealthEnchant()
        {
            Health = Game.PRNG.Next(3, 9);
        }

        public void EquipEffect(Actor actor)
        {
            actor.MaxHealth += Health;
        }

        public void UnequipEffect(Actor actor)
        {
            actor.MaxHealth -= Health;
            actor.ChangeHealth(-Health, false);
        }
    }

    public sealed class ArmourEnchant : Enchant, IOnEquipEnchant
    {
        public int Armour { get; private set; }

        public ArmourEnchant()
        {
            Armour = Game.PRNG.Next(2, 5);
        }

        public void EquipEffect(Actor actor)
        {
            actor.Defenses.Armour += Armour;
        }

        public void UnequipEffect(Actor actor)
        {
            actor.Defenses.Armour -= Armour;
        }
    }

    public sealed class EvasionEnchant : Enchant, IOnEquipEnchant
    {
        public int Evasion { get; private set; }

        public EvasionEnchant()
        {
            Evasion = Game.PRNG.Next(2, 5);
        }

        public void EquipEffect(Actor actor)
        {
            actor.Defenses.Evasion += Evasion;
        }

        public void UnequipEffect(Actor actor)
        {
            actor.Defenses.Evasion -= Evasion;
        }
    }

    public sealed class SpellEnchant : Enchant, IOnEquipEnchant
    {
        public Spell Spell { get; private set; }

        public SpellEnchant()
        {
            Spell = Database.GetSpell(SpellType.PatsonsMagicBullet);
        }

        public void EquipEffect(Actor actor)
        {
            actor.Spells.Add(Spell);
        }

        public void UnequipEffect(Actor actor)
        {
            actor.Spells.Remove(Spell);
        }
    }
}
