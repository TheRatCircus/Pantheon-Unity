// Entity.cs
// Jerome Martina

using Pantheon.Components;
using Pantheon.Utils;
using Pantheon.World;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon
{
    [Serializable]
    public sealed class Entity
    {
        public string Name { get; set; }

        public bool Blocking { get; set; }
        [NonSerialized]
        private GameObject[] gameObjects;
        public GameObject[] GameObjects
        {
            get => gameObjects;
            set => gameObjects = value;
        }

        public Sprite Sprite { get; set; }

        public Dictionary<Type, EntityComponent> Components { get; private set; }
            = new Dictionary<Type, EntityComponent>();

        public Cell Cell
        {
            get => GetComponent<Location>().Cell;
            set => GetComponent<Location>().Cell = value;
        }
        public Level Level
        {
            get => GetComponent<Location>().Level;
            set => GetComponent<Location>().Level = value;
        }

        public Entity(EntityTemplate template)
        {
            Name = template.Name;
            Sprite = template.Sprite;
            foreach (EntityComponent component in template.Components)
                Components.Add(component.GetType(), component);

            Components.Add(typeof(Location), new Location());
        }

        public T GetComponent<T>() where T : EntityComponent
        {
            if (Components.TryGetValue(typeof(T), out EntityComponent ret))
                return (T)ret;
            else
                throw new ArgumentException(
                    $"Component of type {typeof(T)} not found.");
        }

        public bool TryGetComponent<T>(out T ret)
            where T : EntityComponent
        {
            if (!Components.TryGetValue(typeof(T), out EntityComponent c))
            {
                ret = null;
                return false;
            }
            else
            {
                ret = (T)c;
                return true;
            }
        }

        public bool HasComponent<T>() where T : EntityComponent
        {
            return Components.ContainsKey(typeof(T));
        }

        public void Move(Level level, Cell cell)
        {
            Cell prev = Cell;
            if (prev != null)
                prev.Actor = null;
            Level = level;
            Cell = cell;
            if (GameObjects.HasElements())
                GameObjects[0].transform.position = cell.Position.ToVector3();
            cell.Actor = this;
        }
    }
}
