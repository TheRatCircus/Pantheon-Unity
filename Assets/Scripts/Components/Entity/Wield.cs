// Wield.cs
// Jerome Martina

using Pantheon.Utils;

namespace Pantheon.Components.Entity
{
    using Entity = Pantheon.Entity;

    [System.Serializable]
    public sealed class Wield : EntityComponent
    {
        public Entity[] Items { get; set; }
        public bool Wielding => Items.HasElements();

        public Wield(int max) => Items = new Entity[max];

        /// <summary>
        /// Attempt to wield an item.
        /// </summary>
        /// <param name="item">The item to wield.</param>
        /// <returns>Whether the item was successfully wielded.</returns>
        public bool TryWield(Entity item, out Entity unwielded)
        {
            unwielded = Items[0];
            Items[0] = item;
            // TODO: Evaluate wieldability based on size, weight
            return true;
        }

        public Entity[] GetEvocables()
        {
            Entity[] evocables = new Entity[Items.Length];
            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i] != null && Items[i].HasComponent<Evocable>())
                    evocables[i] = Items[i];
            }
            return evocables.Compress();
        }

        public override EntityComponent Clone(bool full)
        {
            return new Wield(Items.Length);
        }
    }
}
