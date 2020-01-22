// Inventory.cs
// Jerome Martina

using Pantheon.World;
using System.Collections.Generic;

namespace Pantheon.Components.Entity
{
    using Entity = Pantheon.Entity;

    [System.Serializable]
    public sealed class Inventory : EntityComponent
    {
        public int Size { get; set; } = 10;
        public List<Entity> Items { get; private set; }

        public bool Empty => Items == null || Items.Count < 1;

        [Newtonsoft.Json.JsonConstructor]
        public Inventory(int size)
        {
            Size = size;
            Items = new List<Entity>(Size);
        }

        public Inventory(int size, List<Entity> items)
        {
            Size = size;
            Items = items;
        }

        public void AddItem(Entity entity)
        {
            entity.InInventory = true;
            Items.Add(entity);
        }

        public void RemoveItem(Entity entity)
        {
            entity.InInventory = false;
            Items.Remove(entity);
        }

        /// <summary>
        /// Move entities in inventory when entity moves.
        /// </summary>
        public void Move(Level level, Cell cell)
        {
            foreach (Entity items in Items)
                items.Move(level, cell);
        }

        public override EntityComponent Clone(bool full)
        {
            if (full)
                return new Inventory(Size, Items);
            else
                return new Inventory(Size);
        }
    }
}
