// Inventory.cs
// Jerome Martina

using Pantheon.World;
using System.Collections.Generic;

namespace Pantheon.Components
{
    [System.Serializable]
    public sealed class Inventory : EntityComponent
    {
        public int Size { get; set; } = 10;
        private List<Entity> items;

        [Newtonsoft.Json.JsonConstructor]
        public Inventory(int size)
        {
            Size = size;
            items = new List<Entity>(Size);
        }

        public Inventory(int size, List<Entity> items)
        {
            Size = size;
            this.items = items;
        }

        public void AddItem(Entity entity)
        {
            items.Add(entity);
        }

        /// <summary>
        /// Move entities in inventory when entity moves.
        /// </summary>
        public void Move(Level level, Cell cell)
        {
            foreach (Entity items in items)
                items.Move(level, cell);
        }

        public override EntityComponent Clone()
        {
            return new Inventory(Size);
        }
    }
}
