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
}
