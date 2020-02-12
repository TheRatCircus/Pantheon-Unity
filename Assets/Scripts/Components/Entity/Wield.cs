// Wield.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon.Utils;
using Pantheon.World;
using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace Pantheon.Components.Entity
{
    using Entity = Pantheon.Entity;

    [Serializable]
    public sealed class Wield : EntityComponent
    {
        public Entity[] Items { get; private set; }
        [JsonIgnore] public bool Wielding => Items.HasElements();

        public event Action<Entity[]> WieldChangeEvent;

        [JsonConstructor]
        public Wield(Entity[] items) => Items = items;

        public Wield(int max) => Items = new Entity[max];

        /// <summary>
        /// Attempt to wield an item.
        /// </summary>
        /// <param name="item">The item to wield.</param>
        /// <returns>Whether the item was successfully wielded.</returns>
        public bool TryWield(Entity item, out Entity unwielded)
        {
            if (Items.Length < 1)
            {
                if (Actor.PlayerControlled(Entity))
                    Locator.Log.Send(
                        $"You have no way of wielding that.",
                        Color.yellow);
                unwielded = null;
                return false;
            }

            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i] == null)
                {
                    // Empty slot found, wield in it
                    unwielded = null;
                    Items[i] = item;
                    item.Wielded = true;
                    WieldChangeEvent?.Invoke(Items);
                    return true;
                }
            }
            // For now, just replace item in first slot
            unwielded = Items[0];
            unwielded.Wielded = false;
            Items[0] = item;
            item.Wielded = true;
            // TODO: Multihanding
            // TODO: Evaluate wieldability based on size, weight
            WieldChangeEvent?.Invoke(Items);
            return true;
        }

        /// <summary>
        /// Wield as many items as possible.
        /// </summary>
        public void ForceWield(Entity[] items)
        {
            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i] != null) // Unwield previous
                    Items[i].Wielded = false;
                Items[i] = items[i];
                items[i].Wielded = true;
            }

            WieldChangeEvent?.Invoke(Items);
        }

        public Entity[] GetEvocables()
        {
            // TODO: Collect with List, convert to array
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
