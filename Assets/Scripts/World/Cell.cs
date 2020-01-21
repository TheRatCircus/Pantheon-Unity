// Cell.cs
// Jerome Martina

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Pantheon.Components;
using Pantheon.Content;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.World
{
    [Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CellFlag : byte
    {
        Visible = 1 << 0,
        Revealed = 1 << 1
    }

    [Serializable]
    public sealed class Cell
    {
        // The offset of each tile from Unity's true grid coords
        public const float TileOffsetX = .5f;
        public const float TileOffsetY = .5f;

        public byte X { get; private set; }
        public byte Y { get; private set; }
        public Vector2Int Position
        {
            get => new Vector2Int(X, Y);
            private set
            {
                if (value.x > byte.MaxValue || value.y > byte.MaxValue)
                    throw new ArgumentException(
                        $"Pos. {value.x}, {value.y} exceeds cells position bounds.");

                X = (byte)value.x;
                Y = (byte)value.y;
            }
        }
        public CellFlag Flags { get; set; }

        public bool Visible
        {
            get => Flags.HasFlag(CellFlag.Visible);
            set
            {
                if (value)
                    Flags |= CellFlag.Visible;
                else
                    Flags &= ~CellFlag.Visible;
            }
        }
        public bool Revealed
        {
            get => Flags.HasFlag(CellFlag.Revealed);
            set
            {
                if (value)
                    Flags |= CellFlag.Revealed;
                else
                    Flags &= ~CellFlag.Revealed;
            }
        }

        public TerrainDefinition Ground { get; set; }
        public TerrainDefinition Wall { get; set; }

        public bool Opaque => Wall != null && Wall.Opaque;
        public bool Blocked
        {
            get
            {
                if (Actor == null) return Wall != null;
                else return true;
            }
        }

        public Entity Actor { get; set; }
        private List<Entity> items;
        public bool HasItems
        {
            get
            {
                if (items == null || items.Count < 1)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Whether a cell can be walked on at all (does not factor entities).
        /// </summary>
        /// <param name="cell"></param>
        /// <returns>True if cell exists, has ground, and is not blocked.</returns>
        public static bool Walkable(Cell cell)
            => cell != null && cell.Ground != null && cell.Wall == null;

        public Cell(Vector2Int position) => Position = position;

        /// <returns>True if a change in visibility actually occurred.</returns>
        public bool SetVisibility(bool visible, int fallOff)
        {
            bool noChange;
            if (!visible)
            {
                noChange = !Visible || !Revealed;
                Visible = false;
            }
            else
            {
                
                if (fallOff > 100)
                {
                    noChange = !Visible || !Revealed;
                    Visible = false;
                }
                else
                {
                    noChange = Visible;
                    Visible = true;
                    Revealed = true;
                }
            }
            return !noChange;
        }

        /// <summary>
        /// Determine where to store a new entity based on its archetype.
        /// </summary>
        /// <param name="entity"></param>
        public void AllocateEntity(Entity entity)
        {
            // Don't store the entity if it's in an inventory
            if (entity.InInventory)
                return;

            if (entity.HasComponent<Actor>())
            {
                if (Actor != null)
                    throw new Exception(
                        "Cannot allocate 2 actors to one cell.");
                else
                    Actor = entity;
                return;
            }

            // Else, assume entity is an item
            if (items == null)
                items = new List<Entity>();
            items.Add(entity);
        }

        /// <summary>
        /// Remove an entity from storage based on its archetype.
        /// </summary>
        /// <param name="entity"></param>
        public void DeallocateEntity(Entity entity)
        {
            if (entity == Actor)
                Actor = null;
            else
            {
                items.Remove(entity);
                if (items.Count == 0)
                    items = null;
            }
        }

        public bool TryGetItem(int index, out Entity item)
        {
            if (!HasItems || items.Count < index)
            {
                item = null;
                return false;
            }
            else
            {
                item = items[index];
                return true;
            }
        }

        public bool HasItem(Entity item) => items.Contains(item);

        public override string ToString()
            => $"Cell {Position}";
    }
}
