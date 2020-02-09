// Wield.cs
// Jerome Martina

using Pantheon.Utils;
using Pantheon.World;
using System;
using System.Runtime.Serialization;

namespace Pantheon.Components.Entity
{
    using Entity = Pantheon.Entity;

    [Serializable]
    public sealed class Wield : EntityComponent
    {
        public Entity[] Items { get; set; }
        public bool Wielding => Items.HasElements();

        public event Action<Entity[]> WieldChangeEvent;

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
            WieldChangeEvent?.Invoke(Items);
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

        public void Move(Level level, Cell cell)
        {
            foreach (Entity item in Items)
                item?.Move(level, cell);
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext ctxt)
        {
            Helpers.ClearNonSerializableDelegates(ref WieldChangeEvent);
        }

        public override EntityComponent Clone(bool full)
        {
            return new Wield(Items.Length);
        }
    }
}
