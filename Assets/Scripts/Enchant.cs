// Enchant.cs
// Jerome Martina

using Pantheon.Actors;
using Pantheon.Core;
using Pantheon.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace Pantheon
{
    public abstract class Enchant
    {
        public string Prefix { get; private set; }
        public string Suffix { get; private set; }

        // Guarantee that all inheritors assign a prefix and suffix
        public Enchant(string prefix, string suffix)
        {
            Prefix = prefix;
            Suffix = suffix;
        }

        public static readonly ReadOnlyCollection<EnchantBuy>
            _enchantCallbacks = new ReadOnlyCollection<EnchantBuy>(
                new List<EnchantBuy>()
            {
                new EnchantBuy(MakeEnchant<HealthEnchant>, 75),
                new EnchantBuy(MakeEnchant<ArmourEnchant>, 75),
                new EnchantBuy(MakeEnchant<EvasionEnchant>, 75),
                new EnchantBuy(MakeEnchant<SpellEnchant>, 100)
            });

        public static T MakeEnchant<T>() where T : Enchant, new()
        {
            return new T();
        }

        /// <summary>
        /// Enchant an item, with a chance at rolling a relic.
        /// </summary>
        /// <param name="item"></param>
        public static void EnchantItem(Item item)
        {
            bool relic = Game.PRNG.Next(0, 21) == 20;

            // Enchanting is based on a point buy system
            int points = relic ? 275 : 150;

            List<EnchantBuy> buys = _enchantCallbacks.ToList();
            buys.Shuffle(true);

            while (points > 0)
            {
                if (buys.Count == 0)
                {
                    UnityEngine.Debug.LogWarning("No more enchants.");
                    break;
                }

                EnchantBuy buy = buys[0];

                if (points >= buy.Cost)
                {
                    Enchant enchant = buy.Enchant.Invoke();
                    item.Enchants.Add(enchant);
                    points -= buy.Cost;
                }
                else
                    buys.RemoveAt(0);
            }

            if (relic)
                WorldGen.Relic.NameRelic(item);
            else
                item.DisplayName = $"magic {item.BaseName}";
        }

        public static void EnchantItem(Item item, bool relic)
        {
            // Enchanting is based on a point buy system
            int points = relic ? 275 : 150;

            List<EnchantBuy> buys = _enchantCallbacks.ToList();
            buys.Shuffle(true);

            while (points > 0)
            {
                if (buys.Count == 0)
                {
                    UnityEngine.Debug.LogWarning("No more enchants.");
                    break;
                }
                    
                EnchantBuy buy = buys[0];

                if (points >= buy.Cost)
                {
                    Enchant enchant = buy.Enchant.Invoke();
                    item.Enchants.Add(enchant);
                    points -= buy.Cost;
                }
                else
                    buys.RemoveAt(0);
            }

            if (relic)
                WorldGen.Relic.NameRelic(item);
            else
                item.DisplayName = $"magic {item.BaseName}";
        }
    }

    public struct EnchantBuy
    {
        public readonly Func<Enchant> Enchant;
        public readonly int Cost;

        public EnchantBuy(Func<Enchant> enchant, int cost)
        {
            Enchant = enchant;
            Cost = cost;
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

        public HealthEnchant() : base("Vital", "of Vitality")
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

        public override string ToString()
        {
            return $"{(Health > 0 ? "+" : "-")}{Health} maximum health";
        }
    }

    public sealed class ArmourEnchant : Enchant, IOnEquipEnchant
    {
        public int Armour { get; private set; }

        public ArmourEnchant() : base("Armoured", "of Guarding")
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

        public override string ToString()
        {
            return $"{(Armour > 0 ? "+" : "-")}{Armour} armour";
        }
    }

    public sealed class EvasionEnchant : Enchant, IOnEquipEnchant
    {
        public int Evasion { get; private set; }

        public EvasionEnchant() : base("Evasive", "of Dodging")
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

        public override string ToString()
        {
            return $"{(Evasion > 0 ? "+" : "-")}{Evasion} evasion";
        }
    }

    public sealed class SpellEnchant : Enchant, IOnEquipEnchant
    {
        public Spell Spell { get; private set; }

        public SpellEnchant() : base("Mage's", "of the Sorcerer")
        {
            Spell = Database.GetSpell(SpellID.PatsonsMagicBullet);
        }

        public void EquipEffect(Actor actor)
        {
            actor.Spells.Add(Spell);
        }

        public void UnequipEffect(Actor actor)
        {
            actor.Spells.Remove(Spell);
        }

        public override string ToString()
        {
            return $"Grants {Spell.DisplayName}";
        }
    }
}
