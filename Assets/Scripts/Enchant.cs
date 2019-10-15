// Enchant.cs
// Jerome Martina

using Pantheon.Actors;

namespace Pantheon
{
    public abstract class Enchant
    {
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
        public int MaxHealth { get; private set; }

        public HealthEnchant(int health)
        {
            MaxHealth = health;
        }

        public void EquipEffect(Actor actor)
        {
            actor.MaxHealth += MaxHealth;
        }

        public void UnequipEffect(Actor actor)
        {
            actor.MaxHealth -= MaxHealth;
            actor.ChangeHealth(-MaxHealth, false);
        }
    }

    public sealed class ArmourEnchant : Enchant, IOnEquipEnchant
    {
        public int Armour { get; private set; }

        public ArmourEnchant(int armour)
        {
            Armour = armour;
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

        public EvasionEnchant(int evasion)
        {
            Evasion = evasion;
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
}
