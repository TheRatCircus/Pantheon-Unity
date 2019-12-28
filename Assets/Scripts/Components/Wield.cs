// Wield.cs
// Jerome Martina

namespace Pantheon.Components
{
    [System.Serializable]
    public sealed class Wield : EntityComponent
    {
        public Entity[] Items { get; set; }

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

        public override EntityComponent Clone(bool full)
        {
            return new Wield(Items.Length);
        }
    }
}
